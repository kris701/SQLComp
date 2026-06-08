namespace SQLComp.Models.Checks
{
	public class ExcistsCheck : ICheck
	{
		public void Initialize(List<string> sourceColumnMap, List<string>? targetColumnMap) { }
		public bool Check(string?[] sourceData, string?[]? targetData) => targetData != null;
		public string GetDescription() => "The target row does not exist";
		public List<string> GetSourceColumns() => new List<string>();
		public List<string> GetTargetColumns() => new List<string>();
	}
}
