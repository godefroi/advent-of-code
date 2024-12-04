namespace AdventOfCode.Year2024.Day04;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		var grid  = CreateMap(input, c => c);
		var part1 = SearchPart1(grid);
		var part2 = SearchPart2(grid);

		return (part1, part2);
	}

	private static int SearchPart1(char[,] grid)
	{
		var xLen = grid.GetLength(0);
		var yLen = grid.GetLength(1);
		var ret  = 0;

		// search horizontal ↔
		for (var y = 0; y < yLen; y++) {
			for (var x = 0; x < xLen - 3; x++) {
				if (grid[x, y] == 'X' && grid[x + 1, y] == 'M' && grid[x + 2, y] == 'A' && grid[x + 3, y] == 'S') {
					ret++;
				} else if (grid[x, y] == 'S' && grid[x + 1, y] == 'A' && grid[x + 2, y] == 'M' && grid[x + 3, y] == 'X') {
					ret++;
				}
			}
		}

		// search vertical ↕
		for (var x = 0; x < xLen; x++) {
			for (var y = 0; y < yLen - 3; y++) {
				if (grid[x, y] == 'X' && grid[x, y + 1] == 'M' && grid[x, y + 2] == 'A' && grid[x, y + 3] == 'S') {
					ret++;
				} else if (grid[x, y] == 'S' && grid[x, y + 1] == 'A' && grid[x, y + 2] == 'M' && grid[x, y + 3] == 'X') {
					ret++;
				}
			}
		}

		// search diagonal ⤡⤢
		for (var x = 0; x < xLen - 3; x++) {
			for (var y = 0; y < yLen - 3; y++) {
				if (grid[x + 0, y + 0] == 'X' && grid[x + 1, y + 1] == 'M' && grid[x + 2, y + 2] == 'A' && grid[x + 3, y + 3] == 'S') {
					ret++;
				} else if (grid[x + 0, y + 0] == 'S' && grid[x + 1, y + 1] == 'A' && grid[x + 2, y + 2] == 'M' && grid[x + 3, y + 3] == 'X') {
					ret++;
				}

				if (grid[x + 0, y + 3] == 'X' && grid[x + 1, y + 2] == 'M' && grid[x + 2, y + 1] == 'A' && grid[x + 3, y + 0] == 'S') {
					ret++;
				} else if (grid[x + 0, y + 3] == 'S' && grid[x + 1, y + 2] == 'A' && grid[x + 2, y + 1] == 'M' && grid[x + 3, y + 0] == 'X') {
					ret++;
				}
			}
		}

		return ret;
	}

	private static int SearchPart2(char[,] grid)
	{
		var xBound = grid.GetLength(0) - 2;
		var yBound = grid.GetLength(1) - 2;
		var ret    = 0;

		for (var y = 0; y < yBound; y++) {
			for (var x = 0; x < xBound; x++) {
				// if there's no A in the middle, we can exit early
				if (grid[x + 1, y + 1] != 'A') {
					continue;
				}

				if (grid[x + 0, y + 0] == 'M' && grid[x + 2, y + 0] == 'M' && grid[x + 0, y + 2] == 'S' && grid[x + 2, y + 2] == 'S') {
					ret++; // M on the top, S on the bottom
				} else if (grid[x + 0, y + 0] == 'S' && grid[x + 2, y + 0] == 'S' && grid[x + 0, y + 2] == 'M' && grid[x + 2, y + 2] == 'M') {
					ret++; // M on the bottom, S on the top
				} else if (grid[x + 0, y + 0] == 'M' && grid[x + 2, y + 0] == 'S' && grid[x + 0, y + 2] == 'M' && grid[x + 2, y + 2] == 'S') {
					ret++; // M on the left, S on the right
				} else if (grid[x + 0, y + 0] == 'S' && grid[x + 2, y + 0] == 'M' && grid[x + 0, y + 2] == 'S' && grid[x + 2, y + 2] == 'M') {
					ret++; // M on the right, S on the left
				}
			}
		}

		return ret;
	}
}
