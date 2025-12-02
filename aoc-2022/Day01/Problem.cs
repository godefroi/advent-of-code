namespace AdventOfCode.Year2022.Day01;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	internal static (long max, long topThree) Execute(string[] lines)
	{
		var elves = ChunkByEmpty(lines).Select(group => group.Select(s => long.Parse(s)).Sum()).OrderDescending().ToList();
		var ret   = (elves.First(), elves.Take(3).Sum());

		Console.WriteLine(ret);
		return ret;
	}

	[Test]
	public async Task SampleInputReturnsCorrectValues()
	{
		var (max, topThree) = Execute(ReadFileLines("inputSample.txt"));

		await Assert.That(max).IsEqualTo(24000);
		await Assert.That(topThree).IsEqualTo(45000);
	}

	[Test]
	public async Task InputReturnsCorrectValues()
	{
		var (max, topThree) = Execute(ReadFileLines("input.txt"));

		await Assert.That(max).IsEqualTo(66616);
		await Assert.That(topThree).IsEqualTo(199172);
	}
}
