namespace AdventOfCode.Year2021.Day15;

using static Dijkstra;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Main, typeof(Problem), null);

	public static (long p1, long p2) Main(string[] input)
	{
		var graph  = input.SelectMany((line, y) => line.Select((c, x) => new Node(x, y, int.Parse(c.ToString())))).ToDictionary(n => (n.X, n.Y));
		var height = input.Length;
		var p1     = RunDijkstra(graph);

		graph = graph.Values.SelectMany(n => Scale(n, 5, height)).ToDictionary(n => (n.X, n.Y));

		var p2 = RunDijkstra(graph);

		Console.WriteLine($"part 1: {p1}");
		Console.WriteLine($"part 2: {p2}");

		return (p1, p2);
	}

	private static IEnumerable<Node> Scale(Node node, int factor, int width)
	{
		for (var x = 0; x < factor; x++) {
			for (var y = 0; y < factor; y++) {
				var nrisk = node.Risk + x + y;

				while (nrisk > 9) {
					nrisk -= 9;
				}

				yield return new Node(node.Y + (width * y), node.X + (width * x), nrisk);
			}
		}
	}

	[Fact(DisplayName = "Day 15 Sample Input")]
	public void SampleInputFunctionCorrectly()
	{
		var (p1, p2) = Main(ReadFileLines("input_sample.txt"));

		Assert.Equal(40, p1);
		Assert.Equal(315, p2);
	}

	[Fact(DisplayName = "Day 15 Main Input")]
	public void MainInputFunctionCorrectly()
	{
		var (p1, p2) = Main(ReadFileLines("input.txt"));

		Assert.Equal(745, p1);
		Assert.Equal(3002, p2);
	}
}
