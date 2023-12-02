namespace aoc_2020.Day08;

public class Problem
{
	public static ProblemMetadata2 Metadata { get; } = new(Main);

	public static (long, long) Main(string[] input)
	{
		var program = input.Select(ParseInstruction).ToArray();
		var result  = RunProgram(program);
		var part1   = result.accumulator;

		//Console.WriteLine($"part 1: {part1}"); // part 1 is 1939

		// one of the jmp or nop in result.visited needs to be changed to cause it to halt
		foreach (var idx in result.visited.Where(v => program[v].instruction == "jmp" || program[v].instruction == "nop")) {
			var nprogram = program.Clone() as (string instruction, int argument)[] ?? throw new InvalidOperationException("Uh, it's null.");

			nprogram[idx] = program[idx].instruction switch {
				"jmp" => ("nop", program[idx].argument),
				"nop" => ("jmp", program[idx].argument),
				_ => throw new InvalidOperationException("Yeah, that's not an instruction we handle."),
			};

			var (_, accumulator, halted) = RunProgram(nprogram);

			if (halted) {
				var part2 = accumulator;
				//Console.WriteLine($"part 2: {part2}"); // part 2 is 2212
				return (part1, part2);
			}
		}

		throw new InvalidOperationException("The program didn't halt.");
	}

	private static (string instruction, int argument) ParseInstruction(string input)
	{
		var parts = input.Split(' ');
		return (parts[0], int.Parse(parts[1]));
	}

	private static (IEnumerable<int> visited, int accumulator, bool halted) RunProgram((string instruction, int argument)[] program)
	{
		var ip      = 0;
		var ax      = 0;
		var visited = new HashSet<int>();

		while (ip < program.Length) {
			if (!visited.Add(ip)) {
				return (visited, ax, false);
			}

			switch (program[ip].instruction) {
				case "nop":
					ip++;
					break;

				case "jmp":
					ip += program[ip].argument;
					break;

				case "acc":
					ax += program[ip].argument;
					ip++;
					break;
			}
		}

		return (visited, ax, true);
	}
}
