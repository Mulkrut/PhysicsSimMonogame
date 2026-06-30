using Microsoft.Xna.Framework;
using System;

namespace Physics_Sim;

public class Water : Particle
{
  private static readonly Random _rng = new Random();

  public static readonly Color[] WaterPalette = new Color[]
  {
    new Color(15,  94,  156),
    new Color(35,  137, 218),
    new Color(28,  163, 236),
    new Color(90,  188, 216),
    new Color(116, 204, 244),
    new Color(240, 255, 255)
  };

  // Overriding the palette from the abstract class
  public override Color[] Palette => WaterPalette;

  public Water()
  {
    IsFalling = true;
    Velocity = new Vector2(0, 0);
    SettleCount = 0;
    Friction = 0;
    Density = 0.2f;
    FlowRange = 30;
    Depth = 0;
    Lifespan = 0;
    VisualOffset = _rng.Next(-1, 2);
    Color = WaterPalette[0];
  }

  public override Particle Clone() => new Water();
  
  public override void UpdateVisuals(int x, int y, World world)
  {
    int depth = 0;

    if (world.IsInBounds(x, y - 1))
    {
      Particle aboveP = world.GetParticle(x, y - 1);
      if (aboveP is Water)
      {
        depth = Math.Min(255, aboveP.Depth + 1);
      }
      else if (aboveP is Stone || aboveP is Wood)
      {
        depth = 8;
      }
    }

    int variableDepth = Math.Max(0, depth + VisualOffset);

    int finalIndex;
    if (variableDepth < 3) finalIndex = 4;
    else if (variableDepth < 6) finalIndex = 3;
    else if (variableDepth < 9) finalIndex = 2;
    else if (variableDepth < 12) finalIndex = 1;
    else finalIndex = 0;

    if (Velocity.Y > 2 || (Velocity.X == 1 && depth < 3))
    {
      finalIndex = 5;
    }

    Color finalColor = WaterPalette[finalIndex];
    world.SetParticleRenderState(x, y, finalColor, (byte)depth);
  }

  public override void Update(int x, int y, World world)
  {
    int flowRange = (world.frameCount + x + y) % 4 == 0 ? 40 : 20;

        if (Velocity.Y == 0 && Velocity.X == 0 && !IsFalling &&
            !world.CanMove(x, y + 1) &&
            !world.CanMove(x - 1, y) &&
            !world.CanMove(x + 1, y) &&
            !world.CanMove(x + 1, y + 1) &&
            !world.CanMove(x - 1, y + 1))
        {
            SettleCount = 50;
            Velocity.Y = 0;
            Velocity.X = 0;
            IsFalling = false;
            world.SetNextGrid(x, y, this);
            return;
        }

        if (PhysicsMovement.TryVertical(x, y, 1, world, this, world.CanMove, 0.4f, 10f)) return;
        if (PhysicsMovement.TryDensityVertical(x, y, 1, world, this)) return;
        if (PhysicsMovement.TryScatterDiagonal(x, y, 1, world, this, world.CanMove, world.CanMoveLiquid)) return;
        if (PhysicsMovement.TryFlowSide(x, y, flowRange, world, this)) return;

        int dir = PhysicsMovement.RandomDir();
        if (world.CanMove(x + dir, y) && world.GetParticle(x + dir, y - 1) is not Water)
        {
            world.MoveParticle(x, y, x + dir, y, this);
            return;
        }

        if (SettleCount < 5) Velocity.X = 1;
        else Velocity.X = 0;

        PhysicsMovement.SettleInPlace(x, y, world, this);
  }
}