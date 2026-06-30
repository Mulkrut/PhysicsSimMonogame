using Microsoft.Xna.Framework;
using System;

namespace Physics_Sim;

public abstract class Particle
{
  public bool IsFalling;
  public Vector2 Velocity;
  public Color Color;
  public byte SettleCount;
  public byte Friction;
  public float Density; //0-1
  public byte FlowRange;
  public byte Depth; //0-15, light -> dark
  public int VisualOffset;
  public int Lifespan;

  private static readonly Random _rng = new Random();

  // Abstract methods need to be implimented by whichever class inherits from it
  public abstract void UpdateVisuals(int x, int y, World world);

  public abstract void Update(int x, int y, World world);

  // Virtual palette — subclasses override with their own
  public virtual Color[] Palette => Array.Empty<Color>();

  // Shared helper so subclasses can pick a random palette color
  protected Color RandomPaletteColor()
  {
      var p = Palette;
      return p.Length > 0 ? p[_rng.Next(p.Length)] : Color.Transparent;
  }

  public abstract Particle Clone();
}