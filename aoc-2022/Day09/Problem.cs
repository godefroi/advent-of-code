namespace aoc_2022.Day09;

public class Problem
{
	public static (int, int) Main(string fileName)
	{
		var instructions = ReadFileLines(fileName, Instruction.Parse);
		var part1        = Part1(instructions);
		var part2        = Part2(instructions);

		return (part1, part2);
	}

	private static (int x, int y) Move((int x, int y) from, Direction direction) => direction switch {
		Direction.Up => (from.x, from.y - 1),
		Direction.Down => (from.x, from.y + 1),
		Direction.Left => (from.x - 1, from.y),
		Direction.Right => (from.x + 1, from.y),
		_ => throw new NotSupportedException("That direction isn't supported."),
	};

	private static int Part1(IEnumerable<Instruction> instructions)
	{
		var head          = (x: 0, y: 0);
		var tail          = (x: 0, y: 0);
		var tailPositions = new HashSet<(int x, int y)>();

		foreach (var (direction, length) in instructions) {
			for (var i = 0; i < length; i++) {
				// move the head
				head = Move(head, direction);

				// tail follows
				tail = Follow(head, tail);

				//Console.WriteLine($"Moved head {direction}, head is at {head}, tail is at {tail}");

				tailPositions.Add(tail);
			}
		}

		return tailPositions.Count;
	}

	private static int Part2(IEnumerable<Instruction> instructions)
	{
		var rope          = Enumerable.Range(0, 10).Select(i => (x: 0, y: 0)).ToArray();
		var tailPositions = new HashSet<(int x, int y)>();

		foreach (var (direction, length) in instructions) {
			for (var i = 0; i < length; i++) {
				// move the head
				rope[0] = Move(rope[0], direction);

				// move each knot down the rope
				for (var j = 1; j < rope.Length; j++) {
					rope[j] = Follow(rope[j - 1], rope[j]);
				}

				//Console.WriteLine($"Moved head {direction}, head is at {head}, tail is at {tail}");
				Draw(rope);

				tailPositions.Add(rope[rope.Length - 1]);
			}
		}

		return tailPositions.Count;
	}

	private static (int x, int y) Follow((int x, int y) head, (int x, int y) tail)
	{
		// if the tail is adjacent to the head, then the tail doesn't move
		if ((tail.x == head.x && tail.y == head.y)         || // tail overlaps head, no moving
			(tail.x == head.x && tail.y == head.y - 1)     || // tail is adjacent up from head
			(tail.x == head.x && tail.y == head.y + 1)     || // tail is adjacent down from head
			(tail.x == head.x - 1 && tail.y == head.y)     || // tail is adjacent left from head
			(tail.x == head.x + 1 && tail.y == head.y)     || // tail is adjacent right from head
			(tail.x == head.x + 1 && tail.y == head.y - 1) || // tail is adjacent up and right from head
			(tail.x == head.x + 1 && tail.y == head.y + 1) || // tail is adjacent down and right from head
			(tail.x == head.x - 1 && tail.y == head.y + 1) || // tail is adjacent up and left from head
			(tail.x == head.x - 1 && tail.y == head.y - 1)) { // tail is adjacent down and left from head
			// do nothing
			return tail;
		}
		
		if (tail.x == head.x && tail.y == head.y - 2) { // tail is two up from head
			return (tail.x, tail.y + 1); // move up
		}
		
		if (tail.x == head.x && tail.y == head.y + 2) { // tail is two down from head
			return (tail.x, tail.y - 1); // move down
		}
		
		if (tail.x == head.x - 2 && tail.y == head.y) { // tail is two left from head
			return (tail.x + 1, tail.y); // move right
		}
		
		if (tail.x == head.x + 2 && tail.y == head.y) { // tail is two right from head
			return (tail.x - 1, tail.y); // move left
		}
		
		if ((tail.x == head.x + 1 && tail.y == head.y - 2) || // head is two up and right
			(tail.x == head.x + 2 && tail.y == head.y - 1) || // head is two right and up
			(tail.x == head.x + 2 && tail.y == head.y - 2)) { // head is two right and two up
			return (tail.x - 1, tail.y + 1); // move diag down-left
		}

		if ((tail.x == head.x - 1 && tail.y == head.y - 2) || // head is two up and left
			(tail.x == head.x - 2 && tail.y == head.y - 1) || // head is two left and up
			(tail.x == head.x - 2 && tail.y == head.y - 2)) {
			return (tail.x + 1, tail.y + 1); // move diag down-right
		}

		if ((tail.x == head.x + 1 && tail.y == head.y + 2) || // head is two down and right
			(tail.x == head.x + 2 && tail.y == head.y + 1) || // head is two right and down
			(tail.x == head.x + 2 && tail.y == head.y + 2)) {
			return (tail.x - 1, tail.y - 1); // move diag up-left
		}
		
		if ((tail.x == head.x - 1 && tail.y == head.y + 2) || // head is two down and left
			(tail.x == head.x - 2 && tail.y == head.y + 1) || // head is two left and down
			(tail.x == head.x - 2 && tail.y == head.y + 2)) {
			return (tail.x + 1, tail.y - 1); // move diag up-right
		}
	
		throw new InvalidDataException($"Head is at {head}, tail is at {tail}, do you know where your ships are?");
	}

	private static void Draw((int x, int y)[] rope)
	{
		//var xmin   = rope.Min(k => k.x);
		//var xmax   = rope.Max(k => k.x);
		//var width  =  - xmin;
		//var height = rope.Max(k => k.y) - rope.Min(k => k.y);
		//var map    = new string[width, height];

		//Array.

		////Console.WriteLine($"{width} {height}");
		//for (var x = xmin; x < )
	}

	private readonly record struct Instruction(Direction Direction, int Length)
	{
		private static readonly Dictionary<char, Direction> _directionMap = Enum.GetValues<Direction>().ToDictionary(d => d.ToString()[0]);

		public static Instruction Parse(string input)
		{
			var parts = input.Split(' ');

			return new Instruction(_directionMap[parts[0][0]], int.Parse(parts[1]));
		}
	}

	private enum Direction
	{
		Up,
		Down,
		Left,
		Right,
	}
}
