namespace AdventOfCode;

public readonly record struct Coordinate3(int X, int Y, int Z)
{
	public static Coordinate3 operator +(Coordinate3 operand1, Coordinate3 operand2) => new(operand1.X + operand2.X, operand1.Y + operand2.Y, operand1.Z + operand2.Z);

	public static Coordinate3 operator -(Coordinate3 operand1, Coordinate3 operand2) => new(operand1.X - operand2.X, operand1.Y - operand2.Y, operand1.Z - operand2.Z);

	public static Coordinate3 Parse(string line)
	{
		var parts = line.Split(',');

		return new Coordinate3(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
	}
}
