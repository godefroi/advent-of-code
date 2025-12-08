using System.Net;

namespace AdventOfCode;

public class ProblemInput(ProblemMetadata problem)
{
	private char[]? _charsInput;
	private string? _stringInput;
	private string[]? _stringsInput;
	private readonly ProblemMetadata _problem = problem;

	public char[] Chars => _charsInput!;

	public string String => _stringInput!;

	public string[] Strings => _stringsInput!;

	public async Task LoadInput(string inputName)
	{
		var inputFilename = Path.Combine(_problem.Path, inputName);

		if (!File.Exists(inputFilename)) {
			await DownloadInput(_problem, inputFilename);
		}

		// yeah, it's not ideal, but it's the only way to be fair
		_stringInput  = await File.ReadAllTextAsync(inputFilename);
		_stringsInput = await File.ReadAllLinesAsync(inputFilename);
		_charsInput   = [.. _stringInput, '\n'];
		// NOTE NOTE NOTE we append a \n here, because we stripped it off in
		// DownloadInput(); that's probably not ideal, but I don't really wanna
		// go through and rework all the problems, so here we stand... at least
		// problems written for char[] will be correct.
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
