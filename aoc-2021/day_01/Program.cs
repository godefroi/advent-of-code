using System.Net;

using day_01;

var cc = new CookieContainer();

cc.Add(new Cookie("session", "53616c7465645f5f15c3b399a1b4cb0c87c1d92afa8cf82daccd8ec71cdd9c80baeeda5ef47e2aeaf23a8ba42f97c022", "/", "adventofcode.com"));

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
