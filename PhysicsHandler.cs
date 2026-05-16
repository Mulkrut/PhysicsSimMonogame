using System;
using System.Drawing;

namespace Physics_Sim;

public static class PhysicsHandler
{
  private static readonly Random _rng = new Random();

  public static void UpdateParticle(int x, int y, World world)
  {
    Particle p = world.GetParticle(x, y);

    if (p.Type == ParticleType.Sand)
    {
      HandleSandMovement(x, y, world, ref p);
    }
    else if (p.Type == ParticleType.Water)
    {
      HandleWaterMovement(x, y, world, ref p);
    }
  }


  private static void HandleSandMovement(int x, int y, World world, ref Particle p)
  {

    // 1. SLEEP CHECK: If already settled, don't even look at it
    if (p.SettleCount >= 5 && !world.CanMove(x,y + 1))
    {
      world.SetNextGrid(x, y, p); // Keep it where it is
      return;
    }

    // 2. GRAVITY & VERTICAL FALL
    if (world.CanMove(x, y + 1))
    {
      p.IsFalling = true;
      if (p.VelocityY < 10) p.VelocityY += 0.2f;

      int steps = (int)Math.Max(1, p.VelocityY);
      for (int d = steps; d > 0; d--)
      {
        if (world.CanMove(x, y + d))
        {
          p.SettleCount = 0; // Falling reset
          world.MoveParticle(x, y, x, y + d, p);
          return; // Exit early!
        }
      }
    }

    // 3. SLOPES (Diagonals)
    int dir = _rng.Next(2) == 0 ? -1 : 1;
    if (world.CanMove(x + dir, y + 1))
    {
      p.SettleCount = 0; // Diagonals count as movement
      world.MoveParticle(x, y, x + dir, y + 1, p);
      return;
    }
    else if (world.CanMove(x - dir, y + 1))
    {
      p.SettleCount = 0;
      world.MoveParticle(x, y, x - dir, y + 1, p);
      return;
    }

    // 4. HORIZONTAL SLIDING (The "Settling" Phase)
    // We do NOT reset SettleCount to 0 here. This allows them to slide a bit then STOP.
    if (world.CanMove(x + dir, y))
    {
      p.SettleCount++;
      world.MoveParticle(x, y, x + dir, y, p);
      return;
    }
    else if (world.CanMove(x - dir, y))
    {
      p.SettleCount++;
      world.MoveParticle(x, y, x - dir, y, p);
      return;
    }

    // 5. IF NO MOVEMENT POSSIBLE
    p.SettleCount++; // Get closer to sleep
    p.VelocityY = 0;
    p.IsFalling = false;
    world.SetNextGrid(x, y, p);
  }


  private static void HandleWaterMovement(int x, int y, World world, ref Particle p)
  {

    // 1. SLEEP CHECK: If already settled, don't even look at it
    if (p.SettleCount >= 10 && !world.CanMove(x, y + 1) && !world.CanMove(x + 1, y + 1) && !world.CanMove(x - 1, y + 1))
    {
      if (world.CanMove(x + 1,y) || world.CanMove(x - 1, y) && p.IsFalling == false)
      {
        //test coloring
        p.Color = new Microsoft.Xna.Framework.Color(116, 204, 244);
        //if (TryFlowSide(x, y, 1, 2, world, ref p)) return;
      }
      else
      {
      world.SetNextGrid(x, y, p); // Keep it where it is
      return;
      }
    }

    // 2. GRAVITY & VERTICAL FALL
    if (world.CanMove(x, y + 1))
    {
      p.IsFalling = true;
      if (p.VelocityY < 10) p.VelocityY += 0.2f;

      int steps = (int)Math.Max(1, p.VelocityY);
      for (int d = steps; d > 0; d--)
      {
        if (world.CanMove(x, y + d))
        {
          p.SettleCount = 0; // Falling reset
          world.MoveParticle(x, y, x, y + d, p);
          return; // Exit early!
        }
      }
    }

    // 3. SLOPES (Diagonals)
    int dir = _rng.Next(2) == 0 ? -1 : 1;
    if (world.CanMove(x + dir, y + 1))
    {
      p.SettleCount = 0; // Diagonals count as movement
      world.MoveParticle(x, y, x + dir, y + 1, p);
      return;
    }
    else if (world.CanMove(x - dir, y + 1))
    {
      p.SettleCount = 0;
      world.MoveParticle(x, y, x - dir, y + 1, p);
      return;
    }


    int flowRange = 20;    

    if (TryFlowSide(x, y, dir, flowRange, world, ref p)) return;

    // 5. IF NO MOVEMENT POSSIBLE
    p.SettleCount++; // Get closer to sleep
    p.VelocityY = 0;
    p.IsFalling = false;
    world.SetNextGrid(x, y, p);
  }

  private static bool TryFlowSide(int startX, int startY, int direction, int maxRange, World world, ref Particle p)
  {
    int leftAirScore = maxRange;
    int rightAirScore = maxRange;

    // scan right for pit
    for (int i = 1; i <= maxRange; i++)
    {
      int checkX = startX + i;
      if (!world.CanMoveLiquid(checkX, startY)) break;
      if (world.CanMove(checkX, startY - 1))
      {
        rightAirScore = i;
        break;
      }
    }

    // Scan Left
    for (int i = 1; i <= maxRange; i++)
    {
      int checkX = startX - i;
      if (!world.CanMoveLiquid(checkX, startY)) break;
      if (world.CanMove(checkX, startY - 1))
      {
        leftAirScore = i;
        break;
      }
    }

    int finalDir;
    if (rightAirScore < leftAirScore) finalDir = 1;
    else if (leftAirScore < rightAirScore) finalDir = -1;
    else finalDir = direction;
    
    if (finalDir != 0 && world.CanMove(startX + finalDir, startY))
    {
      world.MoveParticle(startX, startY, startX + finalDir, startY, p);
      p.SettleCount = 0;
      return true;
    }

    return false;
  }
}