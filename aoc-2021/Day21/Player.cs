namespace AdventOfCode.Year2021.Day21;

internal class Player
{
	public int CurrentSpace;
	public int CurrentScore;

	public static bool operator == (Player p1, Player p2) => (p1.CurrentSpace == p2.CurrentSpace) && (p1.CurrentScore == p2.CurrentScore);

	public static bool operator != (Player p1, Player p2) => !(p1 == p2);

	public override int GetHashCode() => HashCode.Combine(CurrentSpace.GetHashCode(), CurrentScore.GetHashCode());

	public override bool Equals(object? obj) => obj != null && obj is Player other && this == other;

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

	public Player Copy() => new(CurrentSpace, CurrentScore);

	public Player(int startingSpace, int startingScore = 0)
	{
		CurrentSpace = startingSpace;
		CurrentScore = startingScore;
	}
}
