using System.Text.Json.Serialization;

namespace SQLComp.Models.Transformers
{
	[JsonDerivedType(typeof(ReplaceTransformer), typeDiscriminator: nameof(ReplaceTransformer))]
	[JsonDerivedType(typeof(RegexReplaceTransformer), typeDiscriminator: nameof(RegexReplaceTransformer))]
	public interface ITransformer
	{
		public string? Transform(string? item);
	}
}
