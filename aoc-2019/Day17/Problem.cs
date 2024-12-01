using System.Collections.Concurrent;

namespace AdventOfCode.Year2019.Day17;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Main, typeof(Problem), null);

	public static (long, long) Main(string[] input)
	{
		var program = input.Single();
		var map     = ReadField(program);
		var part1   = FindIntersections(map);

		DrawField(map);

		var path = DerivePath(map).SkipWhile(s => int.TryParse(s, out _)).ToList();

		for (var i = path.Count - 2; i > 0; i -= 2) {
			//Console.WriteLine($"{path.Count} -> {i}");
			var subPath  = path.Take(i).ToList();
			var foundPos = FindSequence(path, subPath, 2, false).ToList();

			if (foundPos.Count > 0) {
				var subPathStr = string.Join(',', subPath);

				foundPos.Insert(0, 0);

				Console.WriteLine($"{i} ({subPathStr}) => {string.Join(',', foundPos)} => {subPathStr.Length * foundPos.Count} chars replaced with {foundPos.Count * 2} chars");
			}
		}

		Console.WriteLine(string.Join(',', path));

		// So, I did this manually based on the above output. Sue me.
		// VSCode helpfully highlights other occurrances of strings when you select them...
		var subA = "L,6,R,8,L,4,R,8,L,12";
		var subB = "L,12,R,10,L,4";
		var subC = "L,12,L,6,L,4,L,4";
		var main = "A,B,B,C,B,C,B,C,A,A";

		var part2 = ExecutePath(program, subA, subB, subC, main);

		return (part1, part2);
	}

	private static int ExecutePath(string programString, string subroutineA, string subroutineB, string subroutineC, string mainRoutine)
	{
		var program = programString.Split(',').Select(long.Parse).ToList();

		// wake up the robot
		program[0] = 2;

		var computer    = new IntcodeComputer(program);
		var inputStream = new BlockingCollection<long>();
		var ret         = -1;

		void SendCommand(string command)
		{
			// send the command
			foreach (var c in command.ToCharArray()) {
				inputStream.Add(c);
			}

			// then a newline
			inputStream.Add(10);
		}

		SendCommand(mainRoutine);
		SendCommand(subroutineA);
		SendCommand(subroutineB);
		SendCommand(subroutineC);
		SendCommand("n"); // no continuous video output

		computer.Input  += (s, e) => {
			var nextInput = inputStream.Take();
			//Console.WriteLine($"inputting {nextInput}");
			//Console.ReadKey(true);
			return nextInput;
		};
		computer.Output += (s, e) => {
			if (e.OutputValue > 255) {
				ret = (int)e.OutputValue;
			} else {
				Console.Write((char)e.OutputValue);
			}
		};

		computer.Resume();

		return ret;
	}

	private static Tile[,] ReadField(string program)
	{
		var computer = new IntcodeComputer(program);
		var tileVals = Enum.GetValues<Tile>().ToDictionary(t => (long)t);
		var curList  = new List<Tile>();
		var outp     = new List<List<Tile>>() { curList };

		Tile[,]? map = default;

		computer.Output += (s, e) => {
			// output comes as ascii
			if (e.OutputValue == 10 && curList.Count == 0) {
				// we have complete output
				var widest = outp.Max(l => l.Count);

				map = new Tile[outp.Max(l => l.Count), outp.Count - 1];

				// build the map in a convenient type
				for (var x = 0; x < widest; x++) {
					for (var y = 0; y < outp.Count - 1; y++) {
						map[x, y] = outp[y][x];
					}
				}
			} else if (e.OutputValue == 10) {
				curList = new List<Tile>();
				outp.Add(curList);
			} else {
				if (!tileVals.TryGetValue(e.OutputValue, out var tile)) {
					throw new InvalidOperationException($"Unknown tile {(char)e.OutputValue}");
				} else {
					curList.Add(tile);
				}
			}
		};

		computer.Resume();

		if (map == null) {
			throw new InvalidDataException("The map was not successfully built.");
		}

		return map;
	}

	private static void DrawField(Tile[,] field) => Console.WriteLine(string.Join(Environment.NewLine, Enumerable.Range(0, field.GetLength(1)).Select(y => string.Join(string.Empty, Enumerable.Range(0, field.GetLength(0)).Select(x => ((char)field[x, y]).ToString())))));

	private static IEnumerable<string> DerivePath(Tile[,] map)
	{
		var width  = map.GetLength(0);
		var height = map.GetLength(1);
		var robot  = default((int x, int y)?);

		for (var x = 0; x < width && robot == null; x++) {
			for (var y = 0; y < height && robot == null; y++) {
				if (map[x, y] == Tile.RobotUp || map[x, y] == Tile.RobotDown || map[x, y] == Tile.RobotLeft || map[x, y] == Tile.RobotRight) {
					robot = (x, y);
				}
			}
		}

		if (robot == null) {
			throw new InvalidDataException("Unable to locate robot on map");
		}

		var (currentX, currentY) = (robot.Value.x, robot.Value.y);

		var (xMod, yMod) = map[currentX, currentY] switch {
			Tile.RobotUp => (0, -1),
			Tile.RobotDown => (0, +1),
			Tile.RobotLeft => (-1, 0),
			Tile.RobotRight => (+1, 0),
			_ => throw new InvalidDataException("Invalid tile type for robot")
		};

		var currentLength = 0;

		Console.WriteLine($"Robot is at {currentX},{currentY} and travelling {xMod},{yMod}");

		while (true) {
			var destX    = currentX + xMod;
			var destY    = currentY + yMod;
			var destTile = GetTile(map, destX, destY);

			// if there's scaffolding in our current direction, then add one to currentLength
			if (destTile == Tile.Scaffold) {
				currentLength++;
				currentX = destX;
				currentY = destY;
				continue;
			}

			var (left, right) = CalculateTurns(map, currentX, currentY, xMod, yMod);

			if (left.tile == Tile.Scaffold && right.tile == Tile.Scaffold) {
				// we can't handle a T intersection
				throw new InvalidDataException("There's a T intersection; we don't know how to handle those.");
			} else if (left.tile == Tile.Scaffold) {
				// tile to the left, turn left
				yield return currentLength.ToString();
				xMod          = left.xMod;
				yMod          = left.yMod;
				currentLength = 0;
				yield return "L";
			} else if (right.tile == Tile.Scaffold) {
				// tile to the right
				yield return currentLength.ToString();
				xMod          = right.xMod;
				yMod          = right.yMod;
				currentLength = 0;
				yield return "R";
			} else {
				// there's no scaffold ahead, and if there's none to the left or right, our path is done
				yield return currentLength.ToString();
				yield break;
			}
		}
	}

	private static ((int xMod, int yMod, Tile tile) left, (int xMod, int yMod, Tile tile) right) CalculateTurns(Tile[,] map, int currentX, int currentY, int xMod, int yMod) => (xMod, yMod) switch {
		(0, -1) => ((-1, 0, map[currentX - 1, currentY + 0]), (+1, 0, GetTile(map, currentX + 1, currentY + 0))), // facing up, turning right goes right
		(+1, 0) => ((0, -1, map[currentX + 0, currentY - 1]), (0, +1, GetTile(map, currentX + 0, currentY + 1))), // facing right, turning right goes down
		(0, +1) => ((+1, 0, map[currentX + 1, currentY + 0]), (-1, 0, GetTile(map, currentX - 1, currentY + 0))), // facing down, turning right goes left
		(-1, 0) => ((0, +1, map[currentX + 0, currentY + 1]), (0, -1, GetTile(map, currentX + 0, currentY - 1))), // facing left, turning right goes up
		_ => throw new InvalidOperationException($"{xMod},{yMod} doesn't seem to be a valid direction."),
	};

	private static Tile GetTile(Tile[,] map, int x, int y)
	{
		if (x < 0 || y < 0) {
			return Tile.None;
		} else if (x >= map.GetLength(0)) {
			return Tile.None;
		} else if (y >= map.GetLength(1)) {
			return Tile.None;
		} else {
			return map[x, y];
		}
	}

	private static int FindIntersections(Tile[,] view)
	{
		var width  = view.GetLength(0);
		var height = view.GetLength(1);
		var ret    = 0;

		for (var x = 1; x < width - 1; x++) {
			for (var y = 1; y < height - 1; y++) {
				if (view[x, y] == Tile.Scaffold) {
					if (view[x + 1, y] == Tile.Scaffold
					&& view[x - 1, y] == Tile.Scaffold
					&& view[x, y - 1] == Tile.Scaffold
					&& view[x, y + 1] == Tile.Scaffold) {
						ret += (x * y);
					}
				}
			}
		}

		return ret;
	}

	private enum Tile : long
	{
		None          = ' ',
		Scaffold      = '#',
		Space         = '.',
		RobotUp       = '^',
		RobotDown     = 'v',
		RobotLeft     = '<',
		RobotRight    = '>',
		RobotTumbling = 'X',
	}
}
