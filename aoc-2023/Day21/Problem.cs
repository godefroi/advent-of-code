namespace AdventOfCode.Year2023.Day21;

public partial class Problem
{
	public static ProblemMetadata2 Metadata { get; } = new(Execute, null);

	private static readonly Coordinate _north = new(0, -1);
	private static readonly Coordinate _south = new(0, +1);
	private static readonly Coordinate _east = new(+1, 0);
	private static readonly Coordinate _west = new(-1, 0);


	public static (long, long) Execute(string[] input)
	{
		var map    = CreateMap(input, c => c);
		var width  = map.GetLength(0);
		var height = map.GetLength(1);
		var start  = FindStart(map, width, height);

		var distances = Dijkstra.FindDistances(start, from => FindAdjacentCoordinates(map, width, height, from), 64);

		var part1 = distances.Count(kvp => kvp.Value <= 64 && kvp.Value % 2 == 0);
		// Console.WriteLine(start);
		// foreach (var d in distances) {
		// 	Console.WriteLine($"{d.Key} ({d.Value})");
		// }

		return (part1, 0);
	}

	private static IEnumerable<Coordinate> FindAdjacentCoordinates(char[,] map, int width, int height, Coordinate adjacentTo)
	{
		var north = adjacentTo + _north;
		var south = adjacentTo + _south;
		var east  = adjacentTo + _east;
		var west  = adjacentTo + _west;

		if (north.Y >= 0 && map[north.X, north.Y] == '.') {
			yield return north;
		}

		if (south.Y < height && map[south.X, south.Y] == '.') {
			yield return south;
		}

		if (east.X < width && map[east.X, east.Y] == '.') {
			yield return east;
		}

		if (west.X >= 0 && map[west.X, west.Y] == '.') {
			yield return west;
		}
	}

	private static Coordinate FindStart(char[,] map, int width, int height)
	{
		for (var y = 0; y < height; y++) {
			for (var x = 0; x < width; x++) {
				if (map[x, y] == 'S') {
					return new Coordinate(x, y);
				}
			}
		}

		throw new InvalidOperationException("No start found.");
	}
}
