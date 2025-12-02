namespace AdventOfCode.Year2021.Day11;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long p1, long p2) Execute(string[] input)
	{
		var grid = input.Select(line => line.Select(c => int.Parse(c.ToString())).ToArray()).ToArray();
		var fcnt = 0L;

		long p1 = 0;
		int  p2 = 0;

		//Print(grid);

		for (var step = 1; step <= 1000; step++) {
			var flashed = new List<(int i, int j)>();

			for (var i = 0; i < grid.Length; i++) {
				for (var j = 0; j < grid[i].Length; j++) {
					grid[i][j]++;
				}
			}

			for (var i = 0; i < grid.Length; i++) {
				for (var j = 0; j < grid[i].Length; j++) {
					if (grid[i][j] > 9) {
						Flash(grid, flashed, i, j);
					}
				}
			}

			foreach (var (i, j) in flashed) {
				grid[i][j] = 0;
			}

			fcnt += flashed.Count;

			if (step == 100) {
				p1 = fcnt;
				Console.WriteLine($"part 1: {fcnt}");
			}

			if (flashed.Count == grid.Length * grid[0].Length) {
				Console.WriteLine($"part 2: {step}");
				p2 = step;
				break;
			}

			//Print(grid, flashed, step);
		}

		return (p1, p2);
	}

	private static void Flash(int[][] grid, List<(int i, int j)> flashed, int i, int j)
	{
		if (flashed.Contains((i, j))) {
			return;
		}

		flashed.Add((i, j));

		// above left
		if (i > 0 && j > 0 && ++grid[i - 1][j - 1] > 9) {
			Flash(grid, flashed, i - 1, j - 1);
		}

		// above
		if (i > 0 && ++grid[i - 1][j] > 9) {
			Flash(grid, flashed, i - 1, j);
		}

		// above right
		if (i > 0 && j < grid[i - 1].Length - 1 && ++grid[i - 1][j + 1] > 9) {
			Flash(grid, flashed, i - 1, j + 1);
		}

		// left
		if (j > 0 && ++grid[i][j - 1] > 9) {
			Flash(grid, flashed, i, j - 1);
		}

		// right
		if (j < grid[i].Length - 1 && ++grid[i][j + 1] > 9) {
			Flash(grid, flashed, i, j + 1);
		}

		// below left
		if (i < grid.Length - 1 && j > 0 && ++grid[i + 1][j - 1] > 9) {
			Flash(grid, flashed, i + 1, j - 1);
		}

		// below
		if (i < grid.Length - 1 && ++grid[i + 1][j] > 9) {
			Flash(grid, flashed, i + 1, j);
		}

		// below right
		if (i < grid.Length - 1 && j < grid[i].Length - 1 && ++grid[i + 1][j + 1] > 9) {
			Flash(grid, flashed, i + 1, j + 1);
		}
	}

	private static void Print(int[][] grid, List<(int i, int j)>? flashed = null, int? step = null)
	{
		Console.WriteLine(step.HasValue ? $"After step {step}:" : "Before any steps:");
		for (var i = 0; i < grid.Length; i++) {
			for (var j = 0; j < grid[i].Length; j++) {
				Console.ForegroundColor = flashed != null && flashed.Contains((i, j)) ? ConsoleColor.White : ConsoleColor.Gray;
				Console.Write(grid[i][j]);
			}
			Console.WriteLine();
		}
		Console.WriteLine();
	}

	[Test]
	[DisplayName("Day 11 Sample Input")]
	public async Task SampleInputFunctionCorrectly()
	{
		var (p1, p2) = Execute(ReadFileLines("input_sample.txt"));

		await Assert.That(p1).IsEqualTo(1656);
		await Assert.That(p2).IsEqualTo(195);
	}

	[Test]
	[DisplayName("Day 11 Main Input")]
	public async Task MainInputFunctionCorrectly()
	{
		var (p1, p2) = Execute(ReadFileLines("input.txt"));

		await Assert.That(p1).IsEqualTo(1601);
		await Assert.That(p2).IsEqualTo(368);
	}
}
