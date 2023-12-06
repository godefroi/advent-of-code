using System.Diagnostics;

namespace AdventOfCode.Year2023;

public class Program
{
	public static async Task Main(string[] args)
	{
		aoc_2023.Day01.Problem.Execute(ReadFileLines("input.txt"));

		var sw = Stopwatch.StartNew();

		for (var i = 0; i < 10000; i++)
			aoc_2023.Day01.Problem.Execute(ReadFileLines("input.txt"));

		sw.Stop();

		Console.WriteLine($"avg: {sw.Elapsed.TotalMilliseconds / 10000d}");

		//BenchmarkDotNet.Running.BenchmarkRunner.Run<aoc_2023.Day01.Problem>();

		await Task.CompletedTask;
		//var runner = new Xunit.Sdk.TestRunner<AdventOfCode.Year2023.Day05.IntervalTests>()
	}
}
