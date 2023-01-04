using System.Runtime.CompilerServices;

namespace AdventOfCode;

public static class OCR
{
	private const string AHPRPAUZ = """
		 ##  #  # ###  ###  ###   ##  #  # ####
		#  # #  # #  # #  # #  # #  # #  #    #
		#  # #### #  # #  # #  # #  # #  #   # 
		#### #  # ###  ###  ###  #### #  #  #  
		#  # #  # #    # #  #    #  # #  # #   
		#  # #  # #    #  # #    #  #  ##  ####
		""";

	private static readonly Dictionary<string, string> _strings = new();

	static OCR()
	{
		Add(AHPRPAUZ);
	}

	public static string Recognize(string input) => _strings.TryGetValue(input.TrimEnd('\r', '\n'), out var value) ? value : "--unrecognized--";

	private static void Add(string value, [CallerArgumentExpression(nameof(value))] string argExpression = "") => _strings.Add(value, argExpression);
}
