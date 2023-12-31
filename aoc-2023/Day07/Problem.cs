namespace AdventOfCode.Year2023.Day07;

public partial class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, null);

	private const int SCORE_FIVE_OF_A_KIND = 7;
	private const int SCORE_FOUR_OF_A_KIND = 6;
	private const int SCORE_FULL_HOUSE = 5;
	private const int SCORE_THREE_OF_A_KIND = 4;
	private const int SCORE_TWO_PAIR = 3;
	private const int SCORE_ONE_PAIR = 2;
	private const int SCORE_HIGH_CARD = 1;

	private readonly static Dictionary<char, int> _cardScores = new() {
		{ 'A', 13 },
		{ 'K', 12 },
		{ 'Q', 11 },
		{ 'J', 10 },
		{ 'T', 9 },
		{ '9', 8 },
		{ '8', 7 },
		{ '7', 6 },
		{ '6', 5 },
		{ '5', 4 },
		{ '4', 3 },
		{ '3', 2 },
		{ '2', 1 },
	};

	public static (long, long) Execute(string[] input)
	{
		Span<ScoredHand> part1Hands = stackalloc ScoredHand[input.Length];
		Span<ScoredHand> part2Hands = stackalloc ScoredHand[input.Length];

		for (var i = 0; i < input.Length; i++) {
			var span     = input[i].AsSpan();
			var handSpan = span[..5];
			var bid      = int.Parse(span[6..]);

			part1Hands[i] = new ScoredHand(ScoreHandPart1(handSpan), bid);
			part2Hands[i] = new ScoredHand(ScoreHandPart2(handSpan), bid);
		}

		// sort the hands by score
		part1Hands.Sort(CompareHands);
		part2Hands.Sort(CompareHands);

		var part1 = 0L;
		var part2 = 0L;

		for (var i = 0; i < part1Hands.Length; i++) {
			part1 += (i + 1) * part1Hands[i].Bid;
			part2 += (i + 1) * part2Hands[i].Bid;
		}

		// part2: 250763402 is too low
		// part2: 250825971

		return (part1, part2);
	}

	private static int CompareHands(ScoredHand x, ScoredHand y)
	{
		var handComparison = x.Score.HandScore.CompareTo(y.Score.HandScore);

		if (handComparison != 0) {
			return handComparison;
		}

		var card0Comparison = x.Score.Card0Score.CompareTo(y.Score.Card0Score);

		if (card0Comparison != 0) {
			return card0Comparison;
		}

		var card1Comparison = x.Score.Card1Score.CompareTo(y.Score.Card1Score);

		if (card1Comparison != 0) {
			return card1Comparison;
		}

		var card2Comparison = x.Score.Card2Score.CompareTo(y.Score.Card2Score);

		if (card2Comparison != 0) {
			return card2Comparison;
		}

		var card3Comparison = x.Score.Card3Score.CompareTo(y.Score.Card3Score);

		if (card3Comparison != 0) {
			return card3Comparison;
		}

		var card4Comparison = x.Score.Card4Score.CompareTo(y.Score.Card4Score);

		if (card4Comparison != 0) {
			return card4Comparison;
		}

		return 0;
	}

	private static Score ScoreHandPart1(ReadOnlySpan<char> cards)
	{
		Span<char> groups     = stackalloc char[5];
		Span<int>  counts     = stackalloc int[5];
		Span<int>  cardScores = stackalloc int[5];

		for (var i = 0; i < 5; i++ ) {
			var found = false;

			// score this card, we might need it later
			cardScores[i] = _cardScores[cards[i]];

			for (var j = 0; j < 5; j++) {
				// if this card matches this group,
				// count int and move on
				if (groups[j] == cards[i]) {
					counts[j] += 1;
					found = true;
					break;
				}
			}

			if (found) {
				continue;
			}

			// otherwise, find a group that this card can
			// start
			for (var j = 0; j < 5; j++) {
				if (groups[j] == '\0') {
					groups[j] = cards[i];
					counts[j] = 1;
					break;
				}
			}
		}

		// sort the group counts
		counts.Sort();

		// now, recognize the hand based on group counts
		if (counts[4] == 5) {
			// five of a kind
			return new Score(7, cardScores[0], cardScores[1], cardScores[2], cardScores[3], cardScores[4]);
		} else if (counts[4] == 4) {
			// four of a kind
			return new Score(6, cardScores[0], cardScores[1], cardScores[2], cardScores[3], cardScores[4]);
		} else if (counts[4] == 3 && counts[3] == 2) {
			// full house
			return new Score(5, cardScores[0], cardScores[1], cardScores[2], cardScores[3], cardScores[4]);
		} else if (counts[4] == 3) {
			// three of a kind
			return new Score(4, cardScores[0], cardScores[1], cardScores[2], cardScores[3], cardScores[4]);
		} else if (counts[4] == 2 && counts[3] == 2) {
			// two pair
			return new Score(3, cardScores[0], cardScores[1], cardScores[2], cardScores[3], cardScores[4]);
		} else if (counts[4] == 2) {
			// one pair
			return new Score(2, cardScores[0], cardScores[1], cardScores[2], cardScores[3], cardScores[4]);
		} else {
			// high card
			return new Score(1, cardScores[0], cardScores[1], cardScores[2], cardScores[3], cardScores[4]);
		}
	}

	private static void GroupCards(ReadOnlySpan<char> cards, Span<char> groups, Span<int> counts, Span<int> cardScores, ref int jokerGroup)
	{
		for (var i = 0; i < 5; i++ ) {
			var found = false;

			// score this card, we might need it later
			cardScores[i] = _cardScores[cards[i]];

			for (var j = 0; j < 5; j++) {
				// if this card matches this group,
				// count int and move on
				if (groups[j] == cards[i]) {
					counts[j] += 1;
					found = true;
					break;
				}
			}

			if (found) {
				continue;
			}

			// otherwise, find a group that this card can
			// start
			for (var j = 0; j < 5; j++) {
				if (groups[j] == '\0') {
					groups[j] = cards[i];
					counts[j] = 1;

					// if this card is a joker, then keep track of what group it's in
					if (jokerGroup == -1 && cards[i] == 'J') {
						jokerGroup = j;
					}

					break;
				}
			}
		}
	}

	private static Score ScoreHandPart2(ReadOnlySpan<char> cards)
	{
		Span<char> groups     = stackalloc char[5];
		Span<int>  counts     = stackalloc int[5];
		Span<int>  cardScores = stackalloc int[5];
		var        jokerGroup = -1;
		var        jokerCount = 0;
		var        jackScore = _cardScores['J'];

		// group up the cards, score the cards, find any jokers
		GroupCards(cards, groups, counts, cardScores, ref jokerGroup);

		// if we have jokers, we need to remember how many there were
		// and then, pretend like we had none
		if (jokerGroup > -1) {
			jokerCount = counts[jokerGroup];
			counts[jokerGroup] = 0;
		}

		// modify all jokers' card scores, they score 0
		for (var i = 0; i < 5; i++) {
			if (cardScores[i] == jackScore) {
				cardScores[i] = 0;
			}
		}

		// sort the group counts
		counts.Sort();

		// now, recognize the hand based on group counts
		if (counts[4] == 5) {
			// five of a kind

			// this hand can't be improved upon
			return new Score(SCORE_FIVE_OF_A_KIND, cardScores[0], cardScores[1], cardScores[2], cardScores[3], cardScores[4]);
		} else if (counts[4] == 4) {
			// four of a kind

			// if we have a joker, then this hand becomes 5 of a kind
			// otherwise, it stays 4 of a kind
			return jokerCount switch {
				1 => new Score(SCORE_FIVE_OF_A_KIND, cardScores[0], cardScores[1], cardScores[2], cardScores[3], cardScores[4]),
				_ => new Score(SCORE_FOUR_OF_A_KIND, cardScores[0], cardScores[1], cardScores[2], cardScores[3], cardScores[4]),
			};
		} else if (counts[4] == 3 && counts[3] == 2) {
			// full house

			// we won't get here if we had any jokers, so this hand can't be improved
			return new Score(SCORE_FULL_HOUSE, cardScores[0], cardScores[1], cardScores[2], cardScores[3], cardScores[4]);
		} else if (counts[4] == 3) {
			// three of a kind

			// if there were jokers:
			//   2 jokers -> this hand becomes 5 of a kind
			//   1 joker -> this hand becomes 4 of a kind
			//   otherwise -> this hand stays the same
			return jokerCount switch {
				2 => new Score(SCORE_FIVE_OF_A_KIND, cardScores[0], cardScores[1], cardScores[2], cardScores[3], cardScores[4]),
				1 => new Score(SCORE_FOUR_OF_A_KIND, cardScores[0], cardScores[1], cardScores[2], cardScores[3], cardScores[4]),
				_ => new Score(SCORE_THREE_OF_A_KIND, cardScores[0], cardScores[1], cardScores[2], cardScores[3], cardScores[4]),
			};
		} else if (counts[4] == 2 && counts[3] == 2) {
			// two pair

			// if we have 1 joker, then the odd card is a joker and this becomes a full house
			// otherwise, two pair
			return jokerCount switch {
				1 => new Score(SCORE_FULL_HOUSE, cardScores[0], cardScores[1], cardScores[2], cardScores[3], cardScores[4]),
				_ => new Score(SCORE_TWO_PAIR, cardScores[0], cardScores[1], cardScores[2], cardScores[3], cardScores[4]),
			};
		} else if (counts[4] == 2) {
			// one pair

			// if we have 3 jokers, this becomes 5 of a kind
			// if we have 2 jokers, this becomes 4 of a kind
			// if we have 1 joker, this becomes 3 of a kind
			return jokerCount switch {
				3 => new Score(SCORE_FIVE_OF_A_KIND, cardScores[0], cardScores[1], cardScores[2], cardScores[3], cardScores[4]),
				2 => new Score(SCORE_FOUR_OF_A_KIND, cardScores[0], cardScores[1], cardScores[2], cardScores[3], cardScores[4]),
				1 => new Score(SCORE_THREE_OF_A_KIND, cardScores[0], cardScores[1], cardScores[2], cardScores[3], cardScores[4]),
				_ => new Score(SCORE_ONE_PAIR, cardScores[0], cardScores[1], cardScores[2], cardScores[3], cardScores[4]),
			};
		} else if (counts[4] == 1) {
			// high card

			// if we have 4 jokers, this becomes 5 of a kind
			// 3, becomes 4 of a kind
			// 2, becomes becomes 3 of a kind
			// 1, becomes one pair
			return jokerCount switch {
				4 => new Score(SCORE_FIVE_OF_A_KIND, cardScores[0], cardScores[1], cardScores[2], cardScores[3], cardScores[4]),
				3 => new Score(SCORE_FOUR_OF_A_KIND, cardScores[0], cardScores[1], cardScores[2], cardScores[3], cardScores[4]),
				2 => new Score(SCORE_THREE_OF_A_KIND, cardScores[0], cardScores[1], cardScores[2], cardScores[3], cardScores[4]),
				1 => new Score(SCORE_ONE_PAIR, cardScores[0], cardScores[1], cardScores[2], cardScores[3], cardScores[4]),
				_ => new Score(SCORE_HIGH_CARD, cardScores[0], cardScores[1], cardScores[2], cardScores[3], cardScores[4]),
			};
		} else {
			// all jokers, I guess...
			return new Score(SCORE_FIVE_OF_A_KIND, cardScores[0], cardScores[1], cardScores[2], cardScores[3], cardScores[4]);
		}
	}

	private readonly record struct Score(int HandScore, int Card0Score, int Card1Score, int Card2Score, int Card3Score, int Card4Score);

	private readonly record struct ScoredHand(Score Score, int Bid);

	public class CardScoringTests
	{
		[Theory]
		[InlineData("32T3K", 2, -1, -1, -1, -1, -1, Skip = "card scores not implemented here")]
		[InlineData("T55J5", 4, -1, -1, -1, -1, -1, Skip = "card scores not implemented here")]
		[InlineData("KK677", 3, -1, -1, -1, -1, -1, Skip = "card scores not implemented here")]
		[InlineData("KTJJT", 3, -1, -1, -1, -1, -1, Skip = "card scores not implemented here")]
		[InlineData("QQQJA", 4, -1, -1, -1, -1, -1, Skip = "card scores not implemented here")]
		public void HandsScoreCorrectly(string hand, int handScore, int card0Score, int card1Score, int card2Score, int card3Score, int card4Score)
		{
			var score = ScoreHandPart1(hand.AsSpan());

			Assert.Equal(handScore,  score.HandScore);
			Assert.Equal(card0Score, score.Card0Score);
			Assert.Equal(card1Score, score.Card1Score);
			Assert.Equal(card2Score, score.Card2Score);
			Assert.Equal(card3Score, score.Card3Score);
			Assert.Equal(card4Score, score.Card4Score);
		}
	}
}
