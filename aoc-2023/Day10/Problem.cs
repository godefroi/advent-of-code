using System.Text;

namespace AdventOfCode.Year2023.Day10;

public partial class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		var map    = CreateMap(input, c => c);
		var start  = FindStartingPoint(map);
		var width  = map.GetLength(0);
		var height = map.GetLength(1);
		var part2  = 0;

		var (boundary, part1) = CalculateBoundary(map, start);

		// eliminate the "random bits of pipe"
		TransformMap(map, boundary, width, height);

		for (var y = 0; y < height; y++) {
			for (var x = 0; x < width; x++) {
				if (map[x, y] == '.') {
					if (IsInsideLoop(x, y, map, width, boundary)) {
						part2++;
						map[x, y] = 'I';
					} else {
						map[x, y] = 'O';
					}
				}
			}
		}

		// for (var y = 0; y < height; y++) {
		// 	var sb = new StringBuilder();

		// 	for (var x = 0; x < width; x++) {
		// 		if (map[x, y] == 'I' || map[x, y] == 'O') {
		// 			sb.Append(AnsiCodes.ANSI_CYAN);
		// 		}
		// 		sb.Append(map[x, y]);
		// 		if (map[x, y] == 'I' || map[x, y] == 'O') {
		// 			sb.Append(AnsiCodes.ANSI_RESET);
		// 		}
		// 	}

		// 	Console.WriteLine(sb.ToString());
		// }

		// 165 is too low for part 2

		return (part1, part2);
	}

	private static void TransformMap(char[,] map, IReadOnlySet<Coordinate> boundary, int width, int height)
	{
		for (var y = 0; y < height; y++) {
			for (var x = 0; x < width; x++) {
				if (map[x, y] != '.' && !boundary.Contains(new Coordinate(x, y))) {
					map[x, y] = '.';
				}
			}
		}
	}

	private static (HashSet<Coordinate> Boundary, int MaxDistance) CalculateBoundary(char[,] map, Coordinate start)
	{
		//Console.WriteLine($"start={start}");
		var dir1     = NextCoordinate(map, start, new Coordinate(-1, -1));
		var dir1From = start;
		//Console.WriteLine($"dir1={dir1} ({map[dir1.X, dir1.Y]})");
		var dir2     = NextCoordinate(map, start, dir1);
		var dir2From = start;
		//Console.WriteLine($"dir2={dir2} ({map[dir2.X, dir2.Y]})");
		var maxDist  = 1;
		var boundary = new HashSet<Coordinate>() {
			start,
			dir1,
			dir2
		};

		var n = false;
		var s = false;
		var e = false;
		var w = false;

		// replace the S with the correct boundary piece (because part2)
		if ((dir1.X == start.X && dir1.Y == start.Y - 1) || (dir2.X == start.X && dir2.Y == start.Y - 1)) { n = true; }
		if ((dir1.X == start.X && dir1.Y == start.Y + 1) || (dir2.X == start.X && dir2.Y == start.Y + 1)) { s = true; }
		if ((dir1.X == start.X + 1 && dir1.Y == start.Y) || (dir2.X == start.X + 1 && dir2.Y == start.Y)) { e = true; }
		if ((dir1.X == start.X - 1 && dir1.Y == start.Y) || (dir2.X == start.X - 1 && dir2.Y == start.Y)) { w = true; }

		//Console.WriteLine($"n={n} s={s} e={e} w={w}");

		if (n & s) {
			map[start.X, start.Y] = '|';
		} else if (e & w) {
			map[start.X, start.Y] = '-';
		} else if (n & e) {
			map[start.X, start.Y] = 'L';
		} else if (n & w) {
			map[start.X, start.Y] = 'J';
		} else if (s & e) {
			map[start.X, start.Y] = 'F';
		} else if (s & w) {
			map[start.X, start.Y] = '7';
		} else {
			throw new InvalidOperationException("Could not determine correct replacement for start");
		}

		while (dir1 != dir2) {
			maxDist++;

			var od1 = dir1;
			var od2 = dir2;

			// find out where we're going
			dir1 = NextCoordinate(map, dir1, dir1From);
			//Console.WriteLine($"dir1 {od1} -> {dir1}");
			dir2 = NextCoordinate(map, dir2, dir2From);
			//Console.WriteLine($"dir2 {od2} -> {dir2}");

			// keep track of where we came from
			dir1From = od1;
			dir2From = od2;

			boundary.Add(dir1);
			boundary.Add(dir2);
		}

		return (boundary, maxDist);
	}

	private static bool IsInsideLoop(int x, int y, char[,] map, int mapWidth, IReadOnlySet<Coordinate> boundary)
	{
		var transitionCnt    = 0;
		var boundaryStartDir = 0;

		//Console.WriteLine($"casting from [{x},{y}]");

		// cast a ray east along the X axis, counting transitions
		for (var tx = x + 1; tx < mapWidth; tx++) {
			var testCoords = new Coordinate(tx, y);

			// if this coordinate isn't part of our boundary, then skip it
			if (!boundary.Contains(testCoords)) {
				continue;
			};

			// see what kind of boundary this one is
			var boundaryType = map[testCoords.X, testCoords.Y];

			if (boundaryType == '|') {
				transitionCnt++;
				boundaryStartDir = 0;
				//Console.WriteLine($"             on a boundary at {testCoords} ({boundaryType}), lastDir = {lastDir}, transitionCnt = {transitionCnt}");
			} else {
				var thisDir = boundaryType switch {
					'L' => -1,
					'J' => -1,
					'7' => +1,
					'F' => +1,
					_ => 0,
				};

				//Console.WriteLine($"             on a boundary at {testCoords} ({boundaryType}), lastDir = {lastDir}, thisDir = {thisDir}, transitionCnt = {transitionCnt}");

				if (thisDir != 0) {
					if (boundaryStartDir == 0) {
						boundaryStartDir = thisDir;
					} else {
						// if the start-dir and this-dir are different, it was a real transition
						if (thisDir != boundaryStartDir) {
							transitionCnt++;
						}

						boundaryStartDir = 0;
					}
				}
				// if (thisDir != 0) {
				// 	if (thisDir != lastDir && lastDir != 0) {
				// 		transitionCnt++;
				// 		lastDir = 0;
				// 	} else {
				// 		lastDir = thisDir;
				// 	}
				// }
			}
		}

		//Console.WriteLine($"             transitionCnt = {transitionCnt}, inside loop = {transitionCnt % 2 == 1}");

		return transitionCnt % 2 == 1;
	}

	private static Coordinate FindStartingPoint(char[,] map)
	{
		for (var y = 0; y < map.GetLength(1); y++) {
			for (var x = 0; x < map.GetLength(0); x++) {
				if (map[x, y] == 'S') {
					return new Coordinate(x, y);
				}
			}
		}

		throw new InvalidOperationException("No starting point found.");
	}

	private static Coordinate NextCoordinate(char[,] map, Coordinate currentCoordinate, Coordinate from)
	{
		Coordinate next;

		// north
		next = new Coordinate(currentCoordinate.X, currentCoordinate.Y - 1);
		if (next != from && IsConnected(map, currentCoordinate, next)) {
			return next;
		}

		// south
		next = new Coordinate(currentCoordinate.X, currentCoordinate.Y + 1);
		if (next != from && IsConnected(map, currentCoordinate, next)) {
			return next;
		}

		// east
		next = new Coordinate(currentCoordinate.X + 1, currentCoordinate.Y);
		if (next != from && IsConnected(map, currentCoordinate, next)) {
			return next;
		}

		// west
		next = new Coordinate(currentCoordinate.X - 1, currentCoordinate.Y);
		if (next != from && IsConnected(map, currentCoordinate, next)) {
			return next;
		}

		throw new InvalidOperationException($"No connected coordinates found that are not the 'from' coordinate; currentCoordinate={currentCoordinate}, from={from}");
	}

	private static bool IsConnected(char[,] map, Coordinate from, Coordinate to)
	{
		// first, make sure "to" isn't off the edge of the map
		if (to.X == -1 || to.Y == -1 || to.X >= map.GetLength(0) || to.Y >= map.GetLength(1)) {
			return false;
		}

		var fromChar = map[from.X, from.Y];
		var toChar   = map[to.X, to.Y];

		//Console.WriteLine($"from={from} ({fromChar}), to={to} ({toChar})");

		var canConnectNorth = false;
		var canConnectSouth = false;
		var canConnectEast  = false;
		var canConnectWest  = false;

		switch (fromChar) {
			case '|': canConnectNorth = true; canConnectSouth = true; break;
			case '-': canConnectEast  = true; canConnectWest  = true; break;
			case 'L': canConnectNorth = true; canConnectEast  = true; break;
			case 'J': canConnectNorth = true; canConnectWest  = true; break;
			case '7': canConnectSouth = true; canConnectWest  = true; break;
			case 'F': canConnectSouth = true; canConnectEast  = true; break;
			case 'S': canConnectNorth = true; canConnectSouth = true; canConnectEast = true; canConnectWest = true; break;
		}

		return toChar switch {
			// | is a vertical pipe connecting north and south.
			'|' when (to.Y == from.Y + 1 && canConnectSouth) || (to.Y == from.Y - 1 && canConnectNorth) => true,

			// - is a horizontal pipe connecting east and west.
			'-' when (to.X == from.X + 1 && canConnectEast) || (to.X == from.X - 1 && canConnectWest) => true,

			// L is a 90-degree bend connecting north and east.
			'L' when (to.Y == from.Y + 1 && canConnectSouth) || (to.X == from.X - 1 && canConnectWest) => true,

			// is a 90-degree bend connecting north and west.
			'J' when (to.Y == from.Y + 1 && canConnectSouth) || (to.X == from.X + 1 && canConnectEast) => true,

			// 7 is a 90-degree bend connecting south and west.
			'7' when (to.Y == from.Y - 1 && canConnectNorth) || (to.X == from.X + 1 && canConnectEast) => true,

			// F is a 90-degree bend connecting south and east.
			'F' when (to.Y == from.Y - 1 && canConnectNorth) || (to.X == from.X - 1 && canConnectWest) => true,

			// otherwise, not connected
			_ => false,
		};
	}

	public class Tests
	{
		private static char[,] _realMap = ReadFileAsMap("input.txt");

		[Test]
		[Arguments(118, 102, 118, 101, false)] // S -> - (north)
		[Arguments(118, 102, 118, 103, true)]  // S -> | (south)
		[Arguments(118, 102, 119, 102, false)] // S -> L (east)
		[Arguments(118, 102, 117, 102, true)]  // S -> L (west)
		public async Task IsConnectedReturnsCorrectly(int fromX, int fromY, int toX, int toY, bool connected)
		{
			await Assert.That(IsConnected(_realMap, new Coordinate(fromX, fromY), new Coordinate(toX, toY))).IsEqualTo(connected);
		}

		[Test]
		[Arguments("samplePart1SquareLoop.txt",   0, 0, false)]
		[Arguments("samplePart1SquareLoop.txt",   1, 0, false)]
		[Arguments("samplePart1SquareLoop.txt",   2, 0, false)]
		[Arguments("samplePart1SquareLoop.txt",   3, 0, false)]
		[Arguments("samplePart1SquareLoop.txt",   4, 0, false)]
		[Arguments("samplePart1SquareLoop.txt",   0, 1, false)]
		[Arguments("samplePart1SquareLoop.txt",   4, 1, false)]
		[Arguments("samplePart1SquareLoop.txt",   2, 2, true)]
		[Arguments("samplePart1SquareLoop.txt",   0, 3, false)]
		[Arguments("samplePart1SquareLoop.txt",   4, 3, false)]
		[Arguments("samplePart1SquareLoop.txt",   0, 4, false)]
		[Arguments("samplePart1SquareLoop.txt",   1, 4, false)]
		[Arguments("samplePart1SquareLoop.txt",   2, 4, false)]
		[Arguments("samplePart1SquareLoop.txt",   3, 4, false)]
		[Arguments("samplePart1SquareLoop.txt",   4, 4, false)]
		[Arguments("samplePart2LargerSample.txt", 4, 4, false)]
		public async Task IsInsideLoopReturnsCorrectly(string mapFile, int testX, int testY, bool inside)
		{
			var map   = ReadFileAsMap(mapFile);
			var start = FindStartingPoint(map);
			var (boundary, _) = CalculateBoundary(map, start);

			TransformMap(map, boundary, map.GetLength(0), map.GetLength(1));

			await Assert.That(IsInsideLoop(testX, testY, map, map.GetLength(0), boundary)).IsEqualTo(inside);
		}
	}
}
