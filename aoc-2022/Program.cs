namespace aoc_2022;

public static partial class Program
{
	private const string ANSI_RESET = "\u001b[0m";
	private const string ANSI_GREEN = "\u001b[32m";

	public static async Task Main(string[] args)
	{
		var problems = Enumerable.Empty<ProblemMetadata>();

		if (args == null || args.Length == 0) {
			problems = Problems().OrderBy(p => p.Day).TakeLast(1);
		} else if (args.Length == 1 && "all".Equals(args[0], StringComparison.OrdinalIgnoreCase)) {
			problems = Problems().OrderBy(p => p.Day);
		} else if (int.TryParse(args[0], out var day)) {
			problems = Problems().Where(p => p.Day == day);
		} else {
			Console.WriteLine("Invalid arguments; try specifying a day number, or 'all'.");
			return;
		}

		foreach (var p in problems) {
			await RunProblem(p);
		}
	}

	private static async Task RunProblem(ProblemMetadata problem)
	{
		var inputFilename = Path.Join(problem.Path, "input.txt");

		if (!File.Exists(inputFilename)) {
			await DownloadInput(problem.Day, inputFilename);
		}

		Console.WriteLine($"{ANSI_GREEN}Running day {problem.Day:d2}{ANSI_RESET}");

		problem.Main("input.txt");

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

		await File.WriteAllTextAsync(fileName, string.Join('\n', (await hc.GetStringAsync($"https://adventofcode.com/2022/day/{day}/input")).Split('\n').SkipLast(1)));
	}

	private static partial ProblemMetadata[] Problems();
}
