using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using FourxGame.MonoGame.Core.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FourxGame.MonoGame.Core.Systems.World;

public enum TerrainType
{
  None, Water, Plains, Forest, Desert
}

public closed record TerrainCellGenerationResult(Point GridCoordinates);

public record TerrainCellGenerationSuccess(Point GridCoordinates, TerrainType TerrainType) : TerrainCellGenerationResult(GridCoordinates);
public record TerrainCellGenerationFailure(Point GridCoordinates, string Message) : TerrainCellGenerationResult(GridCoordinates);

public closed record TerrainGridGenerationResult;

public record TerrainGridGenerationSuccess(TerrainData TerrainData) : TerrainGridGenerationResult;
public record TerrainGridGenerationFailure(IReadOnlyCollection<TerrainCellGenerationFailure> CellGenerationFailures, string Message) : TerrainGridGenerationResult;


public class TerrainGenerator(Grid grid, int seed)
{
  private readonly TerrainType[] _validTerrainTypeIds = [.. Enum.GetValues<TerrainType>().Where(v => v != TerrainType.None).Order()];
  private readonly TerrainTilePlacementRule _rule = new();

  public TerrainCellGenerationResult ResolveCell(TerrainData terrainData, Point gridCoordinates, Random random)
  {
    ArgumentNullException.ThrowIfNull(terrainData);
    ArgumentNullException.ThrowIfNull(random);

    var targetCellType = TerrainType.None;
    var localValidTerrainTypeIds = _validTerrainTypeIds.ToList();

    do
    {
      targetCellType = localValidTerrainTypeIds[random.Next(0, localValidTerrainTypeIds.Count)];

      var adjacencies = new TerrainType[] {
          terrainData.TerrainGrid[Math.Clamp(gridCoordinates.X + 1, 0, grid.GridSizeX - 1), Math.Clamp(gridCoordinates.Y    , 0, grid.GridSizeY - 1)],
          terrainData.TerrainGrid[Math.Clamp(gridCoordinates.X - 1, 0, grid.GridSizeX - 1), Math.Clamp(gridCoordinates.Y    , 0, grid.GridSizeY - 1)],
          terrainData.TerrainGrid[Math.Clamp(gridCoordinates.X    , 0, grid.GridSizeX - 1), Math.Clamp(gridCoordinates.Y + 1, 0, grid.GridSizeY - 1)],
          terrainData.TerrainGrid[Math.Clamp(gridCoordinates.X    , 0, grid.GridSizeX - 1), Math.Clamp(gridCoordinates.Y - 1, 0, grid.GridSizeY - 1)],

          terrainData.TerrainGrid[Math.Clamp(gridCoordinates.X + 1, 0, grid.GridSizeX - 1), Math.Clamp(gridCoordinates.Y + 1, 0, grid.GridSizeY - 1)],
          terrainData.TerrainGrid[Math.Clamp(gridCoordinates.X + 1, 0, grid.GridSizeX - 1), Math.Clamp(gridCoordinates.Y - 1, 0, grid.GridSizeY - 1)],
          terrainData.TerrainGrid[Math.Clamp(gridCoordinates.X - 1, 0, grid.GridSizeX - 1), Math.Clamp(gridCoordinates.Y + 1, 0, grid.GridSizeY - 1)],
          terrainData.TerrainGrid[Math.Clamp(gridCoordinates.X - 1, 0, grid.GridSizeX - 1), Math.Clamp(gridCoordinates.Y - 1, 0, grid.GridSizeY - 1)],
        };

      if (_rule.Allows(targetCellType, adjacencies))
      {
        return new TerrainCellGenerationSuccess(gridCoordinates, targetCellType);
      }
      else
      {
        if (!localValidTerrainTypeIds.Remove(targetCellType))
        {
          return new TerrainCellGenerationFailure(gridCoordinates, $"[ResolveCell] {gridCoordinates} {targetCellType} -> Unable to reduce candidates to valid values");
        }
      }
    }
    while (localValidTerrainTypeIds.Count > 0);

    return new TerrainCellGenerationFailure(gridCoordinates, $"Unable to find a valid terrain type for {gridCoordinates}");
  }

  public TerrainGridGenerationResult GenerateTerrain()
  {
    var random = new Random(seed);
    var td = new TerrainData(grid);
    var failures = new List<TerrainCellGenerationFailure>();

    for (var x = 0; x < grid.GridSizeX; x++)
    {
      for (var y = 0; y < grid.GridSizeY; y++)
      {
        var cellResult = ResolveCell(td, new(x, y), random);
        switch (cellResult)
        {
          case TerrainCellGenerationSuccess success:
            td.TerrainGrid[x, y] = success.TerrainType;
            break;
          case TerrainCellGenerationFailure failure:
            failures.Add(failure);
            break;
        }
      }
    }

    if (failures.Count == 0)
    {
      return new TerrainGridGenerationSuccess(td);
    }
    else
    {
      return new TerrainGridGenerationFailure(failures, "Unable to generate a complete grid");
    }
  }
}

public class TerrainData(Grid grid)
{
#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional
  internal TerrainType[,] TerrainGrid { get; } = new TerrainType[grid.GridSizeX, grid.GridSizeY];
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
      for (var y = 0; y < grid.GridSizeY; y++)
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
