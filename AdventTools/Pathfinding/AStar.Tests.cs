﻿using static AdventOfCode.AStar;

namespace AdventOfCode;

public class AStarTests
{
	[Fact]
	public void SimplePathfindingWorks()
	{
		var path = FindPath<Coordinate>((10, 10), (10, 15), coord => new List<(Coordinate, float)> {
			((coord.X + 1, coord.Y), 1),
			((coord.X - 1, coord.Y), 1),
			((coord.X, coord.Y + 1), 1),
			((coord.X, coord.Y - 1), 1)
		}, (f, t) => Coordinate.ManhattanDistance(f, t));

		Assert.NotNull(path);
		Assert.Collection(path,
			c => Assert.Equal(new Coordinate(10, 10), c),
			c => Assert.Equal(new Coordinate(10, 11), c),
			c => Assert.Equal(new Coordinate(10, 12), c),
			c => Assert.Equal(new Coordinate(10, 13), c),
			c => Assert.Equal(new Coordinate(10, 14), c),
			c => Assert.Equal(new Coordinate(10, 15), c));
	}

	[Fact]
	public void PathfindingAroundObstacleWorks()
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

		Assert.NotNull(path);
		Assert.Collection(path,
			c => Assert.Equal(new Coordinate(10, 10), c),
			c => Assert.Equal(new Coordinate(11, 10), c),
			c => Assert.Equal(new Coordinate(11, 11), c),
			c => Assert.Equal(new Coordinate(11, 12), c),
			c => Assert.Equal(new Coordinate(12, 12), c),
			c => Assert.Equal(new Coordinate(13, 12), c),
			c => Assert.Equal(new Coordinate(13, 11), c),
			c => Assert.Equal(new Coordinate(14, 11), c),
			c => Assert.Equal(new Coordinate(15, 11), c),
			c => Assert.Equal(new Coordinate(15, 10), c));
	}

	[Fact]
	public void PathfindingWithWeightsWorks()
	{
		var path = AStar.FindPath<Coordinate>((10, 10), (15, 10), coord => {
			var ret = new List<(Coordinate, float)>() {
				((coord.X + 1, coord.Y), 1),
				((coord.X - 1, coord.Y), 1),
				((coord.X, coord.Y + 1), 1),
				((coord.X, coord.Y - 1), 1)
			};

			if (coord == (12, 10)) {
				// remove the default-weighted item
				Assert.True(ret.Remove(((13, 10), 1)));

				// re-add with a heavier weight
				ret.Add(((13, 10), 5));
			}

			return ret;
		}, (f, t) => Coordinate.ManhattanDistance(f, t));

		Assert.NotNull(path);
		Assert.Collection(path,
			c => Assert.Equal(new Coordinate(10, 10), c),
			c => Assert.Equal(new Coordinate(11, 10), c),
			c => Assert.Equal(new Coordinate(12, 10), c),
			c => Assert.Equal(new Coordinate(12, 11), c),
			c => Assert.Equal(new Coordinate(13, 11), c),
			c => Assert.Equal(new Coordinate(14, 11), c),
			c => Assert.Equal(new Coordinate(14, 10), c),
			c => Assert.Equal(new Coordinate(15, 10), c));
	}
}
