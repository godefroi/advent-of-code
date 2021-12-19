namespace Day19;

using System.Linq;

using Xunit;

using Transform = Func<(int x, int y, int z), (int x, int y, int z)>;

public class Problem
{
	//internal static Transform[] _transforms = new Transform[] {
	//	// facing same: x -> +x, y -> +y, z -> +z
	//	// rotated 90:  x -> +y, y -> -x, z -> +z
	//	// rotated 180: x -> -x, y -> -y, z -> +z
	//	// rotated 270: x -> -y, y -> +x, z -> +z
	//	((int x, int y, int z) inp) => ( inp.x,  inp.y,  inp.z),
	//	((int x, int y, int z) inp) => ( inp.y, -inp.x,  inp.z),
	//	((int x, int y, int z) inp) => (-inp.x, -inp.y,  inp.z),
	//	((int x, int y, int z) inp) => (-inp.y,  inp.x,  inp.z),

	//	// but upside down:
	//	// facing same: x -> +x, y -> -y, z -> -z
	//	// rotated 90:  x -> +y, y -> +x, z -> -z
	//	// rotated 180: x -> -x, y -> +y, z -> -z
	//	// rotated 270: x -> -y, y -> -x, z -> -z
	//	((int x, int y, int z) inp) => ( inp.x, -inp.y, -inp.z),
	//	((int x, int y, int z) inp) => ( inp.y,  inp.x, -inp.z),
	//	((int x, int y, int z) inp) => (-inp.x,  inp.y, -inp.z),
	//	((int x, int y, int z) inp) => (-inp.y, -inp.x, -inp.z),

	//	// facing same: x -> -x, y -> +y, z -> -z
	//	// rotated 90:  x -> -y, y -> -x, z -> -z
	//	// rotated 180: x -> +x, y -> -y, z -> -z
	//	// rotated 270: x -> +y, y -> +x, z -> -z
	//	((int x, int y, int z) inp) => (-inp.x,  inp.y, -inp.z),
	//	((int x, int y, int z) inp) => (-inp.y, -inp.x, -inp.z),
	//	((int x, int y, int z) inp) => ( inp.x, -inp.y, -inp.z),
	//	((int x, int y, int z) inp) => ( inp.y,  inp.x, -inp.z),

	//	// facing same: x -> -x, y -> -y, z -> +z
	//	// rotated 90:  x -> -y, y -> +x, z -> +z
	//	// rotated 180: x -> +x, y -> +y, z -> +z
	//	// rotated 270: x -> +y, y -> -x, z -> +z
	//	((int x, int y, int z) inp) => (-inp.x, -inp.y,  inp.z),
	//	((int x, int y, int z) inp) => (-inp.y,  inp.x,  inp.z),
	//	((int x, int y, int z) inp) => ( inp.x,  inp.y,  inp.z),
	//	((int x, int y, int z) inp) => ( inp.y, -inp.x,  inp.z),

	//	// facing up:
	//	// facing same: x -> +z, y -> +y, z -> -x
	//	// rotated 90:  x -> +z, y -> -x, z -> -y
	//	// rotated 180: x -> +z, y -> -y, z -> +x
	//	// rotated 270: x -> +z, y -> +x, z -> +y
	//	((int x, int y, int z) inp) => ( inp.z,  inp.y, -inp.x),
	//	((int x, int y, int z) inp) => ( inp.z, -inp.x, -inp.y),
	//	((int x, int y, int z) inp) => ( inp.z, -inp.y,  inp.x),
	//	((int x, int y, int z) inp) => ( inp.z,  inp.x,  inp.y),

	//	// facing down:
	//	// facing same: x -> -z, y -> +y, z -> +x
	//	// rotated 90:  x -> -z, y -> -x, z -> +y
	//	// rotated 180: x -> -z, y -> -y, z -> -x
	//	// rotated 270: x -> -z, y -> +x, z -> -y
	//	((int x, int y, int z) inp) => (-inp.z,  inp.y,  inp.x),
	//	((int x, int y, int z) inp) => (-inp.z, -inp.x,  inp.y),
	//	((int x, int y, int z) inp) => (-inp.z, -inp.y, -inp.x),
	//	((int x, int y, int z) inp) => (-inp.z,  inp.x, -inp.y),
	//};

	private const int REQUIRED_MATCHES = 12;
	private const int COORDINATE_SEARCH_SPACE = 5000;

	internal static Transform[] _transforms = new Transform[] {
		((int x, int y, int z) inp) => ( inp.x,  inp.y,  inp.z),
		((int x, int y, int z) inp) => ( inp.x, -inp.z,  inp.y),
		((int x, int y, int z) inp) => ( inp.x, -inp.y, -inp.z),
		((int x, int y, int z) inp) => ( inp.x,  inp.z, -inp.y),

		((int x, int y, int z) inp) => (-inp.x, -inp.y,  inp.z),
		((int x, int y, int z) inp) => (-inp.x, -inp.z, -inp.y),
		((int x, int y, int z) inp) => (-inp.x,  inp.y, -inp.z),
		((int x, int y, int z) inp) => (-inp.x,  inp.z,  inp.y),

		((int x, int y, int z) inp) => (-inp.z,  inp.x, -inp.y),
		((int x, int y, int z) inp) => ( inp.y,  inp.x, -inp.z),
		((int x, int y, int z) inp) => ( inp.z,  inp.x,  inp.y),
		((int x, int y, int z) inp) => (-inp.y,  inp.x,  inp.z),

		((int x, int y, int z) inp) => ( inp.z, -inp.x, -inp.y),
		((int x, int y, int z) inp) => ( inp.y, -inp.x,  inp.z),
		((int x, int y, int z) inp) => (-inp.z, -inp.x,  inp.y),
		((int x, int y, int z) inp) => (-inp.y, -inp.x, -inp.z),

		((int x, int y, int z) inp) => (-inp.y, -inp.z,  inp.x),
		((int x, int y, int z) inp) => ( inp.z, -inp.y,  inp.x),
		((int x, int y, int z) inp) => ( inp.y,  inp.z,  inp.x),
		((int x, int y, int z) inp) => (-inp.z,  inp.y,  inp.x),

		((int x, int y, int z) inp) => ( inp.z,  inp.y, -inp.x),
		((int x, int y, int z) inp) => (-inp.y,  inp.z, -inp.x),
		((int x, int y, int z) inp) => (-inp.z, -inp.y, -inp.x),
		((int x, int y, int z) inp) => ( inp.y, -inp.z, -inp.x),
	};


	internal static void Main(string fileName)
	{
		var input = ParseInput(File.ReadAllLines(fileName));

		//var input = ParseInput(File.ReadAllLines(GetFilePath("input_sample.txt")));
		var destset = input[0].Beacons.ToHashSet();
		var to_map  = input.Skip(1).ToList();
		var offset_map = new Dictionary<Scanner, (int x, int y, int z)>();

		while (to_map.Count > 0) {
			for (var i = 0; i < to_map.Count; i++) {
				var result = MapBeacons2(destset, to_map[i]);

				if (result != null) {
					offset_map.Add(to_map[i], result.Value.offset);

					foreach (var p in result.Value.beacons) {
						destset.Add(p);
					}

					to_map.RemoveAt(i);

					Console.WriteLine($"Found matches for {i}, total set now contains {destset.Count}, {to_map.Count} left to map");
					break;
				}
			}
		}

		var max = 0;

		foreach (var offset1 in offset_map.Values) {
			foreach (var offset2 in offset_map.Values) {
				var dist = Math.Abs(offset1.x - offset2.x) + Math.Abs(offset1.y - offset2.y) + Math.Abs(offset1.z - offset2.z);

				max = Math.Max(max, dist);
			}
		}

		Console.WriteLine($"part 2: {max}"); // part 2 is 11906
	}

	private static IEnumerable<(int xOffset, int yOffset, int zOffset)> PossibleOffsets(IEnumerable<(int x, int y, int z)> first, IEnumerable<(int x, int y, int z)> second)
	{
		foreach (var p1 in first) {
			foreach (var p2 in second) {
				yield return (p1.x - p2.x, p1.y - p2.y, p1.z - p2.z);
				yield return (p2.x - p1.x, p2.y - p1.y, p2.z - p1.z);
			}
		}
	}

	private static List<(int x, int y, int z)>? MapBeacons1(IEnumerable<(int x, int y, int z)> from, Scanner to)
	{
		var targ_x_coords = from.Select(b => b.x).ToList();
		var targ_y_coords = from.Select(b => b.y).ToList();
		var targ_z_coords = from.Select(b => b.z).ToList();
		var xmin          = targ_x_coords.Min() - 1000;
		var ymin          = targ_y_coords.Min() - 1000;
		var zmin          = targ_z_coords.Min() - 1000;
		var xmax          = targ_x_coords.Max() + 1000;
		var ymax          = targ_y_coords.Max() + 1000;
		var zmax          = targ_z_coords.Max() + 1000;

		foreach (var t in _transforms) {
			var tidx = Array.IndexOf(_transforms, t);

			// transform all the points based on this transform
			var cpoints = to.Beacons.Select(b => t(b)).ToList();

			// then, search through offsets
			for (var x = xmin; x <= xmax; x++) {
				var offset_x = cpoints.Select(p => (x: p.x + x, p.y, p.z)).ToList();
				var x_coords = offset_x.Select(p => p.x).ToList();

				var xmatches = x_coords.Intersect(targ_x_coords).Count();

				if (xmatches < REQUIRED_MATCHES) {
					continue;
				}

				//Console.WriteLine($"transform {tidx} offset {x} x-matches {xmatches}");

				for (var y = ymin; y <= ymax; y++) {
					var offset_xy = offset_x.Select(p => (p.x, y: p.y + y, p.z)).ToList();
					var y_coords  = offset_xy.Select(p => p.y).ToList();

					var ymatches = y_coords.Intersect(targ_y_coords).Count();

					if (ymatches < REQUIRED_MATCHES) {
						continue;
					}

					//Console.WriteLine($"  transform {tidx} offset {y} y-matches {ymatches}");

					for (var z = zmin; z <= zmax; z++) {
						var offset_xyz = offset_xy.Select(p => (p.x, p.y, z: p.z + z)).ToList();
						var z_coords   = offset_xyz.Select(p => p.z).ToList();

						var zmatches = z_coords.Intersect(targ_z_coords).Count();

						if (zmatches < REQUIRED_MATCHES) {
							continue;
						}

						//Console.WriteLine($"    transform {tidx} offset {z} z-matches {zmatches}");
						Console.WriteLine($"    *** Mapping scanner {to.Id} using transform {tidx} offset ({x},{y},{z})");

						return offset_xyz;
					}
				}
			}
		}

		return null;
	}

	private static ((int x, int y, int z) offset, List<(int x, int y, int z)> beacons)? MapBeacons2(IEnumerable<(int x, int y, int z)> from, Scanner to)
	{
		//***Mapping scanner 5 using transform 2 offset(48, 24, 1236)

		foreach (var t in _transforms) {
			//Console.WriteLine($"trying transform {Array.IndexOf(_transforms, t)}");
			var transformed = to.Beacons.Select(b => t(b)).ToList();

			//if (to.Id == 5 && Array.IndexOf(_transforms, t) == 2) {
			//	var offsets = PossibleOffsets(from, transformed).ToList();

			//	Console.WriteLine(offsets.Any(o => o.xOffset == 48 && o.yOffset == 24 && o.zOffset == 1236));
			//}

			foreach (var offset in PossibleOffsets(from, transformed)) {
				var transformed_offset = transformed.Select(b => (x: b.x + offset.xOffset, y: b.y + offset.yOffset, z: b.z + offset.zOffset)).ToList();

				if (transformed_offset.Intersect(from).Count() >= REQUIRED_MATCHES) {
					Console.WriteLine($"    *** Mapping scanner {to.Id} using transform {Array.IndexOf(_transforms, t)} offset ({offset.xOffset},{offset.yOffset},{offset.zOffset})");
					return (offset, transformed_offset);
				}
			}
		}

		return null;
	}

	private static string GetFilePath(string fileName, [System.Runtime.CompilerServices.CallerFilePath]string sourceFilepPath = "") => Path.Combine(Path.GetDirectoryName(sourceFilepPath)!, fileName);

	private static List<Scanner> ParseInput(string[] input)
	{
		var ret     = new List<Scanner>();
		var beacons = new List<(int x, int y, int z)>();
		var id      = -1;

		for (var i = 0; i < input.Length; i++) {
			if (input[i] == string.Empty) {
				continue;
			}

			if (input[i].StartsWith("---")) {
				if (id > -1) {
					ret.Add(new Scanner(id, beacons));
					beacons = new List<(int x, int y, int z)>();
				}

				id = int.Parse(input[i].Split(' ')[2]);

				continue;
			}

			var parts = input[i].Split(',');

			beacons.Add((int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2])));
		}

		ret.Add(new Scanner(id, beacons));

		return ret;
	}

	[Fact]
	public void TransformTestsExecuteCorrectly()
	{
		static string StringRep(IEnumerable<(int x, int y, int z)> items) => string.Join(';', items.Select(b => b.ToString()).OrderBy(s => s));

		var input = ParseInput(File.ReadAllLines(GetFilePath("transform_tests.txt")));

		Assert.Equal(5, input.Count);
		Assert.All(input, i => Assert.Equal(0, i.Id));

		var reference = StringRep(input[0].Beacons);

		for (var i = 1; i < input.Count; i++) {
#pragma warning disable xUnit2012 // Do not use Enumerable.Any() to check if a value exists in a collection
			Assert.True(_transforms.Any(t => StringRep(input[i].Beacons.Select(b => t(b))) == reference));
#pragma warning restore xUnit2012 // Do not use Enumerable.Any() to check if a value exists in a collection
		}
	}

	private record struct Scanner(int Id, IEnumerable<(int x, int y, int z)> Beacons);
}
