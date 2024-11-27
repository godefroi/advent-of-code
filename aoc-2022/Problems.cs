namespace AdventOfCode.Year2022;

public static class Problems
{
	private static readonly Lazy<List<ProblemMetadata>> _allByReflection = new(() => ProblemMetadata.FindMetadata(typeof(Problems).Assembly).ToList());

	private static readonly Lazy<Dictionary<int, ProblemMetadata>> _all = new(() => new Dictionary<int, ProblemMetadata>() {
	 	{ 01, aoc_2022.Day01.Problem.Metadata },
	 	{ 02, aoc_2022.Day02.Problem.Metadata },
	 	{ 03, aoc_2022.Day03.Problem.Metadata },
	 	{ 04, aoc_2022.Day04.Problem.Metadata },
	 	{ 05, aoc_2022.Day05.Problem.Metadata },
	 	{ 06, aoc_2022.Day06.Problem.Metadata },
	 	{ 07, aoc_2022.Day07.Problem.Metadata },
	 	{ 08, aoc_2022.Day08.Problem.Metadata },
	 	{ 09, aoc_2022.Day09.Problem.Metadata },
	 	{ 10, aoc_2022.Day10.Problem.Metadata },
	 	{ 11, aoc_2022.Day11.Problem.Metadata },
	 	{ 12, aoc_2022.Day12.Problem.Metadata },
	 	{ 13, aoc_2022.Day13.Problem.Metadata },
	 	{ 14, aoc_2022.Day14.Problem.Metadata },
	 	{ 15, aoc_2022.Day15.Problem.Metadata },
	 	{ 16, aoc_2022.Day16.Problem.Metadata },
	 	{ 17, aoc_2022.Day17.Problem.Metadata },
	 	{ 18, aoc_2022.Day18.Problem.Metadata },
	 	{ 19, aoc_2022.Day19.Problem.Metadata },
	 	{ 20, aoc_2022.Day20.Problem.Metadata },
	 	{ 21, aoc_2022.Day21.Problem.Metadata },
	 	{ 22, aoc_2022.Day22.Problem.Metadata },
	 	{ 23, aoc_2022.Day23.Problem.Metadata },
	 	{ 24, aoc_2022.Day24.Problem.Metadata },
	 	{ 25, aoc_2022.Day25.Problem.Metadata },
	});

	public static IReadOnlyDictionary<int, ProblemMetadata> All { get; } = _all.Value;
}
