namespace SQLComp.Models.Checks
{
	public class ExcistsCheck : ICheck
	{
		public bool Check(string?[] sourceData, Dictionary<string, int> sourceColumnMap, string?[]? targetData, Dictionary<string, int>? targetColumnMap) => targetData != null;
		public string GetDescription() => "The target row does not exist";
		public List<string> GetSourceColumns() => new List<string>();
		public List<string> GetTargetColumns() => new List<string>();
	}
}
