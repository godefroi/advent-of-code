namespace AdventOfCode.Year2022.Day11;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		var monkies = ChunkByEmpty(input).Select(Monkey.Parse).ToList();

		for (var i = 0; i < 20; i++) {
			Console.WriteLine(CalculateState(monkies));
			Console.WriteLine();

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

	private static string CalculateState(List<Monkey> monkies)
	{
		return string.Join(Environment.NewLine, monkies.Select((m, idx) => {
			// op, degree are the missing ones
			return $"{m.Id} {"?"} {"?",2} {m.DivisibilityCheck,2} {m.TrueDestination} {m.FalseDestination} {m.InspectionCount,3} {string.Join(',', m.Items)}";
		}));
	}
}
