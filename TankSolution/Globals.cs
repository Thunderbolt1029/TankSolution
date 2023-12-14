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
    class Globals
    {
        public static bool showCollisionBoxes = false;
        public static bool DrawGuideLines = false;
        public static int MultiShotAmountOfScatterProjectiles = 4;
        public static float MultiShotSpreadAngle = 30f; // Degrees

        public static GameState CurrentGameState = new GameState(MainStateType.InMainMenu);
        public static int CurrentLevel = 0; // Zero Indexed
        public static Rectangle[][] Level =
        {
            new Rectangle[] // Level 0
            {
                new Rectangle(443, 34, 123, 182),
                new Rectangle(221, 426, 215, 116),
                new Rectangle(668, 370, 202, 122)
            },
            new Rectangle[] // Level 1
            {
                new Rectangle(389, 7, 205, 113),
                new Rectangle(13, 270, 108, 211),
                new Rectangle(432, 311, 103, 100),
                new Rectangle(842, 224, 104, 220),
                new Rectangle(386, 598, 204, 133)
            },
            new Rectangle[] // Level 2
            {
                new Rectangle(413, 197, 121, 351),
                new Rectangle(221, 315, 506, 120),
            }
        };
        public static Vector2[][] PickupPositions =
        {
            new Vector2[] { new Vector2(710, 105) }, // Level 0
            new Vector2[] { new Vector2(50, 620), new Vector2(820, 60) }, // Level 1
            new Vector2[] { new Vector2(860, 40), new Vector2(40, 620) }, // Level 2
        };


        public static Player[] Tanks = new Player[2];
        public static List<Shell> ShellList = new List<Shell>();
        public static Pickup[] Pickups = new Pickup[2];

        public static List<Button> MainMenuButtons = new List<Button>();
        public static List<Button> InGameButtons = new List<Button>();
        public static List<Button> PauseMenuButtons = new List<Button>();

        public static Random Random = new Random();

        public static float ResetPickupsLoop = 0f;
        public static float ResetPickupsInterval = 10f;

        public static Texture2D ColorTexture;

        public static bool isCollidingWithBounds(Rectangle rectangle, out List<Rectangle> OverlappingBoundaries)
        {
            OverlappingBoundaries = new List<Rectangle>();
            foreach (Rectangle Boundary in Level[CurrentLevel])
                if (rectangle.Intersects(Boundary))
                {
                    OverlappingBoundaries.Add(Boundary);
                }
            return OverlappingBoundaries.Count != 0;
        }

        public static bool areTanksColliding()
        {
            return Tanks[0].CollisionBox.Intersects(Tanks[1].CollisionBox);
        }
    }
}
