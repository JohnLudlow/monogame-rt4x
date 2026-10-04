
using FourxGame.MonoGame.Core.Entities;
using FourxGame.MonoGame.Core.Systems;
using Microsoft.Xna.Framework;

namespace FourxGame.MonoGame.UnitTests.Systems;

public class SettlementPlacementSystemTests
{
  private static Grid MakeGrid() => new()
  {
    GridCellSize = 48,
    GridSizeX = 32,
    GridSizeY = 16,
    Origin = Vector2.Zero
  };

  [Fact]
  public void TryPlace_EmptyInBoundsCell_AddsSettlement()
  {
    var system = new SettlementPlacementSystem(MakeGrid());
    var cell = new Point(4, 3);
    var placed = system.TryPlace(cell);

    Assert.True(placed);
    Assert.Equal(new Settlement(cell), system.Settlements[cell]);
  }

  [Fact]
  public void TryPlace_OccupiedInBoundsCell_LeavesOriginalSettlementUntouched()
  {
    var system = new SettlementPlacementSystem(MakeGrid());
    var cell = new Point(4, 3);
    var placed = system.TryPlace(cell);
    var placedAgain = system.TryPlace(cell);

    Assert.True(placed);
    Assert.False(placedAgain);
    Assert.Single(system.Settlements);
    Assert.Equal(new Settlement(cell), system.Settlements[cell]);
  }

  [Theory]
  [InlineData(-1, 0)]
  [InlineData(32, 0)]
  [InlineData(0, 16)]
  public void TryPlace_OffGridCell_DoesNotAddSettlement(int x, int y)
  {
    var system = new SettlementPlacementSystem(MakeGrid());

    var placed = system.TryPlace(new Point(x, y));

    Assert.False(placed);
    Assert.Empty(system.Settlements);
  }  
}