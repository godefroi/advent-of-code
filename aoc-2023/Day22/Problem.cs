namespace AdventOfCode.Year2023.Day22;

public partial class Problem
{
	public static ProblemMetadata2 Metadata { get; } = new(Execute, null);

	private readonly static char[] _splitChars = [',', '~'];

	public static (long, long) Execute(string[] input)
	{
		Span<Range> ranges = stackalloc Range[7];

		var bricks = new List<Brick>(input.Length);

		// parse out the bricks
		for (var i = 0; i < input.Length; i++) {
			var lineSpan   = input[i].AsSpan();
			var rangeCount = lineSpan.SplitAny(ranges, _splitChars);

			if (rangeCount != 6) {
				throw new InvalidOperationException("Bad input data");
			}

			var from = Coordinate3.Parse(lineSpan, ranges[..3]);
			var to   = Coordinate3.Parse(lineSpan, ranges[3..6]);

			bricks.Add(new Brick(from, to));
		}

		// Console.WriteLine(bricks[6]);
		// Console.WriteLine(bricks[5]);
		// Console.WriteLine(bricks[6].Overlaps(bricks[5]));
		// Console.WriteLine(bricks[5].Overlaps(bricks[6]));
		// return (0,0);

		// sort the bricks such that the bricks closest to the ground
		// come first in the list
		bricks.Sort();

		// foreach (var b in bricks) {
		// 	Console.WriteLine(b);
		// }
		// Console.WriteLine();

		// settle the bricks to the ground
		for (var i = 0; i < bricks.Count; i++) {
			var settled = false;

			// for each brick, find the highest-z brick that overlaps this
			// brick, then settle this brick on top of that brick
			for (var j = i - 1; j >= 0; j--) {
				if (bricks[i].OverlapsXY(bricks[j])) {
					//Console.WriteLine($"{i} {bricks[i]} overlaps {j} {bricks[j]}");
					bricks[i] = new Brick(
						new Coordinate3(bricks[i].From.X, bricks[i].From.Y, bricks[j].MaxZ + 1),
						new Coordinate3(bricks[i].To.X, bricks[i].To.Y, bricks[j].MaxZ + bricks[i].SizeZ));

					settled = true;

					break;
				}
			}

			if (!settled) {
				// didn't overlap any brick; settle it all the way to the ground
				bricks[i] = new Brick(
					new Coordinate3(bricks[i].From.X, bricks[i].From.Y, 1),
					new Coordinate3(bricks[i].To.X, bricks[i].To.Y, bricks[i].SizeZ));
			}
		}

		// Console.WriteLine();
		// foreach (var b in bricks) {
		// 	Console.WriteLine(b);
		// }

		for (var i = 0; i < bricks.Count; i++) {
			var aboveBricks = bricks.Where(cb => cb.MinZ == bricks[i].MaxZ + 1).ToList();

			Console.WriteLine($"brick at {i} (Z {bricks[i].MinZ}-{bricks[i].MaxZ}) has {aboveBricks.Count} bricks in the Z above it");
		}

		// count which bricks can be disintegrated
		Console.WriteLine(bricks.Count(b => bricks.Where(cb => cb.MinZ == b.MaxZ + 1).Any(cb => cb.OverlapsXY(b))));

		return (0, 0);
	}

	private readonly record struct Brick(Coordinate3 From, Coordinate3 To) : IComparable<Brick>
	{
		public int MaxZ => Math.Max(From.Z, To.Z);

		public int MinZ => Math.Min(From.Z, To.Z);

		public int SizeZ => MaxZ - MinZ + 1;

		public int CompareTo(Brick other) => MinZ.CompareTo(other.MinZ);

		public bool OverlapsXY(Brick other)
		{
			var thisMinX  = Math.Min(From.X, To.X);
			var thisMaxX  = Math.Max(From.X, To.X);
			var thisMinY  = Math.Min(From.Y, To.Y);
			var thisMaxY  = Math.Max(From.Y, To.Y);
			var otherMinX = Math.Min(other.From.X, other.To.X);
			var otherMaxX = Math.Max(other.From.X, other.To.X);
			var otherMinY = Math.Min(other.From.Y, other.To.Y);
			var otherMaxY = Math.Max(other.From.Y, other.To.Y);

			// check whether this brick overlaps other in the X-Y plane
			return
				thisMinX <= otherMaxX && otherMinX <= thisMaxX &&
				thisMinY <= otherMaxY && otherMinY <= thisMaxY;
		}
	}
}
