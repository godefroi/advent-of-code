using System;
using System.Collections.Generic;
using System.Text;

namespace aoc_2019
{
	internal struct Point
	{
		public Point(long x, long y)
		{
			X = x;
			Y = y;
		}

		public readonly long X;
		public readonly long Y;
	}
}
