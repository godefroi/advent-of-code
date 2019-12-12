using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace aoc_2019
{
	internal static class problem_12
	{
		private static Moon[] m_input = new[] {
				new Moon(0, new Vector3(-7, -8, 9)),
				new Moon(1, new Vector3(-12, -3, -4)),
				new Moon(2, new Vector3(6, -17, -9)),
				new Moon(3, new Vector3(4, -10, -6)),
			};

		public static void Part1()
		{
			var moons = m_input;

			for( var n = 0; n < 1000; n++ ) {
				// apply gravity
				for( var i = 0; i < moons.Length; i++ ) {
					foreach( var o in moons ) {
						if( moons[i].Id != o.Id )
							moons[i] = moons[i].ApplyGravity(o);
					}
				}

				// apply velocity
				moons = moons.Select(m => m.ApplyVelocity()).ToArray();
			}

			Console.WriteLine(moons.Sum(m => m.Energy));
		}

		public static void Part2()
		{
			var moons  = m_input;

			var mul_x = CalculateDimension(moons.Select(m => (int)m.Position.X));
			var mul_y = CalculateDimension(moons.Select(m => (int)m.Position.Y));
			var mul_z = CalculateDimension(moons.Select(m => (int)m.Position.Z));

			Console.WriteLine(LeastCommonMultiple(new long[] { mul_x, mul_y, mul_z }));
		}

		private static int CalculateDimension(IEnumerable<int> positions)
		{
			var origin = positions.ToArray();
			var pos    = origin.Clone() as int[];
			var vel    = new int[pos.Length];
			var cnt    = 0;

			while( true ) {
				for( var i = 0; i < pos.Length; i++ ) {
					for( var j = 0; j < pos.Length; j++ ) {
						if( pos[j] > pos[i] )
							vel[i] += 1;
						else if( pos[j] < pos[i] )
							vel[i] -= 1;
					}
				}

				for( var i = 0; i < pos.Length; i++ )
					pos[i] += vel[i];

				cnt++;

				if( vel.All(v => v == 0) && pos.SequenceEqual(origin) )
					return cnt;
			}
		}

		private static long LeastCommonMultiple(long[] numbers) => numbers.Aggregate(LeastCommonMultiple);

		private static long LeastCommonMultiple(long a, long b) => Math.Abs(a * b) / GreatestCommonDenominator(a, b);

		private static long GreatestCommonDenominator(long a, long b) => b == 0 ? a : GreatestCommonDenominator(b, a % b);

		internal class Moon
		{
			public Moon(int id, Vector3 position) : this(id, position, Vector3.Zero) { }

			public Moon(int id, Vector3 position, Vector3 velocity)
			{
				Id       = id;
				Position = position;
				Velocity = velocity;
			}

			public int Id { get; }

			public Vector3 Position { get; }

			public Vector3 Velocity { get; }

			public long Energy => Position.Magnitude * Velocity.Magnitude;

			public Moon ApplyGravity(Moon other)
			{
				var chg_x = other.Position.X > Position.X ? 1 : other.Position.X < Position.X ? -1 : 0;
				var chg_y = other.Position.Y > Position.Y ? 1 : other.Position.Y < Position.Y ? -1 : 0;
				var chg_z = other.Position.Z > Position.Z ? 1 : other.Position.Z < Position.Z ? -1 : 0;

				return new Moon(Id, Position, Velocity + new Vector3(chg_x, chg_y, chg_z));
			}

			public Moon ApplyVelocity() => new Moon(Id, Position + Velocity, Velocity);

			public override string ToString() => $"pos=<x={Position.X,3}, y={Position.Y,3}, z={Position.Z,3}>, vel=<x={Velocity.X,3}, y={Velocity.Y,3}, z={Velocity.Z,3}>";
		}

		internal class Vector3
		{
			public Vector3(long x, long y, long z)
			{
				X = x;
				Y = y;
				Z = z;
			}

			public long X { get; }

			public long Y { get; }

			public long Z { get; }

			public long Magnitude => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);

			public static Vector3 operator +(Vector3 v1, Vector3 v2) => new Vector3(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);

			public static Vector3 Zero { get; } = new Vector3(0, 0, 0);
		}

		internal class Dimension
		{
			public Dimension(int position, int velocity)
			{
				Position = position;
				Velocity = velocity;
			}

			public int Position;
			public int Velocity;
		}
	}
}
