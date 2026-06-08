namespace SQLComp.Models
{
	public class DataModel
	{
		public Dictionary<string, int> ColumnMap { get; set; } = new Dictionary<string, int>();
		public Dictionary<string, string?[]> Data { get; set; } = new Dictionary<string, string?[]>();
	}
}
