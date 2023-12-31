namespace aoc_2022.Day20;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Main, typeof(Problem));

	private const long DECRYPTION_KEY = 811589153;

	public static (long, long) Main(string[] input)
	{
		var numbers   = input.Select(long.Parse).ToArray();
		var mixedList = MixList(numbers);
		var zeroPos   = Array.IndexOf(mixedList, 0);
		var coord1    = mixedList[(1000 + zeroPos) % mixedList.Length];
		var coord2    = mixedList[(2000 + zeroPos) % mixedList.Length];
		var coord3    = mixedList[(3000 + zeroPos) % mixedList.Length];
		var part1     = coord1 + coord2 + coord3;

		numbers   = input.Select(long.Parse).Select(n => n * DECRYPTION_KEY).ToArray();
		mixedList = MixList(numbers, 10);
		zeroPos   = Array.IndexOf(mixedList, 0);
		coord1    = mixedList[(1000 + zeroPos) % mixedList.Length];
		coord2    = mixedList[(2000 + zeroPos) % mixedList.Length];
		coord3    = mixedList[(3000 + zeroPos) % mixedList.Length];
		var part2 = coord1 + coord2 + coord3;

		return (part1, part2);
	}

	private static long[] MixList(long[] numbers, int mixCount = 1)
	{
		var positions = Enumerable.Range(0, numbers.Length).ToArray();

		//Console.WriteLine("Initial arrangement:");
		//Console.WriteLine(string.Join(", ", numbers.Zip(positions).OrderBy(z => z.Second).Select(z => z.First)));

		for (var k = 0; k < mixCount; k++) {
			for (var i = 0; i < numbers.Length; i++) {
				if (numbers[i] == 0) {
					//Console.WriteLine();
					//Console.WriteLine("0 does not move:");
					//Console.WriteLine(string.Join(", ", numbers.Zip(positions).OrderBy(z => z.Second).Select(z => z.First)));
					continue;
				}

				var oldPosition = positions[i];
				var newPosition = WrapPosition(positions[i], numbers[i], numbers.Length);

				if (newPosition < positions[i]) {
					// number is moving left; everything left of the old position and right of the new position moves right one
					//for (var j = newPosition; j < positions[i]; j++) {
					//	positions[j]++;
					//}
					for (var j = 0; j < positions.Length; j++) {
						if (positions[j] >= newPosition && positions[j] < oldPosition) {
							positions[j]++;
						}
					}

					positions[i] = newPosition;
				} else if (newPosition > positions[i]) {
					// number is moving right; everything right of the old position and left of the new position moves left one
					//for (var j = positions[i] + 1; j <= newPosition; j++) {
					//	positions[j]--;
					//}
					for (var j = 0; j < positions.Length; j++) {
						if (positions[j] > oldPosition && positions[j] <= newPosition) {
							positions[j]--;
						}
					}

					positions[i] = newPosition;
				}

				//Console.WriteLine();
				//Console.WriteLine($"{numbers[i]} moves...");
				//Console.WriteLine(string.Join(", ", numbers.Zip(positions).OrderBy(z => z.Second).Select(z => z.First)));
			}

			//Console.WriteLine($"After {k + 1}: {string.Join(", ", numbers.Zip(positions).OrderBy(z => z.Second).Select(z => z.First))}");
		}

		return numbers.Zip(positions).OrderBy(z => z.Second).Select(z => z.First).ToArray();
	}

	private static int WrapPosition(int currentPosition, long shiftCount, int listSize)
	{
		if (shiftCount == 0) {
			return currentPosition;
		}

		var newPosition = (currentPosition + shiftCount) % (listSize - 1);

		if (newPosition < 0) {
			newPosition += listSize - 1;
		}

		return (int)newPosition;
	}

	[Theory]
	[InlineData(0,  1, 7, 1)]
	[InlineData(0,  2, 7, 2)]
	[InlineData(1, -3, 7, 4)]
	[InlineData(2,  3, 7, 5)]
	[InlineData(2, -2, 7, 0)]
	[InlineData(3,  0, 7, 3)]
	[InlineData(5,  4, 7, 3)]
	[InlineData(6,  4, 7, 4)]
	public void WrappingWorks(int currentPosition, int shiftCount, int listSize, int newPosition)
	{
		Assert.Equal(newPosition, WrapPosition(currentPosition, shiftCount, listSize));
	}
}
