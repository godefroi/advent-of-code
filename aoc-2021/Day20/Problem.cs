namespace Day20;

using Xunit;

using Image = Dictionary<(int x, int y), char>;

public class Problem : ProblemBase
{
	internal static void Main(string fileName)
	{
		//new Problem().DefaultFlipWorksCorrectly();return;

		var input       = File.ReadAllLines(GetFilePath(fileName));
		var enhancement = input[0].Select(c => c == '#' ? '1' : '0').ToArray();
		var image       = new Image();

		// this can definitely be more clever
		for (var y = 2; y < input.Length; y++) {
			for (var x = 0; x < input[y].Length; x++) {
				image.Add((x, y - 2), input[y][x] == '#' ? '1' : '0');
			}
		}

		image.Add((int.MinValue, int.MinValue), '0');

		for (var i = 0; i < 50; i++) {
			image = Enhance(image, enhancement);

			if (i == 1) {
				Console.WriteLine($"part 1: {image.Values.Count(v => v == '1')}"); // part 1 is 5682
			}
		}

		Console.WriteLine($"part 2: {image.Values.Count(v => v == '1')}"); // part 2 is 17628
	}

	private static Image Enhance(Image image, char[] enhancement)
	{
		var xmin = image.Keys.Min(k => k.x == int.MinValue ? int.MaxValue : k.x);
		var xmax = image.Keys.Max(k => k.x);
		var ymin = image.Keys.Min(k => k.y == int.MinValue ? int.MaxValue : k.y);
		var ymax = image.Keys.Max(k => k.y);
		var def  = image[(int.MinValue, int.MinValue)];

		Console.WriteLine($"Enhancing, x: {xmin}..{xmax} y: {ymin}.{ymax}");

		var ret = new Image();

		for (var x = xmin - 2; x < xmax + 2; x++) {
			for (var y = ymin - 2; y < ymax + 2; y++) {
				ret.Add((x, y), enhancement[GetEnhancementIndex(x, y, xmin, ymin, xmax, ymax, def, image)]);
			}
		}

		//Console.WriteLine($"def was {def}");
		//Console.WriteLine(new string(Enumerable.Range(0, 9).Select(n => def).ToArray()));
		//Console.WriteLine(enhancement[Convert.ToInt32(new string(Enumerable.Range(0, 9).Select(n => def).ToArray()), 2)]);

		ret.Add((int.MinValue, int.MinValue), enhancement[Convert.ToInt32(new string(Enumerable.Range(0, 9).Select(n => def).ToArray()), 2)]);

		return ret;
	}

	private static int GetEnhancementIndex(int x, int y, int xmin, int ymin, int xmax, int ymax, char def, Image image)
	{
		var coord = new List<char>();

		foreach (var cy in new[] { y - 1, y, y + 1 }) {
			foreach (var cx in new[] { x - 1, x, x + 1 }) {
				if (cy < ymin || cx < xmin || cy > ymax || cx > xmax) {
					coord.Add(def);
				} else {
					coord.Add(image[(cx, cy)]);
				}
			}
		}

		return Convert.ToInt32(new string(coord.ToArray()), 2);
	}

	private static void Print(Image image)
	{
		var xmin = image.Keys.Min(k => k.x == int.MinValue ? int.MaxValue : k.x);
		var xmax = image.Keys.Max(k => k.x);
		var ymin = image.Keys.Min(k => k.y == int.MinValue ? int.MaxValue : k.y);
		var ymax = image.Keys.Max(k => k.y);
		var def  = image[(int.MinValue, int.MinValue)];

		char MapChar(char input) => input switch {
			'0' => '.',
			'1' => '#',
			_ => throw new InvalidOperationException("Not a valid character."),
		};

		Console.WriteLine(new string(Enumerable.Range(xmin - 1, xmax - xmin + 3).Select(n => MapChar(def)).ToArray()));

		for (var y = ymin; y <= ymax; y++) {
			var str = new string(Enumerable.Range(xmin, xmax - xmin + 1).Select(x => MapChar(image[(x, y)])).ToArray());
			Console.WriteLine($"{MapChar(def)}{str}{MapChar(def)}");
		}

		Console.WriteLine(new string(Enumerable.Range(xmin - 1, xmax - xmin + 3).Select(n => MapChar(def)).ToArray()));
	}

	[Fact]
	public void IndexCalculationWorksCorrectly()
	{
		var image = new Image {
			{ (0, 0), '1' }, { (1, 0), '0' }, { (2, 0), '0' }, { (3, 0), '1' }, { (4, 0), '0' },
			{ (0, 1), '1' }, { (1, 1), '0' }, { (2, 1), '0' }, { (3, 1), '0' }, { (4, 1), '0' },
			{ (0, 2), '1' }, { (1, 2), '1' }, { (2, 2), '0' }, { (3, 2), '0' }, { (4, 2), '1' },
			{ (0, 3), '0' }, { (1, 3), '0' }, { (2, 3), '1' }, { (3, 3), '0' }, { (4, 3), '0' },
			{ (0, 4), '0' }, { (1, 4), '0' }, { (2, 4), '1' }, { (3, 4), '1' }, { (4, 4), '1' },
			{ (int.MinValue, int.MinValue), '0' }
		};

		var xmin = image.Keys.Min(k => k.x == int.MinValue ? int.MaxValue : k.x);
		var xmax = image.Keys.Max(k => k.x);
		var ymin = image.Keys.Min(k => k.y == int.MinValue ? int.MaxValue : k.y);
		var ymax = image.Keys.Max(k => k.y);
		var def  = image[(int.MinValue, int.MinValue)]; 

		Assert.Equal( 34, GetEnhancementIndex(2, 2, xmin, ymin, xmax, ymax, def, image));
		Assert.Equal( 18, GetEnhancementIndex(0, 0, xmin, ymin, xmax, ymax, def, image));
		Assert.Equal( 48, GetEnhancementIndex(4, 4, xmin, ymin, xmax, ymax, def, image));
		Assert.Equal(152, GetEnhancementIndex(0, 2, xmin, ymin, xmax, ymax, def, image));
	}

	[Fact]
	public void ImageEnhancementWorksCorrectly()
	{
		var enhancement = "..#.#..#####.#.#.#.###.##.....###.##.#..###.####..#####..#....#..#..##..###..######.###...####..#..#####..##..#.#####...##.#.#..#.##..#.#......#.###.######.###.####...#.##.##..#..#..#####.....#.#....###..#.##......#.....#..#..#..##..#...##.######.####.####.#.#...#.......#..#.#.#...####.##.#......#..#...##.#.##..#...##.#.##..###.#......#.#.......#.#.#.####.###.##...#.....####.#..#..#.##.#....##..#.####....##...##..#...#......#.#.......#.......##..####..#...#.#.#...##..#.#..###..#####........#..####......#..#".Select(c => c == '#' ? '1' : '0').ToArray();
		var image = new Image {
			{ (0, 0), '1' }, { (1, 0), '0' }, { (2, 0), '0' }, { (3, 0), '1' }, { (4, 0), '0' },
			{ (0, 1), '1' }, { (1, 1), '0' }, { (2, 1), '0' }, { (3, 1), '0' }, { (4, 1), '0' },
			{ (0, 2), '1' }, { (1, 2), '1' }, { (2, 2), '0' }, { (3, 2), '0' }, { (4, 2), '1' },
			{ (0, 3), '0' }, { (1, 3), '0' }, { (2, 3), '1' }, { (3, 3), '0' }, { (4, 3), '0' },
			{ (0, 4), '0' }, { (1, 4), '0' }, { (2, 4), '1' }, { (3, 4), '1' }, { (4, 4), '1' },
			{ (int.MinValue, int.MinValue), '0' }
		};

		Print(image);
		Console.WriteLine();

		Enhance(image, enhancement);
		Print(image);
		Console.WriteLine();

		Enhance(image, enhancement);
		Print(image);

		Console.WriteLine(image.Values.Count(v => v == '1'));
	}

	[Fact]
	public void DefaultFlipWorksCorrectly()
	{
		var enhancement = "#.#.#..#####.#.#.#.###.##.....###.##.#..###.####..#####..#....#..#..##..###..######.###...####..#..#####..##..#.#####...##.#.#..#.##..#.#......#.###.######.###.####...#.##.##..#..#..#####.....#.#....###..#.##......#.....#..#..#..##..#...##.######.####.####.#.#...#.......#..#.#.#...####.##.#......#..#...##.#.##..#...##.#.##..###.#......#.#.......#.#.#.####.###.##...#.....####.#..#..#.##.#....##..#.####....##...##..#...#......#.#.......#.......##..####..#...#.#.#...##..#.#..###..#####........#..####......#...".Select(c => c == '#' ? '1' : '0').ToArray();
		var image = new Image {
			{ (0, 0), '1' }, { (1, 0), '0' }, { (2, 0), '0' }, { (3, 0), '1' }, { (4, 0), '0' },
			{ (0, 1), '1' }, { (1, 1), '0' }, { (2, 1), '0' }, { (3, 1), '0' }, { (4, 1), '0' },
			{ (0, 2), '1' }, { (1, 2), '1' }, { (2, 2), '0' }, { (3, 2), '0' }, { (4, 2), '1' },
			{ (0, 3), '0' }, { (1, 3), '0' }, { (2, 3), '1' }, { (3, 3), '0' }, { (4, 3), '0' },
			{ (0, 4), '0' }, { (1, 4), '0' }, { (2, 4), '1' }, { (3, 4), '1' }, { (4, 4), '1' },
			{ (int.MinValue, int.MinValue), '0' }
		};

		Print(image);
		Console.WriteLine();

		image = Enhance(image, enhancement);
		Print(image);
		Console.WriteLine();

		image = Enhance(image, enhancement);
		Print(image);

		Console.WriteLine(image.Values.Count(v => v == '1'));
	}
}
