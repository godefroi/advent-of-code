using Microsoft.Z3;

namespace AdventOfCode.Year2024.Day13;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		var machines = Parse(input);
		var part1    = machines.Sum(Part1);
		var part2    = machines.Sum(Part2);

		return (part1, part2);
	}

	private static long Part1(Machine machine)
	{
		using var ctx   = new Context();
		using var opt   = ctx.MkOptimize();
		using var aCnt  = ctx.MkIntConst("aCnt");
		using var bCnt  = ctx.MkIntConst("bCnt");
		using var aCost = ctx.MkInt(3);
		using var bCost = ctx.MkInt(1);
		using var prMax = ctx.MkInt(100);
		using var aX = ctx.MkInt(machine.Ax);
		using var bX = ctx.MkInt(machine.Bx);
		using var pX = ctx.MkInt(machine.Px);
		using var aY = ctx.MkInt(machine.Ay);
		using var bY = ctx.MkInt(machine.By);
		using var pY = ctx.MkInt(machine.Py);

		// (aX * aCnt) + (bX * bCnt) = pX
		// (aY * aCnt) + (bY * bCnt) = pY
		// cost = (aCnt * aCost) + (bCnt * bCost)

		opt.Assert(ctx.MkEq(ctx.MkAdd(ctx.MkMul(aX * aCnt), ctx.MkMul(bX * bCnt)), pX));
		opt.Assert(ctx.MkEq(ctx.MkAdd(ctx.MkMul(aY * aCnt), ctx.MkMul(bY * bCnt)), pY));
		opt.Assert(ctx.MkLe(aCnt, prMax));
		opt.Assert(ctx.MkLe(bCnt, prMax));

		var min = opt.MkMinimize(ctx.MkAdd(ctx.MkMul(aCnt, aCost), ctx.MkMul(bCnt, bCost)));

		if (opt.Check() != Status.SATISFIABLE) {
			return 0;
		}

		return (min.Value as IntNum ?? throw new InvalidCastException()).Int64;
	}

	private static long Part2(Machine machine)
	{
		using var ctx   = new Context();
		using var opt   = ctx.MkOptimize();
		using var aCnt  = ctx.MkIntConst("aCnt");
		using var bCnt  = ctx.MkIntConst("bCnt");
		using var aCost = ctx.MkInt(3);
		using var bCost = ctx.MkInt(1);
		using var aX = ctx.MkInt(machine.Ax);
		using var bX = ctx.MkInt(machine.Bx);
		using var pX = ctx.MkInt(machine.Px + 10000000000000);
		using var aY = ctx.MkInt(machine.Ay);
		using var bY = ctx.MkInt(machine.By);
		using var pY = ctx.MkInt(machine.Py + 10000000000000);

		// (aX * aCnt) + (bX * bCnt) = pX
		// (aY * aCnt) + (bY * bCnt) = pY
		// cost = (aCnt * aCost) + (bCnt * bCost)

		opt.Assert(ctx.MkEq(ctx.MkAdd(ctx.MkMul(aX * aCnt), ctx.MkMul(bX * bCnt)), pX));
		opt.Assert(ctx.MkEq(ctx.MkAdd(ctx.MkMul(aY * aCnt), ctx.MkMul(bY * bCnt)), pY));

		var min = opt.MkMinimize(ctx.MkAdd(ctx.MkMul(aCnt, aCost), ctx.MkMul(bCnt, bCost)));

		if (opt.Check() != Status.SATISFIABLE) {
			return 0;
		}

		return (min.Value as IntNum ?? throw new InvalidCastException()).Int64;
	}

	private static List<Machine> Parse(string[] input)
	{
		Span<Range> ranges   = stackalloc Range[8];
		Span<char>  split_on = ['+', ',', '='];

		var ret = new List<Machine>(input.Length / 3);
		var idx = 0;

		while (idx < input.Length) {
			// (idx + 0) Button A: X+61, Y+87
			// (idx + 1) Button B: X+94, Y+38
			// (idx + 2) Prize: X=9321, Y=9067
			var a_span = input[idx + 0].AsSpan();
			var b_span = input[idx + 1].AsSpan();
			var p_span = input[idx + 2].AsSpan();

			// X is in ranges[1], Y is in ranges[3]
			a_span.SplitAny(ranges, split_on, StringSplitOptions.RemoveEmptyEntries);
			var a_x = int.Parse(a_span[ranges[1]]);
			var a_y = int.Parse(a_span[ranges[3]]);

			b_span.SplitAny(ranges, split_on, StringSplitOptions.RemoveEmptyEntries);
			var b_x = int.Parse(b_span[ranges[1]]);
			var b_y = int.Parse(b_span[ranges[3]]);

			p_span.SplitAny(ranges, split_on, StringSplitOptions.RemoveEmptyEntries);
			var p_x = int.Parse(p_span[ranges[1]]);
			var p_y = int.Parse(p_span[ranges[3]]);

			ret.Add(new Machine(a_x, a_y, b_x, b_y, p_x, p_y));

			idx += 4;
		}

		return ret;
	}

	private readonly record struct Machine(int Ax, int Ay, int Bx, int By, int Px, int Py);
}
