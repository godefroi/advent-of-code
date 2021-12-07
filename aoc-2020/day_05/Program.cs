using aoc_tools;

//var input = @"FBFBBFFRLR
//BFFFBBFRRR
//FFFBBBFRRR
//BBFFBBFRLL
//".Split(Environment.NewLine).SkipLast(1).ToList();

/*
FBFBBFFRLR: row 44, column 5, seat ID 357.
BFFFBBFRRR: row 70, column 7, seat ID 567.
FFFBBBFRRR: row 14, column 7, seat ID 119.
BBFFBBFRLL: row 102, column 4, seat ID 820.
*/

var input = (await PuzzleInput.GetInputLines()).SkipLast(1);
var seats = input.Select(i => FindSeat(i)).ToList();

Console.WriteLine($"part 1 {seats.Max()}"); // part 1 is 892
Console.WriteLine($"part 2 {Enumerable.Range(seats.Min(), seats.Max()).Single(i => seats.Contains(i + 1) && seats.Contains(i - 1) && !seats.Contains(i))}"); // part 2 is 625

int FindSeat(string pass)
{
	var rs = 0;
	var re = 127;
	var cs = 0;
	var ce = 7;

	foreach (var c in pass) {
		if (c == 'F') {
			re -= (re - rs) / 2 + 1;
		} else if (c == 'B') {
			rs += (re - rs) / 2 + 1;
		} else if (c == 'L') {
			ce -= (ce - cs) / 2 + 1;
		} else if (c == 'R') {
			cs += (ce - cs) / 2 + 1;
		}
	}

	return (rs * 8) + cs;
}
