using System;
using System.Collections.Generic;

namespace aoc_2019.Intcode
{
	internal class Day2Computer : Computer
	{
		[OpCode(1)]
		protected void Add(int value1, int value2, [Address]int outputAddress)
		{
			Core[outputAddress] = value1 + value2;
		}

		[OpCode(2)]
		protected void Multiply(int value1, int value2, [Address]int outputAddress)
		{
			Core[outputAddress] = value1 * value2;
		}
	}
}
