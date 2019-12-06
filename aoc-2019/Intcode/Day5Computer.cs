using System;
using System.Collections.Generic;
using System.Text;

namespace aoc_2019.Intcode
{
	internal class Day5Computer : Day2Computer
	{
		[OpCode(3)]
		protected void GetInput([Address]int address)
		{
			Core[address] = Input.Next();
		}

		[OpCode(4)]
		protected void AddOutput([Address]int address)
		{
			Output.Add(Core[address]);
		}

		[OpCode(5)]
		protected void JumpIfTrue(int value, int address, out int? destination)
		{
			destination = value != 0 ? address : default(int?);
		}

		[OpCode(6)]
		protected void JumpIfFalse(int value, int address, out int? destination)
		{
			destination = value == 0 ? address : default(int?);
		}

		[OpCode(7)]
		protected void LessThan(int value1, int value2, [Address]int outputAddress)
		{
			Core[outputAddress] = value1 < value2  ? 1 : 0;
		}

		[OpCode(8)]
		protected void Equals(int value1, int value2, [Address]int outputAddress)
		{
			Core[outputAddress] = value1 == value2 ? 1 : 0;
		}
	}
}
