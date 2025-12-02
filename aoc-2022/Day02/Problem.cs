namespace AdventOfCode.Year2022.Day02;

using Translation = Dictionary<char, Problem.Move>;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] lines)
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

	[Test]
	[Arguments(15, "inputSample.txt")]
	[Arguments(8890, "input.txt")]
	public async Task InputsScoreCorrectlyPart1(int expected, string fileName)
	{
		var translation = new Translation() {
			{ 'X', Move.Rock },
			{ 'Y', Move.Paper },
			{ 'Z', Move.Scissors },
		};

		await Assert.That(ScoreGame(ReadFileLines(fileName), translation, ScoreRound1)).IsEqualTo(expected);
	}

	[Test]
	[Arguments(12, "inputSample.txt")]
	[Arguments(10238, "input.txt")]
	public async Task InputsScoreCorrectlyPart2(int expected, string fileName)
	{
		var translation = new Translation() {
			{ 'X', Move.Rock },
			{ 'Y', Move.Paper },
			{ 'Z', Move.Scissors },
		};

		await Assert.That(ScoreGame(ReadFileLines(fileName), translation, ScoreRound2)).IsEqualTo(expected);
	}

	[Test]
	public async Task LinesTranslateCorrectly()
	{
		var translation = new Translation() {
			{ 'X', Move.Rock },
			{ 'Y', Move.Paper },
			{ 'Z', Move.Scissors },
		};

		await Assert.That(Translate("A X", translation)).IsEqualTo((Move.Rock, Move.Rock));
		await Assert.That(Translate("A Y", translation)).IsEqualTo((Move.Paper, Move.Rock));
		await Assert.That(Translate("A Z", translation)).IsEqualTo((Move.Scissors, Move.Rock));

		await Assert.That(Translate("B X", translation)).IsEqualTo((Move.Rock, Move.Paper));
		await Assert.That(Translate("B Y", translation)).IsEqualTo((Move.Paper, Move.Paper));
		await Assert.That(Translate("B Z", translation)).IsEqualTo((Move.Scissors, Move.Paper));

		await Assert.That(Translate("C X", translation)).IsEqualTo((Move.Rock, Move.Scissors));
		await Assert.That(Translate("C Y", translation)).IsEqualTo((Move.Paper, Move.Scissors));
		await Assert.That(Translate("C Z", translation)).IsEqualTo((Move.Scissors, Move.Scissors));
	}

	[Test]
	public async Task RoundsScoreCorrectly()
	{
		await Assert.That(ScoreRound1((Move.Rock, Move.Rock))).IsEqualTo(4);     // draw (3), my move is rock (1), total 4
		await Assert.That(ScoreRound1((Move.Rock, Move.Paper))).IsEqualTo(1);    // loss (0), my move is rock (1), total 1
		await Assert.That(ScoreRound1((Move.Rock, Move.Scissors))).IsEqualTo(7); // win (6), my move is rock (1), total 7

		await Assert.That(ScoreRound1((Move.Paper, Move.Rock))).IsEqualTo(8);     // win (6), my move is paper (2), total 8
		await Assert.That(ScoreRound1((Move.Paper, Move.Paper))).IsEqualTo(5);    // draw (3), my move is paper (2), total 5
		await Assert.That(ScoreRound1((Move.Paper, Move.Scissors))).IsEqualTo(2); // loss (0), my move is paper (2), total 2

		await Assert.That(ScoreRound1((Move.Scissors, Move.Rock))).IsEqualTo(3);     // loss (0), my move is scissors (3), total 3
		await Assert.That(ScoreRound1((Move.Scissors, Move.Paper))).IsEqualTo(9);    // win (6), my move is scissors (3), total 9
		await Assert.That(ScoreRound1((Move.Scissors, Move.Scissors))).IsEqualTo(6); // draw (3), my move is scissors (3), total 6
	}
}
