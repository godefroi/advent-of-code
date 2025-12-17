namespace AdventOfCode;

public static class Combinations
{
	public static CombinationsEnumerator<T> EnumerateCombinations<T>(this ReadOnlySpan<T> items, int k) => new(items, k);

	public ref struct CombinationsEnumerator<T>(ReadOnlySpan<T> items, int k)
	{
		private readonly ReadOnlySpan<T> _items = items;
		private readonly Span<int> _indices = new int[k];
		private readonly T[] _current = new T[k];
		private bool _initialized = false;

		public readonly CombinationsEnumerator<T> GetEnumerator() => this;

		public bool MoveNext()
		{
			var k = _indices.Length;
			var i = k - 1;
			var n = _items.Length;

			// if we're not initialized, do that first
			if (!_initialized) {
				for (i = 0; i < k; i++) {
					_indices[i] = i;
				}

				_initialized = true;

				return true;
			}

			// otherwise, make a new combination
			while (i >= 0 && _indices[i] == n - k + i) {
				i--;
			}

			if (i == -1) {
				return false;
			}

			_indices[i]++;

			for (var j = i + 1; j < k; j++) {
				_indices[j] = _indices[j - 1] + 1;
			}

			return true;
		}

		public readonly ReadOnlySpan<T> Current
		{
			get {
				for (var i = 0; i < _indices.Length; i++) {
					_current[i] = _items[_indices[i]];
				}

				return _current;
			}
		}
	}
}

public class CombinationsEnumeratorTests
{
	[Test]
	public async Task EnumeratorWorks()
	{
		var testData   = Enumerable.Range(1, 9).ToArray();
		var combosGen  = new List<(int, int)>();
		var combosIter = new List<(int, int)>();

		Tools.GenerateCombinations(testData, 2, (items, indices, _) => {
			combosGen.Add((items[indices[0]], items[indices[1]]));
		});

		foreach (var combo in testData.EnumerateCombinations(2)) {
			combosIter.Add((combo[0], combo[1]));
		}

		await Assert.That(combosIter).IsEquivalentTo(combosGen);
	}
}
