namespace Day21;

public class Problem : ProblemBase
{
	private static int _rollCount;
	private static int _nextRoll = 1;

	class Player
	{
		public int CurrentSpace;
		public int CurrentScore;

		public static bool operator ==(Player p1, Player p2) => (p1.CurrentSpace == p2.CurrentSpace) && (p1.CurrentScore == p2.CurrentScore);

		public static bool operator !=(Player p1, Player p2) => !(p1 == p2);

		public override int GetHashCode() => (CurrentSpace, CurrentScore).GetHashCode();

		public override bool Equals(object obj) => this == (Player)obj;

		public override string ToString() => $"Space: {CurrentSpace}, Score: {CurrentScore}";

		public void Move(int spaces)
		{
			CurrentSpace += spaces;

			if (CurrentSpace > 10) {
				CurrentSpace %= 10;
			}

			if (CurrentSpace == 0) {
				CurrentSpace = 10;
			}

			CurrentScore += CurrentSpace;
		}

		public Player Copy() => new Player(CurrentSpace, CurrentScore);

		public Player(int startingSpace, int startingScore = 0)
		{
			CurrentSpace = startingSpace;
			CurrentScore = startingScore;
		}
	}

	private const int WINNING_SCORE = 21;

	internal static void Main(string fileName)
	{
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
			[(new Player(7), new Player(2), 0)] = 1
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
	}

	internal static void Part2(string fileName)
	{
		//for (var i = 0; i < 101; i++) {
		//	Console.WriteLine(Roll());
		//}
		//Console.WriteLine(_rollCount);
		//return;
		var input    = File.ReadAllLines(GetFilePath(fileName));
		var p1_pos   = int.Parse(input[0].Split(' ').Last());
		var p2_pos   = int.Parse(input[1].Split(' ').Last());
		var p1_score = 0;
		var p2_score = 0;

		//p1_pos = 4;
		//p2_pos = 8;

		while (true) {
			var p1_roll = Roll() + Roll() + Roll();

			p1_pos += p1_roll;

			while (p1_pos > 10) {
				p1_pos -= 10;
			}

			p1_score += p1_pos;

			Console.WriteLine($"Player 1 rolls {p1_roll} and moves to space {p1_pos} for a total score of {p1_score}");

			if (p1_score >= 1000) {
				break;
			}

			var p2_roll = Roll() + Roll() + Roll();

			p2_pos += p2_roll;

			while (p2_pos > 10) {
				p2_pos -= 10;
			}

			p2_score += p2_pos;

			Console.WriteLine($"Player 2 rolls {p2_roll} and moves to space {p2_pos} for a total score of {p2_score}");

			if (p2_score >= 1000) {
				break;
			}
		}

		Console.WriteLine($"part 1: {Math.Min(p1_score, p2_score) * _rollCount}");
	}

	private static int Roll()
	{
		var ret = _nextRoll;

		_rollCount++;
		_nextRoll++;

		if (_nextRoll > 100) {
			_nextRoll = 1;
		}

		return ret;
	}
}
