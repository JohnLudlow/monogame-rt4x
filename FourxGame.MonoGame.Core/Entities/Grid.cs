using System.Threading;
using Microsoft.Xna.Framework;
public class Grid
{
    public Vector2 Origin {get; set;} = Vector2.Zero;
    public required int GridCellSize {get; set;}
    public required int GridSizeX {get; set;}
    public required int GridSizeY {get; set;}
}