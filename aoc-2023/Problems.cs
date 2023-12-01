using System.Collections.ObjectModel;

namespace AdventOfCode.Year2023;

public static class Problems
{
	private static readonly Lazy<List<ProblemMetadata>> _allByReflection = new(() => {
		return new List<ProblemMetadata>();
	});

	public static ProblemMetadata Day01 { get; } = new ProblemMetadata(typeof(aoc_2023.Day01.Problem), 1, aoc_2023.Day01.Problem.ProblemFolder, fileName => Console.WriteLine(aoc_2023.Day01.Problem.Execute(fileName)), typeof(string));

	public static IReadOnlyDictionary<int, ProblemMetadata> All { get; } = new ReadOnlyDictionary<int, ProblemMetadata>(new Dictionary<int, ProblemMetadata>() {
		{ 1, Day01 },
	});
}
