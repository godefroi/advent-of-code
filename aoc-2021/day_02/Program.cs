using day_02;

using var sr = new StringReader(File.ReadAllText(args[0]));

var part_1_position = new Position();
var part_2_position = new Position();
var aim             = 0;

while (sr.Peek() > -1) {
	var parts = sr.ReadLine().Split(' ');
	var value = int.Parse(parts[1]);

	switch (parts[0]) {
		case "forward":
			part_1_position.X += value;
			part_2_position.X += value;
			part_2_position.Depth += aim * value;
			break;
		case "up":
			part_1_position.Depth -= value;
			aim -= value;
			break;

		case "down":
			part_1_position.Depth += value;
			aim += value;
			break;

		default:
			throw new NotImplementedException($"Instruction {parts[0]} is unsupported.");
	}
}

Console.WriteLine($"part 1: {part_1_position.X * part_1_position.Depth}");
Console.WriteLine($"part 2: {part_2_position.X * part_2_position.Depth}");
