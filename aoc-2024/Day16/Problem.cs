using Microsoft.CodeAnalysis;
using Microsoft.DotNet.PlatformAbstractions;
using Xunit.Internal;

namespace AdventOfCode.Year2024.Day16;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		var startPos = (Location: Coordinate.Empty, Direction: Direction.East);
		var end      = Coordinate.Empty;
		var map      = CreateMap(input, (x, y, c) => {
			if (c == 'S') {
				startPos = ((x, y), Direction.East);
				c = '.';
			} else if (c == 'E') {
				end = (x, y);
				c = '.';
			}

			return c;
		});

		var part1 = FindBestPath(map, startPos, end);
		//var part2 = CountTilesOnPaths(map, startPos, end, part1);
		var part2 = 0;

		return (part1, part2);
	}

	private static long FindBestPath(char[,] map, (Coordinate Location, Direction Direction) startPos, Coordinate end)
	{
		var endPos    = (Location: end, Direction: Direction.East);
		var comparer  = EqualityComparer<(Coordinate Location, Direction Direction)>.Create((x, y) => x.Location == y.Location);
		var nextNodes = ((Coordinate Location, Direction Direction) curPos) => FindAdjacentNodes(map, curPos).Select(an => (an.Item1, (float)an.Item2));
		var path      = AStar.FindPath<(Coordinate Location, Direction Direction)>(startPos, endPos, nextNodes, (from, to) => Coordinate.ManhattanDistance(from.Location, to.Location), comparer);
		var lastDir   = startPos.Direction;
		var cost      = 0;

		if (path == null) {
			throw new InvalidOperationException("Could not find a path");
		}

		foreach (var c in path.Skip(1)) {
			cost += 1 + (lastDir, c.Direction) switch {
				(Direction.North, Direction.North) => 0,
				(Direction.North, Direction.South) => 2000,
				(Direction.North, Direction.East) => 1000,
				(Direction.North, Direction.West) => 1000,

				(Direction.South, Direction.North) => 2000,
				(Direction.South, Direction.South) => 0,
				(Direction.South, Direction.East) => 1000,
				(Direction.South, Direction.West) => 1000,

				(Direction.East, Direction.North) => 1000,
				(Direction.East, Direction.South) => 1000,
				(Direction.East, Direction.East) => 0,
				(Direction.East, Direction.West) => 2000,

				(Direction.West, Direction.North) => 1000,
				(Direction.West, Direction.South) => 1000,
				(Direction.West, Direction.East) => 2000,
				(Direction.West, Direction.West) => 0,

				_ => throw new NotImplementedException($"Rotation from {lastDir} to {c.Direction} is not implemented"),
			};

			lastDir = c.Direction;
		}

		return cost;
	}

	private static long CountTilesOnPaths(char[,] map, (Coordinate Location, Direction Direction) startPos, Coordinate end, long targetCost)
	{
		var states = new Queue<SearchState>();
		var tiles  = new HashSet<Coordinate>() { startPos.Location };
		var hashes = new HashSet<int>();

		// start with our initial state
		states.Enqueue(new(startPos, 0, []));

		while (states.Count > 0) {
//Console.WriteLine(states.Count);
			// grab the next state;

			var (position, cost, path) = states.Dequeue();
Console.WriteLine($"current cost: {cost}");
			// if this state is a winner, then add all the tiles from the path
			// and then we're done with it
			if (position.Location == end && cost == targetCost) {
				tiles.AddRange(path);
				continue;
			}

			// otherwise, for any subsequent states, if they don't cost too much,
			// add them to the list of states to check
			foreach (var (nextStep, addedCost) in FindAdjacentNodes(map, position)) {
				var nextCost = cost + addedCost;

				if (nextCost <= targetCost) {
					var newState = new SearchState(nextStep, nextCost, [.. path, nextStep.Item1]);

					if (hashes.Add(Hash(newState))) {
						states.Enqueue(newState);
					} //else { Console.WriteLine("already saw this state"); }
				} else { Console.WriteLine("exceeded target cost"); }
			}
		}

		return tiles.Count;
	}

	private static IEnumerable<((Coordinate, Direction), int)> FindAdjacentNodes(char[,] map, (Coordinate Location, Direction Direction) curPos)
	{
		var north = curPos.Location + (0, -1);
		var south = curPos.Location + (0, +1);
		var east  = curPos.Location + (+1, 0);
		var west  = curPos.Location + (-1, 0);

		if (map.ValueAt(north) == '.' && curPos.Direction != Direction.South) {
			yield return curPos.Direction switch {
				Direction.North => ((north, Direction.North), 1),
				//Direction.South => ((north, Direction.North), 2001),
				Direction.East => ((north, Direction.North), 1001),
				Direction.West => ((north, Direction.North), 1001),
				_ => throw new InvalidOperationException("Invalid current-facing direction"),
			};
		}

		if (map.ValueAt(south) == '.' && curPos.Direction != Direction.North) {
			yield return curPos.Direction switch {
				//Direction.North => ((south, Direction.South), 2001),
				Direction.South => ((south, Direction.South), 1),
				Direction.East => ((south, Direction.South), 1001),
				Direction.West => ((south, Direction.South), 1001),
				_ => throw new InvalidOperationException("Invalid current-facing direction"),
			};
		}

		if (map.ValueAt(east) == '.' && curPos.Direction != Direction.West) {
			yield return curPos.Direction switch {
				Direction.North => ((east, Direction.East), 1001),
				Direction.South => ((east, Direction.East), 1001),
				Direction.East => ((east, Direction.East), 1),
				//Direction.West => ((east, Direction.East), 2001),
				_ => throw new InvalidOperationException("Invalid current-facing direction"),
			};
		}

		if (map.ValueAt(west) == '.' && curPos.Direction != Direction.East) {
			yield return curPos.Direction switch {
				Direction.North => ((west, Direction.West), 1001),
				Direction.South => ((west, Direction.West), 1001),
				//Direction.East => ((west, Direction.West), 2001),
				Direction.West => ((west, Direction.West), 1),
				_ => throw new InvalidOperationException("Invalid current-facing direction"),
			};
		}

		yield break;
	}

	private static int Hash(SearchState state)
	{
		var combiner = HashCodeCombiner.Start();

		combiner.Add(state.CurrentPosition.Location.X);
		combiner.Add(state.CurrentPosition.Location.Y);
		combiner.Add(state.CurrentPosition.Direction);
		combiner.Add(state.Cost);

		foreach (var c in state.Path.Distinct().OrderBy(c => c.X).ThenBy(c => c.Y)) {
			combiner.Add(c.X);
			combiner.Add(c.Y);
		}

		return combiner.CombinedHash;
	}

	private enum Direction
	{
		North,
		East,
		South,
		West
	}

	private record class SearchState((Coordinate Location, Direction Direction) CurrentPosition, long Cost, List<Coordinate> Path);
}
