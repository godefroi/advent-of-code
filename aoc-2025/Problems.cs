namespace AdventOfCode.Year2025;

public static class Problems
{
	private static readonly Lazy<List<ProblemMetadata>> _allByReflection = new(() => [.. ProblemMetadata.FindMetadata(typeof(Problems).Assembly)]);

	// public static IReadOnlyDictionary<int, ProblemMetadata2> All { get; } = new ReadOnlyDictionary<int, ProblemMetadata2>(new Dictionary<int, ProblemMetadata2>() {
	// 	{ 1, Day01.Problem.Metadata },
	// });

	public static IReadOnlyDictionary<int, ProblemMetadata> All { get; } = _allByReflection.Value.ToDictionary(v => v.Day);
}
