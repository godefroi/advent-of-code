namespace AdventOfCode.Year2025.Day06;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		var ranges      = FindColumns(input);
		var p1          = 0L;
		var p2          = 0L;
		var lastLine    = input.Length - 1;
		var inputLength = input[0].Length;

		for (var i = 0; i < ranges.Count; i++) {
			// figure out what op we're gonna do
			Func<long, long, long> aggFunc = input[lastLine][ranges[i]].Trim() switch {
				"+" => (tot, cur) => tot + cur,
				"*" => (tot, cur) => tot * cur,
				_ => throw new InvalidOperationException($"Operation {input[lastLine][ranges[i]].Trim()} is invalid"),
			};

			// seed for part 1 is the number on the first row
			var p1Seed = long.Parse(input[0][ranges[i]]);

			// seed for part 2 is the number in the first column
			var p2Seed = ParseCephalopodNumber(input, ranges[i], 0);

			// find the width of the column for part 2
			var colWidth = ranges[i].GetOffsetAndLength(inputLength).Length;

			// calculate part 1 for this problem
			p1 += Enumerable
				.Range(1, input.Length - 2)
				.Select(row => long.Parse(input[row][ranges[i]]))
				.Aggregate(p1Seed, aggFunc);

			// calculate part 2 for this problem
			p2 += Enumerable
				.Range(1, colWidth - 1) // remaining columns
				.Select(col => ParseCephalopodNumber(input, ranges[i], col))
				.Aggregate(p2Seed, aggFunc);
		}

		return (p1, p2);
	}

	private static long ParseCephalopodNumber(string[] input, Range colRange, int col)
	{
		var pow = 1;
		var acc = 0;

		for (var i = input.Length - 2; i >= 0; i--) {
			var c = input[i][colRange][col];

			if (c == ' ') {
				continue;
			}

			// add this digit
			acc += (c - '0') * pow;

			// increase our multiplier
			pow *= 10;
		}

		return acc;
	}

	private static List<Range> FindColumns(string[] input)
	{
		var lineLength = input[0].Length;
		var ranges     = new List<Range>(1000); // I have a hunch 1000 is the right number
		var lastEnd    = -1;

		for (var i = 0; i < lineLength; i++) {
			if (input.All(s => s[i] == ' ')) {
				ranges.Add(new(lastEnd + 1, i));
				lastEnd = i;
			}
		}

		ranges.Add(new(lastEnd + 1, new Index(0, true)));

		return ranges;
	}

	public class Day06Tests
	{
		private readonly string[] _sampleInput = """
			123 328  51 64 
			 45 64  387 23 
			  6 98  215 314
			*   +   *   +  
			""".Split("\r\n");

		[Test]
		public async Task Part1Works()
		{
			await Assert.That(Execute(_sampleInput).Item1).IsEqualTo(4277556);
		}

		[Test]
		public async Task Part2Works()
		{
			await Assert.That(Execute(_sampleInput).Item2).IsEqualTo(3263827);
		}
	}
}
