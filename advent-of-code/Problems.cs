namespace AdventOfCode;

public static class Problems
{
	private readonly static Dictionary<int, Lazy<IReadOnlyDictionary<int, ProblemMetadata2>>> _problems = new() {
		{ 2019, new Lazy<IReadOnlyDictionary<int, ProblemMetadata2>>(() => Year2019.Problems.All) },
		{ 2023, new Lazy<IReadOnlyDictionary<int, ProblemMetadata2>>(() => Year2023.Problems.All) },
	};

	private readonly static Lazy<IReadOnlyDictionary<int, ProblemMetadata2>> _currentYearProblems = new(() => _problems[_problems.Keys.Max()].Value);

	private readonly static Lazy<ProblemMetadata2> _currentProblem = new(_currentYearProblems.Value[_currentYearProblems.Value.Keys.Max()]);
	
	public static IReadOnlyDictionary<int, ProblemMetadata2> CurrentYear => _currentYearProblems.Value;

	public static ProblemMetadata2 CurrentProblem => _currentProblem.Value;

	public static IEnumerable<int> Years => _problems.Keys;

	public static IReadOnlyDictionary<int, ProblemMetadata2> GetProblems(int year) => _problems[year].Value;
}
