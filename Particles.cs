using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using System.Security.Cryptography.X509Certificates;

namespace Physics_Sim;
public enum ParticleType { Air, Sand, Water }

public struct Particle
{
  public ParticleType Type;
  public bool IsFalling;
  public float VelocityY;
  public Color Color;
  public int SettleCount;
  public int Friction;
  public float Density; //0-1

  public static Particle Empty => new Particle
  {
    Type = ParticleType.Air,
    IsFalling = false,
    VelocityY = 0,
    Color = Color.Transparent,
    SettleCount = 0,
    Friction = 0,
    Density = 0,
  };

  public static readonly Color[] SandPalette = new Color[]
  {
    new Color(236, 204, 162), // Classic Sand
    new Color(225, 191, 145), // Slightly darker
    new Color(203, 178, 121), // Earthy tan
    new Color(210, 180, 140), // Tan
    new Color(194, 178, 128)  // Ecru/Dark Sand
  };

  public static readonly Color[] waterPalette = new Color[]
{
    new Color(15,94,156), // Goes dark to light
    new Color(35,137,218), 
    new Color(28,163,236),
    new Color(90,188,216),
    new Color(116,204,244)
};
}



