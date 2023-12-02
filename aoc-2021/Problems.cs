namespace AdventOfCode.Year2021;

public static class Problems
{
	private static readonly Lazy<List<ProblemMetadata2>> _allByReflection = new(() => ProblemMetadata2.FindMetadata(typeof(Problems).Assembly).ToList());

	public static IReadOnlyDictionary<int, ProblemMetadata2> All { get; } = _allByReflection.Value.ToDictionary(v => v.Day);
}
