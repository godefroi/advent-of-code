namespace AdventOfCode;

public static class SpanExtensions
{
	public static CharSpanSplitEnumerator EnumerateBySplitting(this ReadOnlySpan<char> characters, char splitOn) =>
		new CharSpanSplitEnumerator(characters, splitOn);

	public ref struct CharSpanSplitEnumerator
	{
		private ReadOnlySpan<char> _chars;
		private char               _splitOn;

		public CharSpanSplitEnumerator(ReadOnlySpan<char> chars, char splitOn)
		{
			_chars   = chars;
			_splitOn = splitOn;
			Current  = default;
		}

		public readonly CharSpanSplitEnumerator GetEnumerator() => this;

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

		public ReadOnlySpan<char> Current { get; private set; }
	}
}
