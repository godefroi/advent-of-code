namespace AdventOfCode.Year2019.Day10;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new((input) => { var (p1, p2) = Execute(input); return (p1.ToString(), p2); }, typeof(Problem), null);

	public static ((long x, long y, int count), long) Execute(string[] input)
	{
		var part1 = Part1(input);
		var part2 = Part2(input, part1.x, part1.y);

		return (part1, part2);
	}

	public static (long x, long y, int count) Part1(string[] map)
	{
		var asteroids = ParseMap(map);
		var results   = new Dictionary<Point, int>();

		// we're building a station on an asteroid, so check each potential location
		foreach (var potentialLocation in asteroids) {
			var cnt = 0;

			// now, for each asteroid which *isn't* our potential location, we'll check to see if we can see it
			foreach (var target in asteroids.Where(a => a != potentialLocation)) {
				var blocked = false;
				//Console.WriteLine(Distance(asteroid, target));

				// check to see if any asteroids block asteroid from seeing target
				// note that we're not checking the target (it can't block itself) and we're not checking the candidate location
				foreach (var blocker in asteroids.Where(a => a != potentialLocation && a != target)) {
					//if( asteroid.X == 1 && asteroid.Y == 0 && target.X == 4 && target.Y == 3 && blocker.X == 3 && blocker.Y == 2) {
					//	Console.WriteLine("this should block");
					//	Console.WriteLine($"\ta->t: {Distance(asteroid, target)} a->b: {Distance(asteroid, blocker)} b->t: {Distance(blocker, target)}");
					//	Console.WriteLine(Distance(asteroid, target) - Distance(asteroid, blocker) - Distance(blocker, target));
					//}

					if (Blocks(potentialLocation, target, blocker)) {
						//Console.WriteLine($"{potentialLocation.X},{potentialLocation.Y} -> {target.X},{target.Y} blocked by {blocker.X},{blocker.Y}");
						blocked = true;
						break;
					}
				}

				if (!blocked) {
					cnt += 1;
					//Console.WriteLine($"{asteroid.X},{asteroid.Y} sees {target.X},{target.Y}");
				}
			}

			results.Add(potentialLocation, cnt);
		}

		//foreach( var kvp in results )
		//	Console.WriteLine($"{kvp.Key.X},{kvp.Key.Y} {kvp.Value}");

		var best = results.OrderByDescending(i => i.Value).First();

		Console.WriteLine($"{best.Key.X},{best.Key.Y} ({best.Value})");

		return (best.Key.X, best.Key.Y, best.Value);
	}

	public static long Part2(string[] map, long stationX, long stationY)
	{
		var asteroids = ParseMap(map);
		var center    = asteroids.Where(p => p.X == stationX && p.Y == stationY).Single();

		var sects = new[] {
			asteroids.Where(p => p.X == center.X && p.Y < center.Y).ToList(),   // above
			asteroids.Where(p => p.X > center.X && p.Y < center.Y).ToList(),    // northeast
			asteroids.Where(p => p.X > center.X && p.Y == center.Y).ToList(),   // right
			asteroids.Where(p => p.X > center.X && p.Y > center.Y).ToList(),    // southeast
			asteroids.Where(p => p.X == center.X && p.Y > center.Y).ToList(),   // below
			asteroids.Where(p => p.X < center.X && p.Y > center.Y).ToList(),    // southwest
			asteroids.Where(p => p.X < center.X && p.Y == center.Y).ToList(),   // left
			asteroids.Where(p => p.X < center.X && p.Y < center.Y).ToList(),    // northwest
		};

		var cnt = 0;

		while (true) {
			for (var i = 0; i < sects.Length; i++) {
				if (sects[i].Count == 0) {
					continue;
				}

				if (i % 2 == 0) {
					// no slope to calculate, just find lowest distance
					var to_kill = sects[i].OrderBy(p => Distance(center, p)).First();

					//Console.WriteLine($"s{i} Killing {to_kill.X},{to_kill.Y} with slope {Slope(center, to_kill)}");
					sects[i].Remove(to_kill);

					if (++cnt == 200) {
						Console.WriteLine($"{to_kill.X},{to_kill.Y} -> {(to_kill.X * 100) + to_kill.Y}");
						return (to_kill.X * 100) + to_kill.Y;
					}
				} else {
					// group by slopes, order, and take out nearest for each slope
					var groups  = sects[i].GroupBy(p => Slope(center, p));
					var ogroups = groups.OrderBy(g => g.Key);
					var to_kill = new List<Point>();

					foreach (var g in ogroups) {
						//Console.WriteLine($"s{i} group for slope {g.Key}:");

						//foreach (var p in g) {
						//	Console.WriteLine($"s{i} \t{p.X},{p.Y} (dist {Distance(center, p)})");
						//}

						to_kill.Add(g.OrderBy(p => Distance(center, p)).First());
					}

					foreach (var p in to_kill) {
						//Console.WriteLine($"s{i} Killing {p.X},{p.Y} with slope {Slope(center, p)}");
						sects[i].Remove(p);

						if (++cnt == 200) {
							Console.WriteLine($"{p.X},{p.Y} -> {(p.X * 100) + p.Y}");
							return (p.X * 100) + p.Y;
						}
					}
				}
			}
		}
	}

	private static double Distance(Point p1, Point p2) => Math.Round(Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2)), 6);

	private static bool Blocks(Point from, Point to, Point blocker) => Math.Round(Math.Abs(Distance(from, to) - Distance(from, blocker) - Distance(blocker, to)), 5) == 0;

	private static double Slope(Point p1, Point p2) => Math.Round(((double)p2.Y - (double)p1.Y) / ((double)p2.X - (double)p1.X), 6);

	private static List<Point> ParseMap(string[] map)
	{
		var asteroids = new List<Point>();
		var y         = 0;

		foreach (var line in map) {
			for (var x = 0; x < line.Length; x++) {
				if (line[x] == '#') {
					asteroids.Add(new Point(x, y));
				}
			}

			y++;
		}

		return asteroids;
	}

	[Test]
	[Arguments("inputSample1.txt", 3, 4, 8)]
	[Arguments("inputSample2.txt", 5, 8, 33)]
	[Arguments("inputSample3.txt", 1, 2, 35)]
	[Arguments("inputSample4.txt", 6, 3, 41)]
	[Arguments("inputSample5.txt", 11, 13, 210)]
	public async Task Part1CalculatesCorrectly(string fileName, long expectedX, long expectedY, int expectedCount)
	{
		var (x, y, count) = Part1(ReadFileLines(fileName));

		await Assert.That(x).IsEqualTo(expectedX);
		await Assert.That(y).IsEqualTo(expectedY);
		await Assert.That(count).IsEqualTo(expectedCount);
	}
}
