using System.Diagnostics;
using System.Net;

using aoc_tools;

const string ANSI_RESET = "\u001b[0m";
const string ANSI_GREEN = "\u001b[32m";

var solutionFolder = DirectoryTools.FindSolutionFolder();

if (args != null && args.Length > 0 && args[0] == "all") {
	var ret = 0;

	foreach (var folder in Directory.GetDirectories(solutionFolder, "day_??").OrderBy(d => int.Parse(d[^2..]))) {
		ret = Math.Max(ret, await RunProject(folder, Path.Combine(folder, "input.txt")));
	}

	return ret;
} else {
	var newestProject = Directory.GetDirectories(solutionFolder, "day_??").OrderByDescending(d => int.Parse(d[^2..])).First();
	var inputFile     = Path.Combine(newestProject, "input.txt");

	return await RunProject(newestProject, inputFile);
}

static async Task<int> RunProject(string projectFolder, string inputFileName)
{
	Console.WriteLine($"{ANSI_GREEN}Running project {Path.GetFileName(projectFolder)}{ANSI_RESET}");

	if (!File.Exists(inputFileName)) {
		await DownloadInput(int.Parse(projectFolder[^2..]), inputFileName);
	}

	var psi = new ProcessStartInfo("dotnet")
	{
		WorkingDirectory = projectFolder,
	};

	psi.ArgumentList.Add("run");
	psi.ArgumentList.Add("--");
	psi.ArgumentList.Add(inputFileName);

	var process = new Process()
	{
		StartInfo = psi,
	};

	process.Start();
	process!.WaitForExit();

	return process!.ExitCode;
}


static async Task DownloadInput(int day, string fileName)
{
	if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("AOC_SESSION"))) {
		throw new InvalidOperationException("Set the AOC_SESSION environment variable.");
	}

	Console.WriteLine($"{ANSI_GREEN}Retrieving input file and saving to {fileName}{ANSI_RESET}");

	var cc = new CookieContainer();

	cc.Add(new Cookie("session", Environment.GetEnvironmentVariable("AOC_SESSION"), "/", "adventofcode.com"));

	using var handler = new HttpClientHandler() { CookieContainer = cc };
	using var hc = new HttpClient(handler);

	await File.WriteAllLinesAsync(fileName, (await hc.GetStringAsync($"https://adventofcode.com/2021/day/{day}/input")).Split('\n').SkipLast(1));
}
