using System.CommandLine;

namespace AdventOfCode.Commands;

public static class Options
{
	public static Option<bool> All { get; } = new("--all", () => false, "Include all problems");

	public static Option<int> Warmups { get; } = new("--warmups", () => 1, "Number of times to execute problems for warmup");

	public static Option<int> Executions { get; } = new("--executions", () => 1000, "Number of times to execute each problem");

	public static Option<string> InputName { get; } = new("--inputFile", () => "input.txt", "Filename to read as problem input");

	public static Option<int> Year { get; } = new("--year", () => Problems.CurrentProblem.Year, "Year on which to operate");

	public static Option<int> Day { get; } = new("--day", () => Problems.CurrentProblem.Day, "Day on which to operate");
}
