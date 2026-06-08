namespace SQLComp.Models.Checks
{
	public class CompareCheck : ICheck
	{
		public enum CompareTypes
		{
			Equals,
			NotEquals
		}

		public string Source { get; set; }
		public int SourceIndex { get; set; }
		public string Target { get; set; }
		public int TargetIndex { get; set; }

		public void Initialize(List<string> sourceColumnMap, List<string> targetColumnMap)
		{
			SourceIndex = sourceColumnMap.IndexOf(Source);
			TargetIndex = targetColumnMap.IndexOf(Target);
		}

		public bool Check(string?[] sourceData, string?[]? targetData)
		{
			if (targetData == null || TargetIndex == -1 || SourceIndex == -1)
				return false;
			return sourceData[SourceIndex] == targetData[TargetIndex];
		}

		public string GetDescription() => $"The source column '{Source}' must be equal to '{Target}'";
		public List<string> GetSourceColumns() => new List<string>() { Source };
		public List<string> GetTargetColumns() => new List<string>() { Target };
	}
}
