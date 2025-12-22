namespace AdventOfCode.Year2025;

public static class Problems
{
	private static readonly Lazy<List<ProblemMetadata>> _allByReflection = new(() => [.. ProblemMetadata.FindMetadata(typeof(Problems).Assembly)]);

	private static readonly Lazy<Dictionary<int, ProblemMetadata>> _all = new(() => new Dictionary<int, ProblemMetadata>() {
	 	{ 01, Day01.Problem.Metadata },
	 	{ 02, Day02.Problem.Metadata },
	 	{ 03, Day03.Problem.Metadata },
	 	{ 04, Day04.Problem.Metadata },
	 	{ 05, Day05.Problem.Metadata },
	 	{ 06, Day06.Problem.Metadata },
	 	{ 07, Day07.Problem.Metadata },
	 	{ 08, Day08.Problem.Metadata },
	 	{ 09, Day09.Problem.Metadata },
	 	{ 10, Day10.Problem.Metadata },
	 	{ 11, Day11.Problem.Metadata },
	 	{ 12, Day12.Problem.Metadata },
	});

	public static IReadOnlyDictionary<int, ProblemMetadata> All { get; } = _all.Value;
}
