using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode;

public static class AStar
{
	//public static Stack<Coordinate>? FindPath(Coordinate start, Coordinate goal, Func<Coordinate, IEnumerable<(Coordinate coordinate, float weight)>> retrieveAdjacentNodes, Func<Coordinate, Coordinate, float>? heuristic = null)
	//{
	//	heuristic ??= Heuristic;

	//	var openSet  = new PriorityQueue<Coordinate, float>();
	//	var cameFrom = new Dictionary<Coordinate, Coordinate>();
	//	var gScore   = new ScoreSet<Coordinate>();
	//	var fScore   = new ScoreSet<Coordinate>();

	//	gScore[start] = 0;

	//	openSet.Enqueue(start, fScore[start]);

	//	while (openSet.Count > 0) {
	//		var current = openSet.Dequeue();

	//		if (current == goal) {
	//			// we're done... reconstruct the path and get out
	//			// return reconstruct_path(cameFrom, current)
	//			var path = new Stack<Coordinate>();
				
	//			path.Push(current);

	//			while (cameFrom.TryGetValue(current, out current)) {
	//				path.Push(current);
	//			}

	//			return path;
	//		}

	//		foreach (var (neighbor, weight) in retrieveAdjacentNodes(current)) {
	//			var tentativeGScore = gScore[current] + weight;

	//			if (tentativeGScore < gScore[neighbor]) {
	//				cameFrom[neighbor] = current;
	//				gScore[neighbor] = tentativeGScore;
	//				fScore[neighbor] = tentativeGScore + heuristic(neighbor, goal);

	//				if (!openSet.UnorderedItems.Any(i => i.Element == neighbor)) {
	//					openSet.Enqueue(neighbor, fScore[neighbor]);
	//				}
	//			}
	//		}
	//	}

	//	// open set is empty bug goal was never reached; pathfinding failed
	//	return null;
	//}

	public static Stack<Coordinate>? FindPath(Coordinate start, Coordinate goal, Func<Coordinate, IEnumerable<(Coordinate coordinate, float weight)>> retrieveAdjacentNodes, Func<Coordinate, Coordinate, float>? heuristic = null) =>
		FindPath(start, goal, retrieveAdjacentNodes, heuristic ?? Heuristic);

	public static Stack<N>? FindPath<N>(N start, N goal, Func<N, IEnumerable<(N node, float weight)>> retrieveAdjacentNodes, Func<N, N, float> heuristic) where N : notnull, IEquatable<N>
	{
		var openSet  = new PriorityQueue<N, float>();
		var cameFrom = new Dictionary<N, N>();
		var gScore   = new ScoreSet<N>();
		var fScore   = new ScoreSet<N>();

		gScore[start] = 0;

		openSet.Enqueue(start, fScore[start]);

		while (openSet.Count > 0) {
			var current = openSet.Dequeue();

			if (current.Equals(goal)) {
				// we're done... reconstruct the path and get out
				// return reconstruct_path(cameFrom, current)
				var path = new Stack<N>();
				
				path.Push(current);

				while (cameFrom.TryGetValue(current, out current)) {
					path.Push(current);
				}

				return path;
			}

			foreach (var (neighbor, weight) in retrieveAdjacentNodes(current)) {
				var tentativeGScore = gScore[current] + weight;

				if (tentativeGScore < gScore[neighbor]) {
					cameFrom[neighbor] = current;
					gScore[neighbor] = tentativeGScore;
					fScore[neighbor] = tentativeGScore + heuristic(neighbor, goal);

					if (!openSet.UnorderedItems.Any(i => i.Element.Equals(neighbor))) {
						openSet.Enqueue(neighbor, fScore[neighbor]);
					}
				}
			}
		}

		// open set is empty but goal was never reached; pathfinding failed
		return null;
	}

	private class ScoreSet<N> : IDictionary<N, float> where N : notnull
	{
		private readonly Dictionary<N, float> _coordinates = new();

		public float this[N key] {
			get => _coordinates.TryGetValue(key, out var value) ? value : float.PositiveInfinity;
			set => ((IDictionary<N, float>)_coordinates)[key] = value;
		}

		public ICollection<N> Keys => _coordinates.Keys;

		public ICollection<float> Values => _coordinates.Values;

		public int Count => _coordinates.Count;

		public bool IsReadOnly => ((ICollection<KeyValuePair<N, float>>)_coordinates).IsReadOnly;

		public void Add(N key, float value) => _coordinates.Add(key, value);

		public void Add(KeyValuePair<N, float> item) => ((ICollection<KeyValuePair<N, float>>)_coordinates).Add(item);

		public void Clear() => _coordinates.Clear();

		public bool Contains(KeyValuePair<N, float> item) => ((ICollection<KeyValuePair<N, float>>)_coordinates).Contains(item);

		public bool ContainsKey(N key) => _coordinates.ContainsKey(key);

		public void CopyTo(KeyValuePair<N, float>[] array, int arrayIndex) => ((ICollection<KeyValuePair<N, float>>)_coordinates).CopyTo(array, arrayIndex);

		public IEnumerator<KeyValuePair<N, float>> GetEnumerator() => ((IEnumerable<KeyValuePair<N, float>>)_coordinates).GetEnumerator();

		public bool Remove(N key) => _coordinates.Remove(key);

		public bool Remove(KeyValuePair<N, float> item) => ((ICollection<KeyValuePair<N, float>>)_coordinates).Remove(item);

		public bool TryGetValue(N key, [MaybeNullWhen(false)] out float value) => _coordinates.TryGetValue(key, out value);

		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_coordinates).GetEnumerator();
	}

	private static float Heuristic(Coordinate from, Coordinate to) => Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
}
