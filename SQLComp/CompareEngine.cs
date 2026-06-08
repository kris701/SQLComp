using Microsoft.Data.SqlClient;
using SQLComp.Helpers;
using SQLComp.Models;
using SQLComp.Models.Results;
using SQLComp.Models.Transformers;
using System.Diagnostics;
using System.Text;

namespace SQLComp
{
	public class CompareEngine : ICompareEngine
	{
		public event OnCompareEngineLogHandler? OnLog;
		public event OnCompareEngineCheckFalseHandler? OnCheckFalse;

		public bool FastCheck { get; set; } = false;
		public uint FetchRetry { get; set; } = 0;

		public async Task<List<ComparisonIssue>> Compare(TableCompareDefinition model)
		{
			var result = new List<ComparisonIssue>();

			var sourceColumns = new List<string>();
			var targetColumns = new List<string>();
			foreach (var check in model.Checks)
			{
				sourceColumns.AddRange(check.GetSourceColumns());
				targetColumns.AddRange(check.GetTargetColumns());
			}

			OnLog?.Invoke("Fetching source data...", LogType.Info);
			var sourceEstimationQuery = BuildEstimationQuery(model.Source);
			var estimatedSourceRows = await GetEstimatedRowCount(sourceEstimationQuery, model.Source.ConnectionString);
			var sourceFetchQuery = BuildFetchQuery(model.Source, sourceColumns);
			var sourceData = await FetchData(sourceFetchQuery, model.Source.ConnectionString, sourceColumns, model.Transformers, model.Source.PkColumn, estimatedSourceRows);
			OnLog?.Invoke($"\tA total of {sourceData.Data.Count} rows to evaluate", LogType.Info);

			OnLog?.Invoke("Fetching target data...", LogType.Info);
			var targetEstimationQuery = BuildEstimationQuery(model.Target);
			var estimatedTargetRows = await GetEstimatedRowCount(targetEstimationQuery, model.Target.ConnectionString);
			var targetFetchQuery = BuildFetchQuery(model.Target, targetColumns);
			var targetData = await FetchData(targetFetchQuery, model.Target.ConnectionString, targetColumns, model.Transformers, model.Target.PkColumn, estimatedTargetRows);
			OnLog?.Invoke($"\tA total of {targetData.Data.Count} rows to evaluate", LogType.Info);

			foreach (var check in model.Checks)
				check.Initialize(sourceData.ColumnMap, targetData.ColumnMap);

			OnLog?.Invoke($"A total of {model.Checks.Count} checks to perform against {sourceData.Data.Count} source rows", LogType.Info);
			OnLog?.Invoke("Comparing data...", LogType.Info);
			var any = false;
			var counter = 0;
			var watch = new Stopwatch();
			watch.Start();
			foreach (var item in sourceData.Data.Keys)
			{
				foreach (var check in model.Checks)
				{
					string?[]? target = null;
					targetData.Data.TryGetValue(item, out target);
					if (!check.Check(sourceData.Data[item], target))
					{
						any = true;
						var issue = new ComparisonIssue(item, sourceData.Data[item], target);
						result.Add(issue);
						OnCheckFalse?.Invoke($"[{item}] " + check.GetDescription());
						break;
					}
				}
				counter++;
				if (watch.ElapsedMilliseconds > 1000)
				{
					OnLog?.Invoke($"\tChecked {counter} out of {sourceData.Data.Count}", LogType.Info);
					watch.Restart();
				}
			}
			watch.Stop();
			OnLog?.Invoke("Comparison complete!", LogType.Info);
			if (any)
				OnLog?.Invoke("\tSome data was not equal!", LogType.Error);
			else
				OnLog?.Invoke("\tAll data correct!", LogType.Success);

			return result;
		}

		private string BuildFetchQuery(DatasourceDefinition def, List<string> columns)
		{
			var sb = new StringBuilder();

			if (!columns.Contains(def.PkColumn))
				columns.Insert(0, def.PkColumn);

			sb.Append("SELECT ");
			if (FastCheck)
				sb.Append("TOP(10) ");
			var counter = 1;
			foreach (var column in columns)
			{
				sb.Append(column);
				if (counter++ < columns.Count)
					sb.Append(",");
			}
			sb.AppendLine($" FROM {def.Table}");
			if (def.Where.Count > 0)
			{
				sb.AppendLine(" WHERE ");
				counter = 1;
				foreach (var where in def.Where)
				{
					sb.Append(where);
					if (counter++ < def.Where.Count)
						sb.Append(" AND ");
				}
			}

			return sb.ToString();
		}

		private string BuildEstimationQuery(DatasourceDefinition def)
		{
			var sb = new StringBuilder();

			sb.Append($"SELECT COUNT(*) FROM {def.Table}");
			if (def.Where.Count > 0)
			{
				sb.AppendLine(" WHERE ");
				var counter = 1;
				foreach (var where in def.Where)
				{
					sb.Append(where);
					if (counter++ < def.Where.Count)
						sb.Append(" AND ");
				}
			}

			return sb.ToString();
		}

		private async Task<uint> GetEstimatedRowCount(string query, string connectionString)
		{
			if (FastCheck)
				return 10;

			uint rows = 0;
			OnLog?.Invoke("\tGetting row estimation...", LogType.Info);
			using (var connection = new SqlConnection(connectionString))
			{
				var command = new SqlCommand(query, connection)
				{
					CommandTimeout = 999999
				};
				connection.Open();
				var reader = await command.ExecuteReaderAsync();
				while (reader.Read())
				{
					var value = reader[0]?.ToString();
					if (value != null)
						rows = uint.Parse(value);
				}
				reader.Close();
			}

			return rows;
		}

		private async Task<DataModel> FetchData(string query, string connectionString, List<string> columns, List<ITransformer> transformers, string pkColumn, uint estimatedRows)
		{
			var returnData = new DataModel();

			foreach (var col in columns)
				returnData.ColumnMap.Add(col);

			var retryCount = 0;
			while (retryCount <= FetchRetry)
			{
				try
				{
					OnLog?.Invoke("\tExecuting query...", LogType.Info);
					uint rows = 0;
					var watch = new Stopwatch();
					watch.Start();
					using (var connection = new SqlConnection(connectionString))
					{
						var command = new SqlCommand(query, connection)
						{
							CommandTimeout = 999999
						};
						connection.Open();
						var reader = await command.ExecuteReaderAsync();
						while (reader.Read())
						{
							string? pkValue = null;
							var newRow = new string?[columns.Count];
							for (int i = 0; i < reader.FieldCount; i++)
							{
								var name = reader.GetName(i);
								var data = reader[i];
								string? dataStr = null;
								if (data != null)
									dataStr = data.ToString();
								foreach (var transformer in transformers)
									dataStr = transformer.Transform(dataStr);

								if (name == pkColumn)
									pkValue = dataStr;
								else
									newRow[i] = dataStr;
							}
							if (pkValue != null)
								returnData.Data.Add(pkValue, newRow);
							rows++;
							if (watch.ElapsedMilliseconds > 1000)
							{
								OnLog?.Invoke($"\t\tFetched {rows} out of {estimatedRows} rows ({PercentageHelpers.GetPercentage(rows, estimatedRows)})", LogType.Info);
								watch.Restart();
							}
						}
						reader.Close();
					}

					return returnData;
				}
				catch (Exception ex)
				{
					OnLog?.Invoke("\tError during data fetching: " + ex.Message, LogType.Error);
					if (retryCount + 1 <= FetchRetry)
					{
						OnLog?.Invoke($"\tRetrying in 30 seconds ({retryCount + 1} out of {FetchRetry} retries)", LogType.Error);
						await Task.Delay(TimeSpan.FromSeconds(30));
					}
				}
				retryCount++;
			}
			throw new Exception("Could not fetch the data within the retry times!");
		}
	}
}
