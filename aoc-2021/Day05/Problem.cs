using System.Text.RegularExpressions;

using Xunit;

namespace Day05;

public class Problem
{
	internal static (int p1, int p2) Main(string fileName)
	{
		var input = File.ReadAllLines(fileName);

		//foreach (var s in ParseSegments(input)) {
		//	Console.WriteLine($"{s.Start.X},{s.Start.Y} to {s.End.X},{s.End.Y}");
		//	foreach (var p in ExpandPoints(s)) {
		//		Console.WriteLine($"\t{p.X},{p.Y}");
		//	}break;
		//}
		//Console.WriteLine();

		//var points = ParseSegments(input).SelectMany(s => ExpandPoints(s)).ToList();
		////var width  = points.Select(p => p.X).Max();
		////var height = points.Select(p => p.Y).Max();
		//var width = 10;
		//var height = 9;

		//for (var y = 0; y <= height; y++) {
		//	for (var x = 0; x <= width; x++) {
		//		Console.Write($"{points.Count(p => p.X == x && p.Y == y).ToString().Replace("0", ".")} ");
		//	}
		//	Console.WriteLine();
		//}
		//Console.WriteLine();

		var segs = ParseSegments(input).ToList();
		var p1   = segs.Where(s => s.Start.X == s.End.X || s.Start.Y == s.End.Y).SelectMany(s => ExpandPoints(s)).GroupBy(p => p).Where(g => g.Count() > 1).Count();
		var p2   = segs.SelectMany(s => ExpandPoints(s)).GroupBy(p => p).Where(g => g.Count() > 1).Count();

		// part 1 is 7414
		Console.WriteLine($"part 1: {p1}");

		// part 2 is 19676
		Console.WriteLine($"part 2: {p2}");

		return (p1, p2);
	}

	private static IEnumerable<Segment> ParseSegments(IEnumerable<string> lines)
	{
		var regex = new Regex("( -> )|,");

		foreach (var line in lines.Where(s => !string.IsNullOrWhiteSpace(s))) {
			var coords = regex.Split(line);
			yield return new Segment(new Point(int.Parse(coords[0]), int.Parse(coords[1])), new Point(int.Parse(coords[3]), int.Parse(coords[4])));
		}
	}

	private static IEnumerable<Point> ExpandPoints(Segment segment)
	{
		if (segment.Start.Y == segment.End.Y && segment.Start.X != segment.End.X) {
			for (var x = Math.Min(segment.Start.X, segment.End.X); x <= Math.Max(segment.Start.X, segment.End.X); x++) {
				yield return new Point(x, segment.Start.Y);
			}
		} else if (segment.Start.X == segment.End.X && segment.Start.Y != segment.End.Y) {
			for (var y = Math.Min(segment.Start.Y, segment.End.Y); y <= Math.Max(segment.Start.Y, segment.End.Y); y++) {
				yield return new Point(segment.Start.X, y);
			}
		} else if (segment.Start.X != segment.End.X && segment.Start.Y != segment.End.Y) {
			var xinc = segment.Start.X < segment.End.X ? 1 : -1;
			var yinc = segment.Start.Y < segment.End.Y ? 1 : -1;

			for (int x = segment.Start.X, y = segment.Start.Y; x != segment.End.X && y != segment.End.Y; x += xinc, y += yinc) {
				yield return new Point(x, y);
			}

			yield return segment.End;
		} else {
			throw new InvalidOperationException("Dunno.");
		}
	}

	[Fact(DisplayName = "Day 05 Sample Input")]
	public void SampleInputFunctionCorrectly()
	{
		var (p1, p2) = Main("../../../Day05/input_sample.txt");

		Assert.Equal(5, p1);
		Assert.Equal(12, p2);
	}

	[Fact(DisplayName = "Day 05 Main Input")]
	public void MainInputFunctionCorrectly()
	{
		var (p1, p2) = Main("../../../Day05/input.txt");

		Assert.Equal(7414, p1);
		Assert.Equal(19676, p2);
	}

	internal record struct Point(int X, int Y);

	internal record struct Segment(Point Start, Point End);
}
