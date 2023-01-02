using Combinatorics.Collections;

namespace aoc_2020.Day01;

public class Problem
{
	public static (int, int) Main(string fileName)
	{
		var input = ReadFileLines(fileName, int.Parse);
		var pair  = new Combinations<int>(input, 2).First(c => c.Sum() == 2020);
		var trip  = new Combinations<int>(input, 3).First(c => c.Sum() == 2020);

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
