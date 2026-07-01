using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace Physics_Sim;

public class World
{
  public readonly int Width;
  public readonly int Height;
  public int frameCount = 0;

  private Particle[,] _grid;
  private Particle[,] _nextGrid;

  public World(int width, int height)
  {
    Width = width;
    Height = height;
    _grid = new Particle[width, height];
    _nextGrid = new Particle[width, height];
  }

  public void Update()
  {
    RunSimulationStep();
    CommitNextGrid();
    ParticleRender.UpdateVisuals(this);
  }

  public void RunSimulationStep()
  {
    Array.Copy(_grid, _nextGrid, _grid.Length);
    bool reverse = frameCount % 2 == 0;

    for (int y = Height - 1; y >= 0; y--)
    {
      if (reverse)
      {
        for (int x = 0; x < Width; x++)
        {
          UpdateCell(x, y);
        }
      }
      else
      {
        for (int x = Width - 1; x >= 0; x--)
        {
          UpdateCell(x, y);
        }
      }
    }

    frameCount++;
  }

  public void CommitNextGrid()
  {
    var temp = _grid;
    _grid = _nextGrid;
    _nextGrid = temp;
  }

  public void UpdateCell(int x, int y)
  {
    Particle p = _grid[x, y];
    if (p == null) return;

    p.Update(x, y, this);
  }

  // --- Helper Methods for PhysicsHandler ---

  public bool CanMove(int x, int y)
  {
    return IsInBounds(x, y) &&
           _grid[x, y] == null &&
           _nextGrid[x, y] == null;
  }


  public bool CanMoveDensity(int x, int y, Particle p)
  {
    if (!IsInBounds(x, y)) return false;
    
    float gridDensity = _grid[x, y]?.Density ?? 0f;
    float nextDensity = _nextGrid[x, y]?.Density ?? 0f;
    
    if (gridDensity == p.Density) return false;
    return gridDensity < p.Density && nextDensity < p.Density;
  }

  //checks if target is valid (air or water)
  public bool CanMoveLiquid(int x, int y)
  {
    return IsInBounds(x, y) &&
           (_grid[x, y] == null || _grid[x, y] is Water) &&
           (_nextGrid[x, y] == null || _nextGrid[x, y] is Water);
  }

  public void MoveParticle(int x1, int y1, int x2, int y2, Particle p)
  {
    _nextGrid[x1, y1] = null;
    _nextGrid[x2, y2] = p;

    // Wake up the particle directly above the one that just moved
    if (p is Fire)
    {
        if (IsInBounds(x1, y1 - 1) && _nextGrid[x1, y1 - 1] != null)
            _nextGrid[x1, y1 - 1].SettleCount = 0;
        if (IsInBounds(x2 + 1, y2) && _nextGrid[x2 + 1, y2] != null)
            _nextGrid[x2 + 1, y2].SettleCount = 0;
        if (IsInBounds(x2 - 1, y2) && _nextGrid[x2 - 1, y2] != null)
            _nextGrid[x2 - 1, y2].SettleCount = 0;
    }
  }

  public void DeleteParticle(int x, int y)
  {
    // C# GC automatically deletes the particle when no reference remains
    _nextGrid[x, y] = null;
  }

  public void SwapParticle(int x1, int y1, int x2, int y2, Particle p)
  {
    Particle temp = GetParticle(x2, y2);

    _nextGrid[x1, y1] = temp;
    _nextGrid[x2, y2] = p;

    if (_nextGrid[x1, y1] != null) _nextGrid[x1, y1].SettleCount = 0;
    if (_nextGrid[x2, y2] != null) _nextGrid[x2, y2].SettleCount = 0;
  }


  public void SetNextGrid(int x, int y, Particle p) => _nextGrid[x, y] = p;


  public void SetNewNextGrid(int x, int y, Particle p)
  {
    if (!IsInBounds(x, y)) return;
    {
      _nextGrid[x, y] = p;
    }
  }

  public Particle GetParticle(int x, int y) => IsInBounds(x, y) ? _grid[x, y] : null;

  public bool IsInBounds(int x, int y) =>
      x >= 0 && x < Width && y >= 0 && y < Height;

  public void SetCell(int x, int y, Particle p)
  {
    if (!IsInBounds(x, y)) return;
    {
      _grid[x, y] = p;
    }
  }

  //Visualiser helper Methods

  public void SetParticleColor(int x, int y, Color color)
  {
    if (!IsInBounds(x, y)) return;
    _grid[x, y].Color = color;
  }

  public void SetParticleDepth(int x, int y, byte depth)
  {
    if (!IsInBounds(x, y)) return;
    _grid[x, y].Depth = depth;
  }

  public void SetParticleRenderState(int x, int y, Color color, byte? depth = null)
  {
    if (!IsInBounds(x, y)) return;

    _grid[x, y].Color = color;
    if (depth.HasValue)
    {
      _grid[x, y].Depth = depth.Value;
    }
  }

  public Particle[,] GetGrid() => _grid;
}
