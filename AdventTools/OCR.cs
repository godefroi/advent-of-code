using System.Runtime.CompilerServices;
using System.Text;

namespace AdventOfCode;

public static partial class OCR
{
	private const string AHPRPAUZ = """
		 ##  #  # ###  ###  ###   ##  #  # ####
		#  # #  # #  # #  # #  # #  # #  #    #
		#  # #### #  # #  # #  # #  # #  #   #
		#### #  # ###  ###  ###  #### #  #  #
		#  # #  # #    # #  #    #  # #  # #
		#  # #  # #    #  # #    #  #  ##  ####
		""";

	private const string UPOJFLBCEZ = """
		#  # ###   ##    ## #### #    ###   ##  #### ####
		#  # #  # #  #    # #    #    #  # #  # #       #
		#  # #  # #  #    # ###  #    ###  #    ###    #
		#  # ###  #  #    # #    #    #  # #    #     #
		#  # #    #  # #  # #    #    #  # #  # #    #
		 ##  #     ##   ##  #    #### ###   ##  #### ####
		""";

	private delegate char GetPixelDelegate<TText>(TText text, int state, int row, int column);

	private static readonly Lazy<Dictionary<ulong, char>> _alphabet = new(GenerateAlphabet);

	public static string Recognize(string[] text) => string.Concat(
		FindCharacters(text, text.Length, 0, GetPixel)
		.Select(p => Hash(text, text.Length, p, 0, GetPixel))
		.Select(h => _alphabet.Value.TryGetValue(h, out var theChar) ? theChar : '■'));

	public static string Recognize(char[,] text) => string.Concat(
		FindCharacters(text, text.GetLength(1), text.GetLength(0), GetPixel)
		.Select(p => Hash(text, text.GetLength(1), p, text.GetLength(0), GetPixel))
		.Select(h => _alphabet.Value.TryGetValue(h, out var theChar) ? theChar : '■'));

	private static Dictionary<ulong, char> GenerateAlphabet()
	{
		var examples6 = new[] {
			".##..###...##..####.####..##..#..#.###...##.#..#.#.....##..###..###...###.#..#.#...#.####",
			"#..#.#..#.#..#.#....#....#..#.#..#..#.....#.#.#..#....#..#.#..#.#..#.#....#..#.#...#....#",
			"#..#.###..#....###..###..#....####..#.....#.##...#....#..#.#..#.#..#.#....#..#..#.#....#.",
			"####.#..#.#....#....#....#.##.#..#..#.....#.#.#..#....#..#.###..###...##..#..#...#....#..",
			"#..#.#..#.#..#.#....#....#..#.#..#..#..#..#.#.#..#....#..#.#....#.#.....#.#..#...#...#...",
			"#..#.###...##..####.#.....###.#..#.###..##..#..#.####..##..#....#..#.###...##....#...####"};
		var hashes6 = FindCharacters(examples6, 6, 0, GetPixel)
			.Zip("ABCEFGHIJKLOPRSUYZ")
			.Select(t => (Hash: Hash(examples6, 6, t.First, 0, GetPixel), Character: t.Second));

		var examples10 = new[] {
			"..##...#####...####..######.######..####..#....#....###.#....#.#......#....#.#####..#####..#....#.######",
			".#..#..#....#.#....#.#......#......#....#.#....#.....#..#...#..#......##...#.#....#.#....#.#....#......#",
			"#....#.#....#.#......#......#......#......#....#.....#..#..#...#......##...#.#....#.#....#..#..#.......#",
			"#....#.#....#.#......#......#......#......#....#.....#..#.#....#......#.#..#.#....#.#....#..#..#......#.",
			"#....#.#####..#......#####..#####..#......######.....#..##.....#......#.#..#.#####..#####....##......#..",
			"######.#....#.#......#......#......#..###.#....#.....#..##.....#......#..#.#.#......#..#.....##.....#...",
			"#....#.#....#.#......#......#......#....#.#....#.....#..#.#....#......#..#.#.#......#...#...#..#...#....",
			"#....#.#....#.#......#......#......#....#.#....#.#...#..#..#...#......#...##.#......#...#...#..#..#.....",
			"#....#.#....#.#....#.#......#......#...##.#....#.#...#..#...#..#......#...##.#......#....#.#....#.#.....",
			"#....#.#####...####..######.#.......###.#.#....#..###...#....#.######.#....#.#......#....#.#....#.######",};
		var hashes10 = FindCharacters(examples10, 10, 0, GetPixel)
			.Zip("ABCEFGHJKLNPRXZ")
			.Select(t => (Hash: Hash(examples10, 10, t.First, 0, GetPixel), Character: t.Second));

		var ret = hashes6.Concat(hashes10).ToDictionary(t => t.Hash, t => t.Character);
		Console.WriteLine("---");
		return ret;
	}

	private static IEnumerable<(int Offset, int Length)> FindCharacters<TText>(TText text, int charHeight, int state, GetPixelDelegate<TText> getPixel)
	{
		var curCol    = 0;
		var charStart = 0;

		while (true) {
			// for each column, go down through the rows, and if none are pixels, we've found a divide between
			// characters. If all are char.MinValue, then we've found the end
			var haveChar  = false;
			var havePixel = false;

			for (var i = 0; i < charHeight; i++) {
				var pixel = getPixel(text, state, i, curCol);

				// track whether there was a pixel anywhere in this column
				haveChar |= pixel != char.MinValue;

				// if we have an "on" pixel, we can go on to the next column
				if (pixel == '#') {
					havePixel = true;
					break;
				}
			}

			// if we had no characters at all in this column, then we're at the
			// end of the string of characters
			if (!haveChar) {
				if (curCol - charStart > 0) {
					yield return (charStart, curCol - charStart);
					yield break;
				}
			}

			// if we had no "on" pixels in this column, then we're at a gap between
			// characters
			if (!havePixel) {
				var len = curCol - charStart;

				if (len > 0) {
					yield return (charStart, len);
				}

				charStart = curCol + 1;
			}

			curCol++;
		}
	}

	private static ulong Hash<TText>(TText text, int charHeight, (int Offset, int Length) position, int state, GetPixelDelegate<TText> getPixel)
	{
		var ret = 0ul;
		var idx = 0;

		var marker = charHeight switch {
			6  => 1ul << 63,
			10 => (1ul << 63) + (1ul << 62),
			_  => throw new NotSupportedException("Only 6-high and 10-high fonts are currently supported."),
		};

		for (var row = 0; row < charHeight; row++) {
			for (var col = position.Offset; col < position.Offset + position.Length; col++) {
Console.Write(getPixel(text, state, row, col));
				if (getPixel(text, state, row, col) == '#') {
					ret += 1ul << idx;
				}

				idx++;
			}
Console.WriteLine();
		}
Console.WriteLine();

		return ret + marker;
	}

	private static char GetPixel(string[] text, int state, int row, int column) => text[row].Length > column ? text[row][column] : char.MinValue;

	private static char GetPixel(char[,] text, int state, int row, int column) => column < state ? text[column, row] : char.MinValue;
}

/*
	The 6-high font (from https://github.com/nbanman/advent-ocr/blob/master/res/font6.txt):

	ABCEFGHIJKLOPRSUYZ

	.##..###...##..####.####..##..#..#.###...##.#..#.#.....##..###..###...###.#..#.#...#.####
	#..#.#..#.#..#.#....#....#..#.#..#..#.....#.#.#..#....#..#.#..#.#..#.#....#..#.#...#....#
	#..#.###..#....###..###..#....####..#.....#.##...#....#..#.#..#.#..#.#....#..#..#.#....#.
	####.#..#.#....#....#....#.##.#..#..#.....#.#.#..#....#..#.###..###...##..#..#...#....#..
	#..#.#..#.#..#.#....#....#..#.#..#..#..#..#.#.#..#....#..#.#....#.#.....#.#..#...#...#...
	#..#.###...##..####.#.....###.#..#.###..##..#..#.####..##..#....#..#.###...##....#...####

	Each letter consists of 24 "pixels"


	The 10-high font (from https://github.com/nbanman/advent-ocr/blob/master/res/font10.txt):

	ABCEFGHJKLNPRXZ

	..##...#####...####..######.######..####..#....#....###.#....#.#......#....#.#####..#####..#....#.######
	.#..#..#....#.#....#.#......#......#....#.#....#.....#..#...#..#......##...#.#....#.#....#.#....#......#
	#....#.#....#.#......#......#......#......#....#.....#..#..#...#......##...#.#....#.#....#..#..#.......#
	#....#.#....#.#......#......#......#......#....#.....#..#.#....#......#.#..#.#....#.#....#..#..#......#.
	#....#.#####..#......#####..#####..#......######.....#..##.....#......#.#..#.#####..#####....##......#..
	######.#....#.#......#......#......#..###.#....#.....#..##.....#......#..#.#.#......#..#.....##.....#...
	#....#.#....#.#......#......#......#....#.#....#.....#..#.#....#......#..#.#.#......#...#...#..#...#....
	#....#.#....#.#......#......#......#....#.#....#.#...#..#..#...#......#...##.#......#...#...#..#..#.....
	#....#.#....#.#....#.#......#......#...##.#....#.#...#..#...#..#......#...##.#......#....#.#....#.#.....
	#....#.#####...####..######.#.......###.#.#....#..###...#....#.######.#....#.#......#....#.#....#.######

	Each letter consists of 60 "pixels"
*/
