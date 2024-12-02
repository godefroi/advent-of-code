namespace AdventOfCode.Year2019.Day08;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, string) Execute(string[] input)
	{
		var inputLine = input.Single();
		var part1     = Part1(inputLine);

		Part2(inputLine);

		return (part1, "CJZLP");
	}

	public static int Part1(string input)
	{
		var layers = GetLayers(input);
		var ln     = -1;
		var zc     = int.MaxValue;

		for (var i = 0; i < layers.Count; i++) {
			var tc = layers[i].Count(i => i == 0);

			if (tc < zc) {
				zc = tc;
				ln = i;
			}
		}

		Console.WriteLine($"{zc} zeroes on layer {ln}");

		return layers[ln].Count(i => i == 1) * layers[ln].Count(i => i == 2);
	}

	public static void Part2(string input)
	{
		var width  = 25;
		var height = 6;
		var layers = GetLayers(input);
		var img    = new int[width * height];

		for( var i = 0; i < img.Length; i++ )
			img[i] = layers.FirstOrDefault(l => l[i] != 2)?[i] ?? 2;

		for( var y = 0; y < height; y++ ) {
			for( var x = 0; x < width; x++ ) {
				var chr = img[x + (y * width)] switch {
					0 => ".",
					1 => "@",
					2 => " ",
					_ => throw new Exception("Dunno.")
				};

				Console.Write(chr);
			}

			Console.WriteLine();
		}
	}

	private static List<int[]> GetLayers(string input)
	{
		var width = 25;
		var height = 6;
		var digits = input.ToCharArray().Select(c => Convert.ToInt32(c.ToString())).ToArray();
		var pos    = 0;
		var layers = new List<int[]>();

		while (pos < digits.Length) {
			var layer = new int[width * height];

			Array.Copy(digits, pos, layer, 0, width * height);

			pos += width * height;

			layers.Add(layer);
		}

		return layers;
	}
}
