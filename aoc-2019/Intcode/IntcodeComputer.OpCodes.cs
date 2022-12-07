using System.Reflection;

namespace aoc_2019.Intcode;

// disable the "not used" warning
#pragma warning disable IDE0051

internal partial class IntcodeComputer
{
	private static readonly Dictionary<int, Instruction> _instructions = new();

	static IntcodeComputer()
	{
		var instructionMethods = typeof(IntcodeComputer)
			.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
			.Select(mi => (method: mi, match: InstructionNameRegex().Match(mi.Name)))
			.Where(p => p.match.Success)
			.Select(p => (p.method, opcode: int.Parse(p.match.Groups["opcode"].ValueSpan), name: p.match.Groups["name"].Value));

		foreach (var (method, opcode, name) in instructionMethods) {
			var parameters = method.GetParameters();

			//Console.WriteLine($"{name} ({opcode}) -> {method.Name} ({parameters.Length} parameters)");

			_instructions.Add(opcode, new Instruction(opcode, parameters.Length, method));
		}
	}

	internal readonly record struct Instruction(long OpCode, int ParameterCount, MethodInfo Method);

	private (Instruction instruction, List<ParameterMode> parameterModes) Decode(long opCode)
	{
		var ocstr  = opCode.ToString().PadLeft(2, '0');
		var instId = Convert.ToInt32(ocstr[^2..]);
		var modes  = ocstr.Reverse().Skip(2).Select(c => (ParameterMode)Convert.ToInt32(c.ToString())).ToList();
		var instr  = _instructions[instId];

		// add enough default modes to cover all the parameters
		while (modes.Count < instr.ParameterCount) {
			modes.Add(ParameterMode.Position);
		}

		return (instr, modes);
	}

	private void Op01Add(long value1Address, long value2Address, long outputAddress)
	{
		_memory[outputAddress] = _memory[value1Address] + _memory[value2Address];
	}

	private void Op02Multiply(long value1Address, long value2Address, long outputAddress)
	{
		_memory[outputAddress] = _memory[value1Address] * _memory[value2Address];
	}

	private void Op03ReadInput(long outputAddress)
	{
		_memory[outputAddress] = Input?.Invoke(this, EventArgs.Empty) ?? throw new InvalidOperationException("Input needed but no handler attached.");
	}

	private void Op04WriteOutput(long valueAddress)
	{
		Output?.Invoke(this, new OutputEventArgs(_memory[valueAddress]));
	}

	private void Op05JumpIfTrue(long inputAddress, long jumpAddress)
	{
		_instructionPointer = _memory[inputAddress] == 0 ? _instructionPointer : _memory[jumpAddress] - 3;
	}

	private void Op06JumpIfFalse(long inputAddress, long jumpAddress)
	{
		_instructionPointer = _memory[inputAddress] != 0 ? _instructionPointer : _memory[jumpAddress] - 3;
	}

	private void Op07LessThan(long value1Address, long value2Address, long outputAddress)
	{
		_memory[outputAddress] = _memory[value1Address] < _memory[value2Address] ? 1 : 0;
	}

	private void Op08Equals(long value1Address, long value2Address, long outputAddress)
	{
		_memory[outputAddress] = _memory[value1Address] == _memory[value2Address] ? 1 : 0;
	}

	private void Op99Terminate()
	{
		Terminated = true;
	}
}
