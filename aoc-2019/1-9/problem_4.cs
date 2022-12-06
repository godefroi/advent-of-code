using System;

namespace aoc_2019
{
	internal static class problem_4
	{
		public static void Part1(string[] args)
		{
			var low  = 357253;
			var high = 892942;
			var cnt  = 0;

			for( var i = low; i <= high; i++ ) {
				var digits = i.ToString().ToCharArray();
				var found  = false;

				for( var ci = 1; ci < digits.Length; ci++ ) {
					if( digits[ci] == digits[ci - 1] )
						found = true;

					if( digits[ci] < digits[ci - 1] ) {
						found = false;
						break;
					}
				}

				if( found )
					cnt += 1;
			}

			Console.WriteLine(cnt);
		}

		public static void Part2(string[] args)
		{
			var low  = 357253;
			var high = 892942;
			var cnt  = 0;

			for( var i = low; i <= high; i++ ) {
				var digits = i.ToString().ToCharArray();
				var found = false;

				for( var ci = 1; ci < digits.Length; ci++ ) {
					if( digits[ci] == digits[ci - 1] && ((ci >= 2 && digits[ci] != digits[ci - 2]) || ci < 2) && ((ci < digits.Length - 1 && digits[ci] != digits[ci + 1]) || ci >= digits.Length - 1) )
						found = true;

					if( digits[ci] < digits[ci - 1] ) {
						found = false;
						break;
					}
				}

				if( found )
					cnt += 1;
			}

			Console.WriteLine(cnt);
		}
	}
}
