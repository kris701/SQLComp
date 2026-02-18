using System.Text.RegularExpressions;

namespace SQLComp.Models.Transformers
{
	public class RegexReplaceTransformer : ITransformer
	{
		public string Match { get; set; }
		public string Substitution { get; set; }

		public string? Transform(string? item)
		{
			if (item == null)
				return null;
			return Regex.Replace(item, Match, Substitution);
		}
	}
}
