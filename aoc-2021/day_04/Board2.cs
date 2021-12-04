using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace day_04;

internal class Board2
{
	private (int Number, bool Selected)[,] _grid;

	public Board2(int rows = 5, int cols = 5)
	{
		_grid = new (int Number, bool Selected)[rows, cols];
	}

	public int Score => Flatten(_grid).Where(x => !x.Selected).Sum(x => x.Number);

	public void Mark(int number)
	{

	}

	private static IEnumerable<T> Flatten<T>(T[,] input)
	{
		var b1 = input.GetUpperBound(0);
		var b2 = input.GetUpperBound(1);

		for (var i = 0; i < b1; i++) {
			for (var j = 0; j < b2; j++) {
				yield return input[i, j];
			}
		}
	}
}
