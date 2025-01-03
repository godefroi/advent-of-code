﻿namespace AdventOfCode.Year2021.Day07;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long p1, long p2) Execute(string[] input)
	{
		var numbers = input.First().Split(',').Select(int.Parse).ToList();
		var p1      = numbers.Min(i => numbers.Sum(ii => Math.Abs(ii - i)));
		var p2      = Enumerable.Range(numbers.Min(), numbers.Max()).Min(i => numbers.Sum(ii => TriangleNumber(ii - i)));

		Console.WriteLine($"part 1: {p1}"); // part 1 is 329389
		Console.WriteLine($"part 2: {p2}"); // part 2 is 86397080

		return (p1, p2);
	}

	private static int TriangleNumber(int i)
	{
		var ret = 0;

		i = Math.Abs(i);

		for (var idx = i; idx > 0; idx--) {
			ret += idx;
		}

		return ret;
	}

	private static long BinomialCoefficient(long n, long k)
	{
		if (k > n) {
			return 0;
		}

		if (n == k) {
			return 1; // only one way to chose when n == k
		}

		if (k > n - k) {
			k = n - k; // Everything is symmetric around n-k, so it is quicker to iterate over a smaller k than a larger one.
		}

		long c = 1;

		for (long i = 1; i <= k; i++) {
			c *= n--;
			c /= i;
		}

		return c;
	}

	[Fact(DisplayName = "Day 07 Sample Input")]
	public void SampleInputFunctionCorrectly()
	{
		var (p1, p2) = Execute(ReadFileLines("input_sample.txt"));

		Assert.Equal(37, p1);
		Assert.Equal(168, p2);
	}

	[Fact(DisplayName = "Day 07 Main Input")]
	public void MainInputFunctionCorrectly()
	{
		var (p1, p2) = Execute(ReadFileLines("input.txt"));

		Assert.Equal(329389, p1);
		Assert.Equal(86397080, p2);
	}
}
