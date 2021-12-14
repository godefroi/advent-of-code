using day_01;

var input = File.ReadAllText(args[0]);
var last  = int.MaxValue;
var lSum  = int.MaxValue;
var inc   = 0;
var winc  = 0;
var win   = new SlidingWindow(3);

using var sr = new StringReader(input);

while (sr.Peek() > -1) {
	var cur = int.Parse(sr.ReadLine());

	// add to the window
	win.Add(cur);

	if (cur > last) {
		inc++;
	}

	// if our window is full, then handle that
	if (win.Count() == 3) {
		var curSum = win.Sum();

		if (curSum > lSum) {
			winc++;
		}

		lSum = curSum;
	}

	last = cur;
}

Console.WriteLine($"part 1: {inc}");
Console.WriteLine($"part 2: {winc}");
