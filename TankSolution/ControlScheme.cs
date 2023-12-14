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
    struct ControlScheme
    {
        public Keys Left, Right, Forward, Reverse, Shoot;

        public ControlScheme(Keys Left, Keys Right, Keys Forward, Keys Reverse, Keys Shoot)
        {
            this.Left = Left;
            this.Right = Right;
            this.Forward = Forward;
            this.Reverse = Reverse;
            this.Shoot = Shoot;
        }
    }
}
