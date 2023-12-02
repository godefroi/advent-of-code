namespace aoc_2019.Day14;

public class Problem
{
	public static ProblemMetadata2 Metadata { get; } = new(Main);

	public static (long, long) Main(string[] input)
	{
		return (Part1(input), Part2(input));
	}

	private static long Part1(string[] input)
	{
		//Console.WriteLine(Math.Ceiling(2f / 4f)); // need/make, should be 1
		//Console.WriteLine(Math.Ceiling(4f / 2f)); // need/make, should be 2
		//Console.WriteLine(Math.Ceiling(4f / 3f)); // need/make, should be 2
		//Console.WriteLine(Math.Ceiling(1f / 1f)); // need/make, should be 1
		//Console.WriteLine(Math.Ceiling(9f / 1f)); // need/make, should be 9
		//Console.WriteLine(Math.Ceiling(1f / 9f)); // need/make, should be 1
		//return;

		//var reactions = ParseInput(@"10 ORE => 10 A
		//	1 ORE => 1 B
		//	7 A, 1 B => 1 C
		//	7 A, 1 C => 1 D
		//	7 A, 1 D => 1 E
		//	7 A, 1 E => 1 FUEL");

		//var reactions = ParseInput(@"9 ORE => 2 A
		//	8 ORE => 3 B
		//	7 ORE => 5 C
		//	3 A, 4 B => 1 AB
		//	5 B, 7 C => 1 BC
		//	4 C, 1 A => 1 CA
		//	2 AB, 3 BC, 4 CA => 1 FUEL");

		var reactions = ParseInput(input);

		var tanks = new Dictionary<string, long>(reactions.Keys.Select(k => new KeyValuePair<string, long>(k, 0)));

		var ore = 0L;

		PerformReaction(reactions, tanks, "FUEL", 1, ref ore);

		return ore;
	}

	private static long Part2(string[] input)
	{
		var target    = 1000000000000;
		var mag       = 0;
		var reactions = ParseInput(input);

		// first, find the order of magnitude
		while (true) {
			//Console.WriteLine(++mag)
			var val = (long)Math.Pow(10, ++mag);

			Console.WriteLine(val);

			if (FindOreForFuel(reactions, val) > target) {
				break;
			}
		}

		var high = (long)Math.Pow(10, mag);
		var low  = (long)Math.Pow(10, mag - 1);

		// then binary search
		while (true) {
			var middle = ((high - low) / 2) + low;

			if (middle == high || middle == low) {
				break;
			}

			Console.WriteLine($"b {middle}");

			if (FindOreForFuel(reactions, middle) > target) {
				high = middle;
			} else {
				low = middle;
			}
		}

		var cur = high;

		while (FindOreForFuel(reactions, cur) > target) {
			cur--;
		}

		return cur;
	}

	private static long FindOreForFuel(Dictionary<string, Reaction> reactions, long fuel)
	{
		var tanks = new Dictionary<string, long>(reactions.Keys.Select(k => new KeyValuePair<string, long>(k, 0)));
		var ore = 0L;

		PerformReaction(reactions, tanks, "FUEL", fuel, ref ore);

		return ore;
	}

	private static void PerformReaction(Dictionary<string, Reaction> reactions, Dictionary<string, long> tanks, string substance, long need, ref long ore)
	{
		if( substance == "ORE" )
			throw new InvalidOperationException("It was ore, dummy");

		if( tanks[substance] >= need )
			return;

		if( tanks[substance] > 0 )
			need -= tanks[substance];

		var react_cnt = (long)Math.Ceiling((double)need / (double)reactions[substance].Quantity);

		foreach( var p in reactions[substance].Precursors ) {
			if( p.Substance == "ORE" ) {
				ore += p.Quantity * react_cnt;
			} else {
				PerformReaction(reactions, tanks, p.Substance, p.Quantity * react_cnt, ref ore);

				// remove the precursor from the tank
				tanks[p.Substance] -= p.Quantity * react_cnt;
			}
		}

		// add the result to the tank
		tanks[substance] += reactions[substance].Quantity * react_cnt;
	}

	private static Dictionary<string, Reaction> ParseInput(string[] input)
	{
		/*
		10 ORE => 10 A
		1 ORE => 1 B
		7 A, 1 B => 1 C
		7 A, 1 C => 1 D
		7 A, 1 D => 1 E
		7 A, 1 E => 1 FUEL
		*/
		var reactions = new Dictionary<string, Reaction>();

		foreach (var parts in input.Select(s => s.Split(" => "))) {
			if (parts.Length != 2) {
				throw new InvalidOperationException("Didn't get expected input");
			}

			var bits = parts[1].Split(' ');

			if (reactions.ContainsKey(bits[1])) {
				throw new InvalidOperationException("We already have a reaction for this output");
			}

			var node = new Reaction(bits[1], long.Parse(bits[0]));

			foreach (var p in parts[0].Split(", ")) {
				bits = p.Split(' ');
				node.Precursors.Add((Substance: bits[1], Quantity: long.Parse(bits[0])));
			}

			reactions.Add(node.Substance, node);
		}

		return reactions;
	}

	private class Reaction
	{
		public Reaction(string substance, long quantity)
		{
			Substance = substance;
			Quantity  = quantity;
		}

		public string Substance { get; }

		public long Quantity { get; }

		public List<(string Substance, long Quantity)> Precursors { get; } = new List<(string Substance, long Quantity)>();
	}
}
