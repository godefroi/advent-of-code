using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode;

public static class AStar
{
	public static Stack<Coordinate>? FindPath(Coordinate start, Coordinate goal, Func<Coordinate, IEnumerable<(Coordinate coordinate, float weight)>> retrieveAdjacentNodes, Func<Coordinate, Coordinate, float>? heuristic = null)
	{
		heuristic ??= Heuristic;

		var openSet  = new PriorityQueue<Coordinate, float>();
		var cameFrom = new Dictionary<Coordinate, Coordinate>();
		var gScore   = new ScoreSet();
		var fScore   = new ScoreSet();

		gScore[start] = 0;

		openSet.Enqueue(start, fScore[start]);

		while (openSet.Count > 0) {
			var current = openSet.Dequeue();

			if (current == goal) {
				// we're done... reconstruct the path and get out
				// return reconstruct_path(cameFrom, current)
				var path = new Stack<Coordinate>();
				
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

					if (!openSet.UnorderedItems.Any(i => i.Element == neighbor)) {
						openSet.Enqueue(neighbor, fScore[neighbor]);
					}
				}
			}
		}

		// open set is empty bug goal was never reached; pathfinding failed
		return null;
	}

	private class ScoreSet : IDictionary<Coordinate, float>
	{
		private readonly Dictionary<Coordinate, float> _coordinates = new();

		public float this[Coordinate key] {
			get => _coordinates.TryGetValue(key, out var value) ? value : float.PositiveInfinity;
			set => ((IDictionary<Coordinate, float>)_coordinates)[key] = value;
		}

		public ICollection<Coordinate> Keys => _coordinates.Keys;

		public ICollection<float> Values => _coordinates.Values;

		public int Count => _coordinates.Count;

		public bool IsReadOnly => ((ICollection<KeyValuePair<Coordinate, float>>)_coordinates).IsReadOnly;

		public void Add(Coordinate key, float value) => _coordinates.Add(key, value);

		public void Add(KeyValuePair<Coordinate, float> item) => ((ICollection<KeyValuePair<Coordinate, float>>)_coordinates).Add(item);

		public void Clear() => _coordinates.Clear();

		public bool Contains(KeyValuePair<Coordinate, float> item) => ((ICollection<KeyValuePair<Coordinate, float>>)_coordinates).Contains(item);

		public bool ContainsKey(Coordinate key) => _coordinates.ContainsKey(key);

		public void CopyTo(KeyValuePair<Coordinate, float>[] array, int arrayIndex) => ((ICollection<KeyValuePair<Coordinate, float>>)_coordinates).CopyTo(array, arrayIndex);

		public IEnumerator<KeyValuePair<Coordinate, float>> GetEnumerator() => ((IEnumerable<KeyValuePair<Coordinate, float>>)_coordinates).GetEnumerator();

		public bool Remove(Coordinate key) => _coordinates.Remove(key);

		public bool Remove(KeyValuePair<Coordinate, float> item) => ((ICollection<KeyValuePair<Coordinate, float>>)_coordinates).Remove(item);

		public bool TryGetValue(Coordinate key, [MaybeNullWhen(false)] out float value) => _coordinates.TryGetValue(key, out value);

		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_coordinates).GetEnumerator();
	}

	public readonly record struct Coordinate(int x, int y)
	{
		public static implicit operator Coordinate((int x, int y) tuple) => new Coordinate(tuple.x, tuple.y);
	}

	private static float Heuristic(Coordinate from, Coordinate to) => Math.Abs(from.x - to.x) + Math.Abs(from.y - to.y);
}
