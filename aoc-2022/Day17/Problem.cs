using System.Diagnostics;
using System.Text;

namespace aoc_2022.Day17;

public class Problem
{
	private const char TRANSLATE_LEFT    = '<';
	private const char TRANSLATE_RIGHT   = '>';
	private const long ROCK_COUNT_MAX_P1 = 2022;
	private const long ROCK_COUNT_MAX_P2 = 1000000000000;

	private static readonly Dictionary<RockType, Coordinate[]> _rocks = new() {
		{ RockType.Horizontal, new[] {
			new Coordinate(2, 0),
			new Coordinate(3, 0),
			new Coordinate(4, 0),
			new Coordinate(5, 0) } },
		{ RockType.Plus, new[] {
			new Coordinate(3, 2),
			new Coordinate(2, 1),
			new Coordinate(3, 1),
			new Coordinate(4, 1),
			new Coordinate(3, 0) } },
		{ RockType.BackwardsL, new[] {
			new Coordinate(4, 2),
			new Coordinate(4, 1),
			new Coordinate(2, 0),
			new Coordinate(3, 0),
			new Coordinate(4, 0) } },
		{ RockType.Vertical, new[] {
			new Coordinate(2, 3),
			new Coordinate(2, 2),
			new Coordinate(2, 1),
			new Coordinate(2, 0) } },
		{ RockType.Block, new[] {
			new Coordinate(2, 1),
			new Coordinate(3, 1),
			new Coordinate(2, 0),
			new Coordinate(3, 0) } }
	};

	private static readonly Dictionary<RockType, RockType> _nextRockType = new() {
		{ RockType.None,       RockType.Horizontal },
		{ RockType.Block,      RockType.Horizontal },
		{ RockType.Horizontal, RockType.Plus       },
		{ RockType.Plus,       RockType.BackwardsL },
		{ RockType.BackwardsL, RockType.Vertical   },
		{ RockType.Vertical,   RockType.Block      },
	};

	public static (long, long) Main(string fileName)
	{
		//fileName = "inputSample.txt";

		var part1 = RunSimulation(fileName, ROCK_COUNT_MAX_P1);
		var part2 = RunSimulation(fileName, ROCK_COUNT_MAX_P2);

		return (part1, part2);
	}

	private static long RunSimulation(string fileName, long rockCount)
	{
		var jets          = ReadFileLines(fileName).Single().ToCharArray().Select(c => (Translation)c).ToArray();
		var chamber       = new HashSet<Coordinate>();
		var currentHeight = 0L;
		var currentJet    = 0;
		var lastRock      = RockType.None;
		var states        = new Dictionary<string, (long, long)>();
		var loopState     = default(string?);
		var heightAdd     = 0L;

		Coordinate[] rock;

		while (rockCount > 0) {
			// get a rock
			(rock, lastRock) = GenerateRock(currentHeight, lastRock);

			//Draw(chamber, rock, currentHeight);

			while (rockCount > 0) {
				// translate the rock to the side
				var translatedRock = TranslateRock(rock, jets[currentJet++]);

				// reset the current jet if we need to
				if (currentJet >= jets.Length) {
					currentJet = 0;
				}

				// if the translation caused us to overlap something in the chamber, skip it
				if (translatedRock.Any(chamber.Contains)) {
					translatedRock = rock;
				}

				//Draw(chamber, translatedRock, currentHeight);

				// translate the rock down
				var fallenRock = FallRock(translatedRock);

				// if the fallen rock interferes with an existing rock in the chamber (or the floor),
				// then it becomes part of the chamber (in its pre-fallen state) and we start a new rock
				if (fallenRock.Any(c => c.y < 0) || fallenRock.Any(chamber.Contains)) {
					Draw(chamber, translatedRock, currentHeight);

					foreach (var c in translatedRock) {
						chamber.Add(c);
					}

					// our new max height is the higher of our current height and the highest part of the new rock
					currentHeight = Math.Max(currentHeight, translatedRock.Max(c => c.y) + 1);

					// count this rock
					rockCount--;

					// if we've seen this state before, fast-forward past any loops
					if (loopState == null) {
						var (state, rockPositions) = CalculateState(chamber, currentHeight, lastRock, currentJet);

						if (states.TryGetValue(state, out var vals)) {
							loopState  = state;

							var rockCountInc = vals.Item2 - rockCount;
							var curHeightInc = currentHeight - vals.Item1;
							var (quotient, remainder) = Math.DivRem(rockCount, rockCountInc);

							heightAdd = quotient * curHeightInc;
							rockCount = remainder;
						} else {
							states.Add(state, (currentHeight, rockCount));
						}
					}

					// start a new rock
					break;
				}

				//Draw(chamber, fallenRock, currentHeight);

				// otherwise, we continue with this rock
				rock = fallenRock;
			}
		}

		return currentHeight + heightAdd;
	}

	private static (string state, int?[] rockPositions) CalculateState(HashSet<Coordinate> chamber, long currentHeight, RockType lastRock, int jetIndex)
	{
		var sb  = new StringBuilder();
		var pos = new int?[7];

		for (var y = currentHeight; y >= 0; y--) {
			for (var x = 0; x < 7; x++) {
				if (pos[x] == null && chamber.Contains(new Coordinate(x, y))) {
					pos[x] = (int)(currentHeight - y);
				}
			}
		}

		sb.AppendJoin(',', pos);
		sb.Append('-');
		sb.Append(lastRock.ToString());
		sb.Append('-');
		sb.Append(jetIndex);

		return (sb.ToString(), pos);
	}

	private static Coordinate[] TranslateRock(Coordinate[] rock, Translation translation)
	{
		// if we're up against the relevant edge, skip this translation
		if (translation == Translation.Left && rock.Any(c => c.x == 0)) {
			return rock;
		} else if (translation == Translation.Right && rock.Any(c => c.x == 6)) {
			return rock;
		}

		var ret = new Coordinate[rock.Length];

		for (var i = 0; i < rock.Length; i++) {
			ret[i] = translation switch {
				Translation.Left  => new Coordinate(rock[i].x - 1, rock[i].y),
				Translation.Right => new Coordinate(rock[i].x + 1, rock[i].y),
				_ => throw new InvalidOperationException("Specified transform is invalid.")
			};
		}

		return ret;
	}

	private static Coordinate[] FallRock(Coordinate[] rock)
	{
		var ret = new Coordinate[rock.Length];

		for (var i = 0; i < rock.Length; i++) {
			ret[i] = new Coordinate(rock[i].x, rock[i].y - 1);
		}

		return ret;
	}

	private static (Coordinate[] Rock, RockType RockType) GenerateRock(long currentHeight, RockType lastRockType) => (_rocks[_nextRockType[lastRockType]].Select(c => new Coordinate(c.x, c.y + currentHeight + 3)).ToArray(), _nextRockType[lastRockType]);

	private static void Draw(HashSet<Coordinate> chamber, Coordinate[] currentRock, long currentHeight)
	{
		if (!Debugger.IsAttached) {
			return;
		}

		var sb = new StringBuilder(9);

		Console.Clear();

		for (var y = currentRock.Length == 0 ? currentHeight : currentRock.Max(c => c.y); y >= 0; y--) {
			sb.Append('|');

			for (var x = 0; x < 7; x++) {
				if (chamber.Contains(new Coordinate(x, y))) {
					sb.Append('#');
				} else if (currentRock.Any(c => c.x == x && c.y == y)) {
					sb.Append('@');
				} else {
					sb.Append('.');
				}
			}

			sb.Append('|');

			Console.WriteLine(sb.ToString());

			sb.Clear();
		}

		Console.WriteLine("+-------+");
	}

	private enum Translation
	{
		Left  = TRANSLATE_LEFT,
		Right = TRANSLATE_RIGHT,
	}

	private enum RockType
	{
		None,
		Horizontal,
		Plus,
		BackwardsL,
		Vertical,
		Block
	}
	
	private readonly record struct Coordinate(int x, long y);
}
