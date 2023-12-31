namespace aoc_2022.Day11;

public class Problem
{
	public static ProblemMetadata2 Metadata { get; } = new(Main, typeof(Problem));

	public static (long, long) Main(string[] input)
	{
		var monkies = ChunkByEmpty(input).Select(Monkey.Parse).ToList();

		for (var i = 0; i < 20; i++) {
			foreach (var monkey in monkies) {
				monkey.PlayTurn(monkies, null);
			}
		}

		var part1 = monkies.OrderByDescending(m => m.InspectionCount).Take(2).Select(m => m.InspectionCount).Aggregate((m1, m2) => m1 * m2);

		monkies = ChunkByEmpty(input).Select(Monkey.Parse).ToList();

		// key realization here is that DivisibilityCheck are all prime numbers...
		var factor = monkies.Aggregate(1, (c, m) => c * m.DivisibilityCheck);

		for (var i = 0; i < 10000; i++) {
			foreach (var monkey in monkies) {
				monkey.PlayTurn(monkies, factor);
			}
		}

		var part2 = monkies.OrderByDescending(m => m.InspectionCount).Take(2).Select(m => m.InspectionCount).Aggregate((m1, m2) => m1 * m2);

		return (part1, part2);
	}
}
