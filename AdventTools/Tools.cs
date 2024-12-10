using System.Runtime.CompilerServices;

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
			.SelectMany(t => list.Where(e => !t.Contains(e)), (t1, t2) => t1.Concat([t2]));
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
	/// GenerateCombinations("abc".ToCharArray(), k, PrintCombination);
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

	public static void GenerateCombinations<T>(ReadOnlySpan<T> array, int k, Action<ReadOnlySpan<T>, ReadOnlySpan<int>, int> action)
	{
		Span<int> indices = stackalloc int[k];

		for (var i = 0; i < k; i++) {
			indices[i] = i;
		}

		while (true) {
			action(array, indices, k);

			var i = k - 1;

			while (i >= 0 && indices[i] == array.Length - k + i) {
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

	public static void GenerateCombinations<T>(T array, int arrayLength, int k, Action<T, ReadOnlySpan<int>, int> action)
	{
		Span<int> indices = stackalloc int[k];

		for (var i = 0; i < k; i++) {
			indices[i] = i;
		}

		while (true) {
			action(array, indices, k);

			var i = k - 1;

			while (i >= 0 && indices[i] == arrayLength - k + i) {
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

	/// <summary>
	/// Generate all permutations (with repetitions) of a given <paramref name="length"/> using a given set of elements
	/// </summary>
	/// <typeparam name="T">The type of elements being used to generate the permutations</typeparam>
	/// <param name="elements">The elements from which to generate the permutations</param>
	/// <param name="length">The number of elements to place in each permutations</param>
	/// <param name="action">The action to perform using the generated permutation</param>
	/// <remarks>
	/// This method implements Bratley's permutations algorithm, or at least, ChatGPT's interpretation of it.
	/// </remarks>
	public static void GeneratePermutations<T>(ReadOnlySpan<T> elements, int length, Action<ReadOnlySpan<T>> action)
	{
		var currentPerm = new T[length];
		Span<int> counters = stackalloc int[length];

		// generate and produce the first permutation
		Array.Fill(currentPerm, elements[0]);
		action(currentPerm);

		while (true) {
			// find the rightmost position that can be incremented
			var pos = length - 1;

			while (pos >= 0 && counters[pos] == elements.Length - 1) {
				pos--;
			}

			// if all permutations have been generated, leave
			if (pos < 0) {
				break;
			}

			// increment the counter at position pos, and update the corresponding element
			counters[pos]++;
			currentPerm[pos] = elements[counters[pos]];

			// reset all counters to the right of pos to the first element
			for (var i = pos + 1; i < length; i++) {
				counters[i] = 0;
				currentPerm[i] = elements[0];
			}

			// produce this permutation
			action(currentPerm);
		}
	}

	public static IEnumerable<T[]> GeneratePermutations<T>(T[] elements, int length)
	{
		var currentPerm = new T[length];
		var counters = new int[length];

		// generate and produce the first permutation
		Array.Fill(currentPerm, elements[0]);
		yield return currentPerm;

		while (true) {
			// find the rightmost position that can be incremented
			var pos = length - 1;

			while (pos >= 0 && counters[pos] == elements.Length - 1) {
				pos--;
			}

			// if all permutations have been generated, leave
			if (pos < 0) {
				break;
			}

			// increment the counter at position pos, and update the corresponding element
			counters[pos]++;
			currentPerm[pos] = elements[counters[pos]];

			// reset all counters to the right of pos to the first element
			for (var i = pos + 1; i < length; i++) {
				counters[i] = 0;
				currentPerm[i] = elements[0];
			}

			// produce this permutation
			yield return currentPerm;
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
}
