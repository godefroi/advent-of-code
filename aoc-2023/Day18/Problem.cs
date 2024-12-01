using System.Text;

namespace AdventOfCode.Year2023.Day18;

// shoelace formula: https://en.wikipedia.org/wiki/Shoelace_formula
// pick's theorem: https://en.wikipedia.org/wiki/Pick%27s_theorem

public partial class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	private readonly static Coordinate _north = new(0, -1);
	private readonly static Coordinate _south = new(0, 1);
	private readonly static Coordinate _east = new(1, 0);
	private readonly static Coordinate _west = new(-1, 0);

	public static (long, long) Execute(string[] input)
	{
		var p1CurCoord = new Coordinate(0, 0);
		var p2CurCoord = new Coordinate(0, 0);
		var p1Shoelace = new Shoelace();
		var p2Shoelace = new Shoelace();
		var p1TotalLen = 0L;
		var p2TotalLen = 0L;

		for (var i = 0; i < input.Length; i++) {
			var lineSpan  = input[i].AsSpan();
			var p1Dir     = lineSpan[0] switch { 'U' => _north, 'D' => _south, 'R' => _east, 'L' => _west, _ => throw new InvalidOperationException("Invalid p1 dir") };
			var p1Len     = int.Parse(lineSpan[2..^10]);
			var p2Dir     = lineSpan[^2] switch { '0' => _east, '1' => _south, '2' => _west, '3' => _north, _ => throw new InvalidOperationException("Invalid p2 dir") };
			var p2Len     = int.Parse(lineSpan[^7..^2], System.Globalization.NumberStyles.HexNumber);

			//Console.WriteLine($"{input[i]} -> dir {lineSpan[^2]} len {p2Len}");

			p1TotalLen += p1Len;
			p2TotalLen += p2Len;

			for (var j = 0; j < p1Len; j++) {
				p1CurCoord += p1Dir;
				p1Shoelace.AddCoordinate(p1CurCoord);
			}

			for (var j = 0; j < p2Len; j++) {
				p2CurCoord += p2Dir;
				p2Shoelace.AddCoordinate(p2CurCoord);
			}
		}

		//var sb = new StringBuilder();
		// for (var y = minY; y <= maxY; y++) {
		// 	for (var x = minX; x <= maxX; x++) {
		// 		sb.Append(coordSet.Contains((x, y)) ? '#' : '.');
		// 	}

		// 	Console.WriteLine(sb.ToString());
		// 	sb.Clear();
		// }

		//Console.WriteLine($"p1TotalLen: {p1TotalLen}");
		//Console.WriteLine($"p2TotalLen: {p2TotalLen}");

		var p1Area = p1Shoelace.Calculate();
		var part1  = (p1TotalLen / 2) + p1Area + 1;
		var p2Area = p2Shoelace.Calculate();
		var part2  = (p2TotalLen / 2) + p2Area + 1;

		// 60005733816 is too low on part 2

		return (part1, part2);
	}

	private static HashSet<Coordinate> FillPolygon(HashSet<Coordinate> border)
	{
		var minX    = border.Min(c => c.X);
		var maxX    = border.Max(c => c.X);
		var minY    = border.Min(c => c.Y);
		var maxY    = border.Max(c => c.Y);
		var offset  = new Coordinate(-minX, -minY);
		var center  = new Coordinate((maxX - minX) / 2, (maxY - minY) / 2); // yep, it's a wild guess...
		var pending = new Stack<Coordinate>();
		var inside  = new HashSet<Coordinate>();

		pending.Push(center);

		while (pending.Count > 0) {
			var thisCoordinate  = pending.Pop();
			var northCoordinate = thisCoordinate + _north;
			var southCoordinate = thisCoordinate + _south;
			var eastCoordinate  = thisCoordinate + _east;
			var westCoordinate  = thisCoordinate + _west;

			if (thisCoordinate.X > maxX || thisCoordinate.X < minX || thisCoordinate.Y > maxY || thisCoordinate.Y < minY) {
				throw new InvalidOperationException($"We must've started outside the polygon ({center})");
			}

			if (!(border.Contains(northCoordinate) || inside.Contains(northCoordinate))) pending.Push(northCoordinate);
			if (!(border.Contains(southCoordinate) || inside.Contains(southCoordinate))) pending.Push(southCoordinate);
			if (!(border.Contains(eastCoordinate) || inside.Contains(eastCoordinate))) pending.Push(eastCoordinate);
			if (!(border.Contains(westCoordinate) || inside.Contains(westCoordinate))) pending.Push(westCoordinate);

			inside.Add(thisCoordinate);
		}

		return inside;
	}

	private class Shoelace
	{
		private long _sum;
		private Coordinate _first = Coordinate.Empty;
		private Coordinate _prev = Coordinate.Empty;

		public void AddCoordinate(Coordinate coordinate)
		{
			// first time through, we just keep hold of the coordinate
			if (_prev == Coordinate.Empty) {
				_first = coordinate;
				_prev = coordinate;
				return;
			}

			_sum += (_prev.X * coordinate.Y) - (_prev.Y * coordinate.X);
			_prev = coordinate;
		}

		public long Calculate()
		{
			_sum += (_prev.X * _first.Y) - (_prev.Y * _first.X);

			return Math.Abs(_sum / 2);
		}
	}

	public class Tests
	{
		[Fact]
		public void ShoelaceWorks()
		{
			var sl = new Shoelace();

			sl.AddCoordinate((2, 7));
			sl.AddCoordinate((10, 1));
			sl.AddCoordinate((8, 6));
			sl.AddCoordinate((11, 7));
			sl.AddCoordinate((7, 10));

			Assert.Equal(32, sl.Calculate());
		}
	}

}
