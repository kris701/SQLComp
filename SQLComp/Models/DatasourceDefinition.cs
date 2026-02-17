using System;
using System.Collections.Generic;
using System.Text;

namespace SQLComp.Models
{
	public class DatasourceDefinition
	{
		public string ConnectionString { get; set; }
		public string Table { get; set; }
		public string PkColumn { get; set; }
		public List<string> Where { get; set; }
	}
}
