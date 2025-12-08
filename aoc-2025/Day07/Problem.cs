namespace AdventOfCode.Year2025.Day07;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		//var beams = new long[input[0].Length];
		Span<long> beams = stackalloc long[input[0].Length];
		var p1    = 0;

		// mark the starting point
		beams[input[0].IndexOf('S')] = 1;

		// and we go from the 2nd row down
		for (var y = 2; y < input.Length; y += 2) {
			for (var x = 0; x < input[y].Length; x++) {
				// if the row above has a beam, and this row has a splitter, then
				// this row no longer has a beam at this x, and instead has one to
				// the left and right
				if (beams[x] > 0 && input[y][x] == '^') {
					p1++;
					beams[x - 1] += beams[x];
					beams[x + 1] += beams[x];
					beams[x    ] = 0;
				}
			}
		}

		var p2 = 0L;

		for (var i = 0; i < beams.Length; i++) {
			p2 += beams[i];
		}

		return (p1, p2);
	}

	public class Day07Tests
	{
		private readonly string[] _sampleData = """
			.......S.......
			...............
			.......^.......
			...............
			......^.^......
			...............
			.....^.^.^.....
			...............
			....^.^...^....
			...............
			...^.^...^.^...
			...............
			..^...^.....^..
			...............
			.^.^.^.^.^...^.
			...............
			""".Split("\r\n");

		[Test]
		public async Task Part1Works()
		{
			await Assert.That(Execute(_sampleData).Item1).IsEqualTo(21);
		}

		[Test]
		public async Task Part2Works()
		{
			await Assert.That(Execute(_sampleData).Item2).IsEqualTo(40);
		}
	}
}
