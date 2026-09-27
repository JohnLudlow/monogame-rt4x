using System.Collections.Generic;
using FourxGame.MonoGame.Core.Commands;
using FourxGame.MonoGame.Core.Commands.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FourxGame.MonoGame.Core.Systems.Input;

public sealed class InputReader(InputBindings bindings)
{
  private int _previousScroll;
  private Point? _dragStartPosition;
  public IReadOnlyList<InputCommand> Produce(
    KeyboardState keyboard, MouseState mouse, float zoom, float dt
  )
  {
    var commands = new List<InputCommand>();
    var pan = Vector2.Zero;

    HandleKeyboardPan(keyboard, dt, commands, pan);
    HandleMouseDragPan(mouse, zoom, commands);
    HandleMouseWheelZoom(mouse, commands);

    return commands;
  }

  private void HandleMouseDragPan(MouseState mouse, float zoom, List<InputCommand> commands)
  {
    var dragDown = IsDragButtonDown(mouse, bindings.DragButton);
    var mousePos = new Point(mouse.X, mouse.Y);

    if (dragDown && _dragStartPosition is Point ds)
    {
      var screenDelta = mousePos - ds;
      if (screenDelta != Point.Zero)
      {
        var worldDelta = new Vector2(screenDelta.X, screenDelta.Y) / zoom;
        var panCommand = new PanCommand(worldDelta);
        commands.Add(panCommand);
      }

      _dragStartPosition = mousePos;
    }

    _dragStartPosition = dragDown ? mousePos : null;
  }

  private static bool IsDragButtonDown(MouseState mouseState, DragButton dragButton) => dragButton switch
  {
    DragButton.Left   => mouseState.LeftButton    == ButtonState.Pressed,
    DragButton.Middle => mouseState.MiddleButton  == ButtonState.Pressed,
    DragButton.Right  => mouseState.RightButton   == ButtonState.Pressed,
    _ => false
  };

  private void HandleMouseWheelZoom(MouseState mouse, List<InputCommand> commands)
  {
    var scrollDelta = mouse.ScrollWheelValue - _previousScroll;
    _previousScroll = mouse.ScrollWheelValue;
    if (scrollDelta != 0)
    {
      var factor = 1f + (scrollDelta / 120f) * bindings.ZoomStep;
      commands.Add(new ZoomCommand(factor, new Vector2(mouse.X, mouse.Y)));
    }
  }

  private void HandleKeyboardPan(KeyboardState keyboard, float dt, List<InputCommand> commands, Vector2 pan)
  {
    if (keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Left)) pan.X += 1; // world moves right => look left
    if (keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.Right)) pan.X -= 1;
    if (keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.Up)) pan.Y += 1;
    if (keyboard.IsKeyDown(Keys.S) || keyboard.IsKeyDown(Keys.Down)) pan.Y -= 1;

    if (pan != Vector2.Zero)
    {
      pan.Normalize();
      commands.Add(new PanCommand(pan * bindings.PanSpeed * dt));
    }
  }
}