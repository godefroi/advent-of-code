using Xunit;

namespace Day09;

public class Problem
{
	public static (int p1, int p2) Main(string fileName)
	{
		var input = File.ReadAllLines(fileName).ToList();
		var map   = input.SelectMany((line, y) => line.Select((c, x) => new Location(x, y, int.Parse(c.ToString())))).ToList();
		var lows  = map.Where(l => Adjacents(map, l).All(a => a.Height > l.Height)).ToList();
		var sizes = new List<int>();
		var p1    = lows.Sum(l => l.Height + 1);

		Console.WriteLine($"part 1: {p1}"); // part 1 is 452
		
		foreach (var low in lows) {
			var basinLocations = Adjacents(map, low).Where(l => l.Height < 9).ToHashSet();
			var lastCount      = basinLocations.Count + 1;
		
			// make sure we get the center in there
			basinLocations.Add(low);
		
			// find all the neighbors
			while (true) {
				var newLocs = basinLocations.SelectMany(l => Adjacents(map, l).Where(a => a.Height < 9)).ToList();
		
				if (newLocs.Where(l => basinLocations.Add(l)).Count() == 0) {
					break;
				}
			}
		
			// find the size of the basin
			sizes.Add(basinLocations.Count);
		}
		
		var p2 = sizes.OrderByDescending(i => i).Take(3).Aggregate(1, (s1, s2) => s1 * s2);

		Console.WriteLine($"part 2: {p2}"); // part 2 is 1263735

		return (p1, p2);
	}

	private static IEnumerable<Location> Adjacents(IEnumerable<Location> input, Location center) => input.Where(l =>
		(l.X == center.X && (l.Y == center.Y + 1 || l.Y == center.Y - 1))
		|| (l.Y == center.Y && (l.X == center.X + 1 || l.X == center.X - 1)));

	[Fact(DisplayName = "Day 09 Sample Input")]
	public void SampleInputFunctionCorrectly()
	{
		var (p1, p2) = Main("../../../Day09/input_sample.txt");

		Assert.Equal(15, p1);
		Assert.Equal(1134, p2);
	}

	[Fact(DisplayName = "Day 09 Main Input")]
	public void MainInputFunctionCorrectly()
	{
		var (p1, p2) = Main("../../../Day09/input.txt");

		Assert.Equal(452, p1);
		Assert.Equal(1263735, p2);
	}

	record struct Location(int X, int Y, int Height);
}
