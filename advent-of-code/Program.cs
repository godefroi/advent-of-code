using System.CommandLine;
using System.Net;

using static AdventOfCode.AnsiCodes;

namespace AdventOfCode;

public static class Program
{
	public static async Task<int> Main(string[] args)
	{
		var command = new RootCommand("Advent of Code runner") {
			Commands.List.GetCommand(),
			Commands.Time.GetCommand(),
			Commands.Benchmarks.GetCommand(),
			Commands.Options.Year,
			Commands.Options.Day,
			Commands.Options.InputName,
		};

		command.SetHandler(async (int year, int day, string inputName) => {
			var problem = Problems.GetProblems(year)[day];

			Console.WriteLine($"{ANSI_GREEN}Running year {problem.Year} day {problem.Day:d2}{ANSI_RESET}");

			var input = await LoadInput(problem, inputName);

			Console.WriteLine(problem.Main(input));
			Console.WriteLine();
		}, Commands.Options.Year, Commands.Options.Day, Commands.Options.InputName);

		return await command.InvokeAsync(args);
	}

	public static async Task<string[]> LoadInput(ProblemMetadata2 problem, string inputName)
	{
		var inputFilename = Path.Combine(problem.Path, inputName);

		if (!File.Exists(inputFilename)) {
			await DownloadInput(problem.Day, inputFilename);
		}

		return File.ReadAllLines(inputFilename);
	}

	private static async Task DownloadInput(int day, string fileName)
	{
		if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("AOC_SESSION"))) {
			throw new InvalidOperationException("Set the AOC_SESSION environment variable.");
		}

		//var assemblyName = System.Reflection.Assembly.GetEntryAssembly()!.GetName().Name;
		// var yearMatch    = System.Text.RegularExpressions.Regex.Match(assemblyName!, "\\d{4}");

		// if (!yearMatch.Success) {
		// 	throw new InvalidOperationException($"Unable to parse year from assembly name {assemblyName}");
		// }

		Console.WriteLine($"{ANSI_GREEN}Retrieving input file and saving to {fileName}{ANSI_RESET}");
		Console.WriteLine($"{ANSI_RED}WARNING- YEAR IS HARD-CODED TO 2023; ONLY 2023 INPUT WILL BE DOWNLOADED!{ANSI_RESET}"); // ProblemMetadata should have year in it
		var yearMatch = (Value: 2023, Dummy: 2023);

		var cc = new CookieContainer();

		cc.Add(new Cookie("session", Environment.GetEnvironmentVariable("AOC_SESSION"), "/", "adventofcode.com"));

		using var handler = new HttpClientHandler() { CookieContainer = cc };
		using var hc      = new HttpClient(handler);

		//var inputLines = (await hc.GetStringAsync($"https://adventofcode.com/{yearMatch.Value}/day/{day}/input")).Split('\n');

		//File.WriteAllText(fileName, string.Join("\n", inputLines.Take(inputLines.Length - 1)));

		File.WriteAllText(fileName, (await hc.GetStringAsync($"https://adventofcode.com/{yearMatch.Value}/day/{day}/input")).TrimEnd('\n'));
	}
}
