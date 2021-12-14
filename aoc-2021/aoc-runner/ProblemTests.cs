using System.Diagnostics;

using Xunit;
using Xunit.Abstractions;

public class ProblemTests
{
	private readonly ITestOutputHelper output;

	public ProblemTests(ITestOutputHelper output)
	{
		this.output = output;
	}

	[Theory]
	[MemberData(nameof(GetTests))]
	public void TestAll(TestData testData) => ExecuteProject(output, testData.ProjectFolder, testData.InputFile);

	private static IEnumerable<object[]> GetTests()
	{
		var solutionFolder = DirectoryTools.FindSolutionFolder();
		var testList       = Environment.GetEnvironmentVariable("AOC_TEST_LIST");

		foreach (var folder in Directory.GetDirectories(solutionFolder, "day_??").OrderBy(d => int.Parse(d[^2..]))) {
			if (!string.IsNullOrWhiteSpace(testList) && testList.IndexOf(Path.GetFileName(folder)) == -1) {
				continue;
			}

			foreach (var input in Directory.GetFiles(folder, "input*.txt")) {
				yield return new object[] { new TestData(folder, Path.GetFileName(input)) };
			}
		}
	}

	private static void ExecuteProject(ITestOutputHelper output, string projectFolder, string inputFileName = "input.txt")
	{
		string? p1 = null;
		string? p2 = null;
		string? p1Val = null;
		string? p2Val = null;

		var inputfile = Path.Combine(projectFolder, inputFileName);
		var valFile = Path.Combine(projectFolder, Path.GetFileNameWithoutExtension(inputFileName) + ".val");

		Assert.True(Directory.Exists(projectFolder));
		Assert.True(File.Exists(inputfile));

		if (File.Exists(valFile)) {
			var lines = File.ReadAllLines(valFile);

			var v1 = lines.SingleOrDefault(l => l.StartsWith("part 1: "));
			var v2 = lines.SingleOrDefault(l => l.StartsWith("part 2: "));

			if (v1 != null) {
				p1Val = v1[8..];
			}

			if (v2 != null) {
				p2Val = v2[8..];
			}
		}

		var process = new Process()
		{
			EnableRaisingEvents = true,
			StartInfo = new ProcessStartInfo(Path.GetFileName("dotnet"))
			{
				WorkingDirectory = projectFolder,
				RedirectStandardOutput = true,
				Arguments = $"run --verbosity quiet -- {inputfile}",
			},
		};

		process.OutputDataReceived += (s, e) => {
			if (e.Data != null) {
				if (e!.Data.StartsWith("part 1: ") && !string.IsNullOrWhiteSpace(p1Val)) {
					p1 = e?.Data?[8..];
				}

				if (e!.Data.StartsWith("part 2: ") && !string.IsNullOrWhiteSpace(p2Val)) {
					p2 = e?.Data?[8..];
				}
			}

			//output.WriteLine($"{e?.Data}");
		};

		process.Start();
		process.BeginOutputReadLine();
		process.WaitForExit();

		if (p1Val != null && p1 != null) {
			Assert.Equal(p1Val, p1);
			//output.WriteLine($"Validated output {p1} against expected output {p1Val}");
		} else if (p1Val != null) {
			throw new Exception($"Validation needed for part 1 against {p1Val}, no output received");
		}

		if (p2Val != null && p2 != null) {
			Assert.Equal(p2Val, p2);
			//output.WriteLine($"Validated output {p2} against expected output {p2Val}");
		} else if (p2Val != null) {
			throw new Exception($"Validation needed for part 2 against {p2Val}, no output received");
		}

		Assert.Equal(0, process.ExitCode);
	}

	public class TestData
	{
		public TestData(string projectFolder, string inputFile)
		{
			ProjectFolder = projectFolder;
			InputFile     = inputFile;
		}

		public string ProjectFolder { get; private set; }

		public string InputFile { get; private set; }

		public override string ToString() => $"{Path.GetFileName(ProjectFolder)} ({InputFile})";
	}
}
