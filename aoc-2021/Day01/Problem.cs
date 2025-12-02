namespace AdventOfCode.Year2021.Day01;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long inc, long winc) Execute(string[] input)
	{
		var numbers = input.Select(int.Parse);
		var last    = int.MaxValue;
		var lSum    = int.MaxValue;
		var inc     = 0;
		var winc    = 0;
		var win     = new SlidingWindow(3);

		foreach (var cur in numbers) {
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

	[Test]
	[DisplayName("Day 01 Sample Input")]
	public async Task SampleInputFunctionCorrectly()
	{
		var (inc, winc) = Execute(ReadFileLines("input_sample.txt"));

		await Assert.That(inc).IsEqualTo(7);
		await Assert.That(winc).IsEqualTo(5);
	}

	[Test]
	[DisplayName("Day 01 Main Input")]
	public async Task MainInputFunctionCorrectly()
	{
		var (inc, winc) = Execute(ReadFileLines("input.txt"));

		await Assert.That(inc).IsEqualTo(1462);
		await Assert.That(winc).IsEqualTo(1497);
	}
}
