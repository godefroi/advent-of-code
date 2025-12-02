using BenchmarkDotNet.Attributes;
using Combinatorics.Collections;

namespace AdventOfCode.Year2024.Day07;

public class Problem
{
	private readonly static Op[] _operations = Enum.GetValues<Op>();

	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), typeof(Benchmarks));

	public static (long, long) Execute(string[] input)
	{
		Span<long> nums = stackalloc long[16];

		var part1 = 0L;
		var part2 = 0L;

		foreach (var line in input) {
			var (testValue, numberCount) = Parse(line, nums);
			var numbers = nums[..numberCount];

			if (CheckEquationPart1(testValue, numbers)) {
				part1 += testValue;
			}

			if (CheckEquationPart2(testValue, numbers)) {
				part2 += testValue;
			}
		}

		return (part1, part2);
	}

	private static (long TestValue, int RemainingCount) Parse(ReadOnlySpan<char> input, Span<long> values)
	{
		Span<Range> ranges = stackalloc Range[16];

		var rangeCount = input.SplitAny(ranges, [':', ' '], StringSplitOptions.RemoveEmptyEntries);
		var testValue  = long.Parse(input[ranges[0]]);

		for (var i = 1; i < rangeCount; i++) {
			values[i - 1] = long.Parse(input[ranges[i]]);
		}

		return (testValue, rangeCount - 1);
	}

	private static bool CheckEquationPart1(long testValue, ReadOnlySpan<long> numbers)
	{
		var testCount = Math.Pow(2, numbers.Length - 1);

		for (var i = 0; i < testCount; i++) {
			var acc = numbers[0];

			for (var op = 0; op < numbers.Length - 1; op++) {
				acc = ((i & (1 << op)) > 0) switch {
					true => acc + numbers[op + 1],
					false => acc * numbers[op + 1],
				};

				// since we can only increase the value, if we're already over it, we can exit early
				if (acc > testValue) {
					break;
				}
			}

			if (acc == testValue) {
				return true;
			}
		}

		return false;
	}

	private static bool CheckEquationPart2(long testValue, ReadOnlySpan<long> numbers)
	{
		var tests = new Variations<Op>(Enum.GetValues<Op>(), numbers.Length - 1, GenerateOption.WithRepetition);
		//var tests = GeneratePermutations(_operations, numbers.Length - 1);
		// TODO: why is my implemenation of this so much slower, when my GeneratePermutations() is so much faster?

		foreach (var test in tests) {
			// go through the numbers and operations, combining where necessary
			var numberEnum = numbers.GetEnumerator();
			using var opEnum = test.GetEnumerator();

			// put the first number into the list
			numberEnum.MoveNext();
			var acc = numberEnum.Current;

			while (opEnum.MoveNext() && numberEnum.MoveNext()) {
				var thisOp  = opEnum.Current;
				var thisNum = numberEnum.Current;

				// apply the operation
				acc = opEnum.Current switch {
					Op.Add => acc + numberEnum.Current,
					Op.Multiply => acc * numberEnum.Current,
					Op.Join => acc * Convert.ToInt64(Math.Pow(10, Math.Floor(Math.Log10(thisNum)) + 1)) + thisNum,
					_ => throw new Exception(),
				};

				// since we can only increase the value, if we're already over it, we can exit early
				if (acc > testValue) {
					break;
				}
			}

			if (acc == testValue) {
				return true;
			}
		}

		return false;
	}

	private static string OpString(Op opr) => opr switch {
		Op.Add => "+",
		Op.Multiply => "*",
		Op.Join => "||",
		_ => throw new Exception()
	};

	private enum Op
	{
		Add,
		Multiply,
		Join,
	}

	[Test]
	public async Task AllPermutationMethodsProduceSimilarResults()
	{
		var permLength = 3;
		var operations = Enum.GetValues<Op>();

		var combinatorics = new Variations<Op>(operations, permLength, GenerateOption.WithRepetition)
			.Select(l => string.Join(' ', l.Select(OpString)))
			.Order()
			.ToList();

		var toolsAction = new List<string>(combinatorics.Count);

		GeneratePermutations<Op>(operations, permLength, ops => toolsAction.Add(string.Join(' ', ops.ToArray().Select(OpString))));
		toolsAction.Sort();

		var toolsEnumerable = GeneratePermutations<Op>(operations, permLength)
			.Select(l => string.Join(' ', l.Select(OpString)))
			.Order()
			.ToList();

		await Assert.That(combinatorics).IsEquivalentTo(toolsAction);
		await Assert.That(toolsAction).IsEquivalentTo(toolsEnumerable);
	}

	public class Benchmarks
	{
		private readonly Op[] _operations = Enum.GetValues<Op>();
		private readonly int _permLength = 3;

		[Benchmark]
		public int GenerateUsingCombinatorics()
		{
			var counter = 0;

			foreach (var perm in new Variations<Op>(_operations, _permLength, GenerateOption.WithRepetition)) {
				counter += perm.Count;
			}

			return counter;
		}

		[Benchmark]
		public int GenerateUsingAction()
		{
			var counter = 0;

			GeneratePermutations<Op>(_operations, _permLength, ops => counter += ops.Length);

			return counter;
		}

		[Benchmark]
		public int GenerateUsingEnumerable()
		{
			var counter = 0;

			foreach (var ops in GeneratePermutations(_operations, _permLength)) {
				counter += ops.Length;
			}

			return counter;
		}
	}
}
