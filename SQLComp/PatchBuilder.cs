using SQLComp.Helpers;
using SQLComp.Models;
using SQLComp.Models.Checks;
using SQLComp.Models.Results;
using SQLComp.Models.Transformers;
using System.Text;
using static SQLComp.IPatchBuilder;

namespace SQLComp
{
	public class PatchBuilder : IPatchBuilder
	{
		public event OnPatchBuilderLogHandler? OnLog;

		public List<ITransformer> Transformers { get; set; } = new List<ITransformer>();

		public void Build(string patchFile, List<ComparisonIssue> result, TableCompareDefinition def)
		{
			uint completed = 0;

			var sb = new StringBuilder();

			foreach (var issue in result)
			{
				if (issue.TargetData == null)
				{
					var targetColumns = new List<string>();
					targetColumns.Add(def.Target.PkColumn);
					var targetValues = new List<string>();
					targetValues.Add($"'{issue.PK}'");
					foreach (var check in def.Checks)
					{
						if (check is CompareCheck comp)
						{
							targetColumns.Add(comp.Target);
							var value = issue.SourceData[comp.SourceIndex];
							if (value == null)
								targetValues.Add("NULL");
							else
								targetValues.Add($"'{value}'");
						}
					}
					var text = $"INSERT INTO {def.Target.Table} ({string.Join(',', targetColumns)}) VALUES ({string.Join(',', targetValues)})";
					foreach (var transformer in Transformers)
						text = transformer.Transform(text);
					sb.AppendLine(text);
				}
				else
				{
					var targetValues = new List<string>();
					foreach (var check in def.Checks)
					{
						if (check is CompareCheck comp && !comp.Check(issue.SourceData, issue.TargetData))
						{
							var value = issue.SourceData[comp.SourceIndex];
							if (value == null)
								value = "NULL";
							else
								value = $"'{value}'";
							targetValues.Add($"{comp.Target} = {value}");
						}
					}
					var text = $"UPDATE {def.Target.Table} SET {string.Join(',', targetValues)} WHERE {def.Target.PkColumn} = '{issue.PK}'";
					foreach (var transformer in Transformers)
						text = transformer.Transform(text);
					sb.AppendLine(text);
				}
				completed++;

				if (completed % 100000 == 0)
				{
					File.AppendAllText(patchFile, sb.ToString());
					OnLog?.Invoke($"\tWrote {completed} out of {result.Count} ({PercentageHelpers.GetPercentage(completed, result.Count)})", LogType.Info);
					sb = new StringBuilder();
				}
			}

			if (sb.Length > 0)
				File.AppendAllText(patchFile, sb.ToString());
		}
	}
}
