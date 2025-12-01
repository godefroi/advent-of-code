namespace AdventOfCode.Year2019.Day01;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		var moduleMasses = input.Select(long.Parse).ToArray();
		var part1        = moduleMasses.Select(CalculateFuel).Sum();
		var part2        = moduleMasses.Select(CalculateFuel2).Sum();

		return (part1, part2);
	}

	private static long CalculateFuel(long mass) => Math.Max((long)Math.Floor(mass / 3d) - 2L, 0L);

	private static long CalculateFuel2(long mass)
	{
		var total = CalculateFuel(mass);
		var last  = total;

		while (last > 0) {
			var more = CalculateFuel(last);

			//Console.WriteLine($"{more} more fuel for the last fuel, which was {last}");
			total += more;

			last = more;
		}

		return total;
	}

	[Test]
	[Arguments(2, 12)]
	[Arguments(2, 14)]
	[Arguments(654, 1969)]
	[Arguments(33583, 100756)]
	public async Task FuelCalculatesCorrectly(long expected, long mass)
	{
		await Assert.That(CalculateFuel(mass)).IsEqualTo(expected);
	}
}
