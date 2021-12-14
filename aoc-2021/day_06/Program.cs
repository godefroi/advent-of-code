var input   = File.ReadAllLines(args[0]).First().Split(',').Select(s => int.Parse(s)).ToList();
//var input   = "3,4,3,1,2".Split(',').Select(s => int.Parse(s)).ToList();
var buckets = Enumerable.Range(0, 9).Select(i => input.LongCount(n => n == i)).ToArray();

//Console.WriteLine(input.Count);
//Console.WriteLine($"Initial state: {string.Join(",", input)}");

for (var day = 0; day < 256; day++) {
	var nbuckets = new long[9];

	// age everyone one day
	Array.Copy(buckets, 1, nbuckets, 0, 8);

	// reset everyone who produced a fish
	nbuckets[6] += buckets[0];

	// add any new fish
	nbuckets[8] = buckets[0];
	buckets = nbuckets;

	//Console.WriteLine($"After {day + 1} days: {string.Join(",", buckets)}");
	if (day == 79) {
		Console.WriteLine($"part 1: {buckets.Sum()}"); // part 1 is 346063
	}
}

Console.WriteLine($"part 2: {buckets.Sum()}"); // part 2 is 1572358335990
