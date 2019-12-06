using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace aoc_2019
{
    class Program
    {
        static void Main(string[] args)
        {
			//problem_1.Run(ReadInput(problem_1.Input).ToArray());
			//problem_1.Part2(ReadInput(problem_1.Input).ToArray());
			//problem_2.Part1(problem_2.Input.Split(','));
			//problem_2.Part2(problem_2.Input.Split(','));
			//var inp = ReadInput(problem_3.Input).ToArray(); problem_3.Part1(inp); var sw = Stopwatch.StartNew(); problem_3.Part1(inp); Console.WriteLine(sw.ElapsedMilliseconds);
			//problem_3.Part2(ReadInput(problem_3.Input).ToArray());
			//problem_4.Part1(null);
			//problem_4.Part2(null);
			//problem_5.Part1();
			//problem_5.Part2();
			//problem_6.Part1();
			problem_6.Part2();
		}

		private static IEnumerable<string> ReadInput(string input)
		{
			using( var sr = new StringReader(input) ) {
				while( sr.Peek() > -1 )
					yield return sr.ReadLine();
			}
		}
	}
}
