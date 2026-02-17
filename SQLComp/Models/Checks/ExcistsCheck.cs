using System;
using System.Collections.Generic;
using System.Text;

namespace SQLComp.Models.Checks
{
	public class ExcistsCheck : ICheck
	{
		public bool Check(Dictionary<string, string?> sourceData, Dictionary<string, string?>? targetData) => targetData != null;
		public string GetDescription() => "The target row does not exist";
		public List<string> GetSourceColumns() => new List<string>();
		public List<string> GetTargetColumns() => new List<string>();
	}
}
