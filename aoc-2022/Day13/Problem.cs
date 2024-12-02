using System.Text.Json.Nodes;

namespace AdventOfCode.Year2022.Day13;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		var idx     = 0;
		var part1   = 0;
		var div1    = new ListOrInteger(new[] {
				new ListOrInteger(new[] {
					new ListOrInteger(2)
				})
			});
		var div2    = new ListOrInteger(new[] {
				new ListOrInteger(new[] {
					new ListOrInteger(6)
				})
			});
		var packets = new List<ListOrInteger>() { div1, div2 };

		foreach (var (list1, list2) in ReadLists(input)) {
			idx++;

			if (list1.CompareTo(list2) < 0) {
				part1 += idx;
			}

			packets.Add(list1);
			packets.Add(list2);
		}

		packets.Sort();

		//foreach (var item in packets) {
		//	Console.WriteLine(item);
		//}

		var part2 = (packets.IndexOf(div1) + 1) * (packets.IndexOf(div2) + 1);

		return (part1, part2);
	}

	private static ListOrInteger Parse(string line)
	{
		var node = JsonNode.Parse(line);

		if (node == null) {
			throw new InvalidDataException("Unable to parse node");
		}

		return new ListOrInteger(node);
	}

	private static (ListOrInteger first, ListOrInteger second) Parse(IEnumerable<string> strings)
	{
		var items = strings.ToArray();

		if (items.Length != 2) {
			throw new InvalidDataException("Should have been two lists");
		}

		return (Parse(items[0]), Parse(items[1]));
	}

	private static IEnumerable<(ListOrInteger first, ListOrInteger second)> ReadLists(string[] input)
	{
		foreach (var chunk in ChunkByEmpty(input)) {
			if (chunk == null) {
				throw new InvalidDataException("Chunk cannot be null");
			}

			yield return Parse(chunk);
		}
	}

	internal class ListOrInteger : IComparable<ListOrInteger>
	{
		private readonly int?                 _integer;
		private readonly List<ListOrInteger>? _list;

		public ListOrInteger(int integer)
		{
			_integer = integer;
			_list    = null;
		}

		public ListOrInteger(List<ListOrInteger> list)
		{
			_integer = null;
			_list    = list;
		}

		public ListOrInteger(IEnumerable<ListOrInteger> list)
		{
			_integer = null;
			_list    = list.ToList();
		}

		public ListOrInteger(JsonNode jsonNode)
		{
			if (jsonNode is JsonValue value) {
				_integer = (int)value;
				_list    = null;
			} else if (jsonNode is JsonArray array) {
				_integer = null;
				_list    = array.Select(n => new ListOrInteger(n!)).ToList();
			}
		}

		public void Add(ListOrInteger item)
		{
			if (_list == null) {
				throw new InvalidOperationException("This instance is not a list.");
			}

			_list.Add(item);
		}

		public int CompareTo(ListOrInteger? other)
		{
			if (other == null) {
				throw new ArgumentNullException(nameof(other));
			}

			if (_integer.HasValue && other._integer.HasValue) {
				// both are integers; simple comparison
				return _integer.Value.CompareTo(other._integer.Value);
			} else if (_list != null && other._list != null) {
				// compare the lists directly
				return Compare(_list, other._list);
			} else if (_integer.HasValue && other._list != null) {
				// this instance is an integer, the other is a list; make a new list
				// of this one, and retry the comparison
				return Compare(new List<ListOrInteger>() { this }, other._list);
			} else if (_list != null && other._integer.HasValue) {
				// this instance is a list, the other is an integer; make a new list
				// of the other one, and retry the comparison
				return Compare(_list, new List<ListOrInteger>() { other });
			} else {
				throw new InvalidOperationException("Should not have been able to arrive here");
			}
		}

		public override string ToString()
		{
			if (_integer.HasValue) {
				return _integer.Value.ToString();
			} else if (_list != null) {
				var sb = new StringBuilder();

				sb.Append("[");
				sb.AppendJoin(',', _list.Select(i => i.ToString()));
				sb.Append("]");

				return sb.ToString();
			} else {
				throw new InvalidOperationException("This instance is neither value or list");
			}
		}

		private static int Compare(IReadOnlyList<ListOrInteger> list1, IReadOnlyList<ListOrInteger> list2)
		{
			// compare items in the lists
			for (var i = 0; i < list1.Count && i < list2.Count; i++) {
				var cmp = list1[i].CompareTo(list2[i]);

				if (cmp != 0) {
					return cmp;
				}
			}

			// existing items in the lists are equal, compare by count
			if (list1.Count < list2.Count) {
				return -1;
			} else if (list1.Count > list2.Count) {
				return 1;
			} else {
				return 0;
			}
		}
	}

	[Fact]
	public void ListsCompareCorrectly1()
	{
		var list1 = new ListOrInteger(new[] {
			new ListOrInteger(1),
			new ListOrInteger(1),
			new ListOrInteger(3),
			new ListOrInteger(1),
			new ListOrInteger(1),
		});

		var list2 = new ListOrInteger(new[] {
			new ListOrInteger(1),
			new ListOrInteger(1),
			new ListOrInteger(5),
			new ListOrInteger(1),
			new ListOrInteger(1),
		});

		Assert.True(list1.CompareTo(list2) < 0);
	}

	[Fact]
	public void ListsCompareCorrectly2()
	{
		var list1 = new ListOrInteger(new[] {
			new ListOrInteger(new[] {
				new ListOrInteger(1),
			}),
			new ListOrInteger(new[] {
				new ListOrInteger(2),
				new ListOrInteger(3),
				new ListOrInteger(4),
			}),
		});

		var list2 = new ListOrInteger(new[] {
			new ListOrInteger(new[] {
				new ListOrInteger(1),
			}),
			new ListOrInteger(4),
		});

		Assert.True(list1.CompareTo(list2) < 0);
	}

	[Fact]
	public void ListsCompareCorrectly3()
	{
		var list1 = new ListOrInteger(new[] {
			new ListOrInteger(9),
		});

		var list2 = new ListOrInteger(new[] {
			new ListOrInteger(new[] {
				new ListOrInteger(8),
				new ListOrInteger(7),
				new ListOrInteger(9),
			}),
		});

		Assert.True(list1.CompareTo(list2) > 0);
	}

	[Fact]
	public void SampleListsCompareCorrectly()
	{
		var results = new[] {
			true,
			true,
			false,
			true,
			false,
			true,
			false,
			false
		};

		foreach (var ((list1, list2), result) in ReadLists(ReadFileLines("inputSample.txt")).Zip(results)/*.Skip(2).Take(1)*/) {
			Assert.Equal(result, list1.CompareTo(list2) < 0);
		}
	}
}
