using System.Text;

namespace aoc_2019.Intcode.Infrastructure;

public class SparseArray<T>
{
	private readonly Dictionary<long, T> _values = new();

	public SparseArray(IEnumerable<T> initialValues)
	{
		var idx = 0;

		foreach (var v in initialValues) {
			_values[idx++] = v;
		}
	}

	public long Length => _values.Keys.Max();

	public T? this[long index]
	{
		get {
			if (_values.TryGetValue(index, out var value)) {
				return value;
			} else return default;
		}
		set {
			if (value == null) {
				_values.Remove(index);
			} else {
				_values[index] = value;
			}
		}
	}

	public override string ToString()
	{
		var sb      = new StringBuilder("[");
		var lastKey = -1L;

		foreach (var key in _values.Keys.Order()) {
			if (lastKey > -1) {
				sb.Append(',');
			}

			sb.Append(this[key]);

			lastKey = key;
		}

		sb.Append(']');

		return sb.ToString();
	}
}
