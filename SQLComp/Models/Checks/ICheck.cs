using System.Text.Json.Serialization;

namespace SQLComp.Models.Checks
{
	[JsonDerivedType(typeof(CompareCheck), typeDiscriminator: nameof(CompareCheck))]
	[JsonDerivedType(typeof(ExcistsCheck), typeDiscriminator: nameof(ExcistsCheck))]
	public interface ICheck
	{
		public bool Check(Dictionary<string, string?> sourceData, Dictionary<string, string?>? targetData);
		public string GetDescription();
		public List<string> GetSourceColumns();
		public List<string> GetTargetColumns();
	}
}
