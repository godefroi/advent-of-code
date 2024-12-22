namespace AdventOfCode.Year2024.Day20;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		var start = Coordinate.Empty;
		var end   = Coordinate.Empty;
		var map   = CreateMap(input, (x, y, c) => {
			if (c == 'S') {
				start = (x, y);
				c = '.';
			} else if (c == 'E') {
				end = (x, y);
				c = '.';
			};

			return c;
		});

		var route = AStar.FindPath(start, end, curPos => FindAdjacents(map, curPos), (from, to) => Coordinate.ManhattanDistance(from, to)) ?? throw new Exception("Unable to find route");

		// route includes start *and* end
		var part1 = FindPotentialCheats(route, 2).Count();
		var part2 = FindPotentialCheats(route, 20).Count();

		return (part1, part2);
	}

	private static IEnumerable<(Coordinate, float)> FindAdjacents(char[,] map, Coordinate curPos)
	{
		var xMax  = map.GetLength(0) - 1;
		var yMax  = map.GetLength(1) - 1;
		var up    = curPos + new Coordinate( 0, -1);
		var down  = curPos + new Coordinate( 0, +1);
		var left  = curPos + new Coordinate(-1,  0);
		var right = curPos + new Coordinate(+1,  0);

		if (curPos.Y > 0 && map.ValueAt(up) == '.') {
			yield return (up, 1);
		}

		if (curPos.Y < yMax && map.ValueAt(down) == '.') {
			yield return (down, 1);
		}

		if (curPos.X > 0 && map.ValueAt(left) == '.') {
			yield return (left, 1);
		}

		if (curPos.X < xMax && map.ValueAt(right) == '.') {
			yield return (right, 1);
		}
	}

	private static IEnumerable<(Coordinate, Coordinate, int)> FindPotentialCheats(IEnumerable<Coordinate> route, int cheatLength)
	{
		var routeList = route.ToList();
		var comparer  = EqualityComparer<(Coordinate, Coordinate)>.Create(
			(p1, p2) => (p1.Item1 == p2.Item1 && p1.Item2 == p2.Item2) || (p1.Item1 == p2.Item2 && p1.Item2 == p2.Item1),
			p => {
				if (p.Item1.X < p.Item2.X) {
					return HashCode.Combine(p.Item1, p.Item2);
				} else if (p.Item1.X > p.Item2.X) {
					return HashCode.Combine(p.Item2, p.Item1);
				} else if (p.Item1.Y < p.Item2.Y) {
					return HashCode.Combine(p.Item1, p.Item2);
				} else if (p.Item1.Y > p.Item2.Y) {
					return HashCode.Combine(p.Item2, p.Item1);
				} else {
					throw new InvalidOperationException("comparing equal??");
				}
			});
		var pairSet   = new HashSet<(Coordinate, Coordinate)>(comparer);

		for (var i = 0; i < routeList.Count; i++) {
			for (var j = 0; j < routeList.Count; j++) {
				// skip pairs that are too close to benefit from cheating
				if (Math.Abs(i - j) <= cheatLength) {
					continue;
				}

				// a cheat can only go two spaces
				if (Coordinate.ManhattanDistance(routeList[i], routeList[j]) > cheatLength) {
					continue;
				}

				var pair = (routeList[i], routeList[j]);

				if (pairSet.Add(pair)) {
					var sdist = Math.Abs(i - j);
					var mdist = Coordinate.ManhattanDistance(pair.Item1, pair.Item2);
					//Console.WriteLine($"step dist: {sdist} man dist: {mdist} savings: {sdist - mdist} {pair}");

					if (sdist - mdist >= 100) {
						yield return (pair.Item1, pair.Item2, sdist - mdist);
					}
				}
			}
		}
	}
}
