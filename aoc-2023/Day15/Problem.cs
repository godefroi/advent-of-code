using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Year2023.Day15;

public partial class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		var inputSpan = input[0].AsSpan();
		var part1     = 0L;
		var part2     = 0L;
		var boxes     = new Box[256];

		// initialize the boxes
		for (var i = 0; i < boxes.Length; i++) {
			boxes[i] = new Box();
		}

		// execute the instructions
		foreach (var component in inputSpan.EnumerateBySplitting(',')) {
			var step = new Step(component);
			var boxN = ComputeHASH(step.Label);
			var box  = boxes[boxN];

			part1 += ComputeHASH(component);

			if (step.Operation == '-') {
				box.RemoveLens(step.Label);
			} else if (step.Operation == '=') {
				box.InsertLens(step.Label, step.FocalLength);
			}
		}

		for (var i = 0; i < boxes.Length; i++) {
			part2 += boxes[i].CalculatePower(i);
		}

		return (part1, part2);
	}

	private static int ComputeHASH(ReadOnlySpan<char> str)
	{
		var currentValue = 0;

		for (var i = 0; i < str.Length; i++) {
			currentValue += str[i];
			currentValue *= 17;
			currentValue %= 256;
		}

		return currentValue;
	}

	private readonly ref struct Step
	{
		public readonly string Label;
		public readonly char Operation;
		public readonly int FocalLength;

		public Step(ReadOnlySpan<char> stepText)
		{
			var opPos = stepText.IndexOfAny('=', '-');

			Label       = stepText[..opPos].ToString();
			Operation   = stepText[opPos];
			FocalLength = opPos < stepText.Length - 1 ? int.Parse(stepText[(opPos + 1)..]) : -1;
		}

		public override string ToString() => $"Step {{ Label = {Label}, Operation = {Operation}, FocalLength = {FocalLength} }}";
	}

	private class BoxedLens(string label, int focalLength)
    {
        public string Label { get; private set; } = label;

        public int FocalLength { get; private set; } = focalLength;

		public void Replace(int newFocalLength) => FocalLength = newFocalLength;

        public override int GetHashCode() => Label.GetHashCode();

		public override bool Equals([NotNullWhen(true)] object? obj) => Label.Equals(obj);
	}

	private class Box
	{
		private LinkedList<BoxedLens> _lenses = [];

		public void RemoveLens(string label)
		{
			var node = FindNode(label);

			if (node != null) {
				_lenses.Remove(node);
			}
		}

		public void InsertLens(string label, int focalLength)
		{
			var newLens = new BoxedLens(label, focalLength);
			var node    = FindNode(label);

			if (node == null) {
				_lenses.AddLast(newLens);
			} else {
				node.Value.Replace(focalLength);
			}
		}

		public long CalculatePower(int boxNumber)
		{
			var slotNumber = 1;
			var power      = 0L;

			foreach (var lens in _lenses) {
				var thisPower = (boxNumber + 1) * slotNumber * lens.FocalLength;

				//Console.WriteLine($"box {boxNumber} lens {slotNumber} ({lens.Label}) power {thisPower}");
				power += thisPower;
				slotNumber++;
			}

			return power;
		}

		private LinkedListNode<BoxedLens>? FindNode(string label)
		{
			var node = _lenses.First;

			while (node != null) {
				if (node.Value.Label == label) {
					return node;
				}

				node = node.Next;
			}

			return null;
		}
	}

	public class Tests
	{
		[Theory]
		[InlineData("HASH", 52)]
		[InlineData("rn=1", 30)]
		[InlineData("cm-", 253)]
		[InlineData("rn", 0)]
		public void HashComputedCorrectly(string str, int hashValue) =>
			Assert.Equal(hashValue, ComputeHASH(str));
	}
}
