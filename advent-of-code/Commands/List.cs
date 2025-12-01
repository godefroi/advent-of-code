using System.CommandLine;

namespace AdventOfCode.Commands;

public static class List
{
	public static Command GetCommand()
	{
		var command = new Command("list", "List available problems") {
			Options.Year,
			Options.Day,
			Options.All,
		};

		command.SetAction(Execute);

		return command;
	}

	private static void Execute(ParseResult parseResult)
	{
		var problems = Problems.GetProblems(parseResult);

		foreach (var yearProblems in problems.GroupBy(p => p.Year).OrderBy(p => p.Key)) {
			Console.WriteLine(yearProblems.Key);

			foreach (var problem in yearProblems.OrderBy(m => m.Day)) {
				Console.WriteLine($"\tDay {problem.Day:00} - {problem.Path}");
			}
		}
	}
}
