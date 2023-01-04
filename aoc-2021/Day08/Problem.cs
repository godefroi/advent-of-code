using Xunit;

namespace Day08;

public class Problem
{
	internal static (int, long) Main(string fileName)
	{
		var input = File.ReadAllLines(fileName);
		var p1    = 0;
		var p2    = 0L;
		var segs  = "abcdefg";

		foreach (var line in input) {
			var parts   = line.Split(" | ");
			var inputs  = parts[0].Split(' ');
			var outputs = parts[1].Split(' ');
			var dmaps   = new string[10];
			var smaps   = new char[7];
			var scounts = parts[0].Replace(" ", "").GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());

			p1 += outputs.Count(i => i.Length == 2);
			p1 += outputs.Count(i => i.Length == 4);
			p1 += outputs.Count(i => i.Length == 3);
			p1 += outputs.Count(i => i.Length == 7);

			dmaps[1] = inputs.Single(i => i.Length == 2);
			dmaps[4] = inputs.Single(i => i.Length == 4);
			dmaps[7] = inputs.Single(i => i.Length == 3);
			dmaps[8] = inputs.Single(i => i.Length == 7);

			/* a */ smaps[0] = dmaps[7].Except(dmaps[4]).Single();
			/* b */ smaps[1] = scounts.Single(kvp => kvp.Value == 6).Key;
			/* c */ smaps[2] = dmaps[4].Single(c => inputs.Count(i => i.Contains(c)) == 8);
			/* d */ smaps[3] = dmaps[4].Single(c => inputs.Where(i => i.Length == 5).All(i => i.Contains(c)));
			/* e */ smaps[4] = scounts.Single(kvp => kvp.Value == 4).Key;
			/* f */ smaps[5] = scounts.Single(kvp => kvp.Value == 9).Key;
			/* g */ smaps[6] = segs.Single(c => !smaps.Contains(c));

			dmaps[0] = inputs.Single(i => i.Length == 6 && i.Except(new[] { smaps[0], smaps[1], smaps[2], smaps[4], smaps[5], smaps[6] }).Count() == 0);
			dmaps[2] = inputs.Single(i => i.Length == 5 && i.Except(new[] { smaps[0], smaps[2], smaps[3], smaps[4], smaps[6] }).Count() == 0);
			dmaps[3] = inputs.Single(i => i.Length == 5 && i.Except(new[] { smaps[0], smaps[2], smaps[3], smaps[5], smaps[6] }).Count() == 0);
			dmaps[5] = inputs.Single(i => i.Length == 5 && i.Except(new[] { smaps[0], smaps[1], smaps[3], smaps[5], smaps[6] }).Count() == 0);
			dmaps[6] = inputs.Single(i => i.Length == 6 && i.Except(new[] { smaps[0], smaps[1], smaps[3], smaps[4], smaps[5], smaps[6] }).Count() == 0);
			dmaps[9] = inputs.Single(i => i.Length == 6 && i.Except(new[] { smaps[0], smaps[1], smaps[2], smaps[3], smaps[5], smaps[6] }).Count() == 0);

			//Console.WriteLine($"{smaps[0]} -> a");
			//Console.WriteLine($"{smaps[1]} -> b");
			//Console.WriteLine($"{smaps[2]} -> c");
			//Console.WriteLine($"{smaps[3]} -> d");
			//Console.WriteLine($"{smaps[4]} -> e");
			//Console.WriteLine($"{smaps[5]} -> f");
			//Console.WriteLine($"{smaps[6]} -> g");
			//return;

			//Console.WriteLine($"0 => {dmaps[0]}");
			//Console.WriteLine($"1 => {dmaps[1]}");
			//Console.WriteLine($"2 => {dmaps[2]}");
			//Console.WriteLine($"3 => {dmaps[3]}");
			//Console.WriteLine($"4 => {dmaps[4]}");
			//Console.WriteLine($"5 => {dmaps[5]}");
			//Console.WriteLine($"6 => {dmaps[6]}");
			//Console.WriteLine($"7 => {dmaps[7]}");
			//Console.WriteLine($"8 => {dmaps[8]}");
			//Console.WriteLine($"9 => {dmaps[9]}");
			//return;

			/*foreach (var g in inputs.SelectMany(c => c).GroupBy(c => c)) {
				Console.WriteLine($"{g.Key} => {g.Count()}");
			}
			return;*/

			var dlist = dmaps.ToList();

			p2 += int.Parse(new string(outputs.Select(o => dlist.IndexOf(dmaps.Single(i => i.Length == o.Length && i.Except(o).Count() == 0))).Select(o => o.ToString()[0]).ToArray()));
		}

		Console.WriteLine($"part 1: {p1}"); // part 1 is 344
		Console.WriteLine($"part 2: {p2}"); // part 2 is 1048410

		return (p1, p2);

		// segment a appears in 8 digits (0,    2, 3,    5, 6, 7, 8  9)
		//         b appears in 6 digits (0,          4, 5, 6,    8, 9)
		//         c appears in 8 digits (0, 1, 2, 3, 4,       7, 8, 9)
		//         d appears in 7 digits (      2, 3, 4, 5, 6,    8, 9)
		//         e appears in 4 digits (0,    2,          6,    8,  )
		//         f appears in 9 digits (0, 1,    3, 4, 5, 6, 7, 8, 9)
		//         g appears in 7 digits (0,    2, 3,    5, 6,    8, 9)
		// (segment counts per digit)     6  2  5  5  4  5  6  3  7  6

		// if it has 2 segments, it's a 1
		// if it has 4 segments, it's a 4
		// if it has 3 segments, it's a 7
		// if it has 7 segments, it's a 8

		// if it appears in 7 but not 4, it's segment a
		// if it appears in 6 digits, it's segment b
		// if it appears in 8 digits and 4, it's segment c
		// if it appears in 4 and also in all digits that has 5 segments, it's segment d
		// if it appears in 4 digits, it's segment e
		// if it appears in 9 digits, it's segment f
		// otherwise, it's segment g
	}

	[Fact(DisplayName = "Day 08 Sample Input")]
	public void SampleInputFunctionCorrectly()
	{
		var (p1, p2) = Main("../../../Day08/input_sample.txt");

		Assert.Equal(26, p1);
		Assert.Equal(61229, p2);
	}

	[Fact(DisplayName = "Day 08 Main Input")]
	public void MainInputFunctionCorrectly()
	{
		var (p1, p2) = Main("../../../Day08/input.txt");

		Assert.Equal(344, p1);
		Assert.Equal(1048410, p2);
	}
}
