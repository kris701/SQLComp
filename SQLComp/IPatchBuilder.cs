using SQLComp.Models;
using SQLComp.Models.Results;
using SQLComp.Models.Transformers;

namespace SQLComp
{
	public interface IPatchBuilder
	{
		public delegate void OnPatchBuilderLogHandler(string logTxt, LogType type);

		public List<ITransformer> Transformers { get; set; }
		public void Build(string patchFile, List<ComparisonIssue> result, TableCompareDefinition def);
	}
}
