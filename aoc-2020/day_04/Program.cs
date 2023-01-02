using System.Text.RegularExpressions;

using aoc_tools;

//var input = @"ecl:gry pid:860033327 eyr:2020 hcl:#fffffd
//byr:1937 iyr:2017 cid:147 hgt:183cm

//iyr:2013 ecl:amb cid:350 eyr:2023 pid:028048884
//hcl:#cfa07d byr:1929

//hcl:#ae17e1 iyr:2013
//eyr:2024
//ecl:brn pid:760753108 byr:1931
//hgt:179cm

//hcl:#cfa07d eyr:2025 pid:166559648
//iyr:2011 ecl:brn hgt:59in
//".Split(Environment.NewLine).SkipLast(1).ToList();

var input = await PuzzleInput.GetInputLines();

Console.WriteLine($"part 1: {ParseCredentials(input).Where(l => l.Count == 7).Count()}"); // part 1 is 264
Console.WriteLine($"part 2: {ParseCredentials(input).Where(l => ValidateCredential(l)).Count()}"); // part 2 is 224

IEnumerable<List<string>> ParseCredentials(IEnumerable<string> input)
{
	var ret = new List<string>();

	foreach (var inp in input) {
		if (string.IsNullOrWhiteSpace(inp) && ret.Count > 0) {
			yield return ret;
			ret = new List<string>();
		} else {
			ret.AddRange(inp.Split(' ').Where(s => !s.StartsWith("cid")));
		}
	}

	if (ret.Count > 0) {
		yield return ret;
	}
}

bool ValidateCredential(List<string> credential)
{
	if (credential.DistinctBy(s => s[..3]).Count() != 7) {
		return false;
	}

	foreach (var cred in credential) {
		switch (cred[..3]) {
			case "byr": // byr (Birth Year) - four digits; at least 1920 and at most 2002.
				var y1 = int.Parse(cred[4..]);
				if (y1 < 1920 || y1 > 2002) {
					return false;
				}
				break;
			case "iyr": // iyr (Issue Year) - four digits; at least 2010 and at most 2020.
				var y2 = int.Parse(cred[4..]);
				if (y2 < 2010 || y2 > 2020) {
					return false;
				}
				break;
			case "eyr": // eyr (Expiration Year) - four digits; at least 2020 and at most 2030.
				var y3 = int.Parse(cred[4..]);
				if (y3 < 2020 || y3 > 2030) {
					return false;
				}
				break;
			case "hgt": // hgt (Height) - a number followed by either cm or in: If cm, at least 150 and at most 193. If in, at least 59 and at most 76.
				int.TryParse(cred[4..^2], out int h);
				var t = cred[^2..];
				if ((t != "in" && t != "cm") || (t == "cm" && (h < 150 || h > 193)) || (t == "in" && (h < 59 || h > 76))) {
					return false;
				}
				break;
			case "hcl": // hcl (Hair Color) - a # followed by exactly six characters 0-9 or a-f.
				if (!Regex.IsMatch(cred[4..], @"\#[0-9a-f]{6}")) {
					return false;
				}
				break;
			case "ecl": // ecl (Eye Color) - exactly one of: amb blu brn gry grn hzl oth.
				if (!new[] { "amb","blu", "brn", "gry", "grn", "hzl", "oth" }.Contains(cred[4..])) {
					return false;
				}
				break;
			case "pid": // pid (Passport ID) - a nine-digit number, including leading zeroes.
				if (!Regex.IsMatch(cred[3..], @":[0-9]{9}$")) {
					return false;
				}
				break;
		}
	}

	return true;
}

internal static class TestData
{
	public static List<string> AllInvalid =>
		@"eyr:1972 cid:100
hcl:#18171d ecl:amb hgt:170 pid:186cm iyr:2018 byr:1926

iyr:2019
hcl:#602927 eyr:1967 hgt:170cm
ecl:grn pid:012533040 byr:1946

hcl:dab227 iyr:2012
ecl:brn hgt:182cm pid:021572410 eyr:2020 byr:1992 cid:277

hgt:59cm ecl:zzz
eyr:2038 hcl:74454a iyr:2023
pid:3556412378 byr:2007
".Split(Environment.NewLine).SkipLast(1).ToList();

	public static List<string> AllValid =>
		@"pid:087499704 hgt:74in ecl:grn iyr:2012 eyr:2030 byr:1980
hcl:#623a2f

eyr:2029 ecl:blu cid:129 byr:1989
iyr:2014 pid:896056539 hcl:#a97842 hgt:165cm

hcl:#888785
hgt:164cm byr:2001 iyr:2015 cid:88
pid:545766238 ecl:hzl
eyr:2022

iyr:2010 hgt:158cm hcl:#b6652a ecl:blu byr:1944 eyr:2021 pid:093154719
".Split(Environment.NewLine).SkipLast(1).ToList();
}