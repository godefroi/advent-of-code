namespace AdventOfCode.Year2023;

public static class Problems
{
	private static readonly Lazy<List<ProblemMetadata>> _allByReflection = new(() => ProblemMetadata.FindMetadata(typeof(Problems).Assembly).ToList());

	// public static IReadOnlyDictionary<int, ProblemMetadata2> All { get; } = new ReadOnlyDictionary<int, ProblemMetadata2>(new Dictionary<int, ProblemMetadata2>() {
	// 	{ 1, aoc_2023.Day01.Problem.Metadata },
	// });

	public static IReadOnlyDictionary<int, ProblemMetadata> All { get; } = _allByReflection.Value.ToDictionary(v => v.Day);
}
