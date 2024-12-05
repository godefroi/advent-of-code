using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Parsing;

namespace AdventOfCode.Commands;

public static class Options
{
	private readonly static int? _envVarYear = int.TryParse(Environment.GetEnvironmentVariable("AOC_YEAR"), out var result) ? result : null;
	private readonly static int? _envVarDay = int.TryParse(Environment.GetEnvironmentVariable("AOC_DAY"), out var result) ? result : null;
	private readonly static string? _envVarInput = Environment.GetEnvironmentVariable("AOC_INPUTFILE");

	public static Option<bool> All { get; } = new("--all", () => false, "Include all problems");

	public static Option<int> Warmups { get; } = new("--warmups", () => 10, "Number of times to execute problems for warmup");

	public static Option<int> Executions { get; } = new("--executions", () => 5000, "Number of times to execute each problem");

	public static Option<string> InputName { get; } = new("--inputFile", () => _envVarInput ?? "input.txt", "Filename to read as problem input (env AOC_INPUTFILE)");

	public static Option<int> Year { get; } = new("--year", () => _envVarYear ?? Problems.CurrentProblem.Year, "Year on which to operate (env AOC_YEAR)");

	public static Option<int> Day { get; } = new("--day", () => _envVarDay ?? Problems.CurrentProblem.Day, "Day on which to operate (env AOC_DAY)");

	public static Lazy<ProblemMetadataBinder> ProblemMetadata => new();

	public class ProblemMetadataBinder : BinderBase<ProblemMetadata[]>
	{
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
		protected override ProblemMetadata[] GetBoundValue(BindingContext bindingContext)
		{
			var yearResult = bindingContext.ParseResult.FindResultFor(Year);
			var dayResult  = bindingContext.ParseResult.FindResultFor(Day);
			var allResult  = bindingContext.ParseResult.FindResultFor(All);

			// "default" means "current, but not explicitly specified"
			// Precedence is explicit (via option) > explicit (via env) > default

			static (T? Value, int SpecificationLevel) GetValue<T>(OptionResult? optionResult, T? envValue)
			{
				if (optionResult != null && !optionResult.IsImplicit) {
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

			var (year, yearLevel) = GetValue(yearResult, _envVarYear);
			var (day,  dayLevel)  = GetValue(dayResult,  _envVarDay);
			var (all,  allLevel)  = GetValue(allResult,  default(bool?));

			// if day and all were all specified on the command line, that's an error
			if (dayLevel == 2 && allLevel == 2) {
				throw new InvalidOperationException($"Cannot specify option {{{All.Name}}} along with {{{Day.Name}}}");
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
}
