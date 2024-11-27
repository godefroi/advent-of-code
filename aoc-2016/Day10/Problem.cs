namespace aoc_2016.Day10;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Main);

	public static (long, long) Main(string[] input)
	{
		var bots      = new Dictionary<int, Bot>();
		var transfers = new Queue<(int Value, int To)>();
		var outputs   = new Dictionary<int, int>();
		var part1     = -1;
		var part2     = -1;

		foreach (var instr in input.Select(Parse)) {
			if (instr.FromBot > -1) {
				bots.Add(instr.FromBot, new Bot(instr.LowBot, instr.HighBot, instr.OutputLow, instr.OutputHigh));
			} else {
				transfers.Enqueue((instr.Value, instr.ToBot));
			}
		}

		while (transfers.Count > 0) {
			var (val, to) = transfers.Dequeue();
			var bot       = bots[to];

			bot.Receive(val);

			if (bot.HasBoth) {
				if (part1 == -1 && bot.LowValue == 17 && bot.HighValue == 61) {
					part1 = to;
				}

				if (bot.OutputLow) {
					outputs[bot.LowTo] = bot.LowValue;
				} else {
					transfers.Enqueue((bot.LowValue, bot.LowTo));
				}

				if (bot.OutputHigh) {
					outputs[bot.HighTo] = bot.HighValue;
				} else {
					transfers.Enqueue((bot.HighValue, bot.HighTo));
				}

				bot.Clear();
			}

			if (part2 == -1 && outputs.TryGetValue(0, out int o0) && outputs.TryGetValue(1, out int o1) && outputs.TryGetValue(2, out int o2)) {
				part2 = o0 * o1 * o2;
			}

			if (part1 > -1 && part2 > -1) {
				break;
			}
		}

		return (part1, part2);
	}

	private static Instruction Parse(string instruction)
	{
		// bot 37 gives low to bot 114 and high to bot 150
		// value 2 goes to bot 156
		Span<Range> ranges = stackalloc Range[13];

		var data = instruction.AsSpan();

		return data.Split(ranges, ' ') switch {
			12 => new Instruction(int.Parse(data[ranges[1]]), int.Parse(data[ranges[6]]), int.Parse(data[ranges[11]]), -1, -1, data[ranges[5]][0] == 'o', data[ranges[10]][0] == 'o'),
			6 => new Instruction(-1, -1, -1, int.Parse(data[ranges[1]]), int.Parse(data[ranges[5]]), false, false),
			_ => throw new InvalidOperationException($"Unrecognized instruction ({data.Split(ranges, ' ')}): {instruction}"),
		};
	}

	private readonly record struct Instruction(int FromBot, int LowBot, int HighBot, int Value, int ToBot, bool OutputLow, bool OutputHigh);

	private class Bot(int lowTo, int highTo, bool outputLow, bool outputHigh)
	{
		private List<int> _values = new(2);

		public int LowValue => Math.Min(_values[0], _values[1]);

		public int HighValue => Math.Max(_values[0], _values[1]);

		public int LowTo { get; } = lowTo;

		public int HighTo { get; } = highTo;

		public bool OutputLow { get; } = outputLow;

		public bool OutputHigh { get; } = outputHigh;

		public bool HasBoth => _values.Count == 2;

		public void Receive(int value)
		{
			if (_values.Count == 2) {
				throw new InvalidOperationException("Cannot receive when already holding two");
			}

			_values.Add(value);
		}

		public void Clear() => _values.Clear();
	}
}
