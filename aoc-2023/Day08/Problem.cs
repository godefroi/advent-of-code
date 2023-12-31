using System.Diagnostics;

namespace AdventOfCode.Year2023.Day08;

public partial class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, null);

	public static (long, long) Execute(string[] input)
	{
		var (curNode, nodes) = ParseNetwork(input.AsSpan()[2..]);
		var instructions     = input[0].ToCharArray();
		var part1            = curNode == null ? -1 : CountSteps(curNode, instructions, n => n.Name == "ZZZ");
		var sw = Stopwatch.StartNew();
		var dists            = nodes.Select(n => CountSteps(n, instructions, n => n.Name[2] == 'Z'));
		sw.Stop();
		var darr = dists.Select(d => (long)d).ToArray();
		var p1time = sw.Elapsed.TotalMilliseconds;
		sw.Restart();
		var part2            = LeastCommonMultiple(darr);
		var p2time = sw.Elapsed.TotalMilliseconds;

		// foreach (var node in nodes) {
		// 	Console.WriteLine($"{node.Name} -> {CountSteps(node, instructions, n => n.Name[2] == 'Z')}");
		// }
		Console.WriteLine($"part 1: {p1time} part 2: {p2time}");

		return (part1, part2);
	}

	private static int CountSteps(MapNode startNode, char[] instructions, Func<MapNode, bool> endCondition)
	{
		var stepCount = 0;
		var instrIdx  = 0;

		while (!endCondition(startNode)) {
			startNode = instructions[instrIdx] == 'R' ? startNode.Right! : startNode.Left!;

			stepCount++;

			if (++instrIdx > instructions.Length - 1) {
				instrIdx = 0;
			}
		}

		return stepCount;
	}

	private static (MapNode? AAA, List<MapNode> StartNodes) ParseNetwork(ReadOnlySpan<string> nodeDefinitions)
	{
		var nodeDict = new Dictionary<string, MapNode>();

		foreach (var nodeDefinition in nodeDefinitions) {
			var nodeName  = nodeDefinition[..3];
			var leftName  = nodeDefinition[7..10];
			var rightName = nodeDefinition[12..15];
			var haveNamed = nodeDict.TryGetValue(nodeName, out var namedNode);
			var haveLeft  = nodeDict.TryGetValue(leftName, out var leftNode);
			var haveRight = nodeDict.TryGetValue(rightName, out var rightNode);

			if (!haveNamed) {
				namedNode = new MapNode(nodeName);
				nodeDict.Add(nodeName, namedNode);
			}

			if (!haveLeft) {
				leftNode = new MapNode(leftName);
				nodeDict.Add(leftName, leftNode);
			}

			if (!haveRight && rightName == leftName) {
				rightNode = leftNode;
			} else if (!haveRight && rightName != leftName) {
				rightNode = new MapNode(rightName);
				nodeDict.Add(rightName, rightNode);
			}

			namedNode!.SetNodes(leftNode!, rightNode!);
		}

		var aaaNode = nodeDict.TryGetValue("AAA", out var maybeNode) ? maybeNode : null;

		return (aaaNode, nodeDict.Values.Where(n => n.Name[2] == 'A').ToList());
	}

	private class MapNode(string name)
	{
		public string Name { get; } = name;

		public MapNode? Left { get; private set; }

		public MapNode? Right { get; private set; }

		public void SetNodes(MapNode left, MapNode right)
		{
			Left  = left;
			Right = right;
		}
	}
}
