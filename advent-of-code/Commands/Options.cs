using System.CommandLine;

namespace AdventOfCode.Commands;

public static class Options
{
	public static Option<bool> All { get; } = new("--all", () => false, "Include all problems");

	public static Option<int> Warmups { get; } = new("--warmups", () => 1, "Number of times to execute problems for warmup");

	public static Option<int> Executions { get; } = new("--executions", () => 1000, "Number of times to execute each problem");

	public static Option<string> InputName { get; } = new("--inputFile", () => GetDefault("AOC_INPUTFILE", "input.txt"), "Filename to read as problem input (env AOC_INPUTFILE)");

	public static Option<int> Year { get; } = new("--year", () => GetDefault("AOC_YEAR", Problems.CurrentProblem.Year), "Year on which to operate (env AOC_YEAR)");

	public static Option<int> Day { get; } = new("--day", () => GetDefault("AOC_DAY", Problems.CurrentProblem.Day), "Day on which to operate (env AOC_DAY)");

	private static string GetDefault(string environmentVarName, string missingDefault) =>
		Environment.GetEnvironmentVariable(environmentVarName) ?? missingDefault;

	private static int GetDefault(string environmentVarName, int missingDefault) =>
		int.TryParse(Environment.GetEnvironmentVariable(environmentVarName), out var result) ? result : missingDefault;
}
