using System.Runtime.CompilerServices;

namespace AdventOfCode.Year2025.Day11;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		var d     = ParseNodesIntoDictionary(input);
		var youId = NodeId("you");
		var outId = NodeId("out");

		d.Add(outId, []);

		var sorted = TopologicalSort(d);

		var p1 = CountPaths(d, sorted, youId, outId);
		var p2 = CountPathsPart2(d, sorted);

		return (p1, p2);
	}

	private static long CountPaths(Dictionary<int, int[]> adjacencyList, int fromNode, int toNode)
	{
		var visited     = adjacencyList.Keys.Select(k => KeyValuePair.Create(k, false)).ToDictionary();
		var currentPath = new List<int>();
		var allPaths    = new List<int[]>();

		Search(fromNode, toNode);

		// foreach (var path in allPaths) {
		// 	Console.WriteLine(string.Join(" -> ", path.Select(NodeName)));
		// }

		return allPaths.Count;

		void Search(int from, int to)
		{
			if (visited[from]) {
				return;
			}

			visited[from] = true;
			currentPath.Add(from);

			if (from == to) {
				allPaths.Add([.. currentPath]);
				visited[from] = false;
				currentPath.RemoveAt(currentPath.Count - 1);
				return;
			}

			foreach (var next in adjacencyList[from]) {
				Search(next, to);
			}

			currentPath.RemoveAt(currentPath.Count - 1);
			visited[from] = false;
		}
	}

	private static long CountPaths(Dictionary<int, int[]> adjacencyList, List<int> sortedNode, int fromNode, int toNode)
	{
		var counts = sortedNode.ToDictionary(n => n, n => 0L);
		var pos    = 0;

		// skip through until fromNode
		for (; pos < sortedNode.Count; pos++) {
			if (sortedNode[pos] == fromNode) {
				break;
			}
		}

		counts[fromNode] = 1;

		// count paths from fromNode to toNode
		for (; pos < sortedNode.Count && sortedNode[pos] != toNode; pos++) {
			if (!counts.TryGetValue(sortedNode[pos], out var currCount)) {
				continue;
			}

			foreach (var cn in adjacencyList[sortedNode[pos]]) {
				counts[cn] += currCount;
			}
		}

		return counts.GetValueOrDefault(toNode);
	}

	private static long CountPathsPart2(Dictionary<int, int[]> adjacencyList, List<int> sortedNodes)
	{
		// the array in counts holds:
		//   - [0] "paths through neither"
		//   - [1] "paths through dac"
		//   - [2] "paths through fft"
		//   - [3] "paths through both"
		var counts = sortedNodes.ToDictionary(n => n, n => new[] { 0L, 0L, 0L, 0L });
		var svrId  = NodeId("svr");
		var fftId  = NodeId("fft");
		var dacId  = NodeId("dac");
		var outId  = NodeId("out");
		var pos    = 0;

		// skip through until fromNode
		for (; pos < sortedNodes.Count; pos++) {
			if (sortedNodes[pos] == svrId) {
				break;
			}
		}

		// increment the "neither" count for our starting position
		counts[svrId][0]++;

		// count paths from fromNode to toNode
		for (; pos < sortedNodes.Count && sortedNodes[pos] != outId; pos++) {
			if (!counts.TryGetValue(sortedNodes[pos], out var currCount)) {
				continue;
			}

			foreach (var cn in adjacencyList[sortedNodes[pos]]) {
				var childCount = counts[cn];

				if (cn == fftId) {
					childCount[0] = currCount[0];                                 // new "neither" is current "neither"
					childCount[1] = currCount[1];                                 // new "dac" is cuurent "dac"
					childCount[2] = currCount[0] + currCount[1] + childCount[2];  // new "fft" is current "neither" + current "dac" + child "both"
					childCount[3] = currCount[2] + currCount[3] + childCount[3];  // new "both" is current "fft" + current "both" + child "both"
				} else if (cn == dacId) {
					childCount[0] = currCount[0];
					childCount[1] = currCount[1] + currCount[0] + childCount[1];
					childCount[2] = currCount[2];
					childCount[3] = currCount[2] + currCount[3] + childCount[3];
				} else {
					childCount[0] += currCount[0];
					childCount[1] += currCount[1];
					childCount[2] += currCount[2];
					childCount[3] += currCount[3];
				}
			}
		}

		return counts.GetValueOrDefault(outId)?[3] ?? -1;
	}

	private static List<int> TopologicalSort(Dictionary<int, int[]> nodes)
	{
		var inDegrees = nodes.Keys.ToDictionary(k => k, k => 0);

		foreach (var edge in nodes.Values.SelectMany(v => v)) {
			inDegrees[edge]++;
		}

		var sortedNodes = new List<int>(nodes.Count);
		var nodeSet     = new Queue<int>(inDegrees.Where(kvp => kvp.Value == 0).Select(kvp => kvp.Key));

		while (nodeSet.Count > 0) {
			var thisNode = nodeSet.Dequeue();

			sortedNodes.Add(thisNode);

			foreach (var m in nodes[thisNode]) {
				// remove edge e from the graph, and if it has no other
				// incoming edges, then insert m into nodeSet
				if (--inDegrees[m] == 0) {
					nodeSet.Enqueue(m);
				}
			}
		}

		if (inDegrees.Values.Any(v => v > 10)) {
			throw new InvalidOperationException("Graph has at least one cycle");
		} else {
			return sortedNodes;
		}
	}

	private static string NodeName(int nodeInt) => $"{(char)(byte)((nodeInt & (0xFF << 16)) >> 16)}{(char)(byte)((nodeInt & (0xFF << 8)) >> 8)}{(char)(byte)(nodeInt & 0xFF)}";

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private static int NodeId(ReadOnlySpan<char> chars) => (((byte)chars[0]) << 16) | (((byte)chars[1]) << 8) | ((byte)chars[2]);

	private static Dictionary<int, int[]> ParseNodesIntoDictionary(string[] input)
	{
		Span<Range> ranges = stackalloc Range[32];

		var nodes = new Dictionary<int, int[]>(input.Length);

		foreach (var inputLine in input) {
			var lineSpan = inputLine.AsSpan();
			var thisNode = NodeId(lineSpan);

			lineSpan = lineSpan[5..];

			var outCount = lineSpan.Split(ranges, ' ');
			var outNodes = new int[outCount];

			for (var i = 0; i < outCount; i++) {
				var nodeSpan = lineSpan[ranges[i]];
				outNodes[i] = NodeId(nodeSpan);
			}

			nodes.Add(thisNode, outNodes);
		}

		return nodes;
	}

	public class Day11Tests
	{
		private readonly string[] _sampleInput = """
			aaa: you hhh
			you: bbb ccc
			bbb: ddd eee
			ccc: ddd eee fff
			ddd: ggg
			eee: out
			fff: out
			ggg: out
			hhh: ccc fff iii
			iii: out
			""".ReplaceLineEndings().Split(Environment.NewLine);

		private readonly string[] _sampleInputPart2 = """
			svr: aaa bbb
			aaa: fft
			fft: ccc
			bbb: tty
			tty: ccc
			ccc: ddd eee
			ddd: hub
			hub: fff
			eee: dac
			dac: fff
			fff: ggg hhh
			ggg: out
			hhh: out
			""".ReplaceLineEndings().Split(Environment.NewLine);

		[Test]
		public async Task Part1Works()
		{
			var d     = ParseNodesIntoDictionary(_sampleInput);
			var youId = NodeId("you");
			var outId = NodeId("out");

			d.Add(outId, []);

			var sorted = TopologicalSort(d);

			var p1 = CountPaths(d, sorted, youId, outId);

			await Assert.That(p1).IsEqualTo(5);
		}

		[Test]
		public async Task Part2Works()
		{
			var d = ParseNodesIntoDictionary(_sampleInputPart2);

			d.Add(NodeId("out"), []);

			var sorted = TopologicalSort(d);

			var p2 = CountPathsPart2(d, sorted);

			await Assert.That(p2).IsEqualTo(2);
		}

		[Test]
		public async Task TopologicalSortWithSampleInput()
		{
			var d = ParseNodesIntoDictionary(_sampleInput);

			d.Add(NodeId("out"), []);

			var sorted = TopologicalSort(d);

			// Verify all nodes are included
			await Assert.That(sorted.Count).IsEqualTo(d.Count);

			// Verify topological order: no node should appear before its predecessors
			var nodeIndex = sorted.Select((node, index) => (node, index)).ToDictionary(x => x.node, x => x.index);

			foreach (var (node, successors) in d) {
				var nodePos = nodeIndex[node];
				foreach (var successor in successors) {
					await Assert.That(nodeIndex[successor]).IsGreaterThan(nodePos);
				}
			}
		}

		[Test]
		public async Task TopologicalSortWithSimpleData()
		{
			// Simple linear chain: aaa -> bbb -> ccc -> ddd
			var simpleInput = new[] {
				"aaa: bbb",
				"bbb: ccc",
				"ccc: ddd",
			};

			var d = ParseNodesIntoDictionary(simpleInput);

			d.Add(NodeId("ddd"), []);

			var sorted = TopologicalSort(d);

			// Verify all nodes are included
			await Assert.That(sorted.Count).IsEqualTo(4);

			// Verify the order is exactly as expected
			var names = sorted.Select(NodeName).ToList();
			await Assert.That(names).IsEquivalentTo(["aaa", "bbb", "ccc", "ddd"]);
		}
	}
}
