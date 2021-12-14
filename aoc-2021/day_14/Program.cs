using aoc_tools;

using System.Collections.Generic;

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

var input   = (await PuzzleInput.GetInputLines()).SkipLast(1).ToList();
var polymer = input[0].ToList();
var rules   = input.Skip(2).ToDictionary(r => (First: r[0], Second: r[1]), r => r[6]);
var ccounts = polymer.GroupBy(c => c).ToDictionary(g => g.Key, g => g.LongCount());
var pcounts = polymer.Zip(polymer.Skip(1)).ToDictionary(p => (First: p.First, Second: p.Second), p => 1L);

foreach (var rule in rules) {
	if (!pcounts.ContainsKey(rule.Key)) {
		pcounts.Add(rule.Key, 0L);
	}
}

foreach (var rule in rules) {
	if (!ccounts.ContainsKey(rule.Key.First)) {
		ccounts.Add(rule.Key.First, 0L);
	}
	if (!ccounts.ContainsKey(rule.Key.Second)) {
		ccounts.Add(rule.Key.Second, 0L);
	}
}

//Console.WriteLine($"part 1: {groups.Max(g => g.LongCount()) - groups.Min(g => g.LongCount())}"); // part 1 is 2233

for (var i = 0; i < 40; i++) {
	var ocounts = pcounts.ToDictionary(p => p.Key, p => p.Value);
	
	foreach (var r in rules) {
		var count = ocounts[r.Key];
		pcounts[r.Key] -= count;
		pcounts[(First: r.Key.First, Second: r.Value)]  += count;
		pcounts[(First: r.Value, Second: r.Key.Second)] += count;
		ccounts[r.Value] += count;
	}
}

Console.WriteLine($"part 2: {ccounts.Max(kvp => kvp.Value) - ccounts.Min(kvp => kvp.Value)}"); // part 2 is 2884513602164
