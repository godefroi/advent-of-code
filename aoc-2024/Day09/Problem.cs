namespace AdventOfCode.Year2024.Day09;

public class Problem
{
	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long, long) Execute(string[] input)
	{
		Span<short> map = stackalloc short[128 * 1024]; // should be plenty, I guess?

		var (mLen, bList) = BuildMap(input[0], map);

		// shorten the map, we don't need the empty space
		map = map[..mLen];

		var part1 = Part1(map);
		var part2 = Part2(bList);

		return (part1, part2);
	}

	private static long Part1(Span<short> map)
	{
		var fIdx = FindEmptyBlock(map, 0);
		var lIdx = map.Length - 1;
		var csum = 0L;

		while (fIdx < lIdx) {
			// move the block at lIdx to fIdx
			map[fIdx] = map[lIdx];
			map[lIdx] = -1;

			// move in from the ends to the next empty and valued blocks
			fIdx = FindEmptyBlock(map, fIdx);
			lIdx = FindNonEmptyBlock(map, lIdx);
		}

		//Console.WriteLine(string.Concat(map.ToArray().Select(s => s switch { -1 => ".", _ => s.ToString() })));
		for (var i = 0; i < map.Length; i++) {
			if (map[i] > -1) {
				csum += map[i] * i;
			}
		}

		return csum;
	}

	private static long Part2(LinkedList<Block> blocks)
	{
		if (blocks.First == null || blocks.Last == null) {
			throw new InvalidOperationException("Cannot operate on an empty list");
		}

		var curBlock   = blocks.Last;
		var movedFiles = new HashSet<short>() { -1 };

		while (curBlock != null) {
			if (movedFiles.Add(curBlock.Value.FileId)) {
				// find a block that curBlock will fit into
				var destBlock = FindEmptyBlock(blocks.First, curBlock);

				if (destBlock == null) {
					curBlock = curBlock.Previous;

					if (curBlock == null) {
						break;
					}

					continue;
				}

				// split destBlock if it's larger than we need
				if (destBlock.Value.Length > curBlock.Value.Length) {
					// make a new empty block for the leftover
					var splitBlock = new Block(destBlock.Value.Length - curBlock.Value.Length, -1);

					// insert it after the existing destBlock
					blocks.AddAfter(destBlock, splitBlock);
				}

				// move curBlock into destBlock, curBlock becomes empty
				destBlock.Value = curBlock.Value;
				curBlock.Value = new Block(destBlock.Value.Length, -1);
			}

			// advance up the list
			curBlock = curBlock.Previous;
		}

		// now, calculate the checksum
		var csum = 0L;
		var bIdx = 0;

		foreach (var block in blocks) {
			// skip empty blocks
			if (block.FileId == -1) {
				bIdx += block.Length;
				continue;
			}

			// otherwise, add to the checksum
			for (var i = 0; i < block.Length; i++) {
				csum += bIdx++ * block.FileId;
			}
		}

		return csum;
	}

	private static int FindEmptyBlock(Span<short> map, int startIndex)
	{
		while (map[startIndex] != -1) {
			startIndex++;
		}

		return startIndex;
	}

	private static LinkedListNode<Block>? FindEmptyBlock(LinkedListNode<Block> block, LinkedListNode<Block> curBlock)
	{
		var minSize = curBlock.Value.Length;

		while (block != curBlock) {
			if (block.Value.FileId == -1 && block.Value.Length >= minSize) {
				return block;
			}

			if (block.Next == null) {
				return null;
			}

			block = block.Next;
		}

		return null;
	}

	private static int FindNonEmptyBlock(Span<short> map, int startIndex)
	{
		while (map[startIndex] == -1) {
			startIndex--;
		}

		return startIndex;
	}

	private static (int MapLength, LinkedList<Block> BlockList) BuildMap(ReadOnlySpan<char> chars, Span<short> ret)
	{
		var file = true;
		var fidx = (short)0;
		var ridx = 0;

		var blockList = new LinkedList<Block>();

		for (var i = 0; i < chars.Length; i++) {
			var cnt = chars[i] switch {
				'0' => 0,
				'1' => 1,
				'2' => 2,
				'3' => 3,
				'4' => 4,
				'5' => 5,
				'6' => 6,
				'7' => 7,
				'8' => 8,
				'9' => 9,
				_ => throw new InvalidOperationException($"Invalid character at position {i}: {chars[i]}"),
			};

			var blockContent = file ? fidx++ : (short)-1;

			for (var j = 0; j < cnt; j++) {
				ret[ridx++] = blockContent;
			}

			blockList.AddLast(new Block(cnt, blockContent));

			file = !file;
		}

		return (ridx, blockList);
	}

	private readonly record struct Block(int Length, short FileId);
}
