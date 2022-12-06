using System;
using System.Collections.Generic;
using System.Text;

namespace aoc_2019
{
	internal static class ArrayExtensions
	{
		public static void Deconstruct<T>(this T[] array, out T item0, out T item1)
		{
			item0 = default(T);
			item1 = default(T);

			if( array != null ) {
				if( array.Length > 1 )
					item1 = array[1];

				if( array.Length > 0 )
					item0 = array[0];
			}
		}

		public static void Deconstruct<T>(this IEnumerable<T> items, out T item0, out T item1)
		{
			item0 = default(T);
			item1 = default(T);

			using( var en = items.GetEnumerator() ) {
				if( !en.MoveNext() )
					return;

				item0 = en.Current;

				if( !en.MoveNext() )
					return;

				item1 = en.Current;
			}
		}
	}
}
