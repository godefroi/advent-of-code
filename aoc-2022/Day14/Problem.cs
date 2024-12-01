namespace AdventOfCode.Year2022.Day14;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Main, typeof(Problem), null);

	private const char CHAR_SAND    = 'o';
	private const char CHAR_ROCK    = '#';
	private const char CHAR_SOURCE  = '+';
	private const char CHAR_NOTHING = '\0';

	public static (long, long) Main(string[] input)
	{
		var rocks = input.Select(Parse).SelectMany(p => p).SelectMany(EnumeratePoints).ToList();
		var xmin  = rocks.Min(p => p.x);
		var xmax  = rocks.Max(p => p.x);
		var ymin  = rocks.Min(p => p.y);
		var ymax  = rocks.Max(p => p.y);
		var field = new char[xmax + 1 + 400, ymax + 1 + 2];

		Console.WriteLine($"x goes from {xmin} to {xmax}, y goes from {ymin} to {ymax}");

		foreach (var (x, y) in rocks) {
			field[x, y] = CHAR_ROCK;
		}

		field[500, 0] = CHAR_SOURCE;

		for (var x = 0; x < field.GetLength(0); x++) {
			field[x, field.GetLength(1) - 1] = CHAR_ROCK;
		}

		//Draw(field, xmin);
		//return (0, 0);

		var sandCount = 0;
		var part1     = default(int?);
		var part2     = default(int?);

		while (part1 == null || part2 == null) {
			var curX = 500;
			var curY = 0;

			while (true) {
				// if down is into the abyss, then we're done
				if (curY + 1 > ymax && part1 == null) {
					part1 = sandCount;
					break;
				}

				// sand tries to move down
				if (field[curX, curY + 1] == CHAR_NOTHING) {
					curY++;
					continue;
				}

				// if it can't, it tries to move down and left
				if (field[curX - 1, curY + 1] == CHAR_NOTHING) {
					curX--;
					curY++;
					continue;
				}

				// if it can't, it tries to move down and right
				if (field[curX + 1, curY + 1] == CHAR_NOTHING) {
					curX++;
					curY++;
					continue;
				}

				// if it couldn't move at all, then this sand stays here
				sandCount++;

				if (curX == 500 && curY == 0) {
					part2 = sandCount;
				} else {
					field[curX, curY] = CHAR_SAND;
				}

				// go on to the next sand
				break;
			}
		}

		return (part1.Value, part2.Value);
	}

	private static IEnumerable<((int x, int y) from, (int x, int y) to)> Parse(string line)
	{
		var points = line.Split(" -> ").Select(p => {
			var coords = p.Split(',');
			return (x: int.Parse(coords[0]), y: int.Parse(coords[1]));
		}).ToList();

		var prev = points[0];

		for (var i = 1; i < points.Count; i++) {
			yield return (prev, points[i]);
			prev = points[i];
		}
	}

	private static IEnumerable<(int x, int y)> EnumeratePoints(((int x, int y) from, (int x, int y) to) line)
	{
		var xMod = line.from.x == line.to.x ? 0 : line.from.x > line.to.x ? -1 : 1;
		var yMod = line.from.y == line.to.y ? 0 : line.from.y > line.to.y ? -1 : 1;

		var (curX, curY) = (line.from.x, line.from.y);

		yield return (curX, curY);

		while (curX != line.to.x || curY != line.to.y) {
			curX += xMod;
			curY += yMod;

			yield return (curX, curY);
		}
	}

	private static void Draw(char[,] field, int xMin)
	{
		var xMax = field.GetLength(0);
		var yMax = field.GetLength(1);

		for (var y = 0; y < yMax; y++) {
			Console.WriteLine(new string(Enumerable.Range(xMin, xMax - xMin).Select(x => field[x, y]).Select(c => c == '\0' ? ' ' : c).ToArray()));
		}
	}

	[Fact]
	public void SamplePointsParseCorrectly()
	{
		Assert.Collection(ReadFileLines("inputSample.txt", Parse).SelectMany(p => p),
			p => Assert.Equal(((498, 4), (498, 6)), p),
			p => Assert.Equal(((498, 6), (496, 6)), p),
			p => Assert.Equal(((503, 4), (502, 4)), p),
			p => Assert.Equal(((502, 4), (502, 9)), p),
			p => Assert.Equal(((502, 9), (494, 9)), p));
	}

	[Fact]
	public void SamplePointsEnumerateCorrectly()
	{
		Assert.Collection(ReadFileLines("inputSample.txt", Parse).SelectMany(p => p).SelectMany(EnumeratePoints).Distinct().Order(),
			p => Assert.Equal((494, 9), p),
			p => Assert.Equal((495, 9), p),
			p => Assert.Equal((496, 6), p),
			p => Assert.Equal((496, 9), p),
			p => Assert.Equal((497, 6), p),
			p => Assert.Equal((497, 9), p),
			p => Assert.Equal((498, 4), p),
			p => Assert.Equal((498, 5), p),
			p => Assert.Equal((498, 6), p),
			p => Assert.Equal((498, 9), p),
			p => Assert.Equal((499, 9), p),
			p => Assert.Equal((500, 9), p),
			p => Assert.Equal((501, 9), p),
			p => Assert.Equal((502, 4), p),
			p => Assert.Equal((502, 5), p),
			p => Assert.Equal((502, 6), p),
			p => Assert.Equal((502, 7), p),
			p => Assert.Equal((502, 8), p),
			p => Assert.Equal((502, 9), p),
			p => Assert.Equal((503, 4), p));
	}
}
