using System.Numerics;

namespace AdventOfCode;

public static class DictionaryExtensions
{
	public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> defaultValueProvider) => dictionary.TryGetValue(key, out var value) ? value : defaultValueProvider();

	public static void Increment<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TValue : struct, IIncrementOperators<TValue>
	{
		// get the current value if it's there, otherwise, it's the default (0)
		if (!dictionary.TryGetValue(key, out var curVal)) {
			curVal = default;
		}

		// increment it
		curVal++;

		// replace it in the dictionary
		dictionary[key] = curVal;
	}

	public static void Increment<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue incrementBy) where TValue : struct, IAdditionOperators<TValue, TValue, TValue>
	{
		// get the current value if it's there, otherwise, it's the default (0)
		if (!dictionary.TryGetValue(key, out var curVal)) {
			curVal = default;
		}

		// increment it
		curVal += incrementBy;

		// replace it in the dictionary
		dictionary[key] = curVal;
	}
}
