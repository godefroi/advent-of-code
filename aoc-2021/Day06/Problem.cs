using Xunit;

namespace Day06;

public class Problem
{
	internal static (long p1, long p2) Main(string fileName)
	{
		var input   = File.ReadAllLines(fileName).First().Split(',').Select(s => int.Parse(s)).ToList();
		var buckets = Enumerable.Range(0, 9).Select(i => input.LongCount(n => n == i)).ToArray();
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

	[Fact(DisplayName = "Day 06 Sample Input")]
	public void SampleInputFunctionCorrectly()
	{
		var (p1, p2) = Main("../../../Day06/input_sample.txt");

		Assert.Equal(5934, p1);
		Assert.Equal(26984457539, p2);
	}

	[Fact(DisplayName = "Day 06 Main Input")]
	public void MainInputFunctionCorrectly()
	{
		var (p1, p2) = Main("../../../Day06/input.txt");

		Assert.Equal(346063, p1);
		Assert.Equal(1572358335990, p2);
	}
}
