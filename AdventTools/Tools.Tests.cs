using static AdventOfCode.FloydWarshall;
using static AdventOfCode.Tools;

namespace AdventOfCode;

public class ToolsTests
{
	[Fact]
	public void ChunkingWorksCorrectly()
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

		Assert.Collection(chunks,
			chunk => Assert.Collection(chunk,
				l => l.Equals("1000"),
				l => l.Equals("2000"),
				l => l.Equals("3000")),
			chunk => Assert.Collection(chunk,
				l => l.Equals("4000")),
			chunk => Assert.Collection(chunk,
				l => l.Equals("5000"),
				l => l.Equals("6000")),
			chunk => Assert.Collection(chunk,
				l => l.Equals("7000"),
				l => l.Equals("8000"),
				l => l.Equals("9000")),
			chunk => Assert.Collection(chunk,
				l => l.Equals("10000")));
	}

	[Theory]
	[InlineData( 0, new[] { 1, 2, 3, 4 }, new[] { 1, 2 }, 0)]
	[InlineData( 1, new[] { 1, 2, 3, 4 }, new[] { 2, 3 }, 0)]
	[InlineData( 2, new[] { 1, 2, 3, 4 }, new[] { 3, 4 }, 0)]
	[InlineData(-1, new[] { 1, 2, 3, 4 }, new[] { 1, 2 }, 1)]
	[InlineData( 1, new[] { 1, 2, 3, 4 }, new[] { 2, 3 }, 1)]
	[InlineData( 6, new[] { 1, 2, 3, 4, 1, 2, 3, 4 }, new[] { 3, 4 }, 4)]
	public void FindSequenceWorksCorrectly(int expectedOffset, int[] toSearch, int[] toFind, int offset)
	{
		Assert.Equal(expectedOffset, FindSequence(toSearch.ToList(), toFind.ToList(), offset));
	}

	[Theory]
	[InlineData(new[] { 0 }, new[] { 1, 2, 3, 4 }, new[] { 1, 2 })]
	[InlineData(new[] { 1 }, new[] { 1, 2, 3, 4 }, new[] { 2, 3 })]
	[InlineData(new[] { 2 }, new[] { 1, 2, 3, 4 }, new[] { 3, 4 })]
	[InlineData(new[] { 2, 6 }, new[] { 1, 2, 3, 4, 1, 2, 3, 4 }, new[] { 3, 4 })]
	[InlineData(new[] { 58, 68 }, new[] { 76, 6, 82, 8, 76, 4, 82, 8, 76, 12, 76, 12, 82, 10, 76, 4, 76, 12, 82, 10, 76, 4, 76, 12, 76, 6, 76, 4, 76, 4, 76, 12, 82, 10, 76, 4, 76, 12, 76, 6, 76, 4, 76, 4, 76, 12, 82, 10, 76, 4, 76, 12, 76, 6, 76, 4, 76, 4, 76, 6, 82, 8, 76, 4, 82, 8, 76, 12, 76, 6, 82, 8, 76, 4, 82, 8, 76, 12 }, new[] { 76, 6, 82, 8, 76, 4, 82, 8, 76, 12 }, 2)]
	public void FindSequencesWorksCorrectly(int[] expectedOffsets, int[] toSearch, int[] toFind, int offset = 0)
	{
		Assert.Equal(expectedOffsets, FindSequence(toSearch.ToList(), toFind.ToList(), offset, false));
	}

	[Fact]
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
