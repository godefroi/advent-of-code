using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aoc_2019
{
	internal static class problem_11
	{
		private static readonly string m_input = @"3,8,1005,8,352,1106,0,11,0,0,0,104,1,104,0,3,8,102,-1,8,10,101,1,10,10,4,10,108,1,8,10,4,10,102,1,8,28,1,1003,20,10,2,106,11,10,2,1107,1,10,1,1001,14,10,3,8,1002,8,-1,10,1001,10,1,10,4,10,1008,8,0,10,4,10,1002,8,1,67,2,1009,7,10,3,8,1002,8,-1,10,1001,10,1,10,4,10,108,0,8,10,4,10,101,0,8,92,1,105,9,10,1006,0,89,1,108,9,10,3,8,1002,8,-1,10,1001,10,1,10,4,10,1008,8,1,10,4,10,1002,8,1,126,1,1101,14,10,1,1005,3,10,1006,0,29,1006,0,91,3,8,102,-1,8,10,101,1,10,10,4,10,108,1,8,10,4,10,1002,8,1,161,1,1,6,10,1006,0,65,2,106,13,10,1006,0,36,3,8,1002,8,-1,10,1001,10,1,10,4,10,1008,8,1,10,4,10,102,1,8,198,1,105,15,10,1,1004,0,10,3,8,1002,8,-1,10,1001,10,1,10,4,10,1008,8,0,10,4,10,101,0,8,228,2,1006,8,10,2,1001,16,10,3,8,102,-1,8,10,1001,10,1,10,4,10,108,0,8,10,4,10,1001,8,0,257,1006,0,19,2,6,10,10,2,4,13,10,2,1002,4,10,3,8,102,-1,8,10,1001,10,1,10,4,10,1008,8,1,10,4,10,1002,8,1,295,3,8,1002,8,-1,10,101,1,10,10,4,10,108,0,8,10,4,10,101,0,8,316,2,101,6,10,1006,0,84,2,1004,13,10,1,1109,3,10,101,1,9,9,1007,9,1046,10,1005,10,15,99,109,674,104,0,104,1,21101,387365315340,0,1,21102,369,1,0,1105,1,473,21101,666685514536,0,1,21102,380,1,0,1106,0,473,3,10,104,0,104,1,3,10,104,0,104,0,3,10,104,0,104,1,3,10,104,0,104,1,3,10,104,0,104,0,3,10,104,0,104,1,21102,1,46266346536,1,21102,427,1,0,1105,1,473,21101,235152829659,0,1,21101,438,0,0,1105,1,473,3,10,104,0,104,0,3,10,104,0,104,0,21102,838337188620,1,1,21101,461,0,0,1105,1,473,21102,988753429268,1,1,21102,1,472,0,1106,0,473,99,109,2,22101,0,-1,1,21101,40,0,2,21101,504,0,3,21102,494,1,0,1106,0,537,109,-2,2105,1,0,0,1,0,0,1,109,2,3,10,204,-1,1001,499,500,515,4,0,1001,499,1,499,108,4,499,10,1006,10,531,1101,0,0,499,109,-2,2106,0,0,0,109,4,2101,0,-1,536,1207,-3,0,10,1006,10,554,21102,1,0,-3,21202,-3,1,1,21201,-2,0,2,21102,1,1,3,21101,573,0,0,1105,1,578,109,-4,2105,1,0,109,5,1207,-3,1,10,1006,10,601,2207,-4,-2,10,1006,10,601,21201,-4,0,-4,1105,1,669,22101,0,-4,1,21201,-3,-1,2,21202,-2,2,3,21101,620,0,0,1106,0,578,22102,1,1,-4,21101,0,1,-1,2207,-4,-2,10,1006,10,639,21101,0,0,-1,22202,-2,-1,-2,2107,0,-3,10,1006,10,661,22101,0,-1,1,21102,661,1,0,106,0,536,21202,-2,-1,-2,22201,-4,-2,-4,109,-5,2106,0,0";

		public static void Part1()
		{
			var program  = Intcode.Computer.Parse(m_input);
			var computer = new Intcode.Day9Computer();

			computer.Initialize(program);

			var robot = new Robot(computer, new Hull());

			robot.Execute();
		}

		public static void Part2()
		{
			var program  = Intcode.Computer.Parse(m_input);
			var computer = new Intcode.Day9Computer();
			var hull     = new Hull();

			hull[0, 0] = true;

			computer.Initialize(program);

			var robot = new Robot(computer, hull);

			robot.Execute();

			//Console.WriteLine($"width: {hull.Width} height: {hull.Height}");
			for( var y = 0; y <= hull.Height; y++ ) {
				for( var x = 0; x <= hull.Width; x++ )
					Console.Write(hull[x, y] ? "*" : " ");

				Console.WriteLine();
			}
		}

		private class Robot
		{
			private Intcode.Computer m_computer;
			private int              m_x;
			private int              m_y;
			private Direction        m_direction;
			private Hull             m_hull;

			public Robot(Intcode.Computer computer, Hull hull)
			{
				m_computer = computer;
				m_hull     = hull;
			}

			public void Execute()
			{
				while( true ) {
					// order of things: input, paint, turn, move
					var interrupt = m_computer.Run();

					// if this isn't the first input, there should be two output available:
					//   first, the color to paint the current panel, and
					//   second, the direction to turn: 0=left, 1=right
					if( m_computer.OutputAvailable ) {
						// paint
						m_hull[m_x, m_y] = m_computer.GetOutput() == 1;

						// turn, move
						ExecuteMove(m_computer.GetOutput());
					}

					if( interrupt == Intcode.Computer.InterruptType.Terminated ) {
						// count panels painted, and we're done
						Console.WriteLine(m_hull.CountPaintedPanels());
						return;
					}

					// input
					m_computer.AddInput(m_hull[m_x, m_y] ? 1L : 0L);
				}
			}

			private void ExecuteMove(long turn)
			{
				// first turn, then move forward one
				m_direction = m_direction switch {
					Direction.Up => turn == 0 ? Direction.Left : Direction.Right,
					Direction.Right => turn == 0 ? Direction.Up : Direction.Down,
					Direction.Down => turn == 0 ? Direction.Right : Direction.Left,
					Direction.Left => turn == 0 ? Direction.Down : Direction.Up,
					_ => throw new Exception("not a direction"),
				};

				switch( m_direction ) {
					case Direction.Up :
						m_y--;
						break;
					case Direction.Right :
						m_x++;
						break;
					case Direction.Down :
						m_y++;
						break;
					case Direction.Left :
						m_x--;
						break;
				}
			}

			private enum Direction
			{
				Up,
				Right,
				Down,
				Left
			}
		}

		private class Hull
		{
			private Dictionary<int, Dictionary<int, bool>> m_panels = new Dictionary<int, Dictionary<int, bool>>();

			public bool this[int x, int y]
			{
				get {
					if( !m_panels.ContainsKey(x) )
						return false;

					if( !m_panels[x].ContainsKey(y) )
						return false;

					return m_panels[x][y];
				}
				set {
					if( !m_panels.ContainsKey(x) )
						m_panels.Add(x, new Dictionary<int, bool>());

					if( !m_panels[x].ContainsKey(y) )
						m_panels[x].Add(y, value);
					else
						m_panels[x][y] = value;
				}
			}

			public int Width => m_panels.Max(kvp => kvp.Key);

			public int Height => m_panels.Max(kvp => kvp.Value.Max(inner => inner.Key));

			public int CountPaintedPanels() => m_panels.Values.Sum(v => v.Count);
		}
	}
}
