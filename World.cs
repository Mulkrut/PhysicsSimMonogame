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

  private int[,] _grid;
  private int[,] _nextGrid;
  
  public World(int width, int height)
  {
    Width = width;
    Height = height;
    _grid = new int[width, height];
    _nextGrid = new int[width, height];
  }

  public void SetCell(int x, int y, int type)
  {
    if (IsInBounds(x,y))
    {
      _grid[x, y] = type;
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
      if (_grid[x, y] == 1) // If it's sand
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
    if (canMove(x, y + 1)) {
      move(x, y, x, y + 1);
    }
    else
    {
      // 2. Check diagonals (randomize order to keep piles even)
      int dir = _rng.Next(2) == 0 ? -1 : 1;

      if (canMove(x + dir, y + 1))
      {
          move(x, y, x + dir, y + 1);
      }
      else if (canMove(x - dir, y + 1))
      {
          move(x, y, x - dir, y + 1);
      }
    }
  }

  private bool canMove(int x, int y)
  {
      // Must be inside the screen and the target cell must be Air (0)
      return IsInBounds(x, y) && _grid[x, y] == 0;
  }

  private void move(int x1, int y1, int x2, int y2)
  {
      int type = _grid[x1, y1];
      _nextGrid[x1, y1] = 0;    // Clear the old spot in the next frame
      _nextGrid[x2, y2] = type; // Set the new spot in the next frame
  }

  private bool IsInBounds(int x, int y)
  {
      return x >= 0 && x < Width && y >= 0 && y < Height;
  }

  public int[,] GetGrid() => _grid;  
}
