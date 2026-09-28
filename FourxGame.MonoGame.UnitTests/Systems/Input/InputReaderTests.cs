using FourxGame.MonoGame.Core.Commands;
using FourxGame.MonoGame.Core.Commands.Input;
using FourxGame.MonoGame.Core.Systems.Input;
using Microsoft.Xna.Framework.Input;

namespace FourxGame.MonoGame.UnitTests.Systems.Input;

public class InputReaderTests
{
  [Fact]
  public void Produce_HeldLeftClick_EmitsOnlyOnePlacementCommand()
  {
    var reader = new InputReader(new InputBindings());
    var released = new MouseState(100, 100, 0,
      ButtonState.Released, ButtonState.Released, ButtonState.Released,
      ButtonState.Released, ButtonState.Released);
    var pressed = new MouseState(100, 100, 0,
      ButtonState.Pressed, ButtonState.Released, ButtonState.Released,
      ButtonState.Released, ButtonState.Released);

    reader.Produce(new KeyboardState(), released, 1f, 0.016f);
    var firstFrame = reader.Produce(new KeyboardState(), pressed, 1f, 0.016f);
    var heldFrame = reader.Produce(new KeyboardState(), pressed, 1f, 0.016f);

    Assert.Single(firstFrame.OfType<PlaceSettlementAtScreenCommand>());
    Assert.Empty(heldFrame.OfType<PlaceSettlementAtScreenCommand>());
  }  
}