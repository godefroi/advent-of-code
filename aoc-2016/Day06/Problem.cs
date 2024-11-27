namespace AdventOfCode.Year2016.Day06;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Main);

	public static (string, string) Main(string[] input)
	{
		var counts = new Dictionary<char, int>[input[0].Length];

		for (var i = 0; i < counts.Length; i++) {
			counts[i] = [];
		}

		foreach (var line in input) {
			for (var i = 0; i < line.Length; i++) {
				counts[i].Increment(line[i]);
			}
		}

		// for (var c = 'a'; c <= 'z'; c++) {
		// 	Console.WriteLine($"{c} {string.Join("\t", counts.Select(dict => dict.GetValueOrDefault(c).ToString("000")))}");
		// }

		var part1 = new string(counts.Select(c => c.MaxBy(kvp => kvp.Value).Key).ToArray());
		var part2 = new string(counts.Select(c => c.MinBy(kvp => kvp.Value).Key).ToArray());

		return (part1, part2);
	}
}
