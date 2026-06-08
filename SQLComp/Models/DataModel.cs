namespace SQLComp.Models
{
	public class DataModel
	{
		public List<string> ColumnMap { get; set; } = new List<string>();
		public Dictionary<string, string?[]> Data { get; set; } = new Dictionary<string, string?[]>();
	}
}
