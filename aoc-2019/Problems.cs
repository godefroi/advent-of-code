namespace AdventOfCode.Year2019;

public static class Problems
{
	private static readonly Lazy<List<ProblemMetadata>> _allByReflection = new(() => ProblemMetadata.FindMetadata(typeof(Problems).Assembly).ToList());

	public static IReadOnlyDictionary<int, ProblemMetadata> All { get; } = _allByReflection.Value.ToDictionary(v => v.Day);
}
