namespace aoc_2020.Day02;

public partial class Problem
{
	public static ProblemMetadata2 Metadata { get; } = new(Main);

	public static (long, long) Main(string[] input)
	{
		//var input = @"1-3 a: abcde
		//1-3 b: cdefg
		//2-9 c: ccccccccc".Split(Environment.NewLine).ToList();

		var v1cnt = 0;
		var v2cnt = 0;

		foreach (var line in input.Where(s => !string.IsNullOrWhiteSpace(s))) {
			var match = ParseRegex().Match(line);

			if (!match.Success) {
				throw new Exception($"Couldn't parse {line}");
			}

			var ccnt = match.Groups["password"].Value.Count(c => c == match.Groups["char"].Value[0]);

			if (ccnt >= int.Parse(match.Groups["min"].Value) && ccnt <= int.Parse(match.Groups["max"].Value)) {
				v1cnt++;
			}

			if (new[] { match.Groups["password"].Value[int.Parse(match.Groups["min"].Value) - 1], match.Groups["password"].Value[int.Parse(match.Groups["max"].Value) - 1] }.Count(c => c == match.Groups["char"].Value[0]) == 1) {
				v2cnt++;
			}
		}

		//Console.WriteLine($"part 1: {v1cnt}"); // 398 is correct
		//Console.WriteLine($"part 2: {v2cnt}"); // 562 is correct
		return (v1cnt, v2cnt);
	}

	[GeneratedRegex(@"(?<min>\d+)\-(?<max>\d+) (?<char>\w): (?<password>\w+)")]
	private static partial Regex ParseRegex();
}
