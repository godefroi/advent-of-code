namespace AdventOfCode.Year2024.Day19;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		return (0, 0);
	}

	// private static bool Possible(char[] searchString, List<char[]> components)
	// {
	// 	var idx = 0;

	// 	while (idx < searchString.Length) {
	// 		components.Where(c => searchString)
	// 	}
	// }

	// private static bool ComponentFits(char[] searchString, int startIndex, char[] subString)
	// {
	// 	if (startIndex + subString.Length > searchString.Length) {
	// 		return false;
	// 	}

	// 	for (var i = 0; i < subString.Length)
	// }

	private readonly record struct MatchProgress(int SearchStart, int ComponentIndex);
}
