using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FourxGame.MonoGame;

public class Game1 : Game
{
  private Texture2D? _pixel;
  private readonly int _gridCellSize = 48;
  private readonly Point _gridSize = new() { X = 32, Y = 16 };
  private readonly Point _gridOrigin = new() { X = 0, Y = 0 };
  private Point _hoveredCell = Point.Zero;

  private GraphicsDeviceManager _graphics;
  private SpriteBatch _spriteBatch;

  public Game1()
  {
    _graphics = new GraphicsDeviceManager(this)
    {
      PreferredBackBufferHeight = 1440,
      PreferredBackBufferWidth = 2560
    };

    Content.RootDirectory = "Content";
    IsMouseVisible = true;
  }

  protected override void Initialize()
  {
    // TODO: Add your initialization logic here

    base.Initialize();
  }

  protected override void LoadContent()
  {
    _spriteBatch = new SpriteBatch(GraphicsDevice);

    _pixel = new Texture2D(GraphicsDevice, 1, 1);
    _pixel.SetData([Color.White]);

    // TODO: use this.Content to load your game content here
  }

  protected override void Update(GameTime gameTime)
  {
    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
      Exit();

    var mouseState = Mouse.GetState();
    var hoveredCell = new Point(
      (mouseState.X - _gridOrigin.X) / _gridCellSize,
      (mouseState.Y - _gridOrigin.Y) / _gridCellSize
    );

    // TODO: Add your update logic here

    base.Update(gameTime);
  }

  protected override void Draw(GameTime gameTime)
  {
    GraphicsDevice.Clear(Color.CornflowerBlue);

    _spriteBatch.Begin();
    {
      for (var x = 0; x <= _gridSize.X; x++)
      {
        _spriteBatch.Draw(
          _pixel,
          new Rectangle(
            new(_gridOrigin.X + (x * _gridCellSize), 0),
            new(1, _gridSize.Y * _gridCellSize)
          ),
          Color.White
        );
      }

      for (var y = 0; y <= _gridSize.Y; y++)
      {
        _spriteBatch.Draw(
          _pixel,
          new Rectangle(
            new(0, _gridOrigin.Y + (y * _gridCellSize)),
            new(_gridSize.X * _gridCellSize, 1)
          ),
          Color.White
        );
      }
    }
    _spriteBatch.End();

    // TODO: Add your drawing code here

    base.Draw(gameTime);
  }
}
