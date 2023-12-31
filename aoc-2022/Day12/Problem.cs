using static AdventOfCode.AStar;

namespace aoc_2022.Day12;

public class Problem
{
	public static ProblemMetadata2 Metadata { get; } = new(Main, typeof(Problem));

	public static (long, long) Main(string[] input)
	{
		var (map, start, goal) = ParseMap(input);

		var part1 = (FindPath<Coordinate>(start, goal, c => FindAdjacentNodes(map, c), (f, t) => Coordinate.ManhattanDistance(f, t))?.Count ?? -1) - 1; // -1 because our algo includes the start
		var part2 = GetStartingPositions(map).Select(candidate => FindPath<Coordinate>(candidate, goal, c => FindAdjacentNodes(map, c), (f, t) => Coordinate.ManhattanDistance(f, t))?.Count).Where(i => i.HasValue).Min() - 1;

		return (part1, part2 ?? int.MaxValue);
	}

	private static IEnumerable<Coordinate> GetStartingPositions(int[,] map)
	{
		var width  = map.GetLength(0);
		var height = map.GetLength(1);

		for (var x = 0; x < width; x++) {
			for (var y = 0; y < height; y++) {
				if (map[x, y] == 0) {
					yield return new Coordinate(x, y);
				}
			}
		}
	}

	private static IEnumerable<(Coordinate, float)> FindAdjacentNodes(int[,] map, Coordinate node)
	{
		var north   = node + (0, -1);
		var south   = node + (0, 1);
		var east    = node + (1, 0);
		var west    = node + (-1, 0);
		var width   = map.GetLength(0);
		var height  = map.GetLength(1);
		var curElev = map[node.X, node.Y];

		if (north.Y >= 0 && map[north.X, north.Y] <= curElev + 1) {
			yield return (north, 1f);
		}

		if (south.Y < height && map[south.X, south.Y] <= curElev + 1) {
			yield return (south, 1f);
		}

		if (east.X < width && map[east.X, east.Y] <= curElev + 1) {
			yield return (east, 1f);
		}

		if (west.X >= 0 && map[west.X, west.Y] <= curElev + 1) {
			yield return (west, 1f);
		}
	}

	private static (int[,] map, (int x, int y) start, (int x, int y) goal) ParseMap(string[] lines)
	{
		var width  = lines[0].Length;
		var height = lines.Length;
		var map    = new int[width, height];

		(int, int) start = (-1, -1), goal = (-1, -1);

		for (var x = 0; x < width; x++) {
			for (var y = 0; y < height; y++) {
				if (lines[y][x] == 'S') {
					start = (x, y);
					map[x, y] = 0;
				} else if (lines[y][x] == 'E') {
					goal = (x, y);
					map[x, y] = 25;
				} else {
					map[x, y] = lines[y][x] - 'a';
				}
			}
		}

		return (map, start, goal);
	}
}
