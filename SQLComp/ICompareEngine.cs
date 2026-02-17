using SQLComp.Models;

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
	public delegate void OnCompareEngineCheckFalseHandler(string logTxt, string pk, Dictionary<string, string?> source, Dictionary<string, string?>? target);

	public interface ICompareEngine
	{
		public event OnCompareEngineLogHandler? OnLog;
		public event OnCompareEngineCheckFalseHandler? OnCheckFalse;

		public Task Compare(TableCompareDefinition model);
	}
}
