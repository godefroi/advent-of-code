namespace AdventOfCode.Year2024.Day18;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, string) Execute(string[] input)
	{
		var (xMax, yMax) = input.Length switch {
			25 => (6, 6),
			3450 => (70, 70),
			_ => throw new ArgumentException($"Invalid input length: {input.Length}"),
		};

		var points    = Parse(input);
		var p1Points  = points.Take(1024).ToHashSet();
		var start     = new Coordinate(0, 0);
		var end       = new Coordinate(xMax, yMax);
		var heuristic = (Coordinate x, Coordinate y) => (float)Coordinate.ManhattanDistance(x, y);
		var part1     = (AStar.FindPath(start, end, p => FindAdjacents(p, xMax, yMax, p1Points), heuristic)?.Count ?? throw new InvalidOperationException("No path found")) - 1;
		var part2     = Coordinate.Empty;

		// TODO: this is brain-dead; instead, we should do a binary search
		for (var i = 1025; i < points.Length; i++) {
			var keepOut = points.Take(i).ToHashSet();

			if (AStar.FindPath(start, end, p => FindAdjacents(p, xMax, yMax, keepOut), heuristic) == null) {
				part2 = points[i - 1];
				break;
			}
		}

		return (part1, part2.ToString());
	}

	private static IEnumerable<(Coordinate, float)> FindAdjacents(Coordinate curPos, int xMax, int yMax, HashSet<Coordinate> keepOut)
	{
		var left  = curPos + (-1,  0);
		var right = curPos + (+1,  0);
		var up    = curPos + ( 0, -1);
		var down  = curPos + ( 0, +1);

		if (left.X >= 0 && !keepOut.Contains(left)) {
			yield return (left, 1);
		}

		if (right.X <= xMax && !keepOut.Contains(right)) {
			yield return (right, 1);
		}

		if (up.Y >= 0 && !keepOut.Contains(up)) {
			yield return (up, 1);
		}

		if (down.Y <= yMax && !keepOut.Contains(down)) {
			yield return (down, 1);
		}
	}

	private static Coordinate[] Parse(string[] input)
	{
		Span<Range> ranges = stackalloc Range[2];

		var ret = new Coordinate[input.Length];

		for (var i = 0; i < input.Length; i++) {
			var inputSpan = input[i].AsSpan();

			inputSpan.Split(ranges, ',');

			ret[i] = new(int.Parse(inputSpan[ranges[0]]), int.Parse(inputSpan[ranges[1]]));
		}

		return ret;
	}
}
