using System;
using FourxGame.MonoGame.Core.Commands.Input;
using FourxGame.MonoGame.Core.Entities;
using FourxGame.MonoGame.Core.Systems;
using FourxGame.MonoGame.Core.Systems.Input;
using FourxGame.MonoGame.Core.Systems.World;
using FourxGame.MonoGame.Systems.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FourxGame.MonoGame;

public class Game1 : Game
{
  private Grid? _grid;
  private TerrainData _terrainData = null!;
  private TerrainRepresentation _terrainRepresentation = null!;
  private Camera? _camera;
  private SettlementPlacementSystem _settlements = null!;
  private SettlementPlacementController _settlementPlacementController = null!;
  private GraphicsDeviceManager _graphics;
  private Texture2D? _pixel;
  private InputReader _inputReader = null!;
  private CameraController _cameraController = null!;
  private Point? _hoveredCell = Point.Zero;

  private SpriteBatch? _spriteBatch;

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
    _grid = new Grid
    {
      GridCellSize = 48,
      GridSizeX = 32,
      GridSizeY = 16,
      Origin = Vector2.Zero
    };

    _camera = new Camera();

    _settlements = new SettlementPlacementSystem(_grid);
    _settlementPlacementController = new SettlementPlacementController(_grid, _camera, _settlements);

    _inputReader = new InputReader(new InputBindings());
    _cameraController = new CameraController(_camera);
    
    var tg = new TerrainGenerator(_grid, 123);
    _terrainData = tg.GenerateTerrain();
    _terrainRepresentation = new TerrainRepresentation();

    base.Initialize();
  }

  protected override void LoadContent()
  {
    _spriteBatch = new SpriteBatch(GraphicsDevice);

    _pixel = new Texture2D(GraphicsDevice, 1, 1);
    _pixel.SetData([Color.White]);
  }

protected override void Update(GameTime gameTime)
{
  ArgumentNullException.ThrowIfNull(gameTime);
  if (_camera is null) throw new InvalidOperationException($"Cannot call {nameof(Update)} when {nameof(_camera)} is null");
  if (_grid is null) throw new InvalidOperationException($"Cannot call {nameof(Update)} when {nameof(_grid)} is null");

  var mouseState = Mouse.GetState();

  var commands = _inputReader.Produce(
    Keyboard.GetState(), 
    mouseState,
    _camera.Zoom,
    (float)gameTime.ElapsedGameTime.TotalSeconds
  );
  _cameraController.Apply(commands);
  _settlementPlacementController.Apply(commands);  

  _hoveredCell = _grid.ScreenToGrid(new Vector2(mouseState.X, mouseState.Y), _camera.Transform);

  base.Update(gameTime);
}

  protected override void Draw(GameTime gameTime)
  {
    if (_camera is null) throw new InvalidOperationException($"Cannot call {nameof(Draw)} when {nameof(_camera)} is null");
    if (_spriteBatch is null) throw new InvalidOperationException($"Cannot call {nameof(Draw)} when {nameof(_spriteBatch)} is null");
    if (_grid is null) throw new InvalidOperationException($"Cannot call {nameof(Draw)} when {nameof(_grid)} is null");
    if (_pixel is null) throw new InvalidOperationException($"Cannot call {nameof(Draw)} when {nameof(_pixel)} is null");

    GraphicsDevice.Clear(Color.CornflowerBlue);

    _spriteBatch.Begin(transformMatrix: _camera.Transform);
    {
      _terrainRepresentation.DrawTerrain(_grid, _terrainData, _spriteBatch, _pixel);
      
      DrawGrid(_spriteBatch, _grid);
      DrawSettlements(_spriteBatch, _grid);
      DrawHoveredCell(_spriteBatch, _grid);
    }
    _spriteBatch.End();

    base.Draw(gameTime);
  }

  private void DrawSettlements(SpriteBatch spriteBatch, Grid grid)
  {
    foreach (var settlement in _settlements.Settlements.Values)
    {
      var tile = grid.GridToWorldRect(settlement.Cell);
      const int markerSize = 20;
      var marker = new Rectangle(
        tile.Center.X - markerSize / 2,
        tile.Center.Y - markerSize / 2,
        markerSize,
        markerSize);

      spriteBatch.Draw(_pixel, marker, Color.Gold);
    }
  }

  private void DrawHoveredCell(SpriteBatch spriteBatch, Grid grid)
  {    
    if (_hoveredCell is Point p)
    {
      spriteBatch.Draw(
        _pixel,
        grid.GridToWorldRect(p),
        Color.Red with { A = 10 }
      );
    }
  }

  private void DrawGrid(SpriteBatch spriteBatch, Grid grid)
  {
    for (var x = 0; x <= grid.GridSizeX; x++)
    {
      spriteBatch.Draw(
        _pixel,
        new Rectangle(
          new((int)Math.Floor(grid.Origin.X + (x * grid.GridCellSize)), 0),
          new(1, grid.GridSizeY * grid.GridCellSize)
        ),
        Color.White
      );
    }

    for (var y = 0; y <= grid.GridSizeY; y++)
    {
      spriteBatch.Draw(
        _pixel,
        new Rectangle(
          new(0, (int)Math.Floor(grid.Origin.Y + (y * grid.GridCellSize))),
          new(grid.GridSizeX * grid.GridCellSize, 1)
        ),
        Color.White
      );
    }
  }
}
