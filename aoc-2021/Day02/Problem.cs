namespace aoc_2021.Day02;

public class Problem
{
	public static (int part1, int part2) Main(string fileName)
	{
		var part_1_position = new Position();
		var part_2_position = new Position();
		var aim             = 0;

		foreach (var parts in ReadFileLines(fileName, l => l.Split(' '))) {
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

	[Fact(DisplayName = "Day 02 Sample Input")]
	public void SampleInputFunctionCorrectly()
	{
		var (p1, p2) = Main("input_sample.txt");

		Assert.Equal(150, p1);
		Assert.Equal(900, p2);
	}

	[Fact(DisplayName = "Day 02 Main Input")]
	public void MainInputFunctionCorrectly()
	{
		var (p1, p2) = Main("input.txt");

		Assert.Equal(   1660158, p1);
		Assert.Equal(1604592846, p2);
	}
}
