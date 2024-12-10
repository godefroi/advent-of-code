namespace AdventOfCode;

public static class ArrayExtensions
{
	public static void Fill<T>(this T[,] array, T value)
	{
		var len0 = array.GetLength(0);
		var len1 = array.GetLength(1);

		for (var a = 0; a < len0; a++) {
			for (var b = 0; b < len1; b++) {
				array[a, b] = value;
			}
		}
	}

	public static T ValueAt<T>(this T[,] map, Coordinate location) => map[location.X, location.Y];
}
