namespace AdventOfCode.Year2022.Day03;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Main, typeof(Problem), null);

	private static readonly Dictionary<char, int> _priorities = Enumerable.Range('a', 'z' - 'a' + 1).Concat(Enumerable.Range('A', 'Z' - 'A' + 1)).Select((c, i) => ((char) c, i)).ToDictionary(item => item.Item1, item => item.i + 1);

	public static (long, long) Main(string[] lines)
	{
		var rucksacks = lines.Select(Parse).ToList();

		var part1 = rucksacks.Select(sack => sack.First.Intersect(sack.Second).Single()).Sum(item => _priorities[item]);
		var part2 = rucksacks.Chunk(3).Select(sacks => sacks[0].Contents.Intersect(sacks[1].Contents).Intersect(sacks[2].Contents).Single()).Sum(item => _priorities[item]);

		Console.WriteLine($"part1: {part1}"); // 7817
		Console.WriteLine($"part2: {part2}"); // 2444

		return (part1, part2);
	}

	private static Rucksack Parse(string line) => new(line[..(line.Length / 2)].ToCharArray(), line[(line.Length / 2)..].ToCharArray());

	private readonly record struct Rucksack(char[] First, char[] Second)
	{
		public IEnumerable<char> Contents => First.Concat(Second);
	}

	[Theory]
	[InlineData("vJrwpWtwJgWrhcsFMMfFFhFp")]
	public void LinesParseCorrectly(string line)
	{
		var (first, second) = Parse(line);

		Assert.Equal(first.Length, second.Length);
	}

	[Theory]
	[InlineData("vJrwpWtwJgWrhcsFMMfFFhFp", 'p')]
	public void IntersectionCalculatesCorrectly(string line, char duplicated)
	{
		var (first, second) = Parse(line);

		Assert.Equal(duplicated, first.Intersect(second).Single());
	}

	[Theory]
	[InlineData('a', 1)]
	[InlineData('b', 2)]
	[InlineData('z', 26)]
	[InlineData('A', 27)]
	[InlineData('B', 28)]
	[InlineData('Z', 52)]
	public void PrioritiesAreCorrect(char item, int priority)
	{
		Assert.Equal(priority, _priorities[item]);
	}

	[Fact]
	public void Part1CalculatesCorrectly()
	{
		Assert.Equal(157, Main(ReadFileLines("inputSample.txt")).Item1);
	}

	[Fact]
	public void Part2CalculatesCorrectly()
	{
		Assert.Equal(70, Main(ReadFileLines("inputSample.txt")).Item2);
	}
}
