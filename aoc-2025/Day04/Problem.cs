namespace AdventOfCode.Year2025.Day04;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		var width  = input[0].Length;
		var height = input.Length;
		var bRolls = new bool[input.Length * input[0].Length];

		for (var y = 0; y < height; y++) {
			var row = input[y];
			for (var x = 0; x < width; x++) {
				bRolls[y * width + x] = row[x] == '@';
			}
		}

		var p1 = CountRemovable(bRolls, width, height);
		var p2 = 0;

		while (true) {
			var cp2 = p2;

			for (var y = 0; y < height; y++) {
				for (var x = 0; x < width; x++) {
					if (Removable(bRolls, width, height, x, y)) {
						p2++;
						bRolls[(y * width) + x] = false;
					}
				}
			}

			if (cp2 == p2) {
				break;
			}
		}

		return (p1, p2);
	}

	private static int CountRemovable(bool[] bRolls, int width, int height)
	{
		var cnt = 0;

		for (var y = 0; y < height; y++) {
			for (var x = 0; x < width; x++) {
				if (Removable(bRolls, width, height, x, y)) {
					cnt++;
				}
			}
		}

		return cnt;
	}

	private static bool Removable(bool[] bRolls, int width, int height, int x, int y)
	{
		var adjacentCount = 0;

		if (!bRolls[(y * width) + x]) {
			return false;
		}

		// check row above
		if (y > 0) {
			var above = (y - 1) * width;
			if ((adjacentCount += (x > 0         && bRolls[above + (x - 1)]) ? 1 : 0) > 3) { return false; }
			if ((adjacentCount += (                 bRolls[above + (x    )]) ? 1 : 0) > 3) { return false; }
			if ((adjacentCount += (x < width - 1 && bRolls[above + (x + 1)]) ? 1 : 0) > 3) { return false; }
		}

		// check this row
		var ourRow = y * width;
		if ((adjacentCount += (x > 0         && bRolls[ourRow + (x - 1)]) ? 1 : 0) > 3) { return false; }
		if ((adjacentCount += (x < width - 1 && bRolls[ourRow + (x + 1)]) ? 1 : 0) > 3) { return false; }

		// check row below
		if (y < height - 1) {
			var below = (y + 1) * width;
			if ((adjacentCount += (x > 0         && bRolls[below + (x - 1)]) ? 1 : 0) > 3) { return false; }
			if ((adjacentCount += (                 bRolls[below + (x    )]) ? 1 : 0) > 3) { return false; }
			if ((adjacentCount += (x < width - 1 && bRolls[below + (x + 1)]) ? 1 : 0) > 3) { return false; }
		}

		return true;
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
