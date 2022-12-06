using System.Net.Http;
using System.Net;
using System.Threading.Tasks;

using static AdventOfCode.AnsiCodes;

namespace AdventOfCode;

public class ProblemRunner
{
	public static async Task Execute(string[] args, ProblemMetadata[]? problemMetadata)
	{
		IEnumerable<ProblemMetadata> problems;

		if (problemMetadata == null || problemMetadata.Length == 0) {
			Console.Error.WriteLine($"{ANSI_RED}No problem classes found; something went wrong with the generator, maybe?{ANSI_RESET}");
			return;
		}

		if (args == null || args.Length == 0) {
			problems = problemMetadata.OrderByDescending(p => p.Day).Take(1);
		} else if (args.Length == 1 && "all".Equals(args[0], StringComparison.OrdinalIgnoreCase)) {
			problems = problemMetadata.OrderBy(p => p.Day);
		} else if (int.TryParse(args[0], out var day)) {
			problems = problemMetadata.Where(p => p.Day == day);
		} else if (args.Length == 1 && "list".Equals(args[0], StringComparison.OrdinalIgnoreCase)) {
			foreach (var pm in problemMetadata) {
				Console.WriteLine($"{pm.Day} in {pm.Path}");
			}

			return;
		} else {
			Console.Error.WriteLine($"{ANSI_RED}Invalid arguments; try specifying a day number, or 'all'.{ANSI_RESET}");
			return;
		}

		foreach (var p in problems) {
			await RunProblem(p);
		}
	}

	private static async Task RunProblem(ProblemMetadata problem)
	{
		var inputFilename = Path.Combine(problem.Path, "input.txt");

		if (!File.Exists(inputFilename)) {
			await DownloadInput(problem.Day, inputFilename);
		}

		Console.WriteLine($"{ANSI_GREEN}Running day {problem.Day:d2}{ANSI_RESET}");

		problem.Main("input.txt");

		Console.WriteLine();
	}

	private static async Task DownloadInput(int day, string fileName)
	{
		if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("AOC_SESSION"))) {
			throw new InvalidOperationException("Set the AOC_SESSION environment variable.");
		}

		Console.WriteLine($"{ANSI_GREEN}Retrieving input file and saving to {fileName}{ANSI_RESET}");

		var cc = new CookieContainer();

		cc.Add(new Cookie("session", Environment.GetEnvironmentVariable("AOC_SESSION"), "/", "adventofcode.com"));

		using var handler = new HttpClientHandler() { CookieContainer = cc };
		using var hc      = new HttpClient(handler);

		//var inputLines = (await hc.GetStringAsync($"https://adventofcode.com/2022/day/{day}/input")).Split('\n');

		//File.WriteAllText(fileName, string.Join("\n", inputLines.Take(inputLines.Length - 1)));

		File.WriteAllText(fileName, (await hc.GetStringAsync($"https://adventofcode.com/2022/day/{day}/input")).TrimEnd('\n'));
	}
}
