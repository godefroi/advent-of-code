using System.Collections.Immutable;
using System.Text;

namespace AdventOfCode.Year2023.Day13;

public partial class Problem
{
	public static ProblemMetadata2 Metadata { get; } = new(Execute, null);

	public static (long, long) Execute(string[] input)
	{
		var part1 = 0;
		var part2 = 0;

		foreach (var chunk in ChunkByEmpty(input)) {
			var map      = new Map(chunk);
			var (p1, p2) = FindReflections(map);

			part1 += p1;
			part2 += p2;
		}

		// 30158 is the answer

		return (part1, part2);
	}

	private static int FindReflection(Map map)
	{
		for (var x = 0; x < map.Columns.Length - 1; x++) {
			if (IsReflection(map.Columns, x)) {
				Console.WriteLine($"Found a reflection at x={x}");
				return x + 1;
			}
		}

		for (var y = 0; y < map.Rows.Length - 1; y++) {
			if (IsReflection(map.Rows, y)) {
				Console.WriteLine($"Found a reflection at y={y}");
				return (y + 1) * 100;
			}
		}

		Console.WriteLine("*** DID NOT FIND A REFLECTION!");
		return 0;
	}

	private static bool IsReflection(char[][] items, int index)
	{
		// if length is 10, and index is 4, then leftMax should be 5 and rightMax should be 5
		var leftIdx  = index;
		var rightIdx = index + 1;

		if (rightIdx >= items.Length) {
			return false;
		}

		for (; leftIdx >= 0 && rightIdx < items.Length; leftIdx--, rightIdx++ ) {
			if (!AreEqual(items[leftIdx], items[rightIdx])) {
				return false;
			}
		}

		return true;
	}

	private static (int reflection, int almostReflection) FindReflections(Map map)
	{
		var reflection = -1;
		var almost     = -1;

		for (var x = 0; x < map.Columns.Length - 1; x++) {
			var thisDist = ReflectionHammingDistance(map.Columns, x);

			if (thisDist == 0) {
				reflection = x + 1;
			} else if (thisDist == 1) {
				almost = x + 1;
			}

			if (reflection > -1 && almost > -1) {
				return (reflection, almost);
			}
		}

		for (var y = 0; y < map.Rows.Length - 1; y++) {
			var thisDist = ReflectionHammingDistance(map.Rows, y);

			if (thisDist == 0) {
				reflection = (y + 1) * 100;
			} else if (thisDist == 1) {
				almost = (y + 1) * 100;
			}

			if (reflection > -1 && almost > -1) {
				return (reflection, almost);
			}
		}

		throw new InvalidOperationException($"Did not find reflections. reflection={reflection}, almost={almost}");
	}


	private static int ReflectionHammingDistance(char[][] items, int index)
	{
		var leftIdx  = index;
		var rightIdx = index + 1;
		var distance = 0;

		if (rightIdx >= items.Length) {
			//return false;
			throw new InvalidOperationException("The final item cannot be the left side of a reflection point.");
		}

		for (; leftIdx >= 0 && rightIdx < items.Length; leftIdx--, rightIdx++ ) {
			distance += HammingDistance(items[leftIdx], items[rightIdx]);

			// we're not interested in any sitch-ee-ations where the
			// distance is greater than one
			if (distance > 1) {
				return distance;
			}
		}

		return distance;
	}

	private static int HammingDistance(char[] c1, char[] c2)
	{
		if (c1.Length != c2.Length) {
			return int.MaxValue;
		}

		var dist = 0;

		for (var i = 0; i < c1.Length; i++) {
			if (c1[i] != c2[i]) {
				dist++;
			}
		}

		return dist;
	}

	private static bool AreEqual(char[] arr1, char[] arr2) {
		for (var i = 0; i < arr1.Length; i++) {
			if (arr1[i] != arr2[i]) {
				return false;
			}
		}

		return true;
	}

	private class Map
	{
		public Map(IEnumerable<string> lines)
		{
			// rows are easy
			Rows = lines.Select(l => l.ToCharArray()).ToArray();

			// foreach (var row in Rows) {
			// 	Console.WriteLine(row);
			// }

			// cols are harder
			var cols = new List<char[]>(Rows[0].Length);
			var sb   = new StringBuilder(Rows.Length);

			for (var x = 0; x < Rows[0].Length; x++) {
				for (var y = 0; y < Rows.Length; y++) {
					sb.Append(Rows[y][x]);
				}

				var col = new char[sb.Length];
				sb.CopyTo(0, col, col.Length);
				cols.Add(col);
				sb.Clear();
			}

			Columns = cols.ToArray();
		}

		public char[][] Rows { get; private set; }

		public char[][] Columns { get; private set; }
	}

	public class Tests
	{
		[Theory]
		[InlineData(new[] { "a", "b", "b", "a", "a", "e", "d", "c", "b", "a" }, 0, false)]
		[InlineData(new[] { "a", "b", "b", "a", "a", "e", "d", "c", "b", "a" }, 1, true)]
		[InlineData(new[] { "a", "b", "b", "a", "a", "e", "d", "c", "b", "a" }, 2, false)]
		[InlineData(new[] { "a", "b", "b", "a", "a", "e", "d", "c", "b", "a" }, 3, false)]
		[InlineData(new[] { "a", "b", "c", "d", "e", "e", "d", "c", "b", "a" }, 0, false)]
		[InlineData(new[] { "a", "b", "c", "d", "e", "e", "d", "c", "b", "a" }, 1, false)]
		[InlineData(new[] { "a", "b", "c", "d", "e", "e", "d", "c", "b", "a" }, 2, false)]
		[InlineData(new[] { "a", "b", "c", "d", "e", "e", "d", "c", "b", "a" }, 3, false)]
		[InlineData(new[] { "a", "b", "c", "d", "e", "e", "d", "c", "b", "a" }, 4, true)]
		[InlineData(new[] { "a", "b", "c", "d", "e", "e", "d", "c", "b", "a" }, 5, false)]
		[InlineData(new[] { "a", "b", "c", "d", "e", "e", "d", "c", "b", "a" }, 6, false)]
		[InlineData(new[] { "a", "b", "c", "d", "e", "e", "d", "c", "b", "a" }, 7, false)]
		[InlineData(new[] { "a", "b", "c", "d", "e", "e", "d", "c", "b", "a" }, 8, false)]
		[InlineData(new[] { "a", "b", "c", "d", "e", "e", "d", "c", "b", "a" }, 9, false)]
		public void ReflectionsAreDetected(string[] strings, int index, bool isReflection) =>
			Assert.Equal(isReflection, IsReflection(strings.Select(s => s.ToCharArray()).ToArray(), index));

		[Fact]
		public void FirstSampleMapFindsReflection()
		{
			var map1 = new Map(ChunkByEmpty(ReadFileLines("inputSample.txt")).First());

			Assert.Equal(5, FindReflection(map1));
		}

		[Fact]
		public void AllRealMapsFindReflection()
		{
			var skipped = 0;

			foreach (var chunk in ChunkByEmpty(ReadFileLines("input.txt"))) {
				var theMap     = new Map(chunk);
				var reflection = FindReflection(theMap);

				Assert.NotEqual(0, reflection);
				skipped++;
			}
		}
	}
}
