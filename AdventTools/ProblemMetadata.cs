using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace AdventOfCode;

public readonly record struct ProblemMetadata(Type ProblemType, int Day, string Path, Action<string> Main, Type BenchmarkType);

public partial record ProblemMetadata2
{
	[SetsRequiredMembers]
	public ProblemMetadata2(Func<string[], (long, long)> main, Type? benchmarkType = null, [CallerFilePath]string filePath = "")
	{
		var match = GetDateRegex().Match(filePath);

		if (!match.Success) {
			throw new InvalidOperationException($"Unable to parse year and day from file path {filePath}");
		}

		Year       = int.Parse(match.Groups["year"].Value);
		Day        = int.Parse(match.Groups["day"].Value);
		Main       = input => main(input).ToString();
		Path       = System.IO.Path.GetDirectoryName(filePath) ?? throw new InvalidOperationException("Unable to find directory name.");
		Benchmarks = benchmarkType;
	}

	[SetsRequiredMembers]
	public ProblemMetadata2(Func<string[], (long, string)> main, Type? benchmarkType = null, [CallerFilePath]string filePath = "")
	{
		var match = GetDateRegex().Match(filePath);

		if (!match.Success) {
			throw new InvalidOperationException($"Unable to parse year and day from file path {filePath}");
		}

		Year       = int.Parse(match.Groups["year"].Value);
		Day        = int.Parse(match.Groups["day"].Value);
		Main       = input => main(input).ToString();
		Path       = System.IO.Path.GetDirectoryName(filePath) ?? throw new InvalidOperationException("Unable to find directory name.");
		Benchmarks = benchmarkType;
	}

	[SetsRequiredMembers]
	public ProblemMetadata2(Func<string[], (string, long)> main, Type? benchmarkType = null, [CallerFilePath]string filePath = "")
	{
		var match = GetDateRegex().Match(filePath);

		if (!match.Success) {
			throw new InvalidOperationException($"Unable to parse year and day from file path {filePath}");
		}

		Year       = int.Parse(match.Groups["year"].Value);
		Day        = int.Parse(match.Groups["day"].Value);
		Main       = input => main(input).ToString();
		Path       = System.IO.Path.GetDirectoryName(filePath) ?? throw new InvalidOperationException("Unable to find directory name.");
		Benchmarks = benchmarkType;
	}

	[SetsRequiredMembers]
	public ProblemMetadata2(Func<string[], (string, string)> main, Type? benchmarkType = null, [CallerFilePath]string filePath = "")
	{
		var match = GetDateRegex().Match(filePath);

		if (!match.Success) {
			throw new InvalidOperationException($"Unable to parse year and day from file path {filePath}");
		}

		Year       = int.Parse(match.Groups["year"].Value);
		Day        = int.Parse(match.Groups["day"].Value);
		Main       = input => main(input).ToString();
		Path       = System.IO.Path.GetDirectoryName(filePath) ?? throw new InvalidOperationException("Unable to find directory name.");
		Benchmarks = benchmarkType;
	}

	public required int Year { get; init; }

	public required int Day { get; init; }
	
	public required Func<string[], string> Main { get; init; }

	public required string Path { get; init; }

	public Type? Benchmarks { get; init; }

	public static IEnumerable<ProblemMetadata2> FindMetadata(Assembly assembly)
	{
		foreach (var type in assembly.ExportedTypes) {
			var members = type.GetMembers(BindingFlags.Public | BindingFlags.Static);
			var metadataField = members.FirstOrDefault(m =>
				m is PropertyInfo prop && prop.PropertyType == typeof(ProblemMetadata2) && prop.CanRead ||
				m is FieldInfo field && field.FieldType == typeof(ProblemMetadata2));

			if (metadataField == null) {
				continue;
			}

			yield return metadataField switch {
				PropertyInfo prop => prop.GetValue(null) as ProblemMetadata2 ?? throw new InvalidOperationException("Returned something non-metadata"),
				FieldInfo field => field.GetValue(null) as ProblemMetadata2 ?? throw new InvalidOperationException("Returned something non-metadata"),
				_ => throw new InvalidOperationException("Only properties and fields are supported."),
			};
		}
	}

	[GeneratedRegex("(?<year>20\\d{2}).*(?<day>\\d{2})")]
	private static partial Regex GetDateRegex();
}
