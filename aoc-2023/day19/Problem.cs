using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Z3;

namespace AdventOfCode.Year2023.Day19;

public partial class Problem
{
	public static ProblemMetadata2 Metadata { get; } = new(Execute, null);

	public static (long, long) Execute(string[] input)
	{
		var eworkflows = new Dictionary<string, ExecutableWorkflow>(750);
		var workflows  = new Dictionary<string, Workflow>(750);
		var wfMode     = true;
		var part1      = 0L;

		foreach (var line in input) {
			if (line == string.Empty) {
				wfMode = false;
				continue;
			}

			if (wfMode) {
				var ewf = new ExecutableWorkflow(line);
				eworkflows.Add(ewf.Name, ewf);

				var wf = new Workflow(line);
				workflows.Add(wf.Name, wf);
			} else {
				// parse a gear
				var gear   = ParseGear(line.AsSpan());
				var result = new Result(Outcome.Transfer, "in");

				while (result.Outcome == Outcome.Transfer) {
					//Console.Write($"{result.Workflow} -> ");
					result = eworkflows[result.Workflow].ProcessGear(gear);
				}

				//Console.WriteLine(result.Outcome);
				if (result.Outcome == Outcome.Accept) {
					part1 += gear.X + gear.M + gear.A + gear.S;
				}
			}
		}

		var chains = new List<List<Condition>>();

		foreach (var wf in workflows.Values) {
			for (var i = 0; i < wf.Conditions.Length; i++) {
				if (wf.Conditions[i].Result.Outcome != Outcome.Accept) {
					continue;
				}

				var chain = new List<Condition>() {
					wf.Conditions[i],
				};

				chains.Add(chain);

				// so we have wf.Conditions[i] which results in acceptance
				// then, go up the chain, inverting the conditions we find,
				// building a big AND expression
				for (var j = i - 1; j >= 0; j--) {
					
				}
			}
		}

		return (part1, 0);
	}

	private static FuncDecl GetFunction(string functionName, Dictionary<string, Workflow> workflows, Context ctx, Dictionary<string, FuncDecl> functionCache)
	{
		// check the cache
		if (functionCache.TryGetValue(functionName, out var cachedDecl)) {
			return cachedDecl;
		}

		// don't have it cached, gotta build it
		var decl = ctx.MkFuncDecl(functionName, ctx.IntSort, ctx.IntSort);
		//decl.

		return decl;
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

			for (var i = 0; i < instructionCount - 2; i++) {
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

	private readonly record struct Result(Outcome Outcome, string Workflow);

	private readonly record struct Gear(int X, int M, int A, int S);
}
