using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System;

namespace TankSolution
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    { 
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        List<Animation> Animations = new List<Animation>();
        Texture2D background;
        Dictionary<int, Texture2D> LevelTexture = new Dictionary<int, Texture2D>();
        Texture2D ExplosionAnimationTexture;
        Texture2D DefaultShellTexture, MissileTexture, MultiShotPelletTexture;

        SoundEffect ExplosionSfx, WallbangSfx, GunSfx, TankHitSfx, GunEmptySfx;
        SoundEffect DriveSfx; // To add

        ControlScheme Player1Controls = new ControlScheme(Keys.A, Keys.D, Keys.W, Keys.S, Keys.Space);
        ControlScheme Player2Controls = new ControlScheme(Keys.Left, Keys.Right, Keys.Up, Keys.Down, Keys.RightControl);

        SpriteFont TitleFont, UIFont;

        KeyboardState currentKey;
        KeyboardState previousKey;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 975;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            IsMouseVisible = true;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            background = Content.Load<Texture2D>("background");
            for (int i = 0; i < Globals.Level.Length; i++)
                LevelTexture.Add(i, Content.Load<Texture2D>($"Buildings_Level{Globals.CurrentLevel}"));

            ExplosionAnimationTexture = Content.Load<Texture2D>("ExplosionAnimation");

            DefaultShellTexture = Content.Load<Texture2D>("Shell_DefaultShell");
            MissileTexture = Content.Load<Texture2D>("Shell_Missile");
            MultiShotPelletTexture = Content.Load<Texture2D>("Shell_MultiShotPellet");

            ExplosionSfx = Content.Load<SoundEffect>("SFX_Explosion");
            WallbangSfx = Content.Load<SoundEffect>("SFX_Wallbang");
            GunSfx = Content.Load<SoundEffect>("SFX_Gunshot");
            TankHitSfx = Content.Load<SoundEffect>("SFX_Tankhit");
            // GunEmptySfx = Content.Load<SoundEffect>("SFX_GunEmpty"); <------------------ When you make it obvs

            TitleFont = Content.Load<SpriteFont>("Font_Title");
            UIFont = Content.Load<SpriteFont>("Font_UI");

            Globals.ColorTexture = new Texture2D(GraphicsDevice, 1, 1);
            Globals.ColorTexture.SetData(new[] { Color.White });

            LoadMainMenu();
            LoadLevel();
            LoadPauseMenu();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            previousKey = currentKey;
            currentKey = Keyboard.GetState();

            if (Globals.CurrentGameState.MainState == MainStateType.InMainMenu)
                foreach (Button buttons in Globals.MainMenuButtons)
                    buttons.Update(gameTime);            

            if (Globals.CurrentGameState.SubState == SubStateType.Paused)
            {
                if (currentKey.IsKeyDown(Keys.Escape) && !previousKey.IsKeyDown(Keys.Escape))
                    Globals.CurrentGameState.SubState = null;

                // Check for Pause buttons clicked
                
                // Globals.CurrentGameState = new GameState(MainStateType.InMainMenu); // Remove me
            }
            else if (Globals.CurrentGameState.MainState == MainStateType.InGame)
            {
                if (currentKey.IsKeyDown(Keys.Escape) && !previousKey.IsKeyDown(Keys.Escape))
                    Globals.CurrentGameState.SubState = SubStateType.Paused;

                foreach (Animation animation in Animations)
                    animation.Update(gameTime);

                foreach (Player Tank in Globals.Tanks)
                    Tank.Update(gameTime);

                foreach (Player Tank in Globals.Tanks)
                {
                    if (Tank.PlayerShoot)
                    {
                        switch (Tank.ActivePowerup)
                        {
                            case PickupType.Missile:
                                Globals.ShellList.Add(new Shell(MissileTexture, Tank.Position, Tank.rotation, 1.25f, Tank, 7f, ShellType.Missile));
                                break;

                            case PickupType.MultiShot:

                                for (int i = 0; i < Globals.MultiShotAmountOfScatterProjectiles; i++)
                                    Globals.ShellList.Add(new Shell(MultiShotPelletTexture, Tank.Position, Tank.rotation + (i - (((float)Globals.MultiShotAmountOfScatterProjectiles - 1) / 2)) * MathHelper.TwoPi * Globals.MultiShotSpreadAngle / 360 / (Globals.MultiShotAmountOfScatterProjectiles - 1), 1.25f, Tank, 15f, ShellType.MultiShotPellet));
                                break;

                            default:
                                Globals.ShellList.Add(new Shell(DefaultShellTexture, Tank.Position, Tank.rotation, 1.25f, Tank, 15f, ShellType.DefaultShell));
                                break;
                        }

                        GunSfx.Play();
                    }
                    else if (Tank.PlayerAttemptToShoot) ;
                    // GunEmptySfx.Play(); <------------- When made remove semi-colon above
                }


                foreach (Shell FiredShell in Globals.ShellList)
                    FiredShell.Update(gameTime);

                if (Globals.ResetPickupsLoop <= 0)
                {
                    Globals.ResetPickupsLoop = Globals.ResetPickupsInterval;
                    for (int i = 0; i < Globals.PickupPositions[Globals.CurrentLevel].Length; i++)
                        if (Globals.Pickups[i] == null)
                            Globals.Pickups[i] = generateNewPickup(i);
                }
                else
                    Globals.ResetPickupsLoop -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                foreach (Pickup pickup in Globals.Pickups)
                    if (pickup != null)
                        pickup.Update(gameTime);

                foreach (Shell FiredShell in Globals.ShellList)
                {
                    if (FiredShell.IsRemoved)
                        continue;

                    foreach (Player Tank in Globals.Tanks)
                    {
                        if (FiredShell.shellType == ShellType.DefaultShell || FiredShell.shellType == ShellType.MultiShotPellet)
                        {
                            if (FiredShell.CollisionBox.Intersects(Tank.CollisionBox) && FiredShell.Shooter != Tank)
                            {
                                FiredShell.IsRemoved = true;
                                Tank.health -= FiredShell.shellType == ShellType.DefaultShell ? 25 : 10;

                                if (Tank.health == 0)
                                {
                                    Animation ExplosionAnimation = new Animation(ExplosionAnimationTexture, 17, false, new Vector2(Tank.Position.X - 100, Tank.Position.Y - 150));
                                    Animations.Add(ExplosionAnimation);

                                    ExplosionSfx.Play();

                                    FiredShell.Shooter.Wins++;
                                }
                                else
                                    TankHitSfx.Play();

                                break;
                            }
                            else if (Globals.isCollidingWithBounds(FiredShell.CollisionBox, out _))
                                WallbangSfx.Play();
                        }
                        else if (FiredShell.shellType == ShellType.Missile)
                        {
                            // Fancy stuff for animation and splash damage detection
                        }
                    }
                }
                for (int i = 0; i < Globals.ShellList.Count; i++)
                {
                    if (Globals.ShellList[i].IsRemoved)
                    {
                        Globals.ShellList.RemoveAt(i);
                        i--;
                    }
                }
                for (int i = 0; i < Animations.Count; i++)
                {
                    if (Animations[i].Removed)
                    {
                        Animations.RemoveAt(i);
                        i--;
                    }
                }
                for (int i = 0; i < Globals.Pickups.Length; i++)
                {
                    if (Globals.Pickups[i] != null && Globals.Pickups[i].Collected)
                    {
                        Globals.Pickups[i] = null;
                        i--;
                    }
                }

                foreach (Player Tank in Globals.Tanks)
                {
                    if (Tank.health <= 0)
                    {
                        resetStage();
                    }
                }
            }

            base.Update(gameTime);           
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            if (Globals.CurrentGameState.MainState == MainStateType.InMainMenu)
            {
                spriteBatch.Draw(Globals.ColorTexture, new Rectangle(0, 0, 975, 720), Color.LightGray);

                foreach (Button button in Globals.MainMenuButtons)
                    button.Draw(spriteBatch);

                spriteBatch.DrawString(TitleFont, "Tank Game !", new Vector2(250, 100), Color.Black);
            }
                  
            if (Globals.CurrentGameState.MainState == MainStateType.InGame)
            {
                spriteBatch.Draw(background, new Rectangle(0, 0, 975, 720), Color.White); // background
                spriteBatch.Draw(LevelTexture[Globals.CurrentLevel], new Rectangle(0, 0, 975, 720), Color.White);

                foreach (Pickup pickup in Globals.Pickups)
                    if (pickup != null)
                        pickup.Draw(spriteBatch);

                foreach (Shell FiredShell in Globals.ShellList)
                    FiredShell.Draw(spriteBatch);

                foreach (Player Tank in Globals.Tanks)
                    Tank.Draw(spriteBatch);

                if (Globals.showCollisionBoxes)
                {
                    foreach (Rectangle Boundary in Globals.Level[Globals.CurrentLevel])
                        spriteBatch.Draw(Globals.ColorTexture, Boundary, Color.Red);
                    foreach (Pickup pickup in Globals.Pickups)
                        if (pickup != null)
                            spriteBatch.Draw(Globals.ColorTexture, pickup.CollisionBox, Color.Goldenrod);
                    foreach (Shell FiredShell in Globals.ShellList)
                        spriteBatch.Draw(Globals.ColorTexture, FiredShell.CollisionBox, Color.Lime);
                    foreach (Player Tank in Globals.Tanks)
                        spriteBatch.Draw(Globals.ColorTexture, Tank.CollisionBox, Color.Blue);
                }


                foreach (Animation animation in Animations)
                    animation.Draw(spriteBatch);

                spriteBatch.DrawString(UIFont, $"Player 1 Health - {Globals.Tanks[0].health}\nPlayer 1 Wins - {Globals.Tanks[0].Wins}", new Vector2(50, 50), Color.Black);
                spriteBatch.DrawString(UIFont, $"Player 2 Health - {Globals.Tanks[1].health}\nPlayer 2 Wins - {Globals.Tanks[1].Wins}", new Vector2(700, 600), Color.Black);
            }

            if (Globals.CurrentGameState.SubState == SubStateType.Paused)
            {
                spriteBatch.Draw(Globals.ColorTexture, new Rectangle(0, 0, 975, 720), new Color(255, 255, 255, 100)); // Fixxxxxxxxxxxxxxxxxxxxxxxxx
            }

            if (Globals.DrawGuideLines)
            {
                Texture2D ColorTexture = new Texture2D(GraphicsDevice, 1, 1);
                ColorTexture.SetData(new[] { Color.White });
                spriteBatch.Draw(ColorTexture, new Rectangle(975 / 2, 0, 1, 720), Color.Red);
                spriteBatch.Draw(ColorTexture, new Rectangle(0, 720 / 2, 975, 1), Color.Red);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        void resetStage()
        {
            Globals.Tanks[0].Position = new Vector2(100, 100);
            Globals.Tanks[0].rotation = 0f;
            Globals.Tanks[0].health = 100;

            Globals.Tanks[1].Position = new Vector2(836, 631);
            Globals.Tanks[1].rotation = MathHelper.Pi;
            Globals.Tanks[1].health = 100;

            foreach (Player Tank in Globals.Tanks)
                Tank.ActivePowerup = PickupType.NoActivePowerup;

            for (int i = 0; i < Globals.Pickups.Length; i++)
                Globals.Pickups[i] = null;
            Globals.ResetPickupsLoop = 0f;

            Globals.ShellList.Clear();
        }
        
        Pickup generateNewPickup(int index = 0)
        {
            PickupType pickupType;
            Vector2 Position;
            Texture2D texture;

            pickupType = (PickupType)Globals.Random.Next(0, (int)PickupType.Count);

            Position = Globals.PickupPositions[Globals.CurrentLevel][index];

            texture = Content.Load<Texture2D>($"Pickup_{pickupType}");

            return new Pickup(pickupType, Position, texture);
        }

        void LoadMainMenu()
        {
            Texture2D PlayButtonTexture, ExitButtonTexture;

            PlayButtonTexture = Content.Load<Texture2D>("Button_Play");
            ExitButtonTexture = Content.Load<Texture2D>("Button_Exit");

            Globals.MainMenuButtons.Add(new Button(new Rectangle(380, 250, PlayButtonTexture.Width, PlayButtonTexture.Height), PlayButtonTexture, () => { Globals.CurrentGameState = new GameState(MainStateType.InGame); }));
            Globals.MainMenuButtons.Add(new Button(new Rectangle(380, 375, ExitButtonTexture.Width, ExitButtonTexture.Height), ExitButtonTexture, Exit));
        }

        void LoadLevel()
        {
            Globals.Tanks[0] = new Player(Content.Load<Texture2D>("Tank1"), new Vector2(100, 100), 0f, 100, Player1Controls);
            Globals.Tanks[1] = new Player(Content.Load<Texture2D>("Tank2"), new Vector2(836, 631), MathHelper.Pi, 100, Player2Controls);
        }

        void LoadPauseMenu()
        {

        }
    }
}
