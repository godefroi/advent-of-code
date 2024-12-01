using System.CommandLine;

namespace AdventOfCode.Commands;

public static class Test
{
	public static Command GetCommand()
	{
		var command = new Command("test", "Execute tests for a problem (or problems)") {
			Options.All,
			Options.Year,
			Options.Day,
		};

		command.SetHandler(Execute, Options.All, Options.Year, Options.Day);

		return command;
	}

	private static async Task<int> Execute(bool all, int year, int day)
	{
		if (all) {
			throw new NotImplementedException("Running tests for all is not yet implemented.");
		}

		if (!Problems.GetProblems(year).TryGetValue(day, out var problem)) {
			Console.Error.WriteLine("The specified problem doesn't seem to exist.");
			return 0;
		}
Console.WriteLine($"Running tests for {problem.Problem.Assembly.Location}");
		using var runner = Xunit.Runners.AssemblyRunner.WithoutAppDomain(problem.Problem.Assembly.Location);
runner.OnDiagnosticMessage += e => {
	Console.WriteLine($"XUNIT: {e.Message}");
};
		//var wh = new WaitHa
		runner.OnExecutionComplete += e => {
			Console.WriteLine($"Execution complete: {e.TotalTests}.");
		};

		runner.OnDiscoveryComplete += e => {
			Console.WriteLine($"Running {e.TestCasesToRun} of {e.TestCasesDiscovered} tests.");
		};

		runner.Start(new Xunit.Runners.AssemblyRunnerStartOptions() {
			TypesToRun = [problem.Problem.FullName],
			DiagnosticMessages = true,
		});

		while (runner.Status != Xunit.Runners.AssemblyRunnerStatus.Idle) {
			await Task.Delay(100);
		}

		return await Task.FromResult(0);
	}
}
