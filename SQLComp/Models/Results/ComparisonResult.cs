namespace SQLComp.Models.Results
{
	public class ComparisonResult
	{
		public int TotalIssues { get => Issues.Count; }
		public List<ComparisonIssue> Issues { get; set; } = new List<ComparisonIssue>();
	}
}
