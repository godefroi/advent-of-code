using System.Runtime.CompilerServices;

namespace AdventOfCode.Year2022.Day23;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		//fileName = "inputSample.txt";

		var elves = input.Select(Parse).SelectMany((coords, y) => coords.Select(c => new Coordinate(c.X, y))).ToHashSet();
		var part1 = 0;
		var round = 0;

		//Draw(elves);

		while (RunRound(elves, round++) > 0) {
			if (round == 10) {
				var xMin  = int.MaxValue;
				var xMax  = int.MinValue;
				var yMin  = int.MaxValue;
				var yMax  = int.MinValue;

				foreach (var c in elves) {
					if (c.X < xMin) xMin = c.X;
					if (c.X > xMax) xMax = c.X;
					if (c.Y < yMin) yMin = c.Y;
					if (c.Y > yMax) yMax = c.Y;
				}

				for (var y = yMin; y <= yMax; y++) {
					for (var x = xMin; x <= xMax; x++) {
						if (!elves.Contains(new Coordinate(x, y))) {
							part1++;
						}
					}
				}
			}

			//Draw(elves);
		}

		return (part1, round);
	}

	private static int RunRound(HashSet<Coordinate> elves, int round)
	{
		var adjacencies = elves.ToDictionary(c => c, c => new Adjacencies(c));
		var proposals   = new Dictionary<Coordinate, List<Coordinate>>();
		var moveCount   = 0;

		// make proposals
		foreach (var (elf, adj) in adjacencies) {
			var containsNW = elves.Contains(adj.NW);
			var containsN  = elves.Contains(adj.N);
			var containsNE = elves.Contains(adj.NE);
			var containsW  = elves.Contains(adj.W);
			var containsE  = elves.Contains(adj.E);
			var containsSW = elves.Contains(adj.SW);
			var containsS  = elves.Contains(adj.S);
			var containsSE = elves.Contains(adj.SE);

			if (!(containsNW || containsN || containsNE || containsW || containsE || containsSW || containsS || containsSE)) {
				continue;
			}

			for (var i = 0; i < 4; i++) {
				var proposeDir = (Direction)((round + i) % 4);

				if (proposeDir == Direction.North) {
					if (!(containsNW || containsN || containsNE)) {
						AddProposal(elf, adj.N);
						break;
					}
				} else if (proposeDir == Direction.South) {
					if (!(containsSW || containsS || containsSE)) {
						AddProposal(elf, adj.S);
						break;
					}
				} else if (proposeDir == Direction.West) {
					if (!(containsNW || containsW || containsSW)) {
						AddProposal(elf, adj.W);
						break;
					}
				} else if (proposeDir == Direction.East) {
					if (!(containsNE || containsE || containsSE)) {
						AddProposal(elf, adj.E);
						break;
					}
				}
			}
		}

		// execute moves
		foreach (var (to, from) in proposals.Where(kvp => kvp.Value.Count == 1)) {
			elves.Remove(from[0]);
			elves.Add(to);
			moveCount++;
		}

		return moveCount;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void AddProposal(Coordinate elf, Coordinate destination)
		{
			if (proposals.TryGetValue(destination, out var curList)) {
				curList.Add(elf);
			} else {
				proposals.Add(destination, new List<Coordinate>(4) { elf });
			}
		}
	}

	private static void Draw(HashSet<Coordinate> elves)
	{
		var xMin = int.MaxValue;
		var xMax = int.MinValue;
		var yMin = int.MaxValue;
		var yMax = int.MinValue;
		var sb   = new StringBuilder();

		foreach (var c in elves) {
			if (c.X < xMin) xMin = c.X;
			if (c.X > xMax) xMax = c.X;
			if (c.Y < yMin) yMin = c.Y;
			if (c.Y > yMax) yMax = c.Y;
		}

		for (var y = yMin; y <= yMax; y++) {
			for (var x = xMin; x <= xMax; x++) {
				sb.Append(elves.Contains(new Coordinate(x, y)) ? '#' : '.');
			}

			sb.AppendLine();
		}

		Console.WriteLine();
		Console.WriteLine(sb.ToString());
	}

	private static IEnumerable<Coordinate> Parse(string line) => Enumerable.Range(0, line.Length).Where(x => line[x] == '#').Select(x => new Coordinate(x, 0));

	[Test]
	public async Task ElvesParseCorrectly()
	{
		await Assert.That(ReadFileLines("inputSample.txt", Parse).SelectMany((coords, y) => coords.Select(c => new Coordinate(c.X, y)))).IsEquivalentTo([
				new Coordinate(4, 0),
				new Coordinate(2, 1),
				new Coordinate(3, 1),
				new Coordinate(4, 1),
				new Coordinate(6, 1),
				new Coordinate(0, 2),
				new Coordinate(4, 2),
				new Coordinate(6, 2),
				new Coordinate(1, 3),
				new Coordinate(5, 3),
				new Coordinate(6, 3),
				new Coordinate(0, 4),
				new Coordinate(2, 4),
				new Coordinate(3, 4),
				new Coordinate(4, 4),
				new Coordinate(0, 5),
				new Coordinate(1, 5),
				new Coordinate(3, 5),
				new Coordinate(5, 5),
				new Coordinate(6, 5),
				new Coordinate(1, 6),
				new Coordinate(4, 6)
			]);
	}

	private readonly record struct Adjacencies(Coordinate NW, Coordinate N, Coordinate NE, Coordinate W, Coordinate E, Coordinate SW, Coordinate S, Coordinate SE)
	{
		public Adjacencies(Coordinate elf) : this(
			new Coordinate(elf.X - 1, elf.Y - 1),
			new Coordinate(elf.X,     elf.Y - 1),
			new Coordinate(elf.X + 1, elf.Y - 1),
			new Coordinate(elf.X - 1, elf.Y),
			new Coordinate(elf.X + 1, elf.Y),
			new Coordinate(elf.X - 1, elf.Y + 1),
			new Coordinate(elf.X,     elf.Y + 1),
			new Coordinate(elf.X + 1, elf.Y + 1)) { }
	}

	private enum Direction : int
	{
		North = 0,
		South = 1,
		West  = 2,
		East  = 3,
	}
}
