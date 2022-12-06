using System;
using System.Collections.Generic;

namespace aoc_2019.Intcode
{
	internal class Day2Computer : Computer
	{
		[OpCode(1)]
		protected void Add(long value1Address, long value2Address, long outputAddress)
		{
			Core[outputAddress] = Core[value1Address] + Core[value2Address];
		}

		[OpCode(2)]
		protected void Multiply(long value1Address, long value2Address, long outputAddress)
		{
			Core[outputAddress] = Core[value1Address] * Core[value2Address];
		}
	}
}
