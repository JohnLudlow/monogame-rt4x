using System;
using Microsoft.Xna.Framework;

namespace FourxGame.MonoGame.Core.Utilities;

/// <summary>
/// Pure, stateless conversions between the coordinate spaces used by the game:
/// screen (pixels), world (game units), and grid (whole cells).
/// </summary>
/// <remarks>
/// These functions are deliberately independent of any <c>Grid</c> or <c>Camera</c>
/// instance so they can be unit-tested in isolation. Callers supply the grid
/// <paramref name="origin"/> and cell size, or the camera transform, as needed.
/// The full conversion chain is Screen ⇄ World ⇄ Grid; there is no direct
/// screen-to-grid step — compose the two conversions.
/// </remarks>
public static class Transforms
{
  /// <summary>
  /// Converts a screen-space position (for example, the mouse position) into
  /// world space by applying the inverse of the camera transform.
  /// </summary>
  /// <param name="screenPos">The position in screen/pixel space.</param>
  /// <param name="cameraTransform">
  /// The camera's world-to-screen transform. It is inverted internally to go the
  /// other way. Use <see cref="Matrix.Invert(ref Matrix, out Matrix)"/> — not a
  /// transpose — because this matrix contains scale and translation.
  /// </param>
  /// <returns>The equivalent position in world space.</returns>
  public static Vector2 ScreenToWorld(Vector2 screenPos, Matrix cameraTransform)
  {
    Matrix.Invert(ref cameraTransform, out var inverse);
    return Vector2.Transform(screenPos, inverse);
  }

  /// <summary>
  /// Converts a world-space position into screen space by applying the camera transform.
  /// </summary>
  /// <param name="worldPos">The position in world/game-unit space.</param>
  /// <param name="cameraTransform">The camera's world-to-screen transform.</param>
  /// <returns>The equivalent position in screen/pixel space.</returns>
  public static Vector2 WorldToScreen(Vector2 worldPos, Matrix cameraTransform)
    => Vector2.Transform(worldPos, cameraTransform);

  /// <summary>
  /// Converts a world-space position into the whole grid cell that contains it.
  /// </summary>
  /// <param name="worldPos">The position in world space.</param>
  /// <param name="origin">The world-space position of grid cell (0, 0)'s top-left corner.</param>
  /// <param name="gridCellSize">The side length of one square cell, in world units.</param>
  /// <returns>The grid coordinates of the containing cell.</returns>
  /// <remarks>
  /// Uses <see cref="Math.Floor(double)"/> rather than a truncating cast so that
  /// negative world coordinates map to the correct cell. A cell owns the half-open
  /// interval <c>[n, n+1)</c> of cell units, so a point exactly on a cell's upper
  /// edge belongs to the next cell.
  /// </remarks>
  public static Point WorldToGrid(Vector2 worldPos, Vector2 origin, int gridCellSize)
  {
    var localX = worldPos.X - origin.X;
    var localY = worldPos.Y - origin.Y;
    return new Point(
      (int)Math.Floor(localX / gridCellSize),
      (int)Math.Floor(localY / gridCellSize)
    );
  }

  /// <summary>
  /// Converts grid coordinates into the world-space position of that cell's centre.
  /// </summary>
  /// <param name="gridPos">The grid coordinates of the cell.</param>
  /// <param name="origin">The world-space position of grid cell (0, 0)'s top-left corner.</param>
  /// <param name="gridCellSize">The side length of one square cell, in world units.</param>
  /// <returns>The world-space position of the centre of the given cell.</returns>
  /// <remarks>
  /// Returns the cell <em>centre</em> (offset by half a cell), which is convenient for
  /// placing sprites and units. Because it returns the centre rather than a corner,
  /// this is not a strict inverse of <see cref="WorldToGrid"/>: the guaranteed
  /// round-trip is <c>WorldToGrid(GridToWorld(cell)) == cell</c>.
  /// </remarks>
  public static Vector2 GridToWorld(Point gridPos, Vector2 origin, int gridCellSize)
    => new(
      origin.X + gridPos.X * gridCellSize + gridCellSize / 2f,
      origin.Y + gridPos.Y * gridCellSize + gridCellSize / 2f
    );

  /// <summary>
  /// Returns the world-space rectangle occupied by the given cell: its top-left
  /// corner plus a width and height of one cell.
  /// </summary>
  /// <param name="gridPos">The grid coordinates of the cell.</param>
  /// <param name="origin">The world-space position of grid cell (0, 0)'s top-left corner.</param>
  /// <param name="gridCellSize">The side length of one square cell, in world units.</param>
  /// <returns>The cell's world-space bounds as a <see cref="Rectangle"/>.</returns>
  /// <remarks>
  /// Unlike <see cref="GridToWorld"/> (which returns the cell centre), this returns the
  /// cell's corner-anchored bounds — convenient for drawing a highlight or hit-testing.
  /// </remarks>
  public static Rectangle GridToWorldRect(Point gridPos, Vector2 origin, int gridCellSize)
    => new(
      (int)Math.Floor(origin.X + (gridPos.X * gridCellSize)),
      (int)Math.Floor(origin.Y + (gridPos.Y * gridCellSize)),
      gridCellSize,
      gridCellSize
    );
}
