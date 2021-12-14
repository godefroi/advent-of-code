//var input = @"5483143223
//2745854711
//5264556173
//6141336146
//6357385478
//4167524645
//2176841721
//6882881134
//4846848554
//5283751526
//".Split(Environment.NewLine).SkipLast(1).ToList();

//var input = @"11111
//19991
//19191
//19991
//11111
//".Split(Environment.NewLine).SkipLast(1).ToList();

var input = File.ReadAllLines(args[0]).ToList();
var grid  = input.Select(line => line.Select(c => int.Parse(c.ToString())).ToArray()).ToArray();
var fcnt  = 0L;

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
		Console.WriteLine($"part 1: {fcnt}");
	}

	if (flashed.Count == grid.Length * grid[0].Length) {
		Console.WriteLine($"part 2: {step}");
		break;
	}

	//Print(grid, flashed, step);
}

void Flash(int[][] grid, List<(int i, int j)> flashed, int i, int j)
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

void Print(int[][] grid, List<(int i, int j)> flashed = null, int? step = null)
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
