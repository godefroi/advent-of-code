namespace AdventOfCode.Year2022.Day08;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Main, typeof(Problem));

	public static (long, long) Main(string[] input)
	{
		var trees  = input.Select(Parse).SelectMany(t => t).ToArray();
		var rows   = new Dictionary<int, List<Tree>>();
		var cols   = new Dictionary<int, List<Tree>>();
		var width  = trees.Max(t => t.X) + 1;
		var height = trees.Max(t => t.Y) + 1;

		for (var y = 0; y < height; y++) {
			rows.Add(y, trees.Where(t => t.Y == y).OrderBy(t => t.X).ToList());
		}

		for (var x = 0; x < width; x++) {
			cols.Add(x, trees.Where(t => t.X == x).OrderBy(t => t.Y).ToList());
		}

		var visible = trees.Count(t => {
			// if the tree is on the edge, it's visible
			if (t.X == 0 || t.Y == 0 || t.X == width - 1 || t.Y == width - 1) {
				return true;
			}

			var row = rows[t.Y];
			var col = cols[t.X];

			// if no tree west is the same or taller, it's visible
			var hiddenWest  = row.Where(c => c.X < t.X).Any(c => c.Height >= t.Height);
			var hiddenEast  = row.Where(c => c.X > t.X).Any(c => c.Height >= t.Height);
			var hiddenNorth = col.Where(c => c.Y < t.Y).Any(c => c.Height >= t.Height);
			var hiddenSouth = col.Where(c => c.Y > t.Y).Any(c => c.Height >= t.Height);

			if (hiddenEast && hiddenNorth && hiddenWest && hiddenSouth) {
				return false;
			} else {
				return true;
			}
		});

		var scenicScore = trees.Max(t => {
			var row = rows[t.Y];
			var col = cols[t.X];

			var westVisibility  = CheckVisibility(row.Where(c => c.X < t.X).OrderByDescending(c => c.X), t).Count();
			var eastVisibility  = CheckVisibility(row.Where(c => c.X > t.X).OrderBy(c => c.X), t).Count();
			var northVisibility = CheckVisibility(col.Where(c => c.Y < t.Y).OrderByDescending(c => c.Y), t).Count();
			var southVisibility = CheckVisibility(col.Where(c => c.Y > t.Y).OrderBy(c => c.Y), t).Count();

			if (t.X == 2 && t.Y == 1) {
				Console.WriteLine($"tree at {t.X},{t.Y} has west {westVisibility}, east {eastVisibility}, north {northVisibility}, south {southVisibility}");
			}

			return westVisibility * eastVisibility * northVisibility * southVisibility;
		});

		return (visible, scenicScore);
	}

	private static IEnumerable<Tree> Parse(string line, int index)
	{
		for (var x = 0; x < line.Length; x++) {
			yield return new Tree(x, index, int.Parse(line.Substring(x, 1)));
		}
	}

	private static IEnumerable<Tree> CheckVisibility(IEnumerable<Tree> candidates, Tree from)
	{
		foreach (var candidate in candidates) {
			if (candidate.Height >= from.Height) {
				yield return candidate;
				yield break;
			} else {
				yield return candidate;
			}
		}
	}

	private readonly record struct Tree(int X, int Y, int Height);
}
