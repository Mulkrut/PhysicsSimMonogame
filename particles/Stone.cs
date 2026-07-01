using Microsoft.Xna.Framework;
using System;

namespace Physics_Sim;

public class Stone : Particle
{
  private static readonly Random _rng = new Random();

  public static readonly Color[] StonePalette = new Color[]
  {
    new Color(144,152,163), // just random greys
    new Color(156,156,156),
    new Color(152,160,167),
    new Color(140,141,141),
    new Color(142,148,148),
  };

  // Overriding the palette from the abstract class
  public override Color[] Palette => StonePalette;

  public Stone()
  {
    IsFalling = false;
    Velocity = new Vector2(0, 0);
    SettleCount = 0;
    Friction = 10;
    Density = 1f;
    FlowRange = 0;
    Depth = 12;
    Lifespan = 0;
    VisualOffset = 0;
    Color = StonePalette[_rng.Next(StonePalette.Length)];
  }

  public override Particle Clone() => new Stone();

  public override void UpdateVisuals(int x, int y, World world)
  {
    
  }

  public override void Update(int x, int y, World world)
  {
    
  }
}