namespace aoc_2019.Day01;

public class Problem
{
	public static ProblemMetadata2 Metadata { get; } = new(Main);

	public static (long, long) Main(string[] input)
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

	[Theory]
	[InlineData(2, 12)]
	[InlineData(2, 14)]
	[InlineData(654, 1969)]
	[InlineData(33583, 100756)]
	public void FuelCalculatesCorrectly(long expected, long mass)
	{
		Assert.Equal(expected, CalculateFuel(mass));
	}
}
