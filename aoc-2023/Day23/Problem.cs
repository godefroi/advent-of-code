namespace AdventOfCode.Year2023.Day23;

public partial class Problem
{
	public static ProblemMetadata2 Metadata { get; } = new(Execute, null);

	private static readonly Coordinate _north = new(0, -1);
	private static readonly Coordinate _south = new(0, +1);
	private static readonly Coordinate _east = new(+1, 0);
	private static readonly Coordinate _west = new(-1, 0);

	public static (long, long) Execute(string[] input)
	{
		var p1map = CreateMap(input, c => c);
		var p2map = CreateMap(input, c => c switch { '#' => '#', _ => '.' });

		return (FindLongestPath(p1map), FindLongestPath(p2map));
	}

	private static int FindLongestPath(char[,] map)
	{
		// find all the nodes in the map
		var nodes        = FindNodes(map).ToList();
		var nodeSet      = nodes.Select(n => n.Node).ToHashSet();
		var entranceNode = nodes[0].Node;
		var exitNode     = nodes[1].Node;

		// use the list of nodes with exits to build an adjacency list
		var adjacencyList = nodes.ToDictionary(t => t.Node, t => new List<Edge>());

		// link up all the exits
		foreach (var (node, exits) in nodes) {
			//Console.WriteLine(node);

			foreach (var exit in exits) {
				var edge = ExplorePath(map, node, exit, nodeSet);

				// if this edge doesn't connect, it's an un-walkable path
				if (edge.To == Coordinate.Empty) {
					continue;
				}

				//Console.WriteLine($"  {exit} heads to {edge.To} in {edge.Length} steps");
				adjacencyList[node].Add(edge);
			}
		}

		// state consists of (current_node, visited_nodes, path_length)
		// whenever we visit a new node, we explore all connected nodes that aren't
		// in visited_nodes
		Stack<State> pendingStates = new();

		var maxPath = 0;

		// our initial state is at the entrance node
		pendingStates.Push(new State(entranceNode, new[] { entranceNode }, 0));

		// this is NP-hard, if the input is adversarial, this could run until the heat death of the universe...
		while (pendingStates.TryPop(out var thisState)) {
			if (thisState.CurrentNode == exitNode) {
				//Console.WriteLine($"reached the exit in {thisState.StepCount} steps");
				maxPath = Math.Max(maxPath, thisState.StepCount);
				continue;
			}

			var possibleEdges = adjacencyList[thisState.CurrentNode].Where(e => !thisState.VisitedNodes.Contains(e.To));

			foreach (var edge in possibleEdges) {
				//Console.WriteLine($"{thisState.CurrentNode} -> {edge.To} ({edge.Length} for a total of {edge.Length + thisState.StepCount})");
				pendingStates.Push(new State(edge.To, thisState.VisitedNodes.Append(edge.To), thisState.StepCount + edge.Length));
			}
		}

		return maxPath;
	}

	private static IEnumerable<(Coordinate Node, List<Coordinate> Exits)> FindNodes(char[,] map)
	{
		var entrance = new Coordinate(1, 0);
		var exit     = new Coordinate(map.GetLength(0) - 2, map.GetLength(1) - 1);

		// entrance node
		yield return (entrance, GetAdjacentPoints(map, entrance, entrance));

		// exit node
		yield return (exit, GetAdjacentPoints(map, exit, exit));

		// everything else
		for (var y = 1; y < map.GetLength(1) - 2; y++) {
			for (var x = 1; x < map.GetLength(0) - 2; x++) {
				// if it's not a path, it definitely isn't a node
				if (map[x, y] == '#') {
					continue;
				}

				var point     = new Coordinate(x, y);
				var adjacents = GetAdjacentPoints(map, point);

				// if it has two exits, it's a normal path
				if (adjacents.Count == 2) {
					continue;
				}

				// otherwise,  it's a node
				yield return ((x, y), GetExits(adjacents, point));
			}
		}
	}

	private static List<(Coordinate Direction, Coordinate Point, char PointChar)> GetAdjacentPoints(char[,] map, Coordinate adjacentTo)
	{
		var ret   = new List<(Coordinate, Coordinate, char)>(4);
		var north = adjacentTo + _north;
		var south = adjacentTo + _south;
		var east  = adjacentTo + _east;
		var west  = adjacentTo + _west;

		if (north.Y >= 0) {
			var northChar = map[north.X, north.Y];
			if (northChar != '#') {
				ret.Add((_north, north, northChar));
			}
		}

		if (south.Y < map.GetLength(1)) {
			var southChar = map[south.X, south.Y];
			if (southChar != '#') {
				ret.Add((_south, south, southChar));
			}
		}

		if (east.X < map.GetLength(0)) {
			var eastChar = map[east.X, east.Y];
			if (eastChar != '#') {
				ret.Add((_east, east, eastChar));
			}
		}

		if (west.X >= 0) {
			var westChar  = map[west.X, west.Y];
			if (westChar != '#') {
				ret.Add((_west, west, westChar));
			}
		}

		return ret;
	}

	private static List<Coordinate> GetExits(List<(Coordinate Direction, Coordinate Point, char PointChar)> adjacentPoints, Coordinate exclude)
	{
		var ret = new List<Coordinate>(4);

		foreach (var (dir, exit, pchar) in adjacentPoints) {
			if (dir == _north && pchar != 'v' && exit != exclude) {
				ret.Add(exit);
			}

			if (dir == _south && pchar != '^' && exit != exclude) {
				ret.Add(exit);
			}

			if (dir == _east && pchar != '<' && exit != exclude) {
				ret.Add(exit);
			}

			if (dir == _west && pchar != '>' && exit != exclude) {
				ret.Add(exit);
			}
		}

		return ret;
	}

	private static List<Coordinate> GetAdjacentPoints(char[,] map, Coordinate current, Coordinate exclude)
	{
		var ret   = new List<Coordinate>(4);
		var north = current + _north;
		var south = current + _south;
		var east  = current + _east;
		var west  = current + _west;

		if (north.Y >= 0) {
			var northChar = map[north.X, north.Y];
			if (northChar != '#' && northChar != 'v') {
				ret.Add(north);
			}
		}

		if (south.Y < map.GetLength(1)) {
			var southChar = map[south.X, south.Y];
			if (southChar != '#' && southChar != '^') {
				ret.Add(south);
			}
		}

		if (east.X < map.GetLength(0)) {
			var eastChar = map[east.X, east.Y];
			if (eastChar != '#' && eastChar != '<') {
				ret.Add(east);
			}
		}

		if (west.X >= 0) {
			var westChar  = map[west.X, west.Y];
			if (westChar != '#' && westChar != '>') {
				ret.Add(west);
			}
		}

		ret.Remove(exclude);

		return ret;
	}

	/// <summary>
	/// Starting at <paramref name="startFrom"/>, travel down the path leaving at <paramref name="firstStep"/> and travel until a crossroads is found
	/// </summary>
	private static Edge ExplorePath(char[,] map, Coordinate startFrom, Coordinate firstStep, HashSet<Coordinate> nodes)
	{
		var pcnt   = 1;
		var curc   = firstStep;
		var ignore = startFrom;

		while (true) {
			var paths = GetAdjacentPoints(map, curc, ignore);

			pcnt++;

			if (paths.Count == 0) {
				// no paths out from here; we cannot go uphill
				return new Edge(Coordinate.Empty, Coordinate.Empty, -1);
			} else if (paths.Count == 1) {
				ignore = curc;
				curc   = paths.Single();

				if (nodes.Contains(curc)) {
					// we found a node
					return new Edge(startFrom, curc, pcnt);
				}
			} else {
				// this spot is a crossroads
				return new Edge(startFrom, curc, pcnt);
			}
		}
	}

	private readonly record struct Edge(Coordinate From, Coordinate To, int Length);

	private record class Node(Coordinate Location, List<Edge> Exits);

	private class State
	{
		public Coordinate CurrentNode { get; set; }

		public HashSet<Coordinate> VisitedNodes { get; } = [];

		public int StepCount { get; set; } = 0;

		public State(Coordinate currentNode, IEnumerable<Coordinate> visitedNodes, int stepCount)
		{
			CurrentNode  = currentNode;
			VisitedNodes = new HashSet<Coordinate>(visitedNodes);
			StepCount    = stepCount;
		}
	}

	public class Tests
	{
		[Fact]
		public void ExplorePathWorks()
		{
			var map  = CreateMap(ReadFileLines("inputSample.txt"), c => c);
			var nodes = FindNodes(map).Select(n => n.Node).ToHashSet();

			// explore from the starting position
			var (start, end, steps) = ExplorePath(map, (1, 0), (1, 0), nodes);

			Assert.Equal(new Coordinate(1, 0), start);
			Assert.Equal(new Coordinate(3, 5), end);
			Assert.Equal(16, steps);
		}
	}
}
