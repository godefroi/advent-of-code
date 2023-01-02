using aoc_tools;

//var input = @"..##.......
//#...#...#..
//.#....#..#.
//..#.#...#.#
//.#...##..#.
//..#.##.....
//.#.#.#....#
//.#........#
//#.##...#...
//#...##....#
//.#..#...#.#".Split(Environment.NewLine).ToList();

var input  = (await PuzzleInput.GetInputLines()).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
var mult   = 0L;
var slopes = new[] {
	(right: 1, down: 1),
	(right: 3, down: 1),
	(right: 5, down: 1),
	(right: 7, down: 1),
	(right: 1, down: 2),
};

foreach (var slope in slopes) {
	var tcnt = 0;
	var x    = 0;
	var y    = 0;

	while (true) {
		var c = Clear(x, y);

		if (c == null) {
			break;
		} else if (!c.Value) {
			tcnt++;
		}

		x += slope.right;
		y += slope.down;
	}

	if (slope.right == 3 && slope.down == 1) {
		Console.WriteLine($"part 1: {tcnt}"); // part 1 is 169
	}

	if (mult == 0) {
		mult = tcnt;
	} else {
		mult *= tcnt;
	}
}

Console.WriteLine($"part 2: {mult}"); // part 2 is 7560370818

bool? Clear(int x, int y) => y > input.Count - 1 ? null : input[y][x % input.First().Length] == '.';
