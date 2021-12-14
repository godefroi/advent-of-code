using Xunit;
using Xunit.Abstractions;

namespace day_01;

public class ProblemTests
{
	private readonly ITestOutputHelper output;

	public ProblemTests(ITestOutputHelper output)
	{
		this.output = output;
	}

	[Theory]
	[MemberData(nameof(GetTests))]
	public void TestAll(TestData testData)
	{
		TestTools.ExecuteProject(output, testData.ProjectFolder, testData.InputFile);
	}

	private static IEnumerable<object[]> GetTests()
	{
		var solutionFolder = DirectoryTools.FindSolutionFolder();

		foreach (var folder in Directory.GetDirectories(solutionFolder, "day_??").OrderBy(d => int.Parse(d[^2..]))) {
			foreach (var input in Directory.GetFiles(folder, "input*.txt")) {
				yield return new object[] { new TestData(folder, Path.GetFileName(input)) };
			}
		}
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
