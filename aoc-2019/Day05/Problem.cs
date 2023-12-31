namespace aoc_2019.Day05;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Main);

	public static (long, long) Main(string[] input)
	{
		var originalProgram = input.Single().Split(',').Select(long.Parse).ToArray();
		var computer        = new IntcodeComputer(originalProgram);

		long part1 = default, part2 = default;

		computer.Input += (s, e) => { Console.WriteLine("input (1)"); return 1; };
		computer.Output += (s, e) => { Console.WriteLine($"output -> {e.OutputValue}"); part1 = e.OutputValue; };

		computer.Resume();

		computer = new IntcodeComputer(originalProgram);

		computer.Input += (s, e) => { Console.WriteLine("input (5)"); return 5; };
		computer.Output += (s, e) => { Console.WriteLine($"output -> {e.OutputValue}"); part2 = e.OutputValue; };

		computer.Resume();

		return (part1, part2);
	}

	[Theory]
	[InlineData("3,9,8,9,10,9,4,9,99,-1,8", 7, 0)] // equal to 8, position mode
	[InlineData("3,9,8,9,10,9,4,9,99,-1,8", 8, 1)]
	[InlineData("3,9,8,9,10,9,4,9,99,-1,8", 9, 0)]
	[InlineData("3,9,7,9,10,9,4,9,99,-1,8", 7, 1)] // less than 8, position mode
	[InlineData("3,9,7,9,10,9,4,9,99,-1,8", 8, 0)]
	[InlineData("3,9,7,9,10,9,4,9,99,-1,8", 9, 0)]
	[InlineData("3,3,1108,-1,8,3,4,3,99",   7, 0)] // equal to 8, immediate mode
	[InlineData("3,3,1108,-1,8,3,4,3,99",   8, 1)]
	[InlineData("3,3,1108,-1,8,3,4,3,99",   9, 0)]
	[InlineData("3,3,1107,-1,8,3,4,3,99",   7, 1)] // less than 8, immediate mode
	[InlineData("3,3,1107,-1,8,3,4,3,99",   8, 0)]
	[InlineData("3,3,1107,-1,8,3,4,3,99",   9, 0)]
	[InlineData("3,12,6,12,15,1,13,14,13,4,13,99,-1,0,1,9", 0,  0)] // jump, position mode
	[InlineData("3,12,6,12,15,1,13,14,13,4,13,99,-1,0,1,9", 10, 1)]
	[InlineData("3,3,1105,-1,9,1101,0,0,12,4,12,99,1",      0,  0)] // jump, immediate mode
	[InlineData("3,3,1105,-1,9,1101,0,0,12,4,12,99,1",      10, 1)]
	[InlineData("3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99", 7,  999)]
	[InlineData("3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99", 8, 1000)]
	[InlineData("3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99", 9, 1001)]
	public void OpcodesOperateAsExpected(string programString, long input, long expectedOutput)
	{
		var computer    = new IntcodeComputer(programString);
		var outputValue = default(long?);

		computer.Input += (s, e) => input;
		computer.Output += (s, e) => outputValue = e.OutputValue;

		computer.Resume();

		Assert.NotNull(outputValue);
		Assert.Equal(expectedOutput, outputValue);
	}
}
