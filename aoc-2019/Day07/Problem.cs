using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace aoc_2019
{
	internal class problem_7
	{
		private static string m_input = "3,8,1001,8,10,8,105,1,0,0,21,42,67,76,89,110,191,272,353,434,99999,3,9,102,2,9,9,1001,9,2,9,1002,9,2,9,1001,9,2,9,4,9,99,3,9,1001,9,4,9,102,4,9,9,101,3,9,9,1002,9,2,9,1001,9,4,9,4,9,99,3,9,102,5,9,9,4,9,99,3,9,1001,9,3,9,1002,9,3,9,4,9,99,3,9,102,3,9,9,101,2,9,9,1002,9,3,9,101,5,9,9,4,9,99,3,9,1001,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,102,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,99,3,9,102,2,9,9,4,9,3,9,101,1,9,9,4,9,3,9,101,1,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,2,9,9,4,9,3,9,101,1,9,9,4,9,3,9,101,2,9,9,4,9,3,9,101,2,9,9,4,9,99,3,9,102,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,101,1,9,9,4,9,3,9,102,2,9,9,4,9,3,9,101,2,9,9,4,9,3,9,101,2,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,101,2,9,9,4,9,99,3,9,102,2,9,9,4,9,3,9,1001,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,101,1,9,9,4,9,3,9,102,2,9,9,4,9,3,9,101,2,9,9,4,9,3,9,101,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,1,9,9,4,9,3,9,1002,9,2,9,4,9,99,3,9,1002,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,102,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,1001,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,101,2,9,9,4,9,99";

		public static void Part1()
		{
			var program = m_input.Split(',').Select(a => Convert.ToInt64(a)).ToArray();
			//var program = "3,15,3,16,1002,16,10,16,1,16,15,15,4,15,99,0,0".Split(',').Select(a => Convert.ToInt32(a)).ToArray();
			//var program = "3,23,3,24,1002,24,10,24,1002,23,-1,23,101,5,23,23,1,24,23,23,4,23,99,0,0".Split(',').Select(a => Convert.ToInt32(a)).ToArray();

			var cur   = long.MinValue;
			var perms = GetPermutations(new long[] { 0, 1, 2, 3, 4 }, 5);
			var comp  = new Intcode.Day5Computer();

			foreach( var perm in perms ) {
				var inp   = 0L;

				foreach( var phase in perm ) {
					comp.Run(program, new[] { phase, inp });
					inp = comp.GetOutput();
				}

				if( cur < inp )
					cur = inp;
			}

			Console.WriteLine(cur);
		}

		public static void Part2()
		{
			//var program = "3,26,1001,26,-4,26,3,27,1002,27,2,27,1,27,26,27,4,27,1001,28,-1,28,1005,28,6,99,0,0,5".Split(',').Select(a => Convert.ToInt32(a)).ToArray();
			//var program = "3,52,1001,52,-5,52,3,53,1,52,56,54,1007,54,5,55,1005,55,26,1001,54,-5,54,1105,1,12,1,53,54,53,1008,54,0,55,1001,55,1,55,2,53,55,53,4,53,1001,56,-1,56,1005,56,6,99,0,0,0,0,10".Split(',').Select(a => Convert.ToInt32(a)).ToArray();
			var program = m_input.Split(',').Select(a => Convert.ToInt64(a)).ToArray();

			var amps = Enumerable.Range(0, 5).Select(i => new Intcode.Day5Computer()).ToArray();
			var cur  = long.MinValue;

			foreach( var perm in GetPermutations(new long[] { 5, 6, 7, 8, 9 }, 5) ) {
				var inps = perm.ToArray();

				Console.WriteLine($"trying {string.Join(',', inps)}");
				// start the program in each computer
				for( var i = 0; i < amps.Length; i++ )
					amps[i].RunInThread(program, i == 0 ? new[] { inps[i], 0 } : new[] { inps[i] });

				// now, go through each amp, get the output, and feed it into the next (last feeds into first)
				while( true ) {
					// get output from previous amp and input to this amp
					for( var i = 1; i < amps.Length; i++ )
						amps[i].AddInput(amps[i - 1].GetOutput());

					// wait for the last amp
					while( !amps[amps.Length - 1].Idle )
						Thread.Sleep(0);

					Console.WriteLine($"it was idle, term: {amps[amps.Length - 1].Terminated} inp: {amps[amps.Length - 1].AwaitingInput}");
					Console.WriteLine($"cur is {cur}");

					// if we terminated, get the last output and check it for bigness
					if( amps[amps.Length - 1].Terminated ) {
						var opt = amps[amps.Length - 1].GetOutput();

						if( opt > cur )
							cur = opt;

						break;
					}

					// get input from last amp for first amp
					amps[0].AddInput(amps[amps.Length - 1].GetOutput());
				}
			}

			Console.WriteLine(cur);
		}

		private static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
		{
			if( length == 1 )
				return list.Select(t => new T[] { t });

			return GetPermutations(list, length - 1).SelectMany(t => list.Where(e => !t.Contains(e)), (t1, t2) => t1.Concat(new T[] { t2 }));
		}
	}
}
