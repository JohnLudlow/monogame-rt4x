using FourxGame.MonoGame.Core.Entities;
using FourxGame.MonoGame.Systems.World;

namespace FourxGame.MonoGame.UnitTests.Systems.World;

public class TerrainGeneratorTests
{
  [Fact]
  public void TerrainGenerator_GeneratedTerrain_EveryCellIsPopulated()
  {
    var grid = new Grid{ 
      GridCellSize = 8,
      GridSizeX = 32,
      GridSizeY = 16
    };

    var tg = new TerrainGenerator(grid, 123);
    var td = tg.GenerateTerrain();

    for(var x = 0; x < grid.GridSizeX; x++)
    {
      for(var y = 0; y < grid.GridSizeY; y++)
      {
        Assert.NotEqual(TerrainType.None, td.TerrainGrid[x, y]);
      }      
    }
  } 

  [Fact]
  public void TerrainGenerator_GeneratedTerrainWithSameSeed_EveryCellIsTheSame()
  {
    var grid = new Grid{ 
      GridCellSize = 8,
      GridSizeX = 32,
      GridSizeY = 16
    };

    var tg1 = new TerrainGenerator(grid, 123);
    var td1 = tg1.GenerateTerrain();

    var tg2 = new TerrainGenerator(grid, 123);
    var td2 = tg2.GenerateTerrain();

    for(var x = 0; x < grid.GridSizeX; x++)
    {
      for(var y = 0; y < grid.GridSizeY; y++)
      {
        Assert.Equal(td1.TerrainGrid[x, y], td2.TerrainGrid[x, y]);
      }      
    }
  }  
}