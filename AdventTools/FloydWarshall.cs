using System.Numerics;

namespace AdventOfCode;

public class FloydWarshall
{
	/// <summary>
	/// Compute the shortest distance between all nodes of a directed, weighted graph with positive or negative edge weights (but no negative cycles)
	/// </summary>
	/// <typeparam name="T">The type of <see cref="Edge{T}"/> being used</typeparam>
	/// <typeparam name="W">The type of weight being used</typeparam>
	/// <param name="edges">The set of weighted edges in the graph</param>
	/// <remarks>
	/// https://en.wikipedia.org/wiki/Floyd%E2%80%93Warshall_algorithm#Algorithm
	/// https://olegkarasik.wordpress.com/2021/04/25/implementing-floyd-warshall-algorithm-for-solving-all-pairs-shortest-paths-problem-in-c/
	/// </remarks>
	public static Dictionary<Edge<T>, W> ComputeDistances<T, W>(IEnumerable<WeightedEdge<T, W>> edges) where W : INumber<W>
	{
		var nodeSet = new HashSet<T>();
		var dist = new Dictionary<Edge<T>, W>();

		// set the distance for the edges we know
		foreach (var edge in edges) {
			nodeSet.Add(edge.Edge.From);
			nodeSet.Add(edge.Edge.To);
			dist.Add(edge.Edge, edge.Weight);
		}

		// nodes are all zero distance from themselves
		foreach (var node in nodeSet) {
			dist.Add(new Edge<T>(node, node), W.Zero);
		}

		foreach (var k in nodeSet) {
			foreach (var i in nodeSet) {
				foreach (var j in nodeSet) {
					var ijEdge = new Edge<T>(i, j);
					var ikEdge = new Edge<T>(i, k);
					var kjEdge = new Edge<T>(k, j);

					//dist[i, j] = Math.Min(dist[i, k], dist[i, k] + dist[k, j]);
					if (dist.TryGetValue(ijEdge, out var ijWeight)) {
						if (dist.TryGetValue(ikEdge, out var ikWeight) && dist.TryGetValue(kjEdge, out var kjWeight)) {
							if (ijWeight > ikWeight + kjWeight) {
								dist[ijEdge] = ikWeight + kjWeight;
							}
						}
					} else {
						// dist[i, j] is infinite... if we have weights for i,k and k,j then add them, otherwise it stays infinite
						if (dist.TryGetValue(ikEdge, out var ikWeight) && dist.TryGetValue(kjEdge, out var kjWeight)) {
							dist.Add(ijEdge, ikWeight + kjWeight);
						}
					}
				}
			}
		}

		return dist;
	}

	public readonly record struct Edge<T>(T From, T To);

	public readonly record struct WeightedEdge<T, W>(Edge<T> Edge, W Weight) where W : INumber<W>
	{
		public WeightedEdge(T From, T To, W Weight) : this(new Edge<T>(From, To), Weight) { }
	}
}
