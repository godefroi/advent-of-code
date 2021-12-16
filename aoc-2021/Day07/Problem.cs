namespace Day07;

public class Problem
{
	public static void Main(string[] args)
	{
		var input = File.ReadAllLines(args[0]).First().Split(',').Select(s => int.Parse(s)).ToList();

		Console.WriteLine($"part 1: {input.Min(i => input.Sum(ii => Math.Abs(ii - i)))}"); // part 1 is 329389
		Console.WriteLine($"part 2: {Enumerable.Range(input.Min(), input.Max()).Min(i => input.Sum(ii => TriangleNumber(ii - i)))}"); // part 2 is 86397080

		static int TriangleNumber(int i)
		{
			var ret = 0;

			i = Math.Abs(i);

			for (var idx = i; idx > 0; idx--) {
				ret += idx;
			}

			return ret;
		}

		static long BinomialCoefficient(long n, long k)
		{
			if (k > n) {
				return 0;
			}

			if (n == k) {
				return 1; // only one way to chose when n == k
			}

			if (k > n - k) {
				k = n - k; // Everything is symmetric around n-k, so it is quicker to iterate over a smaller k than a larger one.
			}

			long c = 1;

			for (long i = 1; i <= k; i++) {
				c *= n--;
				c /= i;
			}

			return c;
		}
	}
}
