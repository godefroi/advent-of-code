using System.Collections.Concurrent;

namespace AdventOfCode.Year2019.Day07;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		var program = input.Single();

		return (Part1(program), Part2(program));
	}

	public static long Part1(string programString)
	{
		var cur   = long.MinValue;
		var perms = new long[] { 0, 1, 2, 3, 4 }.Permutations(5);

		foreach (var perm in perms) {
			var inp = 0L;

			foreach (var phase in perm) {
				var inputs = new Queue<long>(new[] { phase, inp });
				var comp   = new IntcodeComputer(programString);
				var output = default(long?);

				comp.Input += (s, e) => inputs.Dequeue();
				comp.Output += (s, e) => output = e.OutputValue;

				comp.Resume();

				inp = output ?? throw new InvalidOperationException("No output was received");
			}

			if (cur < inp) {
				cur = inp;
			}
		}

		return cur;
	}

	public static long Part2(string programString)
	{
		var cur = long.MinValue;

		foreach (var inps in new long[] { 5, 6, 7, 8, 9 }.Permutations(5).Select(p => p.ToArray())) {
			//Console.WriteLine($"trying {string.Join(',', inps)}");

			var amps     = Enumerable.Range(0, 5).Select(i => new IntcodeComputer(programString)).ToArray();
			var links    = Enumerable.Range(0, 5).Select(i => new BlockingCollection<long>()).ToArray();
			var finalOut = long.MinValue;

			for (var i = 0; i < amps.Length; i++) {
				var outIdx  = i;
				var inIdx   = (i == 0 ? links.Length : i) - 1;
				var amp     = amps[i];
				var outLink = links[i];
				var inLink  = links[inIdx];

				//Console.WriteLine($"Amp {i} outputs to {i} and inputs from {inIdx}");

				// input the phase setting for this amp
				inLink.Add(inps[i]);

				// each amp outputs to the corresponding link, and takes from the link previous to it
				amp.Output += (s, e) => {
					//Console.WriteLine($"Amp {outIdx} is outputting value {e.OutputValue} to link {outIdx}");
					outLink.Add(e.OutputValue);
				};

				// the final amp also needs to update the final output value
				if (i == amps.Length - 1) {
					amp.Output += (s, e) => finalOut = e.OutputValue;
				}

				amp.Input += (s, e) => {
					//Console.WriteLine($"Amp {outIdx} is inputting value from {inIdx}");
					var val = inLink.Take();
					//Console.WriteLine($"Amp {outIdx} got value {val} from {inIdx}");
					return val;
				};
			}

			// the final link (that inputs into the first amp) needs the initial value (0)
			links[4].Add(0);

			// create tasks to execute all the amps
			var tasks = amps.Select(amp => Task.Factory.StartNew(amp.Resume)).ToArray();

			// wait for all the programs to finish
			Task.WaitAll(tasks);

			if (finalOut > cur) {
				cur = finalOut;
			}
		}

		return cur;
	}
}
