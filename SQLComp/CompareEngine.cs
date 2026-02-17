using Microsoft.Data.SqlClient;
using SQLComp.Models;
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

		public async Task Compare(TableCompareDefinition model)
		{
			var sourceColumns = new List<string>();
			var targetColumns = new List<string>();
			foreach (var check in model.Checks)
			{
				sourceColumns.AddRange(check.GetSourceColumns());
				targetColumns.AddRange(check.GetTargetColumns());
			}

			OnLog?.Invoke("Fetching source data...", LogType.Info);
			var sourceData = await ExecuteSQL(BuildQuery(model.Source, sourceColumns), model.Source.ConnectionString, model.Transformers);
			OnLog?.Invoke($"\tA total of {sourceData.Count} rows to evaluate", LogType.Info);
			OnLog?.Invoke("Fetching target data...", LogType.Info);
			var targetData = await ExecuteSQL(BuildQuery(model.Target, targetColumns), model.Target.ConnectionString, model.Transformers);
			OnLog?.Invoke($"\tA total of {targetData.Count} rows to evaluate", LogType.Info);

			OnLog?.Invoke($"A total of {model.Checks.Count} checks to perform against {sourceData.Count} source rows", LogType.Info);
			OnLog?.Invoke("Comparing data...", LogType.Info);
			var any = false;
			var counter = 0;
			var watch = new Stopwatch();
			watch.Start();
			foreach (var item in sourceData.Keys)
			{
				foreach (var check in model.Checks)
				{
					Dictionary<string, string?>? target = null;
					targetData.TryGetValue(item, out target);
					if (!check.Check(sourceData[item], target))
					{
						any = true;
						OnCheckFalse?.Invoke($"[S:{item}]" + check.GetDescription(), item, sourceData[item], target);
						break;
					}
				}
				counter++;
				if (watch.ElapsedMilliseconds > 1000)
				{
					OnLog?.Invoke($"\tChecked {counter} out of {sourceData.Count}", LogType.Info);
					watch.Restart();
				}
			}
			watch.Stop();
			OnLog?.Invoke("Comparison complete!", LogType.Info);
			if (any)
				OnLog?.Invoke("Some data was not equal!", LogType.Error);
			else
				OnLog?.Invoke("All data correct!", LogType.Success);
		}

		private string BuildQuery(DatasourceDefinition def, List<string> columns)
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

		private async Task<Dictionary<string, Dictionary<string, string?>>> ExecuteSQL(string query, string connectionString, List<ITransformer> transformers)
		{
			var returnData = new Dictionary<string, Dictionary<string, string?>>();

			OnLog?.Invoke("\tExecuting query...", LogType.Info);
			using (var connection = new SqlConnection(connectionString))
			{
				var command = new SqlCommand(query, connection)
				{
					CommandTimeout = 999999
				};
				connection.Open();
				var reader = await command.ExecuteReaderAsync();
				OnLog?.Invoke("\tParsing result...", LogType.Info);
				while (reader.Read())
				{
					var pkColumn = reader[0]?.ToString();
					if (pkColumn != null && !returnData.ContainsKey(pkColumn))
					{
						returnData.Add(pkColumn, new Dictionary<string, string?>());
						for (int i = 1; i < reader.FieldCount; i++)
						{
							var name = reader.GetName(i);
							var data = reader[i]?.ToString();
							foreach (var transformer in transformers)
								data = transformer.Transform(data);

							returnData[pkColumn].Add(name, data);
						}
					}
				}
				reader.Close();
			}

			return returnData;
		}
	}
}
