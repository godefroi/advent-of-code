using System;
using System.Linq;

namespace aoc_2019
{
	internal class problem_2
	{
		public static string Input = "1,0,0,3,1,1,2,3,1,3,4,3,1,5,0,3,2,10,1,19,2,19,6,23,2,13,23,27,1,9,27,31,2,31,9,35,1,6,35,39,2,10,39,43,1,5,43,47,1,5,47,51,2,51,6,55,2,10,55,59,1,59,9,63,2,13,63,67,1,10,67,71,1,71,5,75,1,75,6,79,1,10,79,83,1,5,83,87,1,5,87,91,2,91,6,95,2,6,95,99,2,10,99,103,1,103,5,107,1,2,107,111,1,6,111,0,99,2,14,0,0";

		public static void Part1(string[] args)
		{
			var program = args.Select(a => Convert.ToInt64(a)).ToArray();

			program[1] = 12;
			program[2] = 2;

			Console.WriteLine(RunProgram(program));
		}

		public static void Part2(string[] args)
		{
			var program = args.Select(a => Convert.ToInt64(a)).ToArray();

			for( var i = 0; i < 100; i++ ) {
				for( var j = 0; j < 100; j++ ) {
					var copy = program.Clone() as long[];
					copy[1] = i;
					copy[2] = j;

					if( RunProgram(copy) == 19690720 ) {
						Console.WriteLine((i * 100) + j);
						return;
					}
				}
			}

			Console.WriteLine("No solution.");
		}

		private static long RunProgram(long[] ints)
		{
			var pos = 0;

			while( true ) {
				switch( ints[pos] ) {
					case 1:
						ints[ints[pos + 3]] = ints[ints[pos + 1]] + ints[ints[pos + 2]];
						break;

					case 2:
						ints[ints[pos + 3]] = ints[ints[pos + 1]] * ints[ints[pos + 2]];
						break;

					case 99:
						return ints[0];

					default:
						throw new NotSupportedException($"The opcode {ints[pos]} is not supported.");
				}

				pos += 4;
			}
		}
	}
}
