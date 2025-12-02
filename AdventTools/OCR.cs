using System.Collections.ObjectModel;

namespace AdventOfCode;

public class OCR
{
	private delegate char GetPixelDelegate<TText>(TText text, int row, int column);

	private static ReadOnlyDictionary<int, char> _characters = new(ComputeAlphabetHashes());

	public static string Recognize(string[] text)
	{
		var charHeight = text.Length;
		var maxLength  = text.Max(t => t.Length);
		var curChar    = 0;

		// 32 should be enough for anyone
		Span<char> chars = stackalloc char[32];

		// for each character, compute a hash code
		foreach (var (offset, length) in FindCharacters(text, charHeight, maxLength, GetPixel)) {
			// add the character to the result
			chars[curChar++] = _characters[ComputeCharacterHash(text, length, charHeight, offset, GetPixel)];
		}

		return chars[..curChar].ToString();
	}

	public static string Recognize(char[,] text)
	{
		var width   = text.GetLength(1);
		var height  = text.GetLength(0);
		var curChar = 0;

		// 32 should be enough for anyone
		Span<char> chars = stackalloc char[32];

		foreach (var (offset, length) in FindCharacters(text, height, width, GetPixel)) {
			chars[curChar++] = _characters[ComputeCharacterHash(text, length, height, offset, GetPixel)];
		}

		return chars[..curChar].ToString();
	}

	private static int ComputeCharacterHash<T>(T text, int charWidth, int charHeight, int charStartCol, GetPixelDelegate<T> getPixel)
	{
		var hashCode = new HashCode();
		#if OCRDEBUG
		var charStr = new char[charHeight][];
		for (var i = 0; i < charHeight; i++) {
			charStr[i] = new char[charWidth];
		}
		#endif

		// for each "lit" pixel, add the (character-relative) X and Y to the hash
		for (var x = 0; x < charWidth; x++) {
			for (var y = 0; y < charHeight; y++) {
				if (getPixel(text, y, charStartCol + x) == '#') {
					hashCode.Add(x);
					hashCode.Add(y);
				}

				#if OCRDEBUG
				charStr[y][x] = getPixel(text, y, x);
				#endif
			}
		}

		#if OCRDEBUG
		Console.WriteLine($"{string.Join(Environment.NewLine, charStr.Select(charArr => new string(charArr)))}");
		#endif

		return hashCode.ToHashCode();
	}

	private static Dictionary<int, char> ComputeAlphabetHashes()
	{
		var ret = new Dictionary<int, char>();

		ComputeHashes([
			".##..###...##..####.####..##..#..#.###...##.#..#.#.....##..###..###...###.#..#.#...#.####",
			"#..#.#..#.#..#.#....#....#..#.#..#..#.....#.#.#..#....#..#.#..#.#..#.#....#..#.#...#....#",
			"#..#.###..#....###..###..#....####..#.....#.##...#....#..#.#..#.#..#.#....#..#..#.#....#.",
			"####.#..#.#....#....#....#.##.#..#..#.....#.#.#..#....#..#.###..###...##..#..#...#....#..",
			"#..#.#..#.#..#.#....#....#..#.#..#..#..#..#.#.#..#....#..#.#....#.#.....#.#..#...#...#...",
			"#..#.###...##..####.#.....###.#..#.###..##..#..#.####..##..#....#..#.###...##....#...####"],
			"ABCEFGHIJKLOPRSUYZ", ret);

		ComputeHashes([
			"..##...#####...####..######.######..####..#....#....###.#....#.#......#....#.#####..#####..#....#.######",
			".#..#..#....#.#....#.#......#......#....#.#....#.....#..#...#..#......##...#.#....#.#....#.#....#......#",
			"#....#.#....#.#......#......#......#......#....#.....#..#..#...#......##...#.#....#.#....#..#..#.......#",
			"#....#.#....#.#......#......#......#......#....#.....#..#.#....#......#.#..#.#....#.#....#..#..#......#.",
			"#....#.#####..#......#####..#####..#......######.....#..##.....#......#.#..#.#####..#####....##......#..",
			"######.#....#.#......#......#......#..###.#....#.....#..##.....#......#..#.#.#......#..#.....##.....#...",
			"#....#.#....#.#......#......#......#....#.#....#.....#..#.#....#......#..#.#.#......#...#...#..#...#....",
			"#....#.#....#.#......#......#......#....#.#....#.#...#..#..#...#......#...##.#......#...#...#..#..#.....",
			"#....#.#....#.#....#.#......#......#...##.#....#.#...#..#...#..#......#...##.#......#....#.#....#.#.....",
			"#....#.#####...####..######.#.......###.#.#....#..###...#....#.######.#....#.#......#....#.#....#.######"],
			"ABCEFGHJKLNPRXZ", ret);

		ComputeHashes([
			"#####",
			"#...#",
			"#...#",
			"#...#",
			"#####"],
			"O", ret);

		return ret;

		static void ComputeHashes(string[] characters, string alphabet, Dictionary<int, char> ret)
		{
			var charHeight = characters.Length;

			var charsToHash = FindCharacters(characters, characters.Length, characters[0].Length, GetPixel)
				.Zip(alphabet)
				.Select(t => (t.First.Offset, t.First.Length, Char: t.Second));

			foreach (var (offset, length, character) in charsToHash) {
				var hash = ComputeCharacterHash(characters, length, charHeight, offset, GetPixel);
				ret.Add(hash, character);
			}
		}
	}

	private static IEnumerable<(int Offset, int Length)> FindCharacters<TText>(TText text, int charHeight, int colCount, GetPixelDelegate<TText> getPixel)
	{
		var curCol    = 0;
		var charStart = 0;

		while (true) {
			// for each column, go down through the rows, and if none are pixels, we've found a divide between
			// characters. If all are char.MinValue, then we've found the end
			var haveChar  = false;
			var havePixel = false;

			for (var i = 0; i < charHeight; i++) {
				var pixel = getPixel(text, i, curCol);

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

	internal static char GetPixel(string[] text, int row, int column) => row < text.Length && column < text[row].Length ? text[row][column] : char.MinValue;

	internal static char GetPixel(char[,] text, int row, int column) => column < text.GetLength(0) && row < text.GetLength(1) ? text[column, row] : char.MinValue;

	public class OCRTests
	{
		[Test]
		public async Task CharacterEnumerationWorksOnArrays()
		{
			var inputStr = new string[] {
				" ##  #  # ###  ###  ###   ##  #  # ####",
				"#  # #  # #  # #  # #  # #  # #  #    #",
				"#  # #### #  # #  # #  # #  # #  #   # ",
				"#### #  # ###  ###  ###  #### #  #  #  ",
				"#  # #  # #    # #  #    #  # #  # #   ",
				"#  # #  # #    #  # #    #  #  ##  ####"};

			var inputArr = new char[39, 6];

			for (var i = 0; i < inputStr.Length; i++) {
				for (var j = 0; j < inputStr[i].Length; j++) {
					inputArr[j, i] = inputStr[i][j];
				}
			}

			await Assert.That(FindCharacters(inputArr, 6, inputArr.GetLength(0), GetPixel)).IsEquivalentTo([
				(00, 4),
				(05, 4),
				(10, 4),
				(15, 4),
				(20, 4),
				(25, 4),
				(30, 4),
				(35, 4),
			]);
		}

		[Test]
		public async Task CharacterEnumerationWorksOnStrings()
		{
			var inputStr = new string[] {
				" ##  #  # ###  ###  ###   ##  #  # ####",
				"#  # #  # #  # #  # #  # #  # #  #    #",
				"#  # #### #  # #  # #  # #  # #  #   # ",
				"#### #  # ###  ###  ###  #### #  #  #  ",
				"#  # #  # #    # #  #    #  # #  # #   ",
				"#  # #  # #    #  # #    #  #  ##  ####"};

			await Assert.That(FindCharacters(inputStr, 6, inputStr[0].Length, GetPixel)).IsEquivalentTo([
				(00, 4),
				(05, 4),
				(10, 4),
				(15, 4),
				(20, 4),
				(25, 4),
				(30, 4),
				(35, 4),
			]);
		}

		[Test]
		public async Task HashingWorksCorrectlyWithoutTrailingSpaces()
		{
			var z1 = new[] {
				"####",
				"   #",
				"  #",
				" #",
				"#",
				"####"};
			var z2 = new[] {
				"####",
				"   #",
				"  # ",
				" #  ",
				"#   ",
				"####"};

			await Assert.That(ComputeCharacterHash(z2, 4, 6, 0, GetPixel)).IsEqualTo(ComputeCharacterHash(z1, 4, 6, 0, GetPixel));
		}

		[Test]
		public async Task HashingStringsAndArraysProducesSameResults()
		{
			var inputStr = new string[] {
				" ##  #  # ###  ###  ###   ##  #  # ####",
				"#  # #  # #  # #  # #  # #  # #  #    #",
				"#  # #### #  # #  # #  # #  # #  #   # ",
				"#### #  # ###  ###  ###  #### #  #  #  ",
				"#  # #  # #    # #  #    #  # #  # #   ",
				"#  # #  # #    #  # #    #  #  ##  ####"};

			var inputArr = new char[39, 6];

			for (var i = 0; i < inputStr.Length; i++) {
				for (var j = 0; j < inputStr[i].Length; j++) {
					inputArr[j, i] = inputStr[i][j];
				}
			}

			await Assert.That(ComputeCharacterHash(inputArr, 4, 6, 0, GetPixel)).IsEqualTo(ComputeCharacterHash(inputStr, 4, 6, 00, GetPixel));
		}

		[Test]
		public async Task RecognizingStringsWorksCorrectly()
		{
			var inputStr = new string[] {
				" ##  #  # ###  ###  ###   ##  #  # ####",
				"#  # #  # #  # #  # #  # #  # #  #    #",
				"#  # #### #  # #  # #  # #  # #  #   # ",
				"#### #  # ###  ###  ###  #### #  #  #  ",
				"#  # #  # #    # #  #    #  # #  # #   ",
				"#  # #  # #    #  # #    #  #  ##  ####"};

			await Assert.That(Recognize(inputStr)).IsEqualTo("AHPRPAUZ");
		}

		[Test]
		public async Task RecognizingArraysWorksCorrectly()
		{
			var inputStr = new string[] {
				" ##  #  # ###  ###  ###   ##  #  # ####",
				"#  # #  # #  # #  # #  # #  # #  #    #",
				"#  # #### #  # #  # #  # #  # #  #   # ",
				"#### #  # ###  ###  ###  #### #  #  #  ",
				"#  # #  # #    # #  #    #  # #  # #   ",
				"#  # #  # #    #  # #    #  #  ##  ####"};

			var inputChar = new char[39, 6];

			for (var i = 0; i < inputStr.Length; i++) {
				for (var j = 0; j < inputStr[i].Length; j++) {
					inputChar[j, i] = inputStr[i][j];
				}
			}

			await Assert.That(Recognize(inputChar)).IsEqualTo("AHPRPAUZ");
		}
	}
}
