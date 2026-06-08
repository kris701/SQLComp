using SQLComp.Models;
using SQLComp.Models.Results;

namespace SQLComp
{
	public enum LogType
	{
		Info,
		Success,
		Warning,
		Error
	}

	public delegate void OnCompareEngineLogHandler(string logTxt, LogType type);
	public delegate void OnCompareEngineCheckFalseHandler(string reason);

	public interface ICompareEngine
	{
		public event OnCompareEngineLogHandler? OnLog;
		public event OnCompareEngineCheckFalseHandler? OnCheckFalse;

		public bool FastCheck { get; set; }
		public uint FetchRetry { get; set; }

		public Task<ComparisonResult> Compare(TableCompareDefinition model);
	}
}
