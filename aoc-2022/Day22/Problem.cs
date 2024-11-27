namespace AdventOfCode.Year2022.Day22;

public partial class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Main, typeof(Problem));

	public static (long, long) Main(string[] lines)
	{
		var path  = ParseRegex().Matches(lines.Last()).Select(m => m.Value).ToArray();
		var map   = ParseMap(lines);

		var result1 = RunPath(map, path, Wrap);
		var part1   = ((result1.y + 1) * 1000) + ((result1.x + 1) * 4) + (int)result1.facing;

		var result2 = RunPath(map, path, WrapCube);
		var part2   = ((result2.y + 1) * 1000) + ((result2.x + 1) * 4) + (int)result2.facing;

		return (part1, part2);
	}

	private static (int x, int y, Direction facing) RunPath(Tile[,] map, string[] path, Func<int, int, Direction, Tile[,], (int x, int y, Direction direction)> wrapper)
	{
		var width            = map.GetLength(0);
		var height           = map.GetLength(1);
		var currentDirection = Direction.Right;
		var currentX         = Enumerable.Range(0, width).First(x => map[x, 0] == Tile.Floor);
		var currentY         = 0;

		//Console.WriteLine($"Starting at {currentX},{currentY}");

		foreach (var step in path) {
			if (int.TryParse(step, out var steps)) {
				for (var i = 0; i < steps; i++) {
					var (newX, newY, newDirection) = currentDirection switch {
						Direction.Up    when currentY == 0                               => wrapper(currentX, currentY, currentDirection, map),
						Direction.Up    when map[currentX, currentY - 1] == Tile.Nothing => wrapper(currentX, currentY, currentDirection, map),
						Direction.Up                                                     => (currentX, currentY - 1, currentDirection),
						Direction.Down  when currentY == height - 1                      => wrapper(currentX, currentY, currentDirection, map),
						Direction.Down  when map[currentX, currentY + 1] == Tile.Nothing => wrapper(currentX, currentY, currentDirection, map),
						Direction.Down                                                   => (currentX, currentY + 1, currentDirection),
						Direction.Left  when currentX == 0                               => wrapper(currentX, currentY, currentDirection, map),
						Direction.Left  when map[currentX - 1, currentY] == Tile.Nothing => wrapper(currentX, currentY, currentDirection, map),
						Direction.Left                                                   => (currentX - 1, currentY, currentDirection),
						Direction.Right when currentX == width - 1                       => wrapper(currentX, currentY, currentDirection, map),
						Direction.Right when map[currentX + 1, currentY] == Tile.Nothing => wrapper(currentX, currentY, currentDirection, map),
						Direction.Right                                                  => (currentX + 1, currentY, currentDirection),
						_ => throw new InvalidOperationException("Unhandled current state."),
					};

					if (map[newX, newY] == Tile.Floor) {
						currentX         = newX;
						currentY         = newY;
						currentDirection = newDirection;
					}
					//Console.WriteLine($"Now at {currentX},{currentY}");
				}
			} else {
				currentDirection = Rotate(currentDirection, step);
				//Console.WriteLine($"Now facing {currentDirection}");
			}
		}

		return (currentX, currentY, currentDirection);
	}

	private static Direction Rotate(Direction facing, string rotation) => (facing, rotation) switch {
		(Direction.Up, "R") => Direction.Right,
		(Direction.Up, "L") => Direction.Left,
		(Direction.Down, "R") => Direction.Left,
		(Direction.Down, "L") => Direction.Right,
		(Direction.Left, "R") => Direction.Up,
		(Direction.Left, "L") => Direction.Down,
		(Direction.Right, "R") => Direction.Down,
		(Direction.Right, "L") => Direction.Up,
		_ => throw new ArgumentException("Invalid combination of facing and rotation"),
	};

	private static (int x, int y, Direction direction) Wrap(int x, int y, Direction direction, Tile[,] map)
	{
		var width  = map.GetLength(0);
		var height = map.GetLength(1);

		switch (direction) {
			case Direction.Up:
				for (var i = height - 1; i > y; i--) {
					if (map[x, i] != Tile.Nothing) {
						return (x, i, direction);
					}
				}
				throw new InvalidOperationException($"Unable to wrap up from {x},{y}");

			case Direction.Down:
				for (var i = 0; i < height; i++) {
					if (map[x, i] != Tile.Nothing) {
						return (x, i, direction);
					}
				}
				throw new InvalidOperationException($"Unable to wrap down from {x},{y}");

			case Direction.Left:
				for (var i = width - 1; i > x; i--) {
					if (map[i, y] != Tile.Nothing) {
						return (i, y, direction);
					}
				}
				throw new InvalidOperationException($"Unable to wrap left from {x},{y}");

			case Direction.Right:
				for (var i = 0; i < width; i++) {
					if (map[i, y] != Tile.Nothing) {
						return (i, y, direction);
					}
				}
				throw new InvalidOperationException($"Unable to wrap right from {x},{y}");

			default:
				throw new InvalidOperationException("Invalid direction.");
		}
	}

	private static (int x, int y, Direction direction) WrapCube(int x, int y, Direction direction, Tile[,] map)
	{
		var width    = map.GetLength(0);
		var height   = map.GetLength(1);
		var sideSize = -1;
		var wrapToX  = -1;
		var wrapToY  = -1;

		if (width % 50 == 0) {
			sideSize = 50;
		} else if (width % 4 == 0) {
			sideSize = 4;
		} else {
			throw new InvalidDataException($"Map of width {width} is unsupported.");
		}

		var (sideX, xRemainder) = Math.DivRem(x, sideSize);
		var (sideY, yRemainder) = Math.DivRem(y, sideSize);

		// NOTE: this set of transformations works for my map. Maybe it'll work for your map, if your map is my map. More likely it won't.

		if (sideSize == 4) {
			// sample input
			(wrapToX, wrapToY, direction, x, y) = direction switch {
				Direction.Up    when sideX == 0 && sideY == 1 => (2, 0, Direction.Down,  2 * sideSize + (sideSize - xRemainder - 1), 0 * sideSize + yRemainder),
				Direction.Up    when sideX == 2 && sideY == 0 => (0, 1, Direction.Down,  0 * sideSize + (sideSize - xRemainder - 1), 1 * sideSize + yRemainder),
				Direction.Up    when sideX == 1 && sideY == 1 => (2, 0, Direction.Right, 2 * sideSize, 0 * sideSize + xRemainder),
				Direction.Up    when sideX == 3 && sideY == 2 => (2, 1, Direction.Left,  2 * sideSize + (sideSize - 1), 1 * sideSize + (sideSize - xRemainder - 1)),
				Direction.Down  when sideX == 0 && sideY == 1 => (2, 2, Direction.Up,    2 * sideSize + (sideSize - xRemainder - 1), 2 * sideSize + (sideSize - 1)),
				Direction.Down  when sideX == 2 && sideY == 2 => (0, 1, Direction.Up,    0 * sideSize + (sideSize - xRemainder - 1), 1 * sideSize + (sideSize - 1)),
				Direction.Down  when sideX == 1 && sideY == 1 => (2, 2, Direction.Right, 2 * sideSize, 2 * sideSize + (sideSize - xRemainder - 1)),
				Direction.Down  when sideX == 3 && sideY == 2 => (0, 1, Direction.Right, 0 * sideSize, 1 * sideSize + (sideSize - xRemainder - 1)),
				Direction.Left  when sideX == 0 && sideY == 1 => (3, 2, Direction.Up,    3 * sideSize + (sideSize - yRemainder - 1), 2 * sideSize + (sideSize - 1)),
				Direction.Left  when sideX == 2 && sideY == 2 => (1, 1, Direction.Up,    1 * sideSize + (sideSize - yRemainder - 1), 1 * sideSize + (sideSize - 1)),
				Direction.Left  when sideX == 2 && sideY == 0 => (1, 1, Direction.Down,  1 * sideSize + yRemainder, 1 * sideSize + xRemainder),
				Direction.Right when sideX == 2 && sideY == 1 => (3, 2, Direction.Down,  3 * sideSize + (sideSize - yRemainder - 1), 2 * sideSize),
				Direction.Right when sideX == 2 && sideY == 0 => (3, 2, Direction.Left,  3 * sideSize + (sideSize - 1), 2 * sideSize + (sideSize - yRemainder - 1)),
				Direction.Right when sideX == 3 && sideY == 2 => (2, 0, Direction.Left,  2 * sideSize + (sideSize - 1), 0 * sideSize + (sideSize - yRemainder - 1)),
				_ => throw new NotImplementedException("That wrap is not yet implemented."),
			};
		} else if (sideSize == 50) {
			var ox = x;
			var oy = y;
			var od = direction;

			(wrapToX, wrapToY, direction, x, y) = direction switch {
				Direction.Up    when sideX == 1 && sideY == 0 => (0, 3, Direction.Right, 0 * sideSize, 3 * sideSize + xRemainder),
				Direction.Left  when sideX == 1 && sideY == 0 => (0, 2, Direction.Right, 0 * sideSize, 2 * sideSize + (sideSize - yRemainder - 1)),
				Direction.Up    when sideX == 2 && sideY == 0 => (0, 3, Direction.Up,    0 * sideSize + xRemainder, 3 * sideSize + (sideSize - 1)),
				Direction.Right when sideX == 2 && sideY == 0 => (1, 2, Direction.Left,  1 * sideSize + (sideSize - 1), 2 * sideSize + (sideSize - yRemainder - 1)),
				Direction.Down  when sideX == 2 && sideY == 0 => (1, 1, Direction.Left,  1 * sideSize + (sideSize - 1), 1 * sideSize + xRemainder),
				Direction.Left  when sideX == 1 && sideY == 1 => (0, 2, Direction.Down,  0 * sideSize + yRemainder, 2 * sideSize + xRemainder),
				Direction.Right when sideX == 1 && sideY == 1 => (2, 0, Direction.Up,    2 * sideSize + yRemainder, 0 * sideSize + (sideSize - 1)),
				Direction.Up    when sideX == 0 && sideY == 2 => (1, 1, Direction.Right, 1 * sideSize, 1 * sideSize + xRemainder),
				Direction.Left  when sideX == 0 && sideY == 2 => (1, 0, Direction.Right, 1 * sideSize, 0 * sideSize + (sideSize - yRemainder - 1)), // ? maybe, looks good tho
				Direction.Right when sideX == 1 && sideY == 2 => (2, 0, Direction.Left,  2 * sideSize + (sideSize - 1), 0 * sideSize + (sideSize - yRemainder - 1)),
				Direction.Down  when sideX == 1 && sideY == 2 => (0, 3, Direction.Left,  0 * sideSize + (sideSize - 1), 3 * sideSize + xRemainder),
				Direction.Left  when sideX == 0 && sideY == 3 => (1, 0, Direction.Down,  1 * sideSize + yRemainder, 0 * sideSize + xRemainder),
				Direction.Down  when sideX == 0 && sideY == 3 => (2, 0, Direction.Down,  2 * sideSize + xRemainder, 0 * sideSize),
				Direction.Right when sideX == 0 && sideY == 3 => (1, 2, Direction.Up,    1 * sideSize + yRemainder, 2 * sideSize + (sideSize - 1)),
				_ => throw new NotImplementedException("That wrap is not yet implemented."),
			};

			if (od == Direction.Up && direction == Direction.Right) {
				// this wrap looks good
			} else if (od == Direction.Down && direction == Direction.Left) {
				// this wrap looks good
			} else if (od == Direction.Right && direction == Direction.Up) {
				// this wrap looks good
			} else if (od == Direction.Up && direction == Direction.Up) {
				// this wrap looks good
			} else if (od == Direction.Left && direction == Direction.Down) {
				// this wrap looks good
			} else if (od == Direction.Down && direction == Direction.Down) {
				// this wrap looks good
			} else if (od == Direction.Right && direction == Direction.Left) {
				// this wrap looks good
			} else if (od == Direction.Left && direction == Direction.Right) {
				// this wrap looks good
			} else {
				Console.WriteLine($"Wrapped {od} from {ox},{oy} (on side {sideX},{sideY}) to {x},{y} (on side {wrapToX},{wrapToY}) facing {direction}");
			}
		} else {
			throw new InvalidDataException("Whatever map you're using, it's not ours.");
		}

		return (x, y, direction);
	}

	private static Tile[,] ParseMap(string[] input)
	{
		var width = input.SkipLast(2).Max(l => l.Length);
		var map   = new Tile[width, input.Length - 2];

		for (var y = 0; y < input.Length - 2; y++) {
			for (var x = 0; x < width; x++) {
				if (input[y].Length <= x) {
					map[x, y] = Tile.Nothing;
				} else {
					map[x, y] = (Tile)input[y][x];
				}
			}
		}

		return map;
	}

	private static void Draw(Tile[,] map)
	{
		var width = map.GetLength(0);

		for (var i = 0; i < map.GetLength(1); i++) {
			Console.WriteLine(new string(Enumerable.Range(0, width).Select(x => map[x, i]).Select(t => (char)t).ToArray()));
		}
	}

	private enum Tile : int
	{
		Nothing = ' ',
		Floor   = '.',
		Wall    = '#',
	}

	public enum Direction
	{
		Up    = 3,
		Down  = 1,
		Left  = 2,
		Right = 0,
	}

	[GeneratedRegex(@"(\d+|R|L)")]
	private static partial Regex ParseRegex();

	[Fact]
	public void PathParsesCorrectly()
	{
		Assert.Collection(ParseRegex().Matches("10R5L5R10L4R5L5").Select(m => m.Value),
			s => Assert.Equal("10", s),
			s => Assert.Equal("R", s),
			s => Assert.Equal("5", s),
			s => Assert.Equal("L", s),
			s => Assert.Equal("5", s),
			s => Assert.Equal("R", s),
			s => Assert.Equal("10", s),
			s => Assert.Equal("L", s),
			s => Assert.Equal("4", s),
			s => Assert.Equal("R", s),
			s => Assert.Equal("5", s),
			s => Assert.Equal("L", s),
			s => Assert.Equal("5", s));
	}

	[Theory]
	[InlineData(11,  6, Direction.Right,  0,  6)]
	[InlineData( 0,  6, Direction.Left,  11,  6)]
	[InlineData( 5,  4, Direction.Up,     5,  7)]
	[InlineData( 5,  7, Direction.Down,   5,  4)]
	[InlineData(11,  0, Direction.Up,    11, 11)]
	[InlineData(11, 11, Direction.Down,  11,  0)]
	public void WrappingHappensCorrectly(int x, int y, Direction direction, int expectedX, int expectedY)
	{
		var lines = ReadFileLines("inputSample.txt");
		var map   = ParseMap(lines);
		var loc   = Wrap(x, y, direction, map);

		Assert.Equal(expectedX, loc.x);
		Assert.Equal(expectedY, loc.y);
	}

	public static IEnumerable<object[]> CubeWrapTestData { get; } = new[] {
		// left from 2,0 (ends up down on 1,1)
		new object[] { 8, 0, Direction.Left, 4, 4, Direction.Down },
		new object[] { 8, 1, Direction.Left, 5, 4, Direction.Down },
		new object[] { 8, 2, Direction.Left, 6, 4, Direction.Down },
		new object[] { 8, 3, Direction.Left, 7, 4, Direction.Down },

		// up from 2,0 (ends up down on 0,1)
		new object[] {  8, 0, Direction.Up, 3, 4, Direction.Down },
		new object[] {  9, 0, Direction.Up, 2, 4, Direction.Down },
		new object[] { 10, 0, Direction.Up, 1, 4, Direction.Down },
		new object[] { 11, 0, Direction.Up, 0, 4, Direction.Down },

		// right from 2,0 (ends up left on 3,2)
		new object[] { 11, 0, Direction.Right, 15, 11, Direction.Left },
		new object[] { 11, 1, Direction.Right, 15, 10, Direction.Left },
		new object[] { 11, 2, Direction.Right, 15,  9, Direction.Left },
		new object[] { 11, 3, Direction.Right, 15,  8, Direction.Left },

		// up from 0,1 (ends up down on 2,0)
		new object[] { 0, 4, Direction.Up, 11, 0, Direction.Down },
		new object[] { 1, 4, Direction.Up, 10, 0, Direction.Down },
		new object[] { 2, 4, Direction.Up,  9, 0, Direction.Down },
		new object[] { 3, 4, Direction.Up,  8, 0, Direction.Down },

		// left from 0,1 (ends up up on 3,2)
		new object[] { 0, 4, Direction.Left, 15, 11, Direction.Up },
		new object[] { 0, 5, Direction.Left, 14, 11, Direction.Up },
		new object[] { 0, 6, Direction.Left, 13, 11, Direction.Up },
		new object[] { 0, 7, Direction.Left, 12, 11, Direction.Up },

		// down from 0,1 (ends up up on 2,2)
		new object[] { 0, 7, Direction.Down, 11, 11, Direction.Up },
		new object[] { 1, 7, Direction.Down, 10, 11, Direction.Up },
		new object[] { 2, 7, Direction.Down,  9, 11, Direction.Up },
		new object[] { 3, 7, Direction.Down,  8, 11, Direction.Up },

		// up from 1,1 (ends up right on 2,0)
		new object[] { 4, 4, Direction.Up, 8, 0, Direction.Right },
		new object[] { 5, 4, Direction.Up, 8, 1, Direction.Right },
		new object[] { 6, 4, Direction.Up, 8, 2, Direction.Right },
		new object[] { 7, 4, Direction.Up, 8, 3, Direction.Right },

		// down from 1,1 (ends up right on 2,2)
		new object[] { 4, 7, Direction.Down, 8, 11, Direction.Right },
		new object[] { 5, 7, Direction.Down, 8, 10, Direction.Right },
		new object[] { 6, 7, Direction.Down, 8,  9, Direction.Right },
		new object[] { 7, 7, Direction.Down, 8,  8, Direction.Right },

		// right from 2,1 (ends up down on 3,2)
		new object[] { 11, 4, Direction.Right, 15, 8, Direction.Down },
		new object[] { 11, 5, Direction.Right, 14, 8, Direction.Down },
		new object[] { 11, 6, Direction.Right, 13, 8, Direction.Down },
		new object[] { 11, 7, Direction.Right, 12, 8, Direction.Down },

		// left from 2,2 (ends up up on 1,1)
		new object[] { 8,  8, Direction.Left, 7, 7, Direction.Up },
		new object[] { 8,  9, Direction.Left, 6, 7, Direction.Up },
		new object[] { 8, 10, Direction.Left, 5, 7, Direction.Up },
		new object[] { 8, 11, Direction.Left, 4, 7, Direction.Up },

		// down from 2,2 (ends up up on 0,1)
		new object[] {  8, 11, Direction.Down, 3, 7, Direction.Up },
		new object[] {  9, 11, Direction.Down, 2, 7, Direction.Up },
		new object[] { 10, 11, Direction.Down, 1, 7, Direction.Up },
		new object[] { 11, 11, Direction.Down, 0, 7, Direction.Up },

		// up from 3,2 (ends up left on 2,1)
		new object[] { 12, 8, Direction.Up, 11, 7, Direction.Left },
		new object[] { 13, 8, Direction.Up, 11, 6, Direction.Left },
		new object[] { 14, 8, Direction.Up, 11, 5, Direction.Left },
		new object[] { 15, 8, Direction.Up, 11, 4, Direction.Left },

		// right from 3,2 (ends up left on 2,0)
		new object[] { 15,  8, Direction.Right, 11, 3, Direction.Left },
		new object[] { 15,  9, Direction.Right, 11, 2, Direction.Left },
		new object[] { 15, 10, Direction.Right, 11, 1, Direction.Left },
		new object[] { 15, 11, Direction.Right, 11, 0, Direction.Left },

		// down from 3,2 (ends up right on 0,1)
		new object[] { 12, 11, Direction.Down, 0, 7, Direction.Right },
		new object[] { 13, 11, Direction.Down, 0, 6, Direction.Right },
		new object[] { 14, 11, Direction.Down, 0, 5, Direction.Right },
		new object[] { 15, 11, Direction.Down, 0, 4, Direction.Right },
	};

	[Theory]
	[MemberData(nameof(CubeWrapTestData))]
	public void CubeWrappingHappensCorrectly(int x, int y, Direction direction, int expectedX, int expectedY, Direction expectedDirection)
	{
		var lines = ReadFileLines("inputSample.txt");
		var map   = ParseMap(lines);

		var (newX, newY, newDir) = WrapCube(x, y, direction, map);

		Assert.Equal(expectedX,         newX);
		Assert.Equal(expectedY,         newY);
		Assert.Equal(expectedDirection, newDir);
	}
}
