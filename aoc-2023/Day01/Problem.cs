using System.Buffers;
using BenchmarkDotNet.Attributes;

namespace aoc_2023.Day01;

internal readonly record struct NumberPair(ReadOnlyMemory<char> Number, int Value);

public class Problem
{
	public static ProblemMetadata2 Metadata { get; } = new(Execute);

	public static string ProblemFolder => GetFilePath();

	private static NumberPair[] _wordPairs = [
		new("0".AsMemory(),     0),
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

	private static (string, int)[] _words = [
		("0",     0),
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

	public static (long, long) Execute(string fileName)
	{
		var input  = ReadFileLines(fileName);
		//var digits = new[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};

		var part1 = 1;//input.Select(s => (int.Parse(s[s.IndexOfAny(digits)].ToString()) * 10) + s[s.LastIndexOfAny(digits)].ToString())).Sum();
		var part2 = input.Select(s => {
			var lspan = s.AsSpan();
			return (FirstNumber(lspan, _wordPairs) * 10) + LastNumber(lspan, _wordPairs);

			//return (FirstNumber(s, _words) * 10) + LastNumber(s, _words);
		}).Sum();

		// foreach (var line in input) {
		// 	var lnOld = LastNumber(line, _words);
		// 	var lnNew = LastNumber(line.AsSpan(), _wordPairs);

		// 	if (lnOld != lnNew) {
		// 		throw new InvalidOperationException($"Mismatch for string {line}; old={lnOld} new={lnNew}");
		// 	}
		// }

		return (part1, part2);
	}

	// [Benchmark]
	// public void OldWay()
	// {
	// 	foreach (var line in ReadFileLines("input.txt")) {
	// 		FirstNumber(line, _words);
	// 	}
	// }

	// [Benchmark]
	// public void NewWay()
	// {
	// 	foreach (var line in ReadFileLines("input.txt")) {
	// 		FirstNumber(line.AsSpan(), _wordPairs);
	// 	}
	// }

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

	[Fact]
	private static void TestInput()
	{
		var answers = Execute("input.txt");

		Assert.Equal(55108, answers.Item1);
		Assert.Equal(56324, answers.Item2);
	}
}
