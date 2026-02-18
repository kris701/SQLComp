using System;
using System.Collections.Generic;
using System.Text;

namespace SQLComp.Models.Results
{
	public class ComparisonResult
	{
		public int TotalIssues { get => Issues.Count; }
		public List<ComparisonIssue> Issues { get; set; } = new List<ComparisonIssue>();
	}
}
