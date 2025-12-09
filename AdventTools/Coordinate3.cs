using System.Runtime.CompilerServices;

namespace AdventOfCode;

public readonly record struct Coordinate3(long X, long Y, long Z)
{
	public static Coordinate3 operator +(Coordinate3 operand1, Coordinate3 operand2) => new(operand1.X + operand2.X, operand1.Y + operand2.Y, operand1.Z + operand2.Z);

	public static Coordinate3 operator -(Coordinate3 operand1, Coordinate3 operand2) => new(operand1.X - operand2.X, operand1.Y - operand2.Y, operand1.Z - operand2.Z);

	public static Coordinate3 Parse(string line)
	{
		var parts = line.Split(',');

		return new Coordinate3(long.Parse(parts[0]), long.Parse(parts[1]), long.Parse(parts[2]));
	}

	public static Coordinate3 Parse(ReadOnlySpan<char> characterSpan, ReadOnlySpan<Range> ranges)
	{
		if (ranges.Length != 3) {
			throw new InvalidOperationException("Parsing a 3-coordinate requires exactly 3 ranges");
		}

		return new Coordinate3(long.Parse(characterSpan[ranges[0]]), long.Parse(characterSpan[ranges[1]]), long.Parse(characterSpan[ranges[2]]));
	}

	public static double EuclideanDistance(Coordinate3 from, Coordinate3 to) => Math.Sqrt(
		((from.X - to.X) * (from.X - to.X)) +
		((from.Y - to.Y) * (from.Y - to.Y)) +
		((from.Z - to.Z) * (from.Z - to.Z))
	);

	public static Coordinate3 Empty { get; } = new(long.MinValue, long.MinValue, long.MinValue);

	public override string ToString() => $"[{X},{Y},{Z}]";
}
