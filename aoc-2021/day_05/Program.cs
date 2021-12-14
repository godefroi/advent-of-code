using System.Text.RegularExpressions;

//Console.WriteLine(String.Join(", ", ParseSegments(new[] { "9,7 -> 7,9" }).SelectMany(s => ExpandPoints(s))));
//return;
//var input = @"0,9 -> 5,9
//8,0 -> 0,8
//9,4 -> 3,4
//2,2 -> 2,1
//7,0 -> 7,4
//6,4 -> 2,0
//0,9 -> 2,9
//3,4 -> 1,4
//0,0 -> 8,8
//5,5 -> 8,2".Split(Environment.NewLine).ToList();

var input = File.ReadAllLines(args[0]);

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

// part 1 is 7414
Console.WriteLine(ParseSegments(input).Where(s => s.Start.X == s.End.X || s.Start.Y == s.End.Y).SelectMany(s => ExpandPoints(s)).GroupBy(p => p).Where(g => g.Count() > 1).Count());

// part 2 is 19676
Console.WriteLine(ParseSegments(input).SelectMany(s => ExpandPoints(s)).GroupBy(p => p).Where(g => g.Count() > 1).Count());

IEnumerable<Segment> ParseSegments(IEnumerable<string> lines)
{
	var regex = new Regex("( -> )|,");

	foreach (var line in lines.Where(s => !string.IsNullOrWhiteSpace(s))) {
		var coords = regex.Split(line);
		yield return new Segment(new Point(int.Parse(coords[0]), int.Parse(coords[1])), new Point(int.Parse(coords[3]), int.Parse(coords[4])));
	}
}

IEnumerable<Point> ExpandPoints(Segment segment)
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

internal record struct Point(int X, int Y);

internal record struct Segment(Point Start, Point End);
