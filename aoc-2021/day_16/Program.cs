var padding = "";
var input   = File.ReadAllLines(@"..\..\..\input.txt").First();
var packet  = ParsePacket(ParseHex(input));

Console.WriteLine($"part 1: {SumVersions(packet)}");
Console.WriteLine($"part 2: {CalculateValue(packet)}");

static char[] ParseHex(string input) => input.SelectMany(c => c switch {
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

Packet ParsePacket(ArraySegment<char> input)
{
	padding += ' ';

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

	padding = padding[1..];
	return new Packet(tlen, ver, type, null, subs);
}

static (int len, long val) ParseLiteral(ArraySegment<char> input)
{
	var bits = new List<char>();
	var len = 0;

	foreach (var chunk in input.Chunk(5)) {
		bits.AddRange(chunk[1..]);
		len += 5;

		if (chunk[0] == '0') {
			break;
		}
	}

	return (len, Convert.ToInt64(new string(bits.ToArray()), 2));
}

static long SumVersions(Packet packet) => packet.Version + (packet.Contents != null ? packet.Contents.Sum(p => SumVersions(p)) : 0);

static long CalculateValue(Packet packet)
{
	switch (packet.Type) {
		case 0 :
			return packet.Contents.Sum(p => CalculateValue(p));

		case 1:
			return packet.Contents.Aggregate(1L, (x, y) => x * CalculateValue(y));

		case 2:
			return packet.Contents.Min(p => CalculateValue(p));

		case 3:
			return packet.Contents.Max(p => CalculateValue(p));

		case 4:
			return packet.Value.Value;

		case 5:
			return CalculateValue(packet.Contents.Skip(0).First()) > CalculateValue(packet.Contents.Skip(1).First()) ? 1 : 0;

		case 6:
			return CalculateValue(packet.Contents.Skip(0).First()) < CalculateValue(packet.Contents.Skip(1).First()) ? 1 : 0;

		case 7:
			return CalculateValue(packet.Contents.Skip(0).First()) == CalculateValue(packet.Contents.Skip(1).First()) ? 1 : 0;

		default: throw new NotSupportedException($"The packet type {packet.Type} is not supported.");
	}
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