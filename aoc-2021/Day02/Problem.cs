namespace AdventOfCode.Year2021.Day02;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long part1, long part2) Execute(string[] input)
	{
		var part_1_position = new Position();
		var part_2_position = new Position();
		var aim             = 0;

		foreach (var parts in input.Select(l => l.Split(' '))) {
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

		return (part_1_position.X * part_1_position.Depth, part_2_position.X * part_2_position.Depth);
	}

	[Test]
	[DisplayName("Day 02 Sample Input")]
	public async Task SampleInputFunctionCorrectly()
	{
		var (p1, p2) = Execute(ReadFileLines("input_sample.txt"));

		await Assert.That(p1).IsEqualTo(150);
		await Assert.That(p2).IsEqualTo(900);
	}

	[Test]
	[DisplayName("Day 02 Main Input")]
	public async Task MainInputFunctionCorrectly()
	{
		var (p1, p2) = Execute(ReadFileLines("input.txt"));

		await Assert.That(p1).IsEqualTo(1660158);
		await Assert.That(p2).IsEqualTo(1604592846);
	}
}
