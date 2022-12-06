using aoc_2019.Intcode;

namespace aoc_2019.Day02;

public class Problem
{
	public static (long?, long) Main(string fileName)
	{
		var originalProgram = ReadFileLines(fileName).Single().Split(',').Select(long.Parse).ToArray();
		var program         = (long[])originalProgram.Clone();

		program[1] = 12;
		program[2] = 2;

		var computer = new IntcodeComputer(program);

		computer.Resume();

		var part1 = computer.GetMemoryValue(0);

		for (var noun = 0; noun < 100; noun++) {
			for (var verb = 0; verb < 100; verb++) {
				program = (long[])originalProgram.Clone();

				program[1] = noun;
				program[2] = verb;

				computer = new IntcodeComputer(program);
				computer.Resume();

				if (computer.GetMemoryValue(0) == 19690720) {
					return (part1, (noun * 100) + verb);
				}
			}
		}

		throw new Exception("didn't find the correct noun/verb combo");
	}

	[Theory]
	[InlineData("1,9,10,3,2,3,11,0,99,30,40,50", "[3500,9,10,70,2,3,11,0,99,30,40,50]")]
	[InlineData("1,0,0,0,99", "[2,0,0,0,99]")]
	[InlineData("2,3,0,3,99", "[2,3,0,6,99]")]
	[InlineData("2,4,4,5,99,0", "[2,4,4,5,99,9801]")]
	[InlineData("1,1,1,4,99,5,6,0,99", "[30,1,1,4,2,5,6,0,99]")]
	public void SampleProgramsExecuteCorrectly(string program, string output)
	{
		var computer = new IntcodeComputer(program);

		Assert.Equal(IntcodeComputer.InterruptType.Terminated, computer.Resume());
		Assert.Equal(output, computer.ToString());
	}
}
