namespace AdventOfCode.Year2025.Day01;

public class Problem
{
	private const int _dialCount = 100;

	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		// TODO: this is brute-force, and could be *massively* improved performance-wise.

		var cur = 50;
		var p1  = 0;
		var p2  = 0;

		foreach (var num in input.Select(ParseRotation)) {
			if (num < 0) {
				for (var i = 0; i < Math.Abs(num); i++) {
					cur--;

					if (cur == 0) {
						p2++;
					} else if (cur == -1) {
						cur = _dialCount - 1;
					}
				}
			} else if (num > 0) {
				for (var i = 0; i < num; i++) {
					cur++;

					if (cur >= _dialCount) {
						p2++;
						cur = 0;
					}
				}
			} else {
				throw new InvalidOperationException("Cannot rotate zero clicks");
			}

			if (cur == 0) {
				p1++;
			}

			//Console.WriteLine($"{cur},{num},{p1},{p2}");
		}

		return (p1, p2);
	}

	private static int ParseRotation(string line)
	{
		if (!int.TryParse(line[1..], out var num)) {
			throw new InvalidOperationException("Unable to parse input line");
		}

		return line[0] switch {
			'L'	=> -num,
			'R' => num,
			_ => throw new InvalidOperationException("Invalid rotation direction"),
		};
	}

	[Test]
	public async Task SampleWorks()
	{
		var lines = new[] {
			"L68",
			"L30",
			"R48",
			"L5",
			"R60",
			"L55",
			"L1",
			"L99",
			"R14",
			"L82",
		};

		await Assert.That(Execute(lines)).IsEqualTo((3, 6));
	}

	[Test]
	[Arguments(new[] {"L50"}, 1)]
	[Arguments(new[] {"R50"}, 1)]
	[Arguments(new[] {"L100"}, 1)]
	[Arguments(new[] {"R100"}, 1)]
	[Arguments(new[] {"R1000"}, 10)]
	[Arguments(new[] {"L50","R50"}, 1)]
	[Arguments(new[] {"L50","L50"}, 1)]
	[Arguments(new[] {"R150","L100"}, 3)]
	[Arguments(new[] {"R50","L100"}, 2)]
	[Arguments(new[] {"R50","R100"}, 2)]
	[Arguments(new[] {"L25","R75"}, 1)]
	[Arguments(new[] {"L75","R75"}, 2)]
	[Arguments(new[] {"R100","L100"}, 2)]
	[Arguments(new[] {"L50","R100","L50"}, 2)]
	[Arguments(new[] {"R8","R28","R43","L29"}, 2)]
	[Arguments(new[] {"R20","L470"}, 5)]
	public async Task Part2Works(string[] inputs, int p2)
	{
		await Assert.That(Execute(inputs).Item2).IsEqualTo(p2);
	}
}
