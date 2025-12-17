namespace AdventOfCode;

public class AaTree<T>(Comparison<T>? comparison = null) where T : notnull
{
	private readonly Comparison<T> _comparison = comparison ?? Comparer<T>.Default.Compare;

	private Node? _root = null;

	public void Insert(T value) => _root = Insert(value, _root);

	public void Delete(T value) => _root = Delete(value, _root);

	public bool Contains(T value) => Search(value, _root) != null;

	public void Traverse(Func<T, bool> action) => Traverse(_root, action);

	public bool TryFindSuccessor(T value, out T? successorValue)
	{
		var valueNode = Search(value, _root);

		if (valueNode == null) {
			successorValue = default;
			return false;
		}

		// if there's a right subtree, the successor
		// is the leftmost in the right subtree
		if (valueNode.Right != null) {
			var successor = Successor(valueNode.Right);
			successorValue = successor.Value;
			return true;
		}

		// otherwise, we have to do it the hard way
		var current = _root;
		var found   = false;

		successorValue = default;

		while (current != null) {
			var comp = _comparison(value, current.Value);
			if (comp < 0) {
				successorValue = current.Value;
				found = true;
				current = current.Left;
			} else if (comp > 0) {
				current = current.Right;
			} else {
				break;
			}
		}

		return found;
	}

	public bool TryFindPredecessor(T value, out T? predecessorValue)
	{
		var valueNode = Search(value, _root);

		if (valueNode == null) {
			predecessorValue = default;
			return false;
		}
		
		if (valueNode.Left != null) {
			var predecessor = Predecessor(valueNode.Left);
			predecessorValue = predecessor.Value;
			return true;
		}

		// hard way... search from root
		var current = _root;
		var found   = false;

		predecessorValue = default;

		while (current != null) {
			var comp = _comparison(value, current.Value);
			if (comp > 0) {
				predecessorValue = current.Value;
				found = true;
				current = current.Right;
			} else if (comp < 0) {
				current = current.Left;
			} else {
				break;
			}
		}

		return found;
	}

	private Node? Insert(T value, Node? tree)
	{
		if (tree == null) {
			return new Node(value) { Level = 1 };
		}

		var comp = _comparison(value, tree.Value);

		if (comp < 0) {
			tree.Left = Insert(value, tree.Left);
		} else if (comp > 0) {
			tree.Right = Insert(value, tree.Right);
		} else {
			throw new NotSupportedException("Cannot insert a duplicate value");
		}

		tree = Skew(tree);
		tree = Split(tree);

		return tree;
	}

	private Node? Delete(T value, Node? tree)
	{
		if (tree == null) {
			return null;
		}

		var comp = _comparison(value, tree.Value);

		if (comp > 0) {
			tree.Right = Delete(value, tree.Right);
		} else if (comp < 0) {
			tree.Left = Delete(value, tree.Left);
		} else {
			if (tree.Left == null && tree.Right == null) {
				return null;
			} else if (tree.Left == null) {
				return tree.Right;
			} else if (tree.Right == null) {
				return tree.Left;
			} else {
				var successor = Successor(tree.Right);
				tree.Value = successor.Value;
				tree.Right = Delete(successor.Value, tree.Right);
			}
		}

		tree = DecreaseLevel(tree);
		tree = Skew(tree);
		tree?.Right = Skew(tree.Right);

		if (tree?.Right != null) {
			tree.Right.Right = Skew(tree.Right.Right);
		}

		tree = Split(tree);
		tree?.Right = Split(tree.Right);

		return tree;
	}

	private Node? Search(T value, Node? tree)
	{
		if (tree == null) {
			return null;
		}

		var comp = _comparison(value, tree.Value);

		if (comp < 0) {
			return Search(value, tree.Left);
		} else if (comp > 0) {
			return Search(value, tree.Right);
		} else {
			return tree;
		}
	}

	private static bool Traverse(Node? tree, Func<T, bool> action)
	{
		if (tree == null) {
			return true;
		}

		if (!Traverse(tree.Left, action)) {
			return false;
		}

		if (!action(tree.Value)) {
			return false;
		}

		if (!Traverse(tree.Right, action)) {
			return false;
		}

		return true;
	}

	private static Node Successor(Node node)
	{
		while (node.Left != null) {
			node = node.Left;
		}

		return node;
	}

	private static Node Predecessor(Node node)
	{
		while (node.Right != null) {
			node = node.Right;
		}

		return node;
	}

	private static Node? Skew(Node? node)
	{
		if (node == null || node.Left == null) {
			// no node, or node doesn't have a left horizontal link
			return node;
		} else if (node.Left.Level == node.Level) {
			// node has a left horizontal link; skew it by swapping the links
			var oldLeft = node.Left;
			node.Left = oldLeft.Right;
			oldLeft.Right = node;
			return oldLeft;
		} else {
			return node;
		}
	}

	private static Node? Split(Node? node)
	{
		if (node == null || node.Right?.Right == null) {
			return node;
		} else if (node.Level == node.Right.Right.Level) {
			// node has two right horizontal links; elevate the middle node and return it
			var oldRight = node.Right;
			node.Right = node.Left;
			oldRight.Left = node;
			oldRight.Level += 1;
			return oldRight;
		} else {
			return node;
		}
	}

	private static Node DecreaseLevel(Node node)
	{
		var shouldBe = Math.Min(node.Left?.Level ?? 0, node.Right?.Level ?? 0) + 1;

		if (shouldBe < node.Level) {
			node.Level = shouldBe;

			if (node.Right != null && shouldBe < node.Right.Level) {
				node.Right.Level = shouldBe;
			}
		}

		return node;
	}

	internal class Node(T value)
	{
		public T Value { get; set; } = value;
		internal int Level { get; set; }
		public Node? Left { get; set; }
		public Node? Right { get; set; }
	}
}

public class AaTreeTests
{
	[Test]
	public async Task ContainsWorksCorrectly()
	{
		var t = new AaTree<int>();

		t.Insert(2);
		t.Insert(4);
		t.Insert(6);
		t.Insert(8);

		for (var i = 1; i < 10; i++) {
			await Assert.That(t.Contains(i)).IsEqualTo(i % 2 == 0);
		}
	}

	[Test]
	public async Task DeleteWorksCorrectly()
	{
		var t = new AaTree<int>();

		t.Insert(1);
		t.Insert(3);
		t.Insert(5);
		t.Insert(7);
		t.Insert(9);

		for (var i = 1; i < 10; i++) {
			await Assert.That(t.Contains(i)).IsEqualTo(i % 2 != 0);
		}

		t.Delete(5);

		for (var i = 1; i < 10; i++) {
			await Assert.That(t.Contains(i)).IsEqualTo(i != 5 && i % 2 != 0);
		}
	}

	[Test]
	public async Task TraverseWorksCorrectly()
	{
		var t = new AaTree<int>();
		var s = new List<int>();

		t.Insert(1);
		t.Insert(3);
		t.Insert(5);
		t.Insert(7);
		t.Insert(9);
		t.Delete(5);

		t.Traverse(v => {
			s.Add(v);
			return true;
		});

		await Assert.That(s).IsEquivalentTo([
			1, 3, 7, 9
		]);
	}

	[Test]
	public async Task TryFindSuccessorWorksCorrectly()
	{
		var t = new AaTree<int>();

		t.Insert(1);
		t.Insert(3);
		t.Insert(5);
		t.Insert(7);
		t.Insert(9);

		var found = t.TryFindSuccessor(5, out var successor);

		await Assert.That(found).IsTrue();
		await Assert.That(successor).IsEqualTo(7);
	}

	[Test]
	public async Task TryFindSuccessorWorksWhenValueIsNotPresent()
	{
		var t = new AaTree<int>();

		t.Insert(1);
		t.Insert(3);
		t.Insert(5);
		t.Insert(7);
		t.Insert(9);

		var found = t.TryFindSuccessor(10, out var successor);

		await Assert.That(found).IsFalse();
		await Assert.That(successor).IsDefault();
	}

	[Test]
	public async Task TryFindSuccessorWorksWhenSuccessorIsNotPresent()
	{
		var t = new AaTree<int>();

		t.Insert(1);
		t.Insert(3);
		t.Insert(5);
		t.Insert(7);
		t.Insert(9);

		var found = t.TryFindSuccessor(9, out var successor);

		await Assert.That(found).IsFalse();
		await Assert.That(successor).IsDefault();
	}

	[Test]
	public async Task TryFindPredecessorWorksCorrectly()
	{
		var t = new AaTree<int>();

		t.Insert(1);
		t.Insert(3);
		t.Insert(5);
		t.Insert(7);
		t.Insert(9);

		var found = t.TryFindPredecessor(5, out var successor);

		await Assert.That(found).IsTrue();
		await Assert.That(successor).IsEqualTo(3);
	}

	[Test]
	public async Task TryFindPredecessorWorksWhenValueIsNotPresent()
	{
		var t = new AaTree<int>();

		t.Insert(3);
		t.Insert(5);
		t.Insert(7);
		t.Insert(9);

		var found = t.TryFindPredecessor(1, out var successor);

		await Assert.That(found).IsFalse();
		await Assert.That(successor).IsDefault();
	}

	[Test]
	public async Task TryFindPredecessorWorksWhenPredecessorIsNotPresent()
	{
		var t = new AaTree<int>();

		t.Insert(1);
		t.Insert(3);
		t.Insert(5);
		t.Insert(7);
		t.Insert(9);

		var found = t.TryFindPredecessor(1, out var successor);

		await Assert.That(found).IsFalse();
		await Assert.That(successor).IsDefault();
	}
}
