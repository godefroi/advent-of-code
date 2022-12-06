using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace aoc_2019
{
	internal static class problem_10
	{
		private static string m_test1 = @".#..#
.....
#####
....#
...##";

		private static string m_test2 = @"......#.#.
#..#.#....
..#######.
.#.#.###..
.#..#.....
..#....#.#
#..#....#.
.##.#..###
##...#..#.
.#....####";

		private static string m_test3 = @"#.#...#.#.
.###....#.
.#....#...
##.#.#.#.#
....#.#.#.
.##..###.#
..#...##..
..##....##
......#...
.####.###.";

		private static string m_test4 = @".#..#..###
####.###.#
....###.#.
..###.##.#
##.##.#.#.
....###..#
..#.#..#.#
#..#.#.###
.##...##.#
.....#.#..";

		private static string m_test5 = @".#..##.###...#######
##.############..##.
.#.######.########.#
.###.#######.####.#.
#####.##.#.##.###.##
..#####..#.#########
####################
#.####....###.#.#.##
##.#################
#####.##.###..####..
..######..##.#######
####.##.####...##..#
.#####..#.######.###
##...#.##########...
#.##########.#######
.####.#.###.###.#.##
....##.##.###..#####
.#.#.###########.###
#.#.#.#####.####.###
###.##.####.##.#..##";

		private static string m_map = @"###..#.##.####.##..###.#.#..
#..#..###..#.......####.....
#.###.#.##..###.##..#.###.#.
..#.##..##...#.#.###.##.####
.#.##..####...####.###.##...
##...###.#.##.##..###..#..#.
.##..###...#....###.....##.#
#..##...#..#.##..####.....#.
.#..#.######.#..#..####....#
#.##.##......#..#..####.##..
##...#....#.#.##.#..#...##.#
##.####.###...#.##........##
......##.....#.###.##.#.#..#
.###..#####.#..#...#...#.###
..##.###..##.#.##.#.##......
......##.#.#....#..##.#.####
...##..#.#.#.....##.###...##
.#.#..#.#....##..##.#..#.#..
...#..###..##.####.#...#..##
#.#......#.#..##..#...#.#..#
..#.##.#......#.##...#..#.##
#.##..#....#...#.##..#..#..#
#..#.#.#.##..#..#.#.#...##..
.#...#.........#..#....#.#.#
..####.#..#..##.####.#.##.##
.#.######......##..#.#.##.#.
.#....####....###.#.#.#.####
....####...##.#.#...#..#.##.";

		public static void Part1()
		{
			var asteroids = ParseMap(m_map);
			var results = new Dictionary<Point, int>();

			foreach( var asteroid in asteroids ) {
				var cnt = 0;

				foreach( var target in asteroids ) {
					if( target.X == asteroid.X && target.Y == asteroid.Y )
						continue;

					//Console.WriteLine(Distance(asteroid, target));

					var blocked = false;

					// check to see if any asteroids block asteroid from seeing target
					foreach( var blocker in asteroids ) {
						if( (blocker.X == asteroid.X && blocker.Y == asteroid.Y) || (blocker.X == target.X && blocker.Y == target.Y) )
							continue;

						//if( asteroid.X == 1 && asteroid.Y == 0 && target.X == 4 && target.Y == 3 && blocker.X == 3 && blocker.Y == 2) {
						//	Console.WriteLine("this should block");
						//	Console.WriteLine($"\ta->t: {Distance(asteroid, target)} a->b: {Distance(asteroid, blocker)} b->t: {Distance(blocker, target)}");
						//	Console.WriteLine(Distance(asteroid, target) - Distance(asteroid, blocker) - Distance(blocker, target));
						//}

						if( Blocks(asteroid, target, blocker) ) {
							//Console.WriteLine($"{asteroid.X},{asteroid.Y} -> {target.X},{target.Y} blocked by {blocker.X},{blocker.Y}");
							blocked = true;
							break;
						}
					}

					if( !blocked ) {
						cnt += 1;
						//Console.WriteLine($"{asteroid.X},{asteroid.Y} sees {target.X},{target.Y}");
					}
				}

				results.Add(asteroid, cnt);
			}

			//foreach( var kvp in results )
			//	Console.WriteLine($"{kvp.Key.X},{kvp.Key.Y} {kvp.Value}");

			var best = results.OrderByDescending(i => i.Value).First();

			Console.WriteLine($"{best.Key.X},{best.Key.Y} ({best.Value})");
		}

		public static void Part2()
		{
			// "center" asteroid is 22,19
			// less than 1011

			//Console.WriteLine(Slope(new Point(10, 10), new Point(1, 9)));
			//Console.WriteLine(Slope(new Point(10, 10), new Point(9, 0)));
			//return;

			var asteroids = ParseMap(m_map);
			var center    = asteroids.Where(p => p.X == 22 && p.Y == 19).Single();

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

			while( true ) {
				for( var i = 0; i < sects.Length; i++ ) {
					if( sects[i].Count == 0 )
						continue;

					if( i % 2 == 0 ) {
						// no slope to calculate, just find lowest distance
						var to_kill = sects[i].OrderBy(p => Distance(center, p)).First();

						Console.WriteLine($"s{i} Killing {to_kill.X},{to_kill.Y} with slope {Slope(center, to_kill)}");
						sects[i].Remove(to_kill);

						if( ++cnt == 200 ) {
							Console.WriteLine($"{to_kill.X},{to_kill.Y} -> {(to_kill.X * 100) + to_kill.Y}");
							return;
						}
					} else {
						// group by slopes, order, and take out nearest for each slope
						var groups = sects[i].GroupBy(p => Slope(center, p));
						var ogroups = groups.OrderBy(g => g.Key);
						var to_kill = new List<Point>();

						foreach( var g in ogroups ) {
							Console.WriteLine($"s{i} group for slope {g.Key}:");

							foreach( var p in g )
								Console.WriteLine($"s{i} \t{p.X},{p.Y} (dist {Distance(center, p)})");

							to_kill.Add(g.OrderBy(p => Distance(center, p)).First());
						}

						foreach( var p in to_kill ) {
							Console.WriteLine($"s{i} Killing {p.X},{p.Y} with slope {Slope(center, p)}");
							sects[i].Remove(p);

							if( ++cnt == 200 ) {
								Console.WriteLine($"{p.X},{p.Y} -> {(p.X * 100) + p.Y}");
								return;
							}
						}
					}
				}
			}
		}

		private static double Distance(Point p1, Point p2) => Math.Round(Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2)), 6);

		private static bool Blocks(Point from, Point to, Point blocker) => Math.Abs(Distance(from, to) - Distance(from, blocker) - Distance(blocker, to)) == 0;

		private static double Slope(Point p1, Point p2) => Math.Round(((double)p2.Y - (double)p1.Y) / ((double)p2.X - (double)p1.X), 6);

		private static List<Point> ParseMap(string map)
		{
			var asteroids = new List<Point>();

			using( var sr = new StringReader(map) ) {
				var y = 0;

				while( sr.Peek() > -1 ) {
					var line = sr.ReadLine();

					for( var x = 0; x < line.Length; x++ ) {
						if( line[x] == '#' )
							asteroids.Add(new Point(x, y));
					}

					y++;
				}
			}

			return asteroids;
		}

		public static void Part2_Michael()
		{
			//var laserAsteroid = new Asteroid(27, 19);
			var laserAsteroid = new Asteroid(22, 19);

			string map = m_map;
//			string map = @"..#..###....#####....###........#
//.##.##...#.#.......#......##....#
//#..#..##.#..###...##....#......##
//..####...#..##...####.#.......#.#
//...#.#.....##...#.####.#.###.#..#
//#..#..##.#.#.####.#.###.#.##.....
//#.##...##.....##.#......#.....##.
//.#..##.##.#..#....#...#...#...##.
//.#..#.....###.#..##.###.##.......
//.##...#..#####.#.#......####.....
//..##.#.#.#.###..#...#.#..##.#....
//.....#....#....##.####....#......
//.#..##.#.........#..#......###..#
//#.##....#.#..#.#....#.###...#....
//.##...##..#.#.#...###..#.#.#..###
//.#..##..##...##...#.#.#...#..#.#.
//.#..#..##.##...###.##.#......#...
//...#.....###.....#....#..#....#..
//.#...###..#......#.##.#...#.####.
//....#.##...##.#...#........#.#...
//..#.##....#..#.......##.##.....#.
//.#.#....###.#.#.#.#.#............
//#....####.##....#..###.##.#.#..#.
//......##....#.#.#...#...#..#.....
//...#.#..####.##.#.........###..##
//.......#....#.##.......#.#.###...
//...#..#.#.........#...###......#.
//.#.##.#.#.#.#........#.#.##..#...
//.......#.##.#...........#..#.#...
//.####....##..#..##.#.##.##..##...
//.#.#..###.#..#...#....#.###.#..#.
//............#...#...#.......#.#..
//.........###.#.....#..##..#.##...";

			List<Asteroid> asteroids = new List<Asteroid>();
			int currentX = 0;
			int currentY = 0;
			foreach( char mapPoint in map ) {
				//don't stick our laser asteroid in here
				if( mapPoint == '#' && (currentX != laserAsteroid.X || currentY != laserAsteroid.Y) ) {
					var targetAsteroid = new Asteroid(currentX, currentY);

					int deltaX = targetAsteroid.X - laserAsteroid.X;
					int deltaY = targetAsteroid.Y - laserAsteroid.Y;
					targetAsteroid.Distance = Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));

					if( deltaY < 0 && deltaX >= 0 ) {
						//Quadrant1 (NorthEast)
						targetAsteroid.QuadrantOrder = 1;
						targetAsteroid.Slope = deltaX != 0 ? (double)deltaY / deltaX : double.MinValue; //undefined slope as min value so it sorts first
					} else if( deltaY <= 0 && deltaX < 0 ) {
						//Quadrant4 (NorthWest) -- Yes, I know this is not how Cartesian quadrants are numbered.
						targetAsteroid.QuadrantOrder = 4;
						targetAsteroid.Slope = (double)deltaY / deltaX;
					} else if( deltaY > 0 && deltaX <= 0 ) {
						//Quadrant3 (SouthWest)
						targetAsteroid.QuadrantOrder = 3;
						targetAsteroid.Slope = deltaX != 0 ? (double)deltaY / deltaX : double.MinValue; //undefined slope as min value so it sorts first
					} else if( deltaY >= 0 && deltaX > 0 ) {
						//Quadrant2 (SouthEast)
						targetAsteroid.QuadrantOrder = 2;
						targetAsteroid.Slope = (double)deltaY / deltaX;
					}

					asteroids.Add(targetAsteroid);
				}

				if( mapPoint == '\n' ) {
					currentX = 0;
					currentY++;
				} else {
					currentX++;
				}
			}

			asteroids = asteroids.OrderBy(a => a.QuadrantOrder).ThenBy(a => a.Slope).ThenBy(a => a.Distance).ToList();
			var finalOrderAsteroids = new List<Asteroid>();
			int rotationNumber = 1;
			while( finalOrderAsteroids.Count < 200 && finalOrderAsteroids.Count < asteroids.Count ) {
				finalOrderAsteroids.AddRange(GetAsteroidsForNthRotation(asteroids, rotationNumber));
				rotationNumber++;
			}

			Console.WriteLine($"200th Item: {finalOrderAsteroids[199].X},{finalOrderAsteroids[199].Y}, Quadrant: {finalOrderAsteroids[199].QuadrantOrder}, Slope:{finalOrderAsteroids[199].Slope}, Distance: {finalOrderAsteroids[199].Distance}");
			Console.WriteLine($"Silly math: {finalOrderAsteroids[199].X * 100 + finalOrderAsteroids[199].Y}");
		}

		private static List<Asteroid> GetAsteroidsForNthRotation(List<Asteroid> allAsteroidsInOrder, int rotationNumber)
		{
			List<Asteroid> AsteroidsForThisRotation = new List<Asteroid>();
			double previousSlope = 0;
			int slopesInARow = 1;
			for( int i = 0; i < allAsteroidsInOrder.Count; i++ ) {
				if( rotationNumber == 1 && i == 0 ) {
					Console.WriteLine($"killing {allAsteroidsInOrder[i].X},{allAsteroidsInOrder[i].Y} with slope {allAsteroidsInOrder[i].Slope}");
					AsteroidsForThisRotation.Add(allAsteroidsInOrder[i]);
					previousSlope = allAsteroidsInOrder[i].Slope;
					continue;
				}
				if( i == 0 ) {
					previousSlope = allAsteroidsInOrder[i].Slope;
					continue;
				}

				if( previousSlope == allAsteroidsInOrder[i].Slope ) {
					slopesInARow++;
				} else {
					slopesInARow = 1;
					previousSlope = allAsteroidsInOrder[i].Slope;
				}

				if( slopesInARow == rotationNumber ) {
					Console.WriteLine($"killing {allAsteroidsInOrder[i].X},{allAsteroidsInOrder[i].Y} with slope {allAsteroidsInOrder[i].Slope}");
					AsteroidsForThisRotation.Add(allAsteroidsInOrder[i]);
				}
			}
			return AsteroidsForThisRotation;

		}

		public class Asteroid
		{
			public int X { get; private set; }
			public int Y { get; private set; }
			public int QuadrantOrder { get; set; }
			public double Slope { get; set; }
			public double Distance { get; set; }

			public Asteroid(int x, int y)
			{
				X = x;
				Y = y;
			}
		}
	}
}
