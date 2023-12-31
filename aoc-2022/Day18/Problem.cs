using System.Diagnostics.CodeAnalysis;

namespace aoc_2022.Day18;

public class Problem
{
	public static ProblemMetadata2 Metadata { get; } = new(Main, typeof(Problem));

	private static readonly Coordinate3 X_PLUS_ONE  = new( 1,  0,  0);
	private static readonly Coordinate3 X_MINUS_ONE = new(-1,  0,  0);
	private static readonly Coordinate3 Y_PLUS_ONE  = new( 0,  1,  0);
	private static readonly Coordinate3 Y_MINUS_ONE = new( 0, -1,  0);
	private static readonly Coordinate3 Z_PLUS_ONE  = new( 0,  0,  1);
	private static readonly Coordinate3 Z_MINUS_ONE = new( 0,  0, -1);

	public static (long, long) Main(string[] input)
	{
		var cubes   = input.Select(Coordinate3.Parse).ToArray();
		var edgeSet = new HashSet<Edge>(new EdgeComparer());
		var overlap = 0;

		foreach (var edge in cubes.SelectMany(ComputeEdges)) {
			if (!edgeSet.Add(edge)) {
				overlap++;
			}
		}

		var part2 = CalculateExternalSurfaceArea(cubes);

		return (edgeSet.Count - overlap, part2);
	}

	private static int CalculateExternalSurfaceArea(Coordinate3[] lava)
	{
		var lavaSet      = new HashSet<Coordinate3>(lava);
		var seenSet      = new HashSet<Coordinate3>();
		var coordinates  = new Stack<Coordinate3>();
		var surfaceEdges = new HashSet<Edge>(/*new EdgeComparer()*/);
		var minX         = long.MaxValue;
		var minY         = long.MaxValue;
		var minZ         = long.MaxValue;
		var maxX         = long.MinValue;
		var maxY         = long.MinValue;
		var maxZ         = long.MinValue;
		var adjacencies  = new[] {
			X_PLUS_ONE,
			X_MINUS_ONE,
			Y_PLUS_ONE,
			Y_MINUS_ONE,
			Z_PLUS_ONE,
			Z_MINUS_ONE,
		};

		// compute the bounding rectangle
		for (var i = 0; i < lava.Length; i++) {
			if (lava[i].X < minX) minX = lava[i].X;
			if (lava[i].X > maxX) maxX = lava[i].X;
			if (lava[i].Y < minY) minY = lava[i].Y;
			if (lava[i].Y > maxY) maxY = lava[i].Y;
			if (lava[i].Z < minZ) minZ = lava[i].Z;
			if (lava[i].Z > maxZ) maxZ = lava[i].Z;
		}

		// stretch the rectangle so we're sure to explore all edges
		minX -= 1; maxX += 1;
		minY -= 1; maxY += 1;
		minZ -= 1; maxZ += 1;

		coordinates.Push(new Coordinate3(minX, minY, minZ));

		while (coordinates.Count > 0) {
			var coordinate = coordinates.Pop();

			// if this coordinate isn't inside the bounding rectangle, then continue
			if (coordinate.X < minX || coordinate.X > maxX || coordinate.Y < minY || coordinate.Y > maxY || coordinate.Z < minZ || coordinate.Z > maxZ) {
				continue;
			}

			// for every adjacent coordinate, either it's inside the lava (so we add an edge),
			//   or it needs to be examined later
			foreach (var adjacent in adjacencies.Select(c => coordinate + c)) {
				if (lavaSet.Contains(adjacent)) {
					surfaceEdges.Add(new Edge(coordinate, adjacent));
				} else if (seenSet.Add(adjacent)) {
					coordinates.Push(adjacent);
				}
			}
		}

		// the count of edges in the set should be the surfance area, no?
		return surfaceEdges.Count;
	}

	private static IEnumerable<Edge> ComputeEdges(Coordinate3 cube)
	{
		yield return new Edge(cube, cube + X_PLUS_ONE);
		yield return new Edge(cube, cube + X_MINUS_ONE);
		yield return new Edge(cube, cube + Y_PLUS_ONE);
		yield return new Edge(cube, cube + Y_MINUS_ONE);
		yield return new Edge(cube, cube + Z_PLUS_ONE);
		yield return new Edge(cube, cube + Z_MINUS_ONE);
	}

	private class EdgeComparer : IEqualityComparer<Edge>
	{
		public bool Equals(Edge x, Edge y) => (x.Between1 == y.Between1 && x.Between2 == y.Between2) || (x.Between1 == y.Between2 && x.Between2 == y.Between1);

		public int GetHashCode([DisallowNull] Edge obj) => obj.Between1.GetHashCode() + obj.Between2.GetHashCode();
	}

	private readonly struct Edge
	{
		public Edge(Coordinate3 between1, Coordinate3 between2) => (Between1, Between2) = (between1, between2);

		public Coordinate3 Between1 { get; init; }

		public Coordinate3 Between2 { get; init; }
	}
}
