namespace Physics_Sim;

public static class Gravity
{
  public static void Apply(ref Particle p)
  {
    if (p.IsFalling)
    {
      // Use small increments since this runs every frame
      p.VelocityY += 0.1f;
    }
    else
    {
      p.VelocityY = 0;
    }
  }
}