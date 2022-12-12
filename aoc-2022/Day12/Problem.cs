using static AdventOfCode.AStar;

namespace aoc_2022.Day12;

public class Problem
{
	public static (int, int) Main(string fileName)
	{
		var (map, start, goal) = ParseMap(fileName);

		var part1 = (FindPath(start, goal, c => FindAdjacentNodes(map, c))?.Count ?? -1) - 1; // -1 because our algo includes the start
		var part2 = GetStartingPositions(map).Select(candidate => FindPath(candidate, goal, c => FindAdjacentNodes(map, c))?.Count).Where(i => i.HasValue).Min() - 1;

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
		var curElev = map[node.x, node.y];

		if (north.y >= 0 && map[north.x, north.y] <= curElev + 1) {
			yield return (north, 1f);
		}

		if (south.y < height && map[south.x, south.y] <= curElev + 1) {
			yield return (south, 1f);
		}

		if (east.x < width && map[east.x, east.y] <= curElev + 1) {
			yield return (east, 1f);
		}

		if (west.x >= 0 && map[west.x, west.y] <= curElev + 1) {
			yield return (west, 1f);
		}
	}

	private static (int[,] map, (int x, int y) start, (int x, int y) goal) ParseMap(string fileName)
	{
		var lines  = ReadFileLines(fileName);
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
