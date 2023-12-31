using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace AdventOfCode.Year2023.Day20;

public partial class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, null);

	public static (long, long) Execute(string[] input)
	{
		Span<Range>                ranges       = stackalloc Range[10];
		Span<char>                 splitChars   = ['-', '>', ','];
		Dictionary<string, Module> modules      = [];
		HashSet<string>            conjunctions = [];
		Queue<Pulse>               pulses       = [];

		var lowCount  = 0L;
		var highCount = 0L;

		// parse the modules out of the file
		for (var i = 0; i < input.Length; i++) {
			var lineSpan       = input[i].AsSpan();
			var itemCount      = lineSpan.SplitAny(ranges, splitChars, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
			var nameRange      = lineSpan[ranges[0]];
			var destinations   = new string[itemCount - 1];

			for (var j = 0; j < destinations.Length; j++) {
				destinations[j] = lineSpan[ranges[j + 1]].ToString();
			}

			switch (nameRange[0]) {
				case '%':
					modules.Add(nameRange[1..].ToString(), new FlipFlop() { Destinations = [.. destinations] });
					break;

				case '&':
					var cn = nameRange[1..].ToString();
					var cm = new Conjunction() { Destinations = [.. destinations] };

					modules.Add(cn, cm);
					conjunctions.Add(cn);
					break;

				case 'b':
					modules.Add("broadcaster", new Broadcaster() { Destinations = [.. destinations] });
					break;
			}
		}

		// for the conjunctions, pre-load their list of sources
		foreach (var kvp in modules) {
			for (var i = 0; i < kvp.Value.Destinations.Length; i++) {
				if (conjunctions.Contains(kvp.Value.Destinations[i])) {
					modules[kvp.Value.Destinations[i]].ProcessPulse(PulseType.Low, kvp.Key);
				}
			}
		}

		// now, part 2 requires a bit of knowledge about the structure of the
		// graph; essentially, there are four "loops", each of which output to
		// a conjunction, which merges their output into a single conjunction,
		// which sends a pulse to the rx module.

		// we'll have a graph structure like this:
		// loop 1-(conjunction sg)┐
		// loop 2-(conjunction lm)┤
		//                        ├─(conjunction jm)─(rx)
		// loop 3-(conjunction dh)┤
		// loop 4-(conjunction db)┘

		// so, for rx to get a "low", the jm conjunction must get a "high" from
		// all its inputs, so we need to know what they are

		var rxSource    = GetSources(modules, "rx").Single();
		var jmSources   = GetSources(modules, rxSource);
		var loopCenters = jmSources.Select(s => GetSources(modules, s)).SelectMany(s => s).ToList();

		// the input has been very carefully crafted; there is no general solution,
		// but because our puzzle master is a benevolent overlord, we get to use
		// a least common multiple of the four loop periods to calculate the output
		// period.
		var loopCounts = loopCenters.ToDictionary(n => n, n => new List<int>());

		// Console.WriteLine("Conjunctions:");
		// foreach (var c in conjunctions) {
		// 	Console.WriteLine($"  {c} ({string.Join(',', ((Conjunction)modules[c]).GetSources())})");
		// }

		for (var i = 0; i < 15000; i++) {
			pulses.Enqueue(new Pulse(PulseType.Low, "button", "broadcaster"));

			if (i < 1000) {
				lowCount += 1;
			}

			while (pulses.Count > 0) {
				var (type, from, to) = pulses.Dequeue();
				var destModule       = modules.GetValueOrDefault(to);

				if (destModule == null) {
					continue;
				}

				var obType = destModule.ProcessPulse(type, from);

				//Console.WriteLine($"{from} -{type.ToString().ToLowerInvariant()}-> {to}");

				if (obType == PulseType.Low && loopCounts.TryGetValue(to, out var theList)) {
					theList.Add(i);
				}

				// we can exit early if we have enough data
				if (loopCounts.All(c => c.Value.Count >= 2) && i > 1000) {
					break;
				}

				switch (obType) {
					case PulseType.None:
						continue;

					case PulseType.High when i < 1000:
						highCount += destModule.Destinations.Length;
						break;

					case PulseType.Low when i < 1000:
						lowCount += destModule.Destinations.Length;
						break;

					default:
						break;
				}

				foreach (var d in destModule.Destinations) {
					pulses.Enqueue(new Pulse(obType, to, d));
				}
			}
		}

		// foreach (var (dest, list) in loopCounts) {
		// 	Console.WriteLine(dest);
		// 	var p = 0;
		// 	foreach (var cnt in list) {
		// 		Console.WriteLine($"  {cnt} {(p > 0 ? cnt - p : -1)}");
		// 		p = cnt;
		// 	}
		// }

		return (lowCount * highCount, LeastCommonMultiple(loopCounts.Select(s => (long)s.Value[1] - s.Value[0]).ToArray()));
	}

	private static List<string> GetSources(Dictionary<string, Module> modules, string moduleName) => modules
		.Where(kvp => kvp.Value.Destinations.Contains(moduleName))
		.Select(kvp => kvp.Key)
		.ToList();

	private enum PulseType
	{
		None,
		High,
		Low,
	}

	private readonly record struct Pulse(PulseType PulseType, string Source, string Destination);

	private abstract class Module
	{
		public required ImmutableArray<string> Destinations { get; init; }

		public abstract PulseType ProcessPulse(PulseType pulseType, string fromModule);
	}

	private class FlipFlop : Module
	{
		private bool _currentState = false;

		public override PulseType ProcessPulse(PulseType pulseType, string fromModule)
		{
			if (pulseType == PulseType.High) {
				return PulseType.None;
			}

			_currentState = !_currentState;

			return _currentState ? PulseType.High : PulseType.Low;
		}
	}

	private class Conjunction : Module
	{
		private readonly Dictionary<string, PulseType> _lastPulse = [];

		public IEnumerable<string> GetSources() => _lastPulse.Keys;

		public override PulseType ProcessPulse(PulseType pulseType, string fromModule)
		{
			//Console.WriteLine($"conjunction processing {pulseType} from {fromModule}");
			//Console.WriteLine($"  our state is {string.Join(',', _lastPulse.Select(kvp => $"{kvp.Key}({kvp.Value})"))}");
			_lastPulse[fromModule] = pulseType;

			return _lastPulse.Values.All(v => v == PulseType.High) ? PulseType.Low : PulseType.High;
		}
	}

	private class Broadcaster : Module
	{
		public override PulseType ProcessPulse(PulseType pulseType, string fromModule) => pulseType;
	}
}
