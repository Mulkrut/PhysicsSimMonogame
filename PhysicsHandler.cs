using System;

namespace Physics_Sim;

public static class PhysicsHandler
{
    private static readonly Random _rng = new Random();

    public static void UpdateParticle(int x, int y, World world)
    {
        Particle p = world.GetParticle(x, y);

        switch (p.Type)
        {
            case ParticleType.Sand:
                HandleSandMovement(x, y, world, ref p);
                break;
            case ParticleType.Water:
                HandleWaterMovement(x, y, world, ref p);
                break;
            case ParticleType.Fire:
                HandleFireMovement(x, y, world, ref p);
                break;
            case ParticleType.Smoke:
                HandleSmokeMovement(x, y, world, ref p);
                break;
            default:
                world.SetNextGrid(x, y, p);
                break;
        }
    }

    private static void HandleSandMovement(int x, int y, World world, ref Particle p)
    {
        if (p.SettleCount >= 5 && !world.CanMoveDensity(x, y + 1, p))
        {
            PhysicsMovement.KeepInPlace(x, y, world, p);
            return;
        }

        if (PhysicsMovement.TryVertical(x, y, 1, world, ref p, world.CanMove, 0.2f, 10f)) return;
        if (PhysicsMovement.TryDensityVertical(x, y, 1, world, ref p)) return;
        if (PhysicsMovement.TryScatterDiagonal(x, y, 1, world, ref p, world.CanMove, world.CanMove)) return;
        if (PhysicsMovement.TrySlideHorizontal(x, y, world, ref p, world.CanMove, increaseSettleOnSlide: true)) return;

        if (p.SettleCount < 3) p.VelocityX = 1;
        else p.VelocityX = 0;

        PhysicsMovement.SettleInPlace(x, y, world, ref p, zeroVelocityX: false);
    }

    private static void HandleWaterMovement(int x, int y, World world, ref Particle p)
    {
        int flowRange = (world.frameCount + x + y) % 4 == 0 ? 40 : 20;

        if (p.VelocityY == 0 && p.VelocityX == 0 && !p.IsFalling &&
            !world.CanMove(x, y + 1) &&
            !world.CanMove(x - 1, y) &&
            !world.CanMove(x + 1, y) &&
            !world.CanMove(x + 1, y + 1) &&
            !world.CanMove(x - 1, y + 1))
        {
            p.SettleCount = 50;
            p.VelocityY = 0;
            p.VelocityX = 0;
            p.IsFalling = false;
            world.SetNextGrid(x, y, p);
            return;
        }

        if (PhysicsMovement.TryVertical(x, y, 1, world, ref p, world.CanMove, 0.4f, 10f)) return;
        if (PhysicsMovement.TryDensityVertical(x, y, 1, world, ref p)) return;
        if (PhysicsMovement.TryScatterDiagonal(x, y, 1, world, ref p, world.CanMove, world.CanMoveLiquid)) return;
        if (PhysicsMovement.TryFlowSide(x, y, flowRange, world, ref p)) return;

        int dir = PhysicsMovement.RandomDir();
        if (world.CanMove(x + dir, y) && world.GetParticleType(x + dir, y - 1) != ParticleType.Water)
        {
            world.MoveParticle(x, y, x + dir, y, p);
            return;
        }

        if (p.SettleCount < 5) p.VelocityX = 1;
        else p.VelocityX = 0;

        PhysicsMovement.SettleInPlace(x, y, world, ref p);
    }

    private static void HandleSmokeMovement(int x, int y, World world, ref Particle p)
    {
        if (_rng.Next(0, 10) < 3) p.Lifespan--;

        if (p.Lifespan <= 0)
        {
            world.DeleteParticle(x, y);
            return;
        }

        int swayDir = PhysicsMovement.RandomSway(5, 4);

        if (PhysicsMovement.TryVerticalWithSway(x, y, swayDir, -1, world, ref p, world.CanMoveLiquid, 0.1f, 2f)) return;
        if (PhysicsMovement.TryDensityVerticalWithSway(x, y, swayDir, -1, world, ref p)) return;
        if (PhysicsMovement.TryScatterDiagonal(x, y, -1, world, ref p, world.CanMove, world.CanMove)) return;
        if (PhysicsMovement.TrySlideHorizontal(x, y, world, ref p, world.CanMove, increaseSettleOnSlide: true)) return;

        world.SetNextGrid(x, y, p);
    }

    private static void HandleFireMovement(int x, int y, World world, ref Particle p)
    {
        if (p.Lifespan <= 0)
        {
            world.DeleteParticle(x, y);
            if (_rng.Next(2) == 0 && world.CanMove(x, y - 1))
            {
                world.SetNewNextGrid(x, y - 1, ParticleType.Smoke);
            }
            return;
        }

        if (_rng.Next(4) == 0) p.Lifespan--;

        ParticleType right = world.GetParticleType(x + 1, y);
        ParticleType left = world.GetParticleType(x - 1, y);
        ParticleType up = world.GetParticleType(x, y - 1);
        ParticleType down = world.GetParticleType(x, y + 1);

        if (right == ParticleType.Water || left == ParticleType.Water || up == ParticleType.Water || down == ParticleType.Water)
        {
            world.DeleteParticle(x, y);
            world.SetNewNextGrid(x, y, ParticleType.Smoke);
            return;
        }

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

        int swayDir = PhysicsMovement.RandomSway(3, 2);

        if (p.SettleCount > 5)
        {
            if (PhysicsMovement.TryVerticalWithSway(x, y, swayDir, -1, world, ref p, world.CanMove, 0.1f, 3f)) return;
            if (PhysicsMovement.TryDensityVerticalWithSway(x, y, swayDir, -1, world, ref p)) return;
            if (PhysicsMovement.TryScatterDiagonal(x, y, -1, world, ref p, world.CanMove, world.CanMove, resetSettle: false)) return;
        }
        else if (_rng.Next(3) == 2)
        {
            p.SettleCount++;
        }

        if (PhysicsMovement.TrySlideHorizontal(x, y, world, ref p, world.CanMove)) return;

        world.SetNextGrid(x, y, p);
    }
}