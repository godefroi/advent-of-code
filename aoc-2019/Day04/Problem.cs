namespace AdventOfCode.Year2019.Day04;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Main, typeof(Problem), null);

	public static (long, long) Main(string[] input)
	{
		var (low, high) = input.Select(l => { var vals = l.Split('-'); return (low: int.Parse(vals[0]), high: int.Parse(vals[1])); }).Single();
		var part1       = Part1(low, high);
		var part2       = Part2(low, high);

		return (part1, part2);
	}

	public static int Part1(int low, int high)
	{
		var cnt = 0;

		for (var i = low; i <= high; i++) {
			var digits = i.ToString().ToCharArray();
			var found  = false;

			for (var ci = 1; ci < digits.Length; ci++) {
				if (digits[ci] == digits[ci - 1]) {
					found = true;
				}

				if (digits[ci] < digits[ci - 1]) {
					found = false;
					break;
				}
			}

			if (found) {
				cnt += 1;
			}
		}

		return cnt;
	}

	public static int Part2(int low, int high)
	{
		var cnt = 0;

		for (var i = low; i <= high; i++) {
			var digits = i.ToString().ToCharArray();
			var found  = false;

			for (var ci = 1; ci < digits.Length; ci++) {
				if (digits[ci] == digits[ci - 1] && ((ci >= 2 && digits[ci] != digits[ci - 2]) || ci < 2) && ((ci < digits.Length - 1 && digits[ci] != digits[ci + 1]) || ci >= digits.Length - 1)) {
					found = true;
				}

				if (digits[ci] < digits[ci - 1]) {
					found = false;
					break;
				}
			}

			if (found) {
				cnt += 1;
			}
		}

		return cnt;
	}
}
