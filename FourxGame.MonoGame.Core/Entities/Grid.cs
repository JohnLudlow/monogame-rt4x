using FourxGame.MonoGame.Core.Utilities;
using Microsoft.Xna.Framework;

namespace FourxGame.MonoGame.Core.Entities;

/// <summary>
/// A rectangular grid of square cells placed in world space. Provides convenience
/// conversions between world, grid, and screen coordinates that bind the grid's own
/// <see cref="Origin"/> and <see cref="GridCellSize"/>, plus bounds checking.
/// </summary>
public class Grid
{
  /// <summary>The world-space position of cell (0, 0)'s top-left corner.</summary>
  public Vector2 Origin { get; set; } = Vector2.Zero;

  /// <summary>The side length of one square cell, in world units.</summary>
  public required int GridCellSize { get; set; }

  /// <summary>The number of columns (cells along the X axis).</summary>
  public required int GridSizeX { get; set; }

  /// <summary>The number of rows (cells along the Y axis).</summary>
  public required int GridSizeY { get; set; }

  /// <summary>
  /// Returns the grid cell that contains the given world-space position, using this
  /// grid's <see cref="Origin"/> and <see cref="GridCellSize"/>. The result is not
  /// bounds-checked; use <see cref="IsValidGridPosition"/> if you need that.
  /// </summary>
  /// <param name="worldPos">The position in world space.</param>
  /// <returns>The grid coordinates of the containing cell (may be out of bounds).</returns>
  public Point WorldToGrid(Vector2 worldPos) => Transforms.WorldToGrid(worldPos, Origin, GridCellSize);

  /// <summary>
  /// Returns the world-space position of the centre of the given cell, using this
  /// grid's <see cref="Origin"/> and <see cref="GridCellSize"/>.
  /// </summary>
  /// <param name="gridPos">The grid coordinates of the cell.</param>
  /// <returns>The world-space centre of the cell.</returns>
  public Vector2 GridToWorld(Point gridPos) => Transforms.GridToWorld(gridPos, Origin, GridCellSize);

  /// <summary>
  /// Returns the world-space rectangle (corner-anchored, one cell in size) occupied by
  /// the given cell, using this grid's <see cref="Origin"/> and <see cref="GridCellSize"/>.
  /// Useful for drawing a cell highlight or hit-testing.
  /// </summary>
  /// <param name="gridPos">The grid coordinates of the cell.</param>
  /// <returns>The cell's world-space bounds.</returns>
  public Rectangle GridToWorldRect(Point gridPos) => Transforms.GridToWorldRect(gridPos, Origin, GridCellSize);

  /// <summary>
  /// Determines whether the given grid coordinates fall within this grid's bounds
  /// (<c>0 &lt;= X &lt; GridSizeX</c> and <c>0 &lt;= Y &lt; GridSizeY</c>).
  /// </summary>
  /// <param name="gridPos">The grid coordinates to test.</param>
  /// <returns><c>true</c> if the position is a valid cell in this grid; otherwise <c>false</c>.</returns>
  public bool IsValidGridPosition(Point gridPos)
    => gridPos.X >= 0 && gridPos.X < GridSizeX
    && gridPos.Y >= 0 && gridPos.Y < GridSizeY;

  /// <summary>
  /// Converts a screen-space position to a grid cell via world space, returning
  /// <c>null</c> when the position falls outside this grid's bounds.
  /// </summary>
  /// <param name="screenPos">The position in screen/pixel space (for example, the mouse).</param>
  /// <param name="cameraTransform">The camera's world-to-screen transform.</param>
  /// <returns>
  /// The containing cell, or <c>null</c> if the position is off the grid. Returning
  /// <c>null</c> forces callers to handle the out-of-bounds case.
  /// </returns>
  public Point? ScreenToGrid(Vector2 screenPos, Matrix cameraTransform)
  {
    var worldPos = Transforms.ScreenToWorld(screenPos, cameraTransform);
    var gridPos = WorldToGrid(worldPos);
    return IsValidGridPosition(gridPos) ? gridPos : null;
  }
}
