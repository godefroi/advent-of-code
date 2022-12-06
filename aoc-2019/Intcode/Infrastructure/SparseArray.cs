using System;
using System.Collections.Generic;
using System.Text;

namespace aoc_2019.Intcode.Infrastructure
{
	internal class SparseArray<T>
	{
		private Dictionary<long, T> m_values = new Dictionary<long, T>();

		public SparseArray(IEnumerable<T> initialValues)
		{
			var idx = 0;

			foreach( var v in initialValues )
				m_values[idx++] = v;
		}

		public int Length => m_values.Count;

		public T this[long index]
		{
			get {
				if( m_values.ContainsKey(index) )
					return m_values[index];
				else
					return default(T);
			}
			set {
				m_values[index] = value;
			}
		}
	}
}
