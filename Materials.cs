// using Microsoft.Xna.Framework;
// using Microsoft.Xna.Framework.Graphics;
// using System.Collections.Generic;
// using System;

// namespace Physics_Sim;


//TODO include into the game1

// public enum MovementType
// {
//   Static,   // Stone, Wall
//   Granular, // Sand, Gravel
//   Liquid,   // Water, Oil
//   Gas       // Smoke, Steam
// }
// public struct Material
// {
//   public string Name;
//   public MovementType Movement;
//   public Color Color;
//   public float Density;  // Higher numbers sink (Sand = 2.0, Water = 1.0)
//   public float Buoyancy; // For gases (negative density effect)

//   // How likely it is to "slide" horizontally (0.0 to 1.0)
//   public float Friction;
// }  

// public static class MaterialLibrary
// {
//   // A simple array where the Index matches the MaterialId
//   public static readonly Material[] Materials;

//   static MaterialLibrary()
//   {
//     Materials = new Material[5]; // Adjust size as you add more

//     // ID 0: Air
//     Materials[0] = new Material
//     {
//       Name = "Air",
//       Movement = MovementType.Static,
//       Density = 0f,
//       Color = Microsoft.Xna.Framework.Color.Transparent
//     };

//     // ID 1: Sand
//     Materials[1] = new Material
//     {
//       Name = "Sand",
//       Movement = MovementType.Granular,
//       Density = 2.0f, // Heavy
//       Color = Microsoft.Xna.Framework.Color.SandyBrown
//     };

//     // ID 2: Water
//     Materials[2] = new Material
//     {
//       Name = "Water",
//       Movement = MovementType.Liquid,
//       Density = 1.0f, // Less dense than sand
//       Color = Microsoft.Xna.Framework.Color.Blue * 0.5f
//     };

//     // ID 3: Stone
//     Materials[3] = new Material
//     {
//       Name = "Stone",
//       Movement = MovementType.Static,
//       Density = 10.0f,
//       Color = Microsoft.Xna.Framework.Color.Gray
//     };

//     // ID 4: Smoke
//     Materials[4] = new Material
//     {
//       Name = "Steam",
//       Movement = MovementType.Gas,
//       Density = -1.0f, // Negative density makes it rise!
//       Color = Microsoft.Xna.Framework.Color.LightGray * 0.3f
//     };
//   }
// }