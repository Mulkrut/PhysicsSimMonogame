using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Physics_Sim;

public class Game1 : Game
{
  private GraphicsDeviceManager _graphics;
  private SpriteBatch _spriteBatch;

  // Simulation Settings
  private World _world;
  private Texture2D _gridTexture;
  private Color[] _colorBuffer;
  
  private const int GridWidth = 200;
  private const int GridHeight = 150;
  private const int Scale = 6; // Scale up the pixels for visibility

  // texture region that defines the slime sprite in the atlas.
  public Game1()
  {
    _graphics = new GraphicsDeviceManager(this);
    Content.RootDirectory = "Content";
    IsMouseVisible = true;

    // Set window size based on grid size and scale
    _graphics.PreferredBackBufferWidth = GridWidth * Scale;
    _graphics.PreferredBackBufferHeight = GridHeight * Scale;
  }

  protected override void Initialize()
  {
    _world = new World(GridWidth, GridHeight);
    
    // The texture we use to "paint" the grid to the screen
    _gridTexture = new Texture2D(GraphicsDevice, GridWidth, GridHeight);
    _colorBuffer = new Color[GridWidth * GridHeight];
    base.Initialize();
  }

  protected override void LoadContent()
  {
    _spriteBatch = new SpriteBatch(GraphicsDevice);
  }

  protected override void Update(GameTime gameTime)
  {
    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
      Exit();

    var mouseState = Mouse.GetState();

    if (mouseState.LeftButton == ButtonState.Pressed)
      {
      int gridX = mouseState.X / Scale;
      int gridY = mouseState.Y / Scale;
      _world.SetCell(gridX, gridY, 1); // Places
      }

    _world.Update();
    base.Update(gameTime);
  }

  protected override void Draw(GameTime gameTime)
  {
  GraphicsDevice.Clear(Color.Black);

  // 1. Convert the World grid into Colors for the texture
  int[,] currentGrid = _world.GetGrid();
  for (int y = 0; y < GridHeight; y++)
  {
    for (int x = 0; x < GridWidth; x++)
    {
      int cellType = currentGrid[x, y];
      Color cellColor = cellType switch
      {
        1 => Color.SandyBrown,
        _ => Color.Transparent // Air
      };

      _colorBuffer[y * GridWidth + x] = cellColor;
    }
  }

  // 2. Upload color data to the GPU texture
  _gridTexture.SetData(_colorBuffer);

  // 3. Draw the texture scaled up to fill the screen
  _spriteBatch.Begin(samplerState: SamplerState.PointClamp); // PointClamp keeps pixels sharp
  _spriteBatch.Draw(_gridTexture, 
  new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);
  _spriteBatch.End();

  base.Draw(gameTime);
  }
}

