using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AdventOfCode;

public static class Tools
{
	public static string GetFilePath(string fileName, [CallerFilePath]string sourceFilePath = "") => Path.Combine(Path.GetDirectoryName(sourceFilePath)!, fileName);

	public static string[] ReadFileLines(string fileName, [CallerFilePath]string sourceFilePath = "") => File.ReadAllLines(GetFilePath(fileName, sourceFilePath));

	public static T[] ReadFileLines<T>(string fileName, Func<string, T> selector, [CallerFilePath]string sourceFilePath = "") => File.ReadAllLines(GetFilePath(fileName, sourceFilePath)).Select(selector).ToArray();

	public static T[] ReadFileLines<T>(string fileName, Func<string, int, T> selector, [CallerFilePath] string sourceFilePath = "") => File.ReadAllLines(GetFilePath(fileName, sourceFilePath)).Select(selector).ToArray();

	public static char[,] ReadFileAsMap(string fileName, [CallerFilePath] string sourceFilePath = "") => CreateMap(File.ReadAllLines(GetFilePath(fileName, sourceFilePath)), c => c);

	public static T[,] ReadFileAsMap<T>(string fileName, Func<char, T> selector, [CallerFilePath] string sourceFilePath = "") => CreateMap(File.ReadAllLines(GetFilePath(fileName, sourceFilePath)), selector);

	public static T[,] CreateMap<T>(string[] lines, Func<char, T> selector)
	{
		var width  = lines[0].Length;
		var height = lines.Length;
		var map    = new T[width, height];

		for (var x = 0; x < width; x++) {
			for (var y = 0; y < height; y++) {
				map[x, y] = selector(lines[y][x]);
			}
		}

		return map;
	}

	public static long LeastCommonMultiple(long[] numbers) => numbers.Aggregate(LeastCommonMultiple);

	public static long LeastCommonMultiple(long a, long b) => Math.Abs(a * b) / GreatestCommonDenominator(a, b);

	public static long GreatestCommonDenominator(long a, long b) => b == 0 ? a : GreatestCommonDenominator(b, a % b);

	public static IEnumerable<IEnumerable<string>> ChunkByEmpty(IEnumerable<string> lines)
	{
		var doneEnumerating = false;

		using var enumerator = lines.GetEnumerator();

		while (!doneEnumerating) {
			yield return EnumerateWhileNotEmpty(enumerator);
		}

		IEnumerable<string> EnumerateWhileNotEmpty(IEnumerator<string> enumerator)
		{
			while (enumerator.MoveNext()) {
				if (enumerator.Current == string.Empty) {
					yield break;
				} else {
					yield return enumerator.Current;
				}
			}

			doneEnumerating = true;
		}
	}

	public static int FindSequence<T>(List<T> toSearch, List<T> toFind, int startIndex = 0)
	{
		for (var i = startIndex; (toSearch.Count - i) >= toFind.Count; i++) {
			if (toSearch.Skip(i).Take(toFind.Count).SequenceEqual(toFind)) {
				return i;
			}
		}

		return -1;
	}

	public static IEnumerable<int> FindSequence<T>(List<T> toSearch, List<T> toFind, int startIndex, bool allowOverlap)
	{
		for (var i = startIndex; (toSearch.Count - i) >= toFind.Count; i++) {
			var loc = FindSequence(toSearch, toFind, i);

			if (loc > -1) {
				yield return loc;

				if (!allowOverlap) {
					i = loc + toFind.Count - 1;
				}
			} else {
				yield break;
			}
		}
	}

	public static Span<T> AsSpan<T>(this Array array)
	{
		// we would want to .Slice() this for a particular rank when dealing with a multidimensional array
		// https://stackoverflow.com/questions/52750582/span-and-two-dimensional-arrays
		return MemoryMarshal.CreateSpan(ref Unsafe.As<byte, T>(ref MemoryMarshal.GetArrayDataReference(array)), array.Length);
	}

	public static Span<T> AsSpan<T>(this T[,] array)
	{
		return MemoryMarshal.CreateSpan(ref Unsafe.As<byte, T>(ref MemoryMarshal.GetArrayDataReference(array)), array.Length);
	}

	public static IEnumerable<IEnumerable<T>> Permutations<T>(this IEnumerable<T> list, int length)
	{
		if (length == 1) {
			return list.Select(t => new T[] { t });
		}

		return Permutations(list, length - 1).SelectMany(t => list.Where(e => !t.Contains(e)), (t1, t2) => t1.Concat(new T[] { t2 }));
	}

	public static void Deconstruct<T>(this T[] array, out T? item0, out T? item1)
	{
		item0 = default;
		item1 = default;

		if (array != null) {
			if (array.Length > 1) {
				item1 = array[1];
			}

			if (array.Length > 0) {
				item0 = array[0];
			}
		}
	}

	public static void Deconstruct<T>(this IEnumerable<T> items, out T? item0, out T? item1)
	{
		item0 = default;
		item1 = default;

		using var en = items.GetEnumerator();

		if (!en.MoveNext()) {
			return;
		}

		item0 = en.Current;

		if (!en.MoveNext()) {
			return;
		}

		item1 = en.Current;
	}

	public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> source, out TKey Key, out TValue Value) => (Key, Value) = (source.Key, source.Value);
}
