using Combinatorics.Collections;

namespace AdventOfCode.Year2020.Day01;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Main);

	public static (long, long) Main(string[] input)
	{
		var numbers = input.Select(int.Parse);
		var pair    = new Combinations<int>(numbers, 2).First(c => c.Sum() == 2020);
		var trip    = new Combinations<int>(numbers, 3).First(c => c.Sum() == 2020);

		return (Multiply(pair), Multiply(trip));
	}

	public static int Multiply(IEnumerable<int> input)
	{
		var v1 = default(int?);

		foreach (var i in input) {
			if (v1 == null) {
				v1 = i;
			} else {
				v1 *= i;
			}
		}

		return v1 ?? 0;
	}
}
