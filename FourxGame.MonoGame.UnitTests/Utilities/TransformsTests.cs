using Microsoft.Xna.Framework;
using FourxGame.MonoGame.Core.Utilities;

namespace FourxGame.MonoGame.UnitTests.Utilities;

public class TransformsTests
{
  private const int CellSize = 48;

  // --- WorldToGrid: basic mapping -------------------------------------------------

  [Theory]
  [InlineData(0, 0, 0, 0)]      // origin -> cell (0,0)
  [InlineData(47, 47, 0, 0)]    // just inside cell (0,0)
  [InlineData(48, 48, 1, 1)]    // exactly on the boundary -> next cell (half-open [n, n+1))
  [InlineData(72, 24, 1, 0)]    // mixed axes
  [InlineData(96, 96, 2, 2)]
  public void WorldToGrid_MapsPointToContainingCell(float worldX, float worldY, int expectedX, int expectedY)
  {
    var cell = Transforms.WorldToGrid(new Vector2(worldX, worldY), Vector2.Zero, CellSize);

    Assert.Equal(new Point(expectedX, expectedY), cell);
  }

  [Fact]
  public void WorldToGrid_NegativeCoordinates_UsesFloorNotTruncation()
  {
    // A point just left of the origin belongs to cell -1, not cell 0.
    // A truncating (int) cast would wrongly give 0.
    var cell = Transforms.WorldToGrid(new Vector2(-1, -1), Vector2.Zero, CellSize);

    Assert.Equal(new Point(-1, -1), cell);
  }

  [Fact]
  public void WorldToGrid_RespectsNonZeroOrigin()
  {
    var origin = new Vector2(100, 40);

    // A point one cell past the origin should land in cell (1, 0).
    var cell = Transforms.WorldToGrid(new Vector2(100 + CellSize, 40), origin, CellSize);

    Assert.Equal(new Point(1, 0), cell);
  }

  // --- GridToWorld: returns the cell centre ---------------------------------------

  [Fact]
  public void GridToWorld_ReturnsCellCentre()
  {
    // Cell (0,0) with a 48px cell has its centre at (24, 24).
    var world = Transforms.GridToWorld(new Point(0, 0), Vector2.Zero, CellSize);

    Assert.Equal(new Vector2(24f, 24f), world);
  }

  [Fact]
  public void GridToWorld_AddsOrigin()
  {
    var origin = new Vector2(100, 40);

    var world = Transforms.GridToWorld(new Point(0, 0), origin, CellSize);

    Assert.Equal(new Vector2(124f, 64f), world);
  }

  // --- Round-trip invariant -------------------------------------------------------

  [Theory]
  [InlineData(0, 0)]
  [InlineData(1, 0)]
  [InlineData(5, 3)]
  [InlineData(-2, -7)]
  public void WorldToGrid_Of_GridToWorld_RoundTripsToSameCell(int cellX, int cellY)
  {
    var origin = new Vector2(100, 40);
    var start = new Point(cellX, cellY);

    var world = Transforms.GridToWorld(start, origin, CellSize);
    var roundTripped = Transforms.WorldToGrid(world, origin, CellSize);

    Assert.Equal(start, roundTripped);
  }

  // --- GridToWorldRect: corner-anchored cell bounds -------------------------------

  [Fact]
  public void GridToWorldRect_ReturnsCornerAnchoredCellBounds()
  {
    // Cell (0,0), origin zero, 48px -> rect at (0,0) sized 48x48.
    var rect = Transforms.GridToWorldRect(new Point(0, 0), Vector2.Zero, CellSize);

    Assert.Equal(new Rectangle(0, 0, CellSize, CellSize), rect);
  }

  [Fact]
  public void GridToWorldRect_OffsetsByCellAndOrigin()
  {
    var origin = new Vector2(100, 40);

    // Cell (2,1): top-left = origin + (2*48, 1*48) = (196, 88).
    var rect = Transforms.GridToWorldRect(new Point(2, 1), origin, CellSize);

    Assert.Equal(new Rectangle(196, 88, CellSize, CellSize), rect);
  }

  [Fact]
  public void GridToWorldRect_CornerMatchesGridToWorldCentre()
  {
    // The rect's centre should equal GridToWorld (which returns the cell centre).
    var cell = new Point(3, 2);
    var rect = Transforms.GridToWorldRect(cell, Vector2.Zero, CellSize);
    var centre = Transforms.GridToWorld(cell, Vector2.Zero, CellSize);

    Assert.Equal(centre.X, rect.X + rect.Width / 2f);
    Assert.Equal(centre.Y, rect.Y + rect.Height / 2f);
  }

  // --- Screen <-> World via the camera transform ----------------------------------

  [Fact]
  public void WorldToScreen_WithIdentity_ReturnsSamePoint()
  {
    var point = new Vector2(200, 150);

    var screen = Transforms.WorldToScreen(point, Matrix.Identity);

    Assert.Equal(point, screen);
  }

  [Fact]
  public void ScreenToWorld_InvertsWorldToScreen()
  {
    // Camera zoomed 2x and translated by (100, 40): scale then translate.
    var camera = Matrix.CreateScale(2f) * Matrix.CreateTranslation(100, 40, 0);
    var world = new Vector2(200, 150);

    var screen = Transforms.WorldToScreen(world, camera);   // expected (500, 340)
    var back = Transforms.ScreenToWorld(screen, camera);

    Assert.Equal(new Vector2(500f, 340f), screen);
    Assert.Equal(world.X, back.X, precision: 3);
    Assert.Equal(world.Y, back.Y, precision: 3);
  }
}
