namespace AdventOfCode.Year2021.Day25;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		var map   = ParseMap(input);
		var steps = 0;

		while (true) {
			var stepped = Step(map);
			steps++;

			if (map.Cast<char>().SequenceEqual(stepped.Cast<char>())) {
				Console.WriteLine($"Stopped moving after {steps} steps.");
				return (steps, -1);
			}

			map = stepped;
		}
	}

	private static string RenderMap(char[,] map)
	{
		var sb = new StringBuilder();

		for (var y = 0; y < map.GetLength(1); y++) {
			for (var x = 0; x < map.GetLength(0); x++) {
				sb.Append(map[x, y] == '\0' ? '.' : map[x, y]);
			}

			if (y < map.GetLength(1) - 1) {
				sb.AppendLine();
			}
		}

		return sb.ToString();
	}

	private static char[,] ParseMap(string[] map)
	{
		if (!map.All(line => line.Length == map[0].Length)) {
			throw new InvalidOperationException("Mismatched map line lengths.");
		}

		var ret = new char[map[0].Length, map.Length];

		for (var y = 0; y < map.Length; y++) {
			for (var x = 0; x < map[y].Length; x++) {
				ret[x, y] = map[y][x] == '.' ? '\0' : map[y][x];
			}
		}

		return ret;
	}

	private static char[,] Step(char[,] map)
	{
		var ret = new char[map.GetLength(0), map.GetLength(1)];

		// east-facing sea cucumbers
		for (var x = 0; x < map.GetLength(0); x++) {
			// if we're at the rightmost edge of the map, wrap around
			var xdest = x == map.GetLength(0) - 1 ? 0 : x + 1;

			for (var y = 0; y < map.GetLength(1); y++) {
				if (map[x, y] == '>') {
					// move if clear, otherwise wait
					if (map[xdest, y] == '\0') {
						ret[xdest, y] = '>';
					} else {
						ret[x, y] = '>';
					}
				} else if (map[x, y] == 'v') {
					ret[x, y] = 'v';
				}
			}
		}

		map = ret;
		ret = new char[map.GetLength(0), map.GetLength(1)];

		// south-facing sea cucumbers
		for (var y = 0; y < map.GetLength(1); y++) {
			// if we're at the bottom edge of the map, wrap around
			var ydest = y == map.GetLength(1) - 1 ? 0 : y + 1;

			for (var x = 0; x < map.GetLength(0); x++) {
				if (map[x, y] == 'v') {
					// move if clear, otherwise wait
					if (map[x, ydest] == '\0') {
						ret[x, ydest] = 'v';
					} else {
						ret[x, y] = 'v';
					}
				} else if (map[x, y] == '>') {
					ret[x, y] = '>';
				}
			}
		}

		// initialize the new map to empty
		//for (var y = 0; y < map.GetLength(1); y++) {
		//	for (var x = 0; x < map.GetLength(0); x++) {
		//		ret[x, y] = '.';
		//	}
		//}

		return ret;
	}

	[Test]
	public async Task SimpleEastwardMovesWorkCorrectly()
	{
		var map = ParseMap(new[] { "...>>>>>..." });

		await Assert.That(RenderMap(map)).IsEqualTo("...>>>>>...");

		map = Step(map);

		await Assert.That(RenderMap(map)).IsEqualTo("...>>>>.>..");

		map = Step(map);

		await Assert.That(RenderMap(map)).IsEqualTo("...>>>.>.>.");

		map = Step(map);

		await Assert.That(RenderMap(map)).IsEqualTo("...>>.>.>.>");

		map = Step(map);

		await Assert.That(RenderMap(map)).IsEqualTo(">..>.>.>.>.");
	}

	[Test]
	public async Task SimpleCombinedMovesWorkCorrectly()
	{
		var map = ParseMap(@"..........
.>v....v..
.......>..
..........".Split(Environment.NewLine));

		await Assert.That(RenderMap(map)).IsEqualTo(@"..........
.>v....v..
.......>..
..........");

		map = Step(map);

		await Assert.That(RenderMap(map)).IsEqualTo(@"..........
.>........
..v....v>.
..........");
	}

	[Test]
	public async Task WrapAroundMovesWorkCorrectly()
	{
		var map = ParseMap(@"...>...
.......
......>
v.....>
......>
.......
..vvv..".Split(Environment.NewLine));

		await Assert.That(RenderMap(map)).IsEqualTo(@"...>...
.......
......>
v.....>
......>
.......
..vvv..");

		map = Step(map);

		// after 1 step:
		await Assert.That(RenderMap(map)).IsEqualTo(@"..vv>..
.......
>......
v.....>
>......
.......
....v..");

		map = Step(map);

		// after 2 steps:
		await Assert.That(RenderMap(map)).IsEqualTo(@"....v>.
..vv...
.>.....
......>
v>.....
.......
.......");

		map = Step(map);

		// after 3 steps:
		await Assert.That(RenderMap(map)).IsEqualTo(@"......>
..v.v..
..>v...
>......
..>....
v......
.......");

		map = Step(map);

		// after 4 steps:
		await Assert.That(RenderMap(map)).IsEqualTo(@">......
..v....
..>.v..
.>.v...
...>...
.......
v......");
	}

	[Test]
	public async Task SampleInputStepsCorrectly()
	{
		await Assert.That(Execute(ReadFileLines("input_sample.txt")).Item1).IsEqualTo(58);
	}
}
