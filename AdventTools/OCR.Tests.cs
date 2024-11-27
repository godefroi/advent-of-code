namespace AdventOfCode;

public static partial class OCR
{
	[Fact]
	public static void CharacterEnumerationWorksOnStrings()
	{
		var inputStr = new string[] {
			" ##  #  # ###  ###  ###   ##  #  # ####",
			"#  # #  # #  # #  # #  # #  # #  #    #",
			"#  # #### #  # #  # #  # #  # #  #   # ",
			"#### #  # ###  ###  ###  #### #  #  #  ",
			"#  # #  # #    # #  #    #  # #  # #   ",
			"#  # #  # #    #  # #    #  #  ##  ####"};

		Assert.Collection(FindCharacters(inputStr, 6, 0, GetPixel),
			t => { Assert.Equal(00, t.Offset); Assert.Equal(4, t.Length); },
			t => { Assert.Equal(05, t.Offset); Assert.Equal(4, t.Length); },
			t => { Assert.Equal(10, t.Offset); Assert.Equal(4, t.Length); },
			t => { Assert.Equal(15, t.Offset); Assert.Equal(4, t.Length); },
			t => { Assert.Equal(20, t.Offset); Assert.Equal(4, t.Length); },
			t => { Assert.Equal(25, t.Offset); Assert.Equal(4, t.Length); },
			t => { Assert.Equal(30, t.Offset); Assert.Equal(4, t.Length); },
			t => { Assert.Equal(35, t.Offset); Assert.Equal(4, t.Length); }
		);
	}

	[Fact]
	public static void CharacterEnumerationWorksOnArrays()
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

		Assert.Collection(FindCharacters(inputArr, 6, inputArr.GetLength(0), GetPixel),
			t => { Assert.Equal(00, t.Offset); Assert.Equal(4, t.Length); },
			t => { Assert.Equal(05, t.Offset); Assert.Equal(4, t.Length); },
			t => { Assert.Equal(10, t.Offset); Assert.Equal(4, t.Length); },
			t => { Assert.Equal(15, t.Offset); Assert.Equal(4, t.Length); },
			t => { Assert.Equal(20, t.Offset); Assert.Equal(4, t.Length); },
			t => { Assert.Equal(25, t.Offset); Assert.Equal(4, t.Length); },
			t => { Assert.Equal(30, t.Offset); Assert.Equal(4, t.Length); },
			t => { Assert.Equal(35, t.Offset); Assert.Equal(4, t.Length); }
		);
	}

	[Fact]
	public static void HashingWorksCorrectlyWithoutTrailingSpaces()
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

		Assert.Equal(Hash(z1, 6, (0, 4), 0, GetPixel), Hash(z2, 6, (0, 4), 0, GetPixel));
	}

	[Fact]
	public static void HashingStringsAndArraysProducesSameResults()
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

		Assert.Equal(Hash(inputStr, 6, (0, 4), 0, GetPixel), Hash(inputArr, 6, (0, 4), inputArr.GetLength(0), GetPixel));
	}

	[Fact]
	public static void RecognizingStringsWorksCorrectly()
	{
		var inputStr = new string[] {
			" ##  #  # ###  ###  ###   ##  #  # ####",
			"#  # #  # #  # #  # #  # #  # #  #    #",
			"#  # #### #  # #  # #  # #  # #  #   # ",
			"#### #  # ###  ###  ###  #### #  #  #  ",
			"#  # #  # #    # #  #    #  # #  # #   ",
			"#  # #  # #    #  # #    #  #  ##  ####"};

		Assert.Equal("AHPRPAUZ", Recognize(inputStr));
	}

	[Fact]
	public static void RecognizingArraysWorksCorrectly()
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

		Assert.Equal("AHPRPAUZ", Recognize(inputChar));
	}
}
