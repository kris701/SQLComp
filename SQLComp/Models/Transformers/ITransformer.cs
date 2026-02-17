using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SQLComp.Models.Transformers
{
	[JsonDerivedType(typeof(ReplaceTransformer), typeDiscriminator: nameof(ReplaceTransformer))]
	public interface ITransformer
	{
		public string? Transform(string? item);
	}
}
