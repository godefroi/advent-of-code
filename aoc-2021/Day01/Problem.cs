namespace aoc_2021.Day01;

public class Problem
{
	public static (int inc, int winc) Main(string fileName)
	{
		var input = ReadFileLines(fileName, int.Parse);
		var last  = int.MaxValue;
		var lSum  = int.MaxValue;
		var inc   = 0;
		var winc  = 0;
		var win   = new SlidingWindow(3);

		foreach (var cur in input) {
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

		return (inc, winc);
	}

	[Fact(DisplayName = "Day 01 Sample Input")]
	public void SampleInputFunctionCorrectly()
	{
		var (inc, winc) = Main("input_sample.txt");

		Assert.Equal(7, inc);
		Assert.Equal(5, winc);
	}

	[Fact(DisplayName = "Day 01 Main Input")]
	public void MainInputFunctionCorrectly()
	{
		var (inc, winc) = Main("input.txt");

		Assert.Equal(1462, inc);
		Assert.Equal(1497, winc);
	}
}
