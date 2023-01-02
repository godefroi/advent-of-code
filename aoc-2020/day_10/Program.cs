using aoc_tools;

using Combinatorics.Collections;

//var input = @"16
//10
//15
//5
//1
//11
//7
//19
//6
//12
//4
//".Split(Environment.NewLine);

//var input = @"28
//33
//18
//42
//31
//14
//46
//20
//48
//47
//24
//23
//49
//45
//19
//38
//39
//11
//1
//32
//25
//35
//8
//17
//7
//9
//4
//2
//34
//10
//3
//".Split(Environment.NewLine);

var input = await PuzzleInput.GetInputLines();

var adapters = input.SkipLast(1).Select(s => int.Parse(s)).OrderBy(i => i).ToList();

adapters.Insert(0, 0);
adapters.Add(adapters.Last() + 3);

Console.WriteLine(string.Join(',', adapters)); Console.WriteLine();

foreach (var pair in adapters.Zip(adapters.Skip(1))) {
	Console.WriteLine($"{pair.First} + {pair.Second} ({pair.Second - pair.First})");
}

var groups = adapters.Zip(adapters.Skip(1)).GroupBy(pair => pair.Second - pair.First); Console.WriteLine();

Console.WriteLine($"part 1: {groups.Single(g => g.Key == 1).Count() * groups.Single(g => g.Key == 3).Count()}"); // part 1 is 2482

//Enumerable.Range(2, adapters.Count - 1).Sum(len => )

for (var len = 2; len < adapters.Count; len++) {
	var combs = new Combinations<int>(adapters, len);
	//combs.Where(c => )
}
