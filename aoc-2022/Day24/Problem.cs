namespace AdventOfCode.Year2022.Day24;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Main, typeof(Problem), null);

	public static (long, long) Main(string[] input)
	{
		var lines = input.Select(line => line.ToCharArray()).ToArray();
		var board = new char[lines[0].Length, lines.Length];
		var width = Math.Max(-1, lines[0].Length);

		for (var y = 0; y < lines.Length; y++) {
			for (var x = 0; x < width; x++) {
				board[x, y] = lines[y][x];
			}
		}

		var entrance  = (1, 0);
		var exit      = (width - 2, lines.Length - 1);
		var part1     = FindPath(board, entrance, exit,  0);
		var tripBack  = FindPath(board, exit, entrance,  part1);
		var tripForth = FindPath(board, entrance, exit, part1 + tripBack);

		return (part1, part1 + tripBack + tripForth);
	}

	private static char[,] CreateBoardAfter(char[,] board, int minutes)
	{
		var width    = board.GetLength(0);
		var height   = board.GetLength(1);
		var newBoard = new char[width, height];

		for (var y = 0; y < height; y++) {
			for (var x = 0; x < width; x++) {
				newBoard[x, y] = board[x, y] == '#' ? '#' : '.';
			}
		}

		for (var y = 0; y < height; y++) {
			for (var x = 0; x < width; x++) {
				if (board[x, y] == '>') {
					newBoard[(x - 1 + minutes) % (width - 2) + 1, y] = '>';
				}

				if (board[x, y] == '<') {
					newBoard[((x - 1 - minutes) % (width - 2) + (width - 2)) % (width -2 ) + 1, y] = '<';
				}

				if (board[x, y] == 'v') {
					newBoard[x, (y - 1 + minutes) % (height - 2) + 1] = 'v';
				}

				if (board[x, y] == '^') {
					newBoard[x, ((y - 1 - minutes) % (height - 2) + (height - 2)) % (height - 2) + 1] = '^';
				}
			}
		}

		return newBoard;
	}

	private static int FindPath(char[,] initialBoard, (int x, int y) start, (int x, int y) destination, int startMinutes)
	{
		var cachedBoards    = new Dictionary<int, char[,]>();
		var startNode       = new PathNode(start.x, start.y, startMinutes);
		var destinationNode = new PathNode(destination.x, destination.y, -1);
		var heuristic       = (PathNode from, PathNode to) => (float)(Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y));
		var maxX            = initialBoard.GetLength(0) - 1;
		var maxY            = initialBoard.GetLength(1) - 1;

		IEnumerable<(PathNode, float)> CalculateAdjacencies(PathNode from)
		{
			var minutes = from.Minutes + 1;

			if (!cachedBoards.TryGetValue(minutes, out var board)) {
				cachedBoards.Add(minutes, board = CreateBoardAfter(initialBoard, minutes));
			}

			var upX    = from.X;     var upY    = from.Y - 1;
			var downX  = from.X;     var downY  = from.Y + 1;
			var leftX  = from.X - 1; var leftY  = from.Y;
			var rightX = from.X + 1; var rightY = from.Y;

			if (board[from.X, from.Y] == '.') {
				yield return (new PathNode(from.X, from.Y, minutes), 1);
			}

			if (leftX >= 0 && board[leftX, leftY] == '.') {
				yield return (new PathNode(leftX, leftY, destination.x == leftX && destination.y == leftY ? -1 : minutes), 1);
			}

			if (rightX <= maxX && board[rightX, rightY] == '.') {
				yield return (new PathNode(rightX, rightY, destination.x == rightX && destination.y == rightY ? -1 : minutes), 1);
			}

			if (upY >= 0 && board[upX, upY] == '.') {
				yield return (new PathNode(upX, upY, destination.x == upX && destination.y == upY ? -1 : minutes), 1);
			}

			if (downY <= maxY && board[downX, downY] == '.') {
				yield return (new PathNode(downX, downY, destination.x == downX && destination.y == downY ? -1 : minutes), 1);
			}
		};

		var path = AStar.FindPath(startNode, destinationNode, CalculateAdjacencies, heuristic);

		if (path == null) {
			throw new InvalidOperationException("Unable to find a path.");
		}

		return path.Count - 1;
	}

	private static void Draw(char[,] board)
	{
		var sb     = new StringBuilder();
		var width  = board.GetLength(0);
		var height = board.GetLength(1);

		for (var y = 0; y < height; y++) {
			for (var x = 0; x < width; x++) {
				sb.Append(board[x, y]);
			}

			if (y < height - 1) {
				sb.AppendLine();
			}
		}

		Console.WriteLine(sb);
	}

	private readonly record struct PathNode(int X, int Y, int Minutes);
}
