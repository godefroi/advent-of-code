using Combinatorics.Collections;

namespace aoc_2020.Day10;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Main);

	public static (long, long) Main(string[] input)
	{
		var adapters = input.Select(int.Parse).Order().ToList();
		var target   = adapters.Last() + 3;

		adapters.Insert(0, 0);
		adapters.Add(target);

		var groups = adapters.Zip(adapters.Skip(1)).GroupBy(pair => pair.Second - pair.First);
		var part1  = groups.Single(g => g.Key == 1).Count() * groups.Single(g => g.Key == 3).Count();

		//Console.WriteLine($"part 1: {part1}"); // part 1 is 2482

		adapters.Remove(0);
		adapters.Remove(target);

		return (part1, ValidCombinations(target, adapters));
	}

	private static long ValidCombinations(int target, List<int> adapters)
	{
		var counter = new Dictionary<int, long>() {
			{ 0, 1 },
		};

		adapters = adapters.Order().Append(target).ToList();

		foreach (var adapter in adapters) {
			counter[adapter] = counter.GetValueOrDefault(adapter - 3) + counter.GetValueOrDefault(adapter - 2) + counter.GetValueOrDefault(adapter - 1);
		}

		return counter[target];
	}

	[Fact]
	public void AdapterCombinationsWorkCorrectly1()
	{
		var adapters = ReadFileLines("inputSample1.txt", int.Parse).ToList();

		Assert.Equal(8, ValidCombinations(adapters.Max() + 3, adapters));
	}

	[Fact]
	public void AdapterCombinationsWorkCorrectly2()
	{
		var adapters = ReadFileLines("inputSample2.txt", int.Parse).ToList();

		Assert.Equal(19208, ValidCombinations(adapters.Max() + 3, adapters));
	}
}
