using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace day_01;

internal class SlidingWindow
{
	private int?[] _window;
	private int    _pos;

	public SlidingWindow(int size)
	{
		_window = new int?[size];
		_pos    = 0;
	}

	public void Add(int value)
	{
		_window[_pos++] = value;

		if (_pos >= _window.Length) {
			_pos = 0;
		}
	}

	public int Length => _window.Length;

	public int Sum() => _window.Where(v => v.HasValue).Sum().GetValueOrDefault();

	public int Count() => _window.Where(_v => _v.HasValue).Count();
}
