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

	[Fact]
	public void SampleInputReturnsCorrectValues()
	{
		var (max, topThree) = Execute(ReadFileLines("inputSample.txt"));

		Assert.Equal(24000, max);
		Assert.Equal(45000, topThree);
	}

	[Fact]
	public void InputReturnsCorrectValues()
	{
		var (max, topThree) = Execute(ReadFileLines("input.txt"));

		Assert.Equal(66616, max);
		Assert.Equal(199172, topThree);
	}
}
