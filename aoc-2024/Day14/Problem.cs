using System.Numerics;

namespace AdventOfCode.Year2024.Day14;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		var (width, height) = input.Length switch {
			12 => (11, 7),
			500 => (101, 103),
			_ => throw new NotImplementedException($"input lengh {input.Length} is unsupported"),
		};

		var robots       = Parse(input);
		var endPositions = robots
			.Select(r => Move(r.Position, r.Velocity, 100, width, height))
			.ToList();

		var midX = width / 2;
		var midY = height / 2;

		var q1 = endPositions.LongCount(p => p.X < midX && p.Y < midY);
		var q2 = endPositions.LongCount(p => p.X > midX && p.Y < midY);
		var q3 = endPositions.LongCount(p => p.X < midX && p.Y > midY);
		var q4 = endPositions.LongCount(p => p.X > midX && p.Y > midY);
		var part1 = q1 * q2 * q3 * q4;

		var minX  = double.MaxValue;
		var minXS = 0;
		var minY  = double.MaxValue;
		var minYS = 0;
		var tests = Math.Max(width, height);

		for (var s = 0; s <= tests; s++) {
			var positions = robots
				.Select(r => Move(r.Position, r.Velocity, s, width, height))
				.ToList();

			var varX = Variance(positions.Select(p => p.X));
			var varY = Variance(positions.Select(p => p.Y));

			if (varX < minX) {
				minX = varX;
				minXS = s;
			}

			if (varY < minY) {
				minY = varY;
				minYS = s;
			}
		}

		var part2 = minXS + ((PowMod(width, -1, height) * (minYS - minXS)) % height) * width;

		// var treePic = new char[width, height];
		// treePic.Fill('.');
		// foreach (var pos in robots.Select(r => Move(r.Position, r.Velocity, (int)part2, width, height))) {
		// 	treePic[pos.X, pos.Y] = '#';
		// }
		// PrintMap(treePic);

		return (part1, part2);
	}

	private static long ModularInverse(long a, long mod)
	{
		long m0 = mod, t, q;
		long x0 = 0, x1 = 1;

		if (mod == 1) {
			return 0;
		}

		while (a > 1) {
			// q is the quotient
			q = a / mod;

			t = mod;

			// m is remainder now, process same as Euclid's algorithm
			mod = a % mod;
			a = t;

			t = x0;

			x0 = x1 - q * x0;
			x1 = t;
		}

		// Make x1 positive
		if (x1 < 0) {
			x1 += m0;
		}

		return x1;
	}

	private static long PowMod(long baseNum, long exp, long mod)
	{
		ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(mod, 1);

		var result = 1L;

		if (exp < 0) {
			baseNum = ModularInverse(baseNum, mod);
			exp = -exp;
		}

		baseNum %= mod;

		while (exp > 0) {
			if ((exp % 2) == 1) {
				result = (result * baseNum) % mod;
			}

			// Divide exp by 2
			exp /= 2;

			// Square the baseNum and take modulo
			baseNum = (baseNum * baseNum) % mod;
		}

		return result;
	}

	private static double Variance(IEnumerable<int> values)
	{
		var dVals = values.Select(v => Convert.ToDouble(v)).ToArray();
		var total = 0d;

		for (var i = 0; i < dVals.Length; i++) {
			total += dVals[i];
		}

		var avg = total / dVals.Length;

		total = 0d;

		for (var i = 0; i < dVals.Length; i++) {
			total += Math.Pow(dVals[i] - avg, 2.0);
		}

		return total / dVals.Length;
	}

	private static Coordinate Move(Coordinate startPosition, Coordinate velocity, int seconds, int width, int height)
	{
		var movement = (velocity * seconds) + startPosition;
		var x = movement.X % width;
		var y = movement.Y % height;

		if (x < 0) {
			x += width;
		}

		if (y < 0) {
			y += height;
		}

		return (x, y);
	}

	private static List<(Coordinate Position, Coordinate Velocity)> Parse(string[] input)
	{
		Span<Range> ranges = stackalloc Range[8];
		Span<char>  chars  = ['=', ',', ' '];

		var ret = new List<(Coordinate Position, Coordinate Velocity)>(input.Length);

		//p=0,4 v=3,-3
		for (var i = 0; i < input.Length; i++) {
			var inputSpan = input[i].AsSpan();

			inputSpan.SplitAny(ranges, chars, StringSplitOptions.RemoveEmptyEntries);

			ret.Add((
				new Coordinate(int.Parse(inputSpan[ranges[1]]), int.Parse(inputSpan[ranges[2]])),
				new Coordinate(int.Parse(inputSpan[ranges[4]]), int.Parse(inputSpan[ranges[5]]))
			));
		}

		return ret;
	}

	[Theory]
	[InlineData(2, 4, 2, -3, 1, 11, 7, 4, 1)]
	[InlineData(2, 4, 2, -3, 2, 11, 7, 6, 5)]
	[InlineData(2, 4, 2, -3, 3, 11, 7, 8, 2)]
	[InlineData(2, 4, 2, -3, 4, 11, 7, 10, 6)]
	[InlineData(2, 4, 2, -3, 5, 11, 7, 1, 3)]
	public void MoveWorksCorrectly(int startX, int startY, int velX, int velY, int secs, int width, int height, int endX, int endY)
	{
		var end = Move((startX, startY), (velX, velY), secs, width, height);

		Assert.Equal(endX, end.X);
		Assert.Equal(endY, end.Y);
	}
}
