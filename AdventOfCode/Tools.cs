using System.Runtime.CompilerServices;

namespace AdventOfCode;

public static class Tools
{
	public static string GetFilePath(string fileName, [CallerFilePath]string sourceFilePath = "") => Path.Combine(Path.GetDirectoryName(sourceFilePath)!, fileName);

	public static string[] ReadFileLines(string fileName, [CallerFilePath]string sourceFilePath = "") => File.ReadAllLines(GetFilePath(fileName, sourceFilePath));

	public static T[] ReadFileLines<T>(string fileName, Func<string, T> selector, [CallerFilePath]string sourceFilePath = "") => File.ReadAllLines(GetFilePath(fileName, sourceFilePath)).Select(selector).ToArray();

	public static long LeastCommonMultiple(long[] numbers) => numbers.Aggregate(LeastCommonMultiple);

	public static long LeastCommonMultiple(long a, long b) => Math.Abs(a * b) / GreatestCommonDenominator(a, b);

	public static long GreatestCommonDenominator(long a, long b) => b == 0 ? a : GreatestCommonDenominator(b, a % b);

}
