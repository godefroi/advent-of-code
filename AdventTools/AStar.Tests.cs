using static AdventOfCode.AStar;

namespace AdventOfCode;

public class AStarTests
{
	[Fact]
	public void SimplePathfindingWorks()
	{
		var path = FindPath((10, 10), (10, 15), coord => new List<(Coordinate, float)> {
			((coord.x + 1, coord.y), 1),
			((coord.x - 1, coord.y), 1),
			((coord.x, coord.y + 1), 1),
			((coord.x, coord.y - 1), 1)
		});

		Assert.NotNull(path);
		Assert.Collection(path,
			c => Assert.Equal((10, 10), c),
			c => Assert.Equal((10, 11), c),
			c => Assert.Equal((10, 12), c),
			c => Assert.Equal((10, 13), c),
			c => Assert.Equal((10, 14), c),
			c => Assert.Equal((10, 15), c));
	}

	[Fact]
	public void PathfindingAroundObstacleWorks()
	{
		var excluded = new HashSet<Coordinate>() {
			(12, 9),
			(12, 10),
			(12, 11),
		};

		var path = FindPath((10, 10), (15, 10), coord => {
			var ret = new List<(Coordinate, float)>() {
				((coord.x + 1, coord.y), 1),
				((coord.x - 1, coord.y), 1),
				((coord.x, coord.y + 1), 1),
				((coord.x, coord.y - 1), 1)
			};

			return ret.Where(item => !excluded.Contains(item.Item1));
		});

		Assert.NotNull(path);
		Assert.Collection(path,
			c => Assert.Equal((10, 10), c),
			c => Assert.Equal((11, 10), c),
			c => Assert.Equal((11, 11), c),
			c => Assert.Equal((11, 12), c),
			c => Assert.Equal((12, 12), c),
			c => Assert.Equal((13, 12), c),
			c => Assert.Equal((13, 11), c),
			c => Assert.Equal((14, 11), c),
			c => Assert.Equal((15, 11), c),
			c => Assert.Equal((15, 10), c));
	}

	[Fact]
	public void PathfindingWithWeightsWorks()
	{
		var path = AStar.FindPath((10, 10), (15, 10), coord => {
			var ret = new List<(Coordinate, float)>() {
				((coord.x + 1, coord.y), 1),
				((coord.x - 1, coord.y), 1),
				((coord.x, coord.y + 1), 1),
				((coord.x, coord.y - 1), 1)
			};

			if (coord == (12, 10)) {
				// remove the default-weighted item
				Assert.True(ret.Remove(((13, 10), 1)));

				// re-add with a heavier weight
				ret.Add(((13, 10), 5));
			}

			return ret;
		});

		Assert.NotNull(path);
		Assert.Collection(path,
			c => Assert.Equal((10, 10), c),
			c => Assert.Equal((11, 10), c),
			c => Assert.Equal((12, 10), c),
			c => Assert.Equal((12, 11), c),
			c => Assert.Equal((13, 11), c),
			c => Assert.Equal((14, 11), c),
			c => Assert.Equal((14, 10), c),
			c => Assert.Equal((15, 10), c));
	}
}
