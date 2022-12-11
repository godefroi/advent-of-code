using System.Reflection;

namespace aoc_2019.Day13;

public class Problem
{
	private const long TILE_BALL = 4;
	private const long TILE_PADDLE = 3;

	public static (int, long) Main(string fileName)
	{
		var program = ReadFileLines(fileName).Single();
		var part1   = Part1(program);
		var part2   = Part2(program);

		return (part1, part2);
	}

	public static int Part1(string program)
	{
		var computer = new IntcodeComputer(program);
		var screen   = new Screen(false);
		var outputs  = new List<long>(3);

		computer.Output += (s, e) => {
			outputs.Add(e.OutputValue);

			if (outputs.Count == 3) {
				screen[outputs[0], outputs[1]] = outputs[2];
				outputs.Clear();
			}
		};

		computer.Resume();

		return screen.Count(l => l == 2);
	}

	public static long Part2(string input)
	{
		var program = input.Split(',').Select(long.Parse).ToList();

		program[0] = 2;

		var _computer  = new IntcodeComputer(program);
		var _screen    = new Screen(false);
		var _outputs   = new List<long>(3);
		var score      = 0L;
		var ball_pos   = -1L;
		var paddle_pos = -1L;

		_computer.Output += (s, e) => {
			_outputs.Add(e.OutputValue);

			if (_outputs.Count == 3) {
				if (_outputs[0] == -1 && _outputs[1] == 0) {
					score = _outputs[2];
				} else {
					_screen[_outputs[0], _outputs[1]] = _outputs[2];

					if (_outputs[2] == TILE_BALL) {
						ball_pos = _outputs[0];
					} else if (_outputs[2] == TILE_PADDLE) {
						paddle_pos = _outputs[0];
					}
				}
				_outputs.Clear();
			}
		};

		_computer.Input += (s, e) => {
			if (ball_pos == -1 || paddle_pos == -1) {
				throw new InvalidOperationException("didn't find something");
			}

			if (ball_pos < paddle_pos) {
				return -1;
			} else if (ball_pos > paddle_pos) {
				return 1;
			} else {
				return 0;
			}
		};

		_computer.Resume();

		return score;
	}

	private class Arcade
	{
		private readonly IntcodeComputer _computer;
		private readonly Screen          _screen  = new(true);
		private readonly List<long>      _outputs = new(3);

		public Arcade(string program)
		{
			_computer = new(program);

			_computer.Output += (s, e) => {
				_outputs.Add(e.OutputValue);

				if (_outputs.Count == 3) {
					_screen[_outputs[0], _outputs[1]] = _outputs[2];
					_outputs.Clear();
					Console.WriteLine();
					Console.ReadKey(true);
				}
			};

			_computer.Input += (s, e) => {
				var ball_x   = -1;
				var paddle_x = -1;

				for (var y = 0; y <= _screen.Height; y++) {
					for (var x = 0; x <= _screen.Width; x++) {
						switch (_screen[x, y]) {
							case 3:
								paddle_x = x;
								break;
							case 4:
								ball_x = x;
								break;
						}
					}
				}

				if (ball_x == -1 || paddle_x == -1) {
					throw new InvalidOperationException("didn't find something");
				}

				if (ball_x < paddle_x) {
					return -1;
				} else if (ball_x > paddle_x) {
					return 1;
				} else {
					return 0;
				}
			};
		}

		public void Execute()
		{
			_computer.Resume();

		//	while (true) {
		//		var interrupt = _computer.Run();

		//		if (interrupt == Computer.InterruptType.Terminated) {
		//			break;
		//		} else if (interrupt == Computer.InterruptType.Input) {
		//			//m_screen.Redraw();

		//			var ball_x = -1;
		//			var paddle_x = -1;

		//			for (var y = 0; y <= _screen.Height; y++) {
		//				for (var x = 0; x <= _screen.Width; x++) {
		//					switch (_screen[x, y]) {
		//						case 3:
		//							paddle_x = x;
		//							break;
		//						case 4:
		//							ball_x = x;
		//							break;
		//					}
		//				}
		//			}

		//			if (ball_x == -1 || paddle_x == -1)
		//				throw new InvalidOperationException("didn't find something");

		//			if (ball_x < paddle_x)
		//				_computer.AddInput(-1);
		//			else if (ball_x > paddle_x)
		//				_computer.AddInput(1);
		//			else
		//				_computer.AddInput(0);

		//			//while( true ) {
		//			//	var key = Console.ReadKey(true);
		//			//	var inp = default(int?);

		//			//	switch( key.Key ) {
		//			//		case ConsoleKey.LeftArrow:
		//			//			inp = -1;
		//			//			break;
		//			//		case ConsoleKey.RightArrow:
		//			//			inp = 1;
		//			//			break;
		//			//		case ConsoleKey.UpArrow:
		//			//			inp = 0;
		//			//			break;
		//			//	}

		//			//	if( inp.HasValue ) {
		//			//		m_computer.AddInput(inp.Value);
		//			//		break;
		//			//	}
		//			//}
		//		}
		//	}

		//	// take all the output and paint the screen
		//	//while( m_computer.OutputAvailable )
		//	//	m_screen[(int)m_computer.GetOutput(), (int)m_computer.GetOutput()] = (int)m_computer.GetOutput();

		//	// then, count the blocks (2)
		//	Console.WriteLine(_score);
		}
	}

	private class Screen
	{
		private Dictionary<long, Dictionary<long, long>> _cells  = new();
		private bool                                     _draw;
		private int                                      _origin;
		private long                                     _width  = 0;
		private long                                     _height = 0;

		public Screen(bool draw)
		{
			_draw = draw;

			if (_draw) {
				 _origin = Console.CursorTop;
				Redraw();
			}
		}

		public long this[long x, long y]
		{
			get {
				if (!_cells.ContainsKey(x))
					return 0;

				if (!_cells[x].ContainsKey(y))
					return 0;

				return _cells[x][y];
			}
			set {
				if (!_cells.ContainsKey(x))
					_cells.Add(x, new Dictionary<long, long>());

				if (!_cells[x].ContainsKey(y))
					_cells[x].Add(y, value);
				else
					_cells[x][y] = value;

				if (x > _width) {
					_width = x;
				}

				if (y > _height) {
					_height = y;
				}

				if (_draw)
					Redraw();
			}
		}

		public long Width => _width;

		public long Height => _height;

		public int Count(Func<long, bool> predicate) => _cells.Values.Sum(v => v.Values.Count(predicate));

		public void Redraw()
		{
			Console.CursorTop  = _origin;
			Console.CursorLeft = 0;
			Console.Clear();

			var height = (int)Height + 1;
			var width  = (int)Width + 1;

			Console.WriteLine(string.Join(Environment.NewLine, Enumerable.Range(0, height).Select(y => new string(Enumerable.Range(0, width).Select(x => this[x, y] switch {
				0 => ' ',
				1 => '█',
				2 => '#',
				3 => '-',
				4 => 'o',
				_ => throw new InvalidOperationException("Can't draw that character"),
			}).ToArray()))));

			Thread.Sleep(1);
		}
	}
}
