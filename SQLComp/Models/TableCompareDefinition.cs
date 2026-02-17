using SQLComp.Models.Checks;
using SQLComp.Models.Transformers;
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
		public List<ITransformer> Transformers { get; set; }
	}
}
