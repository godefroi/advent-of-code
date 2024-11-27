namespace AdventOfCode.Year2021;

public static class Problems
{
	private static readonly Lazy<List<ProblemMetadata>> _allByReflection = new(() => ProblemMetadata.FindMetadata(typeof(Problems).Assembly).ToList());

	private static readonly Lazy<Dictionary<int, ProblemMetadata>> _all = new(() => new Dictionary<int, ProblemMetadata>() {
	 	{ 01, aoc_2021.Day01.Problem.Metadata },
	 	{ 02, aoc_2021.Day02.Problem.Metadata },
	 	{ 03, aoc_2021.Day03.Problem.Metadata },
	 	{ 04, aoc_2021.Day04.Problem.Metadata },
	 	{ 05, aoc_2021.Day05.Problem.Metadata },
	 	{ 06, aoc_2021.Day06.Problem.Metadata },
	 	{ 07, aoc_2021.Day07.Problem.Metadata },
	 	{ 08, aoc_2021.Day08.Problem.Metadata },
	 	{ 09, aoc_2021.Day09.Problem.Metadata },
	 	{ 10, aoc_2021.Day10.Problem.Metadata },
	 	{ 11, aoc_2021.Day11.Problem.Metadata },
	 	{ 12, aoc_2021.Day12.Problem.Metadata },
	 	{ 13, aoc_2021.Day13.Problem.Metadata },
	 	{ 14, aoc_2021.Day14.Problem.Metadata },
	 	{ 15, aoc_2021.Day15.Problem.Metadata },
	 	{ 16, aoc_2021.Day16.Problem.Metadata },
	 	{ 17, aoc_2021.Day17.Problem.Metadata },
	 	{ 18, aoc_2021.Day18.Problem.Metadata },
	 	{ 19, aoc_2021.Day19.Problem.Metadata },
	 	{ 20, aoc_2021.Day20.Problem.Metadata },
	 	{ 21, aoc_2021.Day21.Problem.Metadata },
	 	{ 22, aoc_2021.Day22.Problem.Metadata },
	 	{ 23, aoc_2021.Day23.Problem.Metadata },
	 	{ 24, aoc_2021.Day24.Problem.Metadata },
	 	{ 25, aoc_2021.Day25.Problem.Metadata },
	});

	public static IReadOnlyDictionary<int, ProblemMetadata> All { get; } = _all.Value;
}
