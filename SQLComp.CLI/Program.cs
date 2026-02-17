using CommandLine;
using CommandLine.Text;
using SQLComp;
using SQLComp.CLI;
using SQLComp.Models;
using SQLComp.Models.Checks;
using System.Text.Json;

internal class Program
{
	static async Task Main(string[] args)
	{
		var parser = new Parser(with => with.HelpWriter = null);
		var parserResult = parser.ParseArguments<Options>(args);
		parserResult.WithNotParsed(errs => DisplayHelp(parserResult, errs));
		await parserResult.WithParsedAsync(Run);
	}

	public static async Task Run(Options opts)
	{
		var def = JsonSerializer.Deserialize<TableCompareDefinition>(File.ReadAllText(opts.TargetPath));
		if (def == null)
			throw new Exception("Cannot parse comparison file!");

		if (opts.ForceRemovePatchFile && File.Exists(opts.OutputPath))
			File.Delete(opts.OutputPath);

		if (File.Exists(opts.OutputPath))
			File.AppendAllText(opts.OutputPath, $"-- Run '{opts.TargetPath}'" + Environment.NewLine);
		else
			File.WriteAllText(opts.OutputPath, $"-- Run '{opts.TargetPath}'" + Environment.NewLine);

		WriteLineColor("Starting compare check on " + opts.TargetPath, LogType.Info);

		var engine = new CompareEngine();
		engine.FastCheck = opts.DoCheck;
		engine.OnLog += (l, t) => WriteLineColor(l, t);
		engine.OnCheckFalse += (l, pk, s, t) =>
		{
			WriteLineColor(l, LogType.Warning);
			if (t == null)
			{
				var targetColumns = new List<string>();
				targetColumns.Add(def.Target.PkColumn);
				var targetValues = new List<string>();
				targetValues.Add(pk);
				foreach (var check in def.Checks)
				{
					if (check is CompareCheck comp)
					{
						targetColumns.Add(comp.Target);
						var value = s[comp.Source];
						if (value == null)
							targetValues.Add("NULL");
						else
							targetValues.Add($"'{value}'");
					}
				}
				File.AppendAllText(opts.OutputPath, $"INSERT INTO {def.Target.Table} ({string.Join(',', targetColumns)}) VALUES ({string.Join(',', targetValues)})" + Environment.NewLine);
			}
			else
			{
				var targetValues = new List<string>();
				foreach (var check in def.Checks)
				{
					if (check is CompareCheck comp && !comp.Check(s, t))
					{
						var value = s[comp.Source];
						if (value == null)
							value = "NULL";
						else
							value = $"'{value}'";
						targetValues.Add($"{comp.Target} = {value}");
					}
				}
				File.AppendAllText(opts.OutputPath, $"UPDATE {def.Target.Table} SET {string.Join(',', targetValues)} WHERE {def.Target.PkColumn} = '{pk}'" + Environment.NewLine);
			}
		};
		await engine.Compare(def);
		WriteLineColor("Comparison complete!", LogType.Info);
	}

	private static void WriteLineColor(string log, LogType type)
	{
		switch (type)
		{
			case LogType.Info:
				Console.ForegroundColor = ConsoleColor.Blue;
				break;
			case LogType.Success:
				Console.ForegroundColor = ConsoleColor.Green;
				break;
			case LogType.Warning:
				Console.ForegroundColor = ConsoleColor.Yellow;
				break;
			case LogType.Error:
				Console.ForegroundColor = ConsoleColor.Red;
				break;
		}
		Console.WriteLine(log);
		Console.ResetColor();
	}

	private static void DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errs)
	{
		var helpText = HelpText.AutoBuild(result, h =>
		{
			h.AddEnumValuesToHelpText = true;
			return h;
		}, e => e, verbsIndex: true);
		Console.WriteLine(helpText);
		HandleParseError(errs);
	}

	private static void HandleParseError(IEnumerable<Error> errs)
	{
		var sentenceBuilder = SentenceBuilder.Create();
		foreach (var error in errs)
			if (error is not HelpRequestedError)
				Console.WriteLine(sentenceBuilder.FormatError(error));
	}
}