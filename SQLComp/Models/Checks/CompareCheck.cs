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
		public string Target { get; set; }

		public bool Check(string?[] sourceData, Dictionary<string, int> sourceColumnMap, string?[]? targetData, Dictionary<string, int>? targetColumnMap)
		{
			if (targetData == null || targetColumnMap == null)
				return false;
			var sourceIndex = -1;
			sourceColumnMap.TryGetValue(Source, out sourceIndex);
			var targetIndex = -1;
			targetColumnMap.TryGetValue(Source, out targetIndex);
			if (sourceIndex == -1 || targetIndex == -1)
				return false;
			return sourceData[sourceIndex] == targetData[targetIndex];
		}

		public string GetDescription() => $"The source column '{Source}' must be equal to '{Target}'";
		public List<string> GetSourceColumns() => new List<string>() { Source };
		public List<string> GetTargetColumns() => new List<string>() { Target };
	}
}
