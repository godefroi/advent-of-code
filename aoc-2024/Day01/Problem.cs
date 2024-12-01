namespace AdventOfCode.Year2024.Day01;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		var (left, right) = OrderInputs(input.Select(ParsePair));
		var part1         = left.Zip(right).Sum(z => Math.Abs(z.First - z.Second));
		var rightCounts   = right.GroupBy(n => n).ToDictionary(g => g.Key, g => g.Count());
		var part2         = left.Aggregate(0L, (total, cur) => total + cur * rightCounts.GetValueOrDefault(cur));

		return (part1, part2);
	}

	private static (List<long> left, List<long> right) OrderInputs(IEnumerable<(long left, long right)> inputs)
	{
		var l = new List<long>(1024);
		var r = new List<long>(1024);

		foreach (var (left, right) in inputs) {
			l.Add(left);
			r.Add(right);
		}

		l.Sort();
		r.Sort();

		return (l, r);
	}

	private static (long left, long right) ParsePair(string input)
	{
		var inputSpan = input.AsSpan();

		// left starts at 0, goes to ' '
		var lEnd = inputSpan.IndexOf(' ');
		var left = long.Parse(inputSpan[..lEnd]);

		// right starts after the spances, goes to the end
		var right = long.Parse(inputSpan[lEnd..].TrimEnd());

		return (left, right);
	}
}
