using System;

namespace Physics_Sim;

public delegate bool MovePredicate(int x, int y);

public static class PhysicsMovement
{
    private static readonly Random _rng = new Random();

    public static int RandomDir() => _rng.Next(2) == 0 ? -1 : 1;

    public static int RandomSway(int leftChanceDenominator, int rightTrigger)
    {
        int roll = _rng.Next(leftChanceDenominator);
        if (roll == 0) return -1;
        if (roll == rightTrigger) return 1;
        return 0;
    }

    public static bool TryVertical(int x, int y, int verticalDir, World world, Particle p, MovePredicate canMove, float acceleration, float maxVelocity)
    {
        if (!canMove(x, y + verticalDir)) return false;

        p.IsFalling = true;
        if (p.Velocity.Y < maxVelocity) p.Velocity.Y += acceleration;

        int steps = (int)Math.Max(1, p.Velocity.Y);
        for (int d = steps; d > 0; d--)
        {
            int targetY = y + (d * verticalDir);
            if (!canMove(x, targetY)) continue;

            p.SettleCount = 0;
            world.MoveParticle(x, y, x, targetY, p);
            return true;
        }

        return false;
    }

    public static bool TryVerticalWithSway(int x, int y, int swayDir, int verticalDir, World world, Particle p, MovePredicate canMove, float acceleration, float maxVelocity)
    {
        int targetX = x + swayDir;
        if (!canMove(targetX, y + verticalDir)) return false;

        if (p.Velocity.Y < maxVelocity) p.Velocity.Y += acceleration;

        int steps = (int)Math.Max(1, p.Velocity.Y);
        for (int d = steps; d > 0; d--)
        {
            int targetY = y + (d * verticalDir);
            if (!canMove(targetX, targetY)) continue;

            p.SettleCount = 0;
            world.MoveParticle(x, y, targetX, targetY, p);
            return true;
        }

        return false;
    }

    public static bool TryDensityVertical(int x, int y, int verticalDir, World world, Particle p, float slowdown = 0.2f)
    {
        int targetY = y + verticalDir;
        if (!world.CanMoveDensity(x, targetY, p)) return false;

        p.IsFalling = true;
        if (p.Velocity.Y > 1.2f) p.Velocity.Y -= slowdown;
        p.SettleCount = 0;
        world.SwapParticle(x, y, x, targetY, p);
        return true;
    }

    public static bool TryDensityVerticalWithSway(int x, int y, int swayDir, int verticalDir, World world, Particle p, float velocityChange = 0.2f)
    {
        int targetX = x + swayDir;
        int targetY = y + verticalDir;
        if (!world.CanMoveDensity(targetX, targetY, p)) return false;

        p.SettleCount = 0;
        if (p.Velocity.Y > 1.2f) p.Velocity.Y += velocityChange;
        world.SwapParticle(x, y, targetX, targetY, p);
        return true;
    }

    public static bool TryScatterDiagonal(int x, int y, int verticalDir, World world, Particle p, MovePredicate diagonalMove, MovePredicate sideMove, bool resetSettle = true)
    {
        int dir = RandomDir();
        if (TryScatterDirection(x, y, dir, verticalDir, world, p, diagonalMove, sideMove, resetSettle)) return true;
        return TryScatterDirection(x, y, -dir, verticalDir, world, p, diagonalMove, sideMove, resetSettle);
    }

    private static bool TryScatterDirection(int x, int y, int dir, int verticalDir, World world, Particle p, MovePredicate diagonalMove, MovePredicate sideMove, bool resetSettle)
    {
        int targetX = x + dir;
        int targetY = y + verticalDir;

        if (!diagonalMove(targetX, targetY)) return false;
        if (!sideMove(targetX, y)) return false;

        if (resetSettle) p.SettleCount = 0;
        world.MoveParticle(x, y, targetX, targetY, p);
        return true;
    }

    public static bool TrySlideHorizontal(int x, int y, World world, Particle p, MovePredicate canMove, bool increaseSettleOnSlide = false)
    {
        int dir = RandomDir();
        if (TrySlideDirection(x, y, dir, world, ref p, canMove, increaseSettleOnSlide)) return true;
        return TrySlideDirection(x, y, -dir, world, ref p, canMove, increaseSettleOnSlide);
    }

    private static bool TrySlideDirection(int x, int y, int dir, World world, ref Particle p, MovePredicate canMove, bool increaseSettleOnSlide)
    {
        int targetX = x + dir;
        if (!canMove(targetX, y)) return false;

        if (increaseSettleOnSlide) p.SettleCount++;
        world.MoveParticle(x, y, targetX, y, p);
        return true;
    }

    public static bool TryFlowSide(int startX, int startY, int maxRange, World world, Particle p)
    {
        int leftAirScore = maxRange;
        int rightAirScore = maxRange;

        for (int i = 1; i <= maxRange; i++)
        {
            int checkX = startX + i;
            if (!world.CanMoveLiquid(checkX, startY)) break;
            if (!world.CanMove(checkX, startY + 1)) continue;

            rightAirScore = i;
            break;
        }

        for (int i = 1; i <= maxRange; i++)
        {
            int checkX = startX - i;
            if (!world.CanMoveLiquid(checkX, startY)) break;
            if (!world.CanMove(checkX, startY + 1)) continue;

            leftAirScore = i;
            break;
        }

        int finalDir = 0;
        if (rightAirScore < leftAirScore) finalDir = 1;
        else if (leftAirScore < rightAirScore) finalDir = -1;

        if (finalDir != 0 && world.CanMove(startX + finalDir, startY))
        {
            p.SettleCount = 0;
            world.MoveParticle(startX, startY, startX + finalDir, startY, p);
            return true;
        }

        return false;
    }

    public static void SettleInPlace(int x, int y, World world, Particle p, bool zeroVelocityX = true)
    {
        p.SettleCount++;
        p.Velocity.Y = 0;
        if (zeroVelocityX) p.Velocity.X = 0;
        p.IsFalling = false;
        world.SetNextGrid(x, y, p);
    }

    public static void KeepInPlace(int x, int y, World world, Particle p)
    {
        world.SetNextGrid(x, y, p);
    }
}