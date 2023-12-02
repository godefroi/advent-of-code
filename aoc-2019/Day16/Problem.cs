namespace aoc_2019.Day16;

public class Problem
{
	public static ProblemMetadata2 Metadata { get; } = new(Main);

	public static (string, string) Main(string[] input)
	{
		var line  = input.Single();
		var part1 = Part1(line);
		var part2 = Part2(line);

		return (part1, part2);
	}

	public static string Part1(string input)
	{
		var num = input.ToCharArray().Select(c => Convert.ToInt32(c.ToString())).ToList();

		for (var i = 0; i < 100; i++)
			num = CalculateFFT(num);

		return num.Take(8).Select(n => n.ToString()).Aggregate((s1, s2) => s1 + s2);
	}

	public static void Part2BruteForce(string input)
	{
		var inp = input.ToCharArray().Select(c => Convert.ToInt32(c.ToString())).ToList();
		var num = new List<int>(inp.Count * 10000);

		for (var i = 0; i < 10000; i++) {
			num.AddRange(inp);
		}

		for (var i = 0; i < 100; i++)
			num = CalculateFFT(num);

		var offset = Convert.ToInt32(string.Join("", num.Take(7)));

		for (var i = 0; i < 8; i++)
			Console.Write(num[offset + i]);

		Console.WriteLine();
	}

	public static string Part2(string inputString)
	{
		// ok, thanks to much google, reddit, and various hand-drawn paper diagrams,
		// I finally, years later, understand what's going on here and why.

		// because of how the pattern works, for the second half of the input, the output
		// digit for the phase is just the cumulative sum of all previous digits mod 10
		// .. that is to say, for position N of the output, sum digits N..end of the input
		// and mod 10 (except the last digit of the output, which is always the same as
		// the last digit of the input). This is because the pattern is all "1" at the end.

		// so, we know that the second half of each output phase is very easy to calculate
		// and, by design, because we're looking in the second half for the answer, we
		// only ever *need* to calculate the second half. tricky.

		var offset     = int.Parse(inputString[..7]);
		var inputChunk = inputString.Select(c => int.Parse(c.ToString())).ToList();
		var input      = new List<int>(Enumerable.Range(0, 10000).Select(idx => inputChunk).SelectMany(i => i)).Skip(offset).ToArray();

		Console.WriteLine($"We're working on {input.Length} digits, starting from {offset} in the original (multiplied) input");

		for (var i = 0; i < 100; i++) {
			var output = new int[input.Length];
			var sum    = input[^1];

			// final output is the same as final input
			output[^1] = input[^1];

			// the rest need sum and mod
			for (var d = input.Length - 2; d >= 0; d--) {
				output[d] = (sum += input[d]) % 10;
			}

			// new input is old output
			input = output;
		}

		return string.Join("", input.Take(8).Select(i => i.ToString()));
	}

	private static List<int> CalculateFFT(List<int> input)
	{
		var ret = new List<int>();

		for (var i = 0; i < input.Count; i++) {
			var pg  = new PatternGenerator(new[] { 0, 1, 0, -1 }, i + 1);
			var sum = 0;

			for (var j = 0; j < input.Count; j++) {
				sum += input[j] * pg.Next();
			}

			ret.Add(Convert.ToInt32(sum.ToString().Last().ToString()));
		}

		return ret;
	}

	internal class PatternGenerator
	{
		private int[] m_pattern;
		private int m_pos;
		private int m_mult;
		private int m_cmult;

		public PatternGenerator(int[] pattern, int multiplier)
		{
			m_pattern = pattern;
			m_mult = multiplier;

			Reset();
		}

		public void Reset()
		{
			m_pos = 0;
			m_cmult = 0;
		}

		public int Next()
		{
			if (++m_cmult >= m_mult) {
				m_pos++;
				m_cmult = 0;
			}

			return m_pattern[m_pos % m_pattern.Length];
		}
	}
}
