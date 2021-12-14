var input = File.ReadAllLines(args[0]).ToList();
var map   = input.SelectMany((line, y) => line.Select((c, x) => new Location(x, y, int.Parse(c.ToString())))).ToList();
var lows  = map.Where(l => Adjacents(map, l).All(a => a.Height > l.Height)).ToList();
var sizes = new List<int>();

Console.WriteLine($"part 1: {lows.Sum(l => l.Height + 1)}"); // part 1 is 452

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

Console.WriteLine($"part 2: {sizes.OrderByDescending(i => i).Take(3).Aggregate(1, (s1, s2) => s1 * s2)}"); // part 2 is 1263735

IEnumerable<Location> Adjacents(IEnumerable<Location> input, Location center) => input.Where(l =>
	(l.X == center.X && (l.Y == center.Y + 1 || l.Y == center.Y - 1))
	|| (l.Y == center.Y && (l.X == center.X + 1 || l.X == center.X - 1)));

record struct Location(int X, int Y, int Height);