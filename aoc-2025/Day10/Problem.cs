namespace AdventOfCode.Year2025.Day10;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		var p1 = input.Sum(l => Part1(l));
		var p2 = input.Sum(Part2Z3);

		return (p1, p2);
	}

	private static int Part1(string inputLine, int allowedDepth = 20)
	{
		var machine = Machine.Parse(inputLine);
		var target  = FromBinary(machine.Lights);
		var buttons = machine.Buttons.Select(FromBinary).ToArray();
		var states  = new HashSet<long>() { 0 };

		// do a dumb breadth-first search of the button-space
		for (var presses = 1; presses <= allowedDepth; presses++) {
			var newStates = new HashSet<long>();

			foreach (var state in states) {
				foreach (var button in buttons) {
					var newState = state ^ button;

					if (newState == target) {
						return presses;
					}

					newStates.Add(newState);
				}
			}

			states = newStates;
		}

		throw new Exception("Did not find a solution in the allowed search depth");
	}

	private static int Part1PowerSet(string inputLine)
	{
		var machine = Machine.Parse(inputLine);
		var target  = FromBinary(machine.Lights);
		var buttons = machine.Buttons.Select(FromBinary).ToArray();
		var min     = int.MaxValue;

		foreach (var set in buttons.EnumeratePowerSet()) {
			var result = 0L;

			for (var i = 0; i < set.Length; i++) {
				result ^= set[i];
			}

			if (result == target && set.Length < min) {
				min = set.Length;
			}
		}

		return min;
	}

	private static long Part2Z3(string inputLine)
	{
		using var ctx = new Microsoft.Z3.Context();

		var machine = Machine.Parse(inputLine);
		var target  = FromBinary(machine.Lights);
		var buttons = machine.Buttons.Select(FromBinary).ToArray();
		var opt     = ctx.MkOptimize();
		var zero    = ctx.MkInt(0);
		var presses = machine.Buttons.Select((_, i) => ctx.MkIntConst($"b{i}Presses")).ToArray();

		// jN = b1Presses + b2Presses + b3Presses + ...
		for (var i = 0; i < machine.Joltage.Length; i++) {
			var joltageExpr = ctx.MkAdd(machine.Buttons
				.Index()
				.Where(b => b.Item.Contains(i))
				.Select(b => presses[b.Index]));

			opt.Assert(ctx.MkEq(joltageExpr, ctx.MkInt(machine.Joltage[i])));
		}

		// buttons can't be pressed a negative number of times
		foreach (var press in presses) {
			opt.Assert(ctx.MkGe(press, zero));
		}

		var totalPresses = ctx.MkAdd(presses);
		var min = opt.MkMinimize(totalPresses);

		//Console.WriteLine(opt.ToString());

		if (opt.Check() != Microsoft.Z3.Status.SATISFIABLE) {
			return 0;
		}

		return (min.Value as Microsoft.Z3.IntNum ?? throw new InvalidCastException()).Int64;
	}

	private static long FromBinary(bool[] bits)
	{
		var ret = 0L;
		var mul = 1L;

		for (var i = 0; i < bits.Length; i++) {
			// if this bit is on, add this digit
			if (bits[i]) {
				ret += mul;
			}

			// advance to the next digit
			mul *= 2;
		}

		return ret;
	}

	private static long FromBinary(int[] onBits)
	{
		var ret = 0L;
		var mul = 1L;
		var pos = 0;

		foreach (var bit in onBits.Order()) {
			while (pos < bit) {
				pos++;
				mul *= 2;
			}

			ret += mul;
		}

		return ret;
	}

	private record class Machine(bool[] Lights, List<int[]> Buttons, int[] Joltage)
	{
		public static Machine Parse(ReadOnlySpan<char> definition)
		{
			Span<Range> chunks = stackalloc Range[32];
			Span<Range> nums   = stackalloc Range[16];

			var rangeCnt = definition.Split(chunks, ' ', StringSplitOptions.None);

			// lights is the first chunk
			var lightChunk = definition[chunks[0]][1..^1];
			var lights     = new bool[lightChunk.Length];

			for (var i = 0; i < lightChunk.Length; i++) {
				lights[i] = lightChunk[i] == '#';
			}

			// buttons are the middle chunks
			var buttons = new List<int[]>(rangeCnt - 2);

			for (var i = 1; i < rangeCnt - 1; i++) {
				var numSpan = definition[chunks[i]][1..^1];
				var numCnt  = numSpan.Split(nums, ',');
				var numbers = new int[numCnt];

				for (var j = 0; j < numbers.Length; j++) {
					numbers[j] = int.Parse(numSpan[nums[j]]);
				}

				buttons.Add(numbers);
			}

			// joltage is the last chunk
			var joltageSpan = definition[chunks[rangeCnt - 1]][1..^1];
			var joltageCnt  = joltageSpan.Split(nums, ',');
			var joltage     = new int[joltageCnt];

			for (var j = 0; j < joltageCnt; j++) {
				joltage[j] = int.Parse(joltageSpan[nums[j]]);
			}

			return new(lights, buttons, joltage);
		}
	}
}
