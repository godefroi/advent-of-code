using BenchmarkDotNet.Attributes;

namespace aoc_2023.Day01;

internal readonly record struct NumberPair(ReadOnlyMemory<char> Number, int Value);

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem));

	private static NumberPair[] _wordPairs = [
		new("1".AsMemory(),     1),
		new("2".AsMemory(),     2),
		new("3".AsMemory(),     3),
		new("4".AsMemory(),     4),
		new("5".AsMemory(),     5),
		new("6".AsMemory(),     6),
		new("7".AsMemory(),     7),
		new("8".AsMemory(),     8),
		new("9".AsMemory(),     9),
		new("one".AsMemory(),   1),
		new("two".AsMemory(),   2),
		new("three".AsMemory(), 3),
		new("four".AsMemory(),  4),
		new("five".AsMemory(),  5),
		new("six".AsMemory(),   6),
		new("seven".AsMemory(), 7),
		new("eight".AsMemory(), 8),
		new("nine".AsMemory(),  9),
	];

	private static NumberPair[] _digitPairs = [
		new("1".AsMemory(),     1),
		new("2".AsMemory(),     2),
		new("3".AsMemory(),     3),
		new("4".AsMemory(),     4),
		new("5".AsMemory(),     5),
		new("6".AsMemory(),     6),
		new("7".AsMemory(),     7),
		new("8".AsMemory(),     8),
		new("9".AsMemory(),     9),
	];

	private static (string, int)[] _words = [
		("1",     1),
		("2",     2),
		("3",     3),
		("4",     4),
		("5",     5),
		("6",     6),
		("7",     7),
		("8",     8),
		("9",     9),
		("one",   1),
		("two",   2),
		("three", 3),
		("four",  4),
		("five",  5),
		("six",   6),
		("seven", 7),
		("eight", 8),
		("nine",  9),
	];

	public static (long, long) Execute(string[] input)
	{
		var part1 = input.AsParallel().Sum(line => ParseNumber(line, _digitPairs));
		var part2 = input.AsParallel().Sum(line => ParseNumber(line, _wordPairs));

		return (part1, part2);
	}

	[Benchmark]
	public void OldWay()
	{
		var total = 0;

		foreach (var line in ReadFileLines("input.txt")) {
			total += (FirstNumber(line, _words) * 10) + LastNumber(line, _words);
		}
	}

	[Benchmark]
	public void NewWay()
	{
		var total = 0;

		foreach (var line in ReadFileLines("input.txt")) {
			var lspan = line.AsSpan();
			total += (FirstNumber(lspan, _wordPairs) * 10) + LastNumber(lspan, _wordPairs);
		}
	}


	[Benchmark]
	public void NewCombined()
	{
		var total = 0;

		foreach (var line in ReadFileLines("input.txt")) {
			total += ParseNumber(line, _wordPairs);
		}
	}

	private static int ParseNumber(string line, NumberPair[] numberPairs)
	{
		var searchIn = line.AsSpan();
		var firstNum = -1;

		while (firstNum == -1 && searchIn.Length > 0) {
			for (var i = 0; i < numberPairs.Length; i++) {
				if (searchIn.StartsWith(numberPairs[i].Number.Span)) {
					firstNum = numberPairs[i].Value;
					break;
				}
			}

			// shorten the span by one
			searchIn = searchIn[1..];
		}

		searchIn = line.AsSpan();

		while (searchIn.Length > 0) {
			for (var i = 0; i < numberPairs.Length; i++) {
				if (searchIn.EndsWith(numberPairs[i].Number.Span)) {
					return (firstNum * 10) + numberPairs[i].Value;
				}
			}

			// shorten the span by one
			searchIn = searchIn[..^1];
		}

		throw new InvalidOperationException("No numbers were found in the string.");
	}

	private static int FirstNumber(ReadOnlySpan<char> searchIn, NumberPair[] numberPairs)
	{
		while (searchIn.Length > 0) {
			for (var i = 0; i < numberPairs.Length; i++) {
				if (searchIn.StartsWith(numberPairs[i].Number.Span)) {
					return numberPairs[i].Value;
				}
			}

			// shorten the span by one
			searchIn = searchIn[1..];
		}

		throw new InvalidOperationException("No numbers were found in the string.");
	}

	private static int LastNumber(ReadOnlySpan<char> searchIn, NumberPair[] numberPairs)
	{
		while (searchIn.Length > 0) {
			for (var i = 0; i < numberPairs.Length; i++) {
				if (searchIn.EndsWith(numberPairs[i].Number.Span)) {
					return numberPairs[i].Value;
				}
			}

			// shorten the span by one
			searchIn = searchIn[..^1];
		}

		throw new InvalidOperationException("No numbers were found in the string.");
	}

	private static int FirstNumber(string s, (string, int)[] words)
	{
		var positions = words.Select(pair => (s.IndexOf(pair.Item1), pair.Item2));

		// if (!positions.Any(p => p.Item1 > -1)) {
		// 	throw new InvalidOperationException($"Couldn't find any numbers in string {s}");
		// }

		return positions.Where(p => p.Item1 > -1).MinBy(p => p.Item1).Item2;
	}

	private static int LastNumber(string s, (string, int)[] words)
	{
		var positions = words.Select(pair => (s.LastIndexOf(pair.Item1), pair.Item2));

		// if (!positions.Any(p => p.Item1 > -1)) {
		// 	throw new InvalidOperationException($"Couldn't find any numbers in string {s}");
		// }

		return positions.Where(p => p.Item1 > -1).MaxBy(p => p.Item1).Item2;
	}

	// [Fact]
	// private static void TestInput()
	// {
	// 	var answers = Execute("input.txt");

	// 	Assert.Equal(55108, answers.Item1);
	// 	Assert.Equal(56324, answers.Item2);
	// }
}
