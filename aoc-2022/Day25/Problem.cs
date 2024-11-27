namespace AdventOfCode.Year2022.Day25;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Main, typeof(Problem));

	public static (string, string) Main(string[] input)
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

	[Theory]
	[MemberData(nameof(TestValues))]
	public void NumbersParseCorrectly(string snafu, long number)
	{
		Assert.Equal(number, ParseNumber(snafu));
	}

	[Theory]
	[MemberData(nameof(TestValues))]
	public void NumbersConvertCorrectly(string snafu, long number)
	{
		Assert.Equal(snafu, ConvertNumber(number));
	}

	public static IEnumerable<object[]> TestValues { get; } = new List<object[]>() {
		new object[] {             "1",         1 },
		new object[] {             "2",         2 },
		new object[] {            "1=",         3 },
		new object[] {            "1-",         4 },
		new object[] {            "10",         5 },
		new object[] {            "11",         6 },
		new object[] {            "12",         7 },
		new object[] {            "2=",         8 },
		new object[] {            "2-",         9 },
		new object[] {            "20",        10 },
		new object[] {           "1=0",        15 },
		new object[] {           "1-0",        20 },
		new object[] {         "2=-01",       976 },
		new object[] {        "1=11-2",      2022 },
		new object[] {       "1-0---0",     12345 },
		new object[] { "1121-1110-1=0", 314159265 },
	};
}
