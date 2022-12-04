namespace aoc_2022.Day04;

public class Problem
{
	public static (int, int) Main(string fileName)
	{
		var pairs = ReadFileLines(fileName, AssignmentPair.Parse);

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

	[Fact]
	public void PairsParseCorrectly()
	{
		Assert.Collection(ReadFileLines("inputSample.txt", AssignmentPair.Parse),
			a => { Assert.Equal(2, a.From1); Assert.Equal(4, a.To1); Assert.Equal(6, a.From2); Assert.Equal(8, a.To2); Assert.Equal(a.Range1, new[] { 2, 3, 4 }); Assert.Equal(a.Range2, new[] { 6, 7, 8 }); },
			a => { Assert.Equal(2, a.From1); Assert.Equal(3, a.To1); Assert.Equal(4, a.From2); Assert.Equal(5, a.To2); Assert.Equal(a.Range1, new[] { 2, 3 }); Assert.Equal(a.Range2, new[] { 4, 5 }); },
			a => { Assert.Equal(5, a.From1); Assert.Equal(7, a.To1); Assert.Equal(7, a.From2); Assert.Equal(9, a.To2); Assert.Equal(a.Range1, new[] { 5, 6, 7 }); Assert.Equal(a.Range2, new[] { 7, 8, 9 }); },
			a => { Assert.Equal(2, a.From1); Assert.Equal(8, a.To1); Assert.Equal(3, a.From2); Assert.Equal(7, a.To2); Assert.Equal(a.Range1, new[] { 2, 3, 4, 5, 6, 7, 8 }); Assert.Equal(a.Range2, new[] { 3, 4, 5, 6, 7 }); },
			a => { Assert.Equal(6, a.From1); Assert.Equal(6, a.To1); Assert.Equal(4, a.From2); Assert.Equal(6, a.To2); Assert.Equal(a.Range1, new[] { 6 }); Assert.Equal(a.Range2, new[] { 4, 5, 6 }); },
			a => { Assert.Equal(2, a.From1); Assert.Equal(6, a.To1); Assert.Equal(4, a.From2); Assert.Equal(8, a.To2); Assert.Equal(a.Range1, new[] { 2, 3, 4, 5, 6 }); Assert.Equal(a.Range2, new[] { 4, 5, 6, 7, 8 }); });
	}

	[Theory]
	[InlineData("inputSample.txt", 2)]
	[InlineData("input.txt", 556)]
	public void Part1CalculatesCorrectly(string fileName, int expectedPart1)
	{
		Assert.Equal(expectedPart1, Main(fileName).Item1);
	}

	[Theory]
	[InlineData("inputSample.txt", 4)]
	[InlineData("input.txt", 876)]
	public void Part2CalculatesCorrectly(string fileName, int expectedPart2)
	{
		Assert.Equal(expectedPart2, Main(fileName).Item2);
	}
}
