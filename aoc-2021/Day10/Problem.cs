using Xunit;

namespace Day10;

public class Problem
{
	private static readonly List<char> _starts = new() { '(', '[', '{', '<' };
	private static readonly List<char> _ends   = new() { ')', ']', '}', '>' };

	internal static (long p1, long p2) Main(string fileName)
	{
		var input  = File.ReadAllLines(fileName).ToList();
		var scores = input.Select(l => ScoreLine(l)).ToList();
		var p1     = scores.Where(s => s > 0).Sum();

		Console.WriteLine($"part 1: {p1}"); // part 1 is 296535
		
		scores = scores.Where(s => s < 0).OrderBy(s => s).ToList();

		var p2 = Math.Abs(scores[scores.Count / 2]);

		Console.WriteLine($"part 2: {p2}"); // part 2 is 4245130838

		return (p1, p2);
	}

	private static long ScoreLine(string line)
	{
		var stack = new Stack<char>();

		for (var pos = 0; pos < line.Length; pos++) {
			if (_starts.Contains(line[pos])) {
				stack.Push(line[pos]);
			} else if (_ends.Contains(line[pos])) {
				var c = stack.Pop();
				if (_starts.IndexOf(c) != _ends.IndexOf(line[pos])) {
					//Console.WriteLine($"invalid char at position {pos} for this line: expected {ends[starts.IndexOf(c)]}, found {line[pos]}");
					return line[pos] switch {
						')' => 3,
						']' => 57,
						'}' => 1197,
						'>' => 25137,
						_ => throw new InvalidOperationException($"Cannot score that character.")
					};
				}
			}
		}

		var score = 0L;

		foreach (var c in stack) {
			score *= 5;
			score += c switch { '(' => 1, '[' => 2, '{' => 3, '<' => 4, _ => throw new InvalidOperationException("No start for that.") };
		}

		return -score;
	}

	[Fact(DisplayName = "Day 10 Sample Input")]
	public void SampleInputFunctionCorrectly()
	{
		var (p1, p2) = Main("../../../Day10/input_sample.txt");

		Assert.Equal(26397, p1);
		Assert.Equal(288957, p2);
	}

	[Fact(DisplayName = "Day 10 Main Input")]
	public void MainInputFunctionCorrectly()
	{
		var (p1, p2) = Main("../../../Day10/input.txt");

		Assert.Equal(296535, p1);
		Assert.Equal(4245130838, p2);
	}
}
