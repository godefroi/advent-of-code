namespace AdventOfCode.Year2023.Day14;

public partial class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, null);

	public static (long, long) Execute(string[] input)
	{
		var p1map = CreateMap(input, c => c);
		var p2map = CreateMap(input, c => c); // cause why not, that's why

		TiltMap(p1map, Direction.North);

		var part1    = CalculateLoad(p1map);
		var trialCnt = 300; // for our input, we need ~300 to find the loop; other inputs may need more
		var weights  = new int[trialCnt];
		var failure  = new int[trialCnt];

		(int offset, int length) largestLoop = (0, 0);

		// run 1000 trials
		for (var i = 0; i < trialCnt; i++) {
			TiltMap(p2map, Direction.North);
			TiltMap(p2map, Direction.West);
			TiltMap(p2map, Direction.South);
			TiltMap(p2map, Direction.East);

			weights[i] = CalculateLoad(p2map);
		}

		for (var i = 0; i < trialCnt / 2; i++) {
			KnuthMorrisPratt.ComputeFailureFunction(weights, i, trialCnt - i, failure);

			// we're looking for a failure function that does NOT end with (0)
			if (failure[trialCnt - i - 1] == 0) {
				continue;
			}

			// then, count the zeroes at the front of the failure function
			var loopLen = 0;

			while (failure[loopLen] == 0) {
				loopLen++;
			}

			var monotonicallyIncreases = true;

			for (var j = loopLen; j < trialCnt - i; j++) {
				if (failure[j] != failure[j - 1] + 1) {
					//Console.WriteLine($"for offset {i}, failure[{j}] = {failure[j]} and failure[{j - 1}] = {failure[j - 1]}");
					monotonicallyIncreases = false;
					break;
				}
			}

			if (!monotonicallyIncreases) {
				continue;
			}

			// Console.WriteLine($"loop of length {loopLen} found at offset {i}");
			// Console.WriteLine($"{i} -> {string.Join(',', failure.Take(trialCnt - i))} ({failure[trialCnt - i - 1]})");
			// Console.WriteLine($"{string.Join(',', weights.Skip(i))}");
			// Console.WriteLine();

			if (loopLen > largestLoop.length) {
				largestLoop = (i, loopLen);
			}
		}

		//Console.WriteLine($"Largest loop found, of length {largestLoop.length}, begins at offset {largestLoop.offset}");

		var targetCycles = 1000000000;
		var loopedCycles = targetCycles - largestLoop.offset;
		var (loops, remainder) = Math.DivRem(loopedCycles, largestLoop.length);

		//Console.WriteLine($"We'll need {loops} loops, for a total cycle count of {(loops * largestLoop.length) + largestLoop.offset} (rem {remainder})");
		//Console.WriteLine($"weight at index [{largestLoop.offset} + {remainder} - 1] is {weights[largestLoop.offset + remainder - 1]}");

		return (part1, weights[largestLoop.offset + remainder - 1]);
	}

	private static void TiltMap(char[,] map, Direction direction)
	{
		var width  = map.GetLength(0);
		var height = map.GetLength(1);

		void SwapIfEmpty(char[,] map, int fromX, int fromY, int toX, int toY)
		{
			if (map[toX, toY] == '.') {
				map[toX, toY] = 'O';
				map[fromX, fromY] = '.';
			}
		}

		switch (direction) {
			case Direction.North:
				// work column-by-column
				for (var x = 0; x < width; x++) {
					// then, for each row, roll rocks north as far as they can go
					// (starting at row 1, since rocks at row 0 can't roll any farther)
					for (var y = 1; y < height; y++) {
						if (map[x, y] == 'O') {
							// roll this rock north
							var i = y;

							while (i > 0 && map[x, i - 1] == '.') {
								i--;
							}

							// the rock will roll to row i
							SwapIfEmpty(map, x, y, x, i);
						}
					}
				}
				break;

			case Direction.South:
				for (var x = 0; x < width; x++) {
					for (var y = height - 2; y >= 0; y--) {
						if (map[x, y] == 'O') {
							var i = y;

							while (i < height - 1 && map[x, i + 1] == '.') {
								i++;
							}

							SwapIfEmpty(map, x, y, x, i);
						}
					}
				}
				break;

			case Direction.West:
				for (var y = 0; y < height; y++) {
					for (var x = 1; x < width; x++) {
						if (map[x, y] == 'O') {
							var i = x;

							while (i > 0 && map[i - 1, y] == '.') {
								i--;
							}

							SwapIfEmpty(map, x, y, i, y);
						}
					}
				}
				break;

			case Direction.East:
				for (var y = 0; y < height; y++) {
					for (var x = width - 2; x >= 0; x--) {
						if (map[x, y] == 'O') {
							var i = x;

							while (i < width - 1 && map[i + 1, y] == '.') {
								i++;
							}

							SwapIfEmpty(map, x, y, i, y);
						}
					}
				}
				break;
		}
	}

	private static int CalculateLoad(char[,] map)
	{
		var load   = 0;
		var height = map.GetLength(1);
		var width  = map.GetLength(0);

		for (var y = 0; y < height; y++) {
			for (var x = 0; x < width; x++) {
				if (map[x, y] == 'O') {
					load += height - y;
				}
			}
		}

		return load;
	}

	private enum Direction
	{
		North,
		South,
		East,
		West,
	}

	public class Tests
	{
		[Fact]
		public void TiltWorksCorrectly()
		{
			var map = CreateMap(ReadFileLines("inputSample.txt"), c => c);

			TiltMap(map, Direction.North);

			Assert.Equal('O', map[0, 3]);
		}
	}
}
