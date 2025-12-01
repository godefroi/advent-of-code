using System.CommandLine;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;

namespace AdventOfCode.Commands;

public static class Benchmarks
{
	public static Command GetCommand()
	{
		var command = new Command("benchmarks", "Execute benchmarks associated with a problem") {
		};

		command.SetAction(Execute);

		return command;
	}

	private static void Execute(ParseResult parseResult)
	{
		if (Problems.CurrentProblem.Benchmarks == null) {
			Console.Error.WriteLine($"No benchmarks defined for problem {Problems.CurrentProblem.Year}/{Problems.CurrentProblem.Day:00}");
			return;
		}

		// var runMethod     = typeof(BenchmarkRunner).GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).SingleOrDefault(m => m.Name == "Run" && m.IsGenericMethod) ?? throw new InvalidOperationException("No Run method found.");
		// var genericMethod = runMethod.MakeGenericMethod(Problems.CurrentProblem.Benchmarks);

		// genericMethod.Invoke(null, [null, null]);
		//BenchmarkRunner.Run<AdventOfCode.Year2023.Day01.Problem>()

		var job = new Job()
			.DontEnforcePowerPlan()
			.Apply(Job.InProcessDontLogOutput)
			//.Apply(Job.InProcess)
			;

		//job.Apply(Job.ShortRun);
		//job.Apply(Job.InProcessDontLogOutput);

		//var config = ManualConfig.CreateMinimumViable();
		//foreach (var cp in config.GetColumnProviders()) { Console.WriteLine(cp.GetType().FullName); }
		//return;
		var config = ManualConfig.CreateEmpty();

		//config.AddLogger(new MyLogger());
		config.AddLogger(ConsoleLogger.Unicode);
		config.AddDiagnoser(new MemoryDiagnoser(new MemoryDiagnoserConfig(false)));

		config.AddColumnProvider(
			DefaultColumnProviders.Descriptor,
			DefaultColumnProviders.Job,
			DefaultColumnProviders.Statistics,
			DefaultColumnProviders.Params,
			DefaultColumnProviders.Metrics);

		config.Options |= ConfigOptions.DisableLogFile;
		config.Options |= ConfigOptions.JoinSummary;

		config.AddJob(job);

		var summary = BenchmarkSwitcher.FromTypes([Problems.CurrentProblem.Benchmarks]).RunAllJoined(config);

		foreach (var bc in summary.BenchmarksCases) {
			Console.WriteLine(bc.Descriptor.WorkloadMethodDisplayInfo);
		}
	}

	private class MyLogger : ILogger
	{
		private bool _writeStatistics = false;

		public string Id => "mylogger?";

		public int Priority => 0;

		public void Flush()
		{
			//throw new NotImplementedException();
		}

		public void Write(LogKind logKind, string text)
		{
			if (logKind == LogKind.Info || logKind == LogKind.Header) {
				return;
			} else if (logKind == LogKind.Statistic) {
				Console.Write(text);
			}

			//Console.Write($"[lk {logKind}]{text}");
		}

		public void WriteLine()
		{
			if (_writeStatistics) {
				Console.WriteLine();
			}
			//Console.WriteLine("[el]");
		}

		public void WriteLine(LogKind logKind, string text)
		{
			if (logKind == LogKind.Header && text == "// * Summary *") {
				_writeStatistics = true;
			} else if (logKind == LogKind.Info || logKind == LogKind.Header || logKind == LogKind.Error) {
				return;
			} else if (logKind == LogKind.Statistic && _writeStatistics) {
				Console.WriteLine(text);
			} else {
				//Console.WriteLine($"[lk {logKind}]{text}");
			}
		}
	}

}
