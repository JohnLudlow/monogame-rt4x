using System;
using Microsoft.Xna.Framework;

namespace FourxGame.MonoGame.Core.Entities;

/// <summary>
/// A 2D view onto the game world. Owns the world-to-screen <see cref="Transform"/>
/// used to draw the world (via <c>SpriteBatch.Begin(transformMatrix: …)</c>) and to
/// convert between screen and world space.
/// </summary>
/// <remarks>
/// The <see cref="Transform"/> is computed lazily and cached: changing
/// <see cref="Position"/> or <see cref="Zoom"/> marks it dirty, and the next read of
/// <see cref="Transform"/> rebuilds it. Callers never have to remember to refresh it.
/// </remarks>
public class Camera
{
  private bool _dirty = true;
  private Matrix _transform;

  /// <summary>
  /// The camera position in world space. Setting this invalidates the cached
  /// <see cref="Transform"/>.
  /// </summary>
  public Vector2 Position
  {
    get;
    set
    {
      field = value;
      _dirty = true;
    }
  } = Vector2.Zero;

  /// <summary>
  /// The zoom (scale) factor, where <c>1.0</c> is 1:1. Values are clamped to the
  /// range <c>[0.1, 5.0]</c> so the view cannot invert or collapse. Setting this
  /// invalidates the cached <see cref="Transform"/>.
  /// </summary>
  public float Zoom
  {
    get;
    set
    {
      field = Math.Clamp(value, .1f, 5f);
      _dirty = true;
    }
  } = 1f;

  /// <summary>
  /// The world-to-screen transform: scale (zoom) applied first, then translation
  /// (pan). Recomputed on read only when <see cref="Position"/> or <see cref="Zoom"/>
  /// has changed since the last read.
  /// </summary>
  public Matrix Transform
  {
    get
    {
      if (_dirty)
      {
        _transform = Matrix.CreateScale(Zoom)
                   * Matrix.CreateTranslation(Position.X, Position.Y, 0);

        _dirty = false;
      }

      return _transform;
    }
  }

  /// <summary>
  /// Moves the camera by <paramref name="delta"/> world units.
  /// </summary>
  /// <param name="delta">The amount to add to <see cref="Position"/>.</param>
  public void Pan(Vector2 delta) => Position += delta;
}
