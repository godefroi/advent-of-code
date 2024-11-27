using System.Collections.Specialized;

using static AdventOfCode.FloydWarshall;

namespace AdventOfCode.Year2022.Day16;

public partial class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Main, typeof(Problem));

	public static (long, long) Main(string[] input)
	{
		var rooms       = input.Select(Room.Parse).ToDictionary(r => r.Name);
		var flowRooms   = rooms.Values.Where(r => r.FlowRate > 0 || r.Name == "AA").Select(r => r.Name).ToHashSet();
		var connections = ComputeDistances(rooms.SelectMany(r => r.Value.Connections.Select(c => new WeightedEdge<string, int>(r.Key, c, 1))))
			.Where(kvp => kvp.Key.From != kvp.Key.To && flowRooms.Contains(kvp.Key.From) && flowRooms.Contains(kvp.Key.To))
			.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
		var masks       = new Dictionary<string, int>();
		var lastMask    = 0;

		foreach (var room in flowRooms) {
			var nextMask = BitVector32.CreateMask(lastMask);
			masks.Add(room, nextMask);
			lastMask = nextMask;
		}

		var part1 = Search(30, rooms, flowRooms, masks, connections).Values.Max();
		var bests = Search(26, rooms, flowRooms, masks, connections);
		var part2 = 0;

		foreach (var best1 in bests) {
			foreach (var best2 in bests) {
				if ((best1.Key & best2.Key) == 0) {
					part2 = Math.Max(part2, best1.Value + best2.Value);
				}
			}
		}

		return (part1, part2);
	}

	private static Dictionary<int, int> Search(int minutes, Dictionary<string, Room> rooms, HashSet<string> flowRooms, Dictionary<string, int> masks, Dictionary<Edge<string>, int> connections)
	{
		var best   = new Dictionary<int, int>();
		var seen   = new HashSet<State>();
		var states = new Queue<State>(new[] {
			new State(new BitVector32(0), minutes, "AA", 0),
		});

		while (states.Count > 0) {
			var state = states.Dequeue();

			if (!best.TryGetValue(state.OpenValves.Data, out var currentBest)) {
				best.Add(state.OpenValves.Data, state.TotalRelease);
			} else if (state.TotalRelease > currentBest) {
				best[state.OpenValves.Data] = state.TotalRelease;
			}

			if (seen.Contains(state)) {
				continue;
			}

			seen.Add(state);

			foreach (var room in flowRooms.Where(r => !state.OpenValves[masks[r]])) {
				if (room == state.CurrentRoom) {
					continue;
				}

				var remaining = state.MinutesRemaining - connections[new Edge<string>(state.CurrentRoom, room)] - 1;

				if (remaining <= 0) {
					continue;
				}

				states.Enqueue(new State(new BitVector32(state.OpenValves.Data | masks[room]), remaining, room, state.TotalRelease + remaining * rooms[room].FlowRate));
			}
		}

		return best;
	}

	private readonly record struct Room(string Name, int FlowRate, IEnumerable<string> Connections)
	{
		public static Room Parse(string line)
		{
			var match = ParseRegex().Match(line);

			if (!match.Success) {
				throw new InvalidDataException($"Unable to parse input {line}");
			}

			return new Room(match.Groups["name"].Value, int.Parse(match.Groups["flowrate"].Value), match.Groups["path"].Captures.Select(c => c.Value).ToList());
		}
	}

	private readonly record struct State(BitVector32 OpenValves, int MinutesRemaining, string CurrentRoom, int TotalRelease);

	[GeneratedRegex(@"Valve (?<name>\w+) has flow rate=(?<flowrate>\d+); (tunnels lead to valves|tunnel leads to valve) ((?<path>[A-Z]+)(, |))+")]
	private static partial Regex ParseRegex();
}
