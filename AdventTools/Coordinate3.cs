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

	public override string ToString() => $"[{X},{Y},{Z}]";
}
