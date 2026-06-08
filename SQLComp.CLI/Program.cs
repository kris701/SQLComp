using CommandLine;
using CommandLine.Text;
using SQLComp;
using SQLComp.CLI;
using SQLComp.Models;
using SQLComp.Models.Checks;
using SQLComp.Models.Transformers;
using System.Text.Json;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

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

		var queryTransformers = new List<ITransformer>();
		foreach (var replaceRegex in opts.PatchRegexes)
		{
			var split = replaceRegex.Split(";;;");
			queryTransformers.Add(new RegexReplaceTransformer() { Match = split[0], Substitution = split[1] });
		}

		if (opts.ForceRemovePatchFile && File.Exists(opts.OutputPath))
			File.Delete(opts.OutputPath);

		if (File.Exists(opts.OutputPath))
			File.AppendAllText(opts.OutputPath, $"-- Run '{opts.TargetPath}'" + Environment.NewLine);
		else
			File.WriteAllText(opts.OutputPath, $"-- Run '{opts.TargetPath}'" + Environment.NewLine);

		WriteLineColor("Starting compare check on " + opts.TargetPath, LogType.Info);

		var engine = new CompareEngine() { 
			FastCheck = opts.DoCheck,
			FetchRetry = opts.RetryTimes
		};
		engine.OnLog += (l, t) => WriteLineColor(l, t);
		var falseCounter = 0;
		engine.OnCheckFalse += (r) =>
		{
			if (falseCounter < 1000 || (falseCounter % 1000 == 0))
				WriteLineColor($"\t{r}", LogType.Warning);
			if (falseCounter == 1000)
				WriteLineColor($"\tToo many check fails! Only printing every 1000 from now on...", LogType.Warning);
			falseCounter++;
		};
		var result = await engine.Compare(def);
		WriteLineColor("Comparison complete!", LogType.Info);
		WriteLineColor("Writing to patch file...", LogType.Info);
		var builder = new PatchBuilder() { Transformers = queryTransformers };
		builder.Build(opts.OutputPath, result, def);
		WriteLineColor("Patch file complete!", LogType.Info);
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