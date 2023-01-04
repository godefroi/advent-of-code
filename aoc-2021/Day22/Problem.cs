using System.Text.RegularExpressions;

namespace Day22;

internal class Problem : ProblemBase
{
	private static Regex _parser = new Regex(@"(?<state>on|off) x=(?<xmin>[-\d]+)\.\.(?<xmax>[-\d]+),y=(?<ymin>[-\d]+)\.\.(?<ymax>[-\d]+),z=(?<zmin>[-\d]+)\.\.(?<zmax>[-\d]+)");

	internal static void Main(string fileName)
	{
		var instructions = Parse(fileName);
		var part1 = Calculate(instructions.Where(c => c.Cuboid.MinX >= -50 && c.Cuboid.MaxX <= 50 && c.Cuboid.MinY >= -50 && c.Cuboid.MaxY <= 50 && c.Cuboid.MinZ >= -50 && c.Cuboid.MaxZ <= 50));
		var part2 = Calculate(instructions);

		Console.WriteLine(part1);
		Console.WriteLine(part2);
	}

	private static long Calculate(IEnumerable<(bool On, Cuboid Cuboid)> instructions)
	{
		var cubes = new Dictionary<Cuboid, long>();

		foreach (var (turnOn, newCuboid) in instructions) {
			var newSign    = turnOn ? 1L : -1L;
			var newCuboids = new Dictionary<Cuboid, long>();

			// determine the intersection between this new cuboid we're turning on or off and each existing cuboid
			foreach (var (existingCuboid, curSign) in cubes) {
				var intersection = newCuboid.Intersect(existingCuboid);

				// if the intersection isn't empty, then we need to flip it
				if (intersection != Cuboid.Empty) {
					newCuboids[intersection] = newCuboids.GetValueOrDefault(intersection, 0) - curSign;
				}
			}

			// if we're turning on, then set that for the whole of the new cuboid
			if (turnOn) {
				newCuboids[newCuboid] = newCuboids.GetValueOrDefault(newCuboid, 0) + newSign;
			}

			// now, update our total set with the results of this new cuboid
			foreach (var kvp in newCuboids) {
				cubes[kvp.Key] = cubes.GetValueOrDefault(kvp.Key, 0) + kvp.Value;
			}
		}

		return cubes.Sum(a => a.Key.Area * a.Value);
	}

	private static IEnumerable<(bool On, Cuboid Cuboid)> Parse(string fileName) => File.ReadAllLines(GetFilePath(fileName)).Select(l => {
		var m = _parser.Match(l);

		if (!m.Success) {
			throw new InvalidOperationException($"Unable to match input line {l}");
		}

		return (m.Groups["state"].Value == "on", new Cuboid(int.Parse(m.Groups["xmin"].Value), int.Parse(m.Groups["xmax"].Value), int.Parse(m.Groups["ymin"].Value), int.Parse(m.Groups["ymax"].Value), int.Parse(m.Groups["zmin"].Value), int.Parse(m.Groups["zmax"].Value)));
	});

	private readonly record struct Cuboid(int MinX, int MaxX, int MinY, int MaxY, int MinZ, int MaxZ)
	{
		private readonly static Cuboid _empty = new(0, 0, 0, 0, 0, 0);

		public long Area => (MaxX - MinX + 1L) * (MaxY - MinY + 1L) * (MaxZ - MinZ + 1L);

		public Cuboid Intersect(Cuboid other)
		{
			var minX = Math.Max(MinX, other.MinX);
			var maxX = Math.Min(MaxX, other.MaxX);

			if (minX > maxX) {
				return _empty;
			}

			var minY = Math.Max(MinY, other.MinY);
			var maxY = Math.Min(MaxY, other.MaxY);

			if (minY > maxY) {
				return _empty;
			}

			var minZ = Math.Max(MinZ, other.MinZ);
			var maxZ = Math.Min(MaxZ, other.MaxZ);

			if (minZ > maxZ) {
				return _empty;
			}

			return new Cuboid(minX, maxX, minY, maxY, minZ, maxZ);
		}

		public static Cuboid Empty => _empty;
	}
}
