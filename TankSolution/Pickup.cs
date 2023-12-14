using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankSolution
{
    enum PickupType
    {
        HealthBox, // 25 more health  -  instant use
        SpeedBoost, // 3 instead of 1.5 speed  -  3 seconds of use
        InstaReload, // not technically instant but pretty fast  -  5 seconds of use
        MultiShot, // shotgun style spread  -  5 shots
        Missile, // slow moving high damage splash projectile slower reload  -  3 shots

        // To make other bits of code easier
        Count,
        NoActivePowerup
    }

    class Pickup
    {
        public Vector2 Position;
        public Player PlayerCollected;
        public bool Collected = false;
        public PickupType Type;

        public Texture2D texture;
        public Rectangle CollisionBox;

        public Pickup(PickupType Type, Vector2 Position, Texture2D texture)
        {
            this.Type = Type;
            this.Position = Position;
            this.texture = texture;

            CollisionBox = new Rectangle(Position.ToPoint(), new Point(75, 75));
        }

        public void Update(GameTime gameTime)
        {
            foreach (Player Tank in Globals.Tanks)
                if (Tank.CollisionBox.Intersects(CollisionBox))
                {
                    PlayerCollected = Tank;
                    Collected = true;

                    Tank.ActivePowerup = Type;
                    Tank.pickedUpOnThisFrame = true;
                }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, new Rectangle(Position.ToPoint(), new Point(75, 75)), Color.White);
        }
        
    }
}
