namespace Day04;

public class Problem
{
	public static void Main(string[] args)
	{
		var input  = File.ReadAllLines(args[0]);
		var balls  = input.First().Split(',').Select(s => int.Parse(s)).ToList();
		var boards = input.Skip(2).Chunk(6).Select(s => new Board(string.Join(" ", s).Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(s => int.Parse(s)))).ToArray();
		var part1  = default(int?);

		Console.WriteLine($"There are {boards.Length} boards");

		foreach (var ball in balls) {
			for (var b = 0; b < boards.Length; b++) {

				// skip boards that have won already
				if (boards[b] == null) {
					continue;
				}

				var score = boards[b].Mark(ball);

				if (score.HasValue) {
					part1 ??= score * ball;

					if (boards.Count(b => b != null) == 1) {
						Console.WriteLine($"part 1: {part1}");
						Console.WriteLine($"part 2: {score * ball}"); // 4809 is too low
						Console.WriteLine($"(board {b} won last)");
					}

					//Console.WriteLine($"Board {b} wins, removing it");
					boards[b] = null;
				}
			}
		}
	}
}
