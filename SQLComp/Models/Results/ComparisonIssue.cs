namespace SQLComp.Models.Results
{
	public class ComparisonIssue
	{
		public string PK { get; set; }
		public string?[] SourceData { get; set; }
		public string?[]? TargetData { get; set; }

		public ComparisonIssue(string pK, string?[] sourceData, string?[]? targetData)
		{
			PK = pK;
			SourceData = sourceData;
			TargetData = targetData;
		}
	}
}
