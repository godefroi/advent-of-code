using System.Net;
using System.Text.RegularExpressions;

namespace aoc_tools;

public class PuzzleInput
{
	private static Regex _dayPattern = new Regex(@"day_(\d+)");

	public static async Task<string> GetInput()
	{
		var day = int.Parse(_dayPattern.Match(System.Reflection.Assembly.GetEntryAssembly().GetName().Name).Groups[1].Value);

		if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("AOC_SESSION"))) {
			throw new InvalidOperationException("Set the AOC_SESSION environment variable.");
		}

		var cc = new CookieContainer();

		cc.Add(new Cookie("session", Environment.GetEnvironmentVariable("AOC_SESSION"), "/", "adventofcode.com"));

		using var handler = new HttpClientHandler() { CookieContainer = cc };
		using var hc      = new HttpClient(handler);

		return await hc.GetStringAsync($"https://adventofcode.com/2021/day/{day}/input");
	}
}
