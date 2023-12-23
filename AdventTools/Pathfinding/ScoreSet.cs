using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode;

	internal class ScoreSet<N> : IDictionary<N, float> where N : notnull
	{
		private readonly Dictionary<N, float> _coordinates = [];

		public float this[N key] {
			get => _coordinates.TryGetValue(key, out var value) ? value : float.PositiveInfinity;
			set => ((IDictionary<N, float>)_coordinates)[key] = value;
		}

		public ICollection<N> Keys => _coordinates.Keys;

		public ICollection<float> Values => _coordinates.Values;

		public int Count => _coordinates.Count;

		public bool IsReadOnly => ((ICollection<KeyValuePair<N, float>>)_coordinates).IsReadOnly;

		public void Add(N key, float value) => _coordinates.Add(key, value);

		public void Add(KeyValuePair<N, float> item) => ((ICollection<KeyValuePair<N, float>>)_coordinates).Add(item);

		public void Clear() => _coordinates.Clear();

		public bool Contains(KeyValuePair<N, float> item) => ((ICollection<KeyValuePair<N, float>>)_coordinates).Contains(item);

		public bool ContainsKey(N key) => _coordinates.ContainsKey(key);

		public void CopyTo(KeyValuePair<N, float>[] array, int arrayIndex) => ((ICollection<KeyValuePair<N, float>>)_coordinates).CopyTo(array, arrayIndex);

		public IEnumerator<KeyValuePair<N, float>> GetEnumerator() => ((IEnumerable<KeyValuePair<N, float>>)_coordinates).GetEnumerator();

		public bool Remove(N key) => _coordinates.Remove(key);

		public bool Remove(KeyValuePair<N, float> item) => ((ICollection<KeyValuePair<N, float>>)_coordinates).Remove(item);

		public bool TryGetValue(N key, [MaybeNullWhen(false)] out float value) => _coordinates.TryGetValue(key, out value);

		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_coordinates).GetEnumerator();
	}
