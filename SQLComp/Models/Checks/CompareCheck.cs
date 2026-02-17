using System;
using System.Collections.Generic;
using System.Text;

namespace SQLComp.Models.Checks
{
	public class CompareCheck : ICheck
	{
		public enum CompareTypes
		{
			Equals,
			NotEquals
		}

		public string Source { get; set; }
		public string Target { get; set; }
		public CompareTypes Comparison { get; set; }

		public bool Check(Dictionary<string, string?> sourceData, Dictionary<string, string?>? targetData)
		{
			if (targetData == null)
				return false;
			string? source = null;
			sourceData.TryGetValue(Source, out source);
			string? target = null;
			targetData.TryGetValue(Target, out target);

			switch (Comparison)
			{
				case CompareTypes.Equals:
					return source == target;
				case CompareTypes.NotEquals:
					return source != target;

				default:
					throw new Exception("Unknown comparison type!");
			}
		}

		public string GetDescription() => $"The source column '{Source}' must be '{Enum.GetName(Comparison)}' to '{Target}'";
		public List<string> GetSourceColumns() => new List<string>() { Source };
		public List<string> GetTargetColumns() => new List<string>() { Target };
	}
}
