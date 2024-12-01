using System.Text.RegularExpressions;
using Microsoft.Diagnostics.Symbols;

namespace AdventOfCode.Year2023.Day03;

public partial class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		//input = File.ReadAllLines(@"C:\Users\MarkParker\source\repos\aoc-erick\2023\dayThree\input.txt");
		var map   = CreateMap(input, c => c);
		var sum   = 0;
		var gsyms = new List<(Coordinate Position, int Number)>();
		var part2 = 0L;

		for (var i = 0; i < input.Length; i++) {
			foreach (Match match in GetSymbolRegex().Matches(input[i])) {
				var symbols = Enumerable.Range(match.Index, match.Length).SelectMany(x => AdjacentSymbols(map, x, i)).ToHashSet();
				var number  = int.Parse(match.Value);

				Console.WriteLine($"[{match.Index},{i}] {number} {symbols.Count > 0}");

				if (symbols.Count > 0) {
					sum += number;
				}

				foreach (var symbol in symbols.Where(s => s.Character == '*')) {
					gsyms.Add((symbol.Position, number));
				}
			}
		}

		foreach (var group in gsyms.GroupBy(i => i.Position).Where(g => g.Count() == 2)) {
			part2 += group.Aggregate(1, (acc, g) => acc * g.Number);
		}

		return (sum, part2);
	}

	private static bool IsSymbol(char character) => character switch {
		'0' => false,
		'1' => false,
		'2' => false,
		'3' => false,
		'4' => false,
		'5' => false,
		'6' => false,
		'7' => false,
		'8' => false,
		'9' => false,
		'.' => false,
		_ => true,
	};

	private static HashSet<Symbol> AdjacentSymbols(char[,] map, int x, int y)
	{
		//Console.WriteLine($"Testing char at {x},{y} ({map[x, y]})");

		var width   = map.GetLength(0);
		var height  = map.GetLength(1);
		var symbols = new HashSet<Symbol>();

		// x is across >
		// y is down
		// 0,0 is top left

		if (y > 0) {
			if (x > 0) {
				// above-left of the first char
				if (IsSymbol(map[x - 1, y - 1])) {
					symbols.Add(new Symbol(new Coordinate(x - 1, y - 1), map[x - 1, y - 1]));
				}
			}

			// above each char
			if (IsSymbol(map[x, y - 1])) {
				symbols.Add(new Symbol(new Coordinate(x, y - 1), map[x, y - 1]));
			}

			// above-right of first char
			if (x < width - 1) {
				if (IsSymbol(map[x + 1, y - 1])) {
					symbols.Add(new Symbol(new Coordinate(x + 1, y - 1), map[x + 1, y - 1]));
				}
			}
		}

		if (x > 0) {
			// left of first char
			if (IsSymbol(map[x - 1, y])) {
				symbols.Add(new Symbol(new Coordinate(x - 1, y), map[x - 1, y]));
			}
		}

		if (x < width - 1) {
			// right of last char
			if (IsSymbol(map[x + 1, y])) {
				symbols.Add(new Symbol(new Coordinate(x + 1, y), map[x + 1, y]));
			}
		}

		if (y < height - 1) {
			if (x > 0) {
			// below-left of first char
				if (IsSymbol(map[x - 1, y + 1])) {
					symbols.Add(new Symbol(new Coordinate(x - 1, y + 1), map[x - 1, y + 1]));
				}
			}

			// below each char
			if (IsSymbol(map[x, y + 1])) {
				symbols.Add(new Symbol(new Coordinate(x, y + 1), map[x, y + 1]));
			}

			// below-right of last char
			if (x < width - 1) {
				if (IsSymbol(map[x + 1, y + 1])) {
					symbols.Add(new Symbol(new Coordinate(x + 1, y + 1), map[x + 1, y + 1]));
				}
			}
		}

		return symbols;
	}


	[GeneratedRegex(@"\d+")]
	private static partial Regex GetSymbolRegex();

	private readonly record struct Symbol(Coordinate Position, char Character);
}
