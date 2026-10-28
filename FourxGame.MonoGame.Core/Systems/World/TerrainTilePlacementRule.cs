using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FourxGame.MonoGame.Core.Systems.World;

public class TerrainTilePlacementRule
{
  public bool Allows(TerrainType terrainTypeA, IReadOnlyCollection<TerrainType> adjacentCells)
  {
    return !adjacentCells.Any(c => !Allows(terrainTypeA, c) || !Allows(c, terrainTypeA));
  }

  public bool Allows(TerrainType terrainTypeA, TerrainType terrainTypeB)
  {
    return (terrainTypeA, terrainTypeB) switch
    {
      (TerrainType.Desert, TerrainType.Forest) => false,
      (TerrainType.Forest, TerrainType.Desert) => false,
      _ => true
    };
  }
}
