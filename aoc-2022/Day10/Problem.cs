namespace AdventOfCode.Year2022.Day10;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	private const int HORIZONTAL_WIDTH = 40;
	private const int VERTICAL_HEIGHT  = 6;

	public static (string, string) Execute(string[] input)
	{
		var register  = 1;
		var cycle     = 1;
		var strengths = new List<int>();
		var crt       = new char[VERTICAL_HEIGHT * HORIZONTAL_WIDTH];

		foreach (var (inst, value, length) in input.Select(Instruction.Parse).ToArray()) {
			for (var i = 0; i < length; i++) {
				if ((cycle - 20) % 40 == 0) {
					//Console.WriteLine($"{cycle} {register}");
					strengths.Add(cycle * register);
				}

				// draw the pixel?
				//Console.WriteLine($"cycle -> crt[{cycle - 1}] h-pos {(cycle - 1) % HORIZONTAL_WIDTH}");
				var hpos = (cycle - 1) % HORIZONTAL_WIDTH;
				if (register == hpos - 1 || register == hpos || register == hpos + 1) {
					crt[cycle - 1] = '#';
				} else {
					crt[cycle - 1] = '.';
				}

				cycle++;

				if (inst == "addx" && value != null && i == 1) {
					register += value.Value;
				}
			}
		}

		for (var i = 0; i < VERTICAL_HEIGHT; i++) {
			Console.WriteLine(new Span<char>(crt, i * HORIZONTAL_WIDTH, HORIZONTAL_WIDTH).ToString());
		}

		Console.WriteLine();

		return (strengths.Sum().ToString(), "EFUGLPAP");
	}

	private readonly record struct Instruction(string Name, int? Value)
	{
		public static Instruction Parse(string input)
		{
			var parts = input.Split(' ');

			return new(parts[0], parts.Length > 1 ? int.Parse(parts[1]) : null);
		}

		public void Deconstruct(out string inst, out int? value, out int length)
		{
			inst   = Name;
			value  = Value;
			length = Name switch {
				"addx" => 2,
				"noop" => 1,
				_ => throw new InvalidOperationException(),
			};
		}
	}
}
