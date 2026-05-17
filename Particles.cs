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
  public float VelocityX;
  public Color Color;
  public int SettleCount;
  public int Friction;
  public float Density; //0-1
  public int FlowRange;
  public int Depth; //0-15, light -> dark
  public int VisualOffset;

  private static readonly Random _rng = new Random();

  public static Particle Empty => new Particle
  {
    Type = ParticleType.Air,
    IsFalling = false,
    VelocityY = 0,
    VelocityX = 0,
    Color = Color.Transparent,
    SettleCount = 0,
    Friction = 0,
    Density = 0f,
    FlowRange = 0,
    Depth = 0,
    VisualOffset = 0
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
    new Color(116,204,244),  //lightest blue
    new Color(240,255,255) // basicly white
  };

  public static Particle Create(ParticleType type)
  {
    switch (type)
    {
      case ParticleType.Air:
        return Empty;

      case ParticleType.Sand:
        return new Particle
        {
            Type = ParticleType.Sand,
            IsFalling = true,
            VelocityY = 0f,
            VelocityX = 0f,
            SettleCount = 0,
            Friction = 5, // Default friction value for sand grains
            Density = 0.6f,
            FlowRange = 3,
            Depth = 0,
            VisualOffset = 0,
            Color = SandPalette[_rng.Next(SandPalette.Length)] // Assigns random textured sand grain on spawn
        };

      case ParticleType.Water:
        return new Particle
        {
            Type = ParticleType.Water,
            IsFalling = true,
            VelocityY = 0f,
            VelocityX = 0f,
            SettleCount = 0,
            Friction = 0, // Water has no internal friction properties
            Density = 0.2f,
            FlowRange = 40,
            Depth = 0,
            VisualOffset = _rng.Next(-1, 2), // Gives a persistent texture offset so it doesn't flicker
            Color = waterPalette[0] // Starts dark, let DepthCalculate handle the coloring dynamically
        };
      default:
        return Empty; // Fallback security
    }
}
}



