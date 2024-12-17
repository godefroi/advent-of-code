namespace AdventOfCode.Year2024.Day15;

public class Problem
{
	private const char _empty = '.';
	private const char _wall = '#';
	private const char _box = 'O';
	private const char _boxLeft = '[';
	private const char _boxRight = ']';
	private const char _robot = '@';

	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		var (map, robot1, directions) = Parse(input);
		var (map2, robot2) = ExpandMap(map);
		var part1 = 0L;
		var part2 = 0L;

		foreach (var move in directions) {
			robot1 = ExecuteP1Move(map, robot1, move);
		}

		for (var x = 0; x < map.GetLength(0); x++) {
			for (var y = 0; y < map.GetLength(1); y++) {
				if (map[x, y] == _box) {
					part1 += (100 * y) + x;
				}
			}
		}

		foreach (var move in directions) {
			robot2 = ExecuteP2Move(map2, robot2, move);
		}

		for (var x = 0; x < map2.GetLength(0); x++) {
			for (var y = 0; y < map2.GetLength(1); y++) {
				if (map2[x, y] == _boxLeft) {
					part2 += (100 * y) + x;
				}
			}
		}

		return (part1, part2);
	}

	private static Coordinate ExecuteP1Move(char[,] map, Coordinate robot, Direction moveDir)
	{
		var move = moveDir switch {
			Direction.Up    => new Coordinate( 0, -1),
			Direction.Down  => new Coordinate( 0,  1),
			Direction.Left  => new Coordinate(-1,  0),
			Direction.Right => new Coordinate( 1,  0),
			_ => throw new ArgumentException("Invalid direction", nameof(moveDir)),
		};

		// start at robot + move
		var checkPos = robot + move;

		// move past the boxes, if there are any
		while (map.ValueAt(checkPos) == _box) {
				checkPos += move;
		}

		// if we found a wall, then we can't move the robot in this direction
		if (map.ValueAt(checkPos) == _wall) {
			return robot;
		}

		// if it's not a wall, and it's not empty, then what is it?
		if (map.ValueAt(checkPos) != _empty) {
			throw new InvalidOperationException("Unknown item at position");
		}

		// move the boxes between the robot and checkPost
		// first box to be moved is (robot + move)    --> this is where the robot will end up
		// last box to be moved is (checkPost - move) --> this won't change
		var newRobot = robot + move;

		map[robot.X, robot.Y]       = _empty;
		map[checkPos.X, checkPos.Y] = _box;
		map[newRobot.X, newRobot.Y] = _robot; // we do this last so that if we didn't move a box, it still becomes the robot

		return newRobot;
	}

	private static Coordinate ExecuteP2Move(char[,] map, Coordinate pushFrom, Direction direction)
	{
		// pushFrom is a single coordinate, so we're going to work from that; it might
		// be a robot, or it might be a (part of a) box. If the thing being pushed is
		// a part of a box, we'll need to "entrain" the other part and try to move that
		// as well.

		var move = direction switch {
			Direction.Up    => new Coordinate( 0, -1),
			Direction.Down  => new Coordinate( 0,  1),
			Direction.Left  => new Coordinate(-1,  0),
			Direction.Right => new Coordinate( 1,  0),
			_ => throw new ArgumentException("Invalid direction", nameof(direction)),
		};

		var checks  = new List<Coordinate>() { pushFrom };
		var updates = new Dictionary<Coordinate, char>();

		while (checks.Count > 0) {
			var newChecks = new HashSet<Coordinate>();

			foreach (var moveFrom in checks) {
				var moveTo   = moveFrom + move;
				var checkObj = map.ValueAt(moveTo);

				// if this spot is a wall, we definitely cannot move, and thus, nobody
				// can move, and the robot stays where it was
				if (checkObj == _wall) {
					return pushFrom;
				}

				// otherwise, we can move; keep track of what's going to be put here
				SetUpdate(updates, moveTo, map.ValueAt(moveFrom));

				// if this spot is empty, then this position (at least) can move and
				// we no longer need to run a check in this spot
				if (checkObj == _empty) {
					continue;
				}

				// otherwise, we'll need to keep checking from here on down/up/whatever
				newChecks.Add(moveTo);

				// if we're moving up or down, and we encountered one side of a box,
				// then we need to also check the other side of the box
				if (direction == Direction.Up || direction == Direction.Down) {
					// if it's the left side of the box, then also check the right side
					if (checkObj == _boxLeft) {
						var otherSide = moveTo + new Coordinate(1, 0);
						newChecks.Add(otherSide);
						SetUpdate(updates, otherSide, _empty);
					} else if (checkObj == _boxRight) {
						var otherSide = moveTo + new Coordinate(-1, 0);
						newChecks.Add(otherSide);
						SetUpdate(updates, otherSide, _empty);
					}
				}
			}

			// come back around with our new set of coordinates to check
			checks = [.. newChecks];
		}

		// empty out the robot's position
		map[pushFrom.X, pushFrom.Y] = _empty;

		// we were successful; run all the updates
		foreach (var (coord, val) in updates) {
			if (val == _robot) {
				pushFrom = coord;
			}

			map[coord.X, coord.Y] = val;
		}

		return pushFrom;
	}

	private static void SetUpdate(Dictionary<Coordinate, char> updates, Coordinate location, char updateTo)
	{
		// if we don't currently have an update for here, then just add it
		if (!updates.TryGetValue(location, out var currentUpdate)) {
			updates.Add(location, updateTo);
		}

		// if it's currently empty, then update it to be whatever this is (because)
		// empty was wrong)
		if (currentUpdate == _empty) {
			updates[location] = updateTo;
		}
	}

	private static (char[,] Map, Coordinate Robot, List<Direction> Movement) Parse(string[] input)
	{
		var height = -1;

		for (var i = 0; i < input.Length; i++) {
			if (string.IsNullOrEmpty(input[i])) {
				height = i;
				break;
			}
		}

		if (height == -1) {
			throw new InvalidOperationException("No map/instruction split found");
		}

		var width = input[0].Length;
		var map   = new char[width, height];
		var moves = new List<Direction>((input.Length - height) * input[height + 1].Length);
		var robot = Coordinate.Empty;

		for (var x = 0; x < width; x++) {
			for (var y = 0; y < height; y++) {
				map[x, y] = input[y][x];

				if (map[x, y] == '@') {
					robot = new(x, y);
				}
			}
		}

		for (var i = height + 1; i < input.Length; i++) {
			for (var x = 0; x < input[i].Length; x++)
			moves.Add(input[i][x] switch {
				'<' => Direction.Left,
				'>' => Direction.Right,
				'^' => Direction.Up,
				'v' => Direction.Down,
				_ => throw new InvalidOperationException("Unrecognized instruction"),
			});
		}

		return (map, robot, moves);
	}

	private static (char[,] Map, Coordinate Robot) ExpandMap(char[,] map)
	{
		var oldWidth  = map.GetLength(0);
		var oldHeight = map.GetLength(1);
		var newMap    = new char[oldWidth * 2, oldHeight];
		var robotPos  = Coordinate.Empty;

		ReadOnlySpan<char> wall  = ['#', '#'];
		ReadOnlySpan<char> box   = ['[', ']'];
		ReadOnlySpan<char> floor = ['.', '.'];
		ReadOnlySpan<char> robot = ['@', '.'];

		for (var x = 0; x < oldWidth; x++) {
			for (var y = 0; y < oldHeight; y++) {
				var newChars = map[x,y] switch {
					'#' => wall,
					'O' => box,
					'.' => floor,
					'@' => robot,
					_ => throw new InvalidOperationException("invalid char encountered"),
				};

				newMap[(x * 2) + 0, y] = newChars[0];
				newMap[(x * 2) + 1, y] = newChars[1];

				if (map[x,y] == '@') {
					robotPos = new Coordinate(x * 2, y);
				}
			}
		}

		return (newMap, robotPos);
	}

	private enum Direction
	{
		Up,
		Right,
		Down,
		Left
	}
}
