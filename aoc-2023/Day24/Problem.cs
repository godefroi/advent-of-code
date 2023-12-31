using Microsoft.Z3;

namespace AdventOfCode.Year2023.Day24;

public partial class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, null);

	private readonly static char[] _splitChars = [',', ' ', '@'];
	private readonly static double _testMin = 200000000000000;
	private readonly static double _testMax = 400000000000000;

	public static (long, long) Execute(string[] input)
	{
		Span<Range> ranges = stackalloc Range[6];

		var hailstones = new Hailstone[input.Length];

		for (var i = 0; i < input.Length; i++) {
			var lineSpan = input[i].AsSpan();

			lineSpan.SplitAny(ranges, _splitChars, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

			var position = Coordinate3.Parse(lineSpan, ranges[0..3]);
			var velocity = Coordinate3.Parse(lineSpan, ranges[3..]);

			hailstones[i] = new Hailstone(position, velocity);
		}

		var intersections = 0L;

		GenerateCombinations(hailstones, 2, (stones, indices, _) => {
			var h1 = stones[indices[0]];
			var h2 = stones[indices[1]];

			// Console.WriteLine($"{h1.Position} @ {h1.Velocity}");
			// Console.WriteLine($"{h2.Position} @ {h2.Velocity}");

			if (h1.Slope == h2.Slope) {
				//Console.WriteLine("no intersection at any time");
			} else {
				var intersectionPoint = Intersection(h1, h2);
				var h1Time            = TimeAtPosition(h1, intersectionPoint);
				var h2Time            = TimeAtPosition(h2, intersectionPoint);

				if (h1Time < 0 && h2Time < 0) {
					//Console.WriteLine("in the past for both");
				} else if (h1Time < 0) {
					//Console.WriteLine("in the past for A");
				} else if (h2Time < 0) {
					//Console.WriteLine("in the past for B");
				} else {
					//Console.WriteLine($"intersection at {intersectionPoint}");
					if (intersectionPoint.X >= _testMin && intersectionPoint.Y >= _testMin && intersectionPoint.X <= _testMax && intersectionPoint.Y <= _testMax) {
						intersections++;
					}
				}
			}
			//Console.WriteLine();
		});

		var part2 = SolvePart2(hailstones[..3]);

		return (intersections, part2);
	}

	private static long SolvePart2(IEnumerable<Hailstone> hailstones)
	{
		using var ctx    = new Context();
		using var solver = ctx.MkSolver();
		using var xr     = ctx.MkIntConst("xr");
		using var yr     = ctx.MkIntConst("yr");
		using var zr     = ctx.MkIntConst("zr");
		using var vxr    = ctx.MkIntConst("vxr");
		using var vyr    = ctx.MkIntConst("vyr");
		using var vzr    = ctx.MkIntConst("vzr");

		foreach (var h in hailstones) {
			//var e1 = $"(xr - {h.Position.X}) * ({h.Velocity.Y} - vyr) = (yr - {h.Position.Y}) * ({h.Velocity.X} - vxr)";
			//var e2 = $"(yr - {h.Position.Y}) * ({h.Velocity.Z} - vzr) = (zr - {h.Position.Z}) * ({h.Velocity.Y} - vyr)";
			// (xr - 288998070705911) * (25 - vyr) = (yr - 281498310692304) * (-63 - vxr)
			// (yr - 281498310692304) * (66 - vzr) = (zr - 225433163951734) * (25 - vyr)
			var pX = ctx.MkInt(h.Position.X);
			var pY = ctx.MkInt(h.Position.Y);
			var pZ = ctx.MkInt(h.Position.Z);
			var vX = ctx.MkInt(h.Velocity.X);
			var vY = ctx.MkInt(h.Velocity.Y);
			var vZ = ctx.MkInt(h.Velocity.Z);

			var e1l = ctx.MkMul(ctx.MkSub(xr, pX), ctx.MkSub(vY, vyr));
			var e1r = ctx.MkMul(ctx.MkSub(yr, pY), ctx.MkSub(vX, vxr));
			solver.Assert(ctx.MkEq(e1l, e1r));

			var e2l = ctx.MkMul(ctx.MkSub(yr, pY), ctx.MkSub(vZ, vzr));
			var e2r = ctx.MkMul(ctx.MkSub(zr, pZ), ctx.MkSub(vY, vyr));
			solver.Assert(ctx.MkEq(e2l, e2r));
		}

		var status = solver.Check();

		if (status != Status.SATISFIABLE) {
			throw new InvalidOperationException("Unable to satisfy equation system");
		}

		var rayPosX = solver.Model.Consts.Single(c => c.Key.Name.ToString() == "xr").Value as IntNum ?? throw new InvalidCastException();
		var rayPosY = solver.Model.Consts.Single(c => c.Key.Name.ToString() == "yr").Value as IntNum ?? throw new InvalidCastException();
		var rayPosZ = solver.Model.Consts.Single(c => c.Key.Name.ToString() == "zr").Value as IntNum ?? throw new InvalidCastException();

		return rayPosX.Int64 + rayPosY.Int64 + rayPosZ.Int64;
	}

	private static (double X, double Y) Intersection(Hailstone h1, Hailstone h2)
	{
		var x = (h2.InterceptY - h1.InterceptY) / (h1.Slope - h2.Slope);
		var y = (h1.Slope * x) + h1.InterceptY;

		return (x, y);
	}

	private static double TimeAtPosition(Hailstone hailstone, (double X, double Y) position) =>
		(position.X - hailstone.Position.X) / hailstone.Velocity.X;

	private readonly record struct Hailstone(Coordinate3 Position, Coordinate3 Velocity)
	{
		public double Slope { get; } = (double)Velocity.Y / Velocity.X;

		public double InterceptY { get; } = Position.Y - ((double)Velocity.Y / Velocity.X) * Position.X;
	}
}
