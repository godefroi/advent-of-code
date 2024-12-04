namespace AdventOfCode;

public static class SpanExtensions
{
	public static CharSpanSplitEnumeratorC EnumerateBySplitting(this ReadOnlySpan<char> characters, char splitOn) => new(characters, splitOn);
	public static CharSpanSplitEnumeratorS EnumerateBySplitting(this ReadOnlySpan<char> characters, ReadOnlySpan<char> splitOn) => new(characters, splitOn);

	public ref struct CharSpanSplitEnumeratorC(ReadOnlySpan<char> chars, char splitOn)
	{
		private ReadOnlySpan<char> _chars = chars;
		private readonly char _splitOn = splitOn;

		public readonly CharSpanSplitEnumeratorC GetEnumerator() => this;

		public bool MoveNext()
		{
			if (_chars.Length == 0) {
				return false;
			}

			var nextSplitIndex = _chars.IndexOf(_splitOn);

			if (nextSplitIndex == -1) {
				Current = _chars;
				_chars  = [];

				return true;
			} else {
				Current = _chars[..nextSplitIndex];
				_chars  = _chars[(nextSplitIndex + 1)..];

				return true;
			}
		}

		public ReadOnlySpan<char> Current { get; private set; } = default;
	}

	public ref struct CharSpanSplitEnumeratorS(ReadOnlySpan<char> chars, ReadOnlySpan<char> splitOn)
	{
		private ReadOnlySpan<char> _chars = chars;
		private readonly ReadOnlySpan<char> _splitOn = splitOn;

		public readonly CharSpanSplitEnumeratorS GetEnumerator() => this;

		public bool MoveNext()
		{
			if (_chars.Length == 0) {
				return false;
			}

			var nextSplitIndex = _chars.IndexOf(_splitOn);

			if (nextSplitIndex == -1) {
				Current = _chars;
				_chars  = [];

				return true;
			} else {
				Current = _chars[..nextSplitIndex];
				_chars  = _chars[(nextSplitIndex + _splitOn.Length)..];

				return true;
			}
		}

		public ReadOnlySpan<char> Current { get; private set; } = default;
	}
}
