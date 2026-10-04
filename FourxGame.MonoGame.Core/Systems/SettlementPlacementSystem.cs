using System.Collections.Generic;
using FourxGame.MonoGame.Core.Entities;
using Microsoft.Xna.Framework;

namespace FourxGame.MonoGame.Core.Systems;

public sealed class SettlementPlacementSystem(Grid grid)
{
  private readonly Dictionary<Point, Settlement> _settlements = [];
  public IReadOnlyDictionary<Point, Settlement> Settlements => _settlements;

  public bool TryPlace(Point cell)
  {
    if (!grid.IsValidGridPosition(cell))
    {
      return false;
    }

    return _settlements.TryAdd(cell, new Settlement(cell));
  }
}