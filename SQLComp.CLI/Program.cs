using SQLComp;
using SQLComp.Models;
using System.Text.Json;

internal class Program
{
	private static async Task Main(string[] args)
	{
		var def = JsonSerializer.Deserialize<TableCompareDefinition>(File.ReadAllText("test.json"));
		if (def == null)
			throw new Exception("Cannot parse!");
		var engine = new CompareEngine();
		engine.OnLog += (l) => Console.WriteLine(l);
		engine.OnCheckFalse += (l, s, t) =>
		{
			Console.WriteLine(l);
		};
		await engine.Compare(def);
	}
}