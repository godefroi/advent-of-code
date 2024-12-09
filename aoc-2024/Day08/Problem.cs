namespace AdventOfCode.Year2024.Day08;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		var antennas = FindAntennas(input);
		var xMax     = input[0].Length - 1;
		var yMax     = input.Length - 1;
		var part1    = Part1(antennas, xMax, yMax);
		var part2    = Part2(antennas, xMax, yMax);

		return (part1, part2);
	}

	private static int Part1(Dictionary<char, List<Coordinate>> allAntennas, int xMax, int yMax)
	{
		var nodes = new HashSet<Coordinate>();

		foreach (var (_, antennas) in allAntennas) {
			GenerateCombinations(antennas, antennas.Count, 2, (ants, idx, _) => {
				foreach (var node in FindAntinodes(ants[idx[0]], ants[idx[1]], xMax, yMax)) {
					nodes.Add(node);
				}
			});
		}

		return nodes.Count;
	}

	private static int Part2(Dictionary<char, List<Coordinate>> allAntennas, int xMax, int yMax)
	{
		var nodes = new HashSet<Coordinate>();

		foreach (var (_, antennas) in allAntennas) {
			GenerateCombinations(antennas, antennas.Count, 2, (ants, idx, _) => {
				foreach (var node in BresenhamExtended(ants[idx[0]], ants[idx[1]], xMax, yMax)) {
					nodes.Add(node);
				}
			});
		}

		return nodes.Count;
	}

	private static Dictionary<char, List<Coordinate>> FindAntennas(string[] input)
	{
		var ret = new Dictionary<char, List<Coordinate>>();

		for (var y = 0; y < input.Length; y++) {
			for (var x = 0; x < input[y].Length; x++) {
				var c = input[y][x];

				if (c == '.') {
					continue;
				}

				if (!ret.TryGetValue(c, out var list)) {
					ret.Add(c, list = []);
				}

				list.Add((x, y));
			}
		}

		return ret;
	}

	private static IEnumerable<Coordinate> FindAntinodes(Coordinate ant1, Coordinate ant2, int xMax, int yMax)
	{
		var node1 = ant1 + (ant1 - ant2);
		var node2 = ant2 + (ant2 - ant1);

		if (node1.X >= 0 && node1.X <= xMax && node1.Y >= 0 && node1.Y <= yMax) {
			yield return node1;
		}

		if (node2.X >= 0 && node2.X <= xMax && node2.Y >= 0 && node2.Y <= yMax) {
			yield return node2;
		}
	}

	private static IEnumerable<Coordinate> BresenhamExtended(Coordinate c1, Coordinate c2, int xMax, int yMax)
	{
		var slope = Slope(c1, c2);
		var diffX = Math.Abs(c1.X - c2.X);
		var diffY = Math.Abs(c1.Y - c2.Y);
		var stepX = c1.X < c2.X ? 1 : -1;
		var stepY = c1.Y < c2.Y ? 1 : -1;
		var err   = diffX - diffY;
		var curr  = c1;
		var set   = new HashSet<Coordinate>(64) { c1, c2 };

		yield return c1;
		yield return c2;

		for (var i = 0; i < 2; i++) {
			// run the algorithm in one direction
			while (true) {
				var e2 = err * 2;

				// adjust x
				if (e2 > -diffY) {
					err -= diffY;
					curr = new Coordinate(curr.X + stepX, curr.Y);
				}

				// adjust y
				if (e2 < diffX) {
					err += diffX;
					curr = new Coordinate(curr.X, curr.Y + stepY);
				}

				// don't go out of bounds
				if (curr.X < 0 || curr.Y < 0 || curr.X > xMax || curr.Y > yMax) {
					break;
				}

				if (Slope(c1, curr) == slope && set.Add(curr)) {
					yield return curr;
				}
			}

			// reset our variables for the second run
			err   = diffX - diffY;
			stepX = -stepX;
			stepY = -stepY;
			curr  = c1;
		}
	}

	private static double Slope(Coordinate p1, Coordinate p2) => ((double)p2.Y - p1.Y) / ((double)p2.X - p1.X);
}
