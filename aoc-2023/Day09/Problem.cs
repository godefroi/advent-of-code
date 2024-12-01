using System.Buffers;
using BenchmarkDotNet.Attributes;

namespace AdventOfCode.Year2023.Day09;

public partial class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), typeof(Benchmarks));

	public static (long, long) Execute(string[] input)
	{
		var values = input.AsParallel()
			.Select(i => i.Split(' ').Select(long.Parse).ToArray())
			.Select(Extrapolate)
			.Aggregate((0L, 0L), (acc, val) => (acc.Item1 + val.pre, acc.Item2 + val.post));

		// var values = input
		// 	.Select(i => i.Split(' ').Select(long.Parse).ToArray())
		// 	.Select(Extrapolate2)
		// 	.Aggregate((0L, 0L), (acc, val) => (acc.Item1 + val.pre, acc.Item2 + val.post));

		return values;
	}

	private static (long post, long pre) Extrapolate(long[] sequence)
	{
		var arrays  = new Stack<long[]>();
		var prevArr = sequence;
		var extPre  = 0L;
		var extPost = 0L;

		arrays.Push(sequence);

		while (true) {
			// transform it
			var thisArr = TransformSequence(prevArr);

			// push this array onto the stack
			arrays.Push(thisArr);

			// if it's all zeroes, we can move on to extrapolation
			if (thisArr.All(l => l == 0)) {
				break;
			}

			// otherwise, do it again
			prevArr = thisArr;
		}

		while (arrays.Count > 0) {
			// get the top array on the stack
			var thisArr = arrays.Pop();

			// extrapolate the pre- value for this array
			extPre = thisArr[0] - extPre;

			// extrapolate the post- value for this array
			extPost = thisArr[^1] + extPost;
		}

		return (extPre, extPost);
	}

	private static (long post, long pre) Extrapolate2(long[] sequence)
	{
		var arrays  = new Stack<(long[] array, int length)>();
		var extPre  = 0L;
		var extPost = 0L;

		(long[] array, int length) prevArr = (sequence, sequence.Length);

		arrays.Push(prevArr);

		while (true) {
			// make a new array
			var newLen  = prevArr.length - 1;
			var newArr  = ArrayPool<long>.Shared.Rent(newLen);
			var allZero = true;

			// transform the previous array into the new array
			for (var i = 0; i < newLen; i++) {
				newArr[i] = prevArr.array[i + 1] - prevArr.array[i];

				// check for zeroes while we're at it
				if (newArr[i] != 0) {
					allZero = false;
				}
			}

			// push this array onto the stack
			prevArr = (newArr, newLen);
			arrays.Push(prevArr);

			// if it's all zeroes, we can move on to extrapolation
			if (allZero) {
				break;
			}
		}

		while (arrays.Count > 0) {
			// get the top array on the stack
			var (thisArr, thisLen) = arrays.Pop();

			//Console.WriteLine($"({thisLen}) {string.Join(", ", thisArr)}");

			// extrapolate the pre- value for this array
			extPre = thisArr[0] - extPre;

			// extrapolate the post- value for this array
			extPost = thisArr[thisLen - 1] + extPost;

			// return the array to the pool
			if (arrays.Count > 1) {
				ArrayPool<long>.Shared.Return(thisArr);
			}
		}

		return (extPre, extPost);
	}

	private static long[] TransformSequence(long[] sequence) => Enumerable.Range(0, sequence.Length - 1).Select(i => sequence[i + 1] - sequence[i]).ToArray();

	public class Benchmarks
	{
		[Benchmark]
		public void ExtrapolateWithoutPool()
		{
			long[] longs = [10L, 13L, 16L, 21L, 30L, 45L, 68L];

			Extrapolate(longs);
		}

		[Benchmark]
		public void ExtrapolateWithPool()
		{
			long[] longs = [10L, 13L, 16L, 21L, 30L, 45L, 68L];

			Extrapolate2(longs);
		}
	}
}
