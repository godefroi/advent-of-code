namespace AdventOfCode.Year2021.Day16;

public class Problem
{
	private static string _padding = "";

	public static ProblemMetadata Metadata { get; } = new(Execute, typeof(Problem), null);

	public static (long versionsum, long value) Execute(string[] input)
	{
		var packet  = ParsePacket(ParseHex(input.First()));
		var ret     = (versionsum: SumVersions(packet), value: CalculateValue(packet));

		Console.WriteLine($"part 1: {ret.versionsum}");
		Console.WriteLine($"part 2: {ret.value}");

		return ret;
	}

	private static long CalculateValue(Packet packet) => packet.Type switch {
		0 => packet.Contents.Sum(p => CalculateValue(p)),
		1 => packet.Contents.Aggregate(1L, (x, y) => x * CalculateValue(y)),
		2 => packet.Contents.Min(p => CalculateValue(p)),
		3 => packet.Contents.Max(p => CalculateValue(p)),
		4 => packet.Value!.Value,
		5 => CalculateValue(packet.Contents.Skip(0).First()) > CalculateValue(packet.Contents.Skip(1).First()) ? 1 : 0,
		6 => CalculateValue(packet.Contents.Skip(0).First()) < CalculateValue(packet.Contents.Skip(1).First()) ? 1 : 0,
		7 => CalculateValue(packet.Contents.Skip(0).First()) == CalculateValue(packet.Contents.Skip(1).First()) ? 1 : 0,
		_ => throw new NotSupportedException($"The packet type {packet.Type} is not supported."),
	};

	private static (int len, long val) ParseLiteral(ArraySegment<char> input)
	{
		var bits = new List<char>();
		var len  = 0;

		foreach (var chunk in input.Chunk(5)) {
			bits.AddRange(chunk[1..]);
			len += 5;

			if (chunk[0] == '0') {
				break;
			}
		}

		return (len, Convert.ToInt64(new string(bits.ToArray()), 2));
	}

	private static long SumVersions(Packet packet) => packet.Version + (packet.Contents != null ? packet.Contents.Sum(SumVersions) : 0);

	private static char[] ParseHex(string input) => input.SelectMany(c => c switch {
			'0' => "0000",
			'1' => "0001",
			'2' => "0010",
			'3' => "0011",
			'4' => "0100",
			'5' => "0101",
			'6' => "0110",
			'7' => "0111",
			'8' => "1000",
			'9' => "1001",
			'A' => "1010",
			'B' => "1011",
			'C' => "1100",
			'D' => "1101",
			'E' => "1110",
			'F' => "1111",
			_ => throw new InvalidOperationException($"The character {c} is not valid input."),
		}).ToArray();

	private static Packet ParsePacket(ArraySegment<char> input)
	{
		_padding += ' ';

		// 3 bits for version
		var ver = Convert.ToInt32(new string(input[0..3]), 2);

		// 3 bits for type
		var type = Convert.ToInt32(new string(input[3..6]), 2);

		//Console.WriteLine($"{padding}ver: {ver} type: {type}");

		if (type == 4) {
			// literal value
			var (len, val) = ParseLiteral(input[6..]);
			//Console.WriteLine($"{padding}read a literal of {len} chars: {val}");
			return new Packet(len + 6, ver, type, val);
		}

		var lType = input[6];
		var subs  = new List<Packet>();
		var tlen  = 7;

		if (lType == '0') {
			// next 15 bits are the length in bits of the sub-packets
			tlen += 15;
			var plen = Convert.ToInt64(new string(input[7..22]), 2);
			//Console.WriteLine($"{padding}tlen is {tlen} before reading subs, need to read {plen} characters");
			// so, read sub-packets until we read plen characters
			while (plen > 0) {
				var spacket = ParsePacket(input[tlen..]);
				plen -= spacket.Length;
				tlen += spacket.Length;
				subs.Add(spacket);
				//Console.WriteLine($"{padding}read a packet of length {spacket.Length}, tlen is now {tlen}");
			}
		} else {
			// next 11 bits are the number of sub-packets
			tlen += 11;

			var pcount = Convert.ToInt64(new string(input[7..18]), 2);
			//Console.WriteLine($"{padding}ltype 1, pcount {pcount}");
			for (var i = 0; i < pcount; i++) {
				var spacket = ParsePacket(input[tlen..]);
				tlen += spacket.Length;
				subs.Add(spacket);
			}
		}

		_padding = _padding[1..];
		return new Packet(tlen, ver, type, null, subs);
	}

	[Test]
	[DisplayName("Day 16 Hex Parsing")]
	public async Task HexValuesParseCorrectly()
	{
		await Assert.That(Convert.ToInt32(new string(ParseHex("0")), 2)).IsEqualTo(0);
		await Assert.That(Convert.ToInt32(new string(ParseHex("1")), 2)).IsEqualTo(1);
		await Assert.That(Convert.ToInt32(new string(ParseHex("2")), 2)).IsEqualTo(2);
		await Assert.That(Convert.ToInt32(new string(ParseHex("3")), 2)).IsEqualTo(3);
		await Assert.That(Convert.ToInt32(new string(ParseHex("4")), 2)).IsEqualTo(4);
		await Assert.That(Convert.ToInt32(new string(ParseHex("5")), 2)).IsEqualTo(5);
		await Assert.That(Convert.ToInt32(new string(ParseHex("6")), 2)).IsEqualTo(6);
		await Assert.That(Convert.ToInt32(new string(ParseHex("7")), 2)).IsEqualTo(7);
		await Assert.That(Convert.ToInt32(new string(ParseHex("8")), 2)).IsEqualTo(8);
		await Assert.That(Convert.ToInt32(new string(ParseHex("9")), 2)).IsEqualTo(9);
		await Assert.That(Convert.ToInt32(new string(ParseHex("A")), 2)).IsEqualTo(10);
		await Assert.That(Convert.ToInt32(new string(ParseHex("B")), 2)).IsEqualTo(11);
		await Assert.That(Convert.ToInt32(new string(ParseHex("C")), 2)).IsEqualTo(12);
		await Assert.That(Convert.ToInt32(new string(ParseHex("D")), 2)).IsEqualTo(13);
		await Assert.That(Convert.ToInt32(new string(ParseHex("E")), 2)).IsEqualTo(14);
		await Assert.That(Convert.ToInt32(new string(ParseHex("F")), 2)).IsEqualTo(15);
	}

	[Test]
	[DisplayName("Day 16 Main Input")]
	public async Task MainInputFunctionCorrectly()
	{
		var (sum, value) = Execute(ReadFileLines("input.txt"));

		await Assert.That(sum).IsEqualTo(843);
		await Assert.That(value).IsEqualTo(5390807940351);
	}

	[Test]
	[DisplayName("Day 16 Literal Value Parsing")]
	public async Task LiteralPacketParsesCorrectly()
	{
		var binary = ParseHex("D2FE28");

		await Assert.That(binary).IsEquivalentTo("110100101111111000101000");

		var packet = ParsePacket(binary);

		await Assert.That(packet.Version).IsEqualTo(6);
		await Assert.That(packet.Type).IsEqualTo(4);
		await Assert.That(packet.Value).IsEqualTo(2021);
	}

	[Test]
	[DisplayName("Day 16 Operator Packet Parsing (first)")]
	public async Task OperatorPacket1ParsesCorrectly()
	{
		var binary = ParseHex("38006F45291200");

		await Assert.That(binary).IsEquivalentTo("00111000000000000110111101000101001010010001001000000000");

		var packet = ParsePacket(binary);

		await Assert.That(packet.Version).IsEqualTo(1);
		await Assert.That(packet.Type).IsEqualTo(6);
		await Assert.That(packet.Value).IsNull();
		await Assert.That(packet.Contents.Count).IsEqualTo(2);

		await Assert.That(packet.Contents.Skip(0).First().Value).IsEqualTo(10);
		await Assert.That(packet.Contents.Skip(1).First().Value).IsEqualTo(20);
	}

	[Test]
	[DisplayName("Day 16 Operator Packet Parsing (second)")]
	public async Task OperatorPacket2ParsesCorrectly()
	{
		var binary = ParseHex("EE00D40C823060");

		await Assert.That(binary).IsEquivalentTo("11101110000000001101010000001100100000100011000001100000");

		var packet = ParsePacket(binary);

		await Assert.That(packet.Version).IsEqualTo(7);
		await Assert.That(packet.Type).IsEqualTo(3);
		await Assert.That(packet.Value).IsNull();
		await Assert.That(packet.Contents.Count).IsEqualTo(3);

		await Assert.That(packet.Contents.Skip(0).First().Value).IsEqualTo(1);
		await Assert.That(packet.Contents.Skip(1).First().Value).IsEqualTo(2);
		await Assert.That(packet.Contents.Skip(2).First().Value).IsEqualTo(3);
	}

	[Test]
	[DisplayName("Day 16 Version Sum Samples")]
	public async Task PacketSumSamplesWork()
	{
		await Assert.That(SumVersions(ParsePacket(ParseHex("8A004A801A8002F478")))).IsEqualTo(16);
		await Assert.That(SumVersions(ParsePacket(ParseHex("620080001611562C8802118E34")))).IsEqualTo(12);
		await Assert.That(SumVersions(ParsePacket(ParseHex("C0015000016115A2E0802F182340")))).IsEqualTo(23);
		await Assert.That(SumVersions(ParsePacket(ParseHex("A0016C880162017C3686B18A3D4780")))).IsEqualTo(31);
	}

	[Test]
	[DisplayName("Day 16 Packet Value Samples")]
	public async Task PacketValueSamplesWork()
	{
		await Assert.That(CalculateValue(ParsePacket(ParseHex("C200B40A82")))).IsEqualTo(3);
		await Assert.That(CalculateValue(ParsePacket(ParseHex("04005AC33890")))).IsEqualTo(54);
		await Assert.That(CalculateValue(ParsePacket(ParseHex("880086C3E88112")))).IsEqualTo(7);
		await Assert.That(CalculateValue(ParsePacket(ParseHex("CE00C43D881120")))).IsEqualTo(9);
		await Assert.That(CalculateValue(ParsePacket(ParseHex("D8005AC2A8F0")))).IsEqualTo(1);
		await Assert.That(CalculateValue(ParsePacket(ParseHex("F600BC2D8F")))).IsEqualTo(0);
		await Assert.That(CalculateValue(ParsePacket(ParseHex("9C005AC2F8F0")))).IsEqualTo(0);
		await Assert.That(CalculateValue(ParsePacket(ParseHex("9C0141080250320F1802104A08")))).IsEqualTo(1);
	}

	public class Packet
	{
		public Packet(int length, int version, int type, long? value = null, IEnumerable<Packet>? contents = null)
		{
			Length  = length;
			Version = version;
			Type    = type;
			Value   = value;

			if (contents != null) {
				Contents = new List<Packet>(contents);
			} else {
				Contents = new List<Packet>();
			}
		}

		public int Length { get; private set; }

		public int Version { get; private set; }

		public int Type { get; private set; }

		public long? Value { get; private set; }

		public List<Packet> Contents { get; private set; }
	}
}
