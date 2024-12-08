namespace AdventOfCode;

public static class EnumerableExtensions
{
	public static IEnumerable<(T1?, T2?, bool first)> Weave<T1, T2>(this IEnumerable<T1> first, IEnumerable<T2> second)
	{
		using var e1 = first.GetEnumerator();
		using var e2 = second.GetEnumerator();

		while (e1.MoveNext()) {
			yield return (e1.Current, default(T2), true);

			if (e2.MoveNext()) {
				yield return (default(T1), e2.Current, false);
			}
		}

		while (e2.MoveNext()) {
			yield return (default(T1), e2.Current, false);
		}
	}
}
