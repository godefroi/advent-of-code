using static AdventOfCode.AStar;

namespace AdventOfCode;

public class AStarTests
{
	[Test]
	public async Task SimplePathfindingWorks()
	{
		var path = FindPath<Coordinate>((10, 10), (10, 15), coord => new List<(Coordinate, float)> {
			((coord.X + 1, coord.Y), 1),
			((coord.X - 1, coord.Y), 1),
			((coord.X, coord.Y + 1), 1),
			((coord.X, coord.Y - 1), 1)
		}, (f, t) => Coordinate.ManhattanDistance(f, t));

		await Assert.That(path).IsNotNull().IsEquivalentTo([
			new Coordinate(10, 10),
			new Coordinate(10, 11),
			new Coordinate(10, 12),
			new Coordinate(10, 13),
			new Coordinate(10, 14),
			new Coordinate(10, 15),
		]);
	}

	[Test]
	public async Task PathfindingAroundObstacleWorks()
	{
		var excluded = new HashSet<Coordinate>() {
			(12, 9),
			(12, 10),
			(12, 11),
		};

		var path = FindPath<Coordinate>((10, 10), (15, 10), coord => {
			var ret = new List<(Coordinate, float)>() {
				((coord.X + 1, coord.Y), 1),
				((coord.X - 1, coord.Y), 1),
				((coord.X, coord.Y + 1), 1),
				((coord.X, coord.Y - 1), 1)
			};

			return ret.Where(item => !excluded.Contains(item.Item1));
		}, (f, t) => Coordinate.ManhattanDistance(f, t));

		await Assert.That(path).IsNotNull().IsEquivalentTo([
			new Coordinate(10, 10),
			new Coordinate(11, 10),
			new Coordinate(11, 11),
			new Coordinate(11, 12),
			new Coordinate(12, 12),
			new Coordinate(13, 12),
			new Coordinate(13, 11),
			new Coordinate(14, 11),
			new Coordinate(15, 11),
			new Coordinate(15, 10),
		]);
	}

	[Test]
	public async Task PathfindingWithWeightsWorks()
	{
		var path = FindPath<Coordinate>((10, 10), (15, 10), coord => {
			var ret = new List<(Coordinate, float)>() {
				((coord.X + 1, coord.Y), 1),
				((coord.X - 1, coord.Y), 1),
				((coord.X, coord.Y + 1), 1),
				((coord.X, coord.Y - 1), 1)
			};

			if (coord == (12, 10)) {
				// remove the default-weighted item
				if (!ret.Remove(((13, 10), 1))) {
					throw new InvalidOperationException("Should have removed the default-weighted item");
				}

				// re-add with a heavier weight
				ret.Add(((13, 10), 5));
			}

			return ret;
		}, (f, t) => Coordinate.ManhattanDistance(f, t));

		await Assert.That(path).IsNotNull().IsEquivalentTo([
			new Coordinate(10, 10),
			new Coordinate(11, 10),
			new Coordinate(12, 10),
			new Coordinate(12, 11),
			new Coordinate(13, 11),
			new Coordinate(14, 11),
			new Coordinate(14, 10),
			new Coordinate(15, 10),
		]);
	}
}
