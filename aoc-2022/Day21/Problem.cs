using System.Linq.Expressions;

namespace aoc_2022.Day21;

public partial class Problem
{
	public static ProblemMetadata2 Metadata { get; } = new(Main, typeof(Problem));

	public static (long, long) Main(string[] input)
	{
		var part1 = Part1(input);
		var part2 = Part2(input);

		return (part1, part2);
	}

	private static long Part1(string[] input)
	{
		var constants   = new Dictionary<string, ConstantExpression>();
		var operations  = new Dictionary<string, (string input1, string input2, string operation)>();
		var expressions = new Dictionary<string, Expression>();

		foreach (var match in input.Select(s => ParseRegex().Match(s))) {
			if (!match.Success) {
				throw new InvalidDataException("The regular expression failed to match");
			}

			if (match.Groups["constant"].Success) {
				constants.Add(match.Groups["name"].Value, Expression.Constant(long.Parse(match.Groups["constant"].Value)));
			} else {
				operations.Add(match.Groups["name"].Value, (match.Groups["input1"].Value, match.Groups["input2"].Value, match.Groups["op"].Value));
			}
		}

		while (operations.Count > 0) {
			var keys = operations.Keys.ToList();

			foreach (var key in keys) {
				var op     = operations[key];
				var input1 = default(Expression);
				var input2 = default(Expression);

				if (constants.TryGetValue(op.input1, out var cinput1)) {
					input1 = cinput1;
				} else if (expressions.TryGetValue(op.input1, out var einput1)) {
					input1 = einput1;
				}

				if (constants.TryGetValue(op.input2, out var cinput2)) {
					input2 = cinput2;
				} else if (expressions.TryGetValue(op.input2, out var einput2)) {
					input2 = einput2;
				}

				if (input1 != null && input2 != null) {
					expressions.Add(key, op.operation switch {
						"+" => Expression.Add(input1, input2),
						"-" => Expression.Subtract(input1, input2),
						"/" => Expression.Divide(input1, input2),
						"*" => Expression.Multiply(input1, input2),
						_ => throw new InvalidDataException("The specified operation is invalid"),
					});

					operations.Remove(key);
				}
			}
		}

		var root = expressions["root"];

		//Console.WriteLine(root.ToString());

		var rootFunc = Expression.Lambda<Func<long>>(root).Compile();

		return rootFunc();
	}

	private static long Part2(string[] input)
	{
		var constants   = new Dictionary<string, Expression>();
		var operations  = new Dictionary<string, (string input1, string input2, string operation)>();
		var expressions = new Dictionary<string, Expression>();
		var parmExp     = Expression.Parameter(typeof(long), "humanInput");

		foreach (var match in input.Select(s => ParseRegex().Match(s))) {
			if (!match.Success) {
				throw new InvalidDataException("The regular expression failed to match");
			}

			if (match.Groups["constant"].Success) {
				constants.Add(match.Groups["name"].Value, Expression.Constant(long.Parse(match.Groups["constant"].Value)));
			} else {
				operations.Add(match.Groups["name"].Value, (match.Groups["input1"].Value, match.Groups["input2"].Value, match.Groups["op"].Value));
			}
		}

		constants.Remove("humn");
		constants.Add("humn", parmExp);

		while (operations.Count > 0) {
			var keys    = operations.Keys.ToList();

			foreach (var key in keys) {
				var op     = operations[key];
				var input1 = default(Expression);
				var input2 = default(Expression);

				if (constants.TryGetValue(op.input1, out var cinput1)) {
					input1 = cinput1;
				} else if (expressions.TryGetValue(op.input1, out var einput1)) {
					input1 = einput1;
				}

				if (constants.TryGetValue(op.input2, out var cinput2)) {
					input2 = cinput2;
				} else if (expressions.TryGetValue(op.input2, out var einput2)) {
					input2 = einput2;
				}

				if (input1 != null && input2 != null) {
					if (key == "root") {
						expressions.Add(key, Expression.Equal(input1, input2));
					} else {
						var opExpression = op.operation switch {
							"+" => Expression.Add(input1, input2),
							"-" => Expression.Subtract(input1, input2),
							"/" => Expression.Divide(input1, input2),
							"*" => Expression.Multiply(input1, input2),
							_ => throw new InvalidDataException("The specified operation is invalid"),
						};

						expressions.Add(key, TryReduce(opExpression));
					}

					operations.Remove(key);
				}
			}
		}

		return Simplify((BinaryExpression)expressions["root"]);
	}

	private static Expression TryReduce(Expression expression)
	{
		try {
			var result = Expression.Lambda<Func<long>>(expression).Compile().Invoke();
			return Expression.Constant(result);
		} catch (InvalidOperationException e) when (e.Message.StartsWith("variable 'humanInput'")) {
			return expression;
		}
	}

	private static long Simplify(BinaryExpression root)
	{
		var (constantValue, operation, remaining, _) = Deconstruct(root);

		if (operation != ExpressionType.Equal) {
			throw new ArgumentException("Specified expression must be an equality comparison expresion.", nameof(root));
		}

		while (remaining != null && remaining is BinaryExpression remainingBinary) {
			var (innerConstant, innerOperation, innerRemaining, leftIsConstant) = Deconstruct(remainingBinary);

			switch (innerOperation) {
				case ExpressionType.Multiply:
					constantValue /= innerConstant;
					break;

				case ExpressionType.Divide:
					if (leftIsConstant) {
						constantValue = innerConstant / constantValue;
					} else {
						constantValue *= innerConstant;
					}
					break;

				case ExpressionType.Add:
					constantValue -= innerConstant;
					break;

				case ExpressionType.Subtract:
					if (leftIsConstant) {
						constantValue = innerConstant - constantValue;
					} else {
						constantValue += innerConstant;
					}
					break;

				default:
					throw new NotSupportedException($"The operation type {innerOperation} is not supported.");
			}

			remaining = innerRemaining;
		}

		return constantValue;
	}

	private static (long constantValue, ExpressionType operation, Expression remaining, bool leftConstant) Deconstruct(BinaryExpression binaryExpression)
	{
		var constant    = default(ConstantExpression);
		var remaining   = default(Expression);
		var leftIsConst = false;

		if (binaryExpression.Left is ConstantExpression leftConst) {
			constant    = leftConst;
			remaining   = binaryExpression.Right;
			leftIsConst = true;
		} else if (binaryExpression.Right is ConstantExpression rightConst) {
			constant    = rightConst;
			remaining   = binaryExpression.Left;
			leftIsConst = false;
		}

		if (constant == null) {
			throw new InvalidOperationException("Neither expression is constant.");
		}

		if (remaining == null) {
			throw new InvalidOperationException("Non-constant side is null.");
		}

		if (constant.Value is not long constantValue) {
			throw new InvalidOperationException("The constant value must be of type long.");
		}

		return (constantValue, binaryExpression.NodeType, remaining, leftIsConst);
	}

	[GeneratedRegex(@"(?<name>[^:]+): ((?<constant>\d+)|(?<input1>[^:]+) (?<op>[\+\-/\*]) (?<input2>[^:]+))")]
	private static partial Regex ParseRegex();
}
