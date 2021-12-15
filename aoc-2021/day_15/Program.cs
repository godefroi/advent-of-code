using Pathfinding.Models;

using System.Numerics;

var input  = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(args[0]), "input_sample.txt"));
var cells  = new List<Cell>();
var width  = input[0].Length;
var height = input.Length;

for (var i = 0; i < height; i++) {
	for (var j = 0; j < width; j++) {
		cells.Add(new Cell(j, i, int.Parse(input[i][j].ToString())));
	}
}

foreach (var row in cells.GroupBy(c => c.Y).OrderBy(g => g.Key)) {
	Console.WriteLine(string.Join("", row.OrderBy(c => c.X).Select(c => c.Risk.ToString())));
}

var start   = cells.Single(c => c.X == 0 && c.Y == 0);
var end     = cells.Single(c => c.X == width - 1 && c.Y == height - 1);

//// find the cost of the most-dumb path
//var best = cells.Where(c => c.X == 0 || c.Y == height - 1).Sum(c => c.Risk) - start.Risk;

//// now explore all paths, terminating if the path is worse than best
//static int? Explore(Cell from, List<Cell> cells, int width, int height, int risk, int best, HashSet<Cell> visited) {

//	foreach (var adj in FindAdjacents(from, cells, width, height)) {
//		// don't re-visit anything we've been to
//		if (visited.Contains(adj)) {
//			continue;
//		}


//	}
//}

//var nodes = new List<List<AStarSharp.Node>>();

//foreach (var row in cells.GroupBy(c => c.Y).OrderBy(g => g.Key)) {
//	var rlist = new List<AStarSharp.Node>();

//	foreach (var cell in row.OrderBy(c => c.X)) {
//		rlist.Add(new AStarSharp.Node(new Vector2(cell.X, cell.Y), true, cell.Risk));
//	}

//	nodes.Add(rlist);
//}

//var astar = new AStarSharp.Astar(nodes);

//var path = astar.FindPath(new Vector2(0, 0), new Vector2(width - 1, height - 1));

//foreach (var n in path) {
//	Console.WriteLine($"{n.Position.X},{n.Position.Y} ({n.Weight})");
//}

//Console.WriteLine(path.Sum(n => n.Weight));

var path = Pathfinding.Pathfinder.FindPath(new MyGrid(cells), new Position(0, 0), new Position(9, 9), Pathfinding.Types.DiagonalMovement.NEVER);

foreach (var p in path) {
	Console.WriteLine($"{p.X},{p.Y}");
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

record struct Cell(int X, int Y, int Risk);

class MyGrid : Pathfinding.Models.BaseGrid
{
	private List<Cell> _cells;

	public MyGrid(List<Cell> cells) : base(cells.Max(c => c.X + 1), cells.Max(c => c.Y + 1))
	{
		_cells = cells;
	}

	public override bool IsWalkableAt(Position p, bool final, params object[] args)
	{
		return true;
	}
}
