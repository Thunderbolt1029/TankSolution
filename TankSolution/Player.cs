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
    class Player
    {
        public KeyboardState currentKey;
        public KeyboardState previousKey;
        public float rotation;
        public float StandardRotationVelocity = 1.0f;
        public float RotationVelocity = 1.0f;
        public float StandardLinearVelocity = 1.5f;
        public float Linearvelocity = 1.5f;
        public Vector2 Direction;
        public Vector2 Position;
        public Vector2 Origin;

        public bool PlayerShoot = false;
        public bool PlayerAttemptToShoot = false;
        public const float standardReloadTime = 0.6f;
        public float reloadTime = 0.6f;
        public float reloadingTime = 0f;

        public PickupType ActivePowerup = PickupType.NoActivePowerup;
        public float PowerupRemaining = 0f; // Could be seconds remaining / shots left
        public bool pickedUpOnThisFrame;

        public Texture2D texture;
        public int health;

        public ControlScheme Controls;
        public Rectangle CollisionBox;

        public int Wins = 0;

        float scale = 1f;
        public int averageSize;

        public Player(Texture2D newTexture, Vector2 spawnPosition, float spawnRotation, int ObjHealth, ControlScheme controls)
        {
            texture = newTexture;
            averageSize = (int)Math.Round((texture.Width * scale + texture.Height * scale) / 2);
            Origin = new Vector2(texture.Width / 2, texture.Height / 2);

            health = ObjHealth;

            Position = spawnPosition;
            rotation = spawnRotation;

            Controls = controls;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color.White, rotation, Origin, scale, SpriteEffects.None, 0);          
        }

        public void Update(GameTime gameTime)
        {
            switch (ActivePowerup)
            {
                case PickupType.HealthBox:
                    health += 25;
                    ActivePowerup = PickupType.NoActivePowerup;
                    break;

                case PickupType.SpeedBoost:
                    PowerupRemaining -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (pickedUpOnThisFrame)
                    {
                        StandardLinearVelocity = 3f;
                        RotationVelocity = 1.5f;
                        PowerupRemaining = 5f;
                    }
                    else if (PowerupRemaining <= 0)
                    {
                        StandardLinearVelocity = 1.5f;
                        RotationVelocity = 1f;
                        ActivePowerup = PickupType.NoActivePowerup;
                    }                        

                    break;

                case PickupType.InstaReload:
                    PowerupRemaining -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (pickedUpOnThisFrame)
                    {
                        reloadTime = 0.1f;
                        PowerupRemaining = 3f;
                    }
                    else if (PowerupRemaining <= 0)
                    {
                        reloadTime = standardReloadTime;
                        ActivePowerup = PickupType.NoActivePowerup;
                    }

                    break;

                case PickupType.MultiShot:
                    if (pickedUpOnThisFrame)
                        PowerupRemaining = 5;
                    else if (PowerupRemaining == 0)
                        ActivePowerup = PickupType.NoActivePowerup;

                    break;

                case PickupType.Missile:
                    if (pickedUpOnThisFrame)
                        PowerupRemaining = 3;
                    else if (PowerupRemaining == 0)
                        ActivePowerup = PickupType.NoActivePowerup;

                    break;

                case PickupType.NoActivePowerup:
                    StandardLinearVelocity = 1.5f;
                    RotationVelocity = 1f;
                    reloadTime = standardReloadTime;
                    PowerupRemaining = 0;
                    break;
            }
            pickedUpOnThisFrame = false;

            if (reloadingTime > 0)
                reloadingTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            else
                reloadingTime = 0;

            previousKey = currentKey;
            currentKey = Keyboard.GetState();

            if (Keyboard.GetState().IsKeyDown(Controls.Left))
                rotation -= MathHelper.ToRadians(RotationVelocity);
            else if (Keyboard.GetState().IsKeyDown(Controls.Right))
                rotation += MathHelper.ToRadians(RotationVelocity);

            Direction = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));

            if (Globals.areTanksColliding())
                Linearvelocity = 0.45f * StandardLinearVelocity;
            else
                Linearvelocity = StandardLinearVelocity;
          
            if (Keyboard.GetState().IsKeyDown(Controls.Forward))
                Position += Direction * Linearvelocity;
            if (Keyboard.GetState().IsKeyDown(Controls.Reverse))
                Position -= Direction * Linearvelocity * 0.5f;

            if (currentKey.IsKeyDown(Controls.Shoot) && previousKey.IsKeyUp(Controls.Shoot) && reloadingTime == 0)
                Shoot();
            else if (currentKey.IsKeyDown(Controls.Shoot) && previousKey.IsKeyUp(Controls.Shoot))
            {
                PlayerAttemptToShoot = true;
                PlayerShoot = false;
            }
            else
            {
                PlayerAttemptToShoot = false;
                PlayerShoot = false;
            }
            
            CollisionBox = new Rectangle((int)Math.Round(Position.X - averageSize/2), (int)Math.Round(Position.Y - averageSize / 2), averageSize, averageSize); // Collisions are crap cause rectangles are axis aligned => no rotation

            if (CollisionBox.X < 0)
                Position.X = 0 + averageSize/2;
            else if (CollisionBox.X+averageSize-1 > 975)
                Position.X = 975 - averageSize/2;
            if (CollisionBox.Y < 0)
                Position.Y = 0 + averageSize/2;
            else if (CollisionBox.Y+averageSize-1 > 720)
                Position.Y = 720 - averageSize/2;


            if (Globals.isCollidingWithBounds(CollisionBox, out List<Rectangle> OverlappingBoundaries))
            {
                foreach (Rectangle OverlappingBoundary in OverlappingBoundaries)
                {
                    int xDisplacement, yDisplacement;

                    int RightOverlap = OverlappingBoundary.Left - CollisionBox.Right;
                    int LeftOverlap = OverlappingBoundary.Right - CollisionBox.Left;
                    if (Math.Abs(RightOverlap) < Math.Abs(LeftOverlap))
                        xDisplacement = RightOverlap;
                    else
                        xDisplacement = LeftOverlap;

                    int TopOverlap = OverlappingBoundary.Bottom - CollisionBox.Top;
                    int BottomOverlap = OverlappingBoundary.Top - CollisionBox.Bottom;
                    if (Math.Abs(TopOverlap) < Math.Abs(BottomOverlap))
                        yDisplacement = TopOverlap;
                    else
                        yDisplacement = BottomOverlap;

                    if (Math.Abs(xDisplacement) < Math.Abs(yDisplacement))
                        Position.X += xDisplacement;
                    else
                        Position.Y += yDisplacement;
                }
            }
            CollisionBox = new Rectangle((int)Math.Round(Position.X - averageSize / 2), (int)Math.Round(Position.Y - averageSize / 2), averageSize, averageSize);
        }

        void Shoot()
        {
            PlayerShoot = true;
            reloadingTime = reloadTime;

            if (ActivePowerup == PickupType.MultiShot || ActivePowerup == PickupType.Missile)
                PowerupRemaining--;
        }
    }
}
