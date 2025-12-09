using BenchmarkDotNet.Attributes;

namespace AdventOfCode.Year2025.Day08;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), typeof(Day08Benchmarks));

	public static (long, long) Execute(string[] input)
	{
		Span<Range> ranges = stackalloc Range[3];

		var coords   = new Coordinate3[input.Length];
		var queue    = new PriorityQueue<(int i1, int i2), long>((input.Length * (input.Length - 1)) / 2);
		var circuits = new List<HashSet<Coordinate3>>();
		var p1       = 0L;
		var p2       = 0L;

		// parse all the coordinates
		for (var i = 0; i < input.Length; i++) {
			input[i].Split(ranges, ',');
			coords[i] = new Coordinate3(long.Parse(input[i][ranges[0]]), long.Parse(input[i][ranges[1]]), long.Parse(input[i][ranges[2]]));
		}

		GenerateCombinations(coords, 2, (ca, indices, _) => {
			var c1   = ca[indices[0]];
			var c2   = ca[indices[1]];
			var dist = ((c1.X - c2.X) * (c1.X - c2.X)) + ((c1.Y - c2.Y) * (c1.Y - c2.Y)) + ((c1.Z - c2.Z) * (c1.Z - c2.Z));
			queue.Enqueue((indices[0], indices[1]), dist);
		});

		for (var i = 0; queue.Count > 0; i++) {
			var (i1, i2) = queue.Dequeue();
			var c1       = coords[i1];
			var c2       = coords[i2];

			// add c1 and c2 to a circuit
			var circuit1 = circuits.SingleOrDefault(c => c.Contains(c1)); // PERF: this could be FirstOrDefault() once we know this works
			var circuit2 = circuits.SingleOrDefault(c => c.Contains(c2)); // PERF: this could be FirstOrDefault() once we know this works

			if (circuit1 != null && circuit2 != null) {
				if (circuit1 != circuit2) {
					// both these boxes are already in *different* circuits; join them
					foreach (var c2box in circuit2) {
						circuit1.Add(c2box);
					}

					circuits.Remove(circuit2);
				}
			} else if (circuit1 != null) {
				// c1 is in a circuit, c2 is not; add c2 to c1's circuit
				circuit1.Add(c2);
			} else if (circuit2 != null) {
				// c2 is in a circuit, c1 is not; add c1 to c2's circuit
				circuit2.Add(c1);
			} else {
				// neither is in a circuit; add a new circuit with both boxes in it
				circuits.Add([c1, c2]);
			}

			if (circuits.Count == 1 && circuits[0].Count == coords.Length) {
				p2 = c1.X * c2.X;
				break;
			}

			// calculate part 1 after 1000 connections
			if (i == 1000) {
				p1 = circuits.Select(c => c.Count).OrderDescending().Take(3).Aggregate(1L, (t, c) => c * t);
			}
		}

		return (p1, p2);
	}

	public static (long, long) ExecutePart1Only(string[] input)
	{
		var closestCount = input.Length == 20 ? 10 : 1000;
		var closest      = new SortedSet<(Coordinate3 C1, Coordinate3 C2, double Distance)>(Comparer<(Coordinate3 C1, Coordinate3 C2, double Distance)>.Create((i1, i2) => i1.Distance.CompareTo(i2.Distance)));
		var max          = (C1: Coordinate3.Empty, C2: Coordinate3.Empty, Distance: double.MaxValue);

		closest.Add(max);

		Span<Range>       ranges = stackalloc Range[3];
		Span<Coordinate3> coords = stackalloc Coordinate3[input.Length];

		// parse all the coordinates
		for (var i = 0; i < input.Length; i++) {
			input[i].Split(ranges, ',');
			coords[i] = new Coordinate3(long.Parse(input[i][ranges[0]]), long.Parse(input[i][ranges[1]]), long.Parse(input[i][ranges[2]]));
		}

		// find the N shortest distances
		GenerateCombinations(coords, 2, (ca, indices, _) => {
			var c1     = ca[indices[0]];
			var c2     = ca[indices[1]];
			var dist   = Coordinate3.EuclideanDistance(c1, c2);
			var newMax = false;

			if (dist < max.Distance || closest.Count < closestCount) {
				if (closest.Count >= closestCount) {
					closest.Remove(max);
					newMax = true;
				}

				var newDist = (c1, c2, dist);

				closest.Add(newDist);

				if (newMax) {
					max = closest.Max;
				}
			}
		});

		var circuits = new List<HashSet<Coordinate3>>();

		foreach (var (c1, c2, _) in closest) {
			// add c1 and c2 to a circuit
			var circuit1 = circuits.SingleOrDefault(c => c.Contains(c1)); // PERF: this could be FirstOrDefault() once we know this works
			var circuit2 = circuits.SingleOrDefault(c => c.Contains(c2)); // PERF: this could be FirstOrDefault() once we know this works

			if (circuit1 != null && circuit2 != null) {
				if (circuit1 != circuit2) {
					// both these boxes are already in *different* circuits; join them
					foreach (var c2box in circuit2) {
						circuit1.Add(c2box);
					}

					circuits.Remove(circuit2);
				}
			} else if (circuit1 != null) {
				// c1 is in a circuit, c2 is not; add c2 to c1's circuit
				circuit1.Add(c2);
			} else if (circuit2 != null) {
				// c2 is in a circuit, c1 is not; add c1 to c2's circuit
				circuit2.Add(c1);
			} else {
				// neither is in a circuit; add a new circuit with both boxes in it
				circuits.Add([c1, c2]);
			}
		}

		var p1 = circuits.Select(c => c.Count).OrderDescending().Take(3).Aggregate(1L, (t, c) => c * t);

		return (p1, 0);
	}

	public class Day08Tests
	{
		private readonly string[] _sampleData = """
			162,817,812
			57,618,57
			906,360,560
			592,479,940
			352,342,300
			466,668,158
			542,29,236
			431,825,988
			739,650,466
			52,470,668
			216,146,977
			819,987,18
			117,168,530
			805,96,715
			346,949,466
			970,615,88
			941,993,340
			862,61,35
			984,92,344
			425,690,689
			""".Split("\r\n");

		[Test]
		public async Task Part1Works()
		{
			await Assert.That(Execute(_sampleData).Item1).IsEqualTo(40);
		}
	}

	public class Day08Benchmarks
	{
		private string[] _lines = [];

		[GlobalSetup]
		public void BenchmarkSetup()
		{
			_lines = ReadFileLines("input.txt");
		}

		[Benchmark]
		public void ListThenSort()
		{
			Span<Range>       ranges = stackalloc Range[3];
			//Span<Coordinate3> coords = stackalloc Coordinate3[input.Length];
			var coords = new Coordinate3[_lines.Length];

			// parse all the coordinates
			for (var i = 0; i < _lines.Length; i++) {
				_lines[i].Split(ranges, ',');
				coords[i] = new Coordinate3(long.Parse(_lines[i][ranges[0]]), long.Parse(_lines[i][ranges[1]]), long.Parse(_lines[i][ranges[2]]));
			}

			// var dists = new List<(Coordinate3 C1, Coordinate3 C2, double Distance)>();
			// GenerateCombinations(coords, 2, (ca, indices, _) => {
			// 	dists.Add((C1: ca[indices[0]], C2: ca[indices[1]], Distance: Coordinate3.EuclideanDistance(ca[indices[0]], ca[indices[1]])));
			// });

			var dists = EnumerateCombinations2(coords)
				.Select(t => (C1: t.Item1, C2: t.Item2, Distance: Coordinate3.EuclideanDistance(t.Item1, t.Item2)))
				.ToList();

			dists.Sort((i1, i2) => i1.Distance.CompareTo(i2.Distance));
		}

		[Benchmark]
		public void PriorityQueueTwiddleED()
		{
			Span<Range> ranges = stackalloc Range[3];
			var coords = new Coordinate3[_lines.Length];

			for (var i = 0; i < _lines.Length; i++) {
				_lines[i].Split(ranges, ',');
				coords[i] = new Coordinate3(long.Parse(_lines[i][ranges[0]]), long.Parse(_lines[i][ranges[1]]), long.Parse(_lines[i][ranges[2]]));
			}

			var pq = new PriorityQueue<(int C1, int C2), double>((_lines.Length * (_lines.Length - 1)) / 2);

			GenerateCombinations(coords, 2, (ca, indices, _) => {
				//dists.Add((C1: ca[indices[0]], C2: ca[indices[1]], Distance: Coordinate3.EuclideanDistance(ca[indices[0]], ca[indices[1]])));
				//var c1 = ca[indices[0]];
				//var c2 = ca[indices[1]];
				pq.Enqueue((indices[0], indices[1]), Coordinate3.EuclideanDistance(ca[indices[0]], ca[indices[1]]));
			});
		}

		[Benchmark]
		public void PriorityQueueTwiddleSD()
		{
			Span<Range> ranges = stackalloc Range[3];
			var coords = new Coordinate3[_lines.Length];

			for (var i = 0; i < _lines.Length; i++) {
				_lines[i].Split(ranges, ',');
				coords[i] = new Coordinate3(long.Parse(_lines[i][ranges[0]]), long.Parse(_lines[i][ranges[1]]), long.Parse(_lines[i][ranges[2]]));
			}

			var pq = new PriorityQueue<(int C1, int C2), long>((_lines.Length * (_lines.Length - 1)) / 2);

			GenerateCombinations(coords, 2, (ca, indices, _) => {
				var c1   = ca[indices[0]];
				var c2   = ca[indices[1]];
				var dist = ((c1.X - c2.X) * (c1.X - c2.X)) + ((c1.Y - c2.Y) * (c1.Y - c2.Y)) + ((c1.Z - c2.Z) * (c1.Z - c2.Z));
				pq.Enqueue((indices[0], indices[1]), dist);
			});
		}

		[Benchmark]
		public void PriorityQueueTwiddleIM()
		{
			Span<Range> ranges = stackalloc Range[3];
			var coords = new Coordinate3[_lines.Length];

			for (var i = 0; i < _lines.Length; i++) {
				_lines[i].Split(ranges, ',');
				coords[i] = new Coordinate3(long.Parse(_lines[i][ranges[0]]), long.Parse(_lines[i][ranges[1]]), long.Parse(_lines[i][ranges[2]]));
			}

			var pq = new PriorityQueue<(int C1, int C2), double>((_lines.Length * (_lines.Length - 1)) / 2);

			GenerateCombinations(coords, 2, (ca, indices, _) => {
				var c1 = ca[indices[0]];
				var c2 = ca[indices[1]];
				var dx = c1.X - c2.X;
				var dy = c1.Y - c2.Y;
				var dz = c1.Z - c2.Z;
				var dist = Math.Sqrt((dx * dx) + (dy * dy) + (dz * dz));
				pq.Enqueue((indices[0], indices[1]), dist);
			});
		}

		[Benchmark]
		public void PriorityQueuePairwise()
		{
			Span<Range> ranges = stackalloc Range[3];
			var coords = new Coordinate3[_lines.Length];

			for (var i = 0; i < _lines.Length; i++) {
				_lines[i].Split(ranges, ',');
				coords[i] = new Coordinate3(long.Parse(_lines[i][ranges[0]]), long.Parse(_lines[i][ranges[1]]), long.Parse(_lines[i][ranges[2]]));
			}

			var pq = new PriorityQueue<(int C1, int C2), double>((_lines.Length * (_lines.Length - 1)) / 2);

			for (var i = 0; i < coords.Length; i++) {
				for (var j = 0; j < i; j++) {
					var c1 = coords[i];
					var c2 = coords[j];
					pq.Enqueue((i, j), Coordinate3.EuclideanDistance(c1, c2));
				}
			}
		}
	}
}
