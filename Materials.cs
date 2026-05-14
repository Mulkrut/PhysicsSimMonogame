using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace Physics_Sim;
public enum ParticleType { Air, Sand, Water }

public struct Particle
{
  public ParticleType Type;
  public bool IsFalling;
  public Color Color; // You can now give every grain a unique shade!

  public static Particle Empty => new Particle { Type = ParticleType.Air, IsFalling = false };
}
