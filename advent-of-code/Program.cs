using System.CommandLine;

using static AdventOfCode.AnsiCodes;

namespace AdventOfCode;

public static class Program
{
	public static async Task<int> Main(string[] args)
	{
		var command = new RootCommand("Advent of Code runner") {
			Commands.List.GetCommand(),
			Commands.Time.GetCommand(),
			Commands.Benchmarks.GetCommand(),
			Commands.Test.GetCommand(),
			Commands.Options.Year,
			Commands.Options.Day,
			Commands.Options.InputName,
		};

		command.SetAction(async parseResult => {
			var year      = parseResult.GetValue(Commands.Options.Year);
			var day       = parseResult.GetValue(Commands.Options.Day);
			var inputName = parseResult.GetValue(Commands.Options.InputName) ?? throw new InvalidOperationException("No input name received from option.");
			var problems  = Problems.GetProblems(parseResult);

			foreach (var problem in problems) {
				Console.WriteLine($"{ANSI_GREEN}Running year {problem.Year} day {problem.Day:d2}{ANSI_RESET}");

				var input = new ProblemInput(problem);

				await input.LoadInput(inputName);

				Console.WriteLine(problem.Main(input));
				Console.WriteLine();
			}
		});

		return await command.Parse(args).InvokeAsync();
	}
}
