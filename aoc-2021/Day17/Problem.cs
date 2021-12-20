using Xunit;

namespace Day17;

public class Problem
{
	internal static (int, int) Main(string fileName)
	{
		var input = File.ReadAllLines(fileName).First();
		var match = System.Text.RegularExpressions.Regex.Match(input, @"x=(?<xmin>[-\d]+)\.\.(?<xmax>[-\d]+), y=(?<ymin>[-\d]+)\.\.(?<ymax>[-\d]+)");
		var xmin  = int.Parse(match.Groups["xmin"].Value);
		var xmax  = int.Parse(match.Groups["xmax"].Value);
		var ymin  = int.Parse(match.Groups["ymin"].Value);
		var ymax  = int.Parse(match.Groups["ymax"].Value);

		Console.WriteLine($"Target area is {xmin}..{xmax} {ymin}..{ymax}");

		var ymaxes = SimulateAll(xmin, xmax, ymin, ymax).ToList();
		var p1     = ymaxes.Max();

		Console.WriteLine($"part 1: {p1}"); // part 1 is 5253 with the main input
		Console.WriteLine($"part 2: {ymaxes.Count}"); // part 2 is 5253 with the main input

		return (p1, ymaxes.Count);
	}

	private static IEnumerable<int> SimulateAll(int xMinimum, int xMaximum, int yMinimum, int yMaximum)
	{
		var ymax = Math.Max(Math.Abs(yMinimum), Math.Abs(yMaximum));

		for (var xv = 0; xv <= xMaximum; xv++) {
			for (var yv = yMinimum - 1; yv < ymax; yv++) {
				var ym = Simulate(xv, yv, xMinimum, xMaximum, yMinimum, yMaximum);

				if (ym == null) {
					//Console.WriteLine("didn't hit target");
				} else {
					//Console.WriteLine($"{xv},{yv}");
					yield return ym.Value;
				}
			}
		}
	}

	private static int? Simulate(int xVelocity, int yVelocity, int xMinimum, int xMaximum, int yMinimum, int yMaximum)
	{
		var x    = 0;
		var y    = 0;
		var ymax = int.MinValue;

		while (true) {
			x += xVelocity;
			y += yVelocity;

			//Console.WriteLine($"{x},{y}");

			ymax = Math.Max(ymax, y);

			// if we hit the target, get out
			if (x >= xMinimum && x <= xMaximum && y >= yMinimum && y <= yMaximum) {
				return ymax;
			}

			if (xVelocity > 0) {
				xVelocity -= 1;

				// if we are not moving in X anymore, and we are short of the target, bail
				if (xVelocity == 0 && x < xMinimum) {
					return null;
				}
			} else if (xVelocity < 0) {
				xVelocity += 1;
			}

			yVelocity -= 1;

			// if we are past the target in either X or Y, bail
			if (x > xMaximum || y < yMinimum) {
				return null;
			}
		}
	}

	[Fact(DisplayName = "Day 17 Sample Input")]
	public void SampleInputFunctionCorrectly()
	{
		var (p1, p2) = Main("../../../Day17/input_sample.txt");

		Assert.Equal(45, p1);
		Assert.Equal(112, p2);
	}

	[Fact(DisplayName = "Day 17 Main Input")]
	public void MainInputFunctionCorrectly()
	{
		var (p1, p2) = Main("../../../Day17/input.txt");

		Assert.Equal(5253, p1);
		Assert.Equal(1770, p2);
	}
}
