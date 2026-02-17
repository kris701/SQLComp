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
	}
}
