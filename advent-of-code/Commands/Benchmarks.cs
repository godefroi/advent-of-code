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
			Options.Year,
			Options.Day,
		};

		command.SetAction(Execute);

		return command;
	}

	private static void Execute(ParseResult parseResult)
	{
		var year    = parseResult.GetValue(Options.Year);
		var day     = parseResult.GetValue(Options.Day);
		var problem = Problems.GetProblems(year)[day];

		if (problem.Benchmarks == null) {
			Console.Error.WriteLine($"No benchmarks defined for problem {problem.Year}/{problem.Day:00}");
			return;
		}

		var job = new Job()
			.DontEnforcePowerPlan()
			.Apply(Job.InProcessDontLogOutput)
			//.Apply(Job.InProcess)
			//.Apply(Job.ShortRun)
			//.Apply(Job.InProcessDontLogOutput)
			;

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

		BenchmarkSwitcher
			.FromTypes([problem.Benchmarks])
			.RunAllJoined(config);
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
