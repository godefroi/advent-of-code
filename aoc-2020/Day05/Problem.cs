namespace aoc_2020.Day05;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Main);

	public static (long, long) Main(string[] input)
	{
		var seats = input.Select(FindSeat);
		var part1 = seats.Max();
		var part2 = Enumerable.Range(seats.Min(), seats.Max()).Single(i => seats.Contains(i + 1) && seats.Contains(i - 1) && !seats.Contains(i));

		Console.WriteLine($"part 1 {part1}"); // part 1 is 892
		Console.WriteLine($"part 2 {part2}"); // part 2 is 625

		return (part1, part2);
	}

	private static int FindSeat(string pass)
	{
		var rs = 0;
		var re = 127;
		var cs = 0;
		var ce = 7;

		foreach (var c in pass) {
			if (c == 'F') {
				re -= (re - rs) / 2 + 1;
			} else if (c == 'B') {
				rs += (re - rs) / 2 + 1;
			} else if (c == 'L') {
				ce -= (ce - cs) / 2 + 1;
			} else if (c == 'R') {
				cs += (ce - cs) / 2 + 1;
			}
		}

		return (rs * 8) + cs;
	}
}
