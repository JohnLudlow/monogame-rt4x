using FourxGame.MonoGame.Core.Systems.World;
using Xunit;

namespace FourxGame.MonoGame.UnitTests.Systems.World;

public class TerrainTilePlacementRuleTests
{
  [Theory]
  [InlineData(TerrainType.Desert, TerrainType.Desert, true)]
  [InlineData(TerrainType.Forest, TerrainType.Forest, true)]
  [InlineData(TerrainType.Plains, TerrainType.Plains, true)]
  [InlineData(TerrainType.Water, TerrainType.Water, true)]

  [InlineData(TerrainType.Desert, TerrainType.Forest, false)]
  [InlineData(TerrainType.Forest, TerrainType.Desert, false)]
  public void TerrainTilePlacementRule_Allows_ReturnsExpectedResult(TerrainType terrainTypeA, TerrainType terrainTypeB, bool expected)
  {
    var rule = new TerrainTilePlacementRule();
    var actual = rule.Allows(terrainTypeA, terrainTypeB);
    Assert.Equal(expected, actual);
  }

  [Theory]
  [InlineData(TerrainType.Desert, new[] {
    TerrainType.Desert, TerrainType.Desert, TerrainType.Desert,
    TerrainType.Desert,                     TerrainType.Desert,
    TerrainType.Desert, TerrainType.Desert, TerrainType.Desert,
  }, true)]

  [InlineData(TerrainType.Forest, new[] {
    TerrainType.Forest, TerrainType.Forest, TerrainType.Forest,
    TerrainType.Forest,                     TerrainType.Forest,
    TerrainType.Forest, TerrainType.Forest, TerrainType.Forest,
  }, true)]

  [InlineData(TerrainType.Plains, new[] {
    TerrainType.Plains, TerrainType.Plains, TerrainType.Plains,
    TerrainType.Plains,                     TerrainType.Plains,
    TerrainType.Plains, TerrainType.Plains, TerrainType.Plains,
  }, true)]

  [InlineData(TerrainType.Water, new[] {
    TerrainType.Water, TerrainType.Water, TerrainType.Water,
    TerrainType.Water,                    TerrainType.Water,
    TerrainType.Water, TerrainType.Water, TerrainType.Water,
  }, true)]

  [InlineData(TerrainType.Forest, new[] {
    TerrainType.Desert, TerrainType.Desert, TerrainType.Desert,
    TerrainType.Desert,                     TerrainType.Desert,
    TerrainType.Desert, TerrainType.Desert, TerrainType.Desert,
  }, false)]

  [InlineData(TerrainType.Desert, new[] {
    TerrainType.Forest, TerrainType.Forest, TerrainType.Forest,
    TerrainType.Forest,                     TerrainType.Forest,
    TerrainType.Forest, TerrainType.Forest, TerrainType.Forest,
  }, false)]

  public void TerrainTilePlacementRule_Allows_WithArrayOfAdjacentCells_ReturnsExpectedResult(TerrainType terrainTypeA, TerrainType[] adjacentCells, bool expected)
  {
    var rule = new TerrainTilePlacementRule();
    var actual = rule.Allows(terrainTypeA, adjacentCells);
    Assert.Equal(expected, actual);
  }
}
