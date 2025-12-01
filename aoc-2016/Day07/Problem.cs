using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace AdventOfCode.Year2016.Day07;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		return (input.Count(SupportsTls), input.Count(SupportsSsl));
	}

	private static bool SupportsTls(string address)
	{
		var addrSpan   = address.AsSpan();
		var inHypernet = false;
		var hasAbba    = false;

		while (addrSpan.Length > 0) {
			var thisChunkEnd = inHypernet ? addrSpan.IndexOf(']') : addrSpan.IndexOf('[');
			var thisChunk    = thisChunkEnd == -1 ? addrSpan : addrSpan[..thisChunkEnd];
			var chunkAbba    = HasAbbaSequence(thisChunk);

			if (inHypernet && chunkAbba) {
				return false;
			} else {
				hasAbba |= chunkAbba;
			}

			// if we're at the end of the span, get us out
			if (thisChunk.Length == addrSpan.Length) {
				break;
			}

			// advance in the span
			addrSpan   = addrSpan[(thisChunk.Length + 1)..];
			inHypernet = !inHypernet;
		}

		return hasAbba;
	}

	private static bool SupportsSsl(string address)
	{
		var addrSpan     = address.AsSpan();
		var inHypernet   = false;
		var abaSequences = new List<AbaSequence>();
		var babSequences = new List<AbaSequence>();

		while (addrSpan.Length > 0) {
			var thisChunkEnd = inHypernet ? addrSpan.IndexOf(']') : addrSpan.IndexOf('[');
			var thisChunk    = thisChunkEnd == -1 ? addrSpan : addrSpan[..thisChunkEnd];

			if (inHypernet) {
				babSequences.AddRange(FindAbaSequences(thisChunk).Select(s => new AbaSequence(s.Inside, s.Outside)));
			} else {
				abaSequences.AddRange(FindAbaSequences(thisChunk));
			}

			// if we're at the end of the span, get us out
			if (thisChunk.Length == addrSpan.Length) {
				break;
			}

			// advance in the span
			addrSpan   = addrSpan[(thisChunk.Length + 1)..];
			inHypernet = !inHypernet;
		}

		return abaSequences.Any(babSequences.Contains);
	}

	private static bool HasAbbaSequence(ReadOnlySpan<char> span)
	{
		for (var i = 0; i < span.Length - 3; i++) {
			if (span[i + 0] != span[i + 1] && span[i + 1] == span[i + 2] && span[i + 3] == span[i + 0]) {
				return true;
			}
		}

		return false;
	}

	private static List<AbaSequence> FindAbaSequences(ReadOnlySpan<char> span)
	{
		var sequences = new List<AbaSequence>();

		for (var i = 0; i < span.Length - 2; i++) {
			if (span[i + 0] != span[i + 1] && span[i + 0] == span[i + 2]) {
				sequences.Add(new(span[i + 0], span[i + 1]));
			}
		}

		return sequences;
	}

	private readonly record struct AbaSequence(char Outside, char Inside);

	[Test]
	[Arguments("abba", true)]
	[Arguments("mnop", false)]
	[Arguments("qrst", false)]
	[Arguments("aaaa", false)]
	[Arguments("xyyx", true)]
	[Arguments("ioxxoj", true)]
	[Arguments("ioj", false)]
	public async Task AbbaSequenceDetectedCorrectly(string input, bool expected) => await Assert.That(HasAbbaSequence(input)).IsEqualTo(expected);

	[Test]
	[Arguments("abba[mnop]qrst", true)]
	[Arguments("abcd[bddb]xyyx", false)]
	[Arguments("aaaa[qwer]tyui", false)]
	[Arguments("ioxxoj[asdfgh]zxcvbn", true)]
	public async Task TlsSupportDetectedCorrectly(string input, bool expected) => await Assert.That(SupportsTls(input)).IsEqualTo(expected);

	[Test]
	[Arguments("aba[bab]xyz", true)]
	[Arguments("xyx[xyx]xyx", false)]
	[Arguments("aaa[kek]eke", true)]
	[Arguments("zazbz[bzb]cdb", true)]
	public async Task SslSupportDetectedCorrectly(string input, bool expected) => await Assert.That(SupportsSsl(input)).IsEqualTo(expected);
}
