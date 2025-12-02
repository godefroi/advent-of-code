namespace AdventOfCode.Year2023.Day12;

public partial class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		Span<Range> groupRanges = stackalloc Range[20]; // probably 20 is enough?
		var groupValues = new int[20];
		var part1 = 0L;
		var part2 = 0L;

		for (var i = 0; i < input.Length; i++) {
			var inputSpan = input[i].AsSpan();
			var spacePos  = inputSpan.IndexOf(' ');
			var record    = inputSpan[..spacePos];
			var groups    = inputSpan[(spacePos + 1)..];
			var p2Record  = new char[(record.Length * 5) + 4].AsSpan();
			var p2Groups  = new int[(groups.Count(',') + 1) * 5].AsSpan();

			var ggvals = groups.ToString().Split(',').Select(int.Parse).ToArray();

			for (var x = 0; x < 5; x++) {
				// copy the record for part 2
				record.CopyTo(p2Record[((record.Length + 1) * x)..]);

				if (x > 0) {
					p2Record[((record.Length + 1) * x) - 1] = '?';
				}

				// copy the group values
				ggvals.CopyTo(p2Groups[(ggvals.Length * x)..]);
			}

			part1 += Solve(record, ggvals, []);
			part2 += Solve(p2Record, p2Groups, []);
		}

		// (8075, 4232520187524)
		return (part1, part2);
	}

	private static long Solve(ReadOnlySpan<char> springs, ReadOnlySpan<int> groups, Dictionary<SolutionState, long> cache)
	{
		// if we have no groups left to satisfiy, then check whether there are
		// remaining springs; if there are, there are no solutions from here
		// otherwise, there's one solution, which is where we're at
		if (groups.Length == 0) {
			if (springs.IndexOf('#') > -1) {
				return 0;
			} else {
				return 1;
			}
		}

		// if we get here, we still have groups that need satisfied, so
		// we'll need to continue our work down the row of characters

		// so find the next spring (or something that could be a spring)
		var idx = springs.IndexOfAny('?', '#');

		// if there aren't any more springs (or can't be), there are no solutions
		if (idx == -1) {
			return 0;
		}

		// make a record of where this solution sits, because we're going to
		// want to be able to refer to it later
		var solutionState = new SolutionState(springs.Length - idx, groups.Length);

		// if we've already solved this particular solution state, then we
		// can avoid all the work and just reuse that solution
		if (cache.TryGetValue(solutionState, out long solution)) {
			return solution;
		}

		// we hold out hope that there could be a solution from here... so we
		// move up to where that solution could possibly start
		springs = springs[idx..];

		// if we can successfully build a set of springs that satisfies the next
		// item in our group requirements, then we can continue on, computing
		// solutions for the rest of the line
		if (CanBuildGroup(springs, groups[0])) {
			solution += Solve(springs[Math.Min(springs.Length, groups[0] + 1)..], groups[1..], cache);
		}

		// if the next spot in the line can be a spring, then we want to look
		// for solutions starting at the next index, reusing our current groups
		// i.e. acting as if this next spot is a '.'
		if (springs[0] == '?') {
			solution += Solve(springs[1..], groups, cache);
		}

		// cache our result, surely it'll come in useful later on; this is
		// "memoization", an important technique in dynamic programming
		cache.Add(solutionState, solution);

		// and finally, we can return the result we computed
		return solution;
	}

	private static bool CanBuildGroup(ReadOnlySpan<char> chars, int groupSize)
	{
		// what we're after here is knowing that everything starting at groupStart
		// and up to groupSize is either a '?' or a '#'... that is to say, the
		// next '.' is farther out than groupSize
		if (chars.Length < groupSize) {
			return false;
		} else if (chars.Length == groupSize) {
			return chars.IndexOf('.') == -1;
		} else {
			for (var i = 0; i < groupSize; i++) {
				if (chars[i] == '.') {
					return false;
				}
			}

			if (chars[groupSize] == '#') {
				return false;
			}

			return true;
		}
	}

	private readonly record struct SolutionState(int RemainingLength, int RemainingGroups);

	public class Tests
	{
		[Test]
		[Arguments(4, "???.?", new[] {1, 1})]
		[Arguments(12, "??#???#?????.?", new[] {5, 1, 1})]
		[Arguments(1, "???.###????.###????.###????.###????.###", new[] {1,1,3,1,1,3,1,1,3,1,1,3,1,1,3})]
		[Arguments(16384, ".??..??...?##.?.??..??...?##.?.??..??...?##.?.??..??...?##.?.??..??...?##.", new[] {1,1,3,1,1,3,1,1,3,1,1,3,1,1,3})]
		[Arguments(1, "?#?#?#?#?#?#?#???#?#?#?#?#?#?#???#?#?#?#?#?#?#???#?#?#?#?#?#?#???#?#?#?#?#?#?#?", new[] {1,3,1,6,1,3,1,6,1,3,1,6,1,3,1,6,1,3,1,6})]
		public async Task SolveWorks(int solutionCount, string chars, int[] groups) =>
			await Assert.That(Solve(chars.AsSpan(), groups.AsSpan(), [])).IsEqualTo(solutionCount);

		[Test]
		[Arguments("##.###", 5, false)]
		[Arguments(".#.###", 5, false)]
		[Arguments("?#?###", 5, false)]
		[Arguments("?#?##.", 5, true)]
		[Arguments("#?#?#", 1, true)]
		public async Task CanBuildGroupWorks(string chars, int groupSize, bool canBuid) =>
			await Assert.That(CanBuildGroup(chars.AsSpan(), groupSize)).IsEqualTo(canBuid);
	}
}
