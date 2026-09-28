using FourxGame.MonoGame.Core.Commands;
using FourxGame.MonoGame.Core.Entities;
using FourxGame.MonoGame.Core.Systems;
using FourxGame.MonoGame.Core.Systems.World;
using Microsoft.Xna.Framework;

namespace FourxGame.MonoGame.UnitTests.Systems.World;

public class SettlementPlacementControllerTests
{
  private static Grid MakeGrid() => new()
  {
    GridCellSize = 48,
    GridSizeX = 32,
    GridSizeY = 16,
    Origin = Vector2.Zero
  };


  [Fact]
  public void Apply_PlaceCommand_MapsScreenPositionThroughCameraToTheTargetCell()
  {
    var grid = MakeGrid();
    var camera = new Camera { Zoom = 2f, Position = new Vector2(100, 40) };
    var settlements = new SettlementPlacementSystem(grid);
    var controller = new SettlementPlacementController(grid, camera, settlements);

    // At this camera transform, world (200, 150) appears at screen (500, 340).
    controller.Apply([new PlaceSettlementAtScreenCommand(new Vector2(500, 340))]);

    Assert.Contains(new Point(4, 3), settlements.Settlements.Keys);
  }
}