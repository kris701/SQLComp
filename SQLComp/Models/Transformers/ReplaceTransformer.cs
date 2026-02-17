namespace SQLComp.Models.Transformers
{
	public class ReplaceTransformer : ITransformer
	{
		public string Replace { get; set; }
		public string With { get; set; }

		public string? Transform(string? item) => item?.Replace(Replace, With);
	}
}
