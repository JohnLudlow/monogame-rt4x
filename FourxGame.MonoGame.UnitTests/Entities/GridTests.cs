namespace FourxGame.MonoGame.UnitTests.Entities;

public class GridTests
{
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
}