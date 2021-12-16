using Xunit;

namespace Day12;

public class Problem
{
	internal static (int p1, int p2) Main(string fileName)
	{
		var input = File.ReadAllLines(fileName).ToList();
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
		
		var p1 = Explore(nodes["start"], new List<string>()).Count();
		var p2 = nodes.Values.Where(n => !n.Big && n.Name != "start" && n.Name != "end").SelectMany(n => Explore(nodes["start"], new List<string>(), n.Name)).Select(p => string.Join(',', p)).Distinct().Count();

		Console.WriteLine($"part 1: {p1}"); // part 1 is 3713
		Console.WriteLine($"part 2: {p2}"); // part 2 is 91292

		return (p1, p2);
	}

	private static IEnumerable<List<string>> Explore(Node from, List<string> path, string? allowedDoubleVisit = null)
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

	[Fact(DisplayName = "Day 12 Sample Input")]
	public void SampleInputFunctionCorrectly()
	{
		var (p1, p2) = Main("../../../Day12/input_sample.txt");

		Assert.Equal(10, p1);
		Assert.Equal(36, p2);
	}

	[Fact(DisplayName = "Day 12 Sample Input (slightly larger)")]
	public void SampleInput2FunctionCorrectly()
	{
		var (p1, p2) = Main("../../../Day12/input_sample_slightly_larger.txt");

		Assert.Equal(19, p1);
		//Assert.Equal(36, p2); // no official answer given for part 2 on this sample
	}

	[Fact(DisplayName = "Day 12 Sample Input (even larger)")]
	public void SampleInput3FunctionCorrectly()
	{
		var (p1, p2) = Main("../../../Day12/input_sample_even_larger.txt");

		Assert.Equal(226, p1);
		//Assert.Equal(36, p2); // no official answer given for part 2 on this sample
	}

	[Fact(DisplayName = "Day 12 Main Input")]
	public void MainInputFunctionCorrectly()
	{
		var (p1, p2) = Main("../../../Day12/input.txt");

		Assert.Equal(3713, p1);
		Assert.Equal(91292, p2);
	}

	record class Node(string Name, List<Node> Connections, bool Big);
}
