using aoc_tools;

//var input = @"nop +0
//acc +1
//jmp +4
//acc +3
//jmp -3
//acc -99
//acc +1
//jmp -4
//acc +6
//".Split(Environment.NewLine).SkipLast(1);

var input   = (await PuzzleInput.GetInputLines()).SkipLast(1);
var program = input.Select(s => ParseInstruction(s)).ToList();
var result  = RunProgram(program);

Console.WriteLine($"part 1: {result.accumulator}"); // part 1 is 1939

// one of the jmp or nop in result.visited needs to be changed to cause it to halt
foreach (var idx in result.visited.Where(v => program[v].instruction == "jmp" || program[v].instruction == "nop")) {
	var nprogram = program.ToList();

	nprogram[idx] = program[idx].instruction switch	{
		"jmp" => ("nop", program[idx].argument),
		"nop" => ("jmp", program[idx].argument),
		_ => throw new InvalidOperationException("Yeah, that's not an instruction we handle."),
	};

	var (_, accumulator, halted) = RunProgram(nprogram);

	if (halted) {
		Console.WriteLine($"part 2: {accumulator}"); // part 2 is 2212
		return;
	}
}

(string instruction, int argument) ParseInstruction(string input)
{
	var parts = input.Split(' ');
	return (parts[0], int.Parse(parts[1]));
}

(IEnumerable<int> visited, int accumulator, bool halted) RunProgram(List<(string instruction, int argument)> program)
{
	var ip = 0;
	var ax = 0;
	var visited = new HashSet<int>();

	while (ip < program.Count) {
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