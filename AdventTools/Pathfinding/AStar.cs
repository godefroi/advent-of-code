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

	public static Stack<N>? FindPath<N>(N start, N goal, Func<N, IEnumerable<(N node, float weight)>> retrieveAdjacentNodes, Func<N, N, float> heuristic) where N : notnull, IEquatable<N> => FindPath(start, goal, retrieveAdjacentNodes, heuristic, EqualityComparer<N>.Default);

	public static Stack<N>? FindPath<N>(N start, N goal, Func<N, IEnumerable<(N node, float weight)>> retrieveAdjacentNodes, Func<N, N, float> heuristic, IEqualityComparer<N> equalityComparer) where N : notnull
	{
		var openSet  = new PriorityQueue<N, float>();
		var cameFrom = new Dictionary<N, N>();
		var gScore   = new ScoreSet<N>();
		var fScore   = new ScoreSet<N>();

		gScore[start] = 0;

		openSet.Enqueue(start, fScore[start]);

		while (openSet.Count > 0) {
			var current = openSet.Dequeue();

			if (equalityComparer.Equals(current, goal)) {
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

	private static Stack<N> ReconstructPath<N>(N? current, Dictionary<N, N> cameFrom) where N : notnull
	{
		ArgumentNullException.ThrowIfNull(current);

		var path = new Stack<N>();

		path.Push(current);

		while (cameFrom.TryGetValue(current, out current)) {
			path.Push(current);
		}

		return path;
	}
}
