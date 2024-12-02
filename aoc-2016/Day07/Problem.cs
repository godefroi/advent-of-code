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

	[Theory]
	[InlineData("abba", true)]
	[InlineData("mnop", false)]
	[InlineData("qrst", false)]
	[InlineData("aaaa", false)]
	[InlineData("xyyx", true)]
	[InlineData("ioxxoj", true)]
	[InlineData("ioj", false)]
	public void AbbaSequenceDetectedCorrectly(string input, bool expected) => Assert.Equal(expected, HasAbbaSequence(input));

	[Theory]
	[InlineData("abba[mnop]qrst", true)]
	[InlineData("abcd[bddb]xyyx", false)]
	[InlineData("aaaa[qwer]tyui", false)]
	[InlineData("ioxxoj[asdfgh]zxcvbn", true)]
	public void TlsSupportDetectedCorrectly(string input, bool expected) => Assert.Equal(expected, SupportsTls(input));

	[Theory]
	[InlineData("aba[bab]xyz", true)]
	[InlineData("xyx[xyx]xyx", false)]
	[InlineData("aaa[kek]eke", true)]
	[InlineData("zazbz[bzb]cdb", true)]
	public void SslSupportDetectedCorrectly(string input, bool expected) => Assert.Equal(expected, SupportsSsl(input));
}
