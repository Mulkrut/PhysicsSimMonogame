using System;
using Microsoft.Xna.Framework;

namespace Physics_Sim;

public static class ParticleRender
{
  private static readonly Random _rng = new Random();

  public static void DepthCalculate(int x, int y, World world)
  {
      int depth = 0;
      
      if (world.IsInBounds(x, y - 1))
      {
          Particle aboveP = world.GetParticle(x, y - 1);
          if (aboveP.Type == ParticleType.Water)
          {
              depth = aboveP.Depth + 1;
          }
          else if (aboveP.Type == ParticleType.Stone) depth = 8;
      }

      Particle p = world.GetParticle(x, y);

      //Variation based on itself
      int variableDepth = depth + p.VisualOffset;

      int finalIndex = 0;
      // Calculate color based on depth
      if (variableDepth < 3)       finalIndex = 4; // Depth 0, 1, 2   -> Lightest Blue
      else if (variableDepth < 6)  finalIndex = 3; // Depth 3, 4, 5   -> Medium Light Blue
      else if (variableDepth < 9)  finalIndex = 2; // Depth 6, 7, 8   -> Medium Blue
      else if (variableDepth < 12) finalIndex = 1; // Depth 9, 10, 11 -> Medium Dark Blue
      else finalIndex = 0; // Depth 12+       -> Darkest Blue

      if (p.VelocityY > 2 || (p.VelocityX == 1 && depth < 3)) finalIndex = 5;
      
      // Old variaton flickered too much
      // int variation = _rng.Next(2) == 0 ? 0 : 1;
      // if (depth == 2 || depth == 5 || depth == 8 || depth == 11)
      // {
      //   finalIndex -= variation;
      // }

      Color finalColor = Particle.waterPalette[finalIndex];

      // Update it directly in place on the active grid!
      world.UpdateWaterVisuals(x, y, depth, finalColor);
  }
}