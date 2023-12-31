namespace aoc_2019.Day19;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Main);

	public static (long, long) Main(string[] input)
	{
		var program = input.Single().Split(',').Select(long.Parse).ToList();
		var part1   = Enumerable.Range(0, 50).SelectMany(x => Enumerable.Range(0, 50).Select(y => (x, y))).Count(c => Affected(program, c.x, c.y));
		var part2   = Part2(program);

		return (part1, part2);
	}

	private static int Part2(IEnumerable<long> program)
	{
		var topSlope    = Enumerable.Range(0, 100).First(y => Affected(program, 49, y)) / 49f;
		var bottomSlope = 49f / Enumerable.Range(0, 50).First(x => Affected(program, x, 49));

		var testWidth = 100;
		var testX     = testWidth;

		while (++testX < int.MaxValue) {
			//Console.WriteLine($"testing {testX}");

			// find the top Y which is affected at the current X
			// this is UGLY... the 20 is a totally arbitrary value here that works for my input
			var testY = (int)(topSlope * testX) - 20;

			while (true) {
				if (Affected(program, testX, testY)) {
					break;
				}

				testY++;
			}

			var topLeft     = new Coordinate(testX - testWidth + 1, testY);
			var topRight    = new Coordinate(testX, testY);
			var bottomLeft  = new Coordinate(testX - testWidth + 1, testY + testWidth - 1);
			var bottomRight = new Coordinate(testX, testY + testWidth - 1);

			// testX, testY is affected and is the top affected coordinate at testX
			// if testX - testWidth is not affected, then the beam is not wide enough at this X
			if (!Affected(program, topLeft.X, topLeft.Y)) {
				continue;
			}

			if (!Affected(program, bottomLeft.X, bottomLeft.Y)) {
				continue;
			}

			return (topLeft.X * 10000) + topLeft.Y;
		}

		throw new Exception("Halting problem, or something.");
	}

	private static bool Affected(IEnumerable<long> program, int x, int y)
	{
		var computer = new IntcodeComputer(program);
		var inputs   = new Queue<int>(new[] { x, y });
		var ret      = false;

		computer.Input  += (s, e) => inputs.Dequeue();
		computer.Output += (s, e) => ret = e.OutputValue == 1L;

		computer.Resume();

		return ret;
	}
}
