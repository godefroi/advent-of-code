namespace aoc_2022.Day19;

public partial class Problem
{
	public static ProblemMetadata2 Metadata { get; } = new(Main, typeof(Problem));

	public static (long, long) Main(string[] input)
	{
		var blueprints = input.Select(Parse).ToArray();
		var part1      = blueprints.Select((bp, idx) => TestBlueprint(bp, 24) * (idx + 1)).Sum();
		var part2      = blueprints.Take(3).Select(bp => TestBlueprint(bp, 32)).Aggregate(1, (i1, i2) => i1 * i2);

		return (part1, part2);
	}

	private static int TestBlueprint(int[] blueprint, int minutes)
	{
		var blueprintId      = blueprint[0];
		var oreRobotCost     = blueprint[1];
		var clayRobotCost    = blueprint[2];
		var obsRobotOreCost  = blueprint[3];
		var obsRobotClayCost = blueprint[4];
		var geoRobotOreCost  = blueprint[5];
		var geoRobotObsCost  = blueprint[6];

		var maxRobots = (ore: new[] { oreRobotCost, clayRobotCost, obsRobotOreCost, geoRobotOreCost }.Max(), clay: obsRobotClayCost);
		var maxGeodes = 0;

		RunSearch(minutes, 1, 0, 0, 0, 0, 0, 0);

		return maxGeodes;

		void RunSearch(int time, int oreRobots, int clayRobots, int obsidianRobots, int ore, int clay, int obsidian, int geodes)
		{
			if (time < 1) {
				return;
			}

			if (geodes > maxGeodes) {
				maxGeodes = geodes;
			}

			if (obsidianRobots > 0) {
				var canBuildGeoRobot = geoRobotOreCost <= ore && geoRobotObsCost <= obsidian;
				var timeSkip         = 1 + (canBuildGeoRobot ? 0 : (int)Math.Max(Math.Ceiling((geoRobotOreCost - ore) / (double)oreRobots), Math.Ceiling((geoRobotObsCost - (double)obsidian) / obsidianRobots)));

				RunSearch(time - timeSkip, oreRobots, clayRobots, obsidianRobots, ore + timeSkip * oreRobots - geoRobotOreCost, clay + timeSkip * clayRobots, obsidian + timeSkip * obsidianRobots - geoRobotObsCost, geodes + time - timeSkip);

				if (canBuildGeoRobot) {
					return;
				}
			}

			if (clayRobots > 0) {
				var canBuildObsRobot = obsRobotOreCost <= ore && obsRobotClayCost <= clay;
				var timeSkip         = 1 + (canBuildObsRobot ? 0 : (int)Math.Max(Math.Ceiling((obsRobotOreCost - ore) / (double)oreRobots), Math.Ceiling((obsRobotClayCost - clay) / (double)clayRobots)));

				if (time - timeSkip > 2) {
					RunSearch(time - timeSkip, oreRobots, clayRobots, obsidianRobots + 1, ore + timeSkip * oreRobots - obsRobotOreCost, clay + timeSkip * clayRobots - obsRobotClayCost, obsidian + timeSkip * obsidianRobots, geodes);
				}
			}

			if (clayRobots < maxRobots.clay) {
				var canBuildClayRobot = clayRobotCost <= ore;
				var timeSkip          = 1 + (canBuildClayRobot ? 0 : (int)Math.Ceiling((clayRobotCost - ore) / (double)oreRobots));

				if (time - timeSkip > 3) {
					RunSearch(time - timeSkip, oreRobots, clayRobots + 1, obsidianRobots, ore + timeSkip * oreRobots - clayRobotCost, clay + timeSkip * clayRobots, obsidian + timeSkip * obsidianRobots, geodes);
				}
			}

			if (oreRobots < maxRobots.ore) {
				var canBuildOreRobot = oreRobotCost <= ore;
				var timeSkip         = 1 + (canBuildOreRobot ? 0 : (int)Math.Ceiling((oreRobotCost - ore) / (double)oreRobots));

				if (time - timeSkip > 4) {
					RunSearch(time - timeSkip, oreRobots + 1, clayRobots, obsidianRobots, ore + timeSkip * oreRobots - oreRobotCost, clay + timeSkip * clayRobots, obsidian + timeSkip * obsidianRobots, geodes);
				}
			}
		}
	}

	private static int[] Parse(string line) => ParseRegex().Matches(line).Select(m => int.Parse(m.Value)).ToArray();

	[GeneratedRegex(@"\d+")]
	private static partial Regex ParseRegex();
}
