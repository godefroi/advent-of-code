using Xunit;

namespace Day04;

public class Problem
{
	internal static (int p1, int p2) Main(string fileName)
	{
		var input  = File.ReadAllLines(fileName);
		var balls  = input.First().Split(',').Select(s => int.Parse(s)).ToList();
		var boards = input.Skip(2).Chunk(6).Select(s => new Board(string.Join(" ", s).Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(s => int.Parse(s)))).ToArray() as Board?[];
		var part1  = default(int?);

		Console.WriteLine($"There are {boards.Length} boards");

		foreach (var ball in balls) {
			for (var b = 0; b < boards.Length; b++) {

				// skip boards that have won already
				if (boards[b] == null) {
					continue;
				}

				var score = boards[b]!.Mark(ball);

				if (score.HasValue) {
					part1 ??= score * ball;

					if (boards.Count(b => b != null) == 1) {
						Console.WriteLine($"part 1: {part1}");
						Console.WriteLine($"part 2: {score * ball}"); // 4809 is too low
						Console.WriteLine($"(board {b} won last)");

						return (part1.Value, score.Value * ball);
					}

					//Console.WriteLine($"Board {b} wins, removing it");
					boards[b] = null;
				}
			}
		}

		throw new InvalidOperationException("Should have terminated.");
	}

	[Fact(DisplayName = "Day 04 Sample Input")]
	public void SampleInputFunctionCorrectly()
	{
		var (p1, p2) = Main("../../../Day04/input_sample.txt");

		Assert.Equal(4512, p1);
		Assert.Equal(1924, p2);
	}

	[Fact(DisplayName = "Day 04 Main Input")]
	public void MainInputFunctionCorrectly()
	{
		var (p1, p2) = Main("../../../Day04/input.txt");

		Assert.Equal(54275, p1);
		Assert.Equal(13158, p2);
	}
}
