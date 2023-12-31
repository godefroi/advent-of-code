namespace aoc_2019.Day11;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Main);

	public static (long, long) Main(string[] input)
	{
		var program = input.Single();
		var part1   = Part1(program);

		Part2(program);

		return (part1, 0);
	}

	private static int Part1(string program)
	{
		var hull = new Hull();

		PaintHull(program, hull);

		return hull.CountPaintedPanels();
	}

	private static void Part2(string program)
	{
		var hull = new Hull();

		hull[0, 0] = Color.White;

		PaintHull(program, hull);

		//Console.WriteLine($"width: {hull.Width} height: {hull.Height}");
		for (var y = 0; y <= hull.Height; y++) {
			for (var x = 0; x <= hull.Width; x++) {
				Console.Write(hull[x, y] == Color.White ? "*" : " ");
			}

			Console.WriteLine();
		}
	}

	private static void PaintHull(string program, Hull hull)
	{
		var computer = new IntcodeComputer(program);
		var mode     = OutputMode.Paint;
		var x        = 0;
		var y        = 0;
		var direction = Direction.Up;

		// input is the camera, give it the color of the current panel
		computer.Input += (s, e) => (int)hull[x, y];

		// output is movement commands
		computer.Output += (s, e) => {
			if (mode == OutputMode.Paint) {
				hull[x, y] = (Color)(int)e.OutputValue;
				mode = OutputMode.Move;
			} else {
				var turn = e.OutputValue;

				// first turn, then move forward one
				direction = direction switch {
					Direction.Up => turn == 0 ? Direction.Left : Direction.Right,
					Direction.Right => turn == 0 ? Direction.Up : Direction.Down,
					Direction.Down => turn == 0 ? Direction.Right : Direction.Left,
					Direction.Left => turn == 0 ? Direction.Down : Direction.Up,
					_ => throw new Exception("not a direction"),
				};

				switch (direction) {
					case Direction.Up:    y--; break;
					case Direction.Right: x++; break;
					case Direction.Down:  y++; break;
					case Direction.Left:  x--; break;
				}

				mode = OutputMode.Paint;
			}
		};

		computer.Resume();
	}

	//public static void Part2(string m_input)
	//{
	//	var program  = Intcode.Computer.Parse(m_input);
	//	var computer = new Intcode.Day9Computer();
	//	var hull     = new Hull();

	//	hull[0, 0] = true;

	//	computer.Initialize(program);

	//	var robot = new Robot(computer, hull);

	//	robot.Execute();

	//	//Console.WriteLine($"width: {hull.Width} height: {hull.Height}");
	//	for( var y = 0; y <= hull.Height; y++ ) {
	//		for( var x = 0; x <= hull.Width; x++ )
	//			Console.Write(hull[x, y] ? "*" : " ");

	//		Console.WriteLine();
	//	}
	//}

	private enum Direction
	{
		Up,
		Right,
		Down,
		Left
	}

	private enum OutputMode
	{
		Paint,
		Move
	}

	private enum Color
	{
		Black = 0,
		White = 1,
	}

	private class Hull
	{
		private readonly Dictionary<int, Dictionary<int, Color>> _panels = new();

		public Color this[int x, int y]
		{
			get => _panels.TryGetValue(x, out var ydict) && ydict.TryGetValue(y, out var value) ? value : Color.Black;
			set {
				if (!_panels.TryGetValue(x, out var ydict)) {
					ydict = new Dictionary<int, Color>();
					_panels.Add(x, ydict);
				}

				ydict[y] = value;
			}
		}

		public int Width => _panels.Max(kvp => kvp.Key);

		public int Height => _panels.Max(kvp => kvp.Value.Max(inner => inner.Key));

		public int CountPaintedPanels() => _panels.Values.Sum(v => v.Count);
	}
}
