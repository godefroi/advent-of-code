namespace AdventOfCode.Year2024.Day06;

public class Problem
{
	private static readonly Coordinate _up = new(0, -1);
	private static readonly Coordinate _down = new(0, +1);
	private static readonly Coordinate _left = new(-1, 0);
	private static readonly Coordinate _right = new(+1, 0);

	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		var pos = Coordinate.Empty;
		var map = CreateMap(input, (x, y, c) => { if (c == '^') { pos = new(x, y); } return c; });
		var (part1, path) = Part1(map, pos);
		var loopSpots = new HashSet<Coordinate>();
		var part2 = path.AsParallel().Count(step => IsLoop(map, step, pos));

		return (part1, part2);
	}

	private static (int DistinctPositions, HashSet<Coordinate> Path) Part1(char[,] map, Coordinate pos)
	{
		var dir  = _up;
		var xLen = map.GetLength(0);
		var yLen = map.GetLength(1);
		var set  = new HashSet<Coordinate>(6144) { pos };

		while (true) {
			var newPos = pos + dir;

			// if we're off the map, we're done
			if (newPos.X < 0 || newPos.Y < 0 || newPos.X >= xLen || newPos.Y >= yLen) {
				return (set.Count, set);
			}

			// if we're up against an obstacle, turn
			if (map[newPos.X, newPos.Y] == '#') {
				dir = Turn(dir);
				continue;
			}

			// otherwise, march
			pos = newPos;
			set.Add(pos);
		}
	}

	private static bool IsLoop(char[,] map, Coordinate obstacle, Coordinate currentPos)
	{
		// Our task, should we choose to accept it, is to determine whether the path
		// would consist of a loop if an obstacle were added directly in front of us
		var xLen       = map.GetLength(0);
		var yLen       = map.GetLength(1);
		var currentDir = _up;

		// if the obstacle would be off the map, then this isn't a loop
		if (obstacle.X < 0 || obstacle.Y < 0 || obstacle.X >= xLen || obstacle.Y >= yLen) {
			return false;
		}

		var loopPath = new HashSet<(Coordinate Position, Coordinate Direction)>() { (currentPos, currentDir) };

		while (true) {
			var newPos = currentPos + currentDir;

			// if we're off the map, we're done, it's not a loop
			if (newPos.X < 0 || newPos.Y < 0 || newPos.X >= xLen || newPos.Y >= yLen) {
				return false;
			}

			// if we're up against an obstacle, turn
			if (map[newPos.X, newPos.Y] == '#' || newPos == obstacle) {
				currentDir = Turn(currentDir);
				continue;
			}

			// otherwise, march
			currentPos = newPos;

			// if we've been here before, we found a loop
			if (!loopPath.Add((currentPos, currentDir))) {
//Console.WriteLine($"Loop when obstacle placed at {obstacle}");
				return true;
			}
		}
	}

	private static Coordinate Turn(Coordinate dir) => dir switch {
		(0, -1) => _right,
		(+1, 0) => _down,
		(0, +1) => _left,
		(-1, 0) => _up,
		_ => throw new InvalidOperationException($"{dir} is not a valid direction."),
	};

	private static Coordinate FindGuard(char[,] map)
	{
		for (var x = 0; x < map.GetLength(0); x++) {
			for (var y = 0; y < map.GetLength(0); y++) {
				if (map[x, y] == '^') {
					return new(x, y);
				}
			}
		}

		throw new InvalidOperationException("No guard found.");
	}
}
