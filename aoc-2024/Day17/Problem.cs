namespace AdventOfCode.Year2024.Day17;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (string, long) Execute(string[] input)
	{
		var computer = Parse(input);

		computer.ExecuteProgram();

		return (string.Concat('[', string.Join(',', computer.Output), ']'), 0);
	}

	private static Computer Parse(string[] input)
	{
		var regA    = int.Parse(input[0].AsSpan()[12..]);
		var regB    = int.Parse(input[1].AsSpan()[12..]);
		var regC    = int.Parse(input[2].AsSpan()[12..]);
		var pSpan   = input[4].AsSpan()[9..];
		var program = new List<int>(pSpan.Count(',') + 1);

		foreach (var num in pSpan.Split(',')) {
			program.Add(int.Parse(pSpan[num]));
		}

		return new Computer(regA, regB, regC, program);
	}

	public class Computer(int registerA, int registerB, int registerC, List<int> program)
	{
		private int _regA = registerA;
		private int _regB = registerB;
		private int _regC = registerC;
		private int _ip = 0;
		private readonly List<int> _program = program;
		private readonly List<int> _output = [];

		public IReadOnlyList<int> Output => _output;

		public void ExecuteProgram()
		{
			var vtable = new Dictionary<OpCode, Action<int>>() {
				{ OpCode.adv, ExecuteAdv },
				{ OpCode.bxl, ExecuteBxl },
				{ OpCode.bst, ExecuteBst },
				{ OpCode.jnz, ExecuteJnz },
				{ OpCode.bxc, ExecuteBxc },
				{ OpCode.oup, ExecuteOut },
				{ OpCode.bdv, ExecuteBdv },
				{ OpCode.cdv, ExecuteCdv },
			};

			while (_ip < _program.Count) {
				var opCode  = (OpCode)_program[_ip++];
				var operand = _program[_ip++];

				vtable[opCode](GetOperandValue(opCode, operand));
			}
		}

		private void ExecuteAdv(int operand)
		{
			var numerator   = _regA;
			var denominator = Math.Pow(2, operand);

			_regA = (int)Math.Truncate(numerator / denominator);
		}

		private void ExecuteBxl(int operand)
		{
			_regB ^= operand;
		}

		private void ExecuteBst(int operand)
		{
			_regB = operand % 8;
		}

		private void ExecuteJnz(int operand)
		{
			if (_regA != 0) {
				_ip = operand;
			}
		}

		private void ExecuteBxc(int operand)
		{
			_regB ^= _regC;
		}

		private void ExecuteOut(int operand)
		{
			_output.Add(operand % 8);
		}

		private void ExecuteBdv(int operand)
		{
			var numerator   = _regA;
			var denominator = Math.Pow(2, operand);

			_regB = (int)Math.Truncate(numerator / denominator);
		}

		private void ExecuteCdv(int operand)
		{
			var numerator   = _regA;
			var denominator = Math.Pow(2, operand);

			_regC = (int)Math.Truncate(numerator / denominator);
		}

		private int GetOperandValue(OpCode opCode, int operand)
		{
			// 0-3 are always literal
			if (operand >= 0 && operand <= 3) {
				return operand;
			}

			if (opCode == OpCode.bxl || opCode == OpCode.jnz || opCode == OpCode.bxc ) {
				// literal
				return operand;
			} else {
				// combo
				return operand switch {
					4 => _regA,
					5 => _regB,
					6 => _regC,
					_ => throw new InvalidOperationException($"Unrecognized combo operand: {operand}"),
				};
			}
		}

		public class InstructionTests
		{
			[Test]
			public async Task BstWorksCorrectly()
			{
				var c = new Computer(0, 0, 9, [2, 6]);

				c.ExecuteProgram();

				await Assert.That(c._regB).IsEqualTo(1);
			}

			[Test]
			public async Task OutWorksCorrectly()
			{
				var c = new Computer(10, 0, 0, [5, 0, 5, 1, 5, 4]);

				c.ExecuteProgram();

				await Assert.That(c._output).IsEquivalentTo([0, 1, 2]);
			}

			[Test]
			public async Task SimpleProgramWorksCorrectly()
			{
				var c = new Computer(2024, 0, 0, [0, 1, 5, 4, 3, 0]);

				c.ExecuteProgram();

				await Assert.That(c._output).IsEquivalentTo([4, 2, 5, 6, 7, 7, 7, 7, 3, 1, 0]);

				await Assert.That(c._regA).IsEqualTo(0);
			}

			[Test]
			public async Task BxlWorksCorrectly()
			{
				var c = new Computer(0, 29, 0, [1, 7]);

				c.ExecuteProgram();

				await Assert.That(c._regB).IsEqualTo(26);
			}

			[Test]
			public async Task BxcWorksCorrectly()
			{
				var c = new Computer(0, 2024, 43690, [4, 0]);

				c.ExecuteProgram();

				await Assert.That(c._regB).IsEqualTo(44354);
			}
		}
	}

	public enum OpCode
	{
		adv = 0,
		bxl = 1,
		bst = 2,
		jnz = 3,
		bxc = 4,
		oup = 5,
		bdv = 6,
		cdv = 7,
	}
}
