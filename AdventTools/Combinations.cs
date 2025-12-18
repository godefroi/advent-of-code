namespace AdventOfCode;

public static class Combinations
{
	public static CombinationsEnumerator<T> EnumerateCombinations<T>(this ReadOnlySpan<T> items, int k) => new(items, k);

	public static PowerSetEnumerator<T> EnumeratePowerSet<T>(this ReadOnlySpan<T> items) => new(items);

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

	public ref struct PowerSetEnumerator<T>(ReadOnlySpan<T> items)
	{
		private readonly ReadOnlySpan<T> _items = items;
		private readonly Span<T> _current = new T[items.Length];
		private int _currentCnt = 0;
		private int _mask = 0;

		public readonly PowerSetEnumerator<T> GetEnumerator() => this;

		public bool MoveNext()
		{
			if (_mask >= (1 << _items.Length)) {
				return false;
			}

			_currentCnt = 0;

			for (var i = 0; i < _items.Length; i++) {
				if ((_mask & (1 << i)) != 0) {
					_current[_currentCnt++] = _items[i];
				}
			}

			_mask++;

			return true;
		}

		public readonly ReadOnlySpan<T> Current => _current[.._currentCnt];
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

public class PowerSetEnumeratorTests
{
	[Test]
	public async Task EnumeratorWorks()
	{
		var testData = new[] { "A", "B", "C" };
		var expectedSets = new[] {
			Array.Empty<string>(),
			["A"],
			["B"],
			["C"],
			["A", "B"],
			["A", "C"],
			["B", "C"],
			["A", "B", "C"]
		};
		var actualSets = new List<string[]>();

		foreach (var items in testData.EnumeratePowerSet()) {
			actualSets.Add([.. items.ToArray().Order()]);
		}

		actualSets.Sort(CompareArrays);

		await Assert.That(actualSets).IsEquivalentTo(expectedSets);
	}

	private static int CompareArrays(string[] left, string[] right)
	{
		var minLength = Math.Min(left.Length, right.Length);

		// Compare element by element up to the minimum length
		for (var i = 0; i < minLength; i++) {
			var elementComparison = left[i].CompareTo(right[i]);

			if (elementComparison != 0) {
				return elementComparison;
			}
		}

		// If all compared elements are equal, the shorter array comes first
		return left.Length.CompareTo(right.Length);
	}
}
