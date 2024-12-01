namespace AdventOfCode.Year2023.Day16;

public partial class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	private static readonly Coordinate _north = new(0, -1);
	private static readonly Coordinate _south = new(0, +1);
	private static readonly Coordinate _east = new(+1, 0);
	private static readonly Coordinate _west = new(-1, 0);

	private static readonly Dictionary<char, Dictionary<Coordinate, Coordinate>> _reflections = new() {
		{ '/', new Dictionary<Coordinate, Coordinate>() {
			{ _north, _east },
			{ _south, _west },
			{ _east, _north },
			{ _west, _south },
		}},
		{ '\\', new Dictionary<Coordinate, Coordinate>() {
			{ _north, _west },
			{ _south, _east },
			{ _east, _south },
			{ _west, _north },
		}},
	};

	private static readonly Dictionary<char, (Coordinate delta1, Coordinate delta2)> _splitDirections = new() {
		{ '|', (_north, _south) },
		{ '-', (_east, _west) },
	};

	public static (long, long) Execute(string[] input)
	{
		var map       = CreateMap(input, c => c);
		var entry     = new Coordinate(0, 0);
		var seen      = new HashSet<(Coordinate, Coordinate)>();
		var energized = new HashSet<Coordinate>();
		var part2     = 0;

		// beam enters top-left corner ([0,0]), heading to the right (+x)
		CalculateBeam(map, entry, _east, energized, seen);
		var part1 = energized.Count;

		void RunTest(Coordinate start, Coordinate direction)
		{
			// clear our state
			energized.Clear();
			seen.Clear();

			// run the test
			CalculateBeam(map, start, direction, energized, seen);

			// save the results
			part2 = Math.Max(part2, energized.Count);
		}

		// now, calculate it for every other possible entry; start with west-edge entering east-wise
		for (var i = 1; i < map.GetLength(1); i++) {
			RunTest((0, i), _east);
		}

		// east-edge entering west-wise
		for (var i = 1; i < map.GetLength(1); i++) {
			RunTest((map.GetLength(0) - 1, i), _west);
		}

		// north-edge entering south-wise
		for (var i = 0; i < map.GetLength(0); i++) {
			RunTest((i, 0), _south);
		}

		// south-edge entering north-wise
		for (var i = 0; i < map.GetLength(0); i++) {
			RunTest((i, map.GetLength(1) - 1), _north);
		}

		// 7839 is too low for part 2

		return (part1, part2);
	}

	private static void CalculateBeam(char[,] map, Coordinate from, Coordinate delta, HashSet<Coordinate> energized, HashSet<(Coordinate, Coordinate)> seen)
	{
		while (true) {
			// if we're off the edge of the map, then we can be done
			if (from.X < 0 || from.X > map.GetLength(0) - 1 || from.Y < 0 || from.Y > map.GetLength(1) - 1) {
				return;
			}

			// figure out what kind of location this is
			var fromChar  = map[from.X, from.Y];
			var thisState = (from, delta);

			// if we've already calculated this beam, we can skip it this time
			if (!seen.Add(thisState)) {
				return;
			}

			// energize this coordinate
			energized.Add(from);

			// and manipulate the beam based on what's there
			switch (fromChar) {
				case '.':
					// empty space, nothing special to do
					break;

				case '|' when delta == _north || delta == _south:
				case '-' when delta == _east || delta == _west:
					// splitter, but we're going parallel to it
					break;

				case '/':
				case '\\':
					// mirror, reflect
					delta = _reflections[fromChar][delta];
					break;

				case '|' when delta == _east || delta == _west:
				case '-' when delta == _north || delta == _south:
					// splitter, which splits the beam
					var (dir1, dir2) = _splitDirections[fromChar];
					// first, send the east beam off to be calculated
					CalculateBeam(map, from + dir1, dir1, energized, seen);
					// then, we continue west
					delta = dir2;
					break;
			}

			// advance the beam in the appropriate direction
			from += delta;
		}
	}
}
