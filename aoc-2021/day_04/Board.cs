namespace day_04;

internal class Board
{
	private readonly int    _width;
	private readonly int    _height;
	private readonly int?[] _grid;

	public Board(int[,] grid)
	{
		_width  = grid.GetLength(0);
		_height = grid.GetLength(1);
		_grid   = new int?[_width * _height];

		for (var x = 0; x < _width; x++) {
			for (var y = 0; y < _height; y++) {
				_grid[(x * _height) + y] = grid[x, y];
			}
		}
	}

	public Board(int width, int height)
	{
		_width  = width;
		_height = height;
		_grid   = new int?[width * height];
	}

	public Board(IEnumerable<int> numbers)
	{
		_grid   = numbers.Select(n => new int?(n)).ToArray();
		_width  = (int)Math.Sqrt(_grid.Length);
		_height = _width;

		if (_width * _height != _grid.Length) {
			throw new InvalidOperationException("The grid isn't square...");
		}
	}

	public int? Mark(int number)
	{
		for (var i = 0; i < _grid.Length; i++) {
			if (_grid[i] != null && _grid[i] == number) {
				_grid[i] = null;
			}
		}

		return Enumerable.Range(0, _height).Any(row => _grid.Skip(_width * row).Take(_width).All(v => v == null))
			|| Enumerable.Range(0, _width).Any(col => _grid.Chunk(_width).Select(c => c[col]).All(v => v == null)) ? _grid.Where(x => x.HasValue).Sum() : null;
	}
}
