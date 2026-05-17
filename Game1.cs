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
  private Brush _brush;
  private Texture2D _gridTexture;
  private Color[] _colorBuffer;
  
  private const int GridWidth = 160;
  private const int GridHeight = 100;
  private const int Scale = 8; // Scale up the pixels for visibility

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

    var mouseState = Mouse.GetState();
    var kbState = Keyboard.GetState();


    //Adjusting Brush size
    if (kbState.IsKeyDown(Keys.Up)) _brush.Size++;
    if (kbState.IsKeyDown(Keys.Down) && _brush.Size > 0) _brush.Size--;

    // Changing brush type
    if (kbState.IsKeyDown(Keys.Q)) _brush.SelectedType = ParticleType.Sand;
    if (kbState.IsKeyDown(Keys.W)) _brush.SelectedType = ParticleType.Water;

    //Shortcuts
    //Reset
    if (kbState.IsKeyDown(Keys.R))
    {
      for (int y = 0; y < GridHeight; y++)
      {
        for (int x = 0; x < GridWidth; x++)
        {
          _world.SetCell(x, y, ParticleType.Air);
        }
      }
    }

    if (mouseState.LeftButton == ButtonState.Pressed)
    {
      int gridX = mouseState.X / Scale;
      int gridY = mouseState.Y / Scale;

      _brush.Draw(gridX, gridY, _world);
    }

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
        Color cellColor = (p.Type == ParticleType.Air) ? Color.Transparent : p.Color;

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

