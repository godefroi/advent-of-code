using Xunit;

namespace Day14;

public class Problem
{
	internal static (long p1, long p2) Main(string fileName)
	{
		var input   = File.ReadAllLines(fileName).ToList();
		var polymer = input[0].ToList();
		var rules   = input.Skip(2).ToDictionary(r => (First: r[0], Second: r[1]), r => r[6]);
		var ccounts = polymer.GroupBy(c => c).ToDictionary(g => g.Key, g => g.LongCount());
		var pcounts = polymer.Zip(polymer.Skip(1)).ToDictionary(p => (First: p.First, Second: p.Second), p => 1L);
		
		long p1 = 0, p2 = 0;

		foreach (var rule in rules) {
			if (!pcounts.ContainsKey(rule.Key)) {
				pcounts.Add(rule.Key, 0L);
			}
		}
		
		foreach (var rule in rules) {
			if (!ccounts.ContainsKey(rule.Key.First)) {
				ccounts.Add(rule.Key.First, 0L);
			}
			if (!ccounts.ContainsKey(rule.Key.Second)) {
				ccounts.Add(rule.Key.Second, 0L);
			}
		}
		
		for (var i = 0; i < 40; i++) {
			var ocounts = pcounts.ToDictionary(p => p.Key, p => p.Value);
			
			foreach (var r in rules) {
				var count = ocounts[r.Key];
				pcounts[r.Key] -= count;
				pcounts[(First: r.Key.First, Second: r.Value)]  += count;
				pcounts[(First: r.Value, Second: r.Key.Second)] += count;
				ccounts[r.Value] += count;
			}
		
			if (i == 9) {
				p1 = ccounts.Max(kvp => kvp.Value) - ccounts.Min(kvp => kvp.Value);
				Console.WriteLine($"part 1: {p1}"); // part 1 is 2233
			}
		}
		
		p2 = ccounts.Max(kvp => kvp.Value) - ccounts.Min(kvp => kvp.Value);
		Console.WriteLine($"part 2: {p2}"); // part 2 is 2884513602164

		return (p1, p2);
	}

	[Fact(DisplayName = "Day 14 Sample Input")]
	public void SampleInputFunctionCorrectly()
	{
		var (p1, p2) = Main("../../../Day14/input_sample.txt");

		Assert.Equal(1588, p1);
		Assert.Equal(2188189693529, p2);
	}

	[Fact(DisplayName = "Day 14 Main Input")]
	public void MainInputFunctionCorrectly()
	{
		var (p1, p2) = Main("../../../Day14/input.txt");

		Assert.Equal(2233, p1);
		Assert.Equal(2884513602164, p2);
	}
}
