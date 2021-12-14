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

var input   = (await PuzzleInput.GetInputLines()).SkipLast(1).ToList();
var cmap    = input.Skip(2).SelectMany(l => new[] { l[0], l[1], l[6] }).Distinct().ToList();
var polymer = input[0].Select(c => cmap.IndexOf(c)).ToList();
var counts  = new long[cmap.Count];
var rules   = new int[cmap.Count, cmap.Count];

foreach (var line in input.Skip(2)) {
	rules[cmap.IndexOf(line[0]), cmap.IndexOf(line[1])] = cmap.IndexOf(line[6]);
}

//Console.WriteLine($"part 1: {groups.Max(g => g.LongCount()) - groups.Min(g => g.LongCount())}"); // part 1 is 2233

var sw = System.Diagnostics.Stopwatch.StartNew();

try {
	foreach (var c in Expand(polymer, rules, 22, 0)) {
		counts[c]++;
	}

	Console.WriteLine(counts.Max() - counts.Min());
} finally {
	Console.WriteLine($"Terminated after {sw.Elapsed}");
}

static IEnumerable<int> Expand(IEnumerable<int> input, int[,] rules, int maxIterations, int iteration = 1)
{
	if (iteration >= maxIterations) {
		foreach (var c in input) {
			yield return c;
			Console.WriteLine($"Yielded {c} from input");
		}

		yield break;
	}

	var prev = -1;

	foreach (var cur in Expand(input, rules, maxIterations, iteration + 1)) {
		if (prev == -1) {
			yield return cur;
			prev = cur;
			continue;
		}

		yield return rules[prev, cur];

		yield return cur;

		prev = cur;
	}
}
