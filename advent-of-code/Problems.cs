using System.CommandLine;
using System.CommandLine.Parsing;
using AdventOfCode.Commands;

namespace AdventOfCode;

public static class Problems
{
	private readonly static Dictionary<int, Lazy<IReadOnlyDictionary<int, ProblemMetadata>>> _problems = new() {
		{ 2016, new Lazy<IReadOnlyDictionary<int, ProblemMetadata>>(() => Year2016.Problems.All) },
		{ 2019, new Lazy<IReadOnlyDictionary<int, ProblemMetadata>>(() => Year2019.Problems.All) },
		{ 2020, new Lazy<IReadOnlyDictionary<int, ProblemMetadata>>(() => Year2020.Problems.All) },
		{ 2021, new Lazy<IReadOnlyDictionary<int, ProblemMetadata>>(() => Year2021.Problems.All) },
		{ 2022, new Lazy<IReadOnlyDictionary<int, ProblemMetadata>>(() => Year2022.Problems.All) },
		{ 2023, new Lazy<IReadOnlyDictionary<int, ProblemMetadata>>(() => Year2023.Problems.All) },
		{ 2024, new Lazy<IReadOnlyDictionary<int, ProblemMetadata>>(() => Year2024.Problems.All) },
		{ 2025, new Lazy<IReadOnlyDictionary<int, ProblemMetadata>>(() => Year2025.Problems.All) },
	};

	private readonly static Lazy<IReadOnlyDictionary<int, ProblemMetadata>> _currentYearProblems = new(() => _problems.OrderByDescending(p => p.Key).First(p => p.Value.Value.Count > 0).Value.Value);

	private readonly static Lazy<ProblemMetadata> _currentProblem = new(() => _currentYearProblems.Value[_currentYearProblems.Value.Keys.Max()]);

	public static IReadOnlyDictionary<int, ProblemMetadata> CurrentYear => _currentYearProblems.Value;

	public static ProblemMetadata CurrentProblem => _currentProblem.Value;

	public static IEnumerable<int> Years => _problems.Keys;

	public static IReadOnlyDictionary<int, ProblemMetadata> GetProblems(int year) => _problems[year].Value;

	/// <remarks>
	/// Problems may be specified in the following ways:
	/// - All problems. In this case, we'll have neither a year nor a day.
	///	- All problems for a specific year. In this case, we'll have a year but not a day.
	///	- A specific problem. In this case, we'll have both a year and a day.
	///
	///	We have three ways to specify the various inputs to this calculation:
	///	- We can directly specify the input using the command line
	///	- We can specify the input (in some cases) using environment variables
	///	- We can allow the input to take its default value
	/// </remarks>
	public static ProblemMetadata[] GetProblems(ParseResult parseResult)
	{
		var yearResult = parseResult.GetResult(Options.Year);
		var dayResult  = parseResult.GetResult(Options.Day);
		var allResult  = parseResult.GetResult(Options.All);

		// "default" means "current, but not explicitly specified"
		// Precedence is explicit (via option) > explicit (via env) > default

		static (T? Value, int SpecificationLevel) GetValue<T>(OptionResult? optionResult, T? envValue)
		{
			if (optionResult != null && !optionResult.Implicit) {
				return (optionResult.GetValueOrDefault<T>(), 2);
			} else if (envValue != null) {
				return (envValue, 1);
			} else if (optionResult != null) {
				return (optionResult.GetValueOrDefault<T>(), 0);
			} else {
				return (default(T), -1);
			}
		}

		// if year is explicit and day is explicit, then we specified an exact day
		// if year is explicit but day is default, then we specified a whole year
		// if year is default and day is explicit, then we specified an exact day (in the default year)
		// if year is default and day is default, then we specified an exact day (default day in default year)

		var (year, yearLevel) = GetValue<int?>(yearResult, int.TryParse(Environment.GetEnvironmentVariable("AOC_YEAR"), out var yResult) ? yResult : null);
		var (day,  dayLevel)  = GetValue<int?>(dayResult,  int.TryParse(Environment.GetEnvironmentVariable("AOC_DAY"), out var dResult) ? dResult : null);
		var (all,  allLevel)  = GetValue(allResult,  default(bool?));

		// if day and all were all specified on the command line, that's an error
		if (dayLevel == 2 && allLevel == 2) {
			throw new InvalidOperationException($"Cannot specify option {{{Options.All.Name}}} along with {{{Options.Day.Name}}}");
		}

		if (allLevel > 1) {
			// if all was specified (it'll be 0 or 2, but we'll choose 1 here for correctness),
			// then we need to know how specific year was
			if (yearLevel == 2) {
				// all was specified, year was specified, we're doing a whole year
				if (year == null) {
					throw new InvalidOperationException("Year was specified, but no value was provided");
				} else {
					return [.. Problems.GetProblems(year.Value).Values.OrderBy(p => p.Day)];
				}
			} else {
				// if you do all, you have to be really explicit on year, or we do
				// all years
				return [.. Problems.Years
					.Select(Problems.GetProblems)
					.SelectMany(p => p.Values)
					.OrderBy(p => p.Year)
					.ThenBy(p => p.Day)
					.ToArray()];
			}
		} else {
			// we're not doing all
			if (year == null || yearLevel < 0) {
				// if year was unprovided (via any method), that's an error
				throw new InvalidOperationException("No year value provided");
			}

			if (day == null || dayLevel < 0) {
				// if year was unprovided (via any method), that's an error
				throw new InvalidOperationException("No day value provided");
			}

			// if you explicitly specified a year but didn't explicitly specify a day
			// then we'll do all for that year
			if (yearLevel >= 1 && dayLevel < yearLevel) {
				return [.. Problems.GetProblems(year.Value).Values.OrderBy(p => p.Day)];
			}

			// otherwise, we're doing a specific day
			var yearProblems = Problems.GetProblems(year.Value);

			if (!yearProblems.TryGetValue(day.Value, out var problem)) {
				throw new InvalidOperationException($"The specified problem, year {{{year.Value}}} day {{{day.Value}}} was not found.");
			}

			return [problem];
		}
	}
}
