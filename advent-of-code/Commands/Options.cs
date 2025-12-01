using System.CommandLine;

namespace AdventOfCode.Commands;

public static class Options
{
	private readonly static int? _envVarYear = int.TryParse(Environment.GetEnvironmentVariable("AOC_YEAR"), out var result) ? result : null;
	private readonly static int? _envVarDay = int.TryParse(Environment.GetEnvironmentVariable("AOC_DAY"), out var result) ? result : null;
	private readonly static string? _envVarInput = Environment.GetEnvironmentVariable("AOC_INPUTFILE");

	public static Option<bool> All { get; } = new("--all") {
		Description = "Include all problems",
		DefaultValueFactory = pr => false,
	};

	public static Option<int> Warmups { get; } = new("--warmups") {
		Description = "Number of times to execute problems for warmup",
		DefaultValueFactory = pr => 10,
	};

	public static Option<int> Executions { get; } = new("--executions") {
		Description = "Number of times to execute each problem",
		DefaultValueFactory = pr => 5000,
	};

	public static Option<string> InputName { get; } = new("--inputFile") {
		Description = "Filename to read as problem input (env AOC_INPUTFILE)",
		DefaultValueFactory = pr => _envVarInput ?? "input.txt",
	};

	public static Option<int> Year { get; } = new("--year") {
		Description = "Year on which to operate (env AOC_YEAR)",
		DefaultValueFactory = pr => _envVarYear ?? Problems.CurrentProblem.Year,
	};

	public static Option<int> Day { get; } = new("--day") {
		Description = "Day on which to operate (env AOC_DAY)",
		DefaultValueFactory = pr => _envVarDay ?? Problems.CurrentProblem.Day,
	};
}
