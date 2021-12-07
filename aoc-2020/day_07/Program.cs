using System.Text.RegularExpressions;

using aoc_tools;

//var input = @"light red bags contain 1 bright white bag, 2 muted yellow bags.
//dark orange bags contain 3 bright white bags, 4 muted yellow bags.
//bright white bags contain 1 shiny gold bag.
//muted yellow bags contain 2 shiny gold bags, 9 faded blue bags.
//shiny gold bags contain 1 dark olive bag, 2 vibrant plum bags.
//dark olive bags contain 3 faded blue bags, 4 dotted black bags.
//vibrant plum bags contain 5 faded blue bags, 6 dotted black bags.
//faded blue bags contain no other bags.
//dotted black bags contain no other bags.
//".Split(Environment.NewLine).SkipLast(1).ToList();

var input       = (await PuzzleInput.GetInputLines()).SkipLast(1).ToList();
var bdict       = new Dictionary<string, Bag>();
var terminators = input.Where(i => i.EndsWith("contain no other bags.")).ToList();

// get all the bags which contain no bags
foreach (var terminator in terminators) {
	var type = terminator[0..^28];
	bdict.Add(type, new Bag(type, new List<Content>()));
	input.Remove(terminator);
}

// then parse everything else
var pinput = input.Select(i => ParseBag(i)).ToList();

// and create the structure
while (pinput.Count > 0) {
	var bag = pinput.First(i => i.contents.All(c => bdict.ContainsKey(c.type)));
	pinput.Remove(bag);
	bdict.Add(bag.type, new Bag(bag.type, bag.contents.Select(c => new Content(bdict[c.type], c.count)).ToList()));
}

Console.WriteLine($"part 1: {bdict.Values.Count(b => CanContain(b, bdict["shiny gold"]))}"); // part 1 is 211
Console.WriteLine($"part 2: {CountContents(bdict["shiny gold"])}"); // part 2 is 12414

bool CanContain(Bag outer, Bag inner) => outer.Held.Any(e => e.Bag.Type == inner.Type || CanContain(e.Bag, inner));

long CountContents(Bag outer) => outer.Held.Sum(h => (CountContents(h.Bag) * h.Count) + h.Count);

(string type, IEnumerable<(string type, int count)> contents) ParseBag(string line)
{
	var o = Regex.Match(line, @"(?<outer>.*) bags contain");
	var i = Regex.Matches(line, @"(?<num>\d+) (?<inner>[\w\s]+) bag(s|)");

	return (o.Groups["outer"].Value, i.Select(m => (m.Groups["inner"].Value, int.Parse(m.Groups["num"].Value))).ToList());
}

internal record class Content(Bag Bag, int Count);

internal record class Bag(string Type, List<Content> Held);
