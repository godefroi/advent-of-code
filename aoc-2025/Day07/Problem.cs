using BenchmarkDotNet.Attributes;

namespace AdventOfCode.Year2025.Day07;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(ExecuteCA, typeof(Problem), typeof(Day07Benchmarks));

	public static (long, long) ExecuteSA(string[] input)
	{
		//var beams = new long[input[0].Length];
		Span<long> beams = stackalloc long[input[0].Length];
		var p1    = 0;

		// mark the starting point
		beams[input[0].IndexOf('S')] = 1;

		// and we go from the 2nd row down
		for (var y = 2; y < input.Length; y += 2) {
			for (var x = 0; x < input[y].Length; x++) {
				// if the row above has a beam, and this row has a splitter, then
				// this row no longer has a beam at this x, and instead has one to
				// the left and right
				if (beams[x] > 0 && input[y][x] == '^') {
					p1++;
					beams[x - 1] += beams[x];
					beams[x + 1] += beams[x];
					beams[x    ] = 0;
				}
			}
		}

		var p2 = 0L;

		for (var i = 0; i < beams.Length; i++) {
			p2 += beams[i];
		}

		return (p1, p2);
	}

	public static (long, long) ExecuteCA(char[] input)
	{
		var width = input.IndexOf('\n');
		var lines = (input.Length / width) - 1;
		var p1    = 0;
		var p2    = 0L;

		Span<long> beams = stackalloc long[width];

		// mark the starting point
		beams[input.IndexOf('S')] = 1;

		// and we go from the 2nd row down
		for (var y = 2; y < lines; y += 2) {
			for (var x = 0; x < width; x++) {
				// if the row above has a beam, and this row has a splitter, then
				// this row no longer has a beam at this x, and instead has one to
				// the left and right
				if (beams[x] > 0 && input[(y * (width + 1)) + x] == '^') {
					p1++;
					beams[x - 1] += beams[x];
					beams[x + 1] += beams[x];
					beams[x    ] = 0;
				}
			}
		}

		for (var i = 0; i < beams.Length; i++) {
			p2 += beams[i];
		}

		return (p1, p2);
	}

	public class Day07Tests
	{
		private readonly char[] _sampleData = """
			.......S.......
			...............
			.......^.......
			...............
			......^.^......
			...............
			.....^.^.^.....
			...............
			....^.^...^....
			...............
			...^.^...^.^...
			...............
			..^...^.....^..
			...............
			.^.^.^.^.^...^.
			...............
			""".ReplaceLineEndings("\n").ToCharArray();

		[Test]
		public async Task Part1Works()
		{
			await Assert.That(ExecuteCA(_sampleData).Item1).IsEqualTo(21);
		}

		[Test]
		public async Task Part2Works()
		{
			await Assert.That(ExecuteCA(_sampleData).Item2).IsEqualTo(40);
		}
	}

	public class Day07Benchmarks
	{
		private string[] _lines = [];
		private char[] _chars = [];

		[GlobalSetup]
		public void BenchmarkSetup()
		{
			_lines = ReadFileLines("input.txt");
			_chars = [.. _lines.SelectMany(l => l + '\n')];
		}

		[Benchmark]
		public void RunWithStrings()
		{
			ExecuteSA(_lines);
		}

		[Benchmark]
		public void RunWithChars()
		{
			ExecuteCA(_chars);
		}
	}
}
