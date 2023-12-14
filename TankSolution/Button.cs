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
    class Button
    {
        public Rectangle rectangle;
        public Texture2D texture;
        public Action Function;

        MouseState MouseCurrentState, MousePreviousState;

        public Button(Rectangle rectangle, Texture2D texture, Action Function)
        {
            this.rectangle = rectangle;
            this.texture = texture;
            this.Function = Function;
        }

        public void Update(GameTime gameTime)
        {
            MousePreviousState = MouseCurrentState;
            MouseCurrentState = Mouse.GetState();

            if (rectangle.Contains(MouseCurrentState.Position) && MouseCurrentState.LeftButton == ButtonState.Pressed && MousePreviousState.LeftButton == ButtonState.Released)
                Function.Invoke();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rectangle, Color.White);
        }
    }
}
