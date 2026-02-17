using SQLComp.Models.Checks;
using System;
using System.Collections.Generic;
using System.Text;

namespace SQLComp.Models
{
	public class TableCompareDefinition
	{
		public DatasourceDefinition Source { get; set; }
		public DatasourceDefinition Target { get; set; }
		public List<ICheck> Checks { get; set; }
	}
}
