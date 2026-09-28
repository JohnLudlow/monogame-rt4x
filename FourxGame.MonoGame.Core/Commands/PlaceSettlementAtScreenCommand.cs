using Microsoft.Xna.Framework;

namespace FourxGame.MonoGame.Core.Commands;

public sealed record PlaceSettlementAtScreenCommand(Vector2 ScreenPosition) : InputCommand;