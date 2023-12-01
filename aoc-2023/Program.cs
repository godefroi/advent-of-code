using System.Diagnostics;

aoc_2023.Day01.Problem.Execute("input.txt");

var sw = Stopwatch.StartNew();

for (var i = 0; i < 10000; i++)
	aoc_2023.Day01.Problem.Execute("input.txt");

sw.Stop();

Console.WriteLine(sw.Elapsed.TotalMilliseconds / 10000d);

//BenchmarkDotNet.Running.BenchmarkRunner.Run<aoc_2023.Day01.Problem>();
