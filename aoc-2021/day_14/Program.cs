using aoc_tools;

//var input = @"NNCB

//CH -> B
//HH -> N
//CB -> H
//NH -> C
//HB -> C
//HC -> B
//HN -> C
//NN -> C
//BH -> H
//NC -> B
//NB -> B
//BN -> B
//BB -> N
//BC -> B
//CC -> N
//CN -> C
//".Split(Environment.NewLine).SkipLast(1).ToList();

var input = (await PuzzleInput.GetInputLines()).SkipLast(1).ToList();

var polymer = input[0].ToCharArray().ToList();
var rules   = input.Skip(2).ToDictionary(r => (First: r[0], Second: r[1]), r => r[6]);

for (var i = 0; i < 40; i++) {
	for (var p = polymer.Count - 1; p > 0; p--) {
		//Console.WriteLine($"{polymer[p - 1]}{polymer[p]}");
		if (rules.TryGetValue((First: polymer[p - 1], Second: polymer[p]), out var insert)) {
			polymer.Insert(p, insert);
		}
		//Console.WriteLine(new String(polymer.ToArray()));
	}
	//break;
	Console.WriteLine($"finished step {i} with a chain of length {polymer.Count}");
}

/*Console.WriteLine(polymer.Count);

foreach (var g in polymer.GroupBy(c => c)) {
	Console.WriteLine($"{g.Key} -> {g.Count()}");
}*/

var groups = polymer.GroupBy(c => c);

Console.WriteLine($"part 1: {groups.Max(g => g.LongCount()) - groups.Min(g => g.LongCount())}"); // part 1 is 2233

record struct Rule(char First, char Second, char Insert);