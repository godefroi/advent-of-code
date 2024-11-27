namespace AdventOfCode.Year2016.Day02;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Main);

	public static (string, string) Main(string[] input)
	{
		var part1Map = new Dictionary<char, Dictionary<char, char>>() {
			{ '1', new Dictionary<char, char>() { {'U', '1'}, {'D', '4'}, {'L', '1'}, {'R', '2'} } },
			{ '2', new Dictionary<char, char>() { {'U', '2'}, {'D', '5'}, {'L', '1'}, {'R', '3'} } },
			{ '3', new Dictionary<char, char>() { {'U', '3'}, {'D', '6'}, {'L', '2'}, {'R', '3'} } },
			{ '4', new Dictionary<char, char>() { {'U', '1'}, {'D', '7'}, {'L', '4'}, {'R', '5'} } },
			{ '5', new Dictionary<char, char>() { {'U', '2'}, {'D', '8'}, {'L', '4'}, {'R', '6'} } },
			{ '6', new Dictionary<char, char>() { {'U', '3'}, {'D', '9'}, {'L', '5'}, {'R', '6'} } },
			{ '7', new Dictionary<char, char>() { {'U', '4'}, {'D', '7'}, {'L', '7'}, {'R', '8'} } },
			{ '8', new Dictionary<char, char>() { {'U', '5'}, {'D', '8'}, {'L', '7'}, {'R', '9'} } },
			{ '9', new Dictionary<char, char>() { {'U', '6'}, {'D', '9'}, {'L', '8'}, {'R', '9'} } },
		};

		var part2Map = new Dictionary<char, Dictionary<char, char>>() {
			{ '1', new Dictionary<char, char>() { {'U', '1'}, {'D', '3'}, {'L', '1'}, {'R', '1'} } },
			{ '2', new Dictionary<char, char>() { {'U', '2'}, {'D', '6'}, {'L', '2'}, {'R', '3'} } },
			{ '3', new Dictionary<char, char>() { {'U', '1'}, {'D', '7'}, {'L', '2'}, {'R', '4'} } },
			{ '4', new Dictionary<char, char>() { {'U', '4'}, {'D', '8'}, {'L', '3'}, {'R', '4'} } },
			{ '5', new Dictionary<char, char>() { {'U', '5'}, {'D', '5'}, {'L', '5'}, {'R', '6'} } },
			{ '6', new Dictionary<char, char>() { {'U', '2'}, {'D', 'A'}, {'L', '5'}, {'R', '7'} } },
			{ '7', new Dictionary<char, char>() { {'U', '3'}, {'D', 'B'}, {'L', '6'}, {'R', '8'} } },
			{ '8', new Dictionary<char, char>() { {'U', '4'}, {'D', 'C'}, {'L', '7'}, {'R', '9'} } },
			{ '9', new Dictionary<char, char>() { {'U', '9'}, {'D', '9'}, {'L', '8'}, {'R', '9'} } },
			{ 'A', new Dictionary<char, char>() { {'U', '6'}, {'D', 'A'}, {'L', 'A'}, {'R', 'B'} } },
			{ 'B', new Dictionary<char, char>() { {'U', '7'}, {'D', 'D'}, {'L', 'A'}, {'R', 'C'} } },
			{ 'C', new Dictionary<char, char>() { {'U', '8'}, {'D', 'C'}, {'L', 'B'}, {'R', 'C'} } },
			{ 'D', new Dictionary<char, char>() { {'U', 'B'}, {'D', 'D'}, {'L', 'D'}, {'R', 'D'} } },
		};

		return (CalculateCode(input, part1Map), CalculateCode(input, part2Map));
	}

	private static string CalculateCode(string[] input, Dictionary<char, Dictionary<char, char>> padMap)
	{
		Span<char> code = stackalloc char[input.Length];

		var pos = '5';

		for (var i = 0; i < input.Length; i++) {
			foreach (var dir in input[i]) {
				pos = padMap[pos][dir];
			}

			code[i] = pos;
		}

		return new string(code);
	}

	[Theory]
	[InlineData(new string[] { "ULL", "RRDDD", "LURDL", "UUUUD" }, "1985")]
	public void Part1CalculatesCorrectly(string[] input, string code)
	{
		var (p1, _) = Main(input);

		Assert.Equal(code, p1);
	}
}
