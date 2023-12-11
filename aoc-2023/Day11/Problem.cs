namespace AdventOfCode.Year2023.Day11;

public partial class Problem
{
	public static ProblemMetadata2 Metadata { get; } = new(Execute, null);

	public static (long, long) Execute(string[] input)
	{
		var galaxies = new List<Coordinate>(256);

		for (var y = 0; y < input.Length; y++) {
			var lineSpan = input[y].AsSpan();

			for (var x = 0; x < lineSpan.Length; x++) {
				if (lineSpan[x] == '#') {
					galaxies.Add(new Coordinate(x, y));
				}
			}
		}

		var part1Galaxies = ExpandUniverse(galaxies, input[0].Length, input.Length, 1);
		var part2Galaxies = ExpandUniverse(galaxies, input[0].Length, input.Length, 999999);

		var part1 = part1Galaxies.Combinations(2).Sum(c => Coordinate.ManhattanDistance(c.First(), c.Last()));
		var part2 = part2Galaxies.Combinations(2).Sum(c => (long)Coordinate.ManhattanDistance(c.First(), c.Last()));

		return (part1, part2);
	}

	private static List<Coordinate> ExpandUniverse(List<Coordinate> galaxies, int width, int height, int expansion)
	{
		var ret = new List<Coordinate>(galaxies);

		for (var x = width - 2; x >= 0; x--) {
			// don't expand columns with galaxies
			if (ret.Any(g => g.X == x)) {
				continue;
			}

			for (var i = 0; i < ret.Count; i++) {
				if (ret[i].X > x) {
					var shiftedGalaxy = ret[i];
					ret[i] = new Coordinate(shiftedGalaxy.X + expansion, shiftedGalaxy.Y);
				}
			}
		}

		for (var y = height - 2; y >= 0; y--) {
			// don't expand rows with galaxies
			if (ret.Any(g => g.Y == y)) {
				continue;
			}

			for (var i = 0; i < ret.Count; i++) {
				if (ret[i].Y > y) {
					var shiftedGalaxy = ret[i];
					ret[i] = new Coordinate(shiftedGalaxy.X, shiftedGalaxy.Y + expansion);
				}
			}
		}

		return ret;
	}
}