using System.CommandLine;

namespace AdventOfCode.Commands;

public static class List
{
	public static Command GetCommand()
	{
		var command = new Command("list", "List available problems") {
			Options.All,
		};

		command.SetHandler(Execute, Options.All);

		return command;
	}

	private static void Execute(bool all)
	{
		foreach (var problem in Problems.CurrentYear.Values.OrderBy(m => m.Day)) {
			Console.WriteLine($"{problem.Year} {problem.Day:00} {problem.Path}");
		}
	}
}
