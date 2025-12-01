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
			Commands.Test.GetCommand(),
			Commands.Options.Year,
			Commands.Options.Day,
			Commands.Options.InputName,
		};

		command.SetAction(async parseResult => {
			var year      = parseResult.GetValue(Commands.Options.Year);
			var day       = parseResult.GetValue(Commands.Options.Day);
			var inputName = parseResult.GetValue(Commands.Options.InputName) ?? throw new InvalidOperationException("No input name received from option.");
			var problem   = Problems.GetProblems(year)[day];

			Console.WriteLine($"{ANSI_GREEN}Running year {problem.Year} day {problem.Day:d2}{ANSI_RESET}");

			var input = await LoadInput(problem, inputName);

			Console.WriteLine(problem.Main(input));
			Console.WriteLine();
		});

		return await command.Parse(args).InvokeAsync();
	}

	public static async Task<string[]> LoadInput(ProblemMetadata problem, string inputName)
	{
		var inputFilename = Path.Combine(problem.Path, inputName);

		if (!File.Exists(inputFilename)) {
			await DownloadInput(problem, inputFilename);
		}

		return File.ReadAllLines(inputFilename);
	}

	private static async Task DownloadInput(ProblemMetadata problem, string fileName)
	{
		if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("AOC_SESSION"))) {
			throw new InvalidOperationException("Set the AOC_SESSION environment variable.");
		}

		var cc = new CookieContainer();

		cc.Add(new Cookie("session", Environment.GetEnvironmentVariable("AOC_SESSION"), "/", "adventofcode.com"));

		using var handler = new HttpClientHandler() { CookieContainer = cc };
		using var hc      = new HttpClient(handler);

		File.WriteAllText(fileName, (await hc.GetStringAsync($"https://adventofcode.com/{problem.Year}/day/{problem.Day}/input")).TrimEnd('\n'));
	}
}
