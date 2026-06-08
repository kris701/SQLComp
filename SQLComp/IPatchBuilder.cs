using SQLComp.Models;
using SQLComp.Models.Results;
using SQLComp.Models.Transformers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SQLComp
{
	public interface IPatchBuilder
	{
		public List<ITransformer> Transformers { get; set; }
		public void Build(string patchFile, ComparisonResult result, TableCompareDefinition def);
	}
}
