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
	public delegate void OnCompareEngineCheckFalseHandler(string logTxt, string pk, string?[] sourceData, Dictionary<string, int> sourceColumnMap, string?[]? targetData, Dictionary<string, int>? targetColumnMap);

	public interface ICompareEngine
	{
		public event OnCompareEngineLogHandler? OnLog;
		public event OnCompareEngineCheckFalseHandler? OnCheckFalse;

		public bool FastCheck { get; set; }

		public Task Compare(TableCompareDefinition model);
	}
}
