using System;
using FourxGame.MonoGame.Core.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FourxGame.MonoGame.Systems.World;

public enum TerrainType
{
    None, Water, Plains, Forest, Desert
}

public class TerrainGenerator(Grid grid, int seed)
{
  internal int _seed = seed;

  private Grid _grid = grid;
  private Random _random = new(seed);

  public TerrainData GenerateTerrain()
  {
    var td = new TerrainData(grid);

    for(var x = 0; x < _grid.GridSizeX; x++)
    {
      for(var y = 0; y < _grid.GridSizeY; y++)
      {
        td.TerrainGrid[x, y] = (TerrainType)_random.Next((int)TerrainType.Water, (int)TerrainType.Desert);
      }      
    }

    return td;
  }
}

public class TerrainData(Grid grid)
{
#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional
  internal TerrainType[,] TerrainGrid {get;} = new TerrainType[grid.GridSizeX, grid.GridSizeY];
#pragma warning restore CA1814 // Prefer jagged arrays over multidimensional

  public TerrainType GetTerrainForCell(Point cell)
  {
    if (!grid.IsValidGridPosition(cell)) return TerrainType.None;

    return TerrainGrid[cell.X, cell.Y];
  }   
}

public class TerrainRepresentation()
{
  public void DrawTerrain(Grid grid, TerrainData terrainData, SpriteBatch spriteBatch, Texture2D texture2D)
  {
    ArgumentNullException.ThrowIfNull(grid);
    ArgumentNullException.ThrowIfNull(terrainData);
    ArgumentNullException.ThrowIfNull(spriteBatch);
    
    for (var x = 0; x < grid.GridSizeX; x++)
    {
      for(var y = 0; y < grid.GridSizeY; y++)
      {
        var cell = new Point(x, y);
        if (!grid.IsValidGridPosition(cell)) continue;
        
        var color = terrainData.TerrainGrid[x, y] switch
        {
          TerrainType.None => Color.Gray,
          TerrainType.Water => Color.Blue,
          TerrainType.Plains => Color.LightGreen,
          TerrainType.Forest => Color.Green,
          TerrainType.Desert => Color.Yellow,
          _ => throw new NotImplementedException(),
        };

        spriteBatch.Draw(
          texture2D,
          grid.GridToWorldRect(cell),
          color
        );
      }      
    }
  }
}