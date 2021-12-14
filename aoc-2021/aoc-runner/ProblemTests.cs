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

	[Fact]
	public void TestAll()
	{
		var solutionFolder = DirectoryTools.FindSolutionFolder();

		foreach (var folder in Directory.GetDirectories(solutionFolder, "day_??").OrderBy(d => int.Parse(d[^2..]))) {
			foreach (var input in Directory.GetFiles(folder, "input*.txt")) {
				TestTools.ExecuteProject(output, folder, Path.GetFileName(input));
			}
		}
	}
}
