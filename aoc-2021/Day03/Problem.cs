using Xunit;

namespace Day03;

public class Problem
{
	internal static (int p1, int p2) Main(string fileName)
	{
		var input    = File.ReadAllLines(fileName);
		var counts   = new int[input.First().Length];
		var inpCount = input.Count();

		foreach (var line in input) {
			for (var i = 0; i < counts.Length; i++) {
				if (line[i] == '1') {
					counts[i]++;
				}
			}
		}

		//var counts = input.SelectMany(line => line.Select((c, index) => (index, value: c == '1' ? 1 : 0))  ).GroupBy(o => o.index).OrderBy(g => g.Key).Select(g => g.Sum(i => i.value)).ToArray();

		var gamma   = Convert.ToInt32(new string(counts.Select(c => c > (inpCount - c) ? '1' : '0').ToArray()), 2);
		var epsilon = Convert.ToInt32(new string(counts.Select(c => c > (inpCount - c) ? '0' : '1').ToArray()), 2);

		Console.WriteLine($"part 1: {gamma * epsilon}"); // 1082324

		var oxy_r = FindRating(input, (common, candidate, position) => common == ' ' ? candidate[position] == '1' : candidate[position] == common);
		var co2_r = FindRating(input, (common, candidate, position) => common == ' ' ? candidate[position] == '0' : candidate[position] != common);

		Console.WriteLine($"part 2: {oxy_r * co2_r} (oxy: {oxy_r}, co2: {co2_r})"); // 1353024 (oxy: 486, co2: 2784)

		return (gamma * epsilon, oxy_r * co2_r);
	}

	private static int FindRating(IEnumerable<string> candidates, Func<char, string, int, bool> predicate, int position = 0)
	{
		var ccnt = candidates.Count();

		if (ccnt == 0) {
			throw new InvalidOperationException("Filtered out everything; something went wrong.");
		}

		// if there's only one, return it
		if (ccnt == 1) {
			return Convert.ToInt32(candidates.Single(), 2);
		}

		// otherwise, filter by the criteria at the given position
		var sum    = candidates.Sum(c => int.Parse(c[position].ToString()));
		var common = sum == (ccnt - sum) ? ' ' : (sum > (ccnt - sum) ? '1' : '0');

		return FindRating(candidates.Where(c => predicate(common, c, position)).ToList(), predicate, position + 1);
	}

	[Fact(DisplayName = "Day 03 Sample Input")]
	public void SampleInputFunctionCorrectly()
	{
		var (p1, p2) = Main("../../../Day03/input_sample.txt");

		Assert.Equal(198, p1);
		Assert.Equal(230, p2);
	}

	[Fact(DisplayName = "Day 03 Main Input")]
	public void MainInputFunctionCorrectly()
	{
		var (p1, p2) = Main("../../../Day03/input.txt");

		Assert.Equal(1082324, p1);
		Assert.Equal(1353024, p2);
	}
}
