using Microsoft.Xna.Framework;
using System;

namespace Physics_Sim;

public class Smoke : Particle
{
  private static readonly Random _rng = new Random();

  public static readonly Color[] SmokePalette = new Color[]
  {
    new Color(55,54,54), //dark to light
    new Color(111,111,111),
    new Color(150,146,146)
  };

  // Overriding the palette from the abstract class
  public override Color[] Palette => SmokePalette;

  public Smoke()
  {
    IsFalling = false;
    Velocity = new Vector2(0, 0);
    SettleCount = 0;
    Friction = 0;
    Density = -1f;
    FlowRange = 0;
    Depth = 0;
    Lifespan = _rng.Next(10, 30);
    VisualOffset = _rng.Next(-1, 2);
    Color = SmokePalette[_rng.Next(SmokePalette.Length)];
  }

  public override Particle Clone() => new Smoke();

  public override void UpdateVisuals(int x, int y, World world)
  {
    // Rendering logic here
  }

  public override void Update(int x, int y, World world)
  {
    if (_rng.Next(0, 10) < 3) Lifespan--;

      if (Lifespan <= 0)
      {
        world.DeleteParticle(x, y);
        return;
      }

      int swayDir = PhysicsMovement.RandomSway(5, 4);

      if (PhysicsMovement.TryVerticalWithSway(x, y, swayDir, -1, world, this, world.CanMoveLiquid, 0.1f, 2f)) return;
      if (PhysicsMovement.TryDensityVerticalWithSway(x, y, swayDir, -1, world, this)) return;
      if (PhysicsMovement.TryScatterDiagonal(x, y, -1, world, this, world.CanMove, world.CanMove)) return;
      if (PhysicsMovement.TrySlideHorizontal(x, y, world, this, world.CanMove, increaseSettleOnSlide: true)) return;

      world.SetNextGrid(x, y, this);
  }
}