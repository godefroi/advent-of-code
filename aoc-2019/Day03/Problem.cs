using System.Diagnostics;

namespace aoc_2019.Day03;

public class Problem
{
	public static (long, int) Main(string fileName)
	{
		var lines = ReadFileLines(fileName);
		var part1 = ShortestIntersectionDistance(lines[0], lines[1]);
		var part2 = FewestStepsToIntersection(lines[0], lines[1]);

		return (part1, part2);
	}

	public static long ShortestIntersectionDistance(string firstWire, string secondWire)
	{
		//Console.WriteLine(Intersects(new Line(10, 10, 20, 10), new Line(15, 5, 15, 15)));
		//Console.WriteLine(Intersects(new Line(10, 10, 20, 10), new Line(25, 5, 25, 15)));

		//var wire_1 = "R75,D30,R83,U83,L12,D49,R71,U7,L72".Split(',');
		//var wire_2 = "U62,R66,U55,R34,D71,R55,D58,R83".Split(',');

		//var wire_1 = "R98,U47,R26,D63,R33,U87,L62,D20,R33,U53,R51".Split(',');
		//var wire_2 = "U98,R91,D20,R16,D67,R40,U7,R15,U6,R7".Split(',');

		var lines_1 = GetLines(firstWire).ToList();
		var lines_2 = GetLines(secondWire).ToList();

		Console.WriteLine($"{lines_1.Count} lines in wire 1");
		Console.WriteLine($"{lines_2.Count} lines in wire 2");

		var dist = long.MaxValue;

		foreach (var l1 in lines_1) {
			foreach (var l2 in lines_2) {
				if (Intersects(l1, l2)) {
					var tdist = Distance(l1, l2);

					Console.WriteLine($"{l1.P1.X},{l1.P1.Y} -> {l1.P2.X},{l1.P2.Y} intersects with {l2.P1.X},{l2.P1.Y} -> {l2.P2.X},{l2.P2.Y}, distance {tdist}");

					if (tdist > 0 && tdist < dist) {
						dist = tdist;
					}
				}
			}
		}

		return dist;
	}

	public static int FewestStepsToIntersection(string firstWire, string secondWire)
	{
		var wire_1  = FollowWire(firstWire.Split(',')).ToList();
		var wire_2  = FollowWire(secondWire.Split(',')).ToList();
		var steps_1 = 0;
		var best    = int.MaxValue;

		foreach (var point_1 in wire_1) {
			var steps_2 = 0;

			steps_1++;

			if (steps_1 > best) {
				Console.WriteLine("We've gone past best just in wire 1");
				return best;
			}

			foreach (var point_2 in wire_2) {
				steps_2++;

				if (steps_1 + steps_2 > best) {
					break;
				}

				if (point_1 == point_2) {
					Console.WriteLine($"Intersection at {point_1.X},{point_1.Y} with distance {steps_1 + steps_2}");

					if (steps_1 + steps_2 < best) {
						best = steps_1 + steps_2;
					}
				}
			}
		}

		return best;
	}

	internal static IEnumerable<Line> GetLines(string line)
	{
		var directions = line.Split(',');
		var last       = new Point(0, 0);

		for( var i = 0; i < directions.Length; i++ ) {
			var len = Convert.ToInt64(directions[i].Substring(1));
			var cur = directions[i][0] switch {
				'R' => new Point(last.X + len, last.Y),
				'L' => new Point(last.X - len, last.Y),
				'U' => new Point(last.X, last.Y - len),
				'D' => new Point(last.X, last.Y + len),
				_ => throw new Exception($"direction {directions[i][0]} is not supported")
			};

			yield return new Line(last, cur);

			last = cur;
		}
	}

	internal static bool Intersects(Line l1, Line l2)
	{
		if( l1.Horizontal ) {
			if( l2.Horizontal ) {
				if( l1.P1.X == l2.P1.X ) {
					if( Math.Max(l1.P1.Y, l1.P2.Y) < Math.Min(l2.P1.Y, l2.P2.Y) || Math.Min(l1.P1.Y, l1.P2.Y) > Math.Max(l2.P1.Y, l2.P2.Y) )
						return false;
					else
						//throw new NotImplementedException("Intersections on two colinear horizontal lines is not implemented.");
						return false;
				} else {
					return false;
				}
			} else {
				// l1 is horizontal, l2 is vertical
				return
					l2.P1.Y >= Math.Min(l1.P1.Y, l1.P2.Y) && l2.P1.Y <= Math.Max(l1.P1.Y, l1.P2.Y) &&
					Math.Min(l2.P1.X, l2.P2.X) <= l1.P1.X && Math.Max(l2.P1.X, l2.P2.X) >= l1.P1.X;
			}
		} else if( l1.Vertical ) {
			if( l2.Vertical ) {
				if( l1.P1.Y == l2.P1.Y ) {
					if( Math.Max(l1.P1.X, l1.P2.X) < Math.Min(l2.P1.X, l2.P2.X) || Math.Min(l1.P1.X, l1.P2.X) > Math.Max(l2.P1.X, l2.P2.X) )
						return false;
					else
						//throw new NotImplementedException("Intersections on two colinear vertical lines is not implemented.");
						return false;
				} else {
					return false;
				}
			} else {
				// l1 is vertical, l2 is horizontal
				return
					l1.P1.Y >= Math.Min(l2.P1.Y, l2.P2.Y) && l1.P1.Y <= Math.Max(l2.P1.Y, l2.P2.Y) &&
					Math.Min(l1.P1.X, l1.P2.X) <= l2.P1.X && Math.Max(l1.P1.X, l1.P2.X) >= l2.P1.X;
			}
		} else {
			throw new Exception("l1 isn't horizontal or vertical...");
		}
	}

	internal static long Distance(Line l1, Line l2)
	{
		if( l1.Horizontal && l2.Vertical )
			return Math.Abs(l1.P1.X) + Math.Abs(l2.P1.Y);
		else if( l1.Vertical && l2.Horizontal )
			return Math.Abs(l1.P1.Y) + Math.Abs(l2.P1.X);
		else
			throw new Exception("Yeah, can't calculate a distance for that");
	}

	internal static IEnumerable<Point> FollowWire(string[] directions)
	{
		var last = new Point(0, 0);

		//yield return last;

		foreach (var dir in directions) {
			var x_step = 0;
			var y_step = 0;
			var len    = Convert.ToInt64(dir.Substring(1));

			switch( dir[0] ) {
				case 'L':
					x_step = -1;
					y_step = 0;
					break;
				case 'R':
					x_step = 1;
					y_step = 0;
					break;
				case 'U':
					x_step = 0;
					y_step = -1;
					break;
				case 'D':
					x_step = 0;
					y_step = 1;
					break;
			}

			for (var i = 0; i < len; i++) {
				var tp = new Point(last.X + x_step, last.Y + y_step);
				yield return tp;
				last = tp;
			}
		}
	}

	internal readonly record struct Line(Point P1, Point P2)
	{
		public Line(long x1, long y1, long x2, long y2) : this(new Point(x1, y1), new Point(x2, y2)) { }

		public bool Horizontal => P1.X == P2.X;

		public bool Vertical => P1.Y == P2.Y;
	}
}
