using BenchmarkDotNet.Attributes;

namespace AdventOfCode.Year2025.Day09;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), typeof(Day09Benchmarks));

	public static (long, long) Execute(string[] input)
	{
		var coords = new Coordinate[input.Length];
		var lines  = new (Coordinate From, Coordinate To)[input.Length];

		var p1 = 0L;
		var p2 = 0L;

		for (var i = 0; i < input.Length; i++) {
			coords[i] = Coordinate.Parse(input[i]);
		}

		for (var i = 0; i < input.Length; i++) {
			var c1         = coords[i + 0];
			var c2         = i == input.Length - 1 ? coords[0] : coords[i + 1];
			var leftEdge   = c1.X < c2.X ? c1.X : c2.X;
			var rightEdge  = c1.X > c2.X ? c1.X : c2.X;
			var topEdge    = c1.Y < c2.Y ? c1.Y : c2.Y;
			var bottomEdge = c1.Y > c2.Y ? c1.Y : c2.Y;

			lines[i] = (new(leftEdge, topEdge), new(rightEdge, bottomEdge));
		}

		foreach (var combo in coords.EnumerateCombinations(2)) {
			var c1         = combo[0];
			var c2         = combo[1];
			var leftEdge   = c1.X < c2.X ? c1.X : c2.X;
			var rightEdge  = c1.X > c2.X ? c1.X : c2.X;
			var topEdge    = c1.Y < c2.Y ? c1.Y : c2.Y;
			var bottomEdge = c1.Y > c2.Y ? c1.Y : c2.Y;
			var area       = (rightEdge - leftEdge + 1L) * (bottomEdge - topEdge + 1L);
			var found      = false;

			if (area > p1) {
				p1 = area;
			}

			foreach (var (from, to) in lines) {
				if (from.X < rightEdge && from.Y < bottomEdge && to.X > leftEdge && to.Y > topEdge) {
					found = true;
					break;
				}
			}

			if (!found && area > p2) {
				p2 = area;
			}
		}

		return (p1, p2);
	}

	public class Day09Tests
	{
		private readonly string[] _sampleInput = """
			7,1
			11,1
			11,7
			9,7
			9,5
			2,5
			2,3
			7,3
			""".Split("\r\n");

		[Test]
		public async Task Part1Works()
		{
			await Assert.That(Execute(_sampleInput).Item1).IsEqualTo(50);
		}

		[Test]
		public async Task Part2Works()
		{
			await Assert.That(Execute(_sampleInput).Item2).IsEqualTo(24);
		}
	}

	public class Day09Benchmarks
	{
		private Coordinate[] _coords = [];
		private (Coordinate From, Coordinate To)[] _lines = [];

		[GlobalSetup]
		public void BenchmarkSetup()
		{
			var input  = ReadFileLines("input.txt");

			_coords = new Coordinate[input.Length];
			_lines  = new (Coordinate From, Coordinate To)[input.Length];

			for (var i = 0; i < input.Length; i++) {
				_coords[i] = Coordinate.Parse(input[i]);
			}

			for (var i = 0; i < input.Length; i++) {
				var c1         = _coords[i + 0];
				var c2         = i == input.Length - 1 ? _coords[0] : _coords[i + 1];
				var leftEdge   = c1.X < c2.X ? c1.X : c2.X;
				var rightEdge  = c1.X > c2.X ? c1.X : c2.X;
				var topEdge    = c1.Y < c2.Y ? c1.Y : c2.Y;
				var bottomEdge = c1.Y > c2.Y ? c1.Y : c2.Y;

				_lines[i] = (new(leftEdge, topEdge), new(rightEdge, bottomEdge));
			}
		}

		[Benchmark]
		public void Enumerator()
		{
			var p1 = 0L;
			var p2 = 0L;

			foreach (var combo in _coords.EnumerateCombinations(2)) {
				var c1         = combo[0];
				var c2         = combo[1];
				var leftEdge   = c1.X < c2.X ? c1.X : c2.X;
				var rightEdge  = c1.X > c2.X ? c1.X : c2.X;
				var topEdge    = c1.Y < c2.Y ? c1.Y : c2.Y;
				var bottomEdge = c1.Y > c2.Y ? c1.Y : c2.Y;
				var area       = (rightEdge - leftEdge + 1L) * (bottomEdge - topEdge + 1L);
				var found      = false;

				if (area > p1) {
					p1 = area;
				}

				foreach (var (from, to) in _lines) {
					if (from.X < rightEdge && from.Y < bottomEdge && to.X > leftEdge && to.Y > topEdge) {
						found = true;
						break;
					}
				}

				if (!found && area > p2) {
					p2 = area;
				}
			}
		}

		[Benchmark]
		public void Generator()
		{
			var p1 = 0L;
			var p2 = 0L;

			GenerateCombinations(_coords, 2, (cs, indices, _) => {
				var c1         = cs[indices[0]];
				var c2         = cs[indices[1]];
				var leftEdge   = c1.X < c2.X ? c1.X : c2.X;
				var rightEdge  = c1.X > c2.X ? c1.X : c2.X;
				var topEdge    = c1.Y < c2.Y ? c1.Y : c2.Y;
				var bottomEdge = c1.Y > c2.Y ? c1.Y : c2.Y;
				var area       = (rightEdge - leftEdge + 1L) * (bottomEdge - topEdge + 1L);
				var found      = false;

				if (area > p1) {
					p1 = area;
				}

				foreach (var (from, to) in _lines) {
					if (from.X < rightEdge && from.Y < bottomEdge && to.X > leftEdge && to.Y > topEdge) {
						found = true;
						break;
					}
				}

				if (!found && area > p2) {
					p2 = area;
				}
			});
		}
	}
}
