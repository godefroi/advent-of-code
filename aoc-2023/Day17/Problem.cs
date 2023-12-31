namespace AdventOfCode.Year2023.Day17;

// https://www.reddit.com/r/adventofcode/comments/18l9mrh

public partial class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, null);

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

		Func<PathState, IEnumerable<(PathState, float)>> findAdjacentNodes1 = currentState => FindAdjacentNodesPart1(map, width, height, currentState);
		Func<PathState, IEnumerable<(PathState, float)>> findAdjacentNodes2 = currentState => FindAdjacentNodesPart2(map, width, height, currentState);
		Func<PathState, PathState, float> distanceHeuristic = (from, to) => Coordinate.ManhattanDistance(from.Node.Coordinates, to.Node.Coordinates);

		var comparer   = EqualityComparer<PathState>.Create((state1, state2) => state1.Node == state2.Node, state => state.Node.GetHashCode());
		var startState = new PathState(start, _east, 0);
		var goalState  = new PathState(goal, _north, -1);

		var path1 = AStar.FindPath(startState, goalState, findAdjacentNodes1, distanceHeuristic, comparer);
		var part1 = path1 == null ? -1 : path1.Skip(1).Sum(n => n.Node.HeatLoss);

		var path2 = AStar.FindPath(startState, goalState, findAdjacentNodes2, distanceHeuristic, comparer);
		var part2 = path2 == null ? -1 : path2.Skip(1).Sum(n => n.Node.HeatLoss);

		return (part1, part2);
	}

	private static List<(PathState, float)> FindAdjacentNodesPart1(MapNode[,] map, int mapWidth, int mapHeight, PathState currentState)
	{
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
		if (south != null && south.Value.Y > mapHeight - 1) south = null;
		if (east != null && east.Value.X > mapWidth - 1) east = null;
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
	}

	private static List<(PathState, float)> FindAdjacentNodesPart2(MapNode[,] map, int mapWidth, int mapHeight, PathState currentState)
	{
		var ((currentNode, _), currentDir, dirCount) = currentState;

		var ret = new List<(PathState, float)>(4);

		// ultra crucible must travel at least 4 nodes before turning
		if (dirCount < 4) {
			var nextCoordinate = currentNode + currentDir;

			// only add this node if it's not off the edge of the map
			if (nextCoordinate.X >= 0 && nextCoordinate.X <= mapWidth - 1 && nextCoordinate.Y >= 0 && nextCoordinate.Y <= mapHeight - 1) {
				var nextNode = map[nextCoordinate.X, nextCoordinate.Y];
				ret.Add((new PathState(nextNode, currentDir, dirCount + 1), nextNode.HeatLoss));
			}

			return ret;
		}

		var north = currentDir == _south ? default(Coordinate?) : currentNode + _north;
		var south = currentDir == _north ? default(Coordinate?) : currentNode + _south;
		var east  = currentDir == _west ? default(Coordinate?) : currentNode + _east;
		var west  = currentDir == _east ? default(Coordinate?) : currentNode + _west;

		// we can't go more than 10 steps in the same direction in a row
		if (north != null && currentDir == _north && dirCount >= 10) north = null;
		if (south != null && currentDir == _south && dirCount >= 10) south = null;
		if (east  != null && currentDir == _east  && dirCount >= 10) east  = null;
		if (west  != null && currentDir == _west  && dirCount >= 10) west  = null;

		// ensure we don't go off the edge of the map
		if (north != null && north.Value.Y < 0) north = null;
		if (south != null && south.Value.Y > mapHeight - 1) south = null;
		if (east != null && east.Value.X > mapWidth - 1) east = null;
		if (west != null && west.Value.X < 0) west = null;

		// we need to have been travelling in the same direction at least 4 nodes to be able to stop
		if (north != null && north.Value.X == mapWidth - 1 && north.Value.Y == mapHeight - 1 && dirCount < 4) north = null;
		if (south != null && south.Value.X == mapWidth - 1 && south.Value.Y == mapHeight - 1 && dirCount < 4) south = null;
		if (east  != null && east.Value.X  == mapWidth - 1 && east.Value.Y  == mapHeight - 1 && dirCount < 4) east  = null;
		if (west  != null && west.Value.X  == mapWidth - 1 && west.Value.Y  == mapHeight - 1 && dirCount < 4) west  = null;

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
	}

	private readonly record struct MapNode(Coordinate Coordinates, int HeatLoss);

	private readonly record struct PathState(MapNode Node, Coordinate Direction, int DirectionCount);
}
