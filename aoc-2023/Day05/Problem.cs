namespace AdventOfCode.Year2023.Day05;

public partial class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		var maps  = new List<RangeSet>();
		var maps2 = new List<Map>();
		var seeds = new List<long>();

		RangeSet? currentSet = null;
		Map?      currentMap = null;

		foreach (var line in input) {
			// skip blank lines
			if (line.Length == 0) {
				continue;
			}

			// parse out seeds
			if (line.StartsWith("seeds: ")) {
				seeds.AddRange(line[7..].Split(' ').Select(long.Parse));
				continue;
			}

			// make a new map when they start
			if (line.EndsWith("map:")) {
				currentSet = new RangeSet();
				maps.Add(currentSet);

				currentMap = new Map();
				maps2.Add(currentMap);

				continue;
			}

			// otherwise, add a range to the current map
			var setData = line.Split(' ').Select(long.Parse).ToArray();
			currentSet!.Add(setData[0], setData[1], setData[2]);

			currentMap!.Add(line);
		}

		// build the seed ranges for part 2
		var seedRanges = new Queue<Interval>();

		for (var i = 0; i < seeds.Count; i += 2) {
			seedRanges.Enqueue(new Interval(seeds[i], seeds[i + 1]));
		}

		// foreach (var range in seedRanges) {
		// 	Console.WriteLine(range);
		// }
		// Console.WriteLine();

		foreach (var map in maps2) {
			seedRanges = map.Intersect(seedRanges);

			// foreach (var range in seedRanges) {
			// 	Console.WriteLine(range);
			// }
			// Console.WriteLine();
		}

		return (seeds.Min(s => MapSeedToLocation(s, maps)), seedRanges.Min(r => r.First));
	}

	private static long MapSeedToLocation(long seed, List<RangeSet> maps)
	{
		foreach (var map in maps) {
			seed = map.Map(seed);
		}

		return seed;
	}

	private static IEnumerable<long> ExpandSeedRanges(List<long> seeds)
	{
		var i = 0;

		while (i < seeds.Count - 2) {
			var rangeStart  = seeds[i];
			var rangeLength = seeds[i + 1];
			var curSeed     = rangeStart;

			while (curSeed < rangeStart + rangeLength) {
				yield return curSeed++;
			}

			i += 2;
		}
	}
}

public class RangeSet()
{
	private readonly List<MappedRange> _ranges = [];

	public void Add(long start, long length, long offset) => _ranges.Add(new MappedRange(start, length, offset));

	public long Map(long from)
	{
		var range = _ranges.SingleOrDefault(r => r.Contains(from));

		if (range == null) {
			return from;
		} else {
			return range.Map(from);
		}
	}

	public record class MappedRange
	{
		private readonly long _offset;
		private readonly long _sourceEnd;

		public long DestinationStart { get; private set; }

		public long SourceStart { get; private set; }

		public long Length { get; private set; }

		public MappedRange(long destStart, long sourceStart, long length)
		{
			DestinationStart = destStart;
			SourceStart      = sourceStart;
			Length           = length;
			_offset          = destStart - sourceStart;
			_sourceEnd       = sourceStart + length;
		}

		public bool Contains(long sourceValue) => sourceValue >= SourceStart && sourceValue < _sourceEnd;

		public long Map(long sourceValue) => Contains(sourceValue) ? sourceValue + _offset : sourceValue;
	}

	public class Tests
	{
		[Fact]
		public void MappedRangeMapsCorrectly()
		{
			var mappedRange = new MappedRange(50, 98, 2);

			Assert.Equal( 97, mappedRange.Map( 97));
			Assert.Equal( 50, mappedRange.Map( 98));
			Assert.Equal( 51, mappedRange.Map( 99));
			Assert.Equal(100, mappedRange.Map(100));
		}

		[Fact]
		public void RangeSetMapsCorrectly()
		{
			var rangeSet = new RangeSet();

			rangeSet.Add(50, 98, 2);
			rangeSet.Add(52, 50, 48);

			Assert.Equal(  0, rangeSet.Map(  0));
			Assert.Equal(  1, rangeSet.Map(  1));
			// ...
			Assert.Equal( 48, rangeSet.Map( 48));
			Assert.Equal( 49, rangeSet.Map( 49));
			Assert.Equal( 52, rangeSet.Map( 50));
			Assert.Equal( 53, rangeSet.Map( 51));
			// ...
			Assert.Equal( 98, rangeSet.Map( 96));
			Assert.Equal( 99, rangeSet.Map( 97));
			Assert.Equal( 50, rangeSet.Map( 98));
			Assert.Equal( 51, rangeSet.Map( 99));
			Assert.Equal(100, rangeSet.Map(100));
		}
	}
}

public readonly record struct Interval
{
	public readonly long First;

	public readonly long Length;

	public readonly long Last;

	public Interval(long start, long length)
	{
		First  = start;
		Length = length;
		Last   = start + length - 1;
	}

	public (Interval intersection, Interval left, Interval right) Intersect(Interval other)
	{
		// we will have a maximum of 3 intervals
		// maybe one from before the intersection part,
		// maybe one from inside the intersection part,
		// maybe one from after the intersection part

		//var ret = new List<Interval>(3);

		// we want intersections of "other" which

		Interval intersection = Empty;
		Interval left         = Empty;
		Interval right        = Empty;

		// if the other (seed) one starts before us, there'll be a left part
		if (other.First < First) {
			left = new Interval(other.First, Math.Min(other.Length, First - other.First));
		}

		// if the other (seed) one ends after us, there'll be a right part
		if (other.Last > Last) {
			if (other.First > Last) {
				right = other;
			} else {
				right = new Interval(Last + 1, other.Last - Last);
			}
		}

		// if the other (seed) one ends after the map starts and starts at or before the map end, there'll be an intersection
		if ((other.Last >= First) && (other.First <= Last)) {
			var intersectionFirst  = Math.Max(other.First, First);
			var intersectionLength = Math.Min(other.Last, Last) - intersectionFirst + 1;

			intersection = new Interval(intersectionFirst, intersectionLength);
		}

		return (intersection, left, right);
	}

	public static Interval Empty { get; } = new Interval(-1, -1);
}

public class Map()
{
	private readonly List<(Interval Interval, long Offset)> _intervals = [];

	public void Add(string inputLine)
	{
		// TODO: could be optimized with spans, but probably doesn't matter
		var parts       = inputLine.Split(' ');
		var sourceStart = long.Parse(parts[1]);

		Add(sourceStart, long.Parse(parts[2]), long.Parse(parts[0]) - sourceStart);
	}

	public void Add(long intervalStart, long intervalLength, long offset) =>
		_intervals.Add((new Interval(intervalStart, intervalLength), offset));

	public Queue<Interval> Intersect(Queue<Interval> seeds)
	{
		var ret = new Queue<Interval>(seeds.Count);

		while (seeds.Count > 0) {
			var seed        = seeds.Dequeue();
			var intersected = false;

			foreach (var (interval, offset) in _intervals) {
				var (intersection, left, right) = interval.Intersect(seed);

				if (intersection != Interval.Empty) {
					// this seed intersected with this interval; map the intersection
					// to the output, then if we have left or right parts, they need
					// to be re-intersected with everything else, so bail and we'll
					// start over

					intersected = true;

					ret.Enqueue(new Interval(intersection.First + offset, intersection.Length));

					if (left != Interval.Empty) {
						seeds.Enqueue(left);
					}

					if (right != Interval.Empty) {
						seeds.Enqueue(right);
					}

					break;
				}
			}

			if (!intersected) {
				// if no intersections were found, we pass this interval to the output unmodified
				ret.Enqueue(seed);
			}
		}

		return ret;
	}
}

public class MapTests
{
	[Fact]
	public void MapWorksCorrectlyWithSingleInterval()
	{
		var map = new Map();

		map.Add(98, 2, 50 - 98);

		var seeds = new Queue<Interval>();

		seeds.Enqueue(new Interval(96, 6));

		var mapped = map.Intersect(seeds).OrderBy(i => i.First).ToArray();

		Assert.Collection(mapped,
			i => { Assert.Equal(50, i.First); Assert.Equal(2, i.Length); },
			i => { Assert.Equal(96, i.First); Assert.Equal(2, i.Length); },
			i => { Assert.Equal(100, i.First); Assert.Equal(2, i.Length); });
	}

	[Fact]
	public void MapWorksCorrectlyWithMultipleIntervals()
	{
		var map = new Map();

		map.Add(98,  2, 50 - 98);
		map.Add(50, 48, 52 - 50);

		var seeds = new Queue<Interval>();

		seeds.Enqueue(new Interval(0, 100));

		var mapped = map.Intersect(seeds).OrderBy(i => i.First).ToArray();

		Assert.Collection(mapped,
			i => { Assert.Equal( 0, i.First); Assert.Equal(49, i.Last); },
			i => { Assert.Equal(50, i.First); Assert.Equal(51, i.Last); },
			i => { Assert.Equal(52, i.First); Assert.Equal(99, i.Last); });
	}

	[Fact]
	public void SampleSeedToSoilMapWorksCorrectly()
	{
		var map = new Map();

		map.Add("50 98 2");
		map.Add("52 50 48");

		var seeds = new Queue<Interval>();

		seeds.Enqueue(new Interval(79, 14));
		//seeds.Enqueue(new Interval(55, 13));

		var mapped = map.Intersect(seeds).OrderBy(i => i.First).ToArray();

		// 79 -> 81
		// 80 -> 82
		// ...
		// 91 -> 93
		// 92 -> 94

		// the 79,14 interval should be mapped into first=81 length=14
		Assert.Collection(mapped,
			i => { Assert.Equal(81, i.First); Assert.Equal(14, i.Length); });
	}
}

public class IntervalTests
{
	[Fact]
	public void IntervalConstructsCorrectly()
	{
		var i1 = new Interval(10, 5); // seed goes 10-14

		Assert.Equal(10, i1.First);
		Assert.Equal( 5, i1.Length);
		Assert.Equal(14, i1.Last);
	}

	[Theory]
	[InlineData(10, 5, 16, 2, 10,  5, -1, -1, -1, -1)] // seed goes 10-14, map range is 16-17    seed range is fully left of map range
	[InlineData(18, 5, 16, 2, -1, -1, -1, -1, 18,  5)] // seed goes 18-22, map range is 16-17    seed range is fully right of map range
	[InlineData(30, 5, 32, 8, 30,  2, 32,  3, -1, -1)] // seed goes 30-34, map range is 32-39    seed range overlaps the left edge of map range
	[InlineData(40, 5, 35, 8, -1, -1, 40,  3, 43,  2)] // seed goes 40-44, map range is 35-42    seed range overlaps the right edge of map range
	[InlineData(12, 2, 12, 2, -1, -1, 12,  2, -1, -1)] // seed goes 12-13, map range is 12-13    seed range fully matches the map range
	[InlineData(10, 8, 12, 3, 10,  2, 12,  3, 15,  3)] // seed goes 10-17, map range is 12-14    seed range fully contains the map range
	[InlineData(22, 2, 18, 9, -1, -1, 22,  2, -1, -1)] // seed goes 22-24, map range is 18-26    seed range is fully contained in the map range
	public void IntersectOperatesCorrectly(int seedFirst, int seedLength, int mapFirst, int mapLength, int leftFirst, int leftLength, int intersectionFirst, int intersectionLength, int rightFirst, int rightLength)
	{
		var (ri, rl, rr) = new Interval(mapFirst, mapLength).Intersect(new Interval(seedFirst, seedLength));

		Assert.Equal(leftFirst,          rl.First);
		Assert.Equal(leftLength,         rl.Length);
		Assert.Equal(intersectionFirst,  ri.First);
		Assert.Equal(intersectionLength, ri.Length);
		Assert.Equal(rightFirst,         rr.First);
		Assert.Equal(rightLength,        rr.Length);
	}
}
