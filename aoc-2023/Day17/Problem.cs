namespace AdventOfCode.Year2023.Day17;

// https://www.reddit.com/r/adventofcode/comments/18l9mrh

public partial class Problem
{
	public static ProblemMetadata2 Metadata { get; } = new(Execute, null);

	private readonly static Coordinate _north = new(0, -1);
	private readonly static Coordinate _south = new(0, 1);
	private readonly static Coordinate _east = new(1, 0);
	private readonly static Coordinate _west = new(-1, 0);

	public static (long, long) Execute(string[] input)
	{
		//input = ReadFileLines("inputSample.txt");
		var map    = CreateMap(input, (x, y, c) => new MapNode(new Coordinate(x, y), int.Parse(c.ToString())));
		var width  = map.GetLength(0);
		var height = map.GetLength(1);
		var start  = map[0, 0];
		var goal   = map[map.GetLength(0) - 1, map.GetLength(1) - 1];

		Func<PathState, IEnumerable<(PathState, float)>> findAdjacentNodes = currentState => {
			var ((currentNode, _), currentDir, dirCount) = currentState;

			var north = currentDir == _south ? default(Coordinate?) : currentNode + _north;
			var south = currentDir == _north ? default(Coordinate?) : currentNode + _south;
			var east  = currentDir == _west ? default(Coordinate?) : currentNode + _east;
			var west  = currentDir == _east ? default(Coordinate?) : currentNode + _west;
			var ret   = new List<(PathState, float)>(4);

			// we can't go more than three steps in the same direction in a row
			if (north != null && currentDir == _north && dirCount >= 3) north = null;
			if (south != null && currentDir == _south && dirCount >= 3) south = null;
			if (east  != null && currentDir == _east  && dirCount >= 3) east  = null;
			if (west  != null && currentDir == _west  && dirCount >= 3) west  = null;

			// ensure we don't go off the edge of the map
			if (north != null && north.Value.Y < 0) north = null;
			if (south != null && south.Value.Y > height - 1) south = null;
			if (east != null && east.Value.X > width - 1) east = null;
			if (west != null && west.Value.X < 0) west = null;

			if (north != null) {
				var northNode = map[north.Value.X, north.Value.Y];
				ret.Add((new PathState(northNode, _north, currentDir == _north ? dirCount + 1 : 1), northNode.HeatLoss));
			}

			if (south != null) {
				var southNode = map[south.Value.X, south.Value.Y];
				ret.Add((new PathState(southNode, _south, currentDir == _south ? dirCount + 1 : 1), southNode.HeatLoss));
			}

			if (east != null) {
				var eastNode = map[east.Value.X, east.Value.Y];
				ret.Add((new PathState(eastNode, _east, currentDir == _east ? dirCount + 1 : 1), eastNode.HeatLoss));
			}

			if (west != null) {
				var westNode = map[west.Value.X, west.Value.Y];
				ret.Add((new PathState(westNode, _west, currentDir == _west ? dirCount + 1 : 1), westNode.HeatLoss));
			}

			return ret;
		};

		var comparer   = EqualityComparer<PathState>.Create((state1, state2) => state1.Node == state2.Node, state => state.Node.GetHashCode());
		var startState = new PathState(start, _east, 0);
		var goalState  = new PathState(goal, _north, -1);

		var path = AStar.FindPath(startState, goalState, findAdjacentNodes, (from, to) => Coordinate.ManhattanDistance(from.Node.Coordinates, to.Node.Coordinates), comparer);
		var part1 = path == null ? -1 : path.Skip(1).Sum(n => n.Node.HeatLoss);

		return (part1, 0);
	}

	private readonly record struct MapNode(Coordinate Coordinates, int HeatLoss);

	private readonly record struct PathState(MapNode Node, Coordinate Direction, int DirectionCount);
}
