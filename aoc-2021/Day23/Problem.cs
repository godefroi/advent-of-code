using System.Diagnostics.CodeAnalysis;

namespace Day23;

public class Problem : ProblemBase
{
	private const int HALL_A_POS_1 = 31;
	private const int HALL_A_POS_2 = 45;
	private const int HALL_A_POS_3 = 57;
	private const int HALL_A_POS_4 = 69;

	private const int HALL_B_POS_1 = 33;
	private const int HALL_B_POS_2 = 47;
	private const int HALL_B_POS_3 = 59;
	private const int HALL_B_POS_4 = 71;

	private const int HALL_C_POS_1 = 35;
	private const int HALL_C_POS_2 = 49;
	private const int HALL_C_POS_3 = 61;
	private const int HALL_C_POS_4 = 73;

	private const int HALL_D_POS_1 = 37;
	private const int HALL_D_POS_2 = 51;
	private const int HALL_D_POS_3 = 63;
	private const int HALL_D_POS_4 = 75;

	private const int CROSS_HALL_POS_1 = 15;    // #############
	private const int CROSS_HALL_POS_2 = 16;    // #12.3.4.5.67#
	private const int CROSS_HALL_POS_3 = 18;    // ###B#C#B#D###
	private const int CROSS_HALL_POS_4 = 20;    //   #D#C#B#A#
	private const int CROSS_HALL_POS_5 = 22;    //   #D#B#A#C#
	private const int CROSS_HALL_POS_6 = 24;    //   #A#D#C#A#
	private const int CROSS_HALL_POS_7 = 25;    //   #########

	private readonly static IReadOnlySet<int> HALL_A     = new HashSet<int>() { HALL_A_POS_1, HALL_A_POS_2, HALL_A_POS_3, HALL_A_POS_4 };
	private readonly static IReadOnlySet<int> HALL_B     = new HashSet<int>() { HALL_B_POS_1, HALL_B_POS_2, HALL_B_POS_3, HALL_B_POS_4 };
	private readonly static IReadOnlySet<int> HALL_C     = new HashSet<int>() { HALL_C_POS_1, HALL_C_POS_2, HALL_C_POS_3, HALL_C_POS_4 };
	private readonly static IReadOnlySet<int> HALL_D     = new HashSet<int>() { HALL_D_POS_1, HALL_D_POS_2, HALL_D_POS_3, HALL_D_POS_4 };
	private readonly static IReadOnlySet<int> CROSS_HALL = new HashSet<int>() { CROSS_HALL_POS_1, CROSS_HALL_POS_2, CROSS_HALL_POS_3, CROSS_HALL_POS_4, CROSS_HALL_POS_5, CROSS_HALL_POS_6, CROSS_HALL_POS_7 };

	internal static void Main(string fileName)
	{
		var lines = File.ReadAllLines(GetFilePath(fileName));

		lines = lines.Take(3).Concat(new[] { "  #D#C#B#A#", "  #D#B#A#C#" }).Concat(lines.TakeLast(2)).ToArray();

		var minwin = int.MaxValue;
		var game   = ParseGame(string.Join(Environment.NewLine, lines));
		var states = new Dictionary<char[], int>(new CharArrayComparer()) {
			{ game, 0 },
		};

		Print(game);
		Console.WriteLine();

		while (states.Count > 0) {
			var newstates = new Dictionary<char[], int>(new CharArrayComparer());

			foreach (var kvp in states) {
				//if (NextPossibleStates(kvp.Key, kvp.Value).Count() == 0) {
				//	Print(kvp.Key);
				//	Console.WriteLine();
				//	System.Diagnostics.Debugger.Break();
				//}

				foreach (var newstate in NextPossibleStates(kvp.Key, kvp.Value)) {
					//Print(newstate.state);
					//Console.WriteLine();

					if (IsWin(newstate.state)) {
						Console.WriteLine($"Found a win with cost {newstate.cost}");
						minwin = Math.Min(minwin, newstate.cost);
					} else if (newstates.ContainsKey(newstate.state)) {
						newstates[newstate.state] = Math.Min(newstates[newstate.state], newstate.cost);
					} else {
						newstates.Add(newstate.state, newstate.cost);
					}
				}
			}

			states = newstates;
		}

		Console.WriteLine(minwin);
	}

	private static IEnumerable<(char[] state, int cost)> NextPossibleStates(char[] state, int cost)
	{
		foreach (var from in PositionsWhichCanMove(state)) {
			foreach (var to in PossibleDestinationsFrom(state, from)) {
				// make a new state with a move from --> to
				var newstate = state.Clone() as char[];

				var moveCost = state[from] switch {
					'A' => 1,
					'B' => 10,
					'C' => 100,
					'D' => 1000,
					_ => throw new InvalidOperationException($"Type at {from} ({state[from]}) has no cost assigned")
				};

				newstate[to.to] = newstate[from];
				newstate[from]  = '.';

				yield return (newstate, (to.score * moveCost) + cost);
			}
		}
	}

	private static void Print(char[] state) => Console.WriteLine(new string(state));

	private static IEnumerable<int> PositionsWhichCanMove(char[] state)
	{
		// each of the cross-hall positions that have something in them could potentially move
		foreach (var pos in CROSS_HALL.Where(p => state[p] != '.')) {
			yield return pos;
		}

		int? MovableRoomPosition(IReadOnlySet<int> roomPositions, char targetType)
		{
			var positions = roomPositions.OrderBy(p => p).ToList();

			if (positions.Any(p => state[p] != '.' && state[p] != targetType)) {
				// at least one position is filled with a non-target type
				//   this means that the topmost one that isn't empty can move
				//   either because it's not target type, or because one below it
				//   isn't target type (so it has to move out of the way)
				return positions.First(p => state[p] != '.');
			} else {
				return null;
			}
		}

		var pa = MovableRoomPosition(HALL_A, 'A');
		var pb = MovableRoomPosition(HALL_B, 'B');
		var pc = MovableRoomPosition(HALL_C, 'C');
		var pd = MovableRoomPosition(HALL_D, 'D');

		if (pa != null) {
			yield return pa.Value;
		}

		if (pb != null) {
			yield return pb.Value;
		}

		if (pc != null) {
			yield return pc.Value;
		}

		if (pd != null) {
			yield return pd.Value;
		}
	}

	private static IEnumerable<(int to, int score)> PossibleDestinationsFrom(char[] state, int from)
	{
		// always check whether we can move to our home location
		var homeDest = HomeHallDestination(state, state[from]);

		//if (homeDest != RoomIsAvailable(state, state[from])) {
		//	Print(state);
		//	Console.WriteLine($"moving from {from} HHD said {homeDest}, RIA said {RoomIsAvailable(state, state[from])}");
		//	throw new InvalidOperationException("old one and new one don't match");
		//}

		//if (CROSS_HALL.Contains(from) && homeDest != null) {
		//	Print(state);
		//	Console.WriteLine($"from {NameFor(from)} to {NameFor(homeDest)}, is clear: {PathIsClear(state, from, homeDest.Value)}");
		//	System.Diagnostics.Debugger.Break();
		//}

		if (homeDest != null && PathIsClear(state, from, homeDest.Value)) {
			//if ((from != 49 && homeDest != 71)
			//	&& (from != 37 && homeDest != 69)
			//	&& (from != 31 && homeDest != 75)
			//	&& (from != 51 && homeDest != 57)
			//	&& (from != 47 && homeDest != 73)
			//	&& (from != 45 && homeDest != 63)
			//	&& (from != 57 && homeDest != 51)
			//	) {
			//	Print(state);
			//	Console.WriteLine($"from {NameFor(from)} to {NameFor(homeDest)}");
			//	System.Diagnostics.Debugger.Break();
			//}

			yield return (homeDest.Value, CountMoves(from, homeDest.Value));
		}

		// if we're moving from the cross hall, we can only move into our home destination
		//   (assuming it's clear), so we're done here
		if (CROSS_HALL.Contains(from)) {
			yield break;
		}

		// otherwise, we're in a non-cross hall, so we can move to any cross-hall position
		foreach (var to in CROSS_HALL.Where(pos => PathIsClear(state, from, pos))) {
			yield return (to, CountMoves(from, to));
		}
	}

	private static int? HomeHallDestination(char[] state, char targetChar)
	{
		var positions = targetChar switch {
			'A' => HALL_A.OrderBy(p => p).ToArray(),
			'B' => HALL_B.OrderBy(p => p).ToArray(),
			'C' => HALL_C.OrderBy(p => p).ToArray(),
			'D' => HALL_D.OrderBy(p => p).ToArray(),
			_ => throw new InvalidOperationException($"No home room known for type {targetChar}"),
		};

		// if they're all clear, can move to 4
		if (state[positions[0]] == '.' && state[positions[1]] == '.' && state[positions[2]] == '.' && state[positions[3]] == '.') {
			return positions[3];
		}

		// if 1-3 are clear and 4 is target, can move to 3
		if (state[positions[0]] == '.' && state[positions[1]] == '.' && state[positions[2]] == '.' && state[positions[3]] == targetChar) {
			return positions[2];
		}

		// if 1-2 are clear and 3-4 is target, can move to 2
		if (state[positions[0]] == '.' && state[positions[1]] == '.' && state[positions[2]] == targetChar && state[positions[3]] == targetChar) {
			return positions[1];
		}

		// if 1 is clear and 2-4 are target, can move to 1
		if (state[positions[0]] == '.' && state[positions[1]] == targetChar && state[positions[2]] == targetChar && state[positions[3]] == targetChar) {
			return positions[0];
		}

		return null;
	}

	private static bool PathIsClear(char[] state, int from, int to)
	{
		// for hall-hall moves, we assume that the hall above us is clear and the target hall is clear to the destination
		if ((HALL_A.Contains(from) || HALL_A.Contains(to)) && (HALL_B.Contains(from) || HALL_B.Contains(to))) {
			return state[CROSS_HALL_POS_3] == '.';
		}

		if ((HALL_A.Contains(from) || HALL_A.Contains(to)) && (HALL_C.Contains(from) || HALL_C.Contains(to))) {
			return state[CROSS_HALL_POS_3] == '.' && state[CROSS_HALL_POS_4] == '.';
		}

		if ((HALL_A.Contains(from) || HALL_A.Contains(to)) && (HALL_D.Contains(from) || HALL_D.Contains(to))) {
			return state[CROSS_HALL_POS_3] == '.' && state[CROSS_HALL_POS_4] == '.' && state[CROSS_HALL_POS_5] == '.';
		}

		if ((HALL_B.Contains(from) || HALL_B.Contains(to)) && (HALL_C.Contains(from) || HALL_C.Contains(to))) {
			return state[CROSS_HALL_POS_4] == '.';
		}

		if ((HALL_B.Contains(from) || HALL_B.Contains(to)) && (HALL_D.Contains(from) || HALL_D.Contains(to))) {
			return state[CROSS_HALL_POS_4] == '.' && state[CROSS_HALL_POS_5] == '.';
		}

		if ((HALL_C.Contains(from) || HALL_C.Contains(to)) && (HALL_D.Contains(from) || HALL_D.Contains(to))) {
			return state[CROSS_HALL_POS_5] == '.';
		}

		// for room->hall moves, we assume that the target room is clear to the destination
		// we do NOT assume that the target position is clear if it's in the hall

		if (HALL_A.Contains(from)) {
			return to switch {
				CROSS_HALL_POS_1 => state[CROSS_HALL_POS_1] == '.' && state[CROSS_HALL_POS_2] == '.',
				CROSS_HALL_POS_2 => state[CROSS_HALL_POS_2] == '.',
				CROSS_HALL_POS_3 => state[CROSS_HALL_POS_3] == '.',
				CROSS_HALL_POS_4 => state[CROSS_HALL_POS_3] == '.' && state[CROSS_HALL_POS_4] == '.',
				CROSS_HALL_POS_5 => state[CROSS_HALL_POS_3] == '.' && state[CROSS_HALL_POS_4] == '.' && state[CROSS_HALL_POS_5] == '.',
				CROSS_HALL_POS_6 => state[CROSS_HALL_POS_3] == '.' && state[CROSS_HALL_POS_4] == '.' && state[CROSS_HALL_POS_5] == '.' && state[CROSS_HALL_POS_6] == '.',
				CROSS_HALL_POS_7 => state[CROSS_HALL_POS_3] == '.' && state[CROSS_HALL_POS_4] == '.' && state[CROSS_HALL_POS_5] == '.' && state[CROSS_HALL_POS_6] == '.' && state[CROSS_HALL_POS_7] == '.',
				_ => throw new InvalidOperationException($"Invalid hall->room move from {NameFor(from)} to {NameFor(to)}"),
			};
		}

		if (HALL_B.Contains(from)) {
			return to switch {
				CROSS_HALL_POS_1 => state[CROSS_HALL_POS_1] == '.' && state[CROSS_HALL_POS_2] == '.' && state[CROSS_HALL_POS_3] == '.',
				CROSS_HALL_POS_2 => state[CROSS_HALL_POS_2] == '.' && state[CROSS_HALL_POS_3] == '.',
				CROSS_HALL_POS_3 => state[CROSS_HALL_POS_3] == '.',
				CROSS_HALL_POS_4 => state[CROSS_HALL_POS_4] == '.',
				CROSS_HALL_POS_5 => state[CROSS_HALL_POS_4] == '.' && state[CROSS_HALL_POS_5] == '.',
				CROSS_HALL_POS_6 => state[CROSS_HALL_POS_4] == '.' && state[CROSS_HALL_POS_5] == '.' && state[CROSS_HALL_POS_6] == '.',
				CROSS_HALL_POS_7 => state[CROSS_HALL_POS_4] == '.' && state[CROSS_HALL_POS_5] == '.' && state[CROSS_HALL_POS_6] == '.' && state[CROSS_HALL_POS_7] == '.',
				_ => throw new InvalidOperationException($"Invalid hall->room move from {NameFor(from)} to {NameFor(to)}"),
			};
		}

		if (HALL_C.Contains(from)) {
			return to switch {
				CROSS_HALL_POS_1 => state[CROSS_HALL_POS_1] == '.' && state[CROSS_HALL_POS_2] == '.' && state[CROSS_HALL_POS_3] == '.' && state[CROSS_HALL_POS_4] == '.',
				CROSS_HALL_POS_2 => state[CROSS_HALL_POS_2] == '.' && state[CROSS_HALL_POS_3] == '.' && state[CROSS_HALL_POS_4] == '.',
				CROSS_HALL_POS_3 => state[CROSS_HALL_POS_3] == '.' && state[CROSS_HALL_POS_4] == '.',
				CROSS_HALL_POS_4 => state[CROSS_HALL_POS_4] == '.',
				CROSS_HALL_POS_5 => state[CROSS_HALL_POS_5] == '.',
				CROSS_HALL_POS_6 => state[CROSS_HALL_POS_5] == '.' && state[CROSS_HALL_POS_6] == '.',
				CROSS_HALL_POS_7 => state[CROSS_HALL_POS_5] == '.' && state[CROSS_HALL_POS_6] == '.' && state[CROSS_HALL_POS_7] == '.',
				_ => throw new InvalidOperationException($"Invalid hall->room move from {NameFor(from)} to {NameFor(to)}"),
			};
		}

		if (HALL_D.Contains(from)) {
			return to switch {
				CROSS_HALL_POS_1 => state[CROSS_HALL_POS_1] == '.' && state[CROSS_HALL_POS_2] == '.' && state[CROSS_HALL_POS_3] == '.' && state[CROSS_HALL_POS_4] == '.' && state[CROSS_HALL_POS_5] == '.',
				CROSS_HALL_POS_2 => state[CROSS_HALL_POS_2] == '.' && state[CROSS_HALL_POS_3] == '.' && state[CROSS_HALL_POS_4] == '.' && state[CROSS_HALL_POS_5] == '.',
				CROSS_HALL_POS_3 => state[CROSS_HALL_POS_3] == '.' && state[CROSS_HALL_POS_4] == '.' && state[CROSS_HALL_POS_5] == '.',
				CROSS_HALL_POS_4 => state[CROSS_HALL_POS_4] == '.' && state[CROSS_HALL_POS_5] == '.',
				CROSS_HALL_POS_5 => state[CROSS_HALL_POS_5] == '.',
				CROSS_HALL_POS_6 => state[CROSS_HALL_POS_6] == '.',
				CROSS_HALL_POS_7 => state[CROSS_HALL_POS_6] == '.' && state[CROSS_HALL_POS_7] == '.',
				_ => throw new InvalidOperationException($"Invalid hall->room move from {NameFor(from)} to {NameFor(to)}"),
			};
		}

		// finally, hall->room moves... for these, we only need to check the hall, not the room
		//   (it's assumed clear to the destination)

		if (HALL_A.Contains(to)) {
			return from switch {
				CROSS_HALL_POS_1 => state[CROSS_HALL_POS_2] == '.',
				CROSS_HALL_POS_2 => true,
				CROSS_HALL_POS_3 => true,
				CROSS_HALL_POS_4 => state[CROSS_HALL_POS_3] == '.',
				CROSS_HALL_POS_5 => state[CROSS_HALL_POS_3] == '.' && state[CROSS_HALL_POS_4] == '.',
				CROSS_HALL_POS_6 => state[CROSS_HALL_POS_3] == '.' && state[CROSS_HALL_POS_4] == '.' && state[CROSS_HALL_POS_5] == '.',
				CROSS_HALL_POS_7 => state[CROSS_HALL_POS_3] == '.' && state[CROSS_HALL_POS_4] == '.' && state[CROSS_HALL_POS_5] == '.' && state[CROSS_HALL_POS_6] == '.',
				_ => throw new InvalidOperationException($"Invalid hall->room move from {NameFor(from)} to {NameFor(to)}"),
			};
		}

		if (HALL_B.Contains(to)) {
			return from switch {
				CROSS_HALL_POS_1 => state[CROSS_HALL_POS_2] == '.' && state[CROSS_HALL_POS_3] == '.',
				CROSS_HALL_POS_2 => state[CROSS_HALL_POS_3] == '.',
				CROSS_HALL_POS_3 => true,
				CROSS_HALL_POS_4 => true,
				CROSS_HALL_POS_5 => state[CROSS_HALL_POS_4] == '.',
				CROSS_HALL_POS_6 => state[CROSS_HALL_POS_4] == '.' && state[CROSS_HALL_POS_5] == '.',
				CROSS_HALL_POS_7 => state[CROSS_HALL_POS_4] == '.' && state[CROSS_HALL_POS_5] == '.' && state[CROSS_HALL_POS_6] == '.',
				_ => throw new InvalidOperationException($"Invalid hall->room move from {NameFor(from)} to {NameFor(to)}"),
			};
		}

		if (HALL_C.Contains(to)) {
			return from switch {
				CROSS_HALL_POS_1 => state[CROSS_HALL_POS_2] == '.' && state[CROSS_HALL_POS_3] == '.' && state[CROSS_HALL_POS_4] == '.',
				CROSS_HALL_POS_2 => state[CROSS_HALL_POS_3] == '.' && state[CROSS_HALL_POS_4] == '.',
				CROSS_HALL_POS_3 => state[CROSS_HALL_POS_4] == '.',
				CROSS_HALL_POS_4 => true,
				CROSS_HALL_POS_5 => true,
				CROSS_HALL_POS_6 => state[CROSS_HALL_POS_5] == '.',
				CROSS_HALL_POS_7 => state[CROSS_HALL_POS_5] == '.' && state[CROSS_HALL_POS_6] == '.',
				_ => throw new InvalidOperationException($"Invalid hall->room move from {NameFor(from)} to {NameFor(to)}"),
			};
		}

		if (HALL_D.Contains(to)) {
			return from switch {
				CROSS_HALL_POS_1 => state[CROSS_HALL_POS_2] == '.' && state[CROSS_HALL_POS_3] == '.' && state[CROSS_HALL_POS_4] == '.' && state[CROSS_HALL_POS_5] == '.',
				CROSS_HALL_POS_2 => state[CROSS_HALL_POS_3] == '.' && state[CROSS_HALL_POS_4] == '.' && state[CROSS_HALL_POS_5] == '.',
				CROSS_HALL_POS_3 => state[CROSS_HALL_POS_4] == '.' && state[CROSS_HALL_POS_5] == '.',
				CROSS_HALL_POS_4 => state[CROSS_HALL_POS_5] == '.',
				CROSS_HALL_POS_5 => true,
				CROSS_HALL_POS_6 => true,
				CROSS_HALL_POS_7 => state[CROSS_HALL_POS_6] == '.',
				_ => throw new InvalidOperationException($"Invalid hall->room move from {NameFor(from)} to {NameFor(to)}"),
			};
		}

		throw new InvalidOperationException($"Cannot check for clear movement from {NameFor(from)} to {NameFor(to)}");
	}

	private static int CountMoves(int from, int to)
	{
		var from_hall_moves = 0;
		var to_hall_moves   = 0;

		int HallMoves(IReadOnlySet<int> hall, int pos)
		{
			if (hall.Contains(pos)) {
				return hall.OrderBy(p => p).ToList().IndexOf(pos) + 1;
			} else {
				return 0;
			}
		}

		// hall -> hall and hall -> crosshall
		if ((from_hall_moves = HallMoves(HALL_A, from)) > 0) {
			if ((to_hall_moves = HallMoves(HALL_B, to)) > 0) {
				return from_hall_moves + to_hall_moves + 2;
			}

			if ((to_hall_moves = HallMoves(HALL_C, to)) > 0) {
				return from_hall_moves + to_hall_moves + 4;
			}

			if ((to_hall_moves = HallMoves(HALL_D, to)) > 0) {
				return from_hall_moves + to_hall_moves + 6;
			}

			if (CROSS_HALL.Contains(to)) {
				return to switch {
					CROSS_HALL_POS_1 => 2 + from_hall_moves,
					CROSS_HALL_POS_2 => 1 + from_hall_moves,
					CROSS_HALL_POS_3 => 1 + from_hall_moves,
					CROSS_HALL_POS_4 => 3 + from_hall_moves,
					CROSS_HALL_POS_5 => 5 + from_hall_moves,
					CROSS_HALL_POS_6 => 7 + from_hall_moves,
					CROSS_HALL_POS_7 => 8 + from_hall_moves,
					_ => throw new InvalidOperationException("Error in place 1"),
				};
			}
		}

		if ((from_hall_moves = HallMoves(HALL_B, from)) > 0) {
			if ((to_hall_moves = HallMoves(HALL_A, to)) > 0) {
				return from_hall_moves + to_hall_moves + 2;
			}

			if ((to_hall_moves = HallMoves(HALL_C, to)) > 0) {
				return from_hall_moves + to_hall_moves + 2;
			}

			if ((to_hall_moves = HallMoves(HALL_D, to)) > 0) {
				return from_hall_moves + to_hall_moves + 4;
			}

			if (CROSS_HALL.Contains(to)) {
				return to switch {
					CROSS_HALL_POS_1 => 4 + from_hall_moves,
					CROSS_HALL_POS_2 => 3 + from_hall_moves,
					CROSS_HALL_POS_3 => 1 + from_hall_moves,
					CROSS_HALL_POS_4 => 1 + from_hall_moves,
					CROSS_HALL_POS_5 => 3 + from_hall_moves,
					CROSS_HALL_POS_6 => 5 + from_hall_moves,
					CROSS_HALL_POS_7 => 6 + from_hall_moves,
					_ => throw new InvalidOperationException("Error in place 2"),
				};
			}
		}

		if ((from_hall_moves = HallMoves(HALL_C, from)) > 0) {
			if ((to_hall_moves = HallMoves(HALL_A, to)) > 0) {
				return from_hall_moves + to_hall_moves + 4;
			}

			if ((to_hall_moves = HallMoves(HALL_B, to)) > 0) {
				return from_hall_moves + to_hall_moves + 2;
			}

			if ((to_hall_moves = HallMoves(HALL_D, to)) > 0) {
				return from_hall_moves + to_hall_moves + 2;
			}

			if (CROSS_HALL.Contains(to)) {
				return to switch {
					CROSS_HALL_POS_1 => 6 + from_hall_moves,
					CROSS_HALL_POS_2 => 5 + from_hall_moves,
					CROSS_HALL_POS_3 => 3 + from_hall_moves,
					CROSS_HALL_POS_4 => 1 + from_hall_moves,
					CROSS_HALL_POS_5 => 1 + from_hall_moves,
					CROSS_HALL_POS_6 => 3 + from_hall_moves,
					CROSS_HALL_POS_7 => 4 + from_hall_moves,
					_ => throw new InvalidOperationException("Error in place 3"),
				};
			}
		}

		if ((from_hall_moves = HallMoves(HALL_D, from)) > 0) {
			if ((to_hall_moves = HallMoves(HALL_A, to)) > 0) {
				return from_hall_moves + to_hall_moves + 6;
			}

			if ((to_hall_moves = HallMoves(HALL_B, to)) > 0) {
				return from_hall_moves + to_hall_moves + 4;
			}

			if ((to_hall_moves = HallMoves(HALL_C, to)) > 0) {
				return from_hall_moves + to_hall_moves + 2;
			}

			if (CROSS_HALL.Contains(to)) {
				return to switch {
					CROSS_HALL_POS_1 => 8 + from_hall_moves,
					CROSS_HALL_POS_2 => 7 + from_hall_moves,
					CROSS_HALL_POS_3 => 5 + from_hall_moves,
					CROSS_HALL_POS_4 => 3 + from_hall_moves,
					CROSS_HALL_POS_5 => 1 + from_hall_moves,
					CROSS_HALL_POS_6 => 1 + from_hall_moves,
					CROSS_HALL_POS_7 => 2 + from_hall_moves,
					_ => throw new InvalidOperationException("Error in place 4"),
				};
			}
		}

		// crosshall -> hall
		if ((to_hall_moves = HallMoves(HALL_A, to)) > 0) {
			return from switch {
				CROSS_HALL_POS_1 => 2 + to_hall_moves,
				CROSS_HALL_POS_2 => 1 + to_hall_moves,
				CROSS_HALL_POS_3 => 1 + to_hall_moves,
				CROSS_HALL_POS_4 => 3 + to_hall_moves,
				CROSS_HALL_POS_5 => 5 + to_hall_moves,
				CROSS_HALL_POS_6 => 7 + to_hall_moves,
				CROSS_HALL_POS_7 => 8 + to_hall_moves,
				_ => throw new InvalidOperationException("Error in place 5"),
			};
		}

		if ((to_hall_moves = HallMoves(HALL_B, to)) > 0) {
			return from switch {
				CROSS_HALL_POS_1 => 4 + to_hall_moves,
				CROSS_HALL_POS_2 => 3 + to_hall_moves,
				CROSS_HALL_POS_3 => 1 + to_hall_moves,
				CROSS_HALL_POS_4 => 1 + to_hall_moves,
				CROSS_HALL_POS_5 => 3 + to_hall_moves,
				CROSS_HALL_POS_6 => 5 + to_hall_moves,
				CROSS_HALL_POS_7 => 6 + to_hall_moves,
				_ => throw new InvalidOperationException("Error in place 6"),
			};
		}

		if ((to_hall_moves = HallMoves(HALL_C, to)) > 0) {
			return from switch {
				CROSS_HALL_POS_1 => 6 + to_hall_moves,
				CROSS_HALL_POS_2 => 5 + to_hall_moves,
				CROSS_HALL_POS_3 => 3 + to_hall_moves,
				CROSS_HALL_POS_4 => 1 + to_hall_moves,
				CROSS_HALL_POS_5 => 1 + to_hall_moves,
				CROSS_HALL_POS_6 => 3 + to_hall_moves,
				CROSS_HALL_POS_7 => 4 + to_hall_moves,
				_ => throw new InvalidOperationException("Error in place 7"),
			};
		}

		if ((to_hall_moves = HallMoves(HALL_D, to)) > 0) {
			return from switch {
				CROSS_HALL_POS_1 => 8 + to_hall_moves,
				CROSS_HALL_POS_2 => 7 + to_hall_moves,
				CROSS_HALL_POS_3 => 5 + to_hall_moves,
				CROSS_HALL_POS_4 => 3 + to_hall_moves,
				CROSS_HALL_POS_5 => 1 + to_hall_moves,
				CROSS_HALL_POS_6 => 1 + to_hall_moves,
				CROSS_HALL_POS_7 => 2 + to_hall_moves,
				_ => throw new InvalidOperationException("Error in place 8"),
			};
		}

		throw new InvalidOperationException($"Did not find a calculation for a move from {from} to {to}");
	}

	private static bool IsWin(char[] state) =>
		HALL_A.All(p => state[p] == 'A') &&
		HALL_B.All(p => state[p] == 'B') &&
		HALL_C.All(p => state[p] == 'C') &&
		HALL_D.All(p => state[p] == 'D');

	private static string NameFor(int? position)
	{
		if (position == null) {
			return "{none}";
		}

		var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static;

		return typeof(Problem).GetFields(flags).Where(f => f.FieldType == typeof(int) && f.IsLiteral && !f.IsInitOnly).Single(f => (int)f.GetValue(null) == position).Name;
	}

	// ************************************
	// new stuff here

	private static int? RoomIsAvailable(char[] state, char targetRoom)
	{
		switch (targetRoom) {
			case 'A':
				if (state[HALL_A_POS_1] == '.' && state[HALL_A_POS_2] == '.' && state[HALL_A_POS_3] == '.' && state[HALL_A_POS_4] == '.') {
					return HALL_A_POS_4;
				} else if (state[HALL_A_POS_1] == '.' && state[HALL_A_POS_2] == '.' && state[HALL_A_POS_3] == '.' && state[HALL_A_POS_4] == 'A') {
					return HALL_A_POS_3;
				} else if (state[HALL_A_POS_1] == '.' && state[HALL_A_POS_2] == '.' && state[HALL_A_POS_3] == 'A' && state[HALL_A_POS_4] == 'A') {
					return HALL_A_POS_2;
				} else if (state[HALL_A_POS_1] == '.' && state[HALL_A_POS_2] == 'A' && state[HALL_A_POS_3] == 'A' && state[HALL_A_POS_4] == 'A') {
					return HALL_A_POS_1;
				} else {
					return null;
				}

			case 'B':
				if (state[HALL_B_POS_1] == '.' && state[HALL_B_POS_2] == '.' && state[HALL_B_POS_3] == '.' && state[HALL_B_POS_4] == '.') {
					return HALL_B_POS_4;
				} else if (state[HALL_B_POS_1] == '.' && state[HALL_B_POS_2] == '.' && state[HALL_B_POS_3] == '.' && state[HALL_B_POS_4] == 'B') {
					return HALL_B_POS_3;
				} else if (state[HALL_B_POS_1] == '.' && state[HALL_B_POS_2] == '.' && state[HALL_B_POS_3] == 'B' && state[HALL_B_POS_4] == 'B') {
					return HALL_B_POS_2;
				} else if (state[HALL_B_POS_1] == '.' && state[HALL_B_POS_2] == 'B' && state[HALL_B_POS_3] == 'B' && state[HALL_B_POS_4] == 'B') {
					return HALL_B_POS_1;
				} else {
					return null;
				}

			case 'C':
				if (state[HALL_C_POS_1] == '.' && state[HALL_C_POS_2] == '.' && state[HALL_C_POS_3] == '.' && state[HALL_C_POS_4] == '.') {
					return HALL_C_POS_4;
				} else if (state[HALL_C_POS_1] == '.' && state[HALL_C_POS_2] == '.' && state[HALL_C_POS_3] == '.' && state[HALL_C_POS_4] == 'C') {
					return HALL_C_POS_3;
				} else if (state[HALL_C_POS_1] == '.' && state[HALL_C_POS_2] == '.' && state[HALL_C_POS_3] == 'C' && state[HALL_C_POS_4] == 'C') {
					return HALL_C_POS_2;
				} else if (state[HALL_C_POS_1] == '.' && state[HALL_C_POS_2] == 'C' && state[HALL_C_POS_3] == 'C' && state[HALL_C_POS_4] == 'C') {
					return HALL_C_POS_1;
				} else {
					return null;
				}

			case 'D':
				if (state[HALL_D_POS_1] == '.' && state[HALL_D_POS_2] == '.' && state[HALL_D_POS_3] == '.' && state[HALL_D_POS_4] == '.') {
					return HALL_D_POS_4;
				} else if (state[HALL_D_POS_1] == '.' && state[HALL_D_POS_2] == '.' && state[HALL_D_POS_3] == '.' && state[HALL_D_POS_4] == 'D') {
					return HALL_D_POS_3;
				} else if (state[HALL_D_POS_1] == '.' && state[HALL_D_POS_2] == '.' && state[HALL_D_POS_3] == 'D' && state[HALL_D_POS_4] == 'D') {
					return HALL_D_POS_2;
				} else if (state[HALL_D_POS_1] == '.' && state[HALL_D_POS_2] == 'D' && state[HALL_D_POS_3] == 'D' && state[HALL_D_POS_4] == 'D') {
					return HALL_D_POS_1;
				} else {
					return null;
				}

			default:
				throw new InvalidOperationException($"There is no destination room for {targetRoom}");
		}
	}

#pragma warning disable IDE0051 // Remove unused private members
	private static void GenerateConstants()
#pragma warning restore IDE0051 // Remove unused private members
	{
/*
#############
#...........#
###B#C#B#D###
  #D#C#B#A#
  #D#B#A#C#
  #A#D#C#A#
  #########
*/
		var g1 = @"#############
#QR.S.T.U.VW#
###A#E#I#M###
  #B#F#J#N#
  #C#G#K#O#
  #D#H#L#P#
  #########";

		var g = ParseGame(g1);

		Console.WriteLine($"private const int HALL_A_POS_1 = {Array.IndexOf(g, 'A')};");
		Console.WriteLine($"private const int HALL_A_POS_2 = {Array.IndexOf(g, 'B')};");
		Console.WriteLine($"private const int HALL_A_POS_3 = {Array.IndexOf(g, 'C')};");
		Console.WriteLine($"private const int HALL_A_POS_4 = {Array.IndexOf(g, 'D')};");
		Console.WriteLine($"private const int HALL_B_POS_1 = {Array.IndexOf(g, 'E')};");
		Console.WriteLine($"private const int HALL_B_POS_2 = {Array.IndexOf(g, 'F')};");
		Console.WriteLine($"private const int HALL_B_POS_3 = {Array.IndexOf(g, 'G')};");
		Console.WriteLine($"private const int HALL_B_POS_4 = {Array.IndexOf(g, 'H')};");
		Console.WriteLine($"private const int HALL_C_POS_1 = {Array.IndexOf(g, 'I')};");
		Console.WriteLine($"private const int HALL_C_POS_2 = {Array.IndexOf(g, 'J')};");
		Console.WriteLine($"private const int HALL_C_POS_3 = {Array.IndexOf(g, 'K')};");
		Console.WriteLine($"private const int HALL_C_POS_4 = {Array.IndexOf(g, 'L')};");
		Console.WriteLine($"private const int HALL_D_POS_1 = {Array.IndexOf(g, 'M')};");
		Console.WriteLine($"private const int HALL_D_POS_2 = {Array.IndexOf(g, 'N')};");
		Console.WriteLine($"private const int HALL_D_POS_3 = {Array.IndexOf(g, 'O')};");
		Console.WriteLine($"private const int HALL_D_POS_4 = {Array.IndexOf(g, 'P')};");

		Console.WriteLine($"private const int CROSS_HALL_POS_1 = {Array.IndexOf(g, 'Q')};");
		Console.WriteLine($"private const int CROSS_HALL_POS_2 = {Array.IndexOf(g, 'R')};");
		Console.WriteLine($"private const int CROSS_HALL_POS_3 = {Array.IndexOf(g, 'S')};");
		Console.WriteLine($"private const int CROSS_HALL_POS_4 = {Array.IndexOf(g, 'T')};");
		Console.WriteLine($"private const int CROSS_HALL_POS_5 = {Array.IndexOf(g, 'U')};");
		Console.WriteLine($"private const int CROSS_HALL_POS_6 = {Array.IndexOf(g, 'V')};");
		Console.WriteLine($"private const int CROSS_HALL_POS_7 = {Array.IndexOf(g, 'W')};");
	}

	internal static char[] ParseGame(string game) => game.ReplaceLineEndings("\n").ToCharArray();

	internal class CharArrayComparer : IEqualityComparer<char[]>
	{
		public bool Equals(char[]? x, char[]? y) => x != null && x.SequenceEqual(y);

		public int GetHashCode([DisallowNull]char[] obj)
		{
			var hc_hall_a = HashCode.Combine(obj[HALL_A_POS_1], obj[HALL_A_POS_2], obj[HALL_A_POS_3], obj[HALL_A_POS_4]);
			var hc_hall_b = HashCode.Combine(obj[HALL_B_POS_1], obj[HALL_B_POS_2], obj[HALL_B_POS_3], obj[HALL_B_POS_4]);
			var hc_hall_c = HashCode.Combine(obj[HALL_C_POS_1], obj[HALL_C_POS_2], obj[HALL_C_POS_3], obj[HALL_C_POS_4]);
			var hc_hall_d = HashCode.Combine(obj[HALL_D_POS_1], obj[HALL_D_POS_2], obj[HALL_D_POS_3], obj[HALL_D_POS_4]);
			var hc_cross  = HashCode.Combine(obj[CROSS_HALL_POS_1], obj[CROSS_HALL_POS_2], obj[CROSS_HALL_POS_3], obj[CROSS_HALL_POS_4], obj[CROSS_HALL_POS_5], obj[CROSS_HALL_POS_6], obj[CROSS_HALL_POS_7]);

			return HashCode.Combine(hc_hall_a, hc_hall_b, hc_hall_c, hc_hall_d, hc_cross);
		}
	}
}
