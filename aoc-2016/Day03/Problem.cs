namespace AdventOfCode.Year2016.Day03;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		var valid = (IEnumerable<(int s1, int s2, int s3)> triangles) => triangles.Count(t => ((t.s2 + t.s3) > t.s1) && ((t.s1 + t.s3) > t.s2) && ((t.s1 + t.s2) > t.s3));
		var rows = input
			.Select(i => (s1: int.Parse(i[0..5]), s2: int.Parse(i[5..10]), s3: int.Parse(i[10..15])))
			.ToList();
		var part1 = valid(rows);
		var part2 = valid(rows
			.Select(r => r.s1)
			.Concat(rows.Select(r => r.s2))
			.Concat(rows.Select(r => r.s3))
			.Chunk(3)
			.Select(c => (s1: c[0], s2: c[1], s3: c[2])));

		return (part1, part2);
	}
}
