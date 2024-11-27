using CommunityToolkit.HighPerformance;

namespace aoc_2016.Day08;

public partial class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Main);

	public static (long, string) Main(string[] input)
	{
		var screen = new char[50, 6];

		screen.AsSpan2D().Fill(' ');

		foreach (var (type, x, y, amount) in input.Select(s => Parse(s))) {
			if (type == InstructionType.Rect) {
				for (var xc = 0; xc < x; xc++) {
					for (var yc = 0; yc < y; yc++) {
						screen[xc, yc] = '#';
					}
				}
			} else if (type == InstructionType.Rotate) {
				if (x > -1) {
					// rotate column 'x' by 'amount'
					for (var i = 0; i < amount; i++) {
						RotateColumn(screen, x);
					}
				} else if (y > -1) {
					// rotate row 'y' by 'amount'
					for (var i = 0; i < amount; i++) {
						RotateRow(screen, y);
					}
				} else {
					throw new NotSupportedException();
				}
			} else {
				throw new NotSupportedException();
			}
		}

		//PrintMap(screen);

		var pcnt = 0;

		foreach (var c in screen.AsSpan2D()) {
			if (c == '#') {
				pcnt++;
			}
		}

		// note that as of this writing, the OCR here doesn't work, because there's extra space after the letters on the screen
		return (pcnt, OCR.Recognize(screen));
	}

	private static void RotateColumn(char[,] screen, int col)
	{
		var rows = screen.GetLength(1);
		var tmp  = screen[col, rows - 1];

		for (var y = rows - 1; y > 0; y--) {
			screen[col, y] = screen[col, y - 1];
		}

		screen[col, 0] = tmp;
	}

	private static void RotateRow(char[,] screen, int row)
	{
		var cols = screen.GetLength(0);
		var tmp  = screen[cols - 1, row];

		for (var x = cols - 1; x > 0; x--) {
			screen[x, row] = screen[x - 1, row];
		}

		screen[0, row] = tmp;
	}

	/*
		rotate row y=0 by 6
		rotate column x=0 by 1
		rect 4x1
	*/
	private static Instruction Parse(ReadOnlySpan<char> input)
	{
		if (input.StartsWith("rect")) {
			Span<Range> ranges = stackalloc Range[2];

			input = input[5..];
			input.Split(ranges, 'x');

			return new Instruction(InstructionType.Rect, int.Parse(input[ranges[0]]), int.Parse(input[ranges[1]]), -1);
		} else if (input.StartsWith("rotate")) {
			// drop off the "rotate " part
			input = input[7..];

			var x = -1;
			var y = -1;

			if (input[0] == 'r' && input[4] == 'y') {
				input = input[6..];
				y     = int.Parse(input[..input.IndexOf(' ')]);
			} else if (input[0] == 'c' && input[7] == 'x') {
				input = input[9..];
				x     = int.Parse(input[..input.IndexOf(' ')]);
			} else {
				throw new InvalidOperationException("Invalid rotate instruction encountered");
			}

			return new Instruction(InstructionType.Rotate, x, y, int.Parse(input[input.LastIndexOf(' ')..]));
		} else {
			throw new InvalidOperationException("Invalid instruction encountered");
		}
	}

	public enum InstructionType { Rect, Rotate }

	public record struct Instruction(InstructionType Type, int X, int Y, int Amount);

	[Theory]
	[InlineData("rect 4x1", InstructionType.Rect, 4, 1, -1)]
	[InlineData("rotate row y=0 by 6", InstructionType.Rotate, -1, 0, 6)]
	[InlineData("rotate column x=0 by 1", InstructionType.Rotate, 0, -1, 1)]
	public void InstructionsParseCorrectly(string input, InstructionType type, int x, int y, int amount) =>
		Assert.Equal(new Instruction(type, x, y, amount), Parse(input));
}
