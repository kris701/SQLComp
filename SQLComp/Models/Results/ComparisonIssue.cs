using System;
using System.Collections.Generic;
using System.Text;

namespace SQLComp.Models.Results
{
	public class ComparisonIssue
	{
		public string Reason { get; set; }
		public string PK { get; set; } 
		public string?[] SourceData { get; set; }
		public Dictionary<string, int> SourceColumnMap { get; set; } 
		public string?[]? TargetData { get; set; }
		public Dictionary<string, int>? TargetColumnMap { get; set; }

		public ComparisonIssue(string reason, string pK, string?[] sourceData, Dictionary<string, int> sourceColumnMap, string?[]? targetData, Dictionary<string, int>? targetColumnMap)
		{
			Reason = reason;
			PK = pK;
			SourceData = sourceData;
			SourceColumnMap = sourceColumnMap;
			TargetData = targetData;
			TargetColumnMap = targetColumnMap;
		}
	}
}
