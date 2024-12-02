namespace AdventOfCode.Year2020.Day06;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		var part1 = ChunkByEmpty(input).Sum(g => g.SelectMany(l => l.ToCharArray()).Distinct().Count());
		var part2 = ChunkByEmpty(input).Sum(g => g.Aggregate((IEnumerable<char>)g.First().ToCharArray(), (prev, next) => prev.Intersect(next.ToCharArray())).Count());

		return (part1, part2);
	}
}
