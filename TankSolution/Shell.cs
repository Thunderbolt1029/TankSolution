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
    enum ShellType
    {
        DefaultShell,
        MultiShotPellet,
        Missile
    }

    class Shell
    {
        private float timer;
        public float LifeSpan;
        public bool IsRemoved = false;
        public Vector2 Direction;
        public Vector2 Position;
        public Vector2 Origin;
        public float Rotation;
        public float LinearVelocity;
        public ShellType shellType;

        public Texture2D texture;

        public Rectangle CollisionBox;
        public Player Shooter;

        float scale = 0.3f;

        public Shell(Texture2D newTexture, Vector2 ShellOrgin, float ShellRotation, float ShellLifeSpan, Player Shooter, float LinearVelocity, ShellType shellType)
        {
            texture = newTexture;

            Origin = new Vector2(texture.Width / 2, texture.Height / 2);
            Position = ShellOrgin;
            Direction = new Vector2((float)Math.Cos(ShellRotation), (float)Math.Sin(ShellRotation));
            Rotation = ShellRotation;
            LifeSpan = ShellLifeSpan;

            this.LinearVelocity = LinearVelocity;
            this.shellType = shellType;

            this.Shooter = Shooter;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color.White, Rotation, Origin, scale, SpriteEffects.None, 0);
        }

        public void Update(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timer >= LifeSpan || Globals.isCollidingWithBounds(CollisionBox, out _))
                IsRemoved = true;

            Position += Direction * LinearVelocity;

            int averageSize = (int)Math.Round((texture.Width * scale + texture.Height * scale) / 2);
            CollisionBox = new Rectangle((int)Math.Round(Position.X - averageSize / 2), (int)Math.Round(Position.Y - averageSize / 2), averageSize, averageSize); // Collisions are crap cause rectangles are axis aligned => no rotation
        }
    }
}
