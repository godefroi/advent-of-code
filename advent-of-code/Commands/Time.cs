using System.CommandLine;
using System.Diagnostics;

namespace AdventOfCode.Commands;

public static class Time
{
	public static Command GetCommand()
	{
		var command = new Command("time", "Time the execution of a problem (or problems)") {
			Options.All,
			Options.Warmups,
			Options.Executions,
		};

		command.SetHandler(Execute, Options.All, Options.Warmups, Options.Executions);

		return command;
	}

	private static async Task Execute(bool all, int warmups, int executions)
	{
		var problem = Problems.CurrentProblem;
		var console = Console.Out;
		var sw      = new Stopwatch();
		var output  = default(string);

		// load the input
		var input = await Program.LoadInput(problem, "input.txt");

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
		Console.WriteLine($"avg: {sw.Elapsed.TotalMilliseconds / executions}");
	}
}
