namespace AdventOfCode.Year2022.Day03;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	private static readonly Dictionary<char, int> _priorities = Enumerable.Range('a', 'z' - 'a' + 1).Concat(Enumerable.Range('A', 'Z' - 'A' + 1)).Select((c, i) => ((char) c, i)).ToDictionary(item => item.Item1, item => item.i + 1);

	public static (long, long) Execute(string[] lines)
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

	[Test]
	[Arguments("vJrwpWtwJgWrhcsFMMfFFhFp")]
	public async Task LinesParseCorrectly(string line)
	{
		var (first, second) = Parse(line);

		await Assert.That(first.Length).IsEqualTo(second.Length);
	}

	[Test]
	[Arguments("vJrwpWtwJgWrhcsFMMfFFhFp", 'p')]
	public async Task IntersectionCalculatesCorrectly(string line, char duplicated)
	{
		var (first, second) = Parse(line);

		await Assert.That(first.Intersect(second).Single()).IsEqualTo(duplicated);
	}

	[Test]
	[Arguments('a', 1)]
	[Arguments('b', 2)]
	[Arguments('z', 26)]
	[Arguments('A', 27)]
	[Arguments('B', 28)]
	[Arguments('Z', 52)]
	public async Task PrioritiesAreCorrect(char item, int priority)
	{
		await Assert.That(_priorities[item]).IsEqualTo(priority);
	}

	[Test]
	public async Task Part1CalculatesCorrectly()
	{
		await Assert.That(Execute(ReadFileLines("inputSample.txt")).Item1).IsEqualTo(157);
	}

	[Test]
	public async Task Part2CalculatesCorrectly()
	{
		await Assert.That(Execute(ReadFileLines("inputSample.txt")).Item2).IsEqualTo(70);
	}
}
