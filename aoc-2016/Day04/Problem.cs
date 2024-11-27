namespace AdventOfCode.Year2016.Day04;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Main);

	public static (long, long) Main(string[] input)
	{
		//input.Select(Parse)
		//Parse(input[0]);
		// Console.WriteLine($"aaaaa-bbb-z-y-x-123[abxyz] {IsReal(Parse("aaaaa-bbb-z-y-x-123[abxyz]"))}");
		// Console.WriteLine($"a-b-c-d-e-f-g-h-987[abcde] {IsReal(Parse("a-b-c-d-e-f-g-h-987[abcde]"))}");
		// Console.WriteLine($"not-a-real-room-404[oarel] {IsReal(Parse("not-a-real-room-404[oarel]"))}");
		// Console.WriteLine($"totally-real-room-200[decoy] {IsReal(Parse("totally-real-room-200[decoy]"))}");
		var real = input
			.Select(Parse)
			.Where(IsReal)
			.ToList();

		var part1 = real.Sum(r => r.SectorId);
		var part2 = real
			.Select(r => (Room: r, Name: new string(r.EncryptedName.Select(c => Decrypt(c, r.SectorId)).ToArray())))
			.Where(t => t.Name.Contains("northpole"))
			.Single()
			.Room.SectorId;

		// foreach (var r in storage) {
		// 	Console.WriteLine($"{r.Name} {r.Room.SectorId}");
		// }

		return (part1, part2);
	}

	private static char Decrypt(char c, int sectorId) => c switch {
		'-' => ' ',
		>= 'a' and <= 'z' => (char)(((c - 'a' + sectorId) % 26) + 'a'),
		>= 'A' and <= 'Z' => (char)(((c - 'A' + sectorId) % 26) + 'A'),
		_ => c,
	};

	private static Room Parse(string input)
	{
		var span     = input.AsSpan();
		var lastDash = span.LastIndexOf('-');
		var sectorId = int.Parse(span[(lastDash + 1)..span.IndexOf('[')]);
		var checksum = span[(span.IndexOf('[') + 1)..^1].ToArray();
		var chars    = new Dictionary<char, int>();

		foreach (var c in span[..lastDash]) {
			if (c == '-') {
				continue;
			}

			chars[c] = chars.GetValueOrDefault(c, 0) + 1;
		}

		return new(chars, sectorId, checksum, new string(span[..lastDash]));
	}

	private static bool IsReal(Room room) =>
		room.Checksum.SequenceEqual(room.Counts.OrderByDescending(kvp => kvp.Value).ThenBy(kvp => kvp.Key).Select(kvp => kvp.Key).Take(5));

	internal readonly record struct Room(Dictionary<char, int> Counts, int SectorId, char[] Checksum, string EncryptedName);
}
