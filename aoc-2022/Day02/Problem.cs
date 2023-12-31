namespace aoc_2022.Day02;

using Translation = Dictionary<char, Problem.Move>;

public class Problem
{
	public static ProblemMetadata2 Metadata { get; } = new(Main, typeof(Problem));

	public static (long, long) Main(string[] lines)
	{
		var translation = new Translation() {
			{ 'X', Move.Rock },
			{ 'Y', Move.Paper },
			{ 'Z', Move.Scissors },
		};

		var score1 = ScoreGame(lines, translation, ScoreRound1);
		var score2 = ScoreGame(lines, translation, ScoreRound2);

		Console.WriteLine($"part 1: {score1}");
		Console.WriteLine($"part 2: {score2}");

		return (score1, score2);
	}

	private static int ScoreGame(string[] lines, Translation translation, Func<(Move Mine, Move Opponent), int> scorer) => lines.Select(l => Translate(l, translation)).Sum(scorer);

	private static (Move Mine, Move Opponent) Translate(string line, Translation translation) => (
		translation[line[2]],
		line[0] switch {
		'A' => Move.Rock,
		'B' => Move.Paper,
		'C' => Move.Scissors,
		_ => throw new InvalidDataException($"The 'opponent' move {line[0]} is unrecognized."),
		});

	private static int ScoreRound1((Move Mine, Move Opponent) thisRound)
	{
		var roundScore = thisRound switch {
			// draw
			(Move.Rock,     Move.Rock)     => 3,
			(Move.Paper,    Move.Paper)    => 3,
			(Move.Scissors, Move.Scissors) => 3,

			// I win
			(Move.Rock,     Move.Scissors) => 6,
			(Move.Paper,    Move.Rock)     => 6,
			(Move.Scissors, Move.Paper)    => 6,

			// I lose
			(Move.Rock,     Move.Paper)    => 0,
			(Move.Paper,    Move.Scissors) => 0,
			(Move.Scissors, Move.Rock)     => 0,

			_ => throw new Exception($"{thisRound} is not a valid move."),
		};

		//Console.WriteLine($"Playing round ({thisRound.Mine.ToString()[0]}, {thisRound.Opponent.ToString()[0]}) round score {roundScore}, play score {(int)thisRound.Mine}");

		return roundScore + (int)thisRound.Mine;
	}

	private static int ScoreRound2((Move Mine, Move Opponent) thisRound) => thisRound switch {
		(Move.Rock,     Move.Rock) => ScoreRound1((Move.Scissors, thisRound.Opponent)), // they play rock, I need to lose, so play scissors
		(Move.Paper,    Move.Rock) => ScoreRound1((Move.Rock,     thisRound.Opponent)), // they play rock, I need to draw, so play rock
		(Move.Scissors, Move.Rock) => ScoreRound1((Move.Paper,    thisRound.Opponent)), // they play rock, I need to win, so play paper

		(Move.Rock,     Move.Paper) => ScoreRound1((Move.Rock,     thisRound.Opponent)), // they play paper, I need to lose, so play rock
		(Move.Paper,    Move.Paper) => ScoreRound1((Move.Paper,    thisRound.Opponent)), // they play paper, I need to draw, so play paper
		(Move.Scissors, Move.Paper) => ScoreRound1((Move.Scissors, thisRound.Opponent)), // they play paper, I need to win, so play scissors

		(Move.Rock,     Move.Scissors) => ScoreRound1((Move.Paper,    thisRound.Opponent)), // they play scissors, I need to lose, so play paper
		(Move.Paper,    Move.Scissors) => ScoreRound1((Move.Scissors, thisRound.Opponent)), // they play scissors, I need to draw, so play scissors
		(Move.Scissors, Move.Scissors) => ScoreRound1((Move.Rock,     thisRound.Opponent)), // they play scissors, I need to win, so play rock

		_ => throw new ArgumentOutOfRangeException(nameof(thisRound), $"{thisRound} is an invalid round.")
	};

	public enum Move
	{
		Rock     = 1,
		Paper    = 2,
		Scissors = 3,
	}

	[Theory]
	[InlineData(15, "inputSample.txt")]
	[InlineData(12679, "scorixear.txt")]
	[InlineData(8890, "input.txt")]
	public void InputsScoreCorrectlyPart1(int expected, string fileName)
	{
		var translation = new Translation() {
			{ 'X', Move.Rock },
			{ 'Y', Move.Paper },
			{ 'Z', Move.Scissors },
		};

		Assert.Equal(expected, ScoreGame(ReadFileLines(fileName), translation, ScoreRound1));
	}

	[Theory]
	[InlineData(12, "inputSample.txt")]
	[InlineData(10238, "input.txt")]
	public void InputsScoreCorrectlyPart2(int expected, string fileName)
	{
		var translation = new Translation() {
			{ 'X', Move.Rock },
			{ 'Y', Move.Paper },
			{ 'Z', Move.Scissors },
		};

		Assert.Equal(expected, ScoreGame(ReadFileLines(fileName), translation, ScoreRound2));
	}

	[Fact]
	public void LinesTranslateCorrectly()
	{
		var translation = new Translation() {
			{ 'X', Move.Rock },
			{ 'Y', Move.Paper },
			{ 'Z', Move.Scissors },
		};

		Assert.Equal((Move.Rock, Move.Rock),     Translate("A X", translation));
		Assert.Equal((Move.Paper, Move.Rock),    Translate("A Y", translation));
		Assert.Equal((Move.Scissors, Move.Rock), Translate("A Z", translation));

		Assert.Equal((Move.Rock, Move.Paper),     Translate("B X", translation));
		Assert.Equal((Move.Paper, Move.Paper),    Translate("B Y", translation));
		Assert.Equal((Move.Scissors, Move.Paper), Translate("B Z", translation));

		Assert.Equal((Move.Rock, Move.Scissors),     Translate("C X", translation));
		Assert.Equal((Move.Paper, Move.Scissors),    Translate("C Y", translation));
		Assert.Equal((Move.Scissors, Move.Scissors), Translate("C Z", translation));
	}

	[Fact]
	public void RoundsScoreCorrectly()
	{
		Assert.Equal(4, ScoreRound1((Move.Rock, Move.Rock)));     // draw (3), my move is rock (1), total 4
		Assert.Equal(1, ScoreRound1((Move.Rock, Move.Paper)));    // loss (0), my move is rock (1), total 1
		Assert.Equal(7, ScoreRound1((Move.Rock, Move.Scissors))); // win (6), my move is rock (1), total 7

		Assert.Equal(8, ScoreRound1((Move.Paper, Move.Rock)));     // win (6), my move is paper (2), total 8
		Assert.Equal(5, ScoreRound1((Move.Paper, Move.Paper)));    // draw (3), my move is paper (2), total 5
		Assert.Equal(2, ScoreRound1((Move.Paper, Move.Scissors))); // loss (0), my move is paper (2), total 2

		Assert.Equal(3, ScoreRound1((Move.Scissors, Move.Rock)));     // loss (0), my move is scissors (3), total 3
		Assert.Equal(9, ScoreRound1((Move.Scissors, Move.Paper)));    // win (6), my move is scissors (3), total 9
		Assert.Equal(6, ScoreRound1((Move.Scissors, Move.Scissors))); // draw (3), my move is scissors (3), total 6
	}
}
