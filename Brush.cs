using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace Physics_Sim;

public class Brush
{
    public int Size = 5; // The radius of the sphere
    public ParticleType SelectedType = ParticleType.Sand;

    public void Draw(int mouseX, int mouseY, World world)
    {
      for (int y = -Size; y <= Size; y++)
      {
        for (int x = -Size; x <= Size; x++)
        {
            // Check if the current point is inside the circle radius
            if (x* x + y* y <= Size* Size)
            {
                int targetX = mouseX + x;
                int targetY = mouseY + y;

                // Set the cell in the world
                world.SetCell(targetX, targetY, SelectedType);
            }
        }
      }
    }
}