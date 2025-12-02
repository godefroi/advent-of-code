namespace AdventOfCode.Year2022.Day05;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (string, string) Execute(string[] input)
	{
		var (stacks, instructionLines) = Parse(input);
		var instructions = instructionLines.Select(ParseInstruction);

		foreach (var instruction in instructions) {
			for (var i = 0; i < instruction.Count; i++) {
				stacks[instruction.To].Push(stacks[instruction.From].Pop());
			}
		}

		var part1 = new string(stacks.Select(s => s.Peek()).ToArray());

		(stacks, instructionLines) = Parse(input);
		instructions = instructionLines.Select(ParseInstruction);

		foreach (var (from, to, count) in instructions) {
			var tmp = new char[count];

			for (var i = 0; i < count; i++) {
				tmp[i] = stacks[from].Pop();
			}

			for (var i = count - 1; i >= 0; i--) {
				stacks[to].Push(tmp[i]);
			}
		}

		var part2 = new string(stacks.Select(s => s.Peek()).ToArray());

		return (part1, part2);
	}

	private static (Stack<char>[] Stacks, IEnumerable<string> Instructions) Parse(string[] lines)
	{
		var divider    = Array.IndexOf(lines, string.Empty);
		var stackCount = lines[divider - 1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
		var stacks     = new Stack<char>[stackCount];

		// instantiate the stacks
		for (var i = 0; i < stackCount; i++) {
			stacks[i] = new Stack<char>();
		}

		// parse the stacks
		for (var i = divider - 2; i >= 0; i--) {
			for (var s = 0; s < stackCount; s++) {
				var c = lines[i][(s * 4) + 1];

				if (c != ' ') {
					stacks[s].Push(c);
				}
			}
		}

		return (stacks, lines.Skip(divider + 1));
	}

	private static (int From, int To, int Count) ParseInstruction(string instruction)
	{
		var parts = instruction.Split(' ');

		return (int.Parse(parts[3]) - 1, int.Parse(parts[5]) - 1, int.Parse(parts[1]));
	}

	[Test]
	public async Task SampleParsesCorrectly()
	{
		var (stacks, instructions) = Parse(ReadFileLines("inputSample.txt"));

		await Assert.That(stacks.Select(s => s.AsEnumerable())).IsEquivalentTo(new IEnumerable<char>[] {
			['N', 'Z'],
			['D', 'C', 'M'],
			['P']
		});

		await Assert.That(instructions).IsEquivalentTo([
			"move 1 from 2 to 1",
			"move 3 from 1 to 3",
			"move 2 from 2 to 1",
			"move 1 from 1 to 2",
		]);
	}

	[Test]
	public async Task SampleInstructionsParseCorrectly()
	{
		await Assert.That(Parse(ReadFileLines("inputSample.txt")).Instructions.Select(ParseInstruction)).IsEquivalentTo([
			(1, 0, 1),
			(0, 2, 3),
			(1, 0, 2),
			(0, 1, 1)
		]);
	}
}
