using aoc_tools;

using Combinatorics.Collections;

var input = (await PuzzleInput.GetInputLines()).Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => int.Parse(s)).ToList();

var pair = (new Combinations<int>(input, 2)).SingleOrDefault(c => c.Sum() == 2020);
var trip = (new Combinations<int>(input, 3)).SingleOrDefault(c => c.Sum() == 2020);

Console.WriteLine($"part 1: {Multiply(pair)}");
Console.WriteLine($"part 2: {Multiply(trip)}");

int Multiply(IEnumerable<int> input)
{
	var v1 = default(int?);

	foreach (var i in input) {
		if (v1 == null) {
			v1 = i;
		} else {
			v1 *= i;
		}
	}

	return v1 ?? 0;
}