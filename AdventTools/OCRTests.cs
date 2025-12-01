namespace AdventOfCode;

public class OCRTests
{
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

		await Assert.That(OCR.FindCharacters(inputStr, 6, 0, OCR.GetPixel)).IsEquivalentTo([
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

		await Assert.That(OCR.FindCharacters(inputArr, 6, inputArr.GetLength(0), OCR.GetPixel)).IsEquivalentTo([
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

		await Assert.That(OCR.Hash(z2, 6, (0, 4), 0, OCR.GetPixel)).IsEqualTo(OCR.Hash(z1, 6, (0, 4), 0, OCR.GetPixel));
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

		await Assert.That(OCR.Hash(inputArr, 6, (0, 4), inputArr.GetLength(0), OCR.GetPixel)).IsEqualTo(OCR.Hash(inputStr, 6, (0, 4), 0, OCR.GetPixel));
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

		await Assert.That(OCR.Recognize(inputStr)).IsEqualTo("AHPRPAUZ");
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

		await Assert.That(OCR.Recognize(inputChar)).IsEqualTo("AHPRPAUZ");
	}
}
