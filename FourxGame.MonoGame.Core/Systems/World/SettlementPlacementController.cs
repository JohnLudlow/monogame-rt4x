using System;
using System.Collections.Generic;
using FourxGame.MonoGame.Core.Commands;
using FourxGame.MonoGame.Core.Entities;

namespace FourxGame.MonoGame.Core.Systems.World;

public sealed class SettlementPlacementController(
  Grid grid,
  Camera camera,
  SettlementPlacementSystem settlements
)
{
  public void Apply(IReadOnlyList<InputCommand> commands)
  {
    ArgumentNullException.ThrowIfNull(commands);

    foreach (var command in commands)
    {
      if (command is not PlaceSettlementAtScreenCommand place)
      {
        continue;
      }

      var cell = grid.ScreenToGrid(place.ScreenPosition, camera.Transform);
      if (cell is not null)
      {
        settlements.TryPlace(cell.Value);
      }
    }
  }
}