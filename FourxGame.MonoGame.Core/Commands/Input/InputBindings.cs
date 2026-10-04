using Microsoft.Xna.Framework.Input;

namespace FourxGame.MonoGame.Core.Commands.Input;

public sealed class InputBindings
{
  public Keys PanUp{get; init;} = Keys.W;
  public Keys PanLeft{get; init;} = Keys.A;
  public Keys PanDown{get; init;} = Keys.S;
  public Keys PanRight{get; init;} = Keys.D;

  public float PanSpeed {get; init;} = 400f;
  public float ZoomStep {get; init;} = .1f;

  public DragButton DragButton {get; init;} = DragButton.Middle;
}

public enum DragButton {None, Left, Middle, Right}