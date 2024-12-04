using System.Text.RegularExpressions;

namespace AdventOfCode.Year2024.Day03;

public partial class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		var part1 = 0L;
		var part2 = 0L;
		var en    = true;

		foreach (var line in input) {
			foreach (Match match in ParseRegex().Matches(line)) {
				if (match.Groups["mul"].Success) {
					var mul = int.Parse(match.Groups["op1"].ValueSpan) * int.Parse(match.Groups["op2"].ValueSpan);

					// part 1 always adds
					part1 += mul;

					// part 2 adds if we're currently enabled
					if (en) {
						part2 += mul;
					}
				} else if (match.Groups["do"].Success) {
					en = true;
				} else if (match.Groups["dont"].Success) {
					en = false;
				}
			}
		}

		return (part1, part2);
	}

	[GeneratedRegex(@"(?<mul>mul\((?<op1>\d{1,3}),(?<op2>\d{1,3})\))|(?<do>do\(\))|(?<dont>don't\(\))")]
	private static partial Regex ParseRegex();
}
