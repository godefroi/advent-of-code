namespace aoc_2019.Intcode;

internal partial class IntcodeComputer
{
	private void Op01Add(long value1Address, long value2Address, long outputAddress)
	{
		_memory[outputAddress] = _memory[value1Address] + _memory[value2Address];
	}

	private void Op02Multiply(long value1Address, long value2Address, long outputAddress)
	{
		_memory[outputAddress] = _memory[value1Address] * _memory[value2Address];
	}
}
