using System.Diagnostics;

namespace AdventOfCode.Year2021.Day21;

public class Problem
{
	private const int WINNING_SCORE = 21;

	public static ProblemMetadata Metadata { get; } = new(Main);

	public static (long, long) Main(string[] input)
	{
		var positions  = input.Select(l => int.Parse(Regex.Match(l, ": (?<start>\\d+)").Groups["start"].ValueSpan)).ToArray();
		var rollCounts = new Dictionary<int, int>() {
			{ 3, 1 },
			{ 4, 3 },
			{ 5, 6 },
			{ 6, 7 },
			{ 7, 6 },
			{ 8, 3 },
			{ 9, 1 },
		};

		var universes = new Dictionary<(Player player1, Player player2, int currentPlayer), long>() {
			[(new Player(positions[0]), new Player(positions[1]), 0)] = 1
		};

		var wins = new[] { 0L, 0L };

		while (universes.Count > 0) {
			var nextUniverses = new Dictionary<(Player, Player, int), long>();

			foreach (var (universe, universeCount) in universes) {
				foreach (var (roll, timesRolled) in rollCounts) {
					var newPlayer1 = universe.player1.Copy();
					var newPlayer2 = universe.player2.Copy();

					if (universe.currentPlayer == 0) {
						newPlayer1.Move(roll);
						if (newPlayer1.CurrentScore >= WINNING_SCORE) {
							wins[0] += universeCount * timesRolled;
							continue;
						}
					} else {
						newPlayer2.Move(roll);
						if (newPlayer2.CurrentScore >= WINNING_SCORE) {
							wins[1] += universeCount * timesRolled;
							continue;
						}
					}

					var newUniverse = (newPlayer1, newPlayer2, universe.currentPlayer == 0 ? 1 : 0);

					if (!nextUniverses.ContainsKey(newUniverse)) {
						nextUniverses[newUniverse] = universeCount * timesRolled;
					} else {
						nextUniverses[newUniverse] += universeCount * timesRolled;
					}
				}
			}

			universes = nextUniverses;
		}

		Console.WriteLine(wins.Max());

		return (-1, -1);
	}
}
