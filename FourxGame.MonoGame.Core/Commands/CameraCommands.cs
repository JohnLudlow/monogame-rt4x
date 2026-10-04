using Microsoft.Xna.Framework;

namespace FourxGame.MonoGame.Core.Commands;

public abstract record InputCommand;

public sealed record PanCommand(Vector2 WorldDelta) : InputCommand;
public sealed record ZoomCommand(float Factor, Vector2 FocusScreen) : InputCommand;