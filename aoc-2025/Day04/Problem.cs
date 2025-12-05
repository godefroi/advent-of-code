namespace AdventOfCode.Year2025.Day04;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		var rolls = input
			.Index()
			.SelectMany(row => row.Item
				.Index()
				.Where(c => c.Item == '@')
				.Select(col => (col.Index, row.Index)))
			.ToHashSet();

		var removable = FindRemovable(rolls);
		var p1        = removable.Count;
		var p2        = 0;

		while (removable.Count > 0) {
			// count the ones we're going to remove
			p2 += removable.Count;

			// remove them
			foreach (var rr in removable) {
				rolls.Remove(rr);
			}

			// get the next list of removable rolls
			removable = FindRemovable(rolls);
		}

		return (p1, p2);
	}

	private static List<Coordinate> FindRemovable(HashSet<(int X, int Y)> rolls)
	{
		return [.. rolls.Where(r => {
			var adacentCount = 0;

			if ((adacentCount += rolls.Contains((r.X - 1, r.Y - 1)) ? 1 : 0) > 3) return false;
			if ((adacentCount += rolls.Contains((r.X    , r.Y - 1)) ? 1 : 0) > 3) return false;
			if ((adacentCount += rolls.Contains((r.X + 1, r.Y - 1)) ? 1 : 0) > 3) return false;
			if ((adacentCount += rolls.Contains((r.X - 1, r.Y    )) ? 1 : 0) > 3) return false;
			if ((adacentCount += rolls.Contains((r.X + 1, r.Y    )) ? 1 : 0) > 3) return false;
			if ((adacentCount += rolls.Contains((r.X - 1, r.Y + 1)) ? 1 : 0) > 3) return false;
			if ((adacentCount += rolls.Contains((r.X    , r.Y + 1)) ? 1 : 0) > 3) return false;
			if ((adacentCount += rolls.Contains((r.X + 1, r.Y + 1)) ? 1 : 0) > 3) return false;

			return true;
		})];
	}

	public class Day04Tests
	{
		private readonly string[] _sampleData = """
			..@@.@@@@.
			@@@.@.@.@@
			@@@@@.@.@@
			@.@@@@..@.
			@@.@@@@.@@
			.@@@@@@@.@
			.@.@.@.@@@
			@.@@@.@@@@
			.@@@@@@@@.
			@.@.@@@.@.
			""".Split("\r\n");

		[Test]
		public async Task Part1Works()
		{
			await Assert.That(Execute(_sampleData).Item1).IsEqualTo(13);
		}

		[Test]
		public async Task Part2Works()
		{
			await Assert.That(Execute(_sampleData).Item2).IsEqualTo(43);
		}
	}
}
