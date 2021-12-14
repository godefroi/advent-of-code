using System.Net;
using System.Text.RegularExpressions;
using System.Reflection;

namespace aoc_tools;

public class PuzzleInput
{
	private static readonly Regex _dayPattern = new Regex(@"day_(\d+)");

	private static int Day => int.Parse(_dayPattern.Match(Assembly.GetEntryAssembly().GetName().Name).Groups[1].Value);

	private static string StartupDirectory => Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

	public static async Task<string> GetInput(string fileName = "input.txt")
	{
		var dir = StartupDirectory;

		while (true) {
			if (dir.EndsWith("bin", StringComparison.OrdinalIgnoreCase)) {
				dir = Path.GetDirectoryName(dir);
				break;
			}

			dir = Path.GetDirectoryName(dir);
		}

		var ifn = Path.Combine(dir, fileName);

		if (fileName == "input.txt" && !File.Exists(ifn)) {
			Console.WriteLine($"Retrieving input file and saving to {ifn}");
			File.WriteAllText(ifn, await DownloadInput());
		} else if (!File.Exists(ifn)) {
			throw new FileNotFoundException($"File {ifn} does not exist.");
		}

		return File.ReadAllText(ifn);
	}

	private static async Task<string> DownloadInput()
	{
		if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("AOC_SESSION"))) {
			throw new InvalidOperationException("Set the AOC_SESSION environment variable.");
		}

		var cc = new CookieContainer();

		cc.Add(new Cookie("session", Environment.GetEnvironmentVariable("AOC_SESSION"), "/", "adventofcode.com"));

		using var handler = new HttpClientHandler() { CookieContainer = cc };
		using var hc      = new HttpClient(handler);

		return await hc.GetStringAsync($"https://adventofcode.com/2021/day/{Day}/input");
	}

	public static async Task<List<string>> GetInputLines(string fileName = "input.txt") => (await GetInput(fileName)).Split('\n').ToList();
}
