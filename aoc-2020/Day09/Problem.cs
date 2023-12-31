using Combinatorics.Collections;

namespace aoc_2020.Day09;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Main);

	public static (long, long) Main(string[] input)
	{
		var nums  = input.Select(long.Parse).ToArray();
		var inval = FindInvalid(nums, nums.Length > 100 ? 25 : 5);
		var part2 = FindSummingSet(nums, inval);

		//Console.WriteLine($"part 1: {inval}"); // part 1 is 167829540
		//Console.WriteLine($"part 2: {part2}"); // part 2 is 28045630

		return (inval, part2);
	}

	private static long FindInvalid(long[] numbers, int preamble)
	{
		var offset = 0;

		while (offset < numbers.Length - preamble) {
			if (!(new Combinations<long>(numbers[offset..(offset + preamble)], 2)).Any(l => l.Sum() == numbers[offset + preamble])) {
				return numbers[offset + preamble];
			}

			offset++;
		}

		throw new InvalidOperationException("No invalid number found.");
	}

	private static long FindSummingSet(long[] numbers, long value)
	{
		// we'll brute-force the search by looking at each possible set length
		for (var len = 2; len < numbers.Length; len++) {
			// look through each set of size len
			for (var offset = 0; offset <= numbers.Length - len; offset++) {
				var seg = new ArraySegment<long>(numbers, offset, len);

				if (seg.Sum() == value) {
					return seg.Min() + seg.Max();
				}
			}
		}

		throw new InvalidOperationException("No summing set found.");
	}
}
