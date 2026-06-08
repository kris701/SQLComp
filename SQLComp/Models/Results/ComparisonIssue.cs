namespace SQLComp.Models.Results
{
	public class ComparisonIssue
	{
		public string PK { get; set; }
		public string?[] SourceData { get; set; }
		public Dictionary<string, int> SourceColumnMap { get; set; }
		public string?[]? TargetData { get; set; }

		public ComparisonIssue(string pK, string?[] sourceData, Dictionary<string, int> sourceColumnMap, string?[]? targetData)
		{
			PK = pK;
			SourceData = sourceData;
			SourceColumnMap = sourceColumnMap;
			TargetData = targetData;
		}
	}
}
