using System.Collections.Concurrent;
using System.Text;

namespace aoc_2019.Day15;

public class Problem
{
	private const long MOVEMENT_NORTH = 1;
	private const long MOVEMENT_SOUTH = 2;
	private const long MOVEMENT_WEST  = 3;
	private const long MOVEMENT_EAST  = 4;

	private const long STATUS_WALL  = 0;
	private const long STATUS_MOVED = 1;
	private const long STATUS_FOUND = 2;

	private const bool DRAW_MAP = false;

	private static int PropagateOxygen(Tile[,] map, int oxygenX, int oxygenY)
	{
		var xmax         = map.GetLength(0);
		var ymax         = map.GetLength(1);
		var newLocations = new List<(int x, int y)>();
		var minutes      = 0;

		map[oxygenX, oxygenY] = Tile.Oxygen;

		while (true) {
			// go through the map and find everywhere that will become oxygen
			for (var y = 0; y < ymax; y++) {
				for (var x = 0; x < xmax; x++) {
					if (map[x, y] == Tile.Oxygen) {
						var northPos = (x: x,     y: y - 1);
						var southPos = (x: x,     y: y + 1);
						var eastPos  = (x: x - 1, y: y);
						var westPos  = (x: x + 1, y: y);

						if (map[northPos.x, northPos.y] == Tile.Floor) {
							newLocations.Add(northPos);
						}

						if (map[southPos.x, southPos.y] == Tile.Floor) {
							newLocations.Add(southPos);
						}
						if (map[eastPos.x, eastPos.y] == Tile.Floor) {
							newLocations.Add(eastPos);
						}
						if (map[westPos.x, westPos.y] == Tile.Floor) {
							newLocations.Add(westPos);
						}
					}
				}
			}

			// if we didn't find any new locations, we're done
			if (newLocations.Count == 0) {
				return minutes; // 371 is wrong
			}

			// now, go through and mark all the new locations as oxygen
			foreach (var pos in newLocations) {
				map[pos.x, pos.y] = Tile.Oxygen;
			}

			// one more minute has passed
			minutes++;

			// clear out our list of new locations
			newLocations.Clear();

			DrawMap(map, (oxygenX, oxygenY));
		}
	}

	public static (int, int) Main(string fileName)
	{
		var program     = ReadFileLines(fileName).Single();
		var computer    = new IntcodeComputer(program);

		// first, explore the complete map to see what we're dealing with
		var (map, origin) = ExploreMap(computer);
		var xmax          = map.GetLength(0);
		var ymax          = map.GetLength(1);
		var oxSysPos      = FindTile(map, Tile.OxygenSystem);

		if (oxSysPos == null) {
			throw new InvalidDataException("Unable to locate the oxygen system on the map.");
		}

		// next, part 1 is the shortest path from the origin to the oxygen system
		var part1 = AStar.FindPath(origin, oxSysPos.Value, coordinate => {
			var ret = new List<(int, int)>();

			if (map[coordinate.X + 1, coordinate.Y] == Tile.Floor || map[coordinate.X + 1, coordinate.Y] == Tile.OxygenSystem) {
				ret.Add((coordinate.X + 1, coordinate.Y));
			}

			if (map[coordinate.X - 1, coordinate.Y] == Tile.Floor || map[coordinate.X - 1, coordinate.Y] == Tile.OxygenSystem) {
				ret.Add((coordinate.X - 1, coordinate.Y));
			}

			if (map[coordinate.X, coordinate.Y + 1] == Tile.Floor || map[coordinate.X, coordinate.Y + 1] == Tile.OxygenSystem) {
				ret.Add((coordinate.X, coordinate.Y + 1));
			}

			if (map[coordinate.X, coordinate.Y - 1] == Tile.Floor || map[coordinate.X, coordinate.Y - 1] == Tile.OxygenSystem) {
				ret.Add((coordinate.X, coordinate.Y - 1));
			}

			return ret.Select(item => (new Coordinate(item.Item1, item.Item2), 1f));
		})!.Count;

		// and part 2 is how long the oxygen takes to propagate throughout the entire map
		var part2 = PropagateOxygen(map, oxSysPos.Value.x, oxSysPos.Value.y);

		// part1 - 1 because the pathfinding algo includes the start position
		return (part1 - 1, part2);
	}

	private static Tile[,] BuildMap(int width, int height)
	{
		var map  = new Tile[width, height];
		var xmax = map.GetLength(0);
		var ymax = map.GetLength(1);

		for (var y = 0; y < ymax; y++) {
			for (var x = 0; x < xmax; x++) {
				map[x, y] = Tile.Unknown;
			}
		}

		return map;
	}

	private static (Tile[,] map, (int x, int y) origin) ExploreMap(IntcodeComputer computer)
	{
		var tokenSource  = new CancellationTokenSource();
		var inputStream  = new BlockingCollection<long>();
		var outputStream = new BlockingCollection<long>();
		var currentPos   = (x: 21, y: 21);      // NOTE: these numbers are chosen for my specific input. You can make them arbitrarily large for
		var map          = BuildMap(41, 41);    // your input... just put the origin in the middle-ish of your map, or trial-and-error like me.
		var explored     = new HashSet<(int x, int y)>();
		var backtracks   = new Stack<Stack<long>>();
		var currentPath  = new Stack<long>();
		var origin       = currentPos;

		computer.Input  += (s, e) => inputStream.Take();
		computer.Output += (s, e) => outputStream.Add(e.OutputValue);

		var computerTask = Task.Run(computer.Resume, tokenSource.Token);

		while (true) {
			// explore the room we're in
			ExploreRoom(map, currentPos, inputStream, outputStream);

			// keep track of where we've been
			explored.Add(currentPos);

			// draw the map for fun
			DrawMap(map, currentPos);

			// calculate the positions of the adjacent locations
			var northDest = (x: currentPos.x, y: currentPos.y - 1);
			var southDest = (x: currentPos.x, y: currentPos.y + 1);
			var eastDest  = (x: currentPos.x + 1, y: currentPos.y);
			var westDest  = (x: currentPos.x - 1, y: currentPos.y);

			// check what exits are valid
			var northValid = (map[northDest.x, northDest.y] == Tile.Floor || map[northDest.x, northDest.y] == Tile.OxygenSystem) && !explored.Contains(northDest);
			var southValid = (map[southDest.x, southDest.y] == Tile.Floor || map[southDest.x, southDest.y] == Tile.OxygenSystem) && !explored.Contains(southDest);
			var eastValid  = (map[eastDest.x,  eastDest.y]  == Tile.Floor || map[eastDest.x,  eastDest.y]  == Tile.OxygenSystem) && !explored.Contains(eastDest);
			var westValid  = (map[westDest.x,  westDest.y]  == Tile.Floor || map[westDest.x,  westDest.y]  == Tile.OxygenSystem) && !explored.Contains(westDest);

			var validExits = $"{(northValid ? "N" : string.Empty)}{(southValid ? "S" : string.Empty)}{(eastValid ? "E" : string.Empty)}{(westValid ? "W" : string.Empty)}";

			//Console.WriteLine($"valid exits are {validExits}");

			if ((northValid ? 1 : 0) + (southValid ? 1 : 0) + (eastValid ? 1 : 0) + (westValid ? 1 : 0) > 1) {
				// so, here we'll have >1 valid, each of which is not in explored
				// so we need to push our current path onto the path stack, then start a new path
				backtracks.Push(currentPath);
				currentPath = new();
			}

			if (outputStream.TryTake(out long val)) {
				throw new InvalidDataException($"shouldn't have gotten output, but got {val}");
			}

			if (northValid) {
				//Console.WriteLine("Moving north");
				currentPos = MakeMoves(currentPos, inputStream, outputStream, MOVEMENT_NORTH);
				currentPath.Push(ReverseMove(MOVEMENT_NORTH));
			} else if (southValid) {
				//Console.WriteLine("Moving south");
				currentPos = MakeMoves(currentPos, inputStream, outputStream, MOVEMENT_SOUTH);
				currentPath.Push(ReverseMove(MOVEMENT_SOUTH));
			} else if (eastValid) {
				//Console.WriteLine("Moving east");
				currentPos = MakeMoves(currentPos, inputStream, outputStream, MOVEMENT_EAST);
				currentPath.Push(ReverseMove(MOVEMENT_EAST));
			} else if (westValid) {
				//Console.WriteLine("Moving west");
				currentPos = MakeMoves(currentPos, inputStream, outputStream, MOVEMENT_WEST);
				currentPath.Push(ReverseMove(MOVEMENT_WEST));
			} else {
				// reverse us down the path we took
				currentPos = MakeMoves(currentPos, inputStream, outputStream, currentPath.ToArray());

				if (backtracks.Count == 0) {
					// this path is dead-ended and is the last path we were working on... so we're done
					break;
				} else {
					// forget our current path and go back to the one we were building previously
					currentPath = backtracks.Pop();
				}
			}
		}

		Console.WriteLine("finished.");
		return (map, origin);
	}

	private static (int x, int y)? FindTile(Tile[,] map, Tile tileType)
	{
		var xmax = map.GetLength(0);
		var ymax = map.GetLength(1);

		for (var y = 0; y < ymax; y++) {
			for (var x = 0; x < xmax; x++) {
				if (map[x, y] == tileType) {
					return (x, y);
				}
			}
		}

		return null;
	}

	private static (int x, int y) MakeMoves((int x, int y) currentLocation, BlockingCollection<long> computerInput, BlockingCollection<long> computerOutput, params long[] movements)
	{
		for (var i = 0; i < movements.Length; i++) {
			var intendedMove     = movements[i];
			var intendedPosition = intendedMove switch {
				MOVEMENT_NORTH => (x: currentLocation.x,     y: currentLocation.y - 1),
				MOVEMENT_SOUTH => (x: currentLocation.x,     y: currentLocation.y + 1),
				MOVEMENT_WEST  => (x: currentLocation.x - 1, y: currentLocation.y),
				MOVEMENT_EAST  => (x: currentLocation.x + 1, y: currentLocation.y),
				_ => throw new InvalidOperationException($"Could not derive an intended destination for intended direction {intendedMove}"),
			};

			computerInput.Add(movements[i]);

			currentLocation = computerOutput.Take() switch {
				STATUS_WALL  => throw new InvalidOperationException("Tried to move but encountered a wall."),
				STATUS_MOVED => intendedPosition,
				STATUS_FOUND => intendedPosition,
				_ => throw new InvalidOperationException("Tried to move and got an unknown status"),
			};
		}

		return currentLocation;
	}

	private static void ExploreRoom(Tile[,] map, (int x, int y) currentLocation, BlockingCollection<long> computerInput, BlockingCollection<long> computerOutput)
	{
		ExploreDirection(MOVEMENT_NORTH);
		ExploreDirection(MOVEMENT_EAST);
		ExploreDirection(MOVEMENT_SOUTH);
		ExploreDirection(MOVEMENT_WEST);

		void ExploreDirection(long intendedMove)
		{
			var moveSuccessful   = false;
			var intendedPosition = intendedMove switch {
				MOVEMENT_NORTH => (x: currentLocation.x,     y: currentLocation.y - 1),
				MOVEMENT_SOUTH => (x: currentLocation.x,     y: currentLocation.y + 1),
				MOVEMENT_WEST  => (x: currentLocation.x - 1, y: currentLocation.y),
				MOVEMENT_EAST  => (x: currentLocation.x + 1, y: currentLocation.y),
				_ => throw new InvalidOperationException($"Could not derive an intended destination for intended direction {intendedMove}"),
			};

			if (map[intendedPosition.x, intendedPosition.y] == Tile.Unknown) {
				// move the robot
				computerInput.Add(intendedMove);

				// and get the response
				switch (computerOutput.Take()) {
					case STATUS_WALL:
						// we didn't move
						//Console.WriteLine($"Tried to move {intendedMove} but failed with STATUS_WALL");
						map[intendedPosition.x, intendedPosition.y] = Tile.Wall;
						break;

					case STATUS_MOVED:
						// we moved, it's a floor
						//Console.WriteLine($"Tried to move {intendedMove} and succeeded with STATUS_MOVED");
						map[intendedPosition.x, intendedPosition.y] = Tile.Floor;
						moveSuccessful = true;
						break;

					case STATUS_FOUND:
						// we moved, it's the oxygen system
						//Console.WriteLine($"Tried to move {intendedMove} and succeeded with STATUS_FOUND");
						map[intendedPosition.x, intendedPosition.y] = Tile.OxygenSystem;
						moveSuccessful = true;
						break;
				}
			}

			// if we moved, move back
			if (moveSuccessful) {
				MakeMoves((0, 0), computerInput, computerOutput, ReverseMove(intendedMove));
			}
		}
	}

	private static long ReverseMove(long move) => move switch {
		MOVEMENT_NORTH => MOVEMENT_SOUTH,
		MOVEMENT_EAST  => MOVEMENT_WEST,
		MOVEMENT_SOUTH => MOVEMENT_NORTH,
		MOVEMENT_WEST  => MOVEMENT_EAST,
		_ => throw new InvalidOperationException($"Could not derive a reverse move for intended direction {move}"),
	};

	private static void DrawMap(Tile[,] map, (int x, int y) currentPosition) => DrawMap(map, new Point(currentPosition.x, currentPosition.y));

	private static void DrawMap(Tile[,] map, Point currentPosition)
	{
#pragma warning disable CS0162
		if (!DRAW_MAP) {
			return;
		}

		var xmax = map.GetLength(0);
		var ymax = map.GetLength(1);
		var sb   = new StringBuilder();

		var (currentX, currentY) = currentPosition;

		for (var y = 0; y < ymax; y++) {
			for (var x = 0; x < xmax; x++) {
				sb.Append(x == currentX && y == currentY ? 'Ω' : map[x, y] switch {
					Tile.Unknown => ' ',
					Tile.Wall => '█',
					Tile.Floor => '▒',
					Tile.OxygenSystem => 'φ',
					Tile.Oxygen => 'O',
					_ => '?'
				});
			}
			sb.AppendLine();
		}

		Console.Clear();
		Console.WriteLine(sb.ToString());
#pragma warning restore CS0162
	}

	[Fact]
	public void OxygenPropagatesCorrectly()
	{
		var map = new Tile[6, 5];

		map[0, 0] = Tile.Unknown; map[1, 0] = Tile.Wall;  map[2, 0] = Tile.Wall;         map[3, 0] = Tile.Unknown; map[4, 0] = Tile.Unknown; map[5, 0] = Tile.Unknown;
		map[0, 1] = Tile.Wall;    map[1, 1] = Tile.Floor; map[2, 1] = Tile.Floor;        map[3, 1] = Tile.Wall;    map[4, 1] = Tile.Wall;    map[5, 1] = Tile.Unknown;
		map[0, 2] = Tile.Wall;    map[1, 2] = Tile.Floor; map[2, 2] = Tile.Wall;         map[3, 2] = Tile.Floor;   map[4, 2] = Tile.Floor;   map[5, 2] = Tile.Wall;
		map[0, 3] = Tile.Wall;    map[1, 3] = Tile.Floor; map[2, 3] = Tile.OxygenSystem; map[3, 3] = Tile.Floor;   map[4, 3] = Tile.Wall;    map[5, 3] = Tile.Unknown;
		map[0, 4] = Tile.Unknown; map[1, 4] = Tile.Wall;  map[2, 4] = Tile.Wall;         map[3, 4] = Tile.Wall;    map[4, 4] = Tile.Unknown; map[5, 4] = Tile.Unknown;

		Assert.Equal(4, PropagateOxygen(map, 2, 3));
	}

	private enum Tile : long
	{
		Unknown      = -1,
		Wall         = 0,
		Floor        = 1,
		OxygenSystem = 2,
		Oxygen       = 99,
	}
}
