using day_04;

//var g = new int[,] {
//	{ 22, 13, 17, 11,  0 },
//	{  8,  2, 23,  4, 24 },
//	{ 21,  9, 14, 16,  7 },
//	{  6, 10,  3, 18,  5 },
//	{  1, 12, 20, 15, 19 },
//};

//var b = new Board(g);

//var inp = @"7,4,9,5,11,17,23,2,0,14,21,24,10,16,13,6,15,25,12,22,18,20,8,19,3,26,1

//22 13 17 11  0
// 8  2 23  4 24
//21  9 14 16  7
// 6 10  3 18  5
// 1 12 20 15 19

// 3 15  0  2 22
// 9 18 13 17  5
//19  8  7 25 23
//20 11 10 24  4
//14 21 16 12  6

//14 21 17 24  4
//10 16 15  9 19
//18  8 23 26 20
//22 11 13  6  5
// 2  0 12  3  7".Split(Environment.NewLine).ToList();

var inp = File.ReadAllLines(args[0]);
Console.WriteLine(inp.Length);
var balls  = inp.First().Split(',').Select(s => int.Parse(s)).ToList();
var boards = inp.Skip(2).Chunk(6).Select(s => new Board(string.Join(" ", s).Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(s => int.Parse(s)))).ToArray();
var part1  = default(int?);

Console.WriteLine($"There are {boards.Length} boards");

foreach (var ball in balls) {
	for (var b = 0; b < boards.Length; b++) {

		// skip boards that have won already
		if (boards[b] == null) {
			continue;
		}

		var score = boards[b].Mark(ball);

		if (score.HasValue) {
			part1 ??= score * ball;

			if (boards.Count(b => b != null) == 1) {
				Console.WriteLine($"part 1: {part1}");
				Console.WriteLine($"part 2: {score * ball}"); // 4809 is too low
				Console.WriteLine($"(board {b} won last)");
			}

			//Console.WriteLine($"Board {b} wins, removing it");
			boards[b] = null;
		}
	}
}
