using aoc_tools;

using Combinatorics.Collections;

//var input = @"35
//20
//15
//25
//47
//40
//62
//55
//65
//95
//102
//117
//150
//182
//127
//219
//299
//277
//309
//576
//".Split(Environment.NewLine);

var input = (await PuzzleInput.GetInputLines());

var nums  = input.SkipLast(1).Select(s => long.Parse(s)).ToArray();
var inval = FindInvalid(nums, nums.Length > 100 ? 25 : 5);

Console.WriteLine($"part 1: {inval}"); // part 1 is 167829540
Console.WriteLine($"part 2: {FindSummingSet(nums, inval)}"); // part 2 is 28045630

long FindInvalid(long[] numbers, int preamble)
{
	var offset = 0;

	while (offset < numbers.Length - preamble) {
		if (!(new Combinations<long>(numbers[offset..(offset + preamble)], 2)).Any(l => l.Sum() == numbers[offset + preamble])) {
			return numbers[offset + preamble];
		}

		offset++;
	}

	throw new InvalidOperationException("No invalid number found.");
}

long FindSummingSet(long[] numbers, long value)
{
	// we'll brute-force the search by looking at each possible set length
	for (var len = 2; len < numbers.Length; len++) {
		// look through each set of size len
		for (var offset = 0; offset <= numbers.Length - len; offset++) {
			var seg = new ArraySegment<long>(numbers, offset, len);

			if (seg.Sum() == value) {
				return seg.Min() + seg.Max();
			}
		}
	}

	throw new InvalidOperationException("No summing set found.");
}