namespace AdventOfCode.Year2024.Day11;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		var buckets = new Dictionary<long, long>();

		foreach (var span in input[0].AsSpan().EnumerateBySplitting(' ')) {
			buckets.Increment(long.Parse(span));
		}

		for (var i = 0; i < 25; i++) {
			buckets = Blink(buckets);
		}

		var part1 = buckets.Values.Sum();

		for (var i = 0; i < 50; i++) {
			buckets = Blink(buckets);
		}

		var part2 = buckets.Values.Sum();

		return (part1, part2);
	}

	private static Dictionary<long, long> Blink(Dictionary<long, long> curState)
	{
		var newState = new Dictionary<long, long>();

		foreach (var (engraving, count) in curState) {
			if (engraving == 0) {
				// If the stone is engraved with the number 0, it is replaced by a stone engraved with the number 1.
				newState.Increment(1, count);
				continue;
			}

			var digCnt = CountDigits(engraving);

			if (digCnt % 2 == 0) {
				// If the stone is engraved with a number that has an even number of digits, it is replaced by two
				// stones. The left half of the digits are engraved on the new left stone, and the right half of the
				// digits are engraved on the new right stone. (The new numbers don't keep extra leading zeroes: 1000
				// would become stones 10 and 0.)
				var vstr = engraving.ToString();
				var e1   = long.Parse(vstr[..(digCnt / 2)]);
				var e2   = long.Parse(vstr[(digCnt / 2)..]);
				newState.Increment(e1, count);
				newState.Increment(e2, count);
			} else {
				// If none of the other rules apply, the stone is replaced by a new stone; the old stone's number
				// multiplied by 2024 is engraved on the new stone.
				newState.Increment(engraving * 2024, count);
			}
		}

		return newState;
	}

	private static int CountDigits(long number) => number == 0L ? 1 : (number > 0L ? 1 : 2) + (int)Math.Log10(Math.Abs((double)number));

	[Test]
	[Arguments([0, 1])]
	[Arguments([1, 1])]
	[Arguments([9, 1])]
	[Arguments([10, 2])]
	[Arguments([11, 2])]
	[Arguments([99, 2])]
	[Arguments([100, 3])]
	[Arguments([101, 3])]
	public async Task CountDigitsWorksCorrectly(long number, int digits) => await Assert.That(CountDigits(number)).IsEqualTo(digits);
}
