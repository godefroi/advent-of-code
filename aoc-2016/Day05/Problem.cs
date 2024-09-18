using System.Security.Cryptography;
using System.Text;

namespace aoc_2016.Day05;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Main);

	public static (string, string) Main(string[] input)
	{
		var part1 = new List<char>(8);

		FindHashes(input[0], s => {
			part1.Add(s[5]);
			return part1.Count < part1.Capacity;
		});

		var part2 = new char[8];
		var p2pos = new bool[8];

		FindHashes(input[0], s => {
			var pos = s[5] switch {
				'0' when !p2pos[0] => 0,
				'1' when !p2pos[1] => 1,
				'2' when !p2pos[2] => 2,
				'3' when !p2pos[3] => 3,
				'4' when !p2pos[4] => 4,
				'5' when !p2pos[5] => 5,
				'6' when !p2pos[6] => 6,
				'7' when !p2pos[7] => 7,
				_ => -1,
			};

			if (pos == -1) {
				return true;
			}

			part2[pos] = s[6];
			p2pos[pos] = true;

			return p2pos.Any(b => !b);
		});

		return (new string(part1.ToArray()), new string(part2));
	}

	private static void FindHashes(string doorId, Func<string, bool> callback)
	{
		var hashBuffer = new byte[MD5.HashSizeInBytes];
		var hashSpan   = hashBuffer.AsSpan();

		foreach (var d in GenerateValues(doorId)) {
			MD5.HashData(d, hashSpan);

			if (hashBuffer[0] == 0 && hashBuffer[1] == 0 && hashBuffer[2] < 16) {
				if (!callback(Convert.ToHexString(hashBuffer).ToLower())) {
					return;
				}
			}
		}

		throw new NotSupportedException("GenerateValues() must not terminate.");
	}

	private static IEnumerable<byte[]> GenerateValues(string doorId)
	{
		var curBuf   = new byte[Encoding.ASCII.GetByteCount(doorId) + Encoding.ASCII.GetByteCount("0")];
		var incValue = 0;

		Encoding.ASCII.GetBytes(doorId, 0, doorId.Length, curBuf, 0);
		Encoding.ASCII.GetBytes("0", 0, 1, curBuf, doorId.Length);

		while (true) {
			// process
			yield return curBuf;

			// increment
			var ivs  = incValue++.ToString();
			var ivbc = Encoding.ASCII.GetByteCount(ivs);

			// make our buffer bigger if we need to
			if (curBuf.Length < doorId.Length + ivbc) {
				curBuf = new byte[doorId.Length + ivbc];
				Encoding.ASCII.GetBytes(doorId, 0, doorId.Length, curBuf, 0);
			}

			// encode the value into the buffer
			Encoding.ASCII.GetBytes(ivs, 0, ivs.Length, curBuf, doorId.Length);
		}
	}
}
