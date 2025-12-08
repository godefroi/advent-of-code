using System.CommandLine;
using System.Diagnostics;

namespace AdventOfCode.Commands;

public static class Time
{
	public static Command GetCommand()
	{
		var command = new Command("time", "Time the execution of a problem (or problems)") {
			Options.All,
			Options.Year,
			Options.Day,
			Options.Warmups,
			Options.Executions,
		};

		command.SetAction(Execute);

		return command;
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "TUnit0055:Do not overwrite the Console writer", Justification = "We're not running tests here.")]
	private static async Task Execute(ParseResult parseResult)
	{
		var all        = parseResult.GetValue(Options.All);
		var year       = parseResult.GetValue(Options.Year);
		var day        = parseResult.GetValue(Options.Day);
		var warmups    = parseResult.GetValue(Options.Warmups);
		var executions = parseResult.GetValue(Options.Executions);
		var problem    = Problems.GetProblems(year)[day];
		var input      = new ProblemInput(problem);
		var console    = Console.Out;
		var sw         = new Stopwatch();
		var output     = default(object);

		// load the input
		await input.LoadInput("input.txt");

		// warmup the problem
		try {
			Console.SetOut(TextWriter.Null);

			// run the warmups
			for (var i = 0; i < warmups; i++) {
				output = problem.Main(input);
			}

			sw.Restart();

			// run the executions
			for (var i = 0; i < executions; i++) {
				problem.Main(input);
			}

			sw.Stop();
		} finally {
			Console.SetOut(console);
		}

		Console.WriteLine($"output: {output}");
		Console.WriteLine($"avg: {sw.Elapsed.TotalMilliseconds / executions:F3}ms");
	}
}
