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

	private static IEnumerable<IEnumerable<string>> ChunkByEmpty(IEnumerable<string> lines)
	{
		var doneEnumerating = false;

		using var enumerator = lines.GetEnumerator();

		while (!doneEnumerating) {
			yield return EnumerateWhileNotEmpty(enumerator);
		}

		IEnumerable<string> EnumerateWhileNotEmpty(IEnumerator<string> enumerator)
		{
			while (enumerator.MoveNext()) {
				if (enumerator.Current == string.Empty) {
					yield break;
				} else {
					yield return enumerator.Current;
				}
			}

			doneEnumerating = true;
		}
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

	[Fact]
	public void ChunkingWorksCorrectly()
	{
		var chunks = ChunkByEmpty(File.ReadAllLines(GetFilePath("inputSample.txt"))).Select(e => e.ToList()).ToList();

		Assert.Collection(chunks,
			chunk => Assert.Collection(chunk,
				l => l.Equals("1000"),
				l => l.Equals("2000"),
				l => l.Equals("3000")),
			chunk => Assert.Collection(chunk,
				l => l.Equals("4000")),
			chunk => Assert.Collection(chunk,
				l => l.Equals("5000"),
				l => l.Equals("6000")),
			chunk => Assert.Collection(chunk,
				l => l.Equals("7000"),
				l => l.Equals("8000"),
				l => l.Equals("9000")),
			chunk => Assert.Collection(chunk,
				l => l.Equals("10000")));
	}
}
