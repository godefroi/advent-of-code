using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
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
				Console.WriteLine($"{pm.Day} in {pm.Path}");
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

			new ProblemMetadata(typeInfo.AsType(), dayNumber, folder, action);
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

	private static async Task RunBenchmarks(IEnumerable<ProblemMetadata> problems, string[] args)
	{
		problems = problems.ToList();

		var assemblies = problems.Select(p => p.ProblemType.Assembly).Distinct().ToList();
		var namespaces = problems.Select(p => p.ProblemType.Namespace).Distinct().ToHashSet();
		var types      = assemblies.SelectMany(asm => asm.DefinedTypes.Where(t => namespaces.Contains(t.Namespace))).Select(ti => ti.AsType()).ToArray();

		//var config = ManualConfig.CreateEmpty();

		var summary = BenchmarkSwitcher.FromTypes(types).Run(args);

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
}
