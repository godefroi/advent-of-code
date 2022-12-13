using System.Runtime.CompilerServices;

namespace AdventOfCode;

public static class Tools
{
	public static string GetFilePath(string fileName, [CallerFilePath]string sourceFilePath = "") => Path.Combine(Path.GetDirectoryName(sourceFilePath)!, fileName);

	public static string[] ReadFileLines(string fileName, [CallerFilePath]string sourceFilePath = "") => File.ReadAllLines(GetFilePath(fileName, sourceFilePath));

	public static T[] ReadFileLines<T>(string fileName, Func<string, T> selector, [CallerFilePath]string sourceFilePath = "") => File.ReadAllLines(GetFilePath(fileName, sourceFilePath)).Select(selector).ToArray();

	public static T[] ReadFileLines<T>(string fileName, Func<string, int, T> selector, [CallerFilePath] string sourceFilePath = "") => File.ReadAllLines(GetFilePath(fileName, sourceFilePath)).Select(selector).ToArray();

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

}
