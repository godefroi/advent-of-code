using BenchmarkDotNet.Attributes;

using System.Collections.Specialized;

namespace aoc_2019.Benchmarks;

public class HashSetVsBitMask
{
	private readonly HashSet<char> _hashSet = new HashSet<char>() {
		'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h'
	};

	private readonly BitVector32 _bitVector = new BitVector32();

	[Benchmark]
	public bool HashSetContains() => _hashSet.Contains('a') && _hashSet.Contains('z');

	[Benchmark]
	public bool BitVectorContains() => _bitVector['a' - 'a'] && _bitVector['z' - 'a'];
}
