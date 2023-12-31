using System.Collections.Specialized;

namespace aoc_2019.Day18;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Main);

	public static (long, long) Main(string[] input)
	{
		//fileName = "inputSample8.txt";

		var map   = CreateMap(input, c => c);
		var paths = CalculatePaths(map, out var entrance);
		var part1 = RecursiveCachingImplementation(entrance, paths);
		var part2 = FindShortestRoute(SplitMap(map));

		//var keys = Enumerable.Range('a', 'l' - 'a' + 1).Select(n => (char)n).ToArray();
		//foreach (var (mpaths, mentrance) in SplitMap(map)) {
		//	Console.WriteLine(mentrance);

		//	foreach (var p in mpaths) {
		//		Console.WriteLine($"  {p.FromTile} -> {p.ToTile} ({p.Length}) {new string(keys.Select(k => p.NeededKeys[k] ? k : '.').ToArray())}");
		//	}
		//}

		return (part1, part2);
	}

	private static int SingleThreadedStackImplementation(Coordinate entrance, List<Path> paths)
	{
		var part1     = int.MaxValue;
		var pathsFrom = paths.DistinctBy(p => p.From).ToDictionary(p => p.From, p => paths.Where(ip => ip.From == p.From).ToList());
		var allKeySet = new KeySet(paths.Select(p => p.FromTile).Where(Tile.AllKeys.Contains).Distinct());
		var states    = new Stack<State>(new[] {
			new State(entrance, new KeySet(), 0)
		});

		while (states.Count > 0) {
			var currentState = states.Pop();

			foreach (var thisPath in pathsFrom[currentState.CurrentPosition]) {
				if (currentState.HaveKeys[thisPath.ToTile]) {
					continue;
				}

				var soughtKey   = thisPath.ToTile;
				var keyLocation = thisPath.To;

				if (!currentState.HaveKeys.Satisfies(thisPath.NeededKeys)) {
					// this path is unavailable to us; we need at least one key we don't have
					continue;
				}

				var newDistance = currentState.Distance + thisPath.Length;
				var newKeySet   = new KeySet(currentState.HaveKeys, soughtKey);

				// if this new state has all the keys, check the length
				if (newKeySet == allKeySet) {
					var oldmin = part1;
					part1 = Math.Min(part1, newDistance);
					if (oldmin != part1) {
						Console.WriteLine($"New minimum completed in {newDistance} steps ({states.Count} in stack).");
					}
				}

				// we're already too far down this path... no good following it further.
				if (newDistance > part1) {
					continue;
				}

				states.Push(new State(thisPath.To, newKeySet, newDistance));
			}
		}

		return part1;
	}

	private static int RecursiveCachingImplementation(Coordinate entrance, List<Path> paths)
	{
		var allKeySet   = new KeySet(paths.Select(p => p.FromTile).Where(Tile.AllKeys.Contains).Distinct());
		var emptyKeySet = new KeySet();
		var pathsFrom   = paths.DistinctBy(p => p.From).ToDictionary(p => p.From, p => paths.Where(ip => ip.From == p.From).ToList());
		var pathCache   = new Dictionary<(Coordinate currentLocation, KeySet haveKeys), int>();

		return DistanceForRemainingKeys(entrance, emptyKeySet);

		int DistanceForRemainingKeys(Coordinate currentLocation, KeySet haveKeys)
		{
			if (haveKeys == allKeySet) {
				return 0;
			}

			var cacheKey = (currentLocation, haveKeys);

			// if we've already derived a best-case path from this point forward, use it
			if (pathCache.TryGetValue(cacheKey, out var distance)) {
				return distance;
			} else {
				distance = int.MaxValue;
			}

			// otherwise, recursively explore all paths from this state forward
			foreach (var path in pathsFrom[currentLocation]) {
				// if the keys we have aren't enough to travel this path, skip it
				if (!haveKeys.Satisfies(path.NeededKeys)) {
					continue;
				}

				// if we already have the key at the end of this path, skip it
				if (haveKeys[path.ToTile]) {
					continue;
				}

				// the distance for everything else from here, starting at this path
				var thisPathDistance = path.Length + DistanceForRemainingKeys(path.To, new KeySet(haveKeys, path.ToTile));

				// if that's the best we've seen, save it
				distance = Math.Min(distance, thisPathDistance);
			}

			// we did all this hard work, save the result in case we come back to this situation
			pathCache.Add(cacheKey, distance);

			return distance;
		}
	}

	private static int FindShortestRoute((List<Path> paths, Coordinate entrance)[] maps)
	{
		var allKeys     = maps.SelectMany(m => m.paths.Select(p => p.ToTile)).ToHashSet();
		var allKeySet   = new KeySet(allKeys);
		var emptyKeySet = new KeySet();
		var stateCache  = new Dictionary<(Coordinate loc1, Coordinate loc2, Coordinate loc3, Coordinate loc4, KeySet haveKeys), int>();

		return DistanceForRemainingKeys(maps[0].entrance, maps[1].entrance, maps[2].entrance, maps[3].entrance, emptyKeySet);

		int DistanceForRemainingKeys(Coordinate loc1, Coordinate loc2, Coordinate loc3, Coordinate loc4, KeySet haveKeys)
		{
			if (haveKeys == allKeySet) {
				return 0;
			}

			var cacheKey = (loc1, loc2, loc3, loc4, haveKeys);

			// if we've already derived a best-case path from this point forward, use it
			if (stateCache.TryGetValue(cacheKey, out var distance)) {
				return distance;
			} else {
				distance = int.MaxValue;
			}

			foreach (var path in maps[0].paths.Where(p => !haveKeys[p.ToTile] && haveKeys.Satisfies(p.NeededKeys) && p.From == loc1)) {
				// the distance for everything else from here, starting at this path
				var thisPathDistance = path.Length + DistanceForRemainingKeys(path.To, loc2, loc3, loc4, new KeySet(haveKeys, path.ToTile));

				// if that's the best we've seen, save it
				distance = Math.Min(distance, thisPathDistance);
			}

			foreach (var path in maps[1].paths.Where(p => !haveKeys[p.ToTile] && haveKeys.Satisfies(p.NeededKeys) && p.From == loc2)) {
				// the distance for everything else from here, starting at this path
				var thisPathDistance = path.Length + DistanceForRemainingKeys(loc1, path.To, loc3, loc4, new KeySet(haveKeys, path.ToTile));

				// if that's the best we've seen, save it
				distance = Math.Min(distance, thisPathDistance);
			}

			foreach (var path in maps[2].paths.Where(p => !haveKeys[p.ToTile] && haveKeys.Satisfies(p.NeededKeys) && p.From == loc3)) {
				// the distance for everything else from here, starting at this path
				var thisPathDistance = path.Length + DistanceForRemainingKeys(loc1, loc2, path.To, loc4, new KeySet(haveKeys, path.ToTile));

				// if that's the best we've seen, save it
				distance = Math.Min(distance, thisPathDistance);
			}

			foreach (var path in maps[3].paths.Where(p => !haveKeys[p.ToTile] && haveKeys.Satisfies(p.NeededKeys) && p.From == loc4)) {
				// the distance for everything else from here, starting at this path
				var thisPathDistance = path.Length + DistanceForRemainingKeys(loc1, loc2, loc3, path.To, new KeySet(haveKeys, path.ToTile));

				// if that's the best we've seen, save it
				distance = Math.Min(distance, thisPathDistance);
			}

			// we did all this hard work, save the result in case we come back to this situation
			stateCache.Add(cacheKey, distance);

			return distance;
		}
	}

	private static List<Path> CalculatePaths(char[,] map, out Coordinate entrance)
	{
		var width  = map.GetLength(0);
		var height = map.GetLength(1);

		entrance = new Coordinate(-1, -1);

		for (var x = 0; x < width; x++) {
			for (var y = 0; y < height; y++) {
				if (map[x, y] == Tile.Entrance) {
					entrance = new Coordinate(x, y);
					return CalculatePaths(map, entrance);
				}
			}
		}

		throw new InvalidDataException("Could not locate entrance in map");
	}

	private static List<Path> CalculatePaths(char[,] map, Coordinate entrance)
	{
		var ret     = new List<Path>();
		var width   = map.GetLength(0);
		var height  = map.GetLength(1);
		var mapKeys = new List<KeyValuePair<char, Coordinate>>();
		var moves   = new[] {
			new Coordinate(+1,  0),
			new Coordinate(-1,  0),
			new Coordinate( 0, +1),
			new Coordinate( 0, -1),
		};

		// search the map for all the keys
		for (var x = 0; x < width; x++) {
			for (var y = 0; y < height; y++) {
				if (Tile.AllKeys.Contains(map[x, y])) {
					mapKeys.Add(KeyValuePair.Create(map[x, y], new Coordinate(x, y)));
				}
			}
		}

		// make sure we found the entrance
		if (entrance.X == -1 || entrance.Y == -1) {
			throw new InvalidDataException("The map contains no entrance.");
		}

		// from the starting point to each key
		foreach (var kvp in mapKeys) {
			var path = FindPath(entrance, kvp.Value);

			if (path != null) {
				ret.Add(path.Value);
			}
		}

		// then from each key to each other key
		foreach (var kvp1 in mapKeys) {
			foreach (var kvp2 in mapKeys) {
				if (kvp1.Key != kvp2.Key) {
					var path = FindPath(kvp1.Value, kvp2.Value);

					if (path != null) {
						ret.Add(path.Value);
					}
				}
			}
		}

		return ret;

		IEnumerable<(Coordinate, float)> AdjacentRooms(Coordinate room) => moves.Select(m => (room + m, 1f)).Where(c => Tile.AllWalkable.Contains(map[c.Item1.X, c.Item1.Y]));

		Path? FindPath(Coordinate from, Coordinate to)
		{
			var steps = AStar.FindPath(from, to, AdjacentRooms, (f, t) => Coordinate.ManhattanDistance(f, t));

			if (steps == null) {
				return null;
			}

			var neededKeys = new KeySet(steps.Select(c => map[c.X, c.Y]).Intersect(Tile.AllDoors).Select(door => Tile.DoorKeys[door]));

			return new Path(from, to, map[from.X, from.Y], map[to.X, to.Y], steps.Count - 1, neededKeys);
		}
	}

	private static (List<Path> paths, Coordinate entrance)[] SplitMap(char[,] originalMap)
	{
		// clone the map so we don't modify the original (for whatever reason)
		// Raymond always says not to change the carpet in a room you're borrowing, or whatever...
		if (originalMap.Clone() is not char[,] map) {
			throw new InvalidOperationException("Didn't clone the map successfully");
		}

		var width  = map.GetLength(0);
		var height = map.GetLength(1);
		var splitX = -1;
		var splitY = -1;

		for (var x = 0; x < width && splitX == -1; x++) {
			for (var y = 0; y < height && splitY == -1; y++) {
				if (map[x, y] == Tile.Entrance) {
					splitX = x;
					splitY = y;
				}
			}
		}

		map[splitX - 1, splitY]     = Tile.Wall;
		map[splitX,     splitY]     = Tile.Wall;
		map[splitX + 1, splitY]     = Tile.Wall;
		map[splitX,     splitY - 1] = Tile.Wall;
		map[splitX,     splitY + 1] = Tile.Wall;
		map[splitX - 1, splitY - 1] = Tile.Entrance;
		map[splitX + 1, splitY - 1] = Tile.Entrance;
		map[splitX - 1, splitY + 1] = Tile.Entrance;
		map[splitX + 1, splitY + 1] = Tile.Entrance;

		var r1Entrance = new Coordinate(splitX - 1, splitY - 1);
		var r2Entrance = new Coordinate(splitX + 1, splitY - 1);
		var r3Entrance = new Coordinate(splitX - 1, splitY + 1);
		var r4Entrance = new Coordinate(splitX + 1, splitY + 1);
		var r1Paths    = CalculatePaths(map, r1Entrance);
		var r2Paths    = CalculatePaths(map, r2Entrance);
		var r3Paths    = CalculatePaths(map, r3Entrance);
		var r4Paths    = CalculatePaths(map, r4Entrance);
		var r1Keys     = r1Paths.Where(p => p.FromTile == Tile.Entrance).Select(p => p.ToTile).ToHashSet();
		var r2Keys     = r2Paths.Where(p => p.FromTile == Tile.Entrance).Select(p => p.ToTile).ToHashSet();
		var r3Keys     = r3Paths.Where(p => p.FromTile == Tile.Entrance).Select(p => p.ToTile).ToHashSet();
		var r4Keys     = r4Paths.Where(p => p.FromTile == Tile.Entrance).Select(p => p.ToTile).ToHashSet();

		r1Paths.RemoveAll(p => p.FromTile != Tile.Entrance && (!r1Keys.Contains(p.FromTile) || !r1Keys.Contains(p.ToTile)));
		r2Paths.RemoveAll(p => p.FromTile != Tile.Entrance && (!r2Keys.Contains(p.FromTile) || !r2Keys.Contains(p.ToTile)));
		r3Paths.RemoveAll(p => p.FromTile != Tile.Entrance && (!r3Keys.Contains(p.FromTile) || !r3Keys.Contains(p.ToTile)));
		r4Paths.RemoveAll(p => p.FromTile != Tile.Entrance && (!r4Keys.Contains(p.FromTile) || !r4Keys.Contains(p.ToTile)));

		return new[] {
			(r1Paths, r1Entrance),
			(r2Paths, r2Entrance),
			(r3Paths, r3Entrance),
			(r4Paths, r4Entrance),
		};
	}

	private readonly record struct State(Coordinate CurrentPosition, KeySet HaveKeys, int Distance);

	private readonly record struct Path(Coordinate From, Coordinate To, char FromTile, char ToTile, int Length, KeySet NeededKeys);

	private record struct KeySet()
	{
		private readonly static int[] _masks;

		private BitVector32 _vector = new();

		static KeySet()
		{
			_masks = new int['z' - 'a' + 1];

			for (var c = 'a'; c <= 'z'; c++) {
				_masks[c - 'a'] = BitVector32.CreateMask(c == 'a' ? 0 : _masks[c - 'a' - 1]);
			}
		}

		public KeySet(IEnumerable<char> keys) : this()
		{
			foreach (var c in keys) {
				Add(c);
			}
		}

		public KeySet(KeySet baseKeys, char addKey) : this()
		{
			_vector = new BitVector32(baseKeys._vector);

			Add(addKey);
		}

		public void Add(char key) => this[key] = true;

		public bool this[char c]
		{
			get => _vector[_masks[c - 'a']];
			set => _vector[_masks[c - 'a']] = value;
		}

		public bool Satisfies(KeySet requiredKeys) => (_vector.Data & requiredKeys._vector.Data) == requiredKeys._vector.Data;

		public bool ContainsAny(KeySet otherKeys) => (_vector.Data & otherKeys._vector.Data) > 0;

		public int Differences(KeySet other)
		{
			var ret = 0;

			for (var i = 0; i < _masks.Length; i++) {
				if (_vector[_masks[i]] != other._vector[_masks[i]]) {
					ret++;
				}
			}

			return ret;
		}

		public KeySet Without(char key)
		{
			var ret = new KeySet();

			ret._vector = new BitVector32(_vector.Data);

			ret[key] = false;

			return ret;
		}
	}

	private static class Tile
	{
		public const char Entrance = '@';
		public const char Wall     = '#';
		public const char Floor    = '.';

		public const char KeyA = 'a'; public const char DoorA = 'A';
		public const char KeyB = 'b'; public const char DoorB = 'B';
		public const char KeyC = 'c'; public const char DoorC = 'C';
		public const char KeyD = 'd'; public const char DoorD = 'D';
		public const char KeyE = 'e'; public const char DoorE = 'E';
		public const char KeyF = 'f'; public const char DoorF = 'F';
		public const char KeyG = 'g'; public const char DoorG = 'G';
		public const char KeyH = 'h'; public const char DoorH = 'H';
		public const char KeyI = 'i'; public const char DoorI = 'I';
		public const char KeyJ = 'j'; public const char DoorJ = 'J';
		public const char KeyK = 'k'; public const char DoorK = 'K';
		public const char KeyL = 'l'; public const char DoorL = 'L';
		public const char KeyM = 'm'; public const char DoorM = 'M';
		public const char KeyN = 'n'; public const char DoorN = 'N';
		public const char KeyO = 'o'; public const char DoorO = 'O';
		public const char KeyP = 'p'; public const char DoorP = 'P';
		public const char KeyQ = 'q'; public const char DoorQ = 'Q';
		public const char KeyR = 'r'; public const char DoorR = 'R';
		public const char KeyS = 's'; public const char DoorS = 'S';
		public const char KeyT = 't'; public const char DoorT = 'T';
		public const char KeyU = 'u'; public const char DoorU = 'U';
		public const char KeyV = 'v'; public const char DoorV = 'V';
		public const char KeyW = 'w'; public const char DoorW = 'W';
		public const char KeyX = 'x'; public const char DoorX = 'X';
		public const char KeyY = 'y'; public const char DoorY = 'Y';
		public const char KeyZ = 'z'; public const char DoorZ = 'Z';

		public static readonly IReadOnlySet<char> AllKeys     = new HashSet<char>(new[] { KeyA, KeyB, KeyC, KeyD, KeyE, KeyF, KeyG, KeyH, KeyI, KeyJ, KeyK, KeyL, KeyM, KeyN, KeyO, KeyP, KeyQ, KeyR, KeyS, KeyT, KeyU, KeyV, KeyW, KeyX, KeyY, KeyZ });
		public static readonly IReadOnlySet<char> AllDoors    = new HashSet<char>(new[] { DoorA, DoorB, DoorC, DoorD, DoorE, DoorF, DoorG, DoorH, DoorI, DoorJ, DoorK, DoorL, DoorM, DoorN, DoorO, DoorP, DoorQ, DoorR, DoorS, DoorT, DoorU, DoorV, DoorW, DoorX, DoorY, DoorZ });
		public static readonly IReadOnlySet<char> AllWalkable = new HashSet<char>(AllKeys.Concat(AllDoors).Append(Entrance).Append(Floor));
		public static readonly IReadOnlyDictionary<char, char> DoorKeys = new Dictionary<char, char>() {
			{ DoorA, KeyA }, { DoorB, KeyB }, { DoorC, KeyC }, { DoorD, KeyD }, { DoorE, KeyE }, { DoorF, KeyF }, { DoorG, KeyG }, { DoorH, KeyH },
			{ DoorI, KeyI }, { DoorJ, KeyJ }, { DoorK, KeyK }, { DoorL, KeyL }, { DoorM, KeyM }, { DoorN, KeyN }, { DoorO, KeyO }, { DoorP, KeyP },
			{ DoorQ, KeyQ }, { DoorR, KeyR }, { DoorS, KeyS }, { DoorT, KeyT }, { DoorU, KeyU }, { DoorV, KeyV }, { DoorW, KeyW }, { DoorX, KeyX },
			{ DoorY, KeyY }, { DoorZ, KeyZ } };
	}

	[Fact]
	public void KeySetWorksCorrectly()
	{
		for (var c = 'a'; c <= 'z'; c++) {
			var ks = new KeySet();

			ks[c] = true;

			AssertAllExcept(ks, c, false);
		}

		void AssertAllExcept(KeySet set, char except, bool values)
		{
			for (var lc = 'a'; lc <= 'z'; lc++) {
				if (lc == except) {
					Assert.Equal(!values, set[lc]);
				} else {
					Assert.Equal(values, set[lc]);
				}
			}
		}
	}

	[Fact]
	public void KeySetSatisfiesCorrectly()
	{
		var haveKeys = new KeySet("abcdefgh");
		var needKeys = new KeySet("aceg");

		Assert.True(haveKeys.Satisfies(needKeys));
	}

	[Theory]
	[InlineData("inputSample6.txt", 26, 8)]
	[InlineData("inputSample7.txt", 50, 24)]
	[InlineData("inputSample8.txt", 127, 32)]
	[InlineData("inputSample9.txt", 114, 72)]
	public void SampleMapsWorkCorrectly(string fileName, int expectedPart1, int expectedPart2)
	{
		var (part1, part2) = Main(ReadFileLines(fileName));

		Assert.Equal(expectedPart1, part1);
		Assert.Equal(expectedPart2, part2);
	}
}
