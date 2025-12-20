namespace AdventOfCode.Year2021.Day06;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long p1, long p2) Execute(string[] input)
	{
		Span<long> buckets  = stackalloc long[9];
		Span<long> nbuckets = stackalloc long[9];

		long p1 = 0, p2 = 0;

		for (var i = 0; i < input[0].Length; i += 2) {
			buckets[input[0][i] - '0']++;
		}

		for (var day = 0; day < 256; day++) {
			// age everyone one day
			for (var i = 1; i < buckets.Length; i++) {
				nbuckets[i - 1] = buckets[i];
			}

			// reset everyone who produced a fish
			nbuckets[6] += buckets[0];

			// add any new fish
			nbuckets[8] = buckets[0];

			// copy back to nbuckets
			for (var i = 0; i < buckets.Length; i++) {
				buckets[i] = nbuckets[i];
			}

			if (day == 79) {
				for (var i = 0; i < buckets.Length; i++) {
					p1 += buckets[i];
				}
			}
		}

		for (var i = 0; i < buckets.Length; i++) {
			p2 += buckets[i];
		}

		// Console.WriteLine($"part 1: {p1}"); // part 1 is 346063
		// Console.WriteLine($"part 2: {p2}"); // part 2 is 1572358335990

		return (p1, p2);
	}

	[Test]
	[DisplayName("Day 06 Sample Input")]
	public async Task SampleInputFunctionCorrectly()
	{
		var (p1, p2) = Execute(ReadFileLines("input_sample.txt"));

		await Assert.That(p1).IsEqualTo(5934);
		await Assert.That(p2).IsEqualTo(26984457539);
	}

	[Test]
	[DisplayName("Day 06 Main Input")]
	public async Task MainInputFunctionCorrectly()
	{
		var (p1, p2) = Execute(ReadFileLines("input.txt"));

		await Assert.That(p1).IsEqualTo(346063);
		await Assert.That(p2).IsEqualTo(1572358335990);
	}
}
