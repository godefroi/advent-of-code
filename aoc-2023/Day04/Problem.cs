using BenchmarkDotNet.Attributes;

namespace AdventOfCode.Year2023.Day04;

public partial class Problem
{
	public static ProblemMetadata2 Metadata { get; } = new(Execute, typeof(Benchmarks));

	public static (long, long) Execute(string[] input)
	{
		// NOTE: sample input only has 5 winning numbers...
		var winningNumberCount = input.Length < 10 ? 5 : 10;
		var cardNumberCount    = input.Length < 10 ? 8 : 25;
		var part1              = 0L;
		var part2              = 0L;

		Span<int>  winningNumbers = stackalloc int[winningNumberCount];
		Span<int>  cardNumbers    = stackalloc int[cardNumberCount];
		Span<char> digits         = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9'];
		Span<int>  copies         = stackalloc int[input.Length];

		// we start with one copy of every card
		copies.Fill(1);

		for (var l = 0; l < input.Length; l++) {
			var lineSpan = input[l].AsSpan();
			var winCount = 0;
			// Card   1: 61 73 92 28 96 76 32 62 44 53 | 61 17 26 13 92  5 73 29 53 42 62 46 96 32 21 97 99 28 12  4  7 44 19 71 76

			if (winningNumberCount == 10) {
				Parse10WinningNumbers(winningNumbers, lineSpan);
				Parse25CardNumbers(cardNumbers, lineSpan);
			} else {
				Parse5WinningNumbers(winningNumbers, lineSpan);
				Parse8CardNumbers(cardNumbers, lineSpan);
			}

			// sort our winning numbers for maximum binary-search-ness
			winningNumbers.Sort();

			for (var i = 0; i < cardNumberCount; i++) {
				if (winningNumbers.BinarySearch(cardNumbers[i]) >= 0) {
					winCount++;
				}
			}

			part1 += winCount == 0 ? 0L : (long)Math.Pow(2, winCount - 1);

			while (winCount > 0) {
				copies[l + winCount] += copies[l];
				winCount--;
			}
		}

		for (var i = 0; i < copies.Length; i++) {
			part2 += copies[i];
		}

		return (part1, part2);
	}

	private static void Parse5WinningNumbers(Span<int> winningNumbers, ReadOnlySpan<char> lineSpan)
	{
		winningNumbers[0] = int.Parse(lineSpan.Slice( 8, 2));
		winningNumbers[1] = int.Parse(lineSpan.Slice(11, 2));
		winningNumbers[2] = int.Parse(lineSpan.Slice(14, 2));
		winningNumbers[3] = int.Parse(lineSpan.Slice(17, 2));
		winningNumbers[4] = int.Parse(lineSpan.Slice(20, 2));
	}

	private static void Parse10WinningNumbers(Span<int> winningNumbers, ReadOnlySpan<char> lineSpan)
	{
		winningNumbers[0] = int.Parse(lineSpan.Slice(10, 2));
		winningNumbers[1] = int.Parse(lineSpan.Slice(13, 2));
		winningNumbers[2] = int.Parse(lineSpan.Slice(16, 2));
		winningNumbers[3] = int.Parse(lineSpan.Slice(19, 2));
		winningNumbers[4] = int.Parse(lineSpan.Slice(22, 2));
		winningNumbers[5] = int.Parse(lineSpan.Slice(25, 2));
		winningNumbers[6] = int.Parse(lineSpan.Slice(28, 2));
		winningNumbers[7] = int.Parse(lineSpan.Slice(31, 2));
		winningNumbers[8] = int.Parse(lineSpan.Slice(34, 2));
		winningNumbers[9] = int.Parse(lineSpan.Slice(37, 2));
	}

	private static void Parse8CardNumbers(Span<int> cardNumbers, ReadOnlySpan<char> lineSpan)
	{
		cardNumbers[0] = int.Parse(lineSpan.Slice(25, 2));
		cardNumbers[1] = int.Parse(lineSpan.Slice(28, 2));
		cardNumbers[2] = int.Parse(lineSpan.Slice(31, 2));
		cardNumbers[3] = int.Parse(lineSpan.Slice(34, 2));
		cardNumbers[4] = int.Parse(lineSpan.Slice(37, 2));
		cardNumbers[5] = int.Parse(lineSpan.Slice(40, 2));
		cardNumbers[6] = int.Parse(lineSpan.Slice(43, 2));
		cardNumbers[7] = int.Parse(lineSpan.Slice(46, 2));
	}

	private static void Parse25CardNumbers(Span<int> cardNumbers, ReadOnlySpan<char> lineSpan)
	{
		cardNumbers[ 0] = int.Parse(lineSpan.Slice( 42, 2));
		cardNumbers[ 1] = int.Parse(lineSpan.Slice( 45, 2));
		cardNumbers[ 2] = int.Parse(lineSpan.Slice( 48, 2));
		cardNumbers[ 3] = int.Parse(lineSpan.Slice( 51, 2));
		cardNumbers[ 4] = int.Parse(lineSpan.Slice( 54, 2));
		cardNumbers[ 5] = int.Parse(lineSpan.Slice( 57, 2));
		cardNumbers[ 6] = int.Parse(lineSpan.Slice( 60, 2));
		cardNumbers[ 7] = int.Parse(lineSpan.Slice( 63, 2));
		cardNumbers[ 8] = int.Parse(lineSpan.Slice( 66, 2));
		cardNumbers[ 9] = int.Parse(lineSpan.Slice( 69, 2));
		cardNumbers[10] = int.Parse(lineSpan.Slice( 72, 2));
		cardNumbers[11] = int.Parse(lineSpan.Slice( 75, 2));
		cardNumbers[12] = int.Parse(lineSpan.Slice( 78, 2));
		cardNumbers[13] = int.Parse(lineSpan.Slice( 81, 2));
		cardNumbers[14] = int.Parse(lineSpan.Slice( 84, 2));
		cardNumbers[15] = int.Parse(lineSpan.Slice( 87, 2));
		cardNumbers[16] = int.Parse(lineSpan.Slice( 90, 2));
		cardNumbers[17] = int.Parse(lineSpan.Slice( 93, 2));
		cardNumbers[18] = int.Parse(lineSpan.Slice( 96, 2));
		cardNumbers[19] = int.Parse(lineSpan.Slice( 99, 2));
		cardNumbers[20] = int.Parse(lineSpan.Slice(102, 2));
		cardNumbers[21] = int.Parse(lineSpan.Slice(105, 2));
		cardNumbers[22] = int.Parse(lineSpan.Slice(108, 2));
		cardNumbers[23] = int.Parse(lineSpan.Slice(111, 2));
		cardNumbers[24] = int.Parse(lineSpan.Slice(114, 2));
	}

	public class Benchmarks
	{
		private string[] _lines = [];

		[GlobalSetup]
		public void BenchmarkSetup()
		{
			_lines = ReadFileLines("input.txt");
		}

		[Benchmark]
		public void ParseBySpans()
		{
			Span<int>  winningNumbers = stackalloc int[10];
			Span<char> digits         = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9'];

			for (var l = 0; l < _lines.Length; l++) {
				var lineSpan = _lines[l].AsSpan();

				// skip to the first winning number
				lineSpan = lineSpan[lineSpan.IndexOf(':')..];
				lineSpan = lineSpan[lineSpan.IndexOfAny(digits)..];

				// parse out the winning numbers
				for (var i = 0; i < winningNumbers.Length; i++) {
					var spacePos = lineSpan.IndexOf(' ');

					winningNumbers[i] = int.Parse(lineSpan[..spacePos]);
					lineSpan          = lineSpan[spacePos..];
					lineSpan          = lineSpan[lineSpan.IndexOfAny(digits)..];
				}
			}
		}

		[Benchmark]
		public void ParseDirectly()
		{
			Span<int> winningNumbers = stackalloc int[10];

			for (var l = 0; l < _lines.Length; l++) {
				var lineSpan = _lines[l].AsSpan();

				// parse out the winning numbers
				winningNumbers[0] = int.Parse(lineSpan.Slice(11, 2));
				winningNumbers[1] = int.Parse(lineSpan.Slice(13, 2));
				winningNumbers[2] = int.Parse(lineSpan.Slice(16, 2));
				winningNumbers[3] = int.Parse(lineSpan.Slice(19, 2));
				winningNumbers[4] = int.Parse(lineSpan.Slice(22, 2));
				winningNumbers[5] = int.Parse(lineSpan.Slice(25, 2));
				winningNumbers[6] = int.Parse(lineSpan.Slice(28, 2));
				winningNumbers[7] = int.Parse(lineSpan.Slice(31, 2));
				winningNumbers[8] = int.Parse(lineSpan.Slice(34, 2));
				winningNumbers[9] = int.Parse(lineSpan.Slice(37, 2));
			}
		}

		[Benchmark]
		public void ParseDirectlyInALoop()
		{
			Span<int> winningNumbers = stackalloc int[10];

			for (var l = 0; l < _lines.Length; l++) {
				var lineSpan = _lines[l].AsSpan();

				// parse out the winning numbers
				for (var i = 0; i < winningNumbers.Length; i++) {
					//Console.WriteLine($"{11 + (i * 2)}");
					winningNumbers[i] = int.Parse(lineSpan.Slice(11 + (i * 3), 2));
				}
			}
		}

		[Benchmark]
		public void ParseDirectlyWithIf()
		{
			Span<int> winningNumbers = stackalloc int[10];

			for (var l = 0; l < _lines.Length; l++) {
				var lineSpan = _lines[l].AsSpan();

				// parse out the winning numbers
				if (winningNumbers.Length == 10) {
					winningNumbers[0] = int.Parse(lineSpan.Slice(11, 2));
					winningNumbers[1] = int.Parse(lineSpan.Slice(13, 2));
					winningNumbers[2] = int.Parse(lineSpan.Slice(16, 2));
					winningNumbers[3] = int.Parse(lineSpan.Slice(19, 2));
					winningNumbers[4] = int.Parse(lineSpan.Slice(22, 2));
					winningNumbers[5] = int.Parse(lineSpan.Slice(25, 2));
					winningNumbers[6] = int.Parse(lineSpan.Slice(28, 2));
					winningNumbers[7] = int.Parse(lineSpan.Slice(31, 2));
					winningNumbers[8] = int.Parse(lineSpan.Slice(34, 2));
					winningNumbers[9] = int.Parse(lineSpan.Slice(37, 2));
				}
			}
		}

		[Benchmark]
		public void ParseDirectlyWithCall()
		{
			Span<int> winningNumbers = stackalloc int[10];

			for (var l = 0; l < _lines.Length; l++) {
				var lineSpan = _lines[l].AsSpan();

				// parse out the winning numbers
				if (winningNumbers.Length == 10) {
					Parse10WinningNumbers(winningNumbers, lineSpan);
				}
			}
		}
	}
}
