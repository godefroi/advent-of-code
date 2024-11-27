namespace AdventOfCode.Year2016.Day01;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Main);

	public static (long, long) Main(string[] input)
	{
		// North is +Y
		// East is +X

		var dir_north = new Coordinate(+0, +1);
		var dir_south = new Coordinate(+0, -1);
		var dir_east  = new Coordinate(+1, +0);
		var dir_west  = new Coordinate(-1, +0);
		var pos       = new Coordinate(0, 0);
		var move      = new Coordinate(+0, +1);
		var posSet    = new HashSet<Coordinate>();
		var bunnyHQ   = Coordinate.Empty;

		foreach (var instruction in input.Single().AsSpan().EnumerateBySplitting(", ")) {
			move = instruction[0] switch {
				'L' when move == dir_north => dir_west,
				'L' when move == dir_south => dir_east,
				'L' when move == dir_east => dir_north,
				'L' when move == dir_west => dir_south,
				'R' when move == dir_north => dir_east,
				'R' when move == dir_south => dir_west,
				'R' when move == dir_east => dir_south,
				'R' when move == dir_west => dir_north,
				_ => throw new InvalidOperationException($"Unsupported condition: turn {instruction[0]} while move={move}"),
			};

			var moveCnt = int.Parse(instruction[1..]);

			if (bunnyHQ == Coordinate.Empty) {
				for (var i = 0; i < moveCnt; i++) {
					pos += move;

					if (bunnyHQ == Coordinate.Empty && !posSet.Add(pos)) {
						bunnyHQ = pos;
					}
				}
			} else {
				pos += move * moveCnt;
			}
			Console.WriteLine($"Moved to {pos}");
		}

		return (Coordinate.ManhattanDistance((0, 0), pos), bunnyHQ == Coordinate.Empty ? long.MinValue : Coordinate.ManhattanDistance((0, 0), bunnyHQ));
	}

	[Theory]
	[InlineData(new string[] { "R2, L3" }, 5)]
	[InlineData(new string[] { "R2, R2, R2" }, 2)]
	[InlineData(new string[] { "R5, L5, R5, R3" }, 12)]
	public void Part1CalculatesCorrectly(string[] input, long distance)
	{
		var (p1, _) = Main(input);

		Assert.Equal(distance, p1);
	}

	[Theory]
	[InlineData(new string[] { "R8, R4, R4, R8" }, 4)]
	public void Part2CalculatesCorrectly(string[] input, long distance)
	{
		var (_, p2) = Main(input);

		Assert.Equal(distance, p2);
	}
}
