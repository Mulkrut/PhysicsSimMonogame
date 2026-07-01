using System;
using System.Runtime.CompilerServices;
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

        // Skips current cell, goes onto next
        if (p == null) continue;

        p.UpdateVisuals(x, y, world);
      }
    }
  }
}