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
    else if (p.Type == ParticleType.Fire)
    {
      HandleFireMovement(x, y, world, ref p);
    }
    else if (p.Type == ParticleType.Smoke)
    {
      HandleSmokeMovement(x, y, world, ref p);
    }
  }


  private static void HandleSandMovement(int x, int y, World world, ref Particle p)
  {

    //SLEEP CHECK: If already settled, don't even look at it
    if (p.SettleCount >= 5 && !world.CanMoveDensity(x,y + 1, p))
    {
      world.SetNextGrid(x, y, p); // Keep it where it is
      return;
    }

    //GRAVITY & VERTICAL FALL
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

    //Density fall like gravity but removes Velocity
    if (world.CanMoveDensity(x, y + 1, p))
    {
      p.IsFalling = true;
      if (p.VelocityY > 1.2) p.VelocityY -= 0.2f;

      world.SwapParticle(x, y, x, y + 1, p);
      return;
    }


    //SLOPES
    int dir = _rng.Next(2) == 0 ? -1 : 1;
    if (world.CanMove(x + dir, y + 1) && (world.CanMove(x + dir, y)))
    {
      p.SettleCount = 0; // Diagonals count as movement
      world.MoveParticle(x, y, x + dir, y + 1, p);
      return;
    }
    else if (world.CanMove(x - dir, y + 1) && (world.CanMove(x - dir, y)))
    {
      p.SettleCount = 0;
      world.MoveParticle(x, y, x - dir, y + 1, p);
      return;
    }

    //HORIZONTAL SLIDING
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

    if (p.SettleCount < 3) p.VelocityX = 1;
    else p.VelocityX = 0;

    // 5. IF NO MOVEMENT POSSIBLE
    p.SettleCount++; // Get closer to sleep
    p.VelocityY = 0;
    p.IsFalling = false;
    world.SetNextGrid(x, y, p);
  }




private static void HandleFireMovement(int x, int y, World world, ref Particle p)
  {
    //Lifespan
    if (p.Lifespan <= 0)
    {
      world.DeleteParticle(x, y);
      if (_rng.Next(2) == 0)
      {
        if (world.CanMove(x, y - 1)) world.SetNewNextGrid(x, y - 1, ParticleType.Smoke);
      }
      return;
    }

    //Decreases lifespan if smooving
    if (_rng.Next(4) == 0) p.Lifespan--;

    //4 surrounding tiles, used for water and wood checks
    ParticleType right = world.GetParticleType(x + 1, y);
    ParticleType left  = world.GetParticleType(x - 1, y);
    ParticleType up    = world.GetParticleType(x, y - 1);
    ParticleType down  = world.GetParticleType(x, y + 1);


    //Touch with water
    if (right == ParticleType.Water ||
        left == ParticleType.Water ||
        up == ParticleType.Water ||
        down == ParticleType.Water)
    {
      world.DeleteParticle(x, y);
      world.SetNewNextGrid(x, y, ParticleType.Smoke);
      return;
    }

    //Touch with wood, multiplies and creates smoke
    if (right == ParticleType.Wood)
    {
      p.Lifespan += 2;
      world.SetNewNextGrid(x + 1, y, ParticleType.Fire);
      if (world.CanMove(x + 1, y + 1)) world.SetNewNextGrid(x + 1, y + 1, ParticleType.Smoke);
      return;
    }
    if (left == ParticleType.Wood)
    {
      p.Lifespan += 2;
      world.SetNewNextGrid(x - 1, y, ParticleType.Fire);
      if (world.CanMove(x - 1, y + 1)) world.SetNewNextGrid(x - 1, y + 1, ParticleType.Smoke);
      return;
    }
    if (down == ParticleType.Wood)
    {
      p.Lifespan += 2;
      world.SetNewNextGrid(x, y + 1, ParticleType.Fire);
      if (world.CanMove(x, y + 2)) world.SetNewNextGrid(x, y + 2, ParticleType.Smoke);
      return;
    }
    if (up == ParticleType.Wood)
    {
      p.Lifespan += 2;
      world.SetNewNextGrid(x, y - 1, ParticleType.Fire);
      return;
    }


    //Random movement left and right
    int swayRand = _rng.Next(3);
    int swayDir = 0;
    if (swayRand == 0)      swayDir = -1; // 1/3 chance left
    else if (swayRand == 2) swayDir = 1;  // 1/3 chance right

    int dir = _rng.Next(2) == 0 ? -1 : 1;


    //Using settlecount to determine if it want to move up or stay in its current y
    if (p.SettleCount > 5)
    {

      //GRAVITY & VERTICAL FALL
      int fallX = x + swayDir;
      if (world.CanMove(fallX, y - 1))
      {
        if (p.VelocityY < 3) p.VelocityY += 0.1f;

        int steps = (int)Math.Max(1, p.VelocityY);
        for (int d = steps; d > 0; d--)
        {
          if (world.CanMove(fallX, y - d))
          {
            world.MoveParticle(x, y, fallX, y - d, p);
            return;
          }
        }
      }

      //Density fall like gravity but removes Velocity
      int densityX = x + swayDir;
      if (world.CanMoveDensity(densityX, y - 1, p))
      {
        if (p.VelocityY > 1.2) p.VelocityY += 0.2f;
        world.SwapParticle(x, y, densityX, y - 1, p);
        return;
      }


      //SLOPES
      if (world.CanMove(x + dir, y - 1) && (world.CanMove(x + dir, y)))
      {
        world.MoveParticle(x, y, x + dir, y - 1, p);
        return;
      }
      else if (world.CanMove(x - dir, y - 1) && (world.CanMove(x - dir, y)))
      {
        world.MoveParticle(x, y, x - dir, y - 1, p);
        return;
      }
    }
    //random settlecount increase
    else if (_rng.Next(3) == 2) p.SettleCount++;

    //HORIZONTAL SLIDING
    if (world.CanMove(x + dir, y))
    {
      world.MoveParticle(x, y, x + dir, y, p);
      return;
    }
    else if (world.CanMove(x - dir, y))
    {
      world.MoveParticle(x, y, x - dir, y, p);
      return;
    }

    //IF NO MOVEMENT POSSIBLE
    world.SetNextGrid(x, y, p);
  }


private static void HandleSmokeMovement(int x, int y, World world, ref Particle p)
  {
    //lifespan decrement
    if (_rng.Next(0, 10) < 3)
    {
      p.Lifespan--;
    }

    //Lifespan
    if (p.Lifespan <= 0)
    {
      world.DeleteParticle(x, y);
      return;
    }

    //Random movement left and right
    int swayRand = _rng.Next(5);
    int swayDir = 0;
    if (swayRand == 0)      swayDir = -1; // 1/3 chance left
    else if (swayRand == 4) swayDir = 1;  // 1/3 chance right

    int dir = _rng.Next(2) == 0 ? -1 : 1;

    //GRAVITY & VERTICAL FALL
    int fallX = x + swayDir;
    if (world.CanMoveLiquid(fallX, y - 1))
    {
      p.IsFalling = true;
      if (p.VelocityY < 2) p.VelocityY += 0.1f;

      int steps = (int)Math.Max(1, p.VelocityY);
      for (int d = steps; d > 0; d--)
      {
        if (world.CanMoveLiquid(fallX, y - d))
        {
          world.MoveParticle(x, y, fallX, y - d, p);
          return; // Exit early!
        }
      }
    }

    //Density fall like gravity but removes Velocity
    int densityX = x + swayDir;
    if (world.CanMoveDensity(densityX, y - 1, p))
    {
      if (p.VelocityY > 1.2) p.VelocityY += 0.2f;
      world.SwapParticle(x, y, densityX, y - 1, p);
      return;
    }


    //SLOPES
    if (world.CanMove(x + dir, y - 1) && (world.CanMove(x + dir, y)))
    {
      p.SettleCount = 0; // Diagonals count as movement
      world.MoveParticle(x, y, x + dir, y - 1, p);
      return;
    }
    else if (world.CanMove(x - dir, y - 1) && (world.CanMove(x - dir, y)))
    {
      p.SettleCount = 0;
      world.MoveParticle(x, y, x - dir, y - 1, p);
      return;
    }


    //HORIZONTAL SLIDING
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

    world.SetNextGrid(x, y, p);
  }

  private static void HandleWaterMovement(int x, int y, World world, ref Particle p)
  {
    int dir = _rng.Next(2) == 0 ? -1 : 1;

    int flowRange = 20;
    if ((world.frameCount + x + y) % 4 == 0)
    {
      flowRange = 40;
    }


    //Optimizing, puts to sleep if surrounded
    if (p.VelocityY == 0 && p.VelocityX == 0 && !p.IsFalling &&
        !world.CanMove(x, y + 1) && // Down
        !world.CanMove(x - 1, y) && // Left
        !world.CanMove(x + 1, y) && // Right
        !world.CanMove(x + 1, y + 1) && // diag left
        !world.CanMove(x - 1, y + 1)) // Diag right
        // !world.CanMove(x, y - 1))   // Up
    {
        p.SettleCount = 50; // Force maximum sleep state
        p.VelocityY = 0;
        p.VelocityX = 0;
        p.IsFalling = false;
        world.SetNextGrid(x, y, p); // Lock it in place
        return; // Exit completely!
    }


    // 2. GRAVITY & VERTICAL FALL
    if (world.CanMove(x, y + 1))
    {
      p.IsFalling = true;
      if (p.VelocityY < 10) p.VelocityY += 0.4f;

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

    //Density fall like gravity but removes Velocity
    if (world.CanMoveDensity(x, y + 1, p))
    {
      p.IsFalling = true;
      if (p.VelocityY > 1.2) p.VelocityY -= 0.2f;

      world.SwapParticle(x, y, x, y + 1, p);
      return;
    }

    // 3. SLOPES (Diagonals)
    if (world.CanMove(x + dir, y + 1) && (world.CanMoveLiquid(x + dir, y)))
    {
      p.SettleCount = 0; // Diagonals count as movement
      world.MoveParticle(x, y, x + dir, y + 1, p);
      return;
    }
    else if (world.CanMove(x - dir, y + 1) && world.CanMoveLiquid(x - dir, y))
    {
      p.SettleCount = 0;
      world.MoveParticle(x, y, x - dir, y + 1, p);
      return;
    }

    if (TryFlowSide(x, y, dir, flowRange, world, ref p)) return;

    //Random movement
    if (world.CanMove(x + dir, y)) {
      if (world.GetParticleType(x + dir, y - 1) != ParticleType.Water)
      {
        world.MoveParticle(x, y, x + dir, y, p);
        return;
      }
    }

    if (p.SettleCount < 5) p.VelocityX = 1;
    else p.VelocityX = 0;

    // 5. IF NO MOVEMENT POSSIBLE
    p.SettleCount++; // Get closer to sleep
    p.VelocityY = 0;
    p.VelocityX = 0;
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
      if (world.CanMove(checkX, startY + 1))
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
      if (world.CanMove(checkX, startY + 1))

      {
        leftAirScore = i;
        break;
      }
    }

    int finalDir;
    if (rightAirScore < leftAirScore) finalDir = 1;
    else if (leftAirScore < rightAirScore) finalDir = -1;
    else finalDir = 0;
    
    if (finalDir != 0 && world.CanMove(startX + finalDir, startY))
    {
      world.MoveParticle(startX, startY, startX + finalDir, startY, p);
      p.SettleCount = 0;
      return true;
    }

    return false;
  }
}