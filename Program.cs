namespace aoc_2022;

public static class Program
{
	const string ANSI_RESET = "\u001b[0m";
	const string ANSI_GREEN = "\u001b[32m";

	public static async Task Main(string[] args)
	{
		var problemTypes  = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.Name == "Problem").OrderBy(t => int.Parse(t.Namespace![^2..]));

		if (args == null || args.Length == 0) {
			var newestProblem = problemTypes.LastOrDefault();

			if (newestProblem == null) {
				throw new InvalidOperationException("Cannot determine newest problem.");
			}

			await RunProblem(newestProblem);
		} else if (args.Length == 1 && args[0] == "all") {
			foreach (var type in problemTypes) {
				await RunProblem(type);
			}
		} else {
			await RunProblem(problemTypes.Single(t => t.Namespace!.Contains(args[0], StringComparison.OrdinalIgnoreCase)));
		}
	}

	private static async Task RunProblem(Type problem)
	{
		var projectFolder = FindProjectFolder();
		var inputFilename = Path.Join(projectFolder, problem.Namespace!.Split('.').Last(), "input.txt");

		if (!File.Exists(inputFilename)) {
			await DownloadInput(int.Parse(problem.Namespace![^2..]), inputFilename);
		}

		Console.WriteLine($"{ANSI_GREEN}Running problem {problem.Namespace}{ANSI_RESET}");

		var flags  = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
		var method = problem.GetMethod("Main", flags);
		var parms  = method!.GetParameters();

		if (parms.Length != 1 || parms[0].ParameterType != typeof(string)) {
			throw new NotSupportedException($"Problem {problem.FullName} method {method.Name} must take exactly one parameter of type [string].");
		}

		method?.Invoke(null, new[] { "input.txt" });

		Console.WriteLine();
	}

	private static async Task DownloadInput(int day, string fileName)
	{
		if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("AOC_SESSION"))) {
			throw new InvalidOperationException("Set the AOC_SESSION environment variable.");
		}

		Console.WriteLine($"{ANSI_GREEN}Retrieving input file and saving to {fileName}{ANSI_RESET}");

		var cc = new CookieContainer();

		cc.Add(new Cookie("session", Environment.GetEnvironmentVariable("AOC_SESSION"), "/", "adventofcode.com"));

		using var handler = new HttpClientHandler() { CookieContainer = cc };
		using var hc      = new HttpClient(handler);

		await File.WriteAllTextAsync(fileName, String.Join('\n', (await hc.GetStringAsync($"https://adventofcode.com/2022/day/{day}/input")).Split('\n').SkipLast(1)));
	}

	private static string FindProjectFolder()
	{
		var ret = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

		while (true) {
			if (Directory.GetFiles(ret!, "*.csproj").Any()) {
				return ret!;
			}

			ret = Path.GetDirectoryName(ret);
		}
	}
}
