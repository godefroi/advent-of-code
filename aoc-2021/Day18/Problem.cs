using Xunit;

namespace Day18;

public class Problem
{
	internal static void Main(string fileName)
	{
		// The largest magnitude of the sum of any two snailfish numbers in this list is 3993. This is the magnitude of  + , which reduces to 
		//var i_l = "[[2,[[7,7],7]],[[5,8],[[9,3],[0,2]]]]".Select(c => ParseComponent(c)).ToList();
		//var i_r = "[[[0,[5,8]],[[1,7],[9,6]]],[[4,[1,2]],[[1,4],2]]]".Select(c => ParseComponent(c)).ToList();
		//var sum = Add(i_l, i_r);

		//Reduce(sum);
		//Console.WriteLine(Stringify(sum));
		//Console.WriteLine(Stringify(sum) == "[[[[7,8],[6,6]],[[6,0],[7,7]]],[[[7,8],[8,8]],[[7,9],[0,6]]]]");

		//return;
		var inputs = File.ReadAllLines(fileName);
		var left   = inputs[0].Select(c => ParseComponent(c)).ToList();

		foreach (var l in inputs.Skip(1)) {
			var right = l.Select(c => ParseComponent(c)).ToList();

			left = Add(left, right);

			Reduce(left);
		}

		Console.WriteLine($"part 1: {Magnitude(left)}"); // part 1 is 3981 on main input

		var max = 0L;

		for (var i = 0; i < inputs.Length; i++) {
			for (var j = 0; j < inputs.Length; j++) {
				if (inputs[i] == inputs[j]) {
					continue;
				}

				var l = inputs[i].Select(c => ParseComponent(c)).ToList();
				var r = inputs[j].Select(c => ParseComponent(c)).ToList();

				var s = Add(l, r);
				Reduce(s);

				var m1 = Magnitude(s);

				s = Add(r, l);
				Reduce(s);

				var m2 = Magnitude(s);

				max = Math.Max(max, m1);
				max = Math.Max(max, m2);
			}
		}

		Console.WriteLine($"part 2: {max}"); // part 2 is 4687
		//var input = "[1,2]";

		//var node = ParseNode("[[[[[9,8],1],2],3],4]".ToCharArray()).Item1;

		//var components = "[1,2]".Select(c => ParseComponent(c)).ToList();

		//Console.WriteLine(Magnitude("[[9,1],[1,9]]".Select(c => ParseComponent(c)).ToList()));
	}

	private static void Print(IEnumerable<Component> components) => Console.WriteLine(Stringify(components));

	private static string Stringify(IEnumerable<Component> components)
	{
		var sb = new System.Text.StringBuilder();

		foreach (var component in components) {
			sb.Append(component.ComponentType switch {
				ComponentType.PairStart => "[",
				ComponentType.PairEnd => "]",
				ComponentType.Divider => ",",
				ComponentType.Value => component.Value.ToString(),
				_ => throw new Exception("Unsupported component type"),
			});
		}

		return sb.ToString();
	}

	private static long Magnitude(List<Component> components)
	{
		var (node, _) = ParseNode(Stringify(components).ToCharArray());

		return Magnitude(node);
	}

	private static long Magnitude(Node node)
	{
		if (node.Value.HasValue) {
			return node.Value.Value;
		} else {
			return (Magnitude(node.Left) * 3) + (Magnitude(node.Right) * 2);
		}
	}

	private static (Node?, int) ParseNode(ArraySegment<char> rest)
	{
		var ret = new Node();
		var pos = 0;

		while (true) {
			Node? child = null;

			switch (rest[pos]) {
				case '[':
					(child, var inc) = ParseNode(rest[(pos + 1)..]);
					pos += inc;
					break;

				case ']':
					return (ret, pos + 1);

				case ',':
					break;

				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
					child = new Node(int.Parse(rest[pos].ToString()));
					break;
			}

			pos += 1;

			if (pos >= rest.Count) {
				return (child, pos);
			}

			if (ret.Left == null) {
				ret.Left = child;
			} else {
				ret.Right = child;
			}
		}

		throw new Exception("Should not have arrived here.");
	}

	private static Node Add(Node left, Node right)
	{
		var ret = new Node(left, right);

		var depth = 0;
		var cur   = ret;

		while (cur != null) {
			cur = cur.Left;
			depth += 1;
		}

		return ret;
	}

	private static List<Component> Add(List<Component> left, List<Component> right)
	{
		var ret = new List<Component>();

		ret.Add(new Component(ComponentType.PairStart, null));
		ret.AddRange(left);
		ret.Add(new Component(ComponentType.Divider, null));
		ret.AddRange(right);
		ret.Add(new Component(ComponentType.PairEnd, null));

		return ret;
	}

	private static List<Component> Reduce(List<Component> list)
	{
		while (true) {
			if (Explode(list)) {
				continue;
			}

			if (Split(list)) {
				continue;
			}

			break;
		}

		return list;
	}

	private static bool Explode(List<Component> list)
	{
		var depth = 0;

		for (var i = 0; i < list.Count; i++) {
			if (list[i].ComponentType == ComponentType.PairStart) {
				depth++;
			} else if (list[i].ComponentType == ComponentType.PairEnd) {
				depth--;
			} else if (list[i].ComponentType == ComponentType.Divider && depth > 4) {
				if (list[i - 1].ComponentType == ComponentType.Value && list[i + 1].ComponentType == ComponentType.Value) {
					// explode this pair
					//Console.WriteLine($"Explode pair [{list[i - 1].Value},{list[i + 1].Value}]");

					// add the left to the first number to the left
					for (var j = i - 2; j >= 0; j--) {
						if (list[j].ComponentType == ComponentType.Value) {
							list[j] = new Component(ComponentType.Value, list[j].Value + list[i - 1].Value);
							break;
						}
					}

					// add the right to the first number to the right
					for (var j = i + 2; j < list.Count; j++) {
						if (list[j].ComponentType == ComponentType.Value) {
							list[j] = new Component(ComponentType.Value, list[j].Value + list[i + 1].Value);
							break;
						}
					}

					// replace the exploded pair with zero
					list.RemoveRange(i - 2, 5);
					list.Insert(i - 2, new Component(ComponentType.Value, 0));

					return true;
				}
			}
		}

		return false;
	}

	private static bool Split(List<Component> list)
	{
		for (var i = 0; i < list.Count; i++) {
			if (list[i].ComponentType == ComponentType.Value && list[i].Value > 9) {
				var val = list[i].Value;

				list.RemoveAt(i);

				list.Insert(i + 0, new Component(ComponentType.PairStart, null));
				list.Insert(i + 1, new Component(ComponentType.Value, (int)Math.Floor(val!.Value / 2f)));
				list.Insert(i + 2, new Component(ComponentType.Divider, null));
				list.Insert(i + 3, new Component(ComponentType.Value, (int)Math.Ceiling(val!.Value / 2f)));
				list.Insert(i + 4, new Component(ComponentType.PairEnd, null));

				return true;
			}
		}

		return false;
	}

	private static (int? AddToLeft, int? AddToRight) Explode(Node node)
	{
		return (null, null);
	}

	private static Component ParseComponent(char character) => (character) switch {
		'[' => new Component(ComponentType.PairStart, null),
		']' => new Component(ComponentType.PairEnd, null),
		',' => new Component(ComponentType.Divider, null),
		'0' => new Component(ComponentType.Value, 0),
		'1' => new Component(ComponentType.Value, 1),
		'2' => new Component(ComponentType.Value, 2),
		'3' => new Component(ComponentType.Value, 3),
		'4' => new Component(ComponentType.Value, 4),
		'5' => new Component(ComponentType.Value, 5),
		'6' => new Component(ComponentType.Value, 6),
		'7' => new Component(ComponentType.Value, 7),
		'8' => new Component(ComponentType.Value, 8),
		'9' => new Component(ComponentType.Value, 9),
		_ => throw new InvalidOperationException($"Invalid character"),
	};

	[Fact(Skip = "Not used in the current implementation")]
	public void NodesParseCorrectly()
	{
		var strings = new[] {
			"[1,2]",
			"[[1,2],3]",
			"[9,[8,7]]",
			"[[1,9],[8,5]]",
			"[[[[1,2],[3,4]],[[5,6],[7,8]]],9]",
			"[[[9,[3,8]],[[0,9],6]],[[[3,7],[4,9]],3]]",
			"[[[[1,3],[5,3]],[[1,3],[8,7]]],[[[4,9],[6,9]],[[8,2],[7,3]]]]" };

		for (var i = 0; i < strings.Length; i++) {
			Assert.Equal(strings[i], ParseNode(strings[i].ToCharArray()).Item1.ToString());
		}
	}

	[Theory]
	[InlineData("[[[[0,9],2],3],4]", "[[[[[9,8],1],2],3],4]")]
	[InlineData("[7,[6,[5,[7,0]]]]", "[7,[6,[5,[4,[3,2]]]]]")]
	[InlineData("[[6,[5,[7,0]]],3]", "[[6,[5,[4,[3,2]]]],1]")]
	[InlineData("[[3,[2,[8,0]]],[9,[5,[4,[3,2]]]]]", "[[3,[2,[1,[7,3]]]],[6,[5,[4,[3,2]]]]]")]
	[InlineData("[[3,[2,[8,0]]],[9,[5,[7,0]]]]", "[[3,[2,[8,0]]],[9,[5,[4,[3,2]]]]]")]
	public void ExplodeWorksCorrectly(string expected, string number)
	{
		var list = number.Select(c => ParseComponent(c)).ToList();

		Assert.True(Explode(list));
		Assert.Equal(expected, Stringify(list));
	}

	[Theory]
	[InlineData("[[5,5],0]", 10)]
	[InlineData("[[5,6],0]", 11)]
	[InlineData("[[6,6],0]", 12)]
	public void SplitWorksCorrectly(string expected, int number)
	{
		var list = new List<Component>() {
			new Component(ComponentType.PairStart, null),
			new Component(ComponentType.Value, number),
			new Component(ComponentType.Divider, null),
			new Component(ComponentType.Value, 0),
			new Component(ComponentType.PairEnd, null)
		};

		Assert.True(Split(list));
		Assert.Equal(expected, Stringify(list));
	}

	[Fact]
	public void AddWorksCorrectly()
	{
		var left  = "[[[[4,3],4],4],[7,[[8,4],9]]]".Select(c => ParseComponent(c)).ToList();
		var right = "[1,1]".Select(c => ParseComponent(c)).ToList();
		var list  = Add(left, right);

		Reduce(list);

		Assert.Equal("[[[[0,7],4],[[7,8],[6,0]]],[8,1]]", Stringify(list));
	}

	[Theory]
	[InlineData(new[] { "[1,1]", "[2,2]", "[3,3]", "[4,4]" }, "[[[[1,1],[2,2]],[3,3]],[4,4]]")]
	[InlineData(new[] { "[1,1]", "[2,2]", "[3,3]", "[4,4]", "[5,5]" }, "[[[[3,0],[5,3]],[4,4]],[5,5]]")]
	[InlineData(new[] { "[1,1]", "[2,2]", "[3,3]", "[4,4]", "[5,5]", "[6,6]" }, "[[[[5,0],[7,4]],[5,5]],[6,6]]")]
	public void AddMultipleWorksCorrectly(string[] inputs, string output)
	{
		var left = inputs[0].Select(c => ParseComponent(c)).ToList();

		foreach (var l in inputs.Skip(1)) {
			var right = l.Select(c => ParseComponent(c)).ToList();

			left = Add(left, right);

			Reduce(left);
		}

		Assert.Equal(output, Stringify(left));
	}

	[Fact]
	public void MaxMagnitudeWorksCorrectly()
	{
		var inputs = new[] {
			"[[[0,[5,8]],[[1,7],[9,6]]],[[4,[1,2]],[[1,4],2]]]",
			"[[[5,[2,8]],4],[5,[[9,9],0]]]",
			"[6,[[[6,2],[5,6]],[[7,6],[4,7]]]]",
			"[[[6,[0,7]],[0,9]],[4,[9,[9,0]]]]",
			"[[[7,[6,4]],[3,[1,3]]],[[[5,5],1],9]]",
			"[[6,[[7,3],[3,2]]],[[[3,8],[5,7]],4]]",
			"[[[[5,4],[7,7]],8],[[8,3],8]]",
			"[[9,3],[[9,9],[6,[4,9]]]]",
			"[[2,[[7,7],7]],[[5,8],[[9,3],[0,2]]]]",
			"[[[[5,2],5],[8,[3,7]]],[[5,[7,5]],[4,4]]]",
		};

		var max = 0L;
		var ml  = "";
		var mr  = "";

		for (var i = 0; i < inputs.Length; i++) {
			for (var j = 0; j < inputs.Length; j++) {
				if (inputs[i] == inputs[j]) {
					continue;
				}

				var l = inputs[i].Select(c => ParseComponent(c)).ToList();
				var r = inputs[j].Select(c => ParseComponent(c)).ToList();

				var mlr = Magnitude(Add(l, r));

				if (mlr > max) {
					ml  = inputs[i];
					mr  = inputs[j];
					max = mlr;
				}

				var mrl = Magnitude(Add(r, l));

				if (mlr > max) {
					mr  = inputs[i];
					ml  = inputs[j];
					max = mrl;
				}
			}
		}

		Console.WriteLine(max);
		Console.WriteLine(ml);
		Console.WriteLine(mr);
	}

	public enum ComponentType
	{
		PairStart,
		PairEnd,
		Value,
		Divider
	}

	public record struct Component(ComponentType ComponentType, int? Value);

	public class Node
	{
		public Node() { }

		public Node(int value)
		{
			Value = value;
		}

		public Node(Node? left, Node? right)
		{
			Left  = left;
			Right = right;
		}

		public Node? Left { get; set; }

		public Node? Right { get; set; }

		public int? Value { get; set; }

		public override string ToString()
		{
			if (Value.HasValue) {
				return Value.Value.ToString();
			} else if (Left == null || Right == null) {
				throw new InvalidOperationException("One of left or right is null");
			} else {
				return $"[{Left},{Right}]";
			}
		}
	}
}
