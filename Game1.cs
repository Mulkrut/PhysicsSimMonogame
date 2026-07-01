using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Physics_Sim;

public class Game1 : Game
{
  private GraphicsDeviceManager _graphics;
  private SpriteBatch _spriteBatch;
  private UserInput _userInput = new UserInput();

  // Simulation Settings
  private World _world;
  private Brush _brush;
  private Texture2D _gridTexture;
  private Color[] _colorBuffer;

  private const int GridWidth = 300;
  private const int GridHeight = 150;
  private const int Scale = 4; // Scale up the pixels for visibility

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
    _brush = new Brush();

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

    _userInput.DetectInput(_world, _brush, Scale);

    _world.Update();
    base.Update(gameTime);
  }

  protected override void Draw(GameTime gameTime)
  {
    GraphicsDevice.Clear(Color.Black);

    Particle[,] currentGrid = _world.GetGrid();

    for (int y = 0; y < GridHeight; y++)
    {
      for (int x = 0; x < GridWidth; x++)
      {
        Particle p = currentGrid[x, y];
        Color cellColor = (p == null) ? Color.Transparent : p.Color;

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
