namespace AdventOfCode;

public readonly record struct Coordinate(int X, int Y)
{
	public static implicit operator Coordinate((int x, int y) tuple) => new(tuple.x, tuple.y);

	public static implicit operator (int x, int y)(Coordinate coordinate) => (coordinate.X, coordinate.Y);

	public static Coordinate operator +(Coordinate operand1, Coordinate operand2) => new(operand1.X + operand2.X, operand1.Y + operand2.Y);

	public static Coordinate operator -(Coordinate operand1, Coordinate operand2) => new(operand1.X - operand2.X, operand1.Y - operand2.Y);

	public static Coordinate operator *(Coordinate operand1, Coordinate operand2) => new(operand1.X * operand2.X, operand1.Y * operand2.Y);

	public static Coordinate operator *(Coordinate operand1, int operand2) => new(operand1.X * operand2, operand1.Y * operand2);

	public static Coordinate operator -(Coordinate operand) => new(-operand.X, -operand.Y);

	public static int ManhattanDistance(Coordinate from, Coordinate to) => Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);

	public static Coordinate Empty { get; } = new Coordinate(int.MinValue, int.MinValue);

	public override string ToString() => $"[{X},{Y}]";
}
