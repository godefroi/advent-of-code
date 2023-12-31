namespace aoc_2022.Day07;

public class Problem
{
	public static ProblemMetadata2 Metadata { get; } = new(Main, typeof(Problem));

	private const int TOTAL_SPACE  = 70000000;
	private const int NEEDED_SPACE = 30000000;

	public static (long, long) Main(string[] input)
	{
		//fileName = "inputSample.txt";

		var root        = new Directory("/");
		var directories = new List<Directory>() { root };
		var current     = root;

		foreach (var line in input) {
			if (line == "$ cd /") {
				// already made the root directory
				current = root;
			} else if (line.StartsWith("$ cd ")) {
				// directory change
				var name = line[5..];
				current = name == ".." ? current!.Parent : current!.Children.Single(d => d.Name == name);
			} else if (line.StartsWith("dir ")) {
				// add a directory to this directory
				var dir = new Directory(line[4..]);
				directories.Add(dir);
				current!.AddChild(dir);
			} else if (line.StartsWith("$ ls")) {
				// do nothing, we ignore this
			} else {
				// add a file to this directory
				var parts = line.Split(' ', 2);
				current!.Files.Add(KeyValuePair.Create(parts[1], int.Parse(parts[0])));
			}
		}

		var part1      = directories.Where(d => d.Size <= 100000).Sum(d => d.Size);
		var needToFree = NEEDED_SPACE - (TOTAL_SPACE - root.Size);
		var part2      = directories.OrderBy(d => d.Size).First(d => d.Size >= needToFree).Size;

		return (part1, part2);
	}

	public class Directory
	{
		private readonly Lazy<int> _size;

		public Directory(string name)
		{
			Children = new List<Directory>();
			Files    = new List<KeyValuePair<string, int>>();
			Name     = name;

			_size = new(() => Files.Sum(f => f.Value) + Children.Sum(c => c.Size));
		}

		public string Name { get; }

		public Directory? Parent { get; private set; }

		public IList<Directory> Children { get; }

		public IList<KeyValuePair<string, int>> Files { get; }

		public int Size => _size.Value;

		public void AddChild(Directory child)
		{
			Children.Add(child);

			child.Parent = this;
		}
	}
}
