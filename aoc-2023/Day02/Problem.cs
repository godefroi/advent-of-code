using System.Buffers;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;

namespace AdventOfCode.Year2023.Day02;

public partial class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Benchmarks));

	public static (long, long) Execute(string[] input)
	{
		//input.Select(ParseBySpan).ToList();
		//return (1, 1);

		// implementation based on the class
		// (slower than span-based parsing) var games = input.Select(ParseBySplit).ToList();
		// var games = input.Select(ParseBySpan).ToList();
		// var stats = games.Select(CalculateStats);
		// var part1 = stats.Where(s => s.possible).Sum(s => s.id);
		// var part2 = stats.Sum(s => s.power);

		// implementation based on the (ref) struct
		var part1 = 0;
		var part2 = 0;

		//foreach (var line in input) {
		Parallel.ForEach(input, line => {
			var (id, possible, power) = CalculateStats(ParseBySpanIntoStruct(line));

			if (possible) {
				Interlocked.Add(ref part1, id);
			}

			Interlocked.Add(ref part2, power);
		});

		// var tot   = 0;

		// foreach (var gameLine in input) {
		// 	//Possible(game);
		// 	var game        = Parse(gameLine);
		// 	var stringified = Stringify(game);
		// 	var possible    = Possible(game);

		// 	Console.WriteLine($"{(possible ? '!' : ' ')} {(stringified == gameLine ? '+' : '-')} {stringified}");

		// 	if (possible) {
		// 		tot += game.Id;
		// 	}
		// }

		return (part1, part2);

		// var game = Parse("Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green");

		// Console.WriteLine(game);
		// foreach (var hand in game.Hands) {
		// 	Console.WriteLine("hand");
		// 	foreach (var kvp in hand) {
		// 		Console.WriteLine($"\t{kvp.Value} {kvp.Key}");
		// 	}
		// }

		//Console.WriteLine(input.Length);
		//return (-1, -1);
	}

	private static string Stringify(Game game)
	{
		var hands = game.Hands.Select(hand => string.Join(", ", hand.Select(kvp => $"{kvp.Value} {kvp.Key}")));
		return $"Game {game.Id}: {string.Join("; ", hands)}";
	}

	private static (int id, bool possible, int power) CalculateStats(Game game)
	{
		var red   = game.Hands.Max(h => h.GetValueOrDefault("red",   0));
		var green = game.Hands.Max(h => h.GetValueOrDefault("green", 0));
		var blue  = game.Hands.Max(h => h.GetValueOrDefault("blue",  0));

		if (red > 12) {
			return (game.Id, false, red * green * blue);
		} else if (green > 13) {
			return (game.Id, false, red * green * blue);
		} else if (blue > 14) {
			return (game.Id, false, red * green * blue);
		}

		return (game.Id, true, red * green * blue);
	}

	private static (int id, bool possible, int power) CalculateStats(GameStruct game)
	{
		var red   = 0;
		var green = 0;
		var blue  = 0;

		if (game.Hand1.Red > red) red = game.Hand1.Red;
		if (game.Hand2.Red > red) red = game.Hand2.Red;
		if (game.Hand3.Red > red) red = game.Hand3.Red;
		if (game.Hand4.Red > red) red = game.Hand4.Red;
		if (game.Hand5.Red > red) red = game.Hand5.Red;
		if (game.Hand6.Red > red) red = game.Hand6.Red;

		if (game.Hand1.Green > green) green = game.Hand1.Green;
		if (game.Hand2.Green > green) green = game.Hand2.Green;
		if (game.Hand3.Green > green) green = game.Hand3.Green;
		if (game.Hand4.Green > green) green = game.Hand4.Green;
		if (game.Hand5.Green > green) green = game.Hand5.Green;
		if (game.Hand6.Green > green) green = game.Hand6.Green;

		if (game.Hand1.Blue > blue) blue = game.Hand1.Blue;
		if (game.Hand2.Blue > blue) blue = game.Hand2.Blue;
		if (game.Hand3.Blue > blue) blue = game.Hand3.Blue;
		if (game.Hand4.Blue > blue) blue = game.Hand4.Blue;
		if (game.Hand5.Blue > blue) blue = game.Hand5.Blue;
		if (game.Hand6.Blue > blue) blue = game.Hand6.Blue;

		if (red > 12) {
			return (game.Id, false, red * green * blue);
		} else if (green > 13) {
			return (game.Id, false, red * green * blue);
		} else if (blue > 14) {
			return (game.Id, false, red * green * blue);
		}

		return (game.Id, true, red * green * blue);
	}

	private static Game ParseBySplit(string line)
	{
		var parts = line.Split(": ");
		var id    = int.Parse(parts[0].Split(' ')[1]);
		var hands = parts[1].Split("; ");

		var dicts = hands.Select(hand => {
			var colors = hand.Split(", ");
			var dict   = new Dictionary<string, int>(colors.Select(color => {
				var cparts = color.Split(' ');
				return KeyValuePair.Create(cparts[1], int.Parse(cparts[0]));
			}));

			return dict;
		});

		return new Game(id, dicts.ToArray());
	}

	private static Game ParseBySpan(string line)
	{
		// Game 1: 3 blue, 2 green, 6 red; 17 green, 4 red, 8 blue; 2 red, 1 green, 10 blue; 1 blue, 5 green
		// we know there are never more than 6 hands (in our input)

		var lineSpan = line.AsSpan();
		var colonIdx = lineSpan.IndexOf(':');
		var gameId   = int.Parse(lineSpan[5..colonIdx]);

		// slice the span to just the hands part
		lineSpan = lineSpan[(colonIdx + 2)..];
		//Console.WriteLine(lineSpan.ToString());

		// rent us an array for the games, and make a span
		var handsArray = ArrayPool<Range>.Shared.Rent(6);
		var handsSpan  = new Span<Range>(handsArray);

		// split the hands into individual entries
		var handCount = lineSpan.Split(handsSpan, ';', StringSplitOptions.TrimEntries);

		// make the array of dictionaries
		var dicts = new Dictionary<string, int>[handCount];

		//Console.WriteLine($"\t{handCount} hands");
		for (var i = 0; i < handCount; i++) {
			var handSpan   = lineSpan[handsArray[i]];
			var cubesArray = ArrayPool<Range>.Shared.Rent(3);
			var cubesSpan  = new Span<Range>(cubesArray);
			var cubesCount = handSpan.Split(cubesSpan, ',', StringSplitOptions.TrimEntries);

			// initialize the dictionary for this hand
			dicts[i] = [];

			//Console.WriteLine($"\t[{handSpan}]");

			for (var j = 0; j < cubesCount; j++) {
				var cubeSpan  = handSpan[cubesArray[j]];
				var spaceIdx  = cubeSpan.IndexOf(' ');
				var thisCount = int.Parse(cubeSpan[..spaceIdx]);
				var thisColor = cubeSpan[(spaceIdx + 1)..].ToString();

				//Console.WriteLine($"\t\t[{thisColor}] [{thisCount}]");

				dicts[i].Add(thisColor, thisCount);
			}

			ArrayPool<Range>.Shared.Return(cubesArray);
		}

		// give back our array, we're done with it
		ArrayPool<Range>.Shared.Return(handsArray);

		return new Game(gameId, dicts);
	}

	private static GameStruct ParseBySpanIntoStruct(string line)
	{
		// Game 1: 3 blue, 2 green, 6 red; 17 green, 4 red, 8 blue; 2 red, 1 green, 10 blue; 1 blue, 5 green
		// we know there are never more than 6 hands (in our input)

		var lineSpan = line.AsSpan();
		var colonIdx = lineSpan.IndexOf(':');
		var gameId   = int.Parse(lineSpan[5..colonIdx]);

		// slice the span to just the hands part
		lineSpan = lineSpan[(colonIdx + 2)..];
		//Console.WriteLine(lineSpan.ToString());

		// rent us an array for the games, and make a span
		var handsArray = ArrayPool<Range>.Shared.Rent(6);
		var handsSpan  = new Span<Range>(handsArray);

		// split the hands into individual entries
		var handCount = lineSpan.Split(handsSpan, ';', StringSplitOptions.TrimEntries);

		// initialize the array of game hands
		var thisGameHands = ArrayPool<GameHand>.Shared.Rent(6);
		thisGameHands[0] = new GameHand(0, 0, 0);
		thisGameHands[1] = new GameHand(0, 0, 0);
		thisGameHands[2] = new GameHand(0, 0, 0);
		thisGameHands[3] = new GameHand(0, 0, 0);
		thisGameHands[4] = new GameHand(0, 0, 0);
		thisGameHands[5] = new GameHand(0, 0, 0);

		//Console.WriteLine($"\t{handCount} hands");
		for (var i = 0; i < handCount; i++) {
			var handSpan   = lineSpan[handsArray[i]];
			var cubesArray = ArrayPool<Range>.Shared.Rent(3);
			var cubesSpan  = new Span<Range>(cubesArray);
			var cubesCount = handSpan.Split(cubesSpan, ',', StringSplitOptions.TrimEntries);
			var red        = 0;
			var green      = 0;
			var blue       = 0;

			//Console.WriteLine($"\t[{handSpan}]");

			for (var j = 0; j < cubesCount; j++) {
				var cubeSpan  = handSpan[cubesArray[j]];
				var spaceIdx  = cubeSpan.IndexOf(' ');
				var thisCount = int.Parse(cubeSpan[..spaceIdx]);
				var thisColor = cubeSpan[(spaceIdx + 1)..];

				//Console.WriteLine($"\t\t[{thisColor}] [{thisCount}]");
				if (thisColor[0] == 'r') {
					red = thisCount;
				} else if (thisColor[0] == 'g') {
					green = thisCount;
				} else if (thisColor[0] == 'b') {
					blue = thisCount;
				}
			}

			ArrayPool<Range>.Shared.Return(cubesArray);

			thisGameHands[i] = new GameHand(red, green, blue);
		}

		// give back our array, we're done with it
		ArrayPool<Range>.Shared.Return(handsArray);

		var game = new GameStruct(gameId, thisGameHands);

		ArrayPool<GameHand>.Shared.Return(thisGameHands);

		return game;
	}

	// [GeneratedRegex(@"Game (?<id>\d+): ((?<num>\d+) (?<color>))+", RegexOptions.ExplicitCapture)]
	// public static partial Regex GetRegex();

	private record Game(int Id, IReadOnlyDictionary<string, int>[] Hands);

	private readonly record struct GameHand(int Red, int Green, int Blue);

	private readonly ref struct GameStruct(int id, GameHand[] hands)
	{
		public readonly int Id         = id;
		public readonly GameHand Hand1 = hands[0];
		public readonly GameHand Hand2 = hands[1];
		public readonly GameHand Hand3 = hands[2];
		public readonly GameHand Hand4 = hands[3];
		public readonly GameHand Hand5 = hands[4];
		public readonly GameHand Hand6 = hands[5];
	}

	public class Benchmarks
	{
		private string[] _lines = [];

		[GlobalSetup]
		public void BenchmarkSetup()
		{
			_lines = ReadFileLines("input.txt");
		}

		[Benchmark]
		public void ParseBySplit()
		{
			foreach (var line in _lines) {
				Problem.ParseBySplit(line);
			}
		}

		[Benchmark]
		public void ParseBySpan()
		{
			foreach (var line in _lines) {
				Problem.ParseBySpan(line);
			}
		}

		[Benchmark]
		public void ParseBySpanIntoStruct()
		{
			foreach (var line in _lines) {
				Problem.ParseBySpanIntoStruct(line);
			}
		}

		[Benchmark]
		public void CalculateStatsClass()
		{
			foreach (var line in _lines) {
				CalculateStats(Problem.ParseBySpan(line));
			}
		}

		[Benchmark]
		public void CalculateStatsStruct()
		{
			foreach (var line in _lines) {
				CalculateStats(Problem.ParseBySpanIntoStruct(line));
			}
		}
	}
}
