using Microsoft.Xna.Framework;
using FourxGame.MonoGame.Core.Entities;

namespace FourxGame.MonoGame.UnitTests.Entities;

public class CameraTests
{
  [Fact]
  public void Camera_Defaults_AreIdentityLike()
  {
    var camera = new Camera();

    Assert.Equal(Vector2.Zero, camera.Position);
    Assert.Equal(1f, camera.Zoom);
    // Zoom 1, position 0 -> identity transform.
    Assert.Equal(Matrix.Identity, camera.Transform);
  }

  [Fact]
  public void Transform_ReflectsZoom_AfterItChanges()
  {
    var camera = new Camera();
    _ = camera.Transform; // force the initial (identity) build so caching is in play

    camera.Zoom = 2f;

    Assert.Equal(Matrix.CreateScale(2f), camera.Transform);
  }

  [Fact]
  public void Transform_ReflectsPosition_AfterItChanges()
  {
    var camera = new Camera();
    _ = camera.Transform;

    camera.Position = new Vector2(100, 40);

    var expected = Matrix.CreateScale(1f) * Matrix.CreateTranslation(100, 40, 0);
    Assert.Equal(expected, camera.Transform);
  }

  [Fact]
  public void Transform_CombinesZoomThenTranslation()
  {
    var camera = new Camera { Zoom = 2f, Position = new Vector2(100, 40) };

    // A world point (200,150) should map to (500,340): scale first, then translate.
    var screen = Vector2.Transform(new Vector2(200, 150), camera.Transform);

    Assert.Equal(new Vector2(500f, 340f), screen);
  }

  [Theory]
  [InlineData(0f, 0.1f)]     // below minimum -> clamped up
  [InlineData(-3f, 0.1f)]    // negative -> clamped, never inverts the view
  [InlineData(10f, 5f)]      // above maximum -> clamped down
  [InlineData(2.5f, 2.5f)]   // in range -> unchanged
  public void Zoom_IsClampedToValidRange(float set, float expected)
  {
    var camera = new Camera { Zoom = set };

    Assert.Equal(expected, camera.Zoom);
  }

  [Fact]
  public void Pan_AddsToPosition()
  {
    var camera = new Camera { Position = new Vector2(10, 20) };

    camera.Pan(new Vector2(5, -8));

    Assert.Equal(new Vector2(15, 12), camera.Position);
  }
}
