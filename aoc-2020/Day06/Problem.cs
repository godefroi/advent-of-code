namespace aoc_2020.Day06;

public class Problem
{
	public static (int, int) Main(string fileName)
	{
		var input = ReadFileLines(fileName);
		var part1 = ChunkByEmpty(input).Sum(g => g.SelectMany(l => l.ToCharArray()).Distinct().Count());
		var part2 = ChunkByEmpty(input).Sum(g => g.Aggregate((IEnumerable<char>)g.First().ToCharArray(), (prev, next) => prev.Intersect(next.ToCharArray())).Count());

		return (part1, part2);
	}
}
