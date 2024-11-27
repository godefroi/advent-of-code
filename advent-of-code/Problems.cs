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
	};

	private readonly static Lazy<IReadOnlyDictionary<int, ProblemMetadata>> _currentYearProblems = new(() => _problems.OrderByDescending(p => p.Key).First(p => p.Value.Value.Count > 0).Value.Value);

	private readonly static Lazy<ProblemMetadata> _currentProblem = new(() => _currentYearProblems.Value[_currentYearProblems.Value.Keys.Max()]);

	public static IReadOnlyDictionary<int, ProblemMetadata> CurrentYear => _currentYearProblems.Value;

	public static ProblemMetadata CurrentProblem => _currentProblem.Value;

	public static IEnumerable<int> Years => _problems.Keys;

	public static IReadOnlyDictionary<int, ProblemMetadata> GetProblems(int year) => _problems[year].Value;
}
