using System.Text.Json.Serialization;

namespace SQLComp.Models.Checks
{
	[JsonDerivedType(typeof(CompareCheck), typeDiscriminator: nameof(CompareCheck))]
	[JsonDerivedType(typeof(ExcistsCheck), typeDiscriminator: nameof(ExcistsCheck))]
	public interface ICheck
	{
		public bool Check(string?[] sourceData, Dictionary<string, int> sourceColumnMap, string?[]? targetData, Dictionary<string, int>? targetColumnMap);
		public string GetDescription();
		public List<string> GetSourceColumns();
		public List<string> GetTargetColumns();
	}
}
