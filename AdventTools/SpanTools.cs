namespace AdventOfCode;

public static class SpanTools
{
	private static readonly System.Buffers.SearchValues<char> _incrementableChars = System.Buffers.SearchValues.Create("012345678");

	/// <summary>
	/// Increments a number represented by a set of digits (as <see cref="char"/>).
	/// </summary>
	public static bool Increment(Span<char> digits)
	{
		var incrementIndex = digits.LastIndexOfAny(_incrementableChars);

		// if there are no digits <9, we can't increment this number
		if (incrementIndex == -1) {
			return false;
		}

		// otherwise, increment the one we found
		digits[incrementIndex] = (char)(Convert.ToUInt16(digits[incrementIndex]) + 1);

		// and if it's not the last one, set the rest to 0
		if (incrementIndex < digits.Length - 1) {
			digits[(incrementIndex + 1)..].Fill('0');
		}

		return true;
	}

	public class SpanToolsTests
	{
		[Test]
		[Arguments("1", "2")]
		[Arguments("9", "9")]
		[Arguments("10", "11")]
		[Arguments("19", "20")]
		[Arguments("191", "192")]
		[Arguments("199", "200")]
		public async Task IncrementWorks(string digits, string expected)
		{
			Span<char> digitSpan = stackalloc char[digits.Length];

			digits.AsSpan().CopyTo(digitSpan);

			var success = Increment(digitSpan);
			var result  = new string(digitSpan);

			await Assert.That(result).IsEqualTo(expected);
			await Assert.That(success).IsEqualTo(digits != expected);
		}
	}
}
