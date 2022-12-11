namespace aoc_2022.Day01;

public class Problem
{
	internal static (long max, long topThree) Main(string fileName)
	{
		var lines = ReadFileLines(fileName);
		var elves = ChunkByEmpty(lines).Select(group => group.Select(s => long.Parse(s)).Sum()).OrderDescending().ToList();
		var ret   = (elves.First(), elves.Take(3).Sum());

		Console.WriteLine(ret);
		return ret;
	}

	[Fact]
	public void SampleInputReturnsCorrectValues()
	{
		var (max, topThree) = Main("inputSample.txt");

		Assert.Equal(24000, max);
		Assert.Equal(45000, topThree);
	}

	[Fact]
	public void InputReturnsCorrectValues()
	{
		var (max, topThree) = Main("input.txt");

		Assert.Equal(66616, max);
		Assert.Equal(199172, topThree);
	}
}
