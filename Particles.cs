using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using System.Security.Cryptography.X509Certificates;

namespace Physics_Sim;
public enum ParticleType { Air, Sand, Stone, Water, Wood, Fire, Smoke }

public struct Particle
{
  public ParticleType Type;
  public bool IsFalling;
  public float VelocityY;
  public float VelocityX;
  public Color Color;
  public byte SettleCount;
  public byte Friction;
  public float Density; //0-1
  public byte FlowRange;
  public byte Depth; //0-15, light -> dark
  public int VisualOffset;
  public int Lifespan;

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
    VisualOffset = 0,
    Lifespan = 0
  };

  public static readonly Color[] SandPalette = new Color[]
  {
    new Color(236, 204, 162), // Classic Sand
    new Color(225, 191, 145), // Slightly darker
    new Color(203, 178, 121), // Earthy tan
    new Color(210, 180, 140), // Tan
    new Color(194, 178, 128)  // Ecru/Dark Sand
  };

  public static readonly Color[] stonePalette = new Color[]
  {
    new Color(144,152,163), // just random greys
    new Color(156,156,156), 
    new Color(152,160,167),
    new Color(140,141,141),
    new Color(142,148,148),
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

  public static readonly Color[] firePalette = new Color[]
  {
    new Color(237,31,10), // Goes red to yellow
    new Color(248,66,11), 
    new Color(249,85,4),
    new Color(247,111,11),
    new Color(255,144,10),
    new Color(255,193,0)
  };

  public static readonly Color[] woodPalette = new Color[]
  {
    new Color(136,99,66),
    new Color(135,103,79), 
    new Color(136,108,80)
  };

  public static readonly Color[] smokePalette = new Color[]
  {
    new Color(55,54,54), //dark to light
    new Color(111,111,111), 
    new Color(150,146,146)
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
            Lifespan = 0,
            Color = SandPalette[_rng.Next(SandPalette.Length)] // Assigns random textured sand grain on spawn
        };

      case ParticleType.Stone:
        return new Particle
        {
            Type = ParticleType.Stone,
            IsFalling = false,
            VelocityY = 0f,
            VelocityX = 0f,
            SettleCount = 0,
            Friction = 10,
            Density = 1f,
            FlowRange = 0,
            Depth = 12,
            VisualOffset = 0,
            Lifespan = 0,
            Color = stonePalette[_rng.Next(stonePalette.Length)]
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
            FlowRange = 30,
            Depth = 0,
            Lifespan = 0,
            VisualOffset = _rng.Next(-1, 2), // Gives a persistent texture offset so it doesn't flicker
            Color = waterPalette[0] // Starts dark, let DepthCalculate handle the coloring dynamically
        };

      case ParticleType.Wood:
        return new Particle
        {
            Type = ParticleType.Wood,
            IsFalling = false,
            VelocityY = 0f,
            VelocityX = 0f,
            SettleCount = 0,
            Friction = 3,
            Density = 1f,
            FlowRange = 0,
            Depth = 0,
            VisualOffset = 0,
            Lifespan = 0,
            Color = woodPalette[_rng.Next(woodPalette.Length)]
        };
      
      case ParticleType.Fire:
        return new Particle
        {
            Type = ParticleType.Fire,
            IsFalling = false,
            VelocityY = 0f,
            VelocityX = 0f,
            SettleCount = 0,
            Friction = 0,
            Density = -0.5f,
            FlowRange = 0,
            Depth = 12,
            VisualOffset = _rng.Next(-1, 2),
            Lifespan = _rng.Next(10, 30), //random start lifespan
            Color = firePalette[0]
        };

      case ParticleType.Smoke:
        return new Particle
        {
            Type = ParticleType.Smoke,
            IsFalling = false,
            VelocityY = 0f,
            VelocityX = 0f,
            SettleCount = 0,
            Friction = 0,
            Density = -1f,
            FlowRange = 0,
            Depth = 0,
            VisualOffset = 0,
            Lifespan = _rng.Next(10, 30), //random start lifespan
            Color = smokePalette[_rng.Next(smokePalette.Length)]
        };
      
      default:
        return Empty; // Fallback security
    }
  } 
}



