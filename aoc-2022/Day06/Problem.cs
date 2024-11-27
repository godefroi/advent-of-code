namespace AdventOfCode.Year2022.Day06;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Main, typeof(Problem));

	public static (long, long) Main(string[] input)
	{
		var packet = input.Single();
		var part1  = FindMarker(packet.ToCharArray(), 4);
		var part2  = FindMarker(packet.ToCharArray(), 14);

		// part1 is 1262
		// part2 is 3444
		return (part1, part2);
	}

	private static int FindMarker(char[] dataStream, int length)
	{
		for (var i = length; i < dataStream.Length; i++) {
			if (dataStream[(i - length)..i].Distinct().Count() == length) {
				return i;
			}
		}

		throw new InvalidDataException("No marker was found.");
	}

	[Theory]
	[InlineData(7, 19, "mjqjpqmgbljsphdztnvjfqwrcgsmlb")]
	[InlineData(5, 23, "bvwbjplbgvbhsrlpgdmjqwftvncz")]
	[InlineData(6, 23, "nppdvjthqldpwncqszvftbrmjlhg")]
	[InlineData(10, 29, "nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg")]
	[InlineData(11, 26, "zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw")]
	public void MarkersFoundCorrectly(int expectedPacket, int expectedMessage, string dataStream)
	{
		Assert.Equal(expectedPacket, FindMarker(dataStream.ToCharArray(), 4));
		Assert.Equal(expectedMessage, FindMarker(dataStream.ToCharArray(), 14));
	}
}
