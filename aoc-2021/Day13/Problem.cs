using Xunit;

namespace Day13;

public class Problem
{
	internal static int Main(string fileName)
	{
		var input = File.ReadAllLines(fileName).ToList();
		var dots  = input.Where(i => i.IndexOf(',') > -1).Select(l => l.Split(',')).Select(p => new Dot(int.Parse(p[0]), int.Parse(p[1]))).ToHashSet();
		var folds = input.Where(i => i.StartsWith("fold")).Select(i => i.Split(' ', '=')).Select(p => (Dimension: p[2][0], Value: int.Parse(p[3]))).ToList();
		
		Fold(dots, folds.First());
		
		var p1 = dots.Distinct().Count();

		Console.WriteLine($"part 1: {p1}"); // part 1 is 720
		
		foreach (var fold in folds.Skip(1)) {
			Fold(dots, fold);
		}
		
		var width = dots.Max(d => d.X);
		
		foreach (var row in dots.GroupBy(d => d.Y).OrderBy(g => g.Key)) {
			var lit = row.Select(d => d.X).ToList();
			Console.WriteLine(new string(Enumerable.Range(0, width + 1).Select(x => row.Any(d => d.X == x) ? '#' : ' ').ToArray()));
		}

		return p1;
	}

	private static void Fold(HashSet<Dot> dots, (char Dimension, int Value) fold)
	{
		if (fold.Dimension == 'y') {
			// horizontal fold
			var to_fold = dots.Where(d => d.Y > fold.Value).ToHashSet();
			dots.RemoveWhere(d => to_fold.Contains(d));
			foreach (var dot in to_fold.Select(dot => new Dot(dot.X, fold.Value - (dot.Y - fold.Value)))) {
				dots.Add(dot);
			}
		} else if (fold.Dimension == 'x') {
			var to_fold = dots.Where(d => d.X > fold.Value).ToHashSet();
			dots.RemoveWhere(d => to_fold.Contains(d));
			foreach (var dot in to_fold.Select(dot => new Dot(fold.Value - (dot.X - fold.Value), dot.Y))) {
				dots.Add(dot);
			}
		} else {
			throw new InvalidOperationException($"Cannot fold along dimension {fold.Dimension}");
		}
	}

	[Fact(DisplayName = "Day 13 Sample Input")]
	public void SampleInputFunctionCorrectly()
	{
		var p1 = Main("../../../Day13/input_sample.txt");

		Assert.Equal(17, p1);
	}

	[Fact(DisplayName = "Day 13 Main Input")]
	public void MainInputFunctionCorrectly()
	{
		var p1 = Main("../../../Day13/input.txt");

		Assert.Equal(720, p1);
	}

	record struct Dot(int X, int Y);
}
