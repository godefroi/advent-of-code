namespace Day15;

public class Node
{
	public int X { get; private set; }

	public int Y { get; private set; }

	public int Risk { get; private set; }

	public int Distance { get; set; } = int.MaxValue;

	public bool Visited { get; set; } = false;

	public Node(int x, int y, int risk)
	{
		X    = x;
		Y    = y;
		Risk = risk;
	}
}

internal static class Dijkstra
{
	private static readonly (int x, int y)[] _adjacent = new[] {
		(x: -1, y:  0),
		(x:  1, y:  0),
		(x:  0, y: -1),
		(x:  0, y:  1)
	 };

	public static int RunDijkstra(Dictionary<(int X, int Y), Node> graph)
	{
		var max = graph.Values.Max(n => n.X);

		return RunDijkstra(graph, graph[(max, max)]);
	}

	public static int RunDijkstra(Dictionary<(int X, int Y), Node> graph, Node target)
	{
		var next = new PriorityQueue<Node, int>();

		graph[(0, 0)].Distance = 0;

		next.Enqueue(graph[(0, 0)], 0);

		while (next.Count > 0) {
			var cur = next.Dequeue();

			if (cur.Visited) {
				continue;
			}

			cur.Visited = true;

			if (cur == target) {
				return target.Distance;
			}

			var neighbors = _adjacent.Select(adj => (cur.X + adj.x, cur.Y + adj.y)).Where(coord => graph.ContainsKey(coord) && !graph[coord].Visited).Select(coord => graph[coord]);

			foreach (var neighbor in neighbors) {
				var alt = cur.Distance + neighbor.Risk;

				if (alt < neighbor.Distance) {
					neighbor.Distance = alt;
				}

				if (neighbor.Distance != int.MaxValue) {
					next.Enqueue(neighbor, neighbor.Distance);
				}
			}
		}

		return target.Distance;
	}
}
