using aoc_tools;

//var input = @"6,10
//0,14
//9,10
//0,3
//10,4
//4,11
//6,0
//6,12
//4,1
//0,13
//10,12
//3,4
//3,0
//8,4
//1,10
//2,14
//8,10
//9,0

//fold along y=7
//fold along x=5
//".Split(Environment.NewLine).SkipLast(1).ToList();

//var input = (await PuzzleInput.GetInputLines()).SkipLast(1).ToList();
var input = (await PuzzleInput.GetInputLines("input_two_point_five.txt")).SkipLast(1).ToList();
var dots  = input.Where(i => i.IndexOf(',') > -1).Select(l => l.Split(',')).Select(p => new Dot(int.Parse(p[0]), int.Parse(p[1]))).ToHashSet();
var folds = input.Where(i => i.StartsWith("fold")).Select(i => i.Split(' ', '=')).Select(p => (Dimension: p[2][0], Value: int.Parse(p[3]))).ToList();

Console.WriteLine($"x: {dots.Max(d => d.X)} y: {dots.Max(d => d.Y)}");
return;

Fold(dots, folds.First());

Console.WriteLine($"part 1: {dots.Distinct().Count()}"); // part 1 is 720

foreach (var fold in folds.Skip(1)) {
	Fold(dots, fold);
}

var width = dots.Max(d => d.X);

foreach (var row in dots.GroupBy(d => d.Y).OrderBy(g => g.Key)) {
	var lit = row.Select(d => d.X).ToList();
	Console.WriteLine(new string(Enumerable.Range(0, width + 1).Select(x => row.Any(d => d.X == x) ? '#' : ' ').ToArray()));
} // part 2 is AHPRPAUZ

void Fold(HashSet<Dot> dots, (char Dimension, int Value) fold)
{
	if (fold.Dimension == 'y') {
		// horizontal fold
		var to_fold = dots.Where(d => d.Y > fold.Value).ToHashSet();
		dots.RemoveWhere(d => to_fold.Contains(d));
		foreach (var dot in to_fold.Select(dot => new Dot(dot.X, fold.Value - (dot.Y - fold.Value)))) {
			dots.Add(dot);
		}
	} else if (fold.Dimension == 'x') {
		var to_fold = dots.Where(d => d.X > fold.Value).ToHashSet();
		dots.RemoveWhere(d => to_fold.Contains(d));
		foreach (var dot in to_fold.Select(dot => new Dot(fold.Value - (dot.X - fold.Value), dot.Y))) {
			dots.Add(dot);
		}
	} else {
		throw new InvalidOperationException($"Cannot fold along dimension {fold.Dimension}");
	}
}
record struct Dot(int X, int Y);

record struct Fold(char Dimension, int Value);
