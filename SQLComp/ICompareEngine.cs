using SQLComp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SQLComp
{
	public delegate void OnCompareEngineLogHandler(string logTxt);
	public delegate void OnCompareEngineCheckFalseHandler(string logTxt, Dictionary<string, string?> source, Dictionary<string, string?>? target);

	public interface ICompareEngine
	{
		public event OnCompareEngineLogHandler? OnLog;
		public event OnCompareEngineCheckFalseHandler? OnCheckFalse;

		public Task Compare(TableCompareDefinition model);
	}
}
