namespace AdventOfCode.Year2023.Day17;

// https://www.reddit.com/r/adventofcode/comments/18l9mrh

public partial class Problem
{
	public static ProblemMetadata2 Metadata { get; } = new(Execute, null);

	public static (long, long) Execute(string[] input)
	{
		input = ReadFileLines("inputSample.txt");
		var map   = CreateMap(input, (x, y, c) => new MapNode(new Coordinate(x, y), int.Parse(c.ToString())));
		var start = map[0, 0];
		var goal  = map[map.GetLength(0) - 1, map.GetLength(1) - 1];

		Func<Stack<MapNode>, IEnumerable<(MapNode node, float weight)>> findAdjacentNodes = currentPath => {
			Coordinate? prev1     = null;
			Coordinate? forbidden = null;

			var mostRecentNodes = currentPath.TakeLast(4).Reverse().ToList();

			var currentNode = mostRecentNodes[0].Coordinates;
			var north       = currentNode + new Coordinate(0, -1);
			var south       = currentNode + new Coordinate(0, 1);
			var east        = currentNode + new Coordinate(1, 0);
			var west        = currentNode + new Coordinate(-1, 0);
			var ret         = new List<(MapNode node, float weight)>(4);

			// if we have at least one path node, we need it, because
			// we can never go backwards
			if (mostRecentNodes.Count > 1) {
				prev1 = mostRecentNodes[1].Coordinates;
			}

			// if we have at least two more, we need two of them, because
			// if they're in a straight line, we can't keep going in that
			// direction
			if (mostRecentNodes.Count >= 4) {
				var prev2 = mostRecentNodes[2].Coordinates;
				var prev3 = mostRecentNodes[3].Coordinates;
				var dir1  = currentNode - prev1;
				var dir2  = prev1 - prev2;
				var dir3  = prev2 - prev3;

				if ((dir1 == dir2) && (dir1 == dir3)) {
					forbidden = currentNode + dir1;
				}
			}

			// now, generate adjacent nodes, but never where we came from (prev1)
			// and never in the forbidden direction

			// north, maybe?
			if (north.Y >= 0 && north != forbidden && north != prev1) {
				var northNode = map[north.X, north.Y];
				ret.Add((northNode, northNode.HeatLoss));
			}

			// south, maybe?
			if (south.Y <= map.GetLength(1) - 1 && south != forbidden && south != prev1) {
				var southNode = map[south.X, south.Y];
				ret.Add((southNode, southNode.HeatLoss));
			}

			// east, maybe?
			if (east.X <= map.GetLength(0) - 1 && east != forbidden && east != prev1) {
				var eastNode = map[east.X, east.Y];
				ret.Add((eastNode, eastNode.HeatLoss));
			}

			// west, maybe?
			if (west.X >= 0 && west != forbidden && west != prev1) {
				var westNode = map[west.X, west.Y];
				ret.Add((westNode, westNode.HeatLoss));
			}

			// Console.WriteLine($"Nodes adjacent to {currentNode} (came from {prev1}):");
			// foreach (var (node, _) in ret) {
			// 	Console.WriteLine($"\t{node.Coordinates}");
			// }
			// Console.ReadKey(true);

			return ret;
		};

		var path = AStar.FindPath(start, goal, findAdjacentNodes, (from, to) => /*Coordinate.ManhattanDistance(from.Coordinates, to.Coordinates)*/0, EqualityComparer<MapNode>.Default);
		var part1 = path == null ? -1 : path.Sum(n => n.HeatLoss);

		return (part1, 0);
	}

	private readonly record struct MapNode(Coordinate Coordinates, int HeatLoss);
}
