using System.Diagnostics;

using Region = System.Collections.Generic.HashSet<AdventOfCode.Coordinate>;

namespace AdventOfCode.Year2024.Day12;

public class Problem
{
	private static readonly Coordinate _left = new(-1, 0);
	private static readonly Coordinate _right = new(1, 0);
	private static readonly Coordinate _up = new(0, -1);
	private static readonly Coordinate _down = new(0, 1);
	private static readonly Edge[] _adjacents = [
		new(_left, Direction.Left),
		new(_right, Direction.Right),
		new(_up, Direction.Up),
		new(_down, Direction.Down),
	];

	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		var map     = CreateMap(input, c => c);
		var xMax    = map.GetLength(0);
		var yMax    = map.GetLength(1);
		var regions = new List<Region>();
		var cDict   = new Dictionary<Coordinate, Region>();

		for (var x = 0; x < xMax; x++) {
			for (var y = 0; y < yMax; y++) {
				var coord = new Coordinate(x, y);

				// if this coordinate already exists in a region, skip it
				if (cDict.ContainsKey(coord)) {
					continue;
				}

				// find the region this coordinate exists in
				var region = FindRegion(map, xMax, yMax, coord);

				// keep track of all the coordinate->region associations
				foreach (var c in region) {
					cDict.Add(c, region);
				}

				// remember this region
				regions.Add(region);
			}
		}

		var (part1, part2) = regions
			.Select(r => { var (p, c) = FindPerimeter(r, cDict); return (Perimeter: r.Count * p, Corners: r.Count * c); })
			.Aggregate((Perimiter: 0, Corners: 0), (tot, cur) => (tot.Perimiter + cur.Perimeter, tot.Corners + cur.Corners));

		return (part1, part2);
	}

	private static Region FindRegion(char[,] map, int xMax, int yMax, Coordinate from)
	{
		var region  = new Region();
		var pending = new Queue<Coordinate>();
		var thisVal = map.ValueAt(from);

		pending.Enqueue(from);

		while (pending.Count > 0) {
			var cur = pending.Dequeue();

			if (map.ValueAt(cur) != thisVal) {
				continue;
			}

			if (!region.Add(cur)) {
				continue;
			}

			if (cur.X > 0) {
				pending.Enqueue(new Coordinate(cur.X - 1, cur.Y));
			}

			if (cur.X < xMax - 1) {
				pending.Enqueue(new Coordinate(cur.X + 1, cur.Y));
			}

			if (cur.Y > 0) {
				pending.Enqueue(new Coordinate(cur.X, cur.Y - 1));
			}

			if (cur.Y < yMax - 1) {
				pending.Enqueue(new Coordinate(cur.X, cur.Y + 1));
			}
		}

		return region;
	}

	private static (int Perimeter, int Corners) FindPerimeter(Region region, Dictionary<Coordinate, Region> regions)
	{
		var edges = region
			.SelectMany(coordinate => _adjacents.Select(a => (RegionCoordinate: coordinate, AdjacentCoordinate: coordinate + a.Coordinate, a.Direction)))
			.Where(edge => !regions.TryGetValue(edge.AdjacentCoordinate, out var otherRegion) || otherRegion != region)
			.Select(edge => new Edge(edge.RegionCoordinate, edge.Direction));

		var edgeList        = new LinkedList<Edge>(edges);
		var perimeterLength = edges.Count();
		var cornerCount     = 0;

		while (edgeList.Count > 0) {
			// take an edge (any edge)
			var firstNode = edgeList.First ?? throw new InvalidOperationException("Cannot handle null nodes in edge list");
			var startEdge = firstNode.Value;
			var edge      = startEdge;

			// follow it around
			do {
				var (nextEdge, corner) = FollowEdge(edge, edgeList);

				cornerCount += corner ? 1 : 0;
				edge = nextEdge;
			} while (edge != startEdge);
		}

		return (perimeterLength, cornerCount);
	}

	private static (Edge NextEdge, bool Corner) FollowEdge(Edge edge, LinkedList<Edge> edgeList)
	{
		var (no_corner, right_turn, left_turn) = edge.Direction switch {
			// if the edge is to the left, then we look for:
			// - above with direction left -> no corner
			// - same with direction up -> corner (right turn)
			// - above+left with direction down -> corner (left turn)
			Direction.Left => (new Edge(edge.Coordinate + _up, Direction.Left), new Edge(edge.Coordinate, Direction.Up), new Edge(edge.Coordinate + _up + _left, Direction.Down)),

			// if the edge is to the top, then we look for:
			// - right with direction up -> no corner
			// - same with direction right -> corner (right turn)
			// - right+up with direction left -> corner (left turn)
			Direction.Up => (new Edge(edge.Coordinate + _right, Direction.Up), new Edge(edge.Coordinate, Direction.Right), new Edge(edge.Coordinate + _up + _right, Direction.Left)),

			// if the edge is to the right, then we look for:
			// - down with direction right -> no corner
			// - same with direction down -> corner (right turn)
			// - right+down with direction up -> corner (left turn)
			Direction.Right => (new Edge(edge.Coordinate + _down, Direction.Right), new Edge(edge.Coordinate, Direction.Down), new Edge(edge.Coordinate + _down + _right, Direction.Up)),

			// if the edge is to the bottom, then we look for:
			// - left with direction down -> no corner
			// - same with direction left -> corner (right turn)
			// - left+down with direction right -> corner (left turn)
			Direction.Down => (new Edge(edge.Coordinate + _left, Direction.Down), new Edge(edge.Coordinate, Direction.Left), new Edge(edge.Coordinate + _down + _left, Direction.Right)),

			// should've been covered by one of the above cases
			_ => throw new UnreachableException($"Unknown direction encountered for edge at {edge.Coordinate}"),
		};

		var node = edgeList.Find(no_corner);

		if (node != null) {
			edgeList.Remove(node);
			return (node.Value, false);
		}

		node = edgeList.Find(right_turn);

		if (node != null) {
			edgeList.Remove(node);
			return (node.Value, true);
		}

		node = edgeList.Find(left_turn);

		if (node != null) {
			edgeList.Remove(node);
			return (node.Value, true);
		}

		throw new UnreachableException($"Following edge from {edge.Coordinate} (edge dir {edge.Direction}), could not find next edge");
	}

	private enum Direction
	{
		Up,
		Right,
		Down,
		Left
	}

	private readonly record struct Edge(Coordinate Coordinate, Direction Direction)
	{
		public static Edge Empty { get; } = new(Coordinate.Empty, Direction.Up);
	}
}
