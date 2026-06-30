using Microsoft.Xna.Framework;
using System;

namespace Physics_Sim;

public class Sand : Particle
{
  private static readonly Random _rng = new Random();

  public static readonly Color[] SandPalette = new Color[]
  {
    new Color(236, 204, 162), // Classic Sand
    new Color(225, 191, 145), // Slightly darker
    new Color(203, 178, 121), // Earthy tan
    new Color(210, 180, 140), // Tan
    new Color(194, 178, 128)  // Ecru/Dark Sand
  };

  // Overriding the palette from the abstract class
  public override Color[] Palette => SandPalette;

  public Sand()
  {
    IsFalling = true;
    Velocity = new Vector2(0, 0);
    SettleCount = 0;
    Friction = 5; // Default friction value for sand grains
    Density = 0.6f;
    FlowRange = 3;
    Depth = 0;
    Lifespan = 0;
    VisualOffset = 0;
    Color = SandPalette[_rng.Next(SandPalette.Length)];
  }

  public override Particle Clone() => new Sand();

  public override void UpdateVisuals(int x, int y, World world)
  {
    // Rendering logic here
  }

  public override void Update(int x, int y, World world)
  {
    if (SettleCount >= 5 && !world.CanMoveDensity(x, y + 1, this))
    {
        PhysicsMovement.KeepInPlace(x, y, world, this);
        return;
    }

    if (PhysicsMovement.TryVertical(x, y, 1, world, this, world.CanMove, 0.2f, 10f)) return;
    if (PhysicsMovement.TryDensityVertical(x, y, 1, world, this)) return;
    if (PhysicsMovement.TryScatterDiagonal(x, y, 1, world, this, world.CanMove, world.CanMove)) return;
    if (PhysicsMovement.TrySlideHorizontal(x, y, world, this, world.CanMove, increaseSettleOnSlide: true)) return;

    if (SettleCount < 3) Velocity.X = 1;
    else Velocity.X = 0;

    PhysicsMovement.SettleInPlace(x, y, world, this, zeroVelocityX: false);
  }
}