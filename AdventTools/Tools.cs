﻿using System.Runtime.CompilerServices;

namespace AdventOfCode;

// cycle detection: https://en.wikipedia.org/wiki/Cycle_detection
// Brent's cycle detection algorithm: https://www.geeksforgeeks.org/brents-cycle-detection-algorithm/

// combintations: https://web.archive.org/web/20160120204911/https://msdn.microsoft.com/en-us/library/aa289166(v=vs.71).aspx
// or maybe gray codes: https://stackoverflow.com/a/127856/90328



public static class Tools
{
	public static string GetFilePath([CallerFilePath]string sourceFilePath = "") => Path.GetDirectoryName(sourceFilePath)!;

	public static string GetFilePath(string fileName, [CallerFilePath]string sourceFilePath = "") => Path.Combine(Path.GetDirectoryName(sourceFilePath)!, fileName);

	public static string[] ReadFileLines(string fileName, [CallerFilePath]string sourceFilePath = "") => File.ReadAllLines(GetFilePath(fileName, sourceFilePath));

	public static T[] ReadFileLines<T>(string fileName, Func<string, T> selector, [CallerFilePath]string sourceFilePath = "") => File.ReadAllLines(GetFilePath(fileName, sourceFilePath)).Select(selector).ToArray();

	public static T[] ReadFileLines<T>(string fileName, Func<string, int, T> selector, [CallerFilePath] string sourceFilePath = "") => File.ReadAllLines(GetFilePath(fileName, sourceFilePath)).Select(selector).ToArray();

	public static char[,] ReadFileAsMap(string fileName, [CallerFilePath] string sourceFilePath = "") => CreateMap(File.ReadAllLines(GetFilePath(fileName, sourceFilePath)), c => c);

	public static T[,] ReadFileAsMap<T>(string fileName, Func<char, T> selector, [CallerFilePath] string sourceFilePath = "") => CreateMap(File.ReadAllLines(GetFilePath(fileName, sourceFilePath)), selector);

	public static T[,] CreateMap<T>(string[] lines, Func<char, T> selector) => CreateMap(lines, (x, y, c) => selector(c));

	public static T[,] CreateMap<T>(string[] lines, Func<int, int, char, T> selector)
	{
		var width  = lines[0].Length;
		var height = lines.Length;
		var map    = new T[width, height];

		for (var x = 0; x < width; x++) {
			for (var y = 0; y < height; y++) {
				map[x, y] = selector(x, y, lines[y][x]);
			}
		}

		return map;
	}

	public static void PrintMap(char[,] map)
	{
		var sb = new System.Text.StringBuilder();

		for (var y = 0; y < map.GetLength(1); y++) {
			for (var x = 0; x < map.GetLength(0); x++) {
				sb.Append(map[x, y]);
			}

			Console.WriteLine(sb.ToString());
			sb.Clear();
		}
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

	/// <summary>
	/// Permutations. There are more permutations than combinations.
	/// </summary>
	public static IEnumerable<IEnumerable<T>> Permutations<T>(this IEnumerable<T> list, int length)
	{
		if (length == 1) {
			return list.Select(t => new T[] { t });
		}

		return Permutations(list, length - 1)
			.SelectMany(t => list.Where(e => !t.Contains(e)), (t1, t2) => t1.Concat(new T[] { t2 }));
	}

	/// <summary>
	/// Combinations. There are fewer combinations than permutations.
	/// </summary>
	public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> list, int length)
	{
		var i = 0;

		foreach (var item in list) {
			if (length == 1) {
				yield return new T[] { item };
			} else {
				foreach (var result in Combinations(list.Skip(i + 1), length - 1)) {
					yield return new T[] { item }.Concat(result);
				}
			}

			i++;
		}
	}

	/// <summary>
	/// Generate combinations (fewer than permutations) using Chase's Twiddle
	/// https://web.archive.org/web/20221024045742/http://www.netlib.no/netlib/toms/382
	/// </summary>
	/// <example>
	/// void PrintCombination(char[] span, int[] indices, int k)
	/// {
	/// 	for (var i = 0; i < k; i++) {
	/// 		Console.Write(span[indices[i]]);
	/// 	}
	/// 	Console.WriteLine();
	/// }
	///
	/// var k = 2;
	/// Console.WriteLine($"Combinations of {input} (size {k}):");
	/// GenerateCombinations("abc".ToCharArray, k, PrintCombination);
	/// </example>
	public static void GenerateCombinations<T>(T[] array, int k, Action<T[], int[], int> action)
	{
		var n = array.Length;
		var indices = new int[k];

		for (var i = 0; i < k; i++) {
			indices[i] = i;
		}

		while (true) {
			action(array, indices, k);

			var i = k - 1;

			while (i >= 0 && indices[i] == n - k + i) {
				i--;
			}

			if (i == -1) {
				break;
			}

			indices[i]++;

			for (int j = i + 1; j < k; j++) {
				indices[j] = indices[j - 1] + 1;
			}
		}
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
