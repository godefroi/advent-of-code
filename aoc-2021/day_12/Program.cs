//var input = @"start-A
//start-b
//A-c
//A-b
//b-d
//A-end
//b-end
//".Split(Environment.NewLine).SkipLast(1).ToList();
var input = File.ReadAllLines(args[0]).ToList();
var nodes = new Dictionary<string, Node>();

foreach (var line in input) {
	var parts = line.Split('-');

	// make sure both these nodes exist
	foreach (var part in parts) {
		if (!nodes.ContainsKey(part)) {
			nodes.Add(part, new Node(part, new List<Node>(), part == part.ToUpperInvariant()));
		}
	}

	// add connections for each node
	nodes[parts[0]].Connections.Add(nodes[parts[1]]);
	nodes[parts[1]].Connections.Add(nodes[parts[0]]);
}

Console.WriteLine($"part 1: { Explore(nodes["start"], new List<string>()).Count()}"); // part 1 is 3713
Console.WriteLine($"part 2: {nodes.Values.Where(n => !n.Big && n.Name != "start" && n.Name != "end").SelectMany(n => Explore(nodes["start"], new List<string>(), n.Name)).Select(p => string.Join(',', p)).Distinct().Count()}"); // part 2 is 91292

static IEnumerable<List<string>> Explore(Node from, List<string> path, string? allowedDoubleVisit = null)
{
	// add the node we came from to the path
	path.Add(from.Name);

	foreach (var node in from.Connections) {
		// if it's the end, we're done with this path
		if (node.Name == "end") {
			path.Add(node.Name);
			yield return path;
		}

		// otherwise, explore each (valid) path out from here
		if (node.Big || !path.Contains(node.Name)) {
			foreach (var p in Explore(node, new List<string>(path), allowedDoubleVisit)) {
				yield return p;
			}
		} else if (path.Contains(node.Name) && node.Name == allowedDoubleVisit) {
			foreach (var p in Explore(node, new List<string>(path), null)) {
				yield return p;
			}
		}
	}
}

record class Node(string Name, List<Node> Connections, bool Big);
