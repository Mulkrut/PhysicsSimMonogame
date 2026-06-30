using Microsoft.Xna.Framework;
using System;

namespace Physics_Sim;

public class Fire : Particle
{
  private static readonly Random _rng = new Random();

  public static readonly Color[] FirePalette = new Color[]
  {
    new Color(237,31,10), // Goes red to yellow
    new Color(248,66,11),
    new Color(249,85,4),
    new Color(247,111,11),
    new Color(255,144,10),
    new Color(255,193,0)
  };

  // Overriding the palette from the abstract class
  public override Color[] Palette => FirePalette;

  public Fire()
  {
    IsFalling = false;
    Velocity = new Vector2(0, 0);
    SettleCount = 0;
    Friction = 0;
    Density = -0.5f;
    FlowRange = 0;
    Depth = 12;
    Lifespan = _rng.Next(10, 30);
    VisualOffset = _rng.Next(-1, 2);
    Color = FirePalette[0];
  }

  public override Particle Clone() => new Fire();

  public override void UpdateVisuals(int x, int y, World world)
  {
    int crammed = 0;
    Particle p = world.GetParticle(x, y);

    if (world.IsInBounds(x, y - 1) && world.GetParticle(x, y - 1) is Fire) crammed++;
    if (world.IsInBounds(x + 1, y) && world.GetParticle(x + 1, y) is Fire) crammed++;
    if (world.IsInBounds(x - 1, y) && world.GetParticle(x - 1, y) is Fire) crammed++;
    if (world.IsInBounds(x, y + 1) && world.GetParticle(x, y + 1) is Fire) crammed++;

    int finalIndex = Math.Clamp(crammed + p.VisualOffset, 0, 5);
    Color finalColor = FirePalette[finalIndex];
    world.SetParticleColor(x, y, finalColor);
  }

  public override void Update(int x, int y, World world)
  {
    if (Lifespan <= 0)
    {
      world.DeleteParticle(x, y);
      if (_rng.Next(2) == 0 && world.CanMove(x, y - 1))
      {
          world.SetNewNextGrid(x, y - 1, new Smoke());
      }
      return;
    }

    if (_rng.Next(4) == 0) Lifespan--;

    Particle right = world.GetParticle(x + 1, y);
    Particle left = world.GetParticle(x - 1, y);
    Particle up = world.GetParticle(x, y - 1);
    Particle down = world.GetParticle(x, y + 1);

    if (right is Water || left is Water || up is Water || down is Water)
    {
        world.DeleteParticle(x, y);
        world.SetNewNextGrid(x, y, new Smoke());
        return;
    }

    if (right is Wood)
    {
        Lifespan += 2;
        world.SetNewNextGrid(x + 1, y, new Fire());
        if (world.CanMove(x + 1, y + 1)) world.SetNewNextGrid(x + 1, y + 1, new Smoke());
        return;
    }
    if (left is Wood)
    {
        Lifespan += 2;
        world.SetNewNextGrid(x - 1, y, new Fire());
        if (world.CanMove(x - 1, y + 1)) world.SetNewNextGrid(x - 1, y + 1, new Smoke());
        return;
    }
    if (down is Wood)
    {
        Lifespan += 2;
        world.SetNewNextGrid(x, y + 1, new Fire());
        if (world.CanMove(x, y + 2)) world.SetNewNextGrid(x, y + 2, new Smoke());
        return;
    }
    if (up is Wood)
    {
        Lifespan += 2;
        world.SetNewNextGrid(x, y - 1, new Fire());
        return;
    }

    int swayDir = PhysicsMovement.RandomSway(3, 2);

    if (SettleCount > 5)
    {
        if (PhysicsMovement.TryVerticalWithSway(x, y, swayDir, -1, world, this, world.CanMove, 0.1f, 3f)) return;
        if (PhysicsMovement.TryDensityVerticalWithSway(x, y, swayDir, -1, world, this)) return;
        if (PhysicsMovement.TryScatterDiagonal(x, y, -1, world, this, world.CanMove, world.CanMove, resetSettle: false)) return;
    }
    else if (_rng.Next(3) == 2)
    {
        SettleCount++;
    }

    if (PhysicsMovement.TrySlideHorizontal(x, y, world, this, world.CanMove)) return;

    world.SetNextGrid(x, y, this);
  }
}