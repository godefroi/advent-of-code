using System;
using System.Collections.Generic;
using System.Text;

namespace aoc_2019.Intcode
{
	internal class Day9Computer : Day5Computer
	{
		[OpCode(9)]
		protected void SetRelativeBase(long newBaseAddress)
		{
			RelativeBase += Core[newBaseAddress];
		}
	}
}
