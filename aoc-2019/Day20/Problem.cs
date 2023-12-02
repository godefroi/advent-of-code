using static AdventOfCode.AStar;

namespace aoc_2019.Day20;

public class Problem
{
	public static ProblemMetadata2 Metadata { get; } = new(Main);

	public static (long, long) Main(string[] input)
	{
		//fileName = "inputSample3.txt";

		var map        = CreateMap(input, c => c);
		var width      = map.GetLength(0);
		var height     = map.GetLength(1);
		var portals    = FindPortals(map, out var entrance, out var exit);
		var portalDict = portals.Values.SelectMany(c => new[] { (c[0], c[1]), (c[1], c[0]) }).ToDictionary(c => c.Item1, c => c.Item2);
		var moves      = new[] {
			new Coordinate(0, +1),
			new Coordinate(0, -1),
			new Coordinate(+1, 0),
			new Coordinate(-1, 0),
		};

		var part1 = (FindPath(entrance, exit, AdjacentNodesPart1, (f, t) => Coordinate.ManhattanDistance(f, t))?.Count ?? 0) - 1;
		var part2 = (FindPath((entrance, 0), (exit, 0), AdjacentNodesPart2, Part2Heuristic)?.Count ?? 0) - 1;

		return (part1, part2);

		IEnumerable<(Coordinate, float)> AdjacentNodesPart1(Coordinate from)
		{
			var validMoves = moves.Select(m => from + m).Where(c => map[c.X, c.Y] == '.');

			if (portalDict.TryGetValue(from, out var to)) {
				validMoves = validMoves.Append(to);
			}

			return validMoves.Select(c => (c, 1f));
		}

		IEnumerable<((Coordinate, int), float)> AdjacentNodesPart2((Coordinate location, int level) from)
		{
			var localMoves = moves.Select(m => from.location + m).Where(c => map[c.X, c.Y] == '.').Select(c => (c, from.level));

			if (portalDict.TryGetValue(from.location, out var to)) {
				if (from.location.X == 2 || from.location.X == width - 3 || from.location.Y == 2 || from.location.Y == height - 3) {
					// this is an outside portal, it goes up a level (level - 1)
					if (from.level > 0) {
						localMoves = localMoves.Append((to, from.level - 1));
					}
				} else {
					// this is an inside portal, it goes down a level (level + 1)
					localMoves = localMoves.Append((to, from.level + 1));
				}
			}

			return localMoves.Select(i => (i, 1f));
		}

		float Part2Heuristic((Coordinate location, int level) from, (Coordinate location, int level) to)
		{
			if (from.level == to.level) {
				return Coordinate.ManhattanDistance(from.location, to.location);
			} else {
				return Math.Abs(from.level - to.level) * 250; // 250 is a guess... there are probably better values?
			}
		}
	}

	private static Dictionary<string, Coordinate[]> FindPortals(char[,] map, out Coordinate entrance, out Coordinate exit)
	{
		// top and bottom portals read top-down
		// left and right portals read left-right

		var width   = map.GetLength(0);
		var height  = map.GetLength(1);
		var portals = new Dictionary<string, Coordinate[]>();

		var insideLeftEdge   = Enumerable.Range(0, width / 2).Select(x => (width / 2) - x).First(x => map[x, height / 2] == '.' || map[x, height / 2] == '#');
		var insideRightEdge  = Enumerable.Range(width / 2, width / 2).First(x => map[x, height / 2] == '.' || map[x, height / 2] == '#');
		var insideTopEdge    = Enumerable.Range(0, height / 2).Select(y => (height / 2) - y).First(y => map[width / 2, y] == '.' || map[width / 2, y] == '#');
		var insideBottomEdge = Enumerable.Range(height / 2, height / 2).First(y => map[width / 2, y] == '.' || map[width / 2, y] == '#');

		var topPortals = Enumerable.Range(3, width - 6).Select(x => (x, string.Concat(map[x, 0], map[x, 1])));
		var (en1, ex1) = HandlePortals(topPortals, p => new Coordinate(p.Item1, 2));

		var bottomPortals = Enumerable.Range(3, width - 6).Select(x => (x, string.Concat(map[x, height - 2], map[x, height - 1]))).Where(p => p.Item2 != "  ");
		var (en2, ex2)    = HandlePortals(bottomPortals, p => new Coordinate(p.Item1, height - 3));

		var leftPortals = Enumerable.Range(3, height - 6).Select(y => (y, string.Concat(map[0, y], map[1, y]))).Where(p => p.Item2 != "  ");
		var (en3, ex3)  = HandlePortals(leftPortals, p => new Coordinate(2, p.Item1));

		var rightPortals = Enumerable.Range(3, height - 6).Select(y => (y, string.Concat(map[width - 2, y], map[width - 1, y]))).Where(p => p.Item2 != "  ");
		var (en4, ex4)   = HandlePortals(rightPortals, p => new Coordinate(width - 3, p.Item1));

		var insideTopPortals = Enumerable.Range(insideLeftEdge + 2, insideRightEdge - insideLeftEdge - 3).Select(x => (x, string.Concat(map[x, insideTopEdge + 1], map[x, insideTopEdge + 2])));
		var (en5, ex5)       = HandlePortals(insideTopPortals, p => new Coordinate(p.Item1, insideTopEdge));

		var insideBottomPortals = Enumerable.Range(insideLeftEdge + 2, insideRightEdge - insideLeftEdge - 3).Select(x => (x, string.Concat(map[x, insideBottomEdge - 2], map[x, insideBottomEdge - 1])));
		var (en6, ex6)          = HandlePortals(insideBottomPortals, p => new Coordinate(p.Item1, insideBottomEdge));

		var insideLeftPortals = Enumerable.Range(insideTopEdge + 2, insideBottomEdge - insideTopEdge - 3).Select(y => (y, string.Concat(map[insideLeftEdge + 1, y], map[insideLeftEdge + 2, y])));
		var (en7, ex7)        = HandlePortals(insideLeftPortals, p => new Coordinate(insideLeftEdge, p.Item1));

		var insideRightPortals = Enumerable.Range(insideTopEdge + 2, insideBottomEdge - insideTopEdge - 3).Select(y => (y, string.Concat(map[insideRightEdge - 2, y], map[insideRightEdge - 1, y])));
		var (en8, ex8)         = HandlePortals(insideRightPortals, p => new Coordinate(insideRightEdge, p.Item1));

		entrance = new[] { en1, en2, en3, en4, en5, en6, en7, en8 }.First(c => c.X > -1 && c.Y > -1);
		exit     = new[] { ex1, ex2, ex3, ex4, ex5, ex6, ex7, ex8 }.First(c => c.X > -1 && c.Y > -1);

		return portals;

		(Coordinate entrance, Coordinate exit) HandlePortals(IEnumerable<(int, string)> portalList, Func<(int, string), Coordinate> calculateCoordinate)
		{
			var lEntrance = new Coordinate(-1, -1);
			var lExit     = new Coordinate(-1, -1);

			foreach (var portal in portalList.Where(p => p.Item2.Trim().Length == 2)) {
				if (portal.Item2 == "AA") {
					lEntrance = calculateCoordinate(portal);
				} else if (portal.Item2 == "ZZ") {
					lExit = calculateCoordinate(portal);
				} else if (portals.TryGetValue(portal.Item2, out var coords)) {
					coords[1] = calculateCoordinate(portal);
				} else {
					portals.Add(portal.Item2, new[] { calculateCoordinate(portal), new Coordinate(-1, -1) });
				}
			}

			return (lEntrance, lExit);
		}
	}
}
