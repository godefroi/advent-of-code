using System;
using System.Collections.Generic;

namespace aoc_2019.Intcode.Infrastructure
{
	internal class OutputStream<T>
	{
		private Queue<T> m_output = new Queue<T>();

		public bool OutputAvailable => m_output.Count > 0;

		internal void AddOutput(T output)
		{
			m_output.Enqueue(output);

			//throw new OutputReadyException();
		}

		public T Get() => m_output.Dequeue();
	}
}
