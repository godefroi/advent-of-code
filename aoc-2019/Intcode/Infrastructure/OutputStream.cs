using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace aoc_2019.Intcode.Infrastructure
{
	internal class OutputStream<T>
	{
		private Queue<T> m_output = new Queue<T>();
		private AutoResetEvent m_event = new AutoResetEvent(false);

		public bool OutputAvailable
		{
			get {
				lock( this )
					return m_output.Count > 0;
			}
		}

		internal void AddOutput(T output)
		{
			lock( this )
				m_output.Enqueue(output);

			m_event.Set();
		}

		public T Get()
		{
			while( true ) {
				lock( this ) {
					if( m_output.Count > 0 )
						return m_output.Dequeue();
				}

				m_event.WaitOne();
			}
		}
	}
}
