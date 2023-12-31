using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;

namespace AdventOfCode.Year2023.Day19;

public partial class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, null);

	public static (long, long) Execute(string[] input)
	{
		var workflows = new Dictionary<string, Workflow>(750);
		var wfMode    = true;
		var part1     = 0L;
		var states    = new Queue<State>();
		var accepts   = new List<State>();

		states.Enqueue(new State(new IntRange(1, 4000), new IntRange(1, 4000), new IntRange(1, 4000), new IntRange(1, 4000), "in"));

		foreach (var line in input) {
			if (line == string.Empty) {
				// we have all our workflows now
				wfMode = false;

				// calculate all the valid ranges
				while (states.TryDequeue(out var thisState)) {
					var thisFlow = workflows[thisState.WorkflowName];

					foreach (var condition in thisFlow.Conditions) {
						var (matched, remaining) = SplitState(thisState, condition);

						if (!matched.IsEmpty()) {
							if (condition.Result.Outcome == Outcome.Accept) {
								accepts.Add(matched);
							} else if (condition.Result.Outcome == Outcome.Transfer) {
								states.Enqueue(matched);
							}
						}

						if (remaining.IsEmpty()) {
							break;
						} else {
							thisState = remaining;
						}
					}
				}

				// and carry on to the gears
				continue;
			}

			if (wfMode) {
				var wf = new Workflow(line);
				workflows.Add(wf.Name, wf);
			} else {
				// parse a gear
				var gear   = ParseGear(line.AsSpan());

				// check to see if it matches any acceptable ranges
				if (accepts.Any(a => a.X.Contains(gear.X) && a.M.Contains(gear.M) && a.A.Contains(gear.A) && a.S.Contains(gear.S))) {
					part1 += gear.X + gear.M + gear.A + gear.S;
				}
			}
		}

		var part2 = accepts.Sum(a => (long)(a.X.Max - a.X.Min + 1) * (a.M.Max - a.M.Min + 1) * (a.A.Max - a.A.Min + 1) * (a.S.Max - a.S.Min + 1));

		return (part1, part2);
	}

	private static (State Matched, State Remaining) SplitState(State inputState, Condition condition)
	{
		switch (condition.Property) {
			case 'x':
				var (xMatched, xRemain) = inputState.X.Split(condition.Operator, condition.Comparand);
				return (
					new State(xMatched, inputState.M, inputState.A, inputState.S, condition.Result.ToWorkflow()),
					new State(xRemain, inputState.M, inputState.A, inputState.S, inputState.WorkflowName));
			case 'm':
				var (mMatched, mRemain) = inputState.M.Split(condition.Operator, condition.Comparand);
				return (
					new State(inputState.X, mMatched, inputState.A, inputState.S, condition.Result.ToWorkflow()),
					new State(inputState.X, mRemain, inputState.A, inputState.S, inputState.WorkflowName));
			case 'a':
				var (aMatched, aRemain) = inputState.A.Split(condition.Operator, condition.Comparand);
				return (
					new State(inputState.X, inputState.M, aMatched, inputState.S, condition.Result.ToWorkflow()),
					new State(inputState.X, inputState.M, aRemain, inputState.S, inputState.WorkflowName));
			case 's':
				var (sMatched, sRemain) = inputState.S.Split(condition.Operator, condition.Comparand);
				return (
					new State(inputState.X, inputState.M, inputState.A, sMatched, condition.Result.ToWorkflow()),
					new State(inputState.X, inputState.M, inputState.A, sRemain, inputState.WorkflowName));
			case ' ' when condition.Operator == Operator.None:
				return (
					new State(inputState.X, inputState.M, inputState.A, inputState.S, condition.Result.ToWorkflow()),
					new State(IntRange.Empty, IntRange.Empty, IntRange.Empty, IntRange.Empty, string.Empty));
			default:
				throw new NotSupportedException("Unsupported property");
		}

		throw new NotImplementedException();
	}

	private static Gear ParseGear(ReadOnlySpan<char> line)
	{
		Span<Range> ranges = stackalloc Range[4];

		// we don't want the { } at the ends
		line = line[1..^1];

		// split on the commas
		line.Split(ranges, ',');

		var x = int.Parse(line[ranges[0]][2..]);
		var m = int.Parse(line[ranges[1]][2..]);
		var a = int.Parse(line[ranges[2]][2..]);
		var s = int.Parse(line[ranges[3]][2..]);

		return new Gear(x, m, a, s);
	}

	private class ExecutableWorkflow
	{
		private static readonly Type _gearType = typeof(Gear);
		private static readonly Type _intType = typeof(int);
		private static readonly Type _resultType = typeof(Result);
		private static readonly PropertyInfo _xProp = _gearType.GetProperty("X", BindingFlags.Public | BindingFlags.Instance) ?? throw new InvalidOperationException();
		private static readonly PropertyInfo _mProp = _gearType.GetProperty("M", BindingFlags.Public | BindingFlags.Instance) ?? throw new InvalidOperationException();
		private static readonly PropertyInfo _aProp = _gearType.GetProperty("A", BindingFlags.Public | BindingFlags.Instance) ?? throw new InvalidOperationException();
		private static readonly PropertyInfo _sProp = _gearType.GetProperty("S", BindingFlags.Public | BindingFlags.Instance) ?? throw new InvalidOperationException();

		private readonly Func<Gear, Result> _expr;

		public ExecutableWorkflow(ReadOnlySpan<char> definition)
		{
			Span<Range> ranges = stackalloc Range[10];

			// this expression takes a parameter (the gear)
			var gearParam = Expression.Parameter(_gearType);

			// find the start of the conditions
			var openBracePos = definition.IndexOf('{');

			// parse the name out of the beginning
			Name = definition[..openBracePos].ToString();

			// and now we're only interested in the definition part
			definition = definition[(openBracePos + 1)..^1];

			// find out where all the definitions start and stop
			var instructionCount = definition.Split(ranges, ',');

			// the final one is a constant
			Expression finalExpression = definition[^1] switch {
				'A' => Expression.Constant(new Result(Outcome.Accept, string.Empty), _resultType),
				'R' => Expression.Constant(new Result(Outcome.Reject, string.Empty), _resultType),
				_  => Expression.Constant(new Result(Outcome.Transfer, definition[ranges[instructionCount - 1]].ToString()), _resultType),
			};

			// and now work back-to-front
			for (var i = instructionCount - 2; i >= 0; i--) {
				var instrSpan = definition[ranges[i]];
				var colonPos  = instrSpan.IndexOf(':');
				var constExpr = Expression.Constant(int.Parse(instrSpan[2..colonPos]), _intType);
				var propExpr  = instrSpan[0] switch {
					'x' => Expression.Property(gearParam, _xProp),
					'm' => Expression.Property(gearParam, _mProp),
					'a' => Expression.Property(gearParam, _aProp),
					's' => Expression.Property(gearParam, _sProp),
					_ => throw new InvalidOperationException("Unknown property reference in condition"),
				};
				var condExpr = instrSpan[1] switch {
					'<' => Expression.LessThan(propExpr, constExpr),
					'>' => Expression.GreaterThan(propExpr, constExpr),
					_ => throw new InvalidOperationException("Unknown operator in condition"),
				};
				var successExpr = instrSpan[colonPos + 1] switch {
					'A' => Expression.Constant(new Result(Outcome.Accept, string.Empty), _resultType),
					'R' => Expression.Constant(new Result(Outcome.Reject, string.Empty), _resultType),
					_  => Expression.Constant(new Result(Outcome.Transfer, instrSpan[(colonPos + 1)..].ToString()), _resultType),
				};

				// make a new ternary, with finalExpression as the "false"
				finalExpression = Expression.Condition(condExpr, successExpr, finalExpression);
			}

			_expr = Expression.Lambda(finalExpression, gearParam).Compile() as Func<Gear, Result> ?? throw new InvalidOperationException("Unable to compile epxression to lambda");
		}

		public string Name { get; private set; }

		public Result ProcessGear(Gear gear) => _expr(gear);
	}

	private class Workflow
	{
		public Workflow(ReadOnlySpan<char> definition)
		{
			Span<Range> ranges = stackalloc Range[10];

			var conditions = new List<Condition>(8);

			// find the start of the conditions
			var openBracePos = definition.IndexOf('{');

			// parse the name out of the beginning
			Name = definition[..openBracePos].ToString();

			// and now we're only interested in the definition part
			definition = definition[(openBracePos + 1)..^1];

			// find out where all the definitions start and stop
			var instructionCount = definition.Split(ranges, ',');

			for (var i = 0; i < instructionCount - 1; i++) {
				var instrSpan = definition[ranges[i]];
				var colonPos  = instrSpan.IndexOf(':');
				var comparand = int.Parse(instrSpan[2..colonPos]);
				var property  = instrSpan[0];
				var op        = instrSpan[1] switch { '<' => Operator.LessThan, '>' => Operator.GreaterThan, _ => throw new InvalidOperationException("Unknown operator") };
				var result    = instrSpan[colonPos + 1] switch {
					'A' => new Result(Outcome.Accept, string.Empty),
					'R' => new Result(Outcome.Reject, string.Empty),
					_  => new Result(Outcome.Transfer, instrSpan[(colonPos + 1)..].ToString()),
				};

				conditions.Add(new Condition(property, op, comparand, result));
			}

			// add the final condition
			conditions.Add(definition[^1] switch {
				'A' => new Condition(' ', Operator.None, 0, new Result(Outcome.Accept, string.Empty)),
				'R' => new Condition(' ', Operator.None, 0, new Result(Outcome.Reject, string.Empty)),
				_ => new Condition(' ', Operator.None, 0, new Result(Outcome.Transfer, definition[ranges[instructionCount - 1]].ToString())),
			});

			Conditions = [.. conditions];
		}

		public string Name { get; private set; }

		public ImmutableArray<Condition> Conditions { get; private set; }
	}

	private enum Outcome
	{
		Accept,
		Reject,
		Transfer
	}

	private enum Operator
	{
		GreaterThan,
		LessThan,
		None,
	}

	private readonly record struct Condition(char Property, Operator Operator, int Comparand, Result Result);

	private readonly record struct Result(Outcome Outcome, string Workflow)
	{
		public string ToWorkflow() => Outcome switch {
			Outcome.Accept => "A",
			Outcome.Reject => "R",
			Outcome.Transfer => Workflow,
			_ => throw new InvalidOperationException("Invalid outcome"),
		};
	}

	private readonly record struct Gear(int X, int M, int A, int S);

	private readonly record struct State(IntRange X, IntRange M, IntRange A, IntRange S, string WorkflowName)
	{
		public bool IsEmpty() => X == IntRange.Empty || M == IntRange.Empty || A == IntRange.Empty || S == IntRange.Empty;
	}

	private readonly record struct IntRange(int Min, int Max)
	{
		public static IntRange Empty { get; } = new IntRange(0, 0);

		public bool Contains(int value) => Min <= value && Max >= value;

		public (IntRange Matched, IntRange Remaining) Split(Operator op, int comparand)
		{
			if (op == Operator.GreaterThan) {
				if (comparand <= Min) {
					return (this, Empty);
				} else {
					// matched is comparand + 1 to max
					// remaining is min to comparand
					return (new IntRange(comparand + 1, Max), new IntRange(Min, comparand));
				}
			} else if (op == Operator.LessThan) {
				if (comparand >= Max) {
					return (this, Empty);
				} else {
					// matched is min to comparand - 1
					// remaining is comparand to max
					return (new IntRange(Min, comparand - 1), new IntRange(comparand, Max));
				}
			} else {
				throw new NotSupportedException("Invalid operator");
			}
		}
	}
}
