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

		private int _sourceIndex = -1;
		private int _targetIndex = -1;

		public void Initialize(Dictionary<string, int> sourceColumnMap, Dictionary<string, int>? targetColumnMap) 
		{
			sourceColumnMap.TryGetValue(Source, out _sourceIndex);
			targetColumnMap?.TryGetValue(Source, out _targetIndex);
		}

		public bool Check(string?[] sourceData, string?[]? targetData)
		{
			if (targetData == null || _targetIndex == -1 || _sourceIndex == -1)
				return false;
			return sourceData[_sourceIndex] == targetData[_targetIndex];
		}

		public string GetDescription() => $"The source column '{Source}' must be equal to '{Target}'";
		public List<string> GetSourceColumns() => new List<string>() { Source };
		public List<string> GetTargetColumns() => new List<string>() { Target };
	}
}
