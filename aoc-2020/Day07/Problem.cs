namespace aoc_2020.Day07;

public partial class Problem
{
	public static ProblemMetadata2 Metadata { get; } = new(Main);

	public static (long, long) Main(string[] input)
	{
		var inputList   = input.ToList();
		var bdict       = new Dictionary<string, Bag>();
		var terminators = inputList.Where(i => i.EndsWith("contain no other bags.")).ToList();

		// get all the bags which contain no bags
		foreach (var terminator in terminators) {
			var type = terminator[0..^28];
			bdict.Add(type, new Bag(type, new List<Content>()));
			inputList.Remove(terminator);
		}

		// then parse everything else
		var pinput = inputList.Select(ParseBag).ToList();

		// and create the structure
		while (pinput.Count > 0) {
			var bag = pinput.First(i => i.contents.All(c => bdict.ContainsKey(c.type)));
			pinput.Remove(bag);
			bdict.Add(bag.type, new Bag(bag.type, bag.contents.Select(c => new Content(bdict[c.type], c.count)).ToList()));
		}

		var part1 = bdict.Values.Count(b => CanContain(b, bdict["shiny gold"]));
		var part2 = CountContents(bdict["shiny gold"]);

		//Console.WriteLine($"part 1: {part1}"); // part 1 is 211
		//Console.WriteLine($"part 2: {part2}"); // part 2 is 12414

		return (part1, part2);
	}

	private static bool CanContain(Bag outer, Bag inner) => outer.Held.Any(e => e.Bag.Type == inner.Type || CanContain(e.Bag, inner));

	private static long CountContents(Bag outer) => outer.Held.Sum(h => CountContents(h.Bag) * h.Count + h.Count);

	private static (string type, IEnumerable<(string type, int count)> contents) ParseBag(string line)
	{
		var o = OuterRegex().Match(line);
		var i = InnerRegex().Matches(line);

		return (o.Groups["outer"].Value, i.Select(m => (m.Groups["inner"].Value, int.Parse(m.Groups["num"].Value))).ToList());
	}

	internal readonly record struct Content(Bag Bag, int Count);

	internal readonly record struct Bag(string Type, List<Content> Held);

	[GeneratedRegex("(?<outer>.*) bags contain")]
	private static partial Regex OuterRegex();

	[GeneratedRegex("(?<num>\\d+) (?<inner>[\\w\\s]+) bag(s|)")]
	private static partial Regex InnerRegex();
}
