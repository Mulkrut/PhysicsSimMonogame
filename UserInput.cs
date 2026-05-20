using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input; // CRITICAL: Added for Mouse, Keyboard, and Keys
using System;

namespace Physics_Sim;


public class UserInput
{

  public void DetectInput(World world, Brush brush, int scale)
  {
    var mouseState = Mouse.GetState();
    var kbState = Keyboard.GetState();


    //Adjusting Brush size
    if (kbState.IsKeyDown(Keys.Up)) brush.Size++;
    if (kbState.IsKeyDown(Keys.Down) && brush.Size > 0) brush.Size--;

    // Changing brush type
    if (kbState.IsKeyDown(Keys.Q)) brush.SelectedType = ParticleType.Sand;
    if (kbState.IsKeyDown(Keys.W)) brush.SelectedType = ParticleType.Water;
    if (kbState.IsKeyDown(Keys.E)) brush.SelectedType = ParticleType.Air;
    if (kbState.IsKeyDown(Keys.S)) brush.SelectedType = ParticleType.Stone;
    if (kbState.IsKeyDown(Keys.A)) brush.SelectedType = ParticleType.Fire;
    if (kbState.IsKeyDown(Keys.D)) brush.SelectedType = ParticleType.Wood;
    if (kbState.IsKeyDown(Keys.F)) brush.SelectedType = ParticleType.Smoke;

    //Shortcuts
    //Reset
    if (kbState.IsKeyDown(Keys.R))
    {
      for (int y = 0; y < world.Height; y++)
      {
        for (int x = 0; x < world.Width; x++)
        {
          world.SetCell(x, y, ParticleType.Air);
        }
      }
    }

    if (mouseState.LeftButton == ButtonState.Pressed)
    {
      int gridX = mouseState.X / scale;
      int gridY = mouseState.Y / scale;

      brush.Draw(gridX, gridY, world);
    }
  }
}