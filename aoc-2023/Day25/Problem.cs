using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Year2023.Day25;

public partial class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	private readonly static char[] _splitChars = [':', ' '];

	public static (long, long) Execute(string[] input)
	{
		var graph             = ParseGraph(input);
		var nodeKeys          = graph.Keys.ToArray();
		var findAdjacentNodes = (string adjacentTo) => graph[adjacentTo].Select(n => (n, 1f));
		var heuristic         = (string from, string to) => 1f;
		var paths             = new List<Stack<string>>();

		for (var j = 0; j < 5; j++) {
			Random.Shared.Shuffle(nodeKeys);

			for (var i = 0; i < nodeKeys.Length - 1; i += 2) {
				var path = AStar.FindPath(nodeKeys[i], nodeKeys[i + 1], findAdjacentNodes, heuristic) ?? throw new InvalidOperationException("No path found");
				//Console.WriteLine($"{nodeKeys[i]} -> {nodeKeys[i + 1]} len {path.Count}");
				//Console.WriteLine($"  {string.Join("->", path)}");
				paths.Add(path);
			}
		}

		var longestPaths = paths.OrderByDescending(s => s.Count).Take(nodeKeys.Length / 2);
		var edgeCounts   = longestPaths.SelectMany(GetEdges).GroupBy(e => e).ToDictionary(g => g.Key, g => g.Count());
		var side1        = default(string);
		var side2        = default(string);

		foreach (var (edge, count) in edgeCounts.OrderByDescending(kvp => kvp.Value).Take(3)) {
			Console.WriteLine($"cutting {edge} ({count})");
			graph[edge.Node1].Remove(edge.Node2);
			graph[edge.Node2].Remove(edge.Node1);
			side1 = edge.Node1;
			side2 = edge.Node2;
		}

		var nodes1 = FindAllNodes(graph, side1 ?? string.Empty);
		var nodes2 = FindAllNodes(graph, side2 ?? string.Empty);

		Console.WriteLine(nodes1.Count);
		Console.WriteLine(nodes2.Count);

		return (nodes1.Count * nodes2.Count, 0);
	}

	private static HashSet<string> FindAllNodes(Dictionary<string, List<string>> graph, string startNode)
	{
		var visited   = new HashSet<string>();
		var unvisited = new Queue<string>();

		unvisited.Enqueue(startNode);

		while (unvisited.TryDequeue(out var currentNode)) {
			visited.Add(currentNode);

			foreach (var neighbor in graph[currentNode]) {
				if (!visited.Contains(neighbor)) {
					unvisited.Enqueue(neighbor);
				}
			}
		}

		return visited;
	}

	private static IEnumerable<Edge> GetEdges(Stack<string> path)
	{
		var cur = path.Pop();

		while (path.Count > 0) {
			var next = path.Pop();

			yield return new Edge(cur, next);

			cur = next;
		}
	}

	private static void ContractEdge(Dictionary<string, List<string>> graph, string sourceNode, string destNode)
	{
		var sourceConns = graph[sourceNode];
		var destConns   = graph[destNode];

		// first, merge the destination's connections into the source's connections
		sourceConns.AddRange(destConns);

		// redirect any edges pointing to the dest to instead point at the source
		foreach (var conn in destConns) {
			var connSet = graph[conn];
			connSet.Remove(destNode);
			connSet.Add(sourceNode);
		}

		// remove loops from the source's connections
		sourceConns.RemoveAll(s => s == sourceNode);

		// remove the destination vertex
		graph.Remove(destNode);
	}

	private static (string From, string To) GetRandomEdge(Dictionary<string, List<string>> graph)
	{
		var fromKeys = graph.Keys.ToArray();
		var fromNode = fromKeys[Random.Shared.Next(graph.Count)];

		// keep trying until we find a node with some connections
		while (graph[fromNode].Count == 0) {
			fromNode = fromKeys[Random.Shared.Next(graph.Count)];
		}

		var toNode = graph[fromNode][Random.Shared.Next(graph[fromNode].Count)];

		return (fromNode, toNode);
	}

	private static int ComputeMinimumCut(Dictionary<string, List<string>> graph)
	{
		// karger's algorithm

		// keep cutting the graph until only two vertices remain
		while (graph.Count > 2) {
			var (src, dest) = GetRandomEdge(graph);

			ContractEdge(graph, src, dest);
		}

		// the number of edges remaining between the two vertices is the cut size
		return graph.Values.First().Count;
	}

	private static Dictionary<string, List<string>> ParseGraph(string[] input)
	{
		Span<Range> ranges = stackalloc Range[15];

		var connections = new Dictionary<string, List<string>>();

		for (var i = 0; i < input.Length; i++) {
			var lineSpan   = input[i].AsSpan();
			var rangeCount = lineSpan.SplitAny(ranges, _splitChars, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

			// first range is the component name, rest is the connections
			var node1 = lineSpan[ranges[0]].ToString();

			for (var j = 1; j < rangeCount; j++) {
				var node2 = lineSpan[ranges[j]].ToString();

				if (!connections.TryGetValue(node1, out var list1)) {
					list1 = new List<string>(15);
					connections.Add(node1, list1);
				}

				if (!connections.TryGetValue(node2, out var list2)) {
					list2 = new List<string>(15);
					connections.Add(node2, list2);
				}

				list1.Add(node2);
				list2.Add(node1);
			}
		}

		return connections;
	}

	private readonly record struct Edge
	{
		public string Node1 { get; init; }
		public string Node2 { get; init; }

		public Edge(string node1, string node2)
		{
			if (string.CompareOrdinal(node1, node2) < 0) {
				Node1 = node1;
				Node2 = node2;
			} else {
				Node1 = node2;
				Node2 = node1;
			}
		}
	}
}
