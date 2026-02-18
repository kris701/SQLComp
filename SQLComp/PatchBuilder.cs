using SQLComp.Models;
using SQLComp.Models.Checks;
using SQLComp.Models.Results;
using SQLComp.Models.Transformers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SQLComp
{
	public class PatchBuilder : IPatchBuilder
	{
		public List<ITransformer> Transformers { get; set; } = new List<ITransformer>();

		public string Build(ComparisonResult result, TableCompareDefinition def)
		{
			var sb = new StringBuilder();

			foreach(var issue in result.Issues)
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
							var value = issue.SourceData[issue.SourceColumnMap[comp.Source]];
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
							var value = issue.SourceData[issue.SourceColumnMap[comp.Source]];
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
			}

			return sb.ToString();
		}
	}
}
