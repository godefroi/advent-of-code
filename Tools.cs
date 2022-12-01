using System.Runtime.CompilerServices;

namespace aoc_2022;

public static class Tools
{
	public static string GetFilePath(string fileName, [CallerFilePath]string sourceFilePath = "") => Path.Combine(Path.GetDirectoryName(sourceFilePath)!, fileName);
}
