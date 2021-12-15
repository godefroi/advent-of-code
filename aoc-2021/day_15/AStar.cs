using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace day_15;

internal class AStar
{
	/*

var start   = cells.Single(c => c.X == 0 && c.Y == 0);
var end     = cells.Single(c => c.X == width - 1 && c.Y == height - 1);
var path    = new Stack<Cell>();
var open    = new List<Cell>() { start };
var closed  = new List<Cell>();
var current = start;

while (open.Count > 0 && !closed.Contains(end)) {
	current = open[0];
	open.RemoveAt(0);
	closed.Add(current);

	foreach (var cell in FindAdjacents(current, cells, width, height).Where(c => !closed.Contains(c) && !open.Contains(c))) {

	}
}

static IEnumerable<Cell> FindAdjacents(Cell current, List<Cell> cells, int width, int height)
{
	// above
	if (current.X > 0) {
		yield return cells.Single(c => c.X == current.X - 1 && c.Y == current.Y);
	}

	// left
	if (current.Y > 0) {
		yield return cells.Single(c => c.Y == current.Y - 1 && c.X == current.X);
	}

	// below
	if (current.X < height - 1) {
		yield return cells.Single(c => c.X == current.X + 1 && c.Y == current.Y);
	}

	// right
	if (current.Y < width - 1) {
		yield return cells.Single(c => c.Y == current.Y + 1 && c.X == current.X);
	}
}

Console.WriteLine(start);
Console.WriteLine(end);
*/

}
