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
			//problem_2.Part1();
			//problem_2.Part2(problem_2.Input.Split(','));
			//var inp = ReadInput(problem_3.Input).ToArray(); problem_3.Part1(inp); var sw = Stopwatch.StartNew(); problem_3.Part1(inp); Console.WriteLine(sw.ElapsedMilliseconds);
			//problem_3.Part2(ReadInput(problem_3.Input).ToArray());
			//problem_4.Part1(null);
			//problem_4.Part2(null);
			//problem_5.Part1();
			//problem_5.Part2();
			//Time(problem_6.Part1);
			//Time(problem_6.Part2);
			//problem_7.Part1();
			//problem_7.Part2();
			//problem_8.Part1();
			//problem_8.Part2();
			//problem_9.Part1();

			//problem_10.Part1();
			//problem_10.Part2();
			//problem_10.Part2_Michael();

			//problem_11.Part1();
			//problem_11.Part2();

			//problem_12.Part2();
			//Time(problem_12.Part2);

			//problem_13.Part1();
			//problem_13.Part2();

			//problem_14.Part1();
			problem_14.Part2();
		}

		private static IEnumerable<string> ReadInput(string input)
		{
			using( var sr = new StringReader(input) ) {
				while( sr.Peek() > -1 )
					yield return sr.ReadLine();
			}
		}

		private static void Time(Action action)
		{
			action();
			var sw = Stopwatch.StartNew();
			action();
			sw.Stop();
			Console.WriteLine($"{sw.ElapsedMilliseconds} ms execution time");
		}
	}
}
