using System.Text.Json.Serialization;

namespace SQLComp.Models.Checks
{
	[JsonDerivedType(typeof(CompareCheck), typeDiscriminator: nameof(CompareCheck))]
	[JsonDerivedType(typeof(ExcistsCheck), typeDiscriminator: nameof(ExcistsCheck))]
	public interface ICheck
	{
		public void Initialize(List<string> sourceColumnMap, List<string> targetColumnMap);
		public bool Check(string?[] sourceData, string?[]? targetData);
		public string GetDescription();
		public List<string> GetSourceColumns();
		public List<string> GetTargetColumns();
	}
}
