namespace AdventOfCode.Year2023.Day21;

public partial class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	private static readonly Coordinate _north = new(0, -1);
	private static readonly Coordinate _south = new(0, +1);
	private static readonly Coordinate _east = new(+1, 0);
	private static readonly Coordinate _west = new(-1, 0);
	private static readonly long        _part2Steps = 26_501_365;

	public static (long, long) Execute(string[] input)
	{
		var map       = CreateMap(input, c => c);
		var width     = map.GetLength(0);
		var height    = map.GetLength(1);
		var start     = FindStart(map, width, height);
		var distances = Dijkstra.FindDistances(start, from => FindAdjacentCoordinates(map, width, height, from));
		var part1     = distances.Values.Count(v => v <= 64 && v % 2 == 0);

		var stepsToEdge  = width / 2; // it takes us this many steps to reach the edge of a tile
		var tileCount    = (_part2Steps - stepsToEdge) / width; // this is how many tiles we could reach (in a straight line) from our starting point
		var evenTiles    = tileCount * tileCount; // even/and odd here and below work because our step count is odd
		var oddTiles     = (tileCount + 1) * (tileCount + 1);
		var evenCorners  = tileCount;
		var oddCorners   = tileCount + 1;
		var evenCornerSz = distances.Count(kvp => kvp.Value > stepsToEdge && kvp.Value % 2 == 0 && Coordinate.ManhattanDistance(kvp.Key, start) > 65);
		var oddCornerSz  = distances.Count(kvp => kvp.Value > stepsToEdge && kvp.Value % 2 == 1 && Coordinate.ManhattanDistance(kvp.Key, start) > 65);
		var evenTileSz   = distances.Values.Count(v => v % 2 == 0);
		var oddTileSz    = distances.Values.Count(v => v % 2 == 1);
		var part2        = (evenTiles * evenTileSz) + (oddTiles * oddTileSz) - (oddCorners * oddCornerSz) + (evenCorners * evenCornerSz);

		return (part1, part2);
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
