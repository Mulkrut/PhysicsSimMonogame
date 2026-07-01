using Microsoft.Xna.Framework;
using System;

namespace Physics_Sim;

public class Wood : Particle
{
  private static readonly Random _rng = new Random();

  public static readonly Color[] WoodPalette = new Color[]
  {
    new Color(136,99,66),
    new Color(135,103,79),
    new Color(136,108,80)
  };

  // Overriding the palette from the abstract class
  public override Color[] Palette => WoodPalette;

  public Wood()
  {
    IsFalling = false;
    Velocity = new Vector2(0, 0);
    SettleCount = 0;
    Friction = 3;
    Density = 1f;
    FlowRange = 0;
    Depth = 0;
    Lifespan = 0;
    VisualOffset = 0;
    Color = WoodPalette[_rng.Next(WoodPalette.Length)];
  }

  public override Particle Clone() => new Wood();

  public override void UpdateVisuals(int x, int y, World world)
  {
    
  }

  public override void Update(int x, int y, World world)
  {
    
  }
}