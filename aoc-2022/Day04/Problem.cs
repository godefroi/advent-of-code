namespace AdventOfCode.Year2022.Day04;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		var pairs = input.Select(AssignmentPair.Parse).ToArray();

		var part1 = pairs.Count(p => p.Intersection.SequenceEqual(p.Range1) || p.Intersection.SequenceEqual(p.Range2));
		var part2 = pairs.Count(p => p.Intersection.Any());

		Console.WriteLine($"part 1: {part1}"); // 556
		Console.WriteLine($"part 2: {part2}"); // 876

		return (part1, part2);
	}

	private readonly record struct AssignmentPair(int From1, int To1, int From2, int To2)
	{
		private readonly IEnumerable<int> _range1       = Enumerable.Range(From1, To1 - From1 + 1).ToArray();
		private readonly IEnumerable<int> _range2       = Enumerable.Range(From2, To2 - From2 + 1).ToArray();
		private readonly IEnumerable<int> _intersection = Enumerable.Range(From1, To1 - From1 + 1).Intersect(Enumerable.Range(From2, To2 - From2 + 1)).ToArray();

		public IEnumerable<int> Range1 => _range1;

		public IEnumerable<int> Range2 => _range2;

		public IEnumerable<int> Intersection => _intersection;

		public static AssignmentPair Parse(string line)
		{
			var parts = line.Split('-', ',');

			return new AssignmentPair(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]), int.Parse(parts[3]));
		}
	}

	[Test]
	public async Task PairsParseCorrectly()
	{
		await Assert.That(ReadFileLines("inputSample.txt", AssignmentPair.Parse)).IsEquivalentTo([
			new AssignmentPair(2, 4, 6, 8),
			new AssignmentPair(2, 3, 4, 5),
			new AssignmentPair(5, 7, 7, 9),
			new AssignmentPair(2, 8, 3, 7),
			new AssignmentPair(6, 6, 4, 6),
			new AssignmentPair(2, 6, 4, 8),
		]);
	}

	[Test]
	[Arguments("inputSample.txt", 2)]
	[Arguments("input.txt", 556)]
	public async Task Part1CalculatesCorrectly(string fileName, int expectedPart1)
	{
		await Assert.That(Execute(ReadFileLines(fileName)).Item1).IsEqualTo(expectedPart1);
	}

	[Test]
	[Arguments("inputSample.txt", 4)]
	[Arguments("input.txt", 876)]
	public async Task Part2CalculatesCorrectly(string fileName, int expectedPart2)
	{
		await Assert.That(Execute(ReadFileLines(fileName)).Item2).IsEqualTo(expectedPart2);
	}
}
