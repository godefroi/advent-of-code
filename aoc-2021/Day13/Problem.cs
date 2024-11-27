namespace AdventOfCode.Year2021.Day13;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Main);

	public static (long, string) Main(string[] input)
	{
		var dots  = input.Where(i => i.IndexOf(',') > -1).Select(l => l.Split(',')).Select(p => new Dot(int.Parse(p[0]), int.Parse(p[1]))).ToHashSet();
		var folds = input.Where(i => i.StartsWith("fold")).Select(i => i.Split(' ', '=')).Select(p => (Dimension: p[2][0], Value: int.Parse(p[3]))).ToList();

		Fold(dots, folds.First());

		var p1 = dots.Distinct().Count();

		Console.WriteLine($"part 1: {p1}"); // part 1 is 720

		foreach (var fold in folds.Skip(1)) {
			Fold(dots, fold);
		}

		var width = dots.Max(d => d.X);
		var sb    = new StringBuilder();

		foreach (var row in dots.GroupBy(d => d.Y).OrderBy(g => g.Key)) {
			var lit = row.Select(d => d.X).ToList();
			sb.AppendLine(new string(Enumerable.Range(0, width + 1).Select(x => row.Any(d => d.X == x) ? '#' : ' ').ToArray()));
		}
throw new NotImplementedException("This is broken, fix me");
		//return (p1, OCR.Recognize(sb.ToString()));
	}

	private static void Fold(HashSet<Dot> dots, (char Dimension, int Value) fold)
	{
		if (fold.Dimension == 'y') {
			// horizontal fold
			var to_fold = dots.Where(d => d.Y > fold.Value).ToHashSet();
			dots.RemoveWhere(to_fold.Contains);
			foreach (var dot in to_fold.Select(dot => new Dot(dot.X, fold.Value - (dot.Y - fold.Value)))) {
				dots.Add(dot);
			}
		} else if (fold.Dimension == 'x') {
			var to_fold = dots.Where(d => d.X > fold.Value).ToHashSet();
			dots.RemoveWhere(to_fold.Contains);
			foreach (var dot in to_fold.Select(dot => new Dot(fold.Value - (dot.X - fold.Value), dot.Y))) {
				dots.Add(dot);
			}
		} else {
			throw new InvalidOperationException($"Cannot fold along dimension {fold.Dimension}");
		}
	}

	private readonly record struct Dot(int X, int Y);

	[Fact(DisplayName = "Day 13 Sample Input")]
	public void SampleInputFunctionCorrectly()
	{
		var (p1, p2) = Main(ReadFileLines("input_sample.txt"));

		Assert.Equal(17, p1);
	}

	[Fact(DisplayName = "Day 13 Main Input")]
	public void MainInputFunctionCorrectly()
	{
		var (p1, p2) = Main(ReadFileLines("input.txt"));

		Assert.Equal(720, p1);
		Assert.Equal("AHPRPAUZ", p2);
	}
}
