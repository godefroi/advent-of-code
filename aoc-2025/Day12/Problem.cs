using System.Buffers;
using System.Security.Cryptography;

namespace AdventOfCode.Year2025.Day12;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		// there are 6 shapes, each a 3x3 grid
		var shapes    = new int[6];
		//var splitters = SearchValues.Create('x', ':', ' ');
		Span<char> splitters = ['x', ':', ' '];
		Span<Range> ranges = stackalloc Range[16];

		for (var i = 0; i < shapes.Length; i++) {
			for (var j = 1; j <= 3; j++) {
				shapes[i] += input[(i * 5) + j].Count(c => c == '#');
			}
		}

		var rCnt = 0;

		//Console.WriteLine(input[6 * 5]);
		for (var i = 6 * 5; i < input.Length; i++) {
			var ispan  = input[i].AsSpan();
			/*var cpos   = ispan.IndexOf(':');
			var xpos   = ispan.IndexOf('x');
			var width  = int.Parse(ispan[..xpos]);
			var height = int.Parse(ispan[(xpos + 1)..cpos]);

			ispan = ispan[(cpos + 2)..];
			Console.WriteLine();*/

			var rcnt   = ispan.SplitAny(ranges, splitters);
			var width  = int.Parse(ispan[ranges[0]]);
			var height = int.Parse(ispan[ranges[1]]);

			var tShapeSize = 0L;

			for (var n = 0; n < shapes.Length; n++) {
				tShapeSize += (shapes[n] * int.Parse(ispan[ranges[n + 3]]));
			}

			//Console.WriteLine($"tShapeSize: {tShapeSize}, area: {width * height}");

			if (tShapeSize <= (width * height)) {
				//Console.WriteLine(input[i]);
				rCnt++;
			}

			//break;
		}
		
		return (rCnt, 0);
	}
}
