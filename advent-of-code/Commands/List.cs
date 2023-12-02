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
		foreach (var year in Problems.Years.Order()) {
			Console.WriteLine(year);

			foreach (var problem in Problems.GetProblems(year).OrderBy(p => p.Key)) {
				Console.WriteLine($"\tDay {problem.Key:00} - {problem.Value.Path}");
			}
		}
	}
}
