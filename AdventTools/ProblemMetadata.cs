using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace AdventOfCode;

public partial record ProblemMetadata
{
	[SetsRequiredMembers]
	public ProblemMetadata(Func<string[], (long, long)> main, Type problemType, Type? benchmarkType, [CallerFilePath]string filePath = "")
	{
		(Year, Day) = ParsePath(filePath);
		Main        = input => main(input.Strings);
		Path        = System.IO.Path.GetDirectoryName(filePath) ?? throw new InvalidOperationException("Unable to find directory name.");
		Problem     = problemType;
		Benchmarks  = benchmarkType;
	}

	[SetsRequiredMembers]
	public ProblemMetadata(Func<string[], (long, string)> main, Type problemType, Type? benchmarkType, [CallerFilePath]string filePath = "")
	{
		(Year, Day) = ParsePath(filePath);
		Main        = input => main(input.Strings);
		Path        = System.IO.Path.GetDirectoryName(filePath) ?? throw new InvalidOperationException("Unable to find directory name.");
		Problem     = problemType;
		Benchmarks  = benchmarkType;
	}

	[SetsRequiredMembers]
	public ProblemMetadata(Func<string[], (string, long)> main, Type problemType, Type? benchmarkType, [CallerFilePath]string filePath = "")
	{
		(Year, Day) = ParsePath(filePath);
		Main        = input => main(input.Strings);
		Path        = System.IO.Path.GetDirectoryName(filePath) ?? throw new InvalidOperationException("Unable to find directory name.");
		Problem     = problemType;
		Benchmarks  = benchmarkType;
	}

	[SetsRequiredMembers]
	public ProblemMetadata(Func<string[], (string, string)> main, Type problemType, Type? benchmarkType, [CallerFilePath]string filePath = "")
	{
		(Year, Day) = ParsePath(filePath);
		Main        = input => main(input.Strings);
		Path        = System.IO.Path.GetDirectoryName(filePath) ?? throw new InvalidOperationException("Unable to find directory name.");
		Problem     = problemType;
		Benchmarks  = benchmarkType;
	}

	[SetsRequiredMembers]
	public ProblemMetadata(Func<char[], (long, long)> main, Type problemType, Type? benchmarkType, [CallerFilePath]string filePath = "")
	{
		(Year, Day) = ParsePath(filePath);
		Main        = input => main(input.Chars);
		Path        = System.IO.Path.GetDirectoryName(filePath) ?? throw new InvalidOperationException("Unable to find directory name.");
		Problem     = problemType;
		Benchmarks  = benchmarkType;
	}

	public required int Year { get; init; }

	public required int Day { get; init; }

	public required Func<ProblemInput, object> Main { get; init; }

	public required string Path { get; init; }

	public required Type Problem { get; init; }

	public Type? Benchmarks { get; init; }

	private static (int Year, int Day) ParsePath(string filePath)
	{
		var match = GetDateRegex().Match(filePath);

		if (!match.Success) {
			throw new InvalidOperationException($"Unable to parse year and day from file path {filePath}");
		}

		return (int.Parse(match.Groups["year"].Value), int.Parse(match.Groups["day"].Value));
	}

	public static IEnumerable<ProblemMetadata> FindMetadata(Assembly assembly)
	{
		foreach (var type in assembly.ExportedTypes) {
			var members = type.GetMembers(BindingFlags.Public | BindingFlags.Static);
			var metadataField = members.FirstOrDefault(m =>
				m is PropertyInfo prop && prop.PropertyType == typeof(ProblemMetadata) && prop.CanRead ||
				m is FieldInfo field && field.FieldType == typeof(ProblemMetadata));

			if (metadataField == null) {
				continue;
			}

			yield return metadataField switch {
				PropertyInfo prop => prop.GetValue(null) as ProblemMetadata ?? throw new InvalidOperationException("Returned something non-metadata"),
				FieldInfo field => field.GetValue(null) as ProblemMetadata ?? throw new InvalidOperationException("Returned something non-metadata"),
				_ => throw new InvalidOperationException("Only properties and fields are supported."),
			};
		}
	}

	[GeneratedRegex("(?<year>20\\d{2}).*(?<day>\\d{2})")]
	private static partial Regex GetDateRegex();
}
