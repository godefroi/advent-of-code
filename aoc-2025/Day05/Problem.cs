namespace AdventOfCode.Year2025.Day05;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		var ranges  = new List<(long From, long To)>(256);
		var i       = 0;
		var p1      = 0;
		var maxSeen = long.MinValue;
		var p2      = 0L;

		for (i = 0; i < input.Length && input[i] != string.Empty; i++) {
			var span  = input[i].AsSpan();
			var split = span.IndexOf('-');
			ranges.Add((long.Parse(span[..split]), long.Parse(span[(split + 1)..])));
		}

		for (++i; i < input.Length; i++) {
			var id = long.Parse(input[i]);
			if (ranges.Any(r => r.Includes(id))) {
				p1++;
			}
		}

		// order the ranges by "from" (ascending) then "to" (descending)
		ranges.Sort((a, b) => { var from = a.From.CompareTo(b.From); return from == 0 ? b.To.CompareTo(a.To) : from; });

		// go through each range, looking at the next item in the list, counting IDs in the range
		for (i = 0; i < ranges.Count; i++) {
			var (from, to) = ranges[i];
			
			// case: we've already counted past the top of this range
			//   ... skip this range and move on
			if (maxSeen > to) {
				continue;
			}

			// otherwise, count everything in this range and move on
			// (except don't count any part of this range we've already counted)
			p2 += to - Math.Max(from, maxSeen) + 1;
			maxSeen = to + 1;
		}

		return (p1, p2);
	}

	public class Day05Tests()
	{
		private readonly string[] _sampleData = """
			3-5
			10-14
			16-20
			12-18
			""".Split("\r\n");

		[Test]
		public async Task Part02Works()
		{
			await Assert.That(Execute(_sampleData).Item2).IsEqualTo(14);
		}

		[Test]
		public async Task Part02WorksOnRedditTestData()
		{
			await Assert.That(Execute([
				"200-300",
				"100-101",
				"1-1",
				"2-2",
				"3-3",
				"1-3",
				"1-3",
				"2-2",
				"50-70",
				"10-10",
				"98-99",
				"99-99",
				"99-99",
				"99-100",
				"1-1",
				"2-1",
				"100-100",
				"100-100",
				"100-101",
				"200-300",
				"201-300",
				"202-300",
				"250-251",
				"98-99",
				"100-100",
				"100-101",
				"1-101"]).Item2).IsEqualTo(202);

			await Assert.That(Execute([
				"1-3",
				"3-5"]).Item2).IsEqualTo(5);

			await Assert.That(Execute([
				"100-120",
				"95-100",
				"120-125"]).Item2).IsEqualTo(31);
		}
	}
}

internal static class RangeExtensions
{
	public static bool Includes(this (long From, long To) range, long value) => value >= range.From && value <= range.To;
}
