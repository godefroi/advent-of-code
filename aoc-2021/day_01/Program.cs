using System.Net;

using day_01;

if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("AOC_SESSION"))) {
	throw new InvalidOperationException("Set the AOC_SESSION environment variable.");
}

var cc = new CookieContainer();

cc.Add(new Cookie("session", Environment.GetEnvironmentVariable("AOC_SESSION"), "/", "adventofcode.com"));

using var handler = new HttpClientHandler() {  CookieContainer = cc };
using var hc      = new HttpClient(handler);

var input = await hc.GetStringAsync("https://adventofcode.com/2021/day/1/input");
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
