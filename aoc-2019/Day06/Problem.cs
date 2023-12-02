namespace aoc_2019.Day06;

public class Problem
{
	public static ProblemMetadata2 Metadata { get; } = new(Main);

	public static (long, long) Main(string[] input)
	{
		var (nodeDictionary, centerOfMass) = BuildTree(input);

		Part1(centerOfMass);
		Part2(nodeDictionary);

		return (0, 0);
	}

	public static void Part1(Node? com)
	{
		ArgumentNullException.ThrowIfNull(com);

		var nlist = new Stack<(Node node, int depth)>();
		var ocnt = 0;

		nlist.Push((com, 0));

		while (nlist.Count > 0) {
			var item = nlist.Pop();

			ocnt += item.node.Children.Count + (item.node.Children.Count * item.depth);

			foreach (var c in item.node.Children) {
				nlist.Push((c, item.depth + 1));
			}
		}

		Console.WriteLine(ocnt);
	}

	public static void Part2(Dictionary<string, Node>? ndic)
	{
		ArgumentNullException.ThrowIfNull(ndic);

		var you_ancestors = GetAncestors(ndic["YOU"]);
		var san_ancestors = GetAncestors(ndic["SAN"]);

		Console.WriteLine($"Ancestor count: you: {you_ancestors.Count} san: {san_ancestors.Count}");

		for (var i = 0; i < you_ancestors.Count; i++) {
			var pos = san_ancestors.IndexOf(you_ancestors[i]);

			if (pos > -1) {
				// found the first common ancestor
				Console.WriteLine($"Common ancestor is {you_ancestors[i].Name} at position {i} for you and {pos} for santa");

				Console.WriteLine($"Traversal distance: {i + pos}");

				break;
			}
		}
	}

	private static (Dictionary<string, Node> tree, Node com) BuildTree(string[] input)
	{
		var ndic = new Dictionary<string, Node>();
		var com  = new Node("COM");

		ndic.Add("COM", com);

		foreach (var line in input) {
			var parts  = line.Split(")");
			var parent = parts[0];
			var child  = parts[1];

			if( !ndic.ContainsKey(parent) ) {
				var pn = new Node(parent);
				ndic.Add(parent, pn);
			}

			if( !ndic.ContainsKey(child) ) {
				var cn = new Node(child);
				ndic.Add(child, cn);
			}

			ndic[parent].Add(ndic[child]);
		}

		return (ndic, com);
	}

	private static List<Node> GetAncestors(Node node)
	{
		var ret = new List<Node>();

		while (node.Parent != null) {
			ret.Add(node.Parent);
			node = node.Parent;
		}

		return ret;
	}

	public class Node
	{
		public Node(string name)
		{
			Name     = name;
			Children = new List<Node>();
		}

		public string Name { get; }

		public Node? Parent { get; private set; }

		public IReadOnlyList<Node> Children { get; }

		public void Add(Node child)
		{
			((List<Node>)Children).Add(child);

			child.Parent = this;
		}
	}
}
