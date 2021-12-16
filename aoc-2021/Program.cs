using System.Net;
using System.Reflection;

const string ANSI_RESET = "\u001b[0m";
const string ANSI_GREEN = "\u001b[32m";

var projectFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

while (true) {
	if (Directory.GetFiles(projectFolder!, "*.csproj").Any()) {
		break;
	}

	projectFolder = Path.GetDirectoryName(projectFolder);
}

if (projectFolder == null) {
	throw new InvalidOperationException("Failed to locate project folder");
}

var newestProblem = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.Name == "Problem").OrderBy(t => t.Name).FirstOrDefault();

if (newestProblem == null) {
	throw new InvalidOperationException("Cannot determine newest problem.");
}

await RunProblem(newestProblem, projectFolder);

static async Task RunProblem(Type problem, string projectFolder)
{
	var inputFilename = Path.Join(projectFolder, problem.Namespace, "input.txt");

	if (!File.Exists(inputFilename)) {
		await DownloadInput(int.Parse(problem.Namespace![^2..]), inputFilename);
	}

	var method = problem.GetMethod("Main");

	Console.WriteLine($"{ANSI_GREEN}Running problem {problem.Namespace}{ANSI_RESET}");
	Console.WriteLine();

	method?.Invoke(null, new[] { new[] { inputFilename } });
}

//var problems =  && t?.Namespace?.IndexOf($"Day{problemNumber,2}") > -1);

//if (args != null && args.Length > 0 && args[0] == "all") {
//	var ret = 0;

//	foreach (var folder in Directory.GetDirectories(solutionFolder, "day_??").OrderBy(d => int.Parse(d[^2..]))) {
//		ret = Math.Max(ret, await RunProject(folder, Path.Combine(folder, "input.txt")));
//	}

//	return ret;
//} else if (args != null && args.Length > 0) {
//	var pFolder = Path.Combine(solutionFolder, args[0]);
//	var iFile   = Path.Combine(pFolder, "input.txt");

//	if (!Directory.Exists(pFolder)) {
//		Console.Error.WriteLine($"Project does not exist: {args[0]}");
//		return 1;
//	}

//	return await RunProject(pFolder, iFile);
//} else {
//	var newestProject = Directory.GetDirectories(solutionFolder, "day_??").OrderByDescending(d => int.Parse(d[^2..])).First();
//	var inputFile     = Path.Combine(newestProject, "input.txt");

//	return await RunProject(newestProject, inputFile);
//}

static async Task DownloadInput(int day, string fileName)
{
	if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("AOC_SESSION"))) {
		throw new InvalidOperationException("Set the AOC_SESSION environment variable.");
	}

	Console.WriteLine($"{ANSI_GREEN}Retrieving input file and saving to {fileName}{ANSI_RESET}");

	var cc = new CookieContainer();

	cc.Add(new Cookie("session", Environment.GetEnvironmentVariable("AOC_SESSION"), "/", "adventofcode.com"));

	using var handler = new HttpClientHandler() { CookieContainer = cc };
	using var hc = new HttpClient(handler);

	await File.WriteAllTextAsync(fileName, String.Join('\n', (await hc.GetStringAsync($"https://adventofcode.com/2021/day/{day}/input")).Split('\n').SkipLast(1)));
}
