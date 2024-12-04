namespace AdventOfCode.Year2024.Day02;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		var part1 = 0;
		var part2 = 0;

		foreach (var line in input) {
			var (p1, p2) = SafeNew(line);

			if (p1) {
				part1++;
			}

			if (p2) {
				part2++;
			}
		}

		return (part1, part2);
	}

	private static (bool Part1, bool Part2) SafeNew(string input)
	{
		Span<int> levels = stackalloc int[8];

		var levelCount = 0;
		var inputSpan  = input.AsSpan();
		var enumerator = inputSpan.Split(' ');

		while (enumerator.MoveNext()) {
			levels[levelCount++] = int.Parse(inputSpan[enumerator.Current]);
		}

		var (safe, failPos) = Safe(levels[..levelCount]);

		if (safe) {
			return (true, true);
		}

		Span<int> adjustedLevels = stackalloc int[levelCount - 1];

		for (var i = 0; i < levelCount; i++) {
			Copy(levels, adjustedLevels, levelCount, i);
			(safe, _) = Safe(adjustedLevels);
			if (safe) {
				return (false, true);
			}
		}

		return (false, false);
	}

	private static (bool Safe, int FailPosition) Safe(ReadOnlySpan<int> levels)
	{
		var increasing = levels[1] > levels[0];

		for (var i = 1; i < levels.Length; i++) {
			var diff = levels[i] - levels [i - 1];

			if (increasing) {
				if (diff < 1 || diff > 3) {
					return (false, i);
				}
			} else {
				if (diff < -3 || diff > -1) {
					return (false, i);
				}
			}
		}

		return (true, -1);
	}

	private static void Copy(Span<int> from, Span<int> to, int count, int skip)
	{
		var d = 0;

		for (var i = 0; i < count; i++) {
			if (i != skip) {
				to[d++] = from[i];
			}
		}
	}

	private ref struct SkippingEnumerator
	{
		public SkippingEnumerator(MemoryExtensions.SpanSplitEnumerator<char> underlyingEnumerator, int skipIndex)
		{
			UnderlyingEnumerator = underlyingEnumerator;
			SkipIndex            = skipIndex;
		}

		public MemoryExtensions.SpanSplitEnumerator<char> UnderlyingEnumerator { get; private set; }

		public int SkipIndex { get; private set; }

		public int CurrentIndex { get; private set; }

		public readonly Range Current => UnderlyingEnumerator.Current;

		public bool MoveNext()
		{
			if (!UnderlyingEnumerator.MoveNext()) {
				return false;
			}

			if (CurrentIndex == SkipIndex) {
				CurrentIndex++;
				return MoveNext();
			} else {
				CurrentIndex++;
				return true;
			}
		}
	}
}
