using System.Text.RegularExpressions;
using static AdventOfCode.SpanTools;

namespace AdventOfCode.Year2025.Day02;

public partial class ProblemDay02
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(ProblemDay02), null);

	public static (long, long) Execute(string[] input)
	{
		var inputSpan = input.Single().AsSpan();
		var p1        = 0L;
		var p2        = 0L;

		Span<char> span = stackalloc char[16];

		foreach (var (from, to) in new RangeSplitEnumerator(inputSpan)) {
			var fromRange = inputSpan[from].ToArray();
			var toRange   = inputSpan[to].ToArray();

			p1 += GenerateInvalidIds(fromRange, toRange).Sum();

			var start = long.Parse(fromRange);
			var end   = long.Parse(toRange);
			var re    = GetInvalidIdRegex();

			for (var i = start; i <= end; i++) {
				if (!i.TryFormat(span, out var len)) {
					throw new InvalidOperationException("Could not format integer to span");
				}

				if (re.IsMatch(span[..len])) {
					p2 += i;
				}
			}
		}

		// TODO: I gave up on the generative solution for p2; it's slow.

		return (p1, p2);
	}

	[GeneratedRegex(@"^(\d+?)\1{1,}$")]
	private static partial Regex GetInvalidIdRegex();

	private static IEnumerable<long> GenerateInvalidIds(char[] rangeStart, char[] rangeEnd)
	{
		if (rangeStart.Length < rangeEnd.Length - 1) {
			throw new NotSupportedException("The case where the number of digits between start and end ranges differes by more than one is not supported.");
		}

		var (begQuo, begRem) = Math.DivRem(rangeStart.Length, 2);
		var (endQuo, endRem) = Math.DivRem(rangeEnd.Length, 2);
		var digits = new char[endQuo];
		var curId  = new char[endQuo * 2];
		var staNum = long.Parse(rangeStart);
		var endNum = long.Parse(rangeEnd);

		// fill the digits array with zeroes
		// because null is I dunno where; you can't increment null to get 1
		digits.AsSpan().Fill('0');

		// pre-fill the digits with the smallest possible invalid ID
		if (begQuo == endQuo && begRem == 1) {
			// easy case; start and end have the same length, but are odd numbers of digits
			// thus, there can be no invalid IDs in there
			yield break;
		} else if (begRem != 0) {
			// the range start has an odd number of digits; the smallest possible
			// invalid id is going to be '1' plus enough '0' to fill out the digits
			// we already filled in the '0' so just set the '1'
			digits[0] = '1';
		} else if (begQuo == 1) {
			// this is a special case where we're dealing with a single character
			// because rangeStart was a two-digit number
			digits[0] = rangeStart[0];

			// if the second digit is larger than the first digit, we need to increment
			// our digit
			if (rangeStart[1] > rangeStart[0] && !Increment(digits)) {
				throw new InvalidOperationException("Could not increment initial digit");
			}
		} else {
			// the range start has an even number of digits; the smallest possible
			// invalid id is going to be the first half of the rangeStart
			var firstHalf = rangeStart[..begQuo];
			var secHalf   = rangeStart[^begQuo..];

			// copy the first half into the digits
			firstHalf.CopyTo(digits.AsSpan()[^begQuo..]);

			// adjust if needed (because the second half is bigger than the first half)
			if (long.Parse(secHalf) > long.Parse(firstHalf)) {
				if (!Increment(digits)) {
					throw new InvalidOperationException("Could not increment initial digits");
				}
			}
		}

		// now, start generating invalid IDs
		do {
			// copy the digits twice
			digits.CopyTo(curId, 0);
			digits.CopyTo(curId, digits.Length);

			// parse the result
			var thisId = long.Parse(curId);

			// if we're past the end of the range, exit
			if (thisId > endNum) {
				break;
			}

			if (thisId < staNum || thisId > endNum) {
				throw new InvalidOperationException($"Generated invalid ID ({thisId}) which is outside the allowed range ({staNum}-{endNum})");
			}

			// otherwise, send it off
			yield return thisId;
		} while (Increment(digits));
	}

	private static bool IsInvalid(ReadOnlySpan<char> id, bool allFactors)
	{
		var trimmedSpan = id.Trim([char.MinValue, '0']); // trim blanks and leading zeroes
		var (quo, rem)  = Math.DivRem(trimmedSpan.Length, 2);

		// if there are an odd number of characters, it can't be invalid
		if (rem != 0) {
			return false;
		}

		// otherwise, it's invalid if it's a repeated set of two groups of digits
		for (var i = 0; i < quo; i++) {
			if (trimmedSpan[i] != trimmedSpan[i + quo]) {
				return false;
			}
		}

		return true;
	}

	private ref struct RangeSplitEnumerator(ReadOnlySpan<char> inputChars)
	{
		private readonly ReadOnlySpan<char> _chars = inputChars;
		private int _curPos = 0;

		public readonly RangeSplitEnumerator GetEnumerator() => this;

		public bool MoveNext()
		{
			if (_curPos >= _chars.Length) {
				return false;
			}

			var nextSplitIndex = _chars[_curPos..].IndexOf(',');

			if (nextSplitIndex == -1) {
				// we're using everything we have left
				Current = Parse(_chars[_curPos..], _curPos);
				_curPos = _chars.Length;

				return true;
			} else {
				// only use up to the next index
				Current = Parse(_chars[_curPos..(_curPos + nextSplitIndex)], _curPos);
				_curPos += nextSplitIndex + 1;

				return true;
			}
		}

		private static (Range From, Range To) Parse(ReadOnlySpan<char> range, int offset)
		{
			var splitPos = range.IndexOf('-');
			var from     = new Range(offset, splitPos + offset);
			var to       = new Range(splitPos + 1 + offset, range.Length + offset);

			return (from, to);
		}

		public (Range From, Range To) Current { get; private set; } = default;
	}

	public class Day02Tests
	{
		private const string _sampleRanges = "11-22,95-115,998-1012,1188511880-1188511890,222220-222224,1698522-1698528,446443-446449,38593856-38593862,565653-565659,824824821-824824827,2121212118-2121212124";

		[Test]
		[Arguments("11", "22", new[] {11L, 22L})]
		[Arguments("95", "119", new[] {99L, 111L})]
		[Arguments("998", "1012", new[] {999L, 1010L})]
		[Arguments("1188511880", "1188511890", new[] {1188511885L})]
		[Arguments("222220", "222224", new[] {222222L})]
		[Arguments("1698522", "1698528", new long[0])]
		[Arguments("446443", "446449", new[] {446446L})]
		[Arguments("38593856", "38593862", new[] {38593859L})]
		[Arguments("565653", "565659", new[] {565656L})]
		[Arguments("824824821", "824824827", new[] {824824824L})]
		[Arguments("2121212118", "2121212124", new[] {2121212121L})]
		[Arguments("26", "45", new[] {33L, 44L})]
		public async Task RegexWorks(string rangeStart, string rangeEnd, long[] invalidIds)
		{
			var start = long.Parse(rangeStart);
			var end   = long.Parse(rangeEnd);
			var re    = GetInvalidIdRegex();
			var invalids = new List<long>();

			Span<char> span = stackalloc char[16];

			for (var i = start; i <= end; i++) {
				if (!i.TryFormat(span, out var len)) {
					throw new InvalidOperationException("Could not format integer to span");
				}

				if (re.IsMatch(span[..len])) {
					invalids.Add(i);
				}
			}

			await Assert.That(invalids).IsEquivalentTo(invalidIds);
		}

		[Test]
		public async Task ExecuteWorks()
		{
			var (p1, p2) = Execute([_sampleRanges]);

			await Assert.That(p1).IsEqualTo(1227775554);
			await Assert.That(p2).IsEqualTo(4174379265);
		}

		[Test]
		public async Task RangesSplitCorrectly()
		{
			var ranges = new List<(long From, long To)>();

			foreach (var (from, to) in new RangeSplitEnumerator(_sampleRanges)) {
				ranges.Add((long.Parse(_sampleRanges[from]), long.Parse(_sampleRanges[to])));
			}

			await Assert.That(ranges).IsEquivalentTo([
				(11L, 22L),
				(95L, 115L),
				(998L, 1012L),
				(1188511880L, 1188511890L),
				(222220L, 222224L),
				(1698522L, 1698528L),
				(446443L, 446449L),
				(38593856L, 38593862L),
				(565653L, 565659L),
				(824824821L, 824824827L),
				(2121212118L, 2121212124L),
			]);
		}

		[Test]
		[Arguments("95", false)]
		[Arguments("96", false)]
		[Arguments("97", false)]
		[Arguments("98", false)]
		[Arguments("99", true)]
		[Arguments("100", false)]
		[Arguments("101", false)]
		[Arguments("102", false)]
		[Arguments("103", false)]
		[Arguments("\099", true)]
		[Arguments("099", true)]
		public async Task IsInvalidWorks(string digits, bool invalid)
		{
			await Assert.That(IsInvalid(digits, false)).IsEqualTo(invalid);
		}

		[Test]
		[Arguments("11", "22", new[] {11L, 22L})]
		[Arguments("95", "119", new[] {99L})]
		[Arguments("998", "1012", new[] {1010L})]
		[Arguments("1188511880", "1188511890", new[] {1188511885L})]
		[Arguments("222220", "222224", new[] {222222L})]
		[Arguments("1698522", "1698528", new long[0])]
		[Arguments("446443", "446449", new[] {446446L})]
		[Arguments("38593856", "38593862", new[] {38593859L})]
		[Arguments("565653", "565659", new long[0])]
		[Arguments("824824821", "824824827", new long[0])]
		[Arguments("2121212118", "2121212124", new long[0])]
		[Arguments("26", "45", new[] {33L, 44L})]
		public async Task InvalidIdsGenerateCorrectly(string rangeStart, string rangeEnd, long[] invalidIds)
		{
			await Assert.That(GenerateInvalidIds([.. rangeStart], [.. rangeEnd]).ToList()).IsEquivalentTo(invalidIds);
		}
	}
}
