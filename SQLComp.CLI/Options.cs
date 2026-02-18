using CommandLine;

namespace SQLComp.CLI
{
	public class Options
	{
		[Option('t', "target", Required = true, HelpText = "Target comparison file", Default = "")]
		public string TargetPath { get; set; } = "";
		[Option('o', "output", Required = false, HelpText = "Output patching file", Default = "patch.sql")]
		public string OutputPath { get; set; } = "";
		[Option('f', "force", Required = false, HelpText = "Force remove existing patch file", Default = false)]
		public bool ForceRemovePatchFile { get; set; } = false;
		[Option('c', "check", Required = false, HelpText = "Select only the top 10 rows (this is just for testing you made the correct syntax)", Default = false)]
		public bool DoCheck { get; set; } = false;
		[Option('p', "patchreg", Required = false, HelpText = "Set of patch regex replacements to execute (MATCH;;;REPLACE)", Default = false)]
		public IEnumerable<string> PatchRegexes { get; set; } = new List<string>();
		[Option('r', "retry", Required = false, HelpText = "How many times the system should retry to get data if something fails", Default = 0)]
		public uint RetryTimes { get; set; } = 0;
	}
}
