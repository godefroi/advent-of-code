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

		command.SetAction(Execute);

		return command;
	}

	private static async Task<int> Execute(ParseResult parseResult)
	{
		var all  = parseResult.GetValue(Options.All);
		var year = parseResult.GetValue(Options.Year);
		var day  = parseResult.GetValue(Options.Day);

		if (all) {
			throw new NotImplementedException("Running tests for all is not yet implemented.");
		}

		if (!Problems.GetProblems(year).TryGetValue(day, out var problem)) {
			Console.Error.WriteLine("The specified problem doesn't seem to exist.");
			return 1;
		}

		if (string.IsNullOrWhiteSpace(problem.Problem.FullName)) {
			Console.Error.WriteLine($"No type name available for specified problem.");
			return 2;
		}

		await using var runner = Xunit.Runners.AssemblyRunner.WithoutAppDomain(problem.Problem.Assembly.Location);

		var tcs = new TaskCompletionSource();

		runner.OnDiagnosticMessage += e => {
			Console.WriteLine($"XUNIT: {e.Message}");
		};

		runner.OnTestStarting += e => {
			Console.WriteLine($"Running {e.TestDisplayName}");
		};

		runner.OnTestFailed += e => {
			Console.WriteLine($"Test FAILED: {e.TestDisplayName}");
		};

		runner.OnTestPassed += e => {
			Console.WriteLine($"Test passed: {e.TestDisplayName}");
		};

		runner.OnExecutionComplete += e => {
			//Console.WriteLine($"Execution complete: {e.TotalTests}.");
			tcs.SetResult();
		};

		// runner.OnDiscoveryComplete += e => {
		// 	Console.WriteLine($"Running {e.TestCasesToRun} of {e.TestCasesDiscovered} tests.");
		// };

		runner.Start(new Xunit.Runners.AssemblyRunnerStartOptions() {
			TypesToRun = [problem.Problem.FullName],
			DiagnosticMessages = true,
		});

		await tcs.Task;

		while (runner.Status != Xunit.Runners.AssemblyRunnerStatus.Idle) {
			await Task.Delay(50);
		}

		return 0;
	}
}
