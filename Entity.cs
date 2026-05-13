// using Microsoft.Xna.Framework;
// using Microsoft.Xna.Framework.Graphics;
// using System.Collections.Generic;

// namespace Physics_Game;

// //Yoinked fra Silvija
// public class Entity
// {
//     // Core state — every entity has these
//     public Vector2 Position;
//     public Vector2 Velocity = Vector2.Zero;
//     public Vector2 Size = new Vector2(1f, 1f);
//     public Vector2 Scale = new Vector2(1f, 1f);
//     public bool Active = true;

//     // Components
//     private List<Component> _components = new List<Component>();

//     public Entity(Vector2 position)
//     {
//         Position = position;
//     }

//     // Add a component and return it (fluent style)
//     public T AddComponent<T>(T component) where T : Component
//     {
//         component.Entity = this;
//         _components.Add(component);
//         return component;
//     }

//     // Get a component by type
//     public T GetComponent<T>() where T : Component
//     {
//         foreach (var c in _components)
//             if (c is T match) return match;
//         return null;
//     }

//     public void Update(double deltaTime)
//     {
//         if (!Active) return;
//         foreach (var c in _components)
//             if (c.Enabled) c.Update(deltaTime);
//     }

//     public void Draw(SpriteBatch spriteBatch)
//     {
//         if (!Active) return;
//         foreach (var c in _components)
//             if (c.Enabled) c.Draw(spriteBatch);
//     }
// }
