namespace AdventOfCode;

public class Dijkstra
{
	public static IReadOnlyDictionary<N, float> FindDistances<N>(N start, Func<N, IEnumerable<N>> findAdjacentNodes, float maxDist = float.MaxValue) where N : notnull, IEquatable<N>
	{
		var dist     = new ScoreSet<N>();
		var queue    = new PriorityQueue<N, float>();
		var queueSet = new HashSet<N>();
		var cameFrom = new Dictionary<N, N>();

		dist[start] = 0;

		queue.Enqueue(start, dist[start]);
		queueSet.Add(start);

		while (queue.TryDequeue(out var current, out _)) {
			queueSet.Remove(current);

			foreach (var neighbor in findAdjacentNodes(current)) {
				var alt = dist[current] + 1; // the +1 here is the "distance between current and neighbor"

				// ignore anything with a higher distance than maxDist
				if (alt > maxDist) {
					continue;
				}

				if (alt < dist[neighbor]) {
					dist[neighbor]     = alt;
					cameFrom[neighbor] = current;

					if (!queueSet.Contains(neighbor)) {
						queue.Enqueue(neighbor, alt);
						queueSet.Add(neighbor);
					}
				}
			}
		}

		return dist.AsReadOnly();
	}

	public class Tests
	{
		[Fact]
		public void SimpleDistanceFindingWorks()
		{
			// ...
			// ...
			// ...
			Func<Coordinate, IEnumerable<Coordinate>> findAdjacentNodes = from => {
				return from switch {
					//                              N       S       E       W
					(1, 1) => new Coordinate[] {         (1, 2), (2, 1)         },
					(1, 2) => new Coordinate[] { (1, 1), (1, 3), (2, 2)         },
					(1, 3) => new Coordinate[] { (1, 2),         (2, 3)         },

					(2, 1) => new Coordinate[] {         (2, 2), (3, 1), (1, 1) },
					(2, 2) => new Coordinate[] { (2, 1), (2, 3), (3, 2), (1, 2) },
					(2, 3) => new Coordinate[] { (2, 2),         (3, 3), (1, 3) },

					(3, 1) => new Coordinate[] {         (3, 2),         (2, 1) },
					(3, 2) => new Coordinate[] { (3, 1), (3, 3),         (2, 2) },
					(3, 3) => new Coordinate[] { (3, 2),                 (2, 3) },

					_ => throw new InvalidOperationException(),
				};
			};

			var distances = FindDistances((2, 2), findAdjacentNodes);

			Assert.Collection(distances.OrderBy(d => d.Key.Y).ThenBy(d => d.Key.X),
				kvp => { Assert.Equal(new Coordinate(1, 1), kvp.Key); Assert.Equal(2f, kvp.Value); },
				kvp => { Assert.Equal(new Coordinate(2, 1), kvp.Key); Assert.Equal(1f, kvp.Value); },
				kvp => { Assert.Equal(new Coordinate(3, 1), kvp.Key); Assert.Equal(2f, kvp.Value); },
				kvp => { Assert.Equal(new Coordinate(1, 2), kvp.Key); Assert.Equal(1f, kvp.Value); },
				kvp => { Assert.Equal(new Coordinate(2, 2), kvp.Key); Assert.Equal(0f, kvp.Value); },
				kvp => { Assert.Equal(new Coordinate(3, 2), kvp.Key); Assert.Equal(1f, kvp.Value); },
				kvp => { Assert.Equal(new Coordinate(1, 3), kvp.Key); Assert.Equal(2f, kvp.Value); },
				kvp => { Assert.Equal(new Coordinate(2, 3), kvp.Key); Assert.Equal(1f, kvp.Value); },
				kvp => { Assert.Equal(new Coordinate(3, 3), kvp.Key); Assert.Equal(2f, kvp.Value); }
			);
		}
	}
}
