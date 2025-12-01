using static AdventOfCode.FloydWarshall;
using static AdventOfCode.Tools;

namespace AdventOfCode;

public class ToolsTests
{
	[Test]
	public async Task ChunkingWorksCorrectly()
	{
		var sampleData = new[] {
			"1000",
			"2000",
			"3000",
			"",
			"4000",
			"",
			"5000",
			"6000",
			"",
			"7000",
			"8000",
			"9000",
			"",
			"10000" };

		var chunks = ChunkByEmpty(sampleData).Select(e => e.ToList()).ToList();

		await Assert.That(chunks).IsEquivalentTo([
			new List<string>(["1000", "2000", "3000"]),
			new List<string>(["4000"]),
			new List<string>(["5000", "6000"]),
			new List<string>(["7000", "8000", "9000"]),
			new List<string>(["10000"]),
		]);
	}

	[Test]
	[Arguments( 0, new[] { 1, 2, 3, 4 }, new[] { 1, 2 }, 0)]
	[Arguments( 1, new[] { 1, 2, 3, 4 }, new[] { 2, 3 }, 0)]
	[Arguments( 2, new[] { 1, 2, 3, 4 }, new[] { 3, 4 }, 0)]
	[Arguments(-1, new[] { 1, 2, 3, 4 }, new[] { 1, 2 }, 1)]
	[Arguments( 1, new[] { 1, 2, 3, 4 }, new[] { 2, 3 }, 1)]
	[Arguments( 6, new[] { 1, 2, 3, 4, 1, 2, 3, 4 }, new[] { 3, 4 }, 4)]
	public async Task FindSequenceWorksCorrectly(int expectedOffset, int[] toSearch, int[] toFind, int offset)
	{
		await Assert.That(FindSequence([.. toSearch], [.. toFind], offset)).IsEqualTo(expectedOffset);
	}

	[Test]
	[Arguments(new[] { 0 }, new[] { 1, 2, 3, 4 }, new[] { 1, 2 })]
	[Arguments(new[] { 1 }, new[] { 1, 2, 3, 4 }, new[] { 2, 3 })]
	[Arguments(new[] { 2 }, new[] { 1, 2, 3, 4 }, new[] { 3, 4 })]
	[Arguments(new[] { 2, 6 }, new[] { 1, 2, 3, 4, 1, 2, 3, 4 }, new[] { 3, 4 })]
	[Arguments(new[] { 58, 68 }, new[] { 76, 6, 82, 8, 76, 4, 82, 8, 76, 12, 76, 12, 82, 10, 76, 4, 76, 12, 82, 10, 76, 4, 76, 12, 76, 6, 76, 4, 76, 4, 76, 12, 82, 10, 76, 4, 76, 12, 76, 6, 76, 4, 76, 4, 76, 12, 82, 10, 76, 4, 76, 12, 76, 6, 76, 4, 76, 4, 76, 6, 82, 8, 76, 4, 82, 8, 76, 12, 76, 6, 82, 8, 76, 4, 82, 8, 76, 12 }, new[] { 76, 6, 82, 8, 76, 4, 82, 8, 76, 12 }, 2)]
	public async Task FindSequencesWorksCorrectly(int[] expectedOffsets, int[] toSearch, int[] toFind, int offset = 0)
	{
		await Assert.That(FindSequence([.. toSearch], [.. toFind], offset, false)).IsEquivalentTo(expectedOffsets);
	}

	[Test]
	public void FloydWarshallWorks()
	{
		var inputGraph = new[] {
			new WeightedEdge<string, int>("1", "3", -2),
			new WeightedEdge<string, int>("3", "4", 2),
			new WeightedEdge<string, int>("4", "2", -1),
			new WeightedEdge<string, int>("2", "1", 4),
			new WeightedEdge<string, int>("2", "3", 3),
		};

		ComputeDistances(inputGraph);
	}
}
