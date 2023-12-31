namespace AdventOfCode.Year2023.Day22;

public partial class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, null);

	private readonly static char[] _splitChars = [',', '~'];

	public static (long, long) Execute(string[] input)
	{
		Span<Range> ranges = stackalloc Range[7];

		var bricks = new List<Brick>(input.Length);
		var name   = 'A';

		// parse out the bricks
		for (var i = 0; i < input.Length; i++) {
			var lineSpan   = input[i].AsSpan();
			var rangeCount = lineSpan.SplitAny(ranges, _splitChars);

			if (rangeCount != 6) {
				throw new InvalidOperationException("Bad input data");
			}

			var from = Coordinate3.Parse(lineSpan, ranges[..3]);
			var to   = Coordinate3.Parse(lineSpan, ranges[3..6]);

			bricks.Add(new Brick(from, to, input.Length <= 26 ? name++ : ' '));
		}

		var overlaps = bricks.ToDictionary(brick => brick, brick => bricks.Where(b => b != brick && OverlapsXY(brick, b)).ToList());

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
			var toSettle = bricks[i];

			// for each brick, find the highest-z brick that overlaps this
			// brick, then settle this brick on top of that brick
			var onto = overlaps[toSettle]             // out of the bricks that overlap with this brick
				.Where(b => b.MaxZ < toSettle.MinZ)   // that are at a lower Z level than this brick
				.MaxBy(b => b.MaxZ);                  // take the one at the highest Z level

			// if we have a brick to settle onto, do that; otherwise, settle to Z=0
			if (onto != null) {
				toSettle.Settle(onto);
			} else {
				toSettle.Settle(0);
			}
		}

		// Console.WriteLine();
		// foreach (var b in bricks) {
		// 	Console.WriteLine(b);
		// }

		// for (var i = 0; i < bricks.Count; i++) {
		// 	var aboveBricks = bricks.Where(cb => cb.MinZ == bricks[i].MaxZ + 1).ToList();

		// 	Console.WriteLine($"brick {bricks[i].Name} at {i} (Z {bricks[i].MinZ}-{bricks[i].MaxZ}) has {aboveBricks.Count} bricks in the Z above it");
		// }

		// supports is the bricks that overlap this brick, and are in the Z layer
		// immediately above; that is to say, their MinZ is our (MaxZ + 1)
		var supports = bricks.ToDictionary(b => b,
			b => overlaps[b]                        // for this brick, consider all the bricks that overlap
				.Where(o => o.MinZ == (b.MaxZ + 1)) // that are in the Z level immediately above our top
				.ToList());                         // make a list out of it

		// supportedBy is the bricks that overlap this brick, and are in the Z
		// layer immediately BELOW
		var supportedBy = bricks.ToDictionary(b => b,
			b => overlaps[b]
				.Where(o => o.MaxZ == (b.MinZ - 1))
				.ToList());

		// foreach (var (brick, theList) in supports) {
		// 	if (theList.Count == 0) {
		// 		Console.WriteLine($"brick {brick.Name} supports no bricks");
		// 	} else {
		// 		Console.WriteLine($"brick {brick.Name} supports bricks {string.Join(',', theList.Select(s => s.Name))}");
		// 	}
		// }
		// Console.WriteLine();

		// foreach (var (brick, theList) in supportedBy) {
		// 	if (theList.Count == 0) {
		// 		Console.WriteLine($"brick {brick.Name} is supported by no bricks");
		// 	} else {
		// 		Console.WriteLine($"brick {brick.Name} is supported by bricks {string.Join(',', theList.Select(s => s.Name))}");
		// 	}
		// }
		// Console.WriteLine();

		// count which bricks can be disintegrated; a brick could be disintegrated
		// if all the bricks it supports are supported by more than one brick
		var part1 = bricks.Count(b => supports[b].All(s => supportedBy[s].Count > 1));

		// figure out how many bricks fall when we start blowing things up
		var part2 = bricks.Sum(b => CountFalling(b, supports, supportedBy));
		// foreach (var b in bricks) {
		// 	Console.WriteLine($"{b.Name} => {CountFalling(b, supports, supportedBy)}");
		// }

		return (part1, part2);
	}

	private static int CountFalling(Brick brick, Dictionary<Brick, List<Brick>> supports, Dictionary<Brick, List<Brick>> supportedBy)
	{
		HashSet<Brick> fallen = [];

		CountFalling(brick, supports, supportedBy, fallen);

		return fallen.Count;
	}

	private static void CountFalling(Brick brick, Dictionary<Brick, List<Brick>> supports, Dictionary<Brick, List<Brick>> supportedBy, HashSet<Brick> fallen)
	{
		// the bricks that fall are the bricks supported by this brick
		// that no other (non-fallen) bricks support
		var toFall = supports[brick].Where(s => !supportedBy[s].Except(fallen.Append(brick)).Any()).ToList();

		//Console.WriteLine($"toFall for brick {brick.Name} is {toFall.Count} ({string.Join(',', toFall.Select(b => b.Name))})");

		// how many bricks fall is ... well, you get it
		foreach (var b in toFall) {
			fallen.Add(b);
		}

		foreach (var b in toFall) {
			CountFalling(b, supports, supportedBy, fallen);
		}
	}

	private static bool OverlapsXY(Brick b1, Brick b2)
	{
		var thisMinX  = Math.Min(b1.From.X, b1.To.X);
		var thisMaxX  = Math.Max(b1.From.X, b1.To.X);
		var thisMinY  = Math.Min(b1.From.Y, b1.To.Y);
		var thisMaxY  = Math.Max(b1.From.Y, b1.To.Y);
		var otherMinX = Math.Min(b2.From.X, b2.To.X);
		var otherMaxX = Math.Max(b2.From.X, b2.To.X);
		var otherMinY = Math.Min(b2.From.Y, b2.To.Y);
		var otherMaxY = Math.Max(b2.From.Y, b2.To.Y);

		// check whether this brick overlaps other in the X-Y plane
		return
			thisMinX <= otherMaxX && otherMinX <= thisMaxX &&
			thisMinY <= otherMaxY && otherMinY <= thisMaxY;
	}

	private class Brick : IComparable<Brick>
	{
		public char Name { get; }

		public Coordinate3 From { get; private set; }

		public Coordinate3 To { get; private set; }

		public long MaxZ => Math.Max(From.Z, To.Z);

		public long MinZ => Math.Min(From.Z, To.Z);

		public long SizeZ => MaxZ - MinZ + 1;

		public Brick(Coordinate3 from, Coordinate3 to, char name = ' ')
		{
			Name = name;
			From = from;
			To   = to;
		}

		public void Settle(long newMinZ)
		{
			var settleDist = MinZ - newMinZ;

			From = new Coordinate3(From.X, From.Y, From.Z - settleDist);
			To   = new Coordinate3(To.X,   To.Y,   To.Z - settleDist);
		}

		public void Settle(Brick onto) => Settle(onto.MaxZ + 1);

		public int CompareTo(Brick? other) => other == null ? -1 : MinZ.CompareTo(other.MinZ);
	}

	public class Tests
	{
		[Fact]
		public void SettleWorks()
		{
			var b = new Brick(new Coordinate3(1, 1, 10), new Coordinate3(1, 1, 12));

			Assert.Equal(10, b.MinZ);
			Assert.Equal(12, b.MaxZ);
			Assert.Equal(3, b.SizeZ);

			b.Settle(5);

			Assert.Equal(5, b.MinZ);
			Assert.Equal(7, b.MaxZ);
			Assert.Equal(3, b.SizeZ);
		}

		[Fact]
		public void SettleOntoWorks()
		{
			var b1 = new Brick(new Coordinate3(1, 1, 10), new Coordinate3(1, 1, 12));
			var b2 = new Brick(new Coordinate3(1, 1, 25), new Coordinate3(1, 1, 27));

			Assert.Equal(10, b1.MinZ);
			Assert.Equal(12, b1.MaxZ);
			Assert.Equal(3, b1.SizeZ);

			Assert.Equal(25, b2.MinZ);
			Assert.Equal(27, b2.MaxZ);
			Assert.Equal(3, b2.SizeZ);

			b2.Settle(b1);

			Assert.Equal(13, b2.MinZ);
			Assert.Equal(15, b2.MaxZ);
			Assert.Equal(3, b2.SizeZ);
		}
	}
}
