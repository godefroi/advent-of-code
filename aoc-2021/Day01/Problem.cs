using Xunit;

namespace Day01;

public class Problem
{
	internal static (int inc, int winc) Main(string fileName)
	{
		var input = File.ReadAllText(fileName);
		var last  = int.MaxValue;
		var lSum  = int.MaxValue;
		var inc   = 0;
		var winc  = 0;
		var win   = new SlidingWindow(3);

		using var sr = new StringReader(input);

		while (sr.Peek() > -1) {
			var cur = int.Parse(sr.ReadLine()!);

			// add to the window
			win.Add(cur);

			if (cur > last) {
				inc++;
			}

			// if our window is full, then handle that
			if (win.Count() == 3) {
				var curSum = win.Sum();

				if (curSum > lSum) {
					winc++;
				}

				lSum = curSum;
			}

			last = cur;
		}

		Console.WriteLine($"part 1: {inc}");
		Console.WriteLine($"part 2: {winc}");

		return (inc, winc);
	}

	[Fact(DisplayName = "Day 01 Sample Input")]
	public void SampleInputFunctionCorrectly()
	{
		var (inc, winc) = Main("../../../Day01/input_sample.txt");

		Assert.Equal(7, inc);
		Assert.Equal(5, winc);
	}

	[Fact(DisplayName = "Day 01 Main Input")]
	public void MainInputFunctionCorrectly()
	{
		var (inc, winc) = Main("../../../Day01/input.txt");

		Assert.Equal(1462, inc);
		Assert.Equal(1497, winc);
	}
}
