using System.Diagnostics;
using Microsoft.DotNet.PlatformAbstractions;

namespace AdventOfCode.Year2024.Day05;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		var rules = new HashSet<Rule>(1200, new RuleEqalityComparer());
		var i     = 0;

		// read and parse all the rules
		for (; !string.IsNullOrWhiteSpace(input[i]); i++) {
			rules.Add(Rule.Parse(input[i]));
		}

		var (part1, part2) = input
			.Skip(i + 1)                      // skip the rules
			.Select(u => IsCorrect(rules, u)) // check for correctness
			.Select(u => (Part1: u.Correct ? u.MiddlePage : 0, Part2: u.Correct ? 0 : u.MiddlePage))
			.Aggregate((Part1: 0, Part2: 0), (agg, cur) => (agg.Part1 + cur.Part1, agg.Part2 + cur.Part2));

		return (part1, part2);
	}

	private static (bool Correct, int MiddlePage) IsCorrect(HashSet<Rule> rules, ReadOnlySpan<char> update)
	{
		Span<Range> ranges = stackalloc Range[32]; // 32 ranges should be enough for anyone!
		Span<int>   pages  = stackalloc int[32];

		var pageCount = update.Split(ranges, ',');

		for (var i = 0; i < pageCount; i++) {
			pages[i] = int.Parse(update[ranges[i]]);
		}

		for (var i = 0; i < pageCount; i++) {
			// validate that all pages after this page have rules
			for (var j = i + 1; j < pageCount; j++) {
				if (!rules.TryGetValue(new Rule(pages[i], pages[j]), out var rule)) {
					throw new InvalidOperationException($"Could not find a rule for pair {pages[i]}|{pages[j]}");
				}

				if (rule.Before != pages[i] || rule.After != pages[j]) {
					pages = pages[..pageCount];
					var comparer = new RuleComparer(rules);
					pages.Sort(comparer);
					return (false, pages[(pageCount - 1) / 2]);
				}
			}

			// validate that all pages before this page have rules
			for (var j = i - 1; j >= 0; j--) {
				if (!rules.TryGetValue(new Rule(pages[i], pages[j]), out var rule)) {
					throw new InvalidOperationException($"Could not find a rule for pair {pages[j]}|{pages[i]}");
				}

				if (rule.Before != pages[j] || rule.After != pages[i]) {
					pages = pages[..pageCount];
					var comparer = new RuleComparer(rules);
					pages.Sort(comparer);
					return (false, pages[(pageCount - 1) / 2]);
				}
			}
		}

		return (true, pages[(pageCount - 1) / 2]);
	}

	private readonly record struct Rule(int Before, int After)
	{
		public static Rule Parse(ReadOnlySpan<char> input)
		{
			Span<Range> ranges = stackalloc Range[2];

			input.Split(ranges, '|');

			return new Rule(int.Parse(input[ranges[0]]), int.Parse(input[ranges[1]]));
		}
	}

	private class RuleEqalityComparer : IEqualityComparer<Rule>
	{
		public bool Equals(Rule x, Rule y) => (x.Before == y.Before && x.After == y.After) || (x.Before == y.After && x.After == y.Before);

		public int GetHashCode(Rule obj)
		{
			// GetHashCode should return the same thing regardless of order
			var combiner = HashCodeCombiner.Start();

			combiner.Add(Math.Min(obj.Before, obj.After));
			combiner.Add(Math.Max(obj.Before, obj.After));

			return combiner.CombinedHash;
		}
	}

	private class RuleComparer(HashSet<Rule> rules) : IComparer<int>
	{
		private HashSet<Rule> _rules = rules;

		public int Compare(int x, int y)
		{
			if (x == y) {
				return 0;
			}

			if (!_rules.TryGetValue(new Rule(x, y), out var rule)) {
				throw new InvalidOperationException($"Could not find a rule for pair {x}|{y}");
			}

			if (rule.Before == x && rule.After == y) {
				return -1;
			} else if (rule.After == x && rule.Before == y) {
				return 1;
			} else {
				throw new UnreachableException("Invalid rule or page numbers");
			}
		}
	}

	[Fact]
	private void RuleWorksCorrectlyInHashSet()
	{
		var rule1 = new Rule(10, 20);
		var rule2 = new Rule(20, 10);
		var set   = new HashSet<Rule>(new RuleEqalityComparer());

		Assert.True(set.Add(rule1));
		Assert.Contains(rule1, set);
		Assert.Contains(rule2, set);
		Assert.False(set.Add(rule2));
		Assert.Single(set);
	}
}
