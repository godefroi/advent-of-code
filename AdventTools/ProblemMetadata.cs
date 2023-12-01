using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AdventOfCode;

public readonly record struct ProblemMetadata(Type ProblemType, int Day, string Path, Action<string> Main, Type BenchmarkType);

public record ProblemMetadata2
{
	public required int Year { get; init; }

	public required int Day { get; init; }
	
	public required Action<string> Main { get; init; }

	[SetsRequiredMembers]
	public ProblemMetadata2(Func<string, (long, long)> main, [CallerFilePath]string filePath = "")
	{
		// parse year/day from file path
		Year = 2023;
		Day  = 1;
		Main = s => Console.WriteLine(main(s));
	}

	[SetsRequiredMembers]
	public ProblemMetadata2(Action<string> main, [CallerFilePath]string filePath = "")
	{
		// parse year/day from file path
		Year = 2023;
		Day  = 1;
		Main = main;
	}
}

public static class MetadataReflector
{

}
