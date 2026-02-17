using System;
using System.Collections.Generic;
using System.Text;

namespace SQLComp.Models
{
	public class DataModel
	{
		public Dictionary<string, int> ColumnMap { get; set; } = new Dictionary<string, int>();
		public Dictionary<string, string?[]> Data { get; set; } = new Dictionary<string,string?[]>();
	}
}
