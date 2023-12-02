using System.CommandLine;

namespace AdventOfCode.Commands;

public static class Options
{
	public static Option<bool> All { get; } = new("--all", () => false, "Include all problems");

	public static Option<int> Warmups { get; } = new("--warmups", () => 1, "Number of times to execute problems for warmup");

	public static Option<int> Executions { get; } = new("--executions", () => 1000, "Number of times to execute each problem");

	public static Option<string> InputName { get; } = new("--inputFile", () => "input.txt", "Filename to read as problem input");
}
