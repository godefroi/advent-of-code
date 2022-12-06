using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace aoc_2019
{
	internal static class problem_14
	{
		private static string m_input = @"3 QVSV => 2 WXRQ
1 KXSC, 2 PSBCN, 11 DNCJV, 2 FTCT, 1 BGMC => 7 PTHL
1 PXFX => 1 LBZJ
2 WXRQ, 12 ZSCZD => 2 HLQM
1 HDTJ, 1 LBZJ, 1 SLPCX, 5 SMCGZ, 3 MFMX, 4 CHZT, 12 BKBCB => 1 HRNSK
10 WSNDR, 1 JCBJ, 3 LBZJ => 2 QBTSV
22 LHZDG, 5 WFXH => 4 XTQRH
1 HLQM => 3 WSNDR
4 NTJCX => 6 TVMCM
1 VDSW, 9 SLPCX => 1 QCMX
2 MFMX => 8 NTJCX
154 ORE => 4 BSTS
12 TKML => 7 FWTFH
14 VDSW, 7 FVGK, 2 JCBJ => 4 LVFB
15 PLGZ, 27 FTCT, 1 LVFB => 4 TNGFX
2 WHPJT, 20 FXPHZ => 7 PQKMJ
6 NJWBT, 8 KVTD, 1 LQFW => 4 ZCDCW
1 QVSV, 2 FXPHZ => 5 ZSCZD
16 LRNQK => 6 BKBCB
5 FXPHZ => 1 FVGK
2 PXFX => 5 CHZT
17 SMZS, 1 VDSW, 7 BSTS => 5 SLPCX
9 RXJQJ, 2 ZVTW, 1 JMDT => 8 BGMC
5 PXFX, 1 FVGK, 2 TGHSD => 2 LRNQK
13 JMDT, 1 BHRFW, 32 MCKPL => 5 KXSC
5 CBZMB => 8 BLTD
3 KVTD, 2 LQFW, 1 LBZJ => 5 NJWBT
1 MCKPL, 2 CHZT, 6 TKML => 6 JCBJ
1 JSBS => 9 TGHSD
6 RXJQJ, 20 LRNQK, 29 KVTD => 8 PLGZ
18 WHPJT => 3 SMCGZ
157 ORE => 8 PNFB
9 QBTSV, 1 LFRF, 2 TNGFX, 4 FTCT, 9 QCMX, 4 PSBCN, 14 ZCDCW, 1 TVMCM => 7 CKQG
8 WHPJT => 9 LFRF
5 VDSW, 24 FWTFH => 1 JMDT
2 WXRQ, 4 BLTD => 7 WHPJT
14 VDSW => 3 CBZMB
1 QCMX, 19 BHRFW, 2 NJWBT => 3 FTCT
3 XTQRH => 2 KVTD
5 QBTSV, 2 JMDT, 3 LVFB => 3 HDTJ
16 PQKMJ, 1 WSNDR => 5 DNCJV
1 CBZMB, 2 PTHL, 6 HRNSK, 80 WHPJT, 10 CKQG, 55 ZVTW => 1 FUEL
5 BKBCB, 3 WSNDR => 1 MCKPL
158 ORE => 3 LHZDG
1 HLQM, 1 ZSCZD => 2 VDSW
140 ORE => 6 QVSV
4 ZSCZD, 13 TGHSD => 1 TKML
1 SLPCX, 3 TKML => 2 HWDQZ
1 BSTS, 8 WXRQ => 5 LQFW
3 BGMC, 3 LRNQK, 3 QBTSV => 6 PSBCN
1 PNFB => 4 FXPHZ
8 WXRQ => 7 JSBS
1 WXRQ, 8 PNFB, 3 XTQRH => 9 PXFX
1 WSNDR, 13 JSBS, 1 VDSW => 8 SMZS
6 NJWBT => 4 BHRFW
1 PXFX, 11 JSBS => 5 RXJQJ
103 ORE => 2 WFXH
5 WHPJT, 6 LRNQK => 2 MFMX
32 HWDQZ, 1 JMDT => 5 ZVTW";

		public static void Part1()
		{
			//Console.WriteLine(Math.Ceiling(2f / 4f)); // need/make, should be 1
			//Console.WriteLine(Math.Ceiling(4f / 2f)); // need/make, should be 2
			//Console.WriteLine(Math.Ceiling(4f / 3f)); // need/make, should be 2
			//Console.WriteLine(Math.Ceiling(1f / 1f)); // need/make, should be 1
			//Console.WriteLine(Math.Ceiling(9f / 1f)); // need/make, should be 9
			//Console.WriteLine(Math.Ceiling(1f / 9f)); // need/make, should be 1
			//return;

			//var reactions = ParseInput(@"10 ORE => 10 A
			//	1 ORE => 1 B
			//	7 A, 1 B => 1 C
			//	7 A, 1 C => 1 D
			//	7 A, 1 D => 1 E
			//	7 A, 1 E => 1 FUEL");

			//var reactions = ParseInput(@"9 ORE => 2 A
			//	8 ORE => 3 B
			//	7 ORE => 5 C
			//	3 A, 4 B => 1 AB
			//	5 B, 7 C => 1 BC
			//	4 C, 1 A => 1 CA
			//	2 AB, 3 BC, 4 CA => 1 FUEL");

			var reactions = ParseInput(m_input);

			var tanks = new Dictionary<string, long>(reactions.Keys.Select(k => new KeyValuePair<string, long>(k, 0)));

			var ore = 0L;
			PerformReaction(reactions, tanks, "FUEL", 1, ref ore);
			Console.WriteLine(ore);
		}

		public static void Part2()
		{
			var target    = 1000000000000;
			var mag       = 0;
			var reactions = ParseInput(m_input);

			// first, find the order of magnitude
			while( true ) {
				//Console.WriteLine(++mag)
				var val = (long)Math.Pow(10, ++mag);

				Console.WriteLine(val);

				if( FindOreForFuel(reactions, val) > target )
					break;
			}

			var high = (long)Math.Pow(10, mag);
			var low  = (long)Math.Pow(10, mag - 1);

			// then binary search
			while( true ) {
				var middle = ((high - low) / 2) + low;

				if( middle == high || middle == low )
					break;

				Console.WriteLine($"b {middle}");

				if( FindOreForFuel(reactions, middle) > target )
					high = middle;
				else
					low = middle;
			}

			var cur = high;

			while( FindOreForFuel(reactions, cur) > target )
				cur--;

			Console.WriteLine(cur);
		}

		private static long FindOreForFuel(Dictionary<string, Reaction> reactions, long fuel)
		{
			var tanks = new Dictionary<string, long>(reactions.Keys.Select(k => new KeyValuePair<string, long>(k, 0)));
			var ore = 0L;

			PerformReaction(reactions, tanks, "FUEL", fuel, ref ore);

			return ore;
		}

		private static void PerformReaction(Dictionary<string, Reaction> reactions, Dictionary<string, long> tanks, string substance, long need, ref long ore)
		{
			if( substance == "ORE" )
				throw new InvalidOperationException("It was ore, dummy");

			if( tanks[substance] >= need )
				return;

			if( tanks[substance] > 0 )
				need -= tanks[substance];

			var react_cnt = (long)Math.Ceiling((double)need / (double)reactions[substance].Quantity);

			foreach( var p in reactions[substance].Precursors ) {
				if( p.Substance == "ORE" ) {
					ore += p.Quantity * react_cnt;
				} else {
					PerformReaction(reactions, tanks, p.Substance, p.Quantity * react_cnt, ref ore);

					// remove the precursor from the tank
					tanks[p.Substance] -= p.Quantity * react_cnt;
				}
			}

			// add the result to the tank
			tanks[substance] += reactions[substance].Quantity * react_cnt;
		}

		private static Dictionary<string, Reaction> ParseInput(string input)
		{
			/*
			10 ORE => 10 A
			1 ORE => 1 B
			7 A, 1 B => 1 C
			7 A, 1 C => 1 D
			7 A, 1 D => 1 E
			7 A, 1 E => 1 FUEL
			*/
			var reactions = new Dictionary<string, Reaction>();

			using( var sr = new StringReader(input) ) {
				while( sr.Peek() > -1 ) {
					var line = sr.ReadLine();
					var parts = line.Split(" => ");

					if( parts.Length != 2 )
						throw new InvalidOperationException("Didn't get expected input");

					var bits = parts[1].Split(' ');
					
					if( reactions.ContainsKey(bits[1]) )
						throw new InvalidOperationException("We already have a reaction for this output");

					var node = new Reaction(bits[1], long.Parse(bits[0]));

					foreach( var p in parts[0].Split(", ") ) {
						bits = p.Split(' ');
						node.Precursors.Add((Substance: bits[1], Quantity: long.Parse(bits[0])));
					}

					reactions.Add(node.Substance, node);
				}
			}

			return reactions;
		}

		private class Reaction
		{
			public Reaction(string substance, long quantity)
			{
				Substance = substance;
				Quantity  = quantity;
			}

			public string Substance { get; }

			public long Quantity { get; }

			public List<(string Substance, long Quantity)> Precursors { get; } = new List<(string Substance, long Quantity)>();
		}
	}
}
