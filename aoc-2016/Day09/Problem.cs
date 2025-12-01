using System.Buffers;
using CommunityToolkit.HighPerformance;

namespace AdventOfCode.Year2016.Day09;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		var chars = new char[input.Sum(s => s.Length)];
		var pos   = 0;

		for (var i = 0; i < input.Length; i++) {
			Array.Copy(input[i].ToCharArray(), 0, chars, pos, input[i].Length);
			pos += input[i].Length;
		}

		return (CountV1(chars), Count(chars, true));
	}

	private static long CountV1(ReadOnlySpan<char> chars) => Count(chars, false);

	private static long Count(ReadOnlySpan<char> chars, bool recurse)
	{
		var total = 0L;

		while (!chars.IsEmpty) {
			var mStart = chars.IndexOf('(');

			if (mStart == -1) {
				// no repeat markers
				total += chars.Length;
				break;
			} else if (mStart == 0) {
				// read out the repeat marker and handle it
				var mEnd  = chars.IndexOf(')');
				var mSpan = chars[1..mEnd];
				var xpos  = mSpan.IndexOf('x');
				var ccnt  = int.Parse(mSpan[..xpos]);
				var rcnt  = int.Parse(mSpan[(xpos + 1)..]);

				if (recurse) {
					// process the repeated chunk
					total += rcnt * Count(chars[(mEnd + 1)..(mEnd + 1 + ccnt)], true);
				} else {
					// add the repeated chunk count
					total += ccnt * rcnt;
				}

				// advance past the chunk
				chars = chars[(mEnd + 1 + ccnt)..];
			} else {
				// read and count up to the marker
				total += mStart;
				chars = chars[mStart..];
			}
		}

		return total;
	}

	[Test]
	[Arguments("ADVENT", 6)]
	[Arguments("A(1x5)BC", 7)]
	[Arguments("(3x3)XYZ", 9)]
	[Arguments("A(2x2)BCD(2x2)EFG", 11)]
	[Arguments("(6x1)(1x3)A", 6)]
	[Arguments("X(8x2)(3x3)ABCY", 18)]
	public async Task CountV1FunctionsCorrectly(string input, long expected) => await Assert.That(CountV1(input)).IsEqualTo(expected);

	[Test]
	[Arguments("X(8x2)(3x3)ABCY", 20)]
	public async Task CountV2FunctionsCorrectly(string input, long expected) => await Assert.That(Count(input, true)).IsEqualTo(expected);

}

internal class MemorySegment<T> : ReadOnlySequenceSegment<T>
{
    public MemorySegment(ReadOnlyMemory<T> memory)
    {
        Memory = memory;
    }

    public MemorySegment<T> Append(ReadOnlyMemory<T> memory)
    {
        var segment = new MemorySegment<T>(memory) {
            RunningIndex = RunningIndex + Memory.Length
        };

        Next = segment;

        return segment;
    }
}
