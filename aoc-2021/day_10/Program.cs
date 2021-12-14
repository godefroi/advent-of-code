var input  = File.ReadAllLines(args[0]).ToList();
var starts = new List<char>() { '(', '[', '{', '<' };
var ends   = new List<char>() { ')', ']', '}', '>' };
var scores = input.Select(l => ScoreLine(l)).ToList();

Console.WriteLine($"part 1: {scores.Where(s => s > 0).Sum()}"); // part 1 is 296535

scores = scores.Where(s => s < 0).OrderBy(s => s).ToList();

Console.WriteLine($"part 2: {Math.Abs(scores[scores.Count / 2])}"); // part 2 is 4245130838

long ScoreLine(string line)
{
	var stack = new Stack<char>();

	for (var pos = 0; pos < line.Length; pos++) {
		if (starts.Contains(line[pos])) {
			stack.Push(line[pos]);
		} else if (ends.Contains(line[pos])) {
			var c = stack.Pop();
			if (starts.IndexOf(c) != ends.IndexOf(line[pos])) {
				//Console.WriteLine($"invalid char at position {pos} for this line: expected {ends[starts.IndexOf(c)]}, found {line[pos]}");
				return line[pos] switch {
					')' => 3, ']' => 57, '}' => 1197, '>' => 25137, _ => throw new InvalidOperationException($"Cannot score that character.")
				};
			}
		}
	}

	var score = 0L;

	foreach (var c in stack) {
		score *= 5;
		score += c switch { '(' => 1, '[' => 2, '{' => 3, '<' => 4, _ => throw new InvalidOperationException("No start for that.") };
	}

	return -score;
}
