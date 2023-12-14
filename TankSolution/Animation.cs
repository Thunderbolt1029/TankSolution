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
    class Animation
    {
        Texture2D Texture;
        int Frames, CurrentFrame;
        float elapsed = 0, delay = 200f;
        bool loop;
        public bool Removed = false;
        Vector2 Origin;

        public Animation(Texture2D Texture, int Frames, bool loop, Vector2 Origin)
        {
            this.Texture = Texture;
            this.Frames = Frames;
            this.loop = loop;
            this.Origin = Origin;
        }

        public void Update(GameTime gameTime)
        {
            elapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (elapsed >= delay)
            {
                if (CurrentFrame > Frames && loop)
                    CurrentFrame = 0;
                else if (CurrentFrame < 0)
                    Removed = true;
                else
                    CurrentFrame++;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, new Rectangle((int)Origin.X, (int)Origin.Y, Texture.Width/Frames, Texture.Height), new Rectangle((Texture.Width / Frames) * CurrentFrame, 0, Texture.Width/Frames, Texture.Height), Color.White);
        }
    }
}
