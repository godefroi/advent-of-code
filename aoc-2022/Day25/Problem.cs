namespace AdventOfCode.Year2022.Day25;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (string, string) Execute(string[] input)
	{
		return (ConvertNumber(input.Select(ParseNumber).Sum()), string.Empty);
	}

	private static long ParseNumber(string number)
	{
		var pow = 0;
		var ret = 0L;

		foreach (var digit in Enumerable.Range(0, number.Length).Select(idx => number.Substring(number.Length - 1 - idx, 1))) {
			var value = digit switch {
				"-" => -1,
				"=" => -2,
				_ => long.Parse(digit),
			};

			var place = pow switch {
				0 => 1,
				_ => (long)Math.Pow(5, pow),
			};

			ret += value * place;
			pow++;
		}

		return ret;
	}

	private static string ConvertNumber(long number)
	{
		var digits = new LinkedList<char>();

		while (number > 0) {
			long remainder;

			(number, remainder) = Math.DivRem(number, 5);

			switch (remainder) {
				case 0:
					digits.AddFirst('0');
					break;
				case 1:
					digits.AddFirst('1');
					break;
				case 2:
					digits.AddFirst('2');
					break;
				case 3:
					number += 1;
					digits.AddFirst('=');
					break;
				case 4:
					number += 1;
					digits.AddFirst('-');
					break;
				default:
					throw new InvalidDataException("shouldn't have seen this");
			}
		}

		return new string(digits.ToArray());
	}

	public static IEnumerable<(string snafu, long number)> GetTestValues()
	{
		yield return ("1",             1);
		yield return ("2",             2);
		yield return ("1=",            3);
		yield return ("1-",            4);
		yield return ("10",            5);
		yield return ("11",            6);
		yield return ("12",            7);
		yield return ("2=",            8);
		yield return ("2-",            9);
		yield return ("20",            10);
		yield return ("1=0",           15);
		yield return ("1-0",           20);
		yield return ("2=-01",         976);
		yield return ("1=11-2",        2022);
		yield return ("1-0---0",       12345);
		yield return ("1121-1110-1=0", 314159265);
	}

	[Test]
	[MethodDataSource(nameof(GetTestValues))]
	public async Task NumbersParseCorrectly(string snafu, long number)
	{
		await Assert.That(ParseNumber(snafu)).IsEqualTo(number);
	}

	[Test]
	[MethodDataSource(nameof(GetTestValues))]
	public async Task NumbersConvertCorrectly(string snafu, long number)
	{
		await Assert.That(ConvertNumber(number)).IsEqualTo(snafu);
	}
}
