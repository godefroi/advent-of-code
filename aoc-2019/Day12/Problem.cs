using System.Text.RegularExpressions;

namespace aoc_2019.Day12;

public partial class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Main);

	public static (long, long) Main(string[] input)
	{
		var moons = input.Select(Parse).Select((vector, idx) => new Moon(idx, vector)).ToArray();
		var part1 = Part1(moons);
		var part2 = Part2(moons);

		return (part1, part2);
	}

	private static long Part1(Moon[] moons)
	{
		for (var n = 0; n < 1000; n++) {
			// apply gravity
			for (var i = 0; i < moons.Length; i++) {
				foreach (var o in moons) {
					if (moons[i].Id != o.Id) {
						moons[i] = moons[i].ApplyGravity(o);
					}
				}
			}

			// apply velocity
			moons = moons.Select(m => m.ApplyVelocity()).ToArray();
		}

		return moons.Sum(m => m.Energy);
	}

	private static long Part2(Moon[] moons)
	{
		var mul_x = CalculateDimension(moons.Select(m => (int)m.Position.X));
		var mul_y = CalculateDimension(moons.Select(m => (int)m.Position.Y));
		var mul_z = CalculateDimension(moons.Select(m => (int)m.Position.Z));

		return LeastCommonMultiple(new long[] { mul_x, mul_y, mul_z });
	}

	private static Vector3 Parse(string input)
	{
		var match = VectorRegex().Match(input);

		if (!match.Success) {
			throw new InvalidDataException($"The input {input} was not parseable.");
		}

		return new(long.Parse(match.Groups["x"].Value), long.Parse(match.Groups["y"].Value), long.Parse(match.Groups["z"].Value));
	}

	private static int CalculateDimension(IEnumerable<int> positions)
	{
		var origin = positions.ToArray();
		var pos    = (int[])origin.Clone();
		var vel    = new int[pos.Length];
		var cnt    = 0;

		while (true) {
			for (var i = 0; i < pos.Length; i++) {
				for (var j = 0; j < pos.Length; j++) {
					if (pos[j] > pos[i]) {
						vel[i] += 1;
					} else if (pos[j] < pos[i]) {
						vel[i] -= 1;
					}
				}
			}

			for (var i = 0; i < pos.Length; i++) {
				pos[i] += vel[i];
			}

			cnt++;

			if (vel.All(v => v == 0) && pos.SequenceEqual(origin)) {
				return cnt;
			}
		}
	}

	internal readonly record struct Moon(int Id, Vector3 Position, Vector3 Velocity)
	{
		public Moon(int id, Vector3 position) : this(id, position, Vector3.Zero) { }

		public long Energy => Position.Magnitude * Velocity.Magnitude;

		public Moon ApplyGravity(Moon other) => new(Id, Position, Velocity + new Vector3(
			(other.Position.X - Position.X) switch { > 0 => 1, < 0 => -1, 0 => 0 },
			(other.Position.Y - Position.Y) switch { > 0 => 1, < 0 => -1, 0 => 0 },
			(other.Position.Z - Position.Z) switch { > 0 => 1, < 0 => -1, 0 => 0 }));

		public Moon ApplyVelocity() => new(Id, Position + Velocity, Velocity);
	}

	internal readonly record struct Vector3(long X, long Y, long Z)
	{
		public long Magnitude => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);

		public static Vector3 operator +(Vector3 v1, Vector3 v2) => new(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);

		public static Vector3 Zero { get; } = new Vector3(0, 0, 0);
	}

	[GeneratedRegex(@"\<x=(?<x>[-\d]+), y=(?<y>[-\d]+), z=(?<z>[-\d]+)\>")]
	private static partial Regex VectorRegex();
}
