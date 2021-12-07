using System.Text.RegularExpressions;

using aoc_tools;

//var input = @"abc

//a
//b
//c

//ab
//ac

//a
//a
//a
//a

//b
//".TrimEnd().ReplaceLineEndings().Split($"{Environment.NewLine}{Environment.NewLine}").ToList();

var input = (await PuzzleInput.GetInput()).TrimEnd().ReplaceLineEndings().Split($"{Environment.NewLine}{Environment.NewLine}").ToList();

Console.WriteLine($"part 1: {input.Sum(l => Regex.Replace(l, @"[^a-z]", string.Empty).Distinct().Count())}"); // part 1 is 7110
Console.WriteLine($"part 2: {input.Sum(l => { var rcnt = 0; var chars = Regex.Replace(l, @"[^a-z]", m => { rcnt++; return string.Empty; }); return chars.GroupBy(c => c).Where(g => g.Count() == (rcnt / 2) + 1).Count(); })}"); // part 2 is 3628
