namespace AdventOfCode.Year2023.Day06;

public partial class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		var races = Parse(input);
		var part1 = 1L;

		foreach (var race in races) {
			part1 *= CountWins(race);
		}

		var part2Time     = long.Parse(input[0][9..].Replace(" ", ""));
		var part2Distance = long.Parse(input[1][9..].Replace(" ", ""));
		var part2Race     = new Race(part2Time, part2Distance);

		//var middle = part2Time / 2;
		// Console.WriteLine($"1 -> {IsWin(part2Race, 1)}");
		// Console.WriteLine($"{middle} -> {IsWin(part2Race, middle)}");
		// Console.WriteLine($"{part2Time - 1} -> {IsWin(part2Race, part2Time - 1)}");

		var part2 = BinarySearch(part2Race);

		return (part1, part2);
	}

	private static long BinarySearch(Race race)
	{
		var middle = race.Duration / 2;

		// the middle is (we're assuming) a win; we're looking for the point
		// where we transition from not-win to win

		var lower    = 1L;
		var upper    = middle;
		var firstWin = -1L;
		var lastWin  = -1L;

		// for (var i = 12; i < 16; i++) {
		// 	Console.WriteLine($"{i} -> {IsWin(race, i)}");
		// }

		while (true) {
		//for (var i = 0; i < 25; i++) {
			var center = lower + ((upper - lower) / 2);

			//Console.WriteLine($"lower: {lower} ({IsWin(race, lower)}); center: {center} ({IsWin(race, center)}); upper: {upper} ({IsWin(race, upper)})");

			if (upper - lower <= 2) {
				//Console.WriteLine($"DONE! lower: {lower} ({IsWin(race, lower)}); center: {center} ({IsWin(race, center)}); upper: {upper} ({IsWin(race, upper)})");
				if (IsWin(race, lower)) {
					firstWin = lower;
				} else if (IsWin(race, center)) {
					firstWin = center;
				} else if (IsWin(race, upper)) {
					firstWin = upper;
				}

				break;
			}

			if (IsWin(race, center)) {
				// the center between lower and upper is a win...
				// we're interested in the half below that center
				upper = center;
			} else {
				// the center is a loss; we're interested in the top half
				lower = center;
			}
		}

		//Console.WriteLine($"first win: {firstWin}");

		lower = middle;
		upper = race.Duration - 1;

		while (true) {
		//for (var i = 0; i < 25; i++) {
			var center = lower + ((upper - lower) / 2);

			//Console.WriteLine($"lower: {lower} ({IsWin(race, lower)}); center: {center} ({IsWin(race, center)}); upper: {upper} ({IsWin(race, upper)})");

			if (upper - lower <= 2) {
				//Console.WriteLine($"DONE! lower: {lower} ({IsWin(race, lower)}); center: {center} ({IsWin(race, center)}); upper: {upper} ({IsWin(race, upper)})");

				if (IsWin(race, upper)) {
					lastWin = upper;
				} else if (IsWin(race, center)) {
					lastWin = center;
				} else if (IsWin(race, lower)) {
					lastWin = lower;
				}

				break;
			}

			if (IsWin(race, center)) {
				lower = center;
			} else {
				upper = center;
			}
		}

		//Console.WriteLine($"last win: {lastWin}");

		return lastWin - firstWin + 1;
	}

	private static List<Race> Parse(string[] input)
	{
		var times   = input[0][9..].Split(' ', StringSplitOptions.RemoveEmptyEntries);
		var records = input[1][9..].Split(' ', StringSplitOptions.RemoveEmptyEntries);
		var ret     = new List<Race>();

		for (var i = 0; i < times.Length; i++) {
			ret.Add(new Race(int.Parse(times[i]), int.Parse(records[i])));
		}

		return ret;
	}

	private static int CountWins(Race race)
	{
		var wins = 0;

		for (var i = 1; i < race.Duration; i++) {
			if (i * (race.Duration - i) > race.Record) {
				wins++;
			}
		}

		return wins;
	}

	private static bool IsWin(Race race, long pressDuration) => (pressDuration * (race.Duration - pressDuration)) > race.Record;

	private readonly record struct Race(long Duration, long Record);

	public class Tests
	{
		//public
	}
}
