namespace AdventOfCode.Year2019.Day09;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Main);

	public static (long, long) Main(string[] input)
	{
		var program  = input.Single();
		var computer = new IntcodeComputer(program);
		var part1    = default(long);
		var part2    = default(long);

		computer.Input += (s, e) => 1;
		computer.Output += (s, e) => part1 = e.OutputValue;
		computer.Resume();

		computer = new IntcodeComputer(program);

		computer.Input += (s, e) => 2;
		computer.Output += (s, e) => part2 = e.OutputValue;
		computer.Resume();

		return (part1, part2);
	}

	[Fact]
	public void ProgramCopiesItself()
	{
		var program  = "109,1,204,-1,1001,100,1,100,1008,100,16,101,1006,101,0,99".Split(',').Select(long.Parse).ToList();
		var computer = new IntcodeComputer(program);
		var ptr      = 0;

		computer.Output += (s, e) => Assert.Equal(program[ptr++], e.OutputValue);

		computer.Resume();

		Assert.Equal(ptr, program.Count);
	}

	[Fact]
	public void ProgramProduces16DigitNumber()
	{
		var computer = new IntcodeComputer("1102,34915192,34915192,7,4,7,99,0");
		var output   = default(string);

		computer.Output += (s, e) => output = e.OutputValue.ToString();

		computer.Resume();

		Assert.NotNull(output);
		Assert.Equal(16, output.Length);
	}


	[Fact]
	public void ProgramOutputsCorrectNumber()
	{
		var program  = "104,1125899906842624,99".Split(',').Select(long.Parse).ToList();
		var computer = new IntcodeComputer(program);
		var output   = default(long?);

		computer.Output += (s, e) => output = e.OutputValue;

		computer.Resume();

		Assert.NotNull(output);
		Assert.Equal(program[1], output);
	}
}
