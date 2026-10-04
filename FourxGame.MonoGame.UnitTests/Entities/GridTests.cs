using Microsoft.Xna.Framework;
using FourxGame.MonoGame.Core.Entities;

namespace FourxGame.MonoGame.UnitTests.Entities;

public class GridTests
{
  private static Grid MakeGrid(int cellSize = 48, int sizeX = 32, int sizeY = 16, Vector2 origin = default)
    => new() { GridCellSize = cellSize, GridSizeX = sizeX, GridSizeY = sizeY, Origin = origin };

  [Fact]
  public void Grid_Ctor_ConstructsGrid()
  {
    var grid = new Grid
    {
      GridCellSize = 0,
      GridSizeX = 0,
      GridSizeY = 0
    };

    Assert.IsType<Grid>(grid);
    Assert.Equal(0, grid.GridCellSize);
    Assert.Equal(0, grid.GridSizeX);
    Assert.Equal(0, grid.GridSizeY);
  }

  // --- WorldToGrid / GridToWorld (using the grid's own origin + cell size) ---------

  [Fact]
  public void WorldToGrid_UsesGridOriginAndCellSize()
  {
    var grid = MakeGrid(origin: new Vector2(100, 40));

    var cell = grid.WorldToGrid(new Vector2(100 + 48, 40));

    Assert.Equal(new Point(1, 0), cell);
  }

  [Theory]
  [InlineData(0, 0)]
  [InlineData(3, 5)]
  [InlineData(31, 15)]
  public void WorldToGrid_Of_GridToWorld_RoundTrips(int cellX, int cellY)
  {
    var grid = MakeGrid(origin: new Vector2(100, 40));
    var start = new Point(cellX, cellY);

    var roundTripped = grid.WorldToGrid(grid.GridToWorld(start));

    Assert.Equal(start, roundTripped);
  }

  // --- IsValidGridPosition (half-open bounds) --------------------------------------

  [Theory]
  [InlineData(0, 0, true)]     // top-left corner is valid
  [InlineData(31, 15, true)]   // bottom-right corner is valid
  [InlineData(-1, 0, false)]   // negative
  [InlineData(0, -1, false)]
  [InlineData(32, 0, false)]   // == GridSizeX is out (half-open)
  [InlineData(0, 16, false)]   // == GridSizeY is out
  public void IsValidGridPosition_ChecksHalfOpenBounds(int x, int y, bool expected)
  {
    var grid = MakeGrid();

    Assert.Equal(expected, grid.IsValidGridPosition(new Point(x, y)));
  }

  // --- GridToWorldRect (uses the grid's own origin + cell size) --------------------

  [Fact]
  public void GridToWorldRect_UsesGridOriginAndCellSize()
  {
    var grid = MakeGrid(origin: new Vector2(100, 40));

    var rect = grid.GridToWorldRect(new Point(1, 0));

    Assert.Equal(new Rectangle(148, 40, 48, 48), rect);
  }

  // --- ScreenToGrid: null when off-grid, cell when on ------------------------------

  [Fact]
  public void ScreenToGrid_Identity_ReturnsCell_WhenInsideGrid()
  {
    var grid = MakeGrid();               // origin (0,0), 48px, 32x16
    // Identity camera => screen coordinates equal world coordinates.
    var cell = grid.ScreenToGrid(new Vector2(100, 100), Matrix.Identity);

    Assert.Equal(new Point(2, 2), cell); // 100/48 = 2 (floored)
  }

  [Fact]
  public void ScreenToGrid_ReturnsNull_WhenOutsideGrid()
  {
    var grid = MakeGrid();

    // Far past the right/bottom edge (grid spans 0..1536 x 0..768 world units).
    var cell = grid.ScreenToGrid(new Vector2(5000, 5000), Matrix.Identity);

    Assert.Null(cell);
  }

  [Fact]
  public void ScreenToGrid_ReturnsNull_ForNegativeWorldPosition()
  {
    var grid = MakeGrid();

    var cell = grid.ScreenToGrid(new Vector2(-10, -10), Matrix.Identity);

    Assert.Null(cell);
  }

  [Fact]
  public void ScreenToGrid_AppliesCameraTransform()
  {
    var grid = MakeGrid();
    // Camera zoomed 2x, translated (100,40): world (200,150) draws to screen (500,340).
    var camera = Matrix.CreateScale(2f) * Matrix.CreateTranslation(100, 40, 0);

    // So screen (500,340) should map back to world (200,150) -> cell (4,3): 200/48=4, 150/48=3.
    var cell = grid.ScreenToGrid(new Vector2(500, 340), camera);

    Assert.Equal(new Point(4, 3), cell);
  }
}
