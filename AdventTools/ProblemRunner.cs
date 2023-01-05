using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Parameters;
using BenchmarkDotNet.Running;

using System.Net;
using System.Reflection;

using static AdventOfCode.AnsiCodes;

namespace AdventOfCode;

public class ProblemRunner
{
	public static async Task Execute(string[] args, ProblemMetadata[]? problemMetadata)
	{
		if (problemMetadata == null || problemMetadata.Length == 0) {
			Console.Error.WriteLine($"{ANSI_RED}No problem classes found; something went wrong with the generator, maybe?{ANSI_RESET}");
			return;
		}

		if (args == null || args.Length == 0) {
			await RunProblems(problemMetadata.OrderByDescending(p => p.Day).Take(1));
		} else if (args.Length == 1 && "all".Equals(args[0], StringComparison.OrdinalIgnoreCase)) {
			await RunProblems(problemMetadata.OrderBy(p => p.Day));
		} else if (int.TryParse(args[0], out var day)) {
			await RunProblems(problemMetadata.Where(p => p.Day == day));
		} else if (args.Length == 1 && "list".Equals(args[0], StringComparison.OrdinalIgnoreCase)) {
			foreach (var pm in problemMetadata) {
				Console.WriteLine($"Day {pm.Day} in {pm.Path}");
				foreach (var benchmark in pm.BenchmarkType.GetMethods()) {
					var attr = benchmark.GetCustomAttribute<BenchmarkAttribute>();

					if (attr == null) {
						continue;
					}

					Console.WriteLine($"\t{benchmark.Name} ({attr.Description})");
				}
			}
		} else if (args.Length > 0 && "bench".Equals(args[0], StringComparison.OrdinalIgnoreCase)) {
			if (args.Length > 1 && "all".Equals(args[1], StringComparison.OrdinalIgnoreCase)) {
				await RunBenchmarks(problemMetadata, args.Skip(2).ToArray());
			} else {
				await RunBenchmarks(problemMetadata.OrderByDescending(p => p.Day).Take(1), args.Skip(1).ToArray());
			}
		} else {
			Console.Error.WriteLine($"{ANSI_RED}Invalid arguments; try specifying a day number, or 'all'.{ANSI_RESET}");
		}
	}

	private static List<ProblemMetadata> GetProblems()
	{
		var entryAssembly = Assembly.GetEntryAssembly();

		if (entryAssembly == null) {
			throw new InvalidOperationException("The entry assembly is undetermined.");
		}

		var problems      = new List<ProblemMetadata>(25);
		var projectFolder = Path.GetDirectoryName(entryAssembly.Location);

		while (projectFolder != null) {
			if (File.Exists(Path.Combine(projectFolder, "*.csproj"))) {
				break;
			}

			projectFolder = Path.GetDirectoryName(projectFolder);
		}

		if (projectFolder == null) {
			throw new InvalidOperationException("Unable to determine location of project.");
		}

		foreach (var typeInfo in entryAssembly.DefinedTypes) {
			if (typeInfo.Name != "Problem") {
				continue;
			}

			if (typeInfo.Namespace == null) {
				continue;
			}

			var ns     = typeInfo.Namespace.Split('.').Last();
			var folder = Path.Combine(projectFolder, ns);

			if (!Directory.Exists(folder)) {
				continue;
			}

			if (!int.TryParse(ns.AsSpan(ns.Length - 2), out var dayNumber)) {
				continue;
			}

			var mainMethod = typeInfo.DeclaredMethods.SingleOrDefault(m => m.Name == "Main");

			if (mainMethod == null) {
				continue;
			}

			var parameters = mainMethod.GetParameters();

			if (parameters.Length != 1 || parameters[0].ParameterType != typeof(string)) {
				continue;
			}

			var action = mainMethod.ReturnType == typeof(void) ? mainMethod.CreateDelegate<Action<string>>() : new Action<string>(fn => Console.WriteLine(mainMethod.CreateDelegate<Func<string, object?>>().Invoke(fn)));

			new ProblemMetadata(typeInfo.AsType(), dayNumber, folder, action, typeof(void));
		}

		return problems;
	}

	private static async Task RunProblems(IEnumerable<ProblemMetadata> problems)
	{
		foreach (var problem in problems) {
			await RunProblem(problem);
		}
	}

	private static async Task RunProblem(ProblemMetadata problem)
	{
		var inputFilename = Path.Combine(problem.Path, "input.txt");

		if (!File.Exists(inputFilename)) {
			await DownloadInput(problem.Day, inputFilename);
		}

		Console.WriteLine($"{ANSI_GREEN}Running day {problem.Day:d2}{ANSI_RESET}");

		problem.Main("input.txt");

		Console.WriteLine();
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

	private static async Task RunBenchmarks(IEnumerable<ProblemMetadata> problems, string[] args)
	{
		//ProblemBenchmark.Problems = problems.Order().Take(2).ToList();
		//BenchmarkRunner.Run<ProblemBenchmark>();

		var job = new Job().DontEnforcePowerPlan();

		job.Apply(Job.ShortRun);
		job.Apply(Job.InProcessDontLogOutput);

		//var config = ManualConfig.CreateMinimumViable();
		//foreach (var cp in config.GetColumnProviders()) { Console.WriteLine(cp.GetType().FullName); }
		//return;
		var config = ManualConfig.CreateEmpty();

		config.AddLogger(new MyLogger());
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

		var summary = BenchmarkSwitcher.FromTypes(problems.OrderBy(p => p.Day).Take(4).Select(p => p.BenchmarkType).ToArray()).RunAllJoined(config);

		foreach (var bc in summary.BenchmarksCases) {
			Console.WriteLine(bc.Descriptor.WorkloadMethodDisplayInfo);
		}

		//var benchmarkType   = typeof(GenericBenchmark);
		//var benchmarkMethod = benchmarkType.GetMethod("Execute") ?? throw new Exception("Unable to locate benchmark method");

		////var descriptors = problems.Select(p => new Descriptor(benchmarkType, benchmarkMethod, description: p.ProblemType.Namespace ?? "no-namespace", categories: new[] { "cat1", p.ProblemType.FullName ?? "no fullname" })).ToArray();
		////var cases       = descriptors.Select(d => BenchmarkCase.Create(d, ))

		//var job = new Job("teh_job");

		//problems.Select(p => {
		//	var parameterDef  = new ParameterDefinition("mainAction", true, Array.Empty<object>(), true, typeof(Action<string>), 1);
		//	var parameterInst = new ParameterInstance(parameterDef, p.Main, BenchmarkDotNet.Reports.SummaryStyle.Default);
		//	var descriptor    = new Descriptor(benchmarkType, benchmarkMethod, description: p.ProblemType.Namespace ?? "no-namespace", categories: new[] { "cat1", p.ProblemType.FullName ?? "no fullname" });
		//	var benchmarkCase = BenchmarkCase.Create(descriptor, job, new ParameterInstances(new List<ParameterInstance>() { parameterInst }), null);
		//});

		////var d = new Descriptor()
		////var bmCase = BenchmarkCase.Create()
		//var bri = new BenchmarkRunInfo()
		//BenchmarkRunner.Run()

		//var assemblies = problems.Select(p => p.ProblemType.Assembly).Distinct().ToList();
		//var namespaces = problems.Select(p => p.ProblemType.Namespace).Distinct().ToHashSet();
		//var types      = assemblies.SelectMany(asm => asm.DefinedTypes.Where(t => namespaces.Contains(t.Namespace))).Select(ti => ti.AsType()).ToArray();

		////var config = ManualConfig.CreateEmpty();

		//var summary = BenchmarkSwitcher.FromTypes(types).Run(args);

		await Task.CompletedTask;
	}

	private static async Task DownloadInput(int day, string fileName)
	{
		if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("AOC_SESSION"))) {
			throw new InvalidOperationException("Set the AOC_SESSION environment variable.");
		}

		var assemblyName = System.Reflection.Assembly.GetEntryAssembly()!.GetName().Name;
		var yearMatch    = System.Text.RegularExpressions.Regex.Match(assemblyName!, "\\d{4}");

		if (!yearMatch.Success) {
			throw new InvalidOperationException($"Unable to parse year from assembly name {assemblyName}");
		}

		Console.WriteLine($"{ANSI_GREEN}Retrieving input file and saving to {fileName}{ANSI_RESET}");

		var cc = new CookieContainer();

		cc.Add(new Cookie("session", Environment.GetEnvironmentVariable("AOC_SESSION"), "/", "adventofcode.com"));

		using var handler = new HttpClientHandler() { CookieContainer = cc };
		using var hc      = new HttpClient(handler);

		//var inputLines = (await hc.GetStringAsync($"https://adventofcode.com/{yearMatch.Value}/day/{day}/input")).Split('\n');

		//File.WriteAllText(fileName, string.Join("\n", inputLines.Take(inputLines.Length - 1)));

		File.WriteAllText(fileName, (await hc.GetStringAsync($"https://adventofcode.com/{yearMatch.Value}/day/{day}/input")).TrimEnd('\n'));
	}

	public class ProblemBenchmark
	{
		public static IEnumerable<ProblemMetadata>? Problems { get; set; }

		[Benchmark(Description = "foo")]
		//[BenchmarkDotNet.Attributes.SimpleJob(BenchmarkDotNet.Engines.RunStrategy.)]
		//[BenchmarkCategory()]
		[ArgumentsSource(nameof(Problems))]
		public void Execute(ProblemMetadata problemMetadata)
		{
			problemMetadata.Main("input.txt");
		}

		public IEnumerable<object[]> GetProblems()
		{
			if (Problems == null ) {
				return Enumerable.Empty<object[]>();
			}
			Console.WriteLine($"returning {Problems.Count()} problems");
			return Problems.Select(p => new object[] { p.Main, "input.txt" });
		}
	}
}
