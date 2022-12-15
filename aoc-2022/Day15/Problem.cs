using Microsoft.Z3;

using System.Text.RegularExpressions;

using static AdventOfCode.AStar;

namespace aoc_2022.Day15;

public partial class Problem
{
	public static (int, long) Main(string fileName)
	{
		//return BruteForce(fileName);
		return Solver(fileName);
	}

	private static (int, long) Solver(string fileName)
	{
		using var ctx    = new Context();
		using var solver = ctx.MkSolver();

		using var x = ctx.MkIntConst("x");
		using var y = ctx.MkIntConst("y");

		solver.Assert(ctx.MkGt(x, ctx.MkInt(0)));
		solver.Assert(ctx.MkLt(x, ctx.MkInt(4000000)));
		solver.Assert(ctx.MkGt(y, ctx.MkInt(0)));
		solver.Assert(ctx.MkLt(y, ctx.MkInt(4000000)));

		foreach (var sd in ReadFileLines(fileName, Parse)) {
			var d = Math.Abs(sd.sensor.x - sd.beacon.x) + Math.Abs(sd.sensor.y - sd.beacon.y);

			var xDiff = sd.sensor.x - x;
			var xAbs  = ctx.MkITE(ctx.MkLt(xDiff, ctx.MkInt(0)), ctx.MkMul(xDiff, ctx.MkInt(-1)), xDiff);
			var yDiff = sd.sensor.y - y;
			var yAbs  = ctx.MkITE(ctx.MkLt(yDiff, ctx.MkInt(0)), ctx.MkMul(yDiff, ctx.MkInt(-1)), yDiff);
			var dist  = ctx.MkAdd((ArithExpr)xAbs, (ArithExpr)yAbs);

			solver.Assert(ctx.MkGt(dist, ctx.MkInt(d)));
		}

		var status = solver.Check();

		var xNum = solver.Model.Consts.Single(c => c.Key.Name.ToString() == "x").Value as IntNum ?? throw new InvalidCastException();
		var yNum = solver.Model.Consts.Single(c => c.Key.Name.ToString() == "y").Value as IntNum ?? throw new InvalidCastException();

		Console.WriteLine($"{xNum.Int},{yNum.Int}");
		Console.WriteLine((xNum.Int * 4000000L) + yNum.Int);

		return (0, (xNum.Int * 4000000L) + yNum.Int);
	}

	private static (int, long) BruteForce(string fileName)
	{
		var sensors        = ReadFileLines(fileName, Parse);
		var excludedPoints = sensors.Select(s => ExcludedPoints(s.sensor, s.beacon, 2000000));
		var part1          = excludedPoints.SelectMany(c => c).Except(sensors.Select(s => s.beacon)).Except(sensors.Select(s => s.sensor)).Distinct().Count();

		var candidatePoints = sensors.SelectMany(s => CandidatePoints(s.sensor, s.beacon));

		var xMax  = 4000000;
		var yMax  = 4000000;
		var p     = candidatePoints.Except(sensors.Select(s => s.beacon)).Where(c => c.x >= 0 && c.x <= xMax && c.y >= 0 && c.y <= yMax).First(c => !sensors.Any(s => ManhattanDistance(c, s.sensor) <= s.Range));
		var part2 = (p.x * 4000000L) + p.y;

		return (part1, part2);
	}

	private static IEnumerable<Coordinate> ExcludedPoints(Coordinate sensor, Coordinate closestBeacon, int yInterest)
	{
		var beaconDistance = ManhattanDistance(sensor, closestBeacon);
		var xMin = sensor.x - beaconDistance;
		var xMax = sensor.x + beaconDistance;
		var yMin = sensor.y - beaconDistance; 
		var yMax = sensor.y + beaconDistance;

		// shortcut for sensors outside the range of interest
		//if (yMin > yInterest || yMax < yInterest) {
		//	yield break;
		//}

		// closest excluded points will be at (sensor.x, sensor.y - beaconDistance) etc

		for (var x = xMin; x <= xMax; x++) {
			//for (var y = yMin; y <= yMax; y++) {
			for (var y = yInterest; y <= yInterest; y++) {
				if (ManhattanDistance(x, y, sensor.x, sensor.y) <= beaconDistance) {
					yield return new Coordinate(x, y);
				}
			}
		}
	}

	private static IEnumerable<Coordinate> CandidatePoints(Coordinate sensor, Coordinate closestBeacon)
	{
		var beaconDistance = ManhattanDistance(sensor, closestBeacon);
		var xMin           = sensor.x - beaconDistance - 1;
		var xMax           = sensor.x + beaconDistance + 1;
		var yMin           = sensor.y - beaconDistance - 1;
		var yMax           = sensor.y + beaconDistance + 1;
		var x              = sensor.x;
		var y              = yMin;

		// top-right side
		while (x < xMax) {
			yield return new Coordinate(x++, y++);
		}

		// bottom-right side
		while (y < yMax) {
			yield return new Coordinate(x--, y++);
		}

		// bottom-left side
		while (x > xMin) {
			yield return new Coordinate(x--, y--);
		}

		// top-left side
		while (y > yMin) {
			yield return new Coordinate(x++, y--);
		}
	}

	private static int ManhattanDistance(Coordinate point1, Coordinate point2) => Math.Abs(point1.x - point2.x) + Math.Abs(point1.y - point2.y);

	private static int ManhattanDistance(int x1, int y1, int x2, int y2) => Math.Abs(x1 - x2) + Math.Abs(y1 - y2);

	private static SensorData Parse(string line)
	{
		var match = ParseRegex().Match(line);

		if (!match.Success) {
			throw new InvalidDataException("No match during parse");
		}

		return new SensorData((int.Parse(match.Groups["sensorx"].Value), int.Parse(match.Groups["sensory"].Value)), (int.Parse(match.Groups["beaconx"].Value), int.Parse(match.Groups["beacony"].Value)));
	}

	private readonly record struct SensorData(Coordinate sensor, Coordinate beacon)
	{
		private readonly Lazy<int> _range = new(ManhattanDistance(sensor, beacon));

		public int Range => _range.Value;
	}

	[GeneratedRegex(@"Sensor at x=(?<sensorx>[\-\d]+), y=(?<sensory>[\-\d]+): closest beacon is at x=(?<beaconx>[\-\d]+), y=(?<beacony>[\-\d]+)")]
	private static partial Regex ParseRegex();
}
