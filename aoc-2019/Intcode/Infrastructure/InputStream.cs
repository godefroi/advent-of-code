using System;
using System.Collections.Generic;

namespace aoc_2019.Intcode.Infrastructure
{
	internal class InputStream<T>
	{
		private Queue<T> m_input;

		public InputStream(IEnumerable<T> input)
		{
			m_input = new Queue<T>(input);
		}

		public T Next()
		{
			if( m_input.Count > 0 )
				return m_input.Dequeue();
			else
				throw new InputNeededException();
		}

		public void AddInput(T input) => m_input.Enqueue(input);
	}
}
