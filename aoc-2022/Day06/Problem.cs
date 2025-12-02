namespace AdventOfCode.Year2022.Day06;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
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

	[Test]
	[Arguments(7, 19, "mjqjpqmgbljsphdztnvjfqwrcgsmlb")]
	[Arguments(5, 23, "bvwbjplbgvbhsrlpgdmjqwftvncz")]
	[Arguments(6, 23, "nppdvjthqldpwncqszvftbrmjlhg")]
	[Arguments(10, 29, "nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg")]
	[Arguments(11, 26, "zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw")]
	public async Task MarkersFoundCorrectly(int expectedPacket, int expectedMessage, string dataStream)
	{
		await Assert.That(FindMarker(dataStream.ToCharArray(), 4)).IsEqualTo(expectedPacket);
		await Assert.That(FindMarker(dataStream.ToCharArray(), 14)).IsEqualTo(expectedMessage);
	}
}
