using System;
using System.Collections.Generic;
using System.Threading;

namespace aoc_2019.Intcode.Infrastructure
{
	internal class InputStream<T>
	{
		private Queue<T>       m_input;
		private AutoResetEvent m_event = new AutoResetEvent(false);
		private bool           m_waiting;

		public InputStream(IEnumerable<T> input)
		{
			m_input = new Queue<T>(input);
		}

		public bool AwaitingInput
		{
			get {
				lock( this )
					return m_waiting;
			}
			private set {
				lock( this )
					m_waiting = value;
			}
		}

		public T Next()
		{
			while( true ) {
				lock( this ) {
					if( m_input.Count > 0 ) {
						AwaitingInput = false;
						return m_input.Dequeue();
					}
				}

				// block here until input is available
				AwaitingInput = true;
				m_event.WaitOne();
			}
		}

		public void AddInput(T input)
		{
			lock( this )
				m_input.Enqueue(input);

			m_event.Set();
		}
	}
}
