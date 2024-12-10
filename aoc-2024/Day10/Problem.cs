namespace AdventOfCode.Year2024.Day10;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		var heads = new List<Coordinate>(256); // plenty for actual input
		var map   = CreateMap(input, (x, y, c) => {
			if (c == '0') {
				heads.Add(new(x, y));
			}
			return c - 48;
		});
		var xMax = map.GetLength(0);
		var yMax = map.GetLength(1);

		var (part1, part2) = heads
			.AsParallel()
			.Select(h => ScoreTrail(h, map, xMax, yMax))
			.Aggregate((p1: 0, p2: 0), (tot, cur) => (tot.p1 + cur.Score, tot.p2 + cur.Rating));

		return (part1, part2);
	}

	private static (int Score, int Rating) ScoreTrail(Coordinate start, int[,] map, int xMax, int yMax)
	{
		var ends   = new HashSet<Coordinate>(32);
		var rating = 0;
		var queue  = new Queue<Coordinate>();

		queue.Enqueue(start);

		while (queue.Count > 0) {
			var thisStep = queue.Dequeue();

			foreach (var nextStep in NextSteps(thisStep, map, xMax, yMax)) {
				if (map.ValueAt(nextStep) == 9) {
					ends.Add(nextStep);
					rating++;
				} else {
					queue.Enqueue(nextStep);
				}
			}
		}

		return (ends.Count, rating);
	}

	private static IEnumerable<Coordinate> NextSteps(Coordinate from, int[,] map, int xMax, int yMax)
	{
		var thisHeight = map.ValueAt(from);

		// left
		if (from.X > 0 && map[from.X - 1, from.Y] == thisHeight + 1) {
			yield return new Coordinate(from.X - 1, from.Y);
		}

		// right
		if (from.X < xMax - 1 && map[from.X + 1, from.Y] == thisHeight + 1) {
			yield return new Coordinate(from.X + 1, from.Y);
		}

		// up
		if (from.Y > 0 && map[from.X, from.Y - 1] == thisHeight + 1) {
			yield return new Coordinate(from.X, from.Y - 1);
		}

		// down
		if (from.Y < yMax - 1 && map[from.X, from.Y + 1] == thisHeight + 1) {
			yield return new Coordinate(from.X, from.Y + 1);
		}
	}
}
