using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace Physics_Sim;

public class World
{
  public readonly int Width;
  public readonly int Height;
  private readonly Random _rng = new Random();

  private Particle[,] _grid;
  private Particle[,] _nextGrid;
  
  public World(int width, int height)
  {
    Width = width;
    Height = height;
    _grid = new Particle[width, height];
    _nextGrid = new Particle[width, height];
  }

  public void SetCell(int x, int y, ParticleType type)
  {
    if (IsInBounds(x,y))
    {
      if (type == ParticleType.Air)
      {
        _grid[x, y] = Particle.Empty;
      }
      else if (type == ParticleType.Sand)
      {
        _grid[x, y] = new Particle
        {
          Type = ParticleType.Sand,
          IsFalling = true,
          VelocityY = 0,
          Color = Particle.SandPalette[_rng.Next(Particle.SandPalette.Length)]
        };
      }
    }
  }

public void Update()
  {
  // Copy current state to nextGrid buffer
  Array.Copy(_grid, _nextGrid, _grid.Length);

  // Iterate bottom-to-top to prevent "teleporting" sand
  for (int y = Height - 1; y >= 0; y--)
  {
    for (int x = 0; x < Width; x++)
    {
      if (_grid[x, y].Type == ParticleType.Sand) // If it's sand
      {
        UpdatePhysics(x, y);
      }
    }
  }

  // Apply all changes to the main grid
  Array.Copy(_nextGrid, _grid, _nextGrid.Length);
  }

  private void UpdatePhysics(int x, int y)
  {
    //checks if spot below is valid
    Particle p = _grid[x, y];
    bool moved = false;

    // Gravity, make its own later.
    if (canMove(x, y + 1))
    {
      p.IsFalling = true;
      if (p.VelocityY < 10) p.VelocityY+= 0.2f;
    }
    else
    {
      p.IsFalling = false;
      p.VelocityY = 0;
    }


    //Logic for falling straight down
    if (canMove(x, y + 1)) {
      p.IsFalling = true;
      int steps = (int)Math.Max(1, p.VelocityY);
      for (int d = steps; d > 0; d--)
      {
        if (canMove(x, y + d))
        {
          move(x, y, x, y + d, p);
          moved = true;
          break;
        }
      }
    }

    //Impact
  if (!moved && p.VelocityY > 2f)
    {
      int scatterDir = _rng.Next(2) == 0 ? -1 : 1;

      // Check if we can slide horizontally (or diagonally) to dissipate energy
      if (canMove(x + scatterDir, y + 1))
      {
        p.VelocityY *= 0.5f; // Lose half energy on impact
        move(x, y, x + scatterDir, y + 1, p);
        moved = true;
      }
      else if (canMove(x - scatterDir, y + 1))
      {
        p.VelocityY *= 0.5f;
        move(x, y, x - scatterDir, y + 1, p);
        moved = true;
      }
    }


    //Logic for falling to the side
    if (!moved)
    {
      // 2. Check diagonals (randomize order to keep piles even)
      int dir = _rng.Next(2) == 0 ? -1 : 1;
      if (canMove(x + dir, y + 1))
      {
          move(x, y, x + dir, y + 1, p);
          p.IsFalling = true;
          moved = true;
      }
      else if (canMove(x - dir, y + 1))
      {
          move(x, y, x - dir, y + 1, p);
          p.IsFalling = true;
          moved = true;
      }
    }

    //Logic if it cant move
    if (!moved)
    {
      p.IsFalling = false;
      p.VelocityY = 0;
      _nextGrid[x, y] = p;
    }
  }

  private bool canMove(int x, int y)
  {
      // Must be inside the screen and the target cell must be Air (0)
      return IsInBounds(x, y) && _grid[x, y].Type == ParticleType.Air;
  }

  private void move(int x1, int y1, int x2, int y2, Particle p)
  {
      _nextGrid[x1, y1] = Particle.Empty;    // Clear the old spot in the next frame
      _nextGrid[x2, y2] = p; // Set the new spot in the next frame
  }

  private bool IsInBounds(int x, int y)
  {
      return x >= 0 && x < Width && y >= 0 && y < Height;
  }

  public Particle[,] GetGrid() => _grid;  
}
