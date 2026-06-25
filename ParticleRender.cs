using System;
using Microsoft.Xna.Framework;

namespace Physics_Sim;

public static class ParticleRender
{
  public static void UpdateVisuals(World world)
  {
    for (int y = 0; y < world.Height; y++)
    {
      for (int x = 0; x < world.Width; x++)
      {
        Particle p = world.GetParticle(x, y);
        switch (p.Type)
        {
          case ParticleType.Water:
            UpdateWaterVisual(x, y, world);
            break;
          case ParticleType.Fire:
            UpdateFireVisual(x, y, world);
            break;
        }
      }
    }
  }



  public static void UpdateWaterVisual(int x, int y, World world)
  {
    int depth = 0;

    if (world.IsInBounds(x, y - 1))
    {
      Particle aboveP = world.GetParticle(x, y - 1);
      if (aboveP.Type == ParticleType.Water)
      {
        depth = Math.Min(255, aboveP.Depth + 1);
      }
      else if (aboveP.Type == ParticleType.Stone || aboveP.Type == ParticleType.Wood)
      {
        depth = 8;
      }
    }

    Particle p = world.GetParticle(x, y);
    int variableDepth = Math.Max(0, depth + p.VisualOffset);

    int finalIndex;
    if (variableDepth < 3) finalIndex = 4;
    else if (variableDepth < 6) finalIndex = 3;
    else if (variableDepth < 9) finalIndex = 2;
    else if (variableDepth < 12) finalIndex = 1;
    else finalIndex = 0;

    if (p.VelocityY > 2 || (p.VelocityX == 1 && depth < 3))
    {
      finalIndex = 5;
    }

    Color finalColor = Particle.waterPalette[finalIndex];
    world.SetParticleRenderState(x, y, finalColor, (byte)depth);
  }

  public static void UpdateFireVisual(int x, int y, World world)
  {
    int crammed = 0;
    Particle p = world.GetParticle(x, y);

    if (world.IsInBounds(x, y - 1) && world.GetParticle(x, y - 1).Type == p.Type) crammed++;
    if (world.IsInBounds(x + 1, y) && world.GetParticle(x + 1, y).Type == p.Type) crammed++;
    if (world.IsInBounds(x - 1, y) && world.GetParticle(x - 1, y).Type == p.Type) crammed++;
    if (world.IsInBounds(x, y + 1) && world.GetParticle(x, y + 1).Type == p.Type) crammed++;

    int finalIndex = Math.Clamp(crammed + p.VisualOffset, 0, 5);
    Color finalColor = Particle.firePalette[finalIndex];
    world.SetParticleColor(x, y, finalColor);
  }
}