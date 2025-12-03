namespace AdventOfCode.Year2025.Day03;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		var batteries = new int[input[0].Length];
		var bSpan     = batteries.AsSpan();
		var p1        = 0;
		var p2        = 0L;

		Span<long> powers = [
			1L,
			10L,
			100L,
			1000L,
			10000L,
			100000L,
			1000000L,
			10000000L,
			100000000L,
			1000000000L,
			10000000000L,
			100000000000L];

		foreach (var bl in input) {
			// validating inputs is for the weak-minded.
			for (var i = 0; i < batteries.Length; i++) {
				batteries[i] = bl[i] - '0';
			}

			var tens = FindMax(bSpan[..^1]);
			var ones = FindMax(bSpan[(tens.Index + 1)..]).Item;

			p1 += (tens.Item * 10) + ones;

			var toSearch = bSpan;

			for (var i = 11; i >= 0; i--) {
				// find our next value, leaving enough remaining in
				// the span to ensure we have enough for the rest
				var thisMax = FindMax(toSearch[..^i]);

				// calculate the value for this digit
				p2 += thisMax.Item * powers[i];

				// reduce our search space for the next digit
				toSearch = toSearch[(thisMax.Index + 1)..];
			}
		}

		return (p1, p2);
	}

	private static (int Index, int Item) FindMax(Span<int> ints)
	{
		var maxVal = int.MinValue;
		var maxIdx = -1;

		// we want to find the highest item in the span... but since we
		// know that we'll never find anything bigger than 9, we can stop
		// if we find that
		for (var i = 0; i < ints.Length && maxVal < 9; i++) {
			if (ints[i] > maxVal) {
				maxVal = ints[i];
				maxIdx = i;
			}
		}

		return (maxIdx, maxVal);
	}

	public class Day03Tests
	{
		private readonly string[] _sampleInput = """
			987654321111111
			811111111111119
			234234234234278
			818181911112111
			""".Split("\r\n");

		[Test]
		public async Task Part1Works()
		{
			await Assert.That(Execute(_sampleInput).Item1).IsEqualTo(357);
		}

		[Test]
		public async Task Part2Works()
		{
			await Assert.That(Execute(_sampleInput).Item2).IsEqualTo(3121910778619L);
		}
	}
}
