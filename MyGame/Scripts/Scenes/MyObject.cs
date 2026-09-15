using Microsoft.Xna.Framework;

using FreedomEngine.Components;
using FreedomEngine.Components.Collisions;
using FreedomEngine.Graphics;

namespace MyGame.Scripts.Scenes
{
    public class MyObject : GameObject
    {
        public MyObject(Sprite sprite, Vector2 position, CollisionMask collisionMask = null) : base(sprite, position, collisionMask)
        {
        }
    }
}