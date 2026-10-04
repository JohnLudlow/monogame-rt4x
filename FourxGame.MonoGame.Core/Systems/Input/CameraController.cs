using System;
using System.Collections.Generic;
using FourxGame.MonoGame.Core.Commands;
using FourxGame.MonoGame.Core.Entities;
using FourxGame.MonoGame.Core.Utilities;

namespace FourxGame.MonoGame.Core.Systems.Input;

public sealed class CameraController(Camera camera)
{
  public void Apply(IReadOnlyList<InputCommand> commands)
  {
    ArgumentNullException.ThrowIfNull(commands);

    foreach (var command in commands)
    {
      switch (command)
      {
        case PanCommand pan:
          camera.Pan(pan.WorldDelta);
          break;
        
        case ZoomCommand zoom:
          var before = Transforms.ScreenToWorld(zoom.FocusScreen, camera.Transform);      
          camera.Zoom *= zoom.Factor;
          var after = Transforms.ScreenToWorld(zoom.FocusScreen, camera.Transform);
    
          camera.Pan(after - before);
          break;
      }
    }
  }
}