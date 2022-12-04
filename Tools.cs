using System.Runtime.CompilerServices;

namespace aoc_2022;

public static class Tools
{
	public static string GetFilePath(string fileName, [CallerFilePath]string sourceFilePath = "") => Path.Combine(Path.GetDirectoryName(sourceFilePath)!, fileName);

	public static string[] ReadFileLines(string fileName, [CallerFilePath]string sourceFilePath = "") => File.ReadAllLines(GetFilePath(fileName, sourceFilePath));

	public static T[] ReadFileLines<T>(string fileName, Func<string, T> selector, [CallerFilePath]string sourceFilePath = "") => File.ReadAllLines(GetFilePath(fileName, sourceFilePath)).Select(selector).ToArray()
}
