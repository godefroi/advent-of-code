namespace AdventOfCode;

public static class DictionaryExtensions
{
	public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> defaultValueProvider) => dictionary.TryGetValue(key, out var value) ? value : defaultValueProvider();
}
