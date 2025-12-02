namespace AdventOfCode.Year2021.Day06;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long p1, long p2) Execute(string[] input)
	{
		var numbers  = input.First().Split(',').Select(int.Parse).ToList();
		var buckets = Enumerable.Range(0, 9).Select(i => numbers.LongCount(n => n == i)).ToArray();
		long p1 = 0, p2 = 0;

		//Console.WriteLine(input.Count);
		//Console.WriteLine($"Initial state: {string.Join(",", input)}");

		for (var day = 0; day < 256; day++) {
			var nbuckets = new long[9];

			// age everyone one day
			Array.Copy(buckets, 1, nbuckets, 0, 8);

			// reset everyone who produced a fish
			nbuckets[6] += buckets[0];

			// add any new fish
			nbuckets[8] = buckets[0];
			buckets     = nbuckets;

			//Console.WriteLine($"After {day + 1} days: {string.Join(",", buckets)}");
			if (day == 79) {
				p1 = buckets.Sum();
			}
		}

		p2 = buckets.Sum();

		Console.WriteLine($"part 1: {p1}"); // part 1 is 346063
		Console.WriteLine($"part 2: {p2}"); // part 2 is 1572358335990

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
