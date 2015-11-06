using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using WindowsGameLibrary1;

namespace asteroids
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        int displaywidth;
        int displayheight;

        SpriteFont mainfont;
        Boolean gameover = false;

       
        graphics2d background, gameoverimage;

        Random randomiser = new Random();

        const int numberofballs = 100;
        int ballsinplay = 25;
        const float ballsizeratio = 0.01f;
        const float miniballsize = 0.05f;
        const float astspeed = 20f;
        sprites2d[] balls = new sprites2d[numberofballs];

        SoundEffect collidesound;
        SoundEffect soundtrack1;
        SoundEffectInstance music1;
        SoundEffect soundtrack2;
        SoundEffectInstance music2;
        Boolean ybuttonreleased = true, playtrack1 = true;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.graphics.PreferredBackBufferWidth = 1280;
            this.graphics.PreferredBackBufferHeight = 720;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            displaywidth = graphics.GraphicsDevice.Viewport.Width;
            displayheight = graphics.GraphicsDevice.Viewport.Height;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            background = new graphics2d(Content, "fullmoon", displaywidth, displayheight);
            gameoverimage = new graphics2d(Content, "gameover", displaywidth, displayheight);

            for (int i = 0; i < numberofballs; i++)
            {
                float tempsize = (float)(randomiser.Next(100) + 1) * ballsizeratio;
                if (tempsize < miniballsize) tempsize = miniballsize;

                balls[i] = new sprites2d(Content, "ast1", displaywidth / 2, displayheight / 2, tempsize,
                    new Color((byte)randomiser.Next(255), (byte)randomiser.Next(255), (byte)randomiser.Next(255)), true, randomiser);
            }
            collidesound = Content.Load<SoundEffect>("rocks");
            soundtrack1 = Content.Load<SoundEffect>("explosive_attack");
            music1 = soundtrack1.CreateInstance();
            music1.IsLooped = true;
            music1.Volume = 0.5f;
            soundtrack2 = Content.Load<SoundEffect>("iron-man");
            music2 = soundtrack2.CreateInstance();
            music2.IsLooped = true;
            music2.Volume = 0.5f;
            mainfont = Content.Load<SpriteFont>("quartz4");



            reset();
        }

        void reset()
        {
            for(int i=0;i < numberofballs;i++)
            {
                balls[i].velocity = new Vector3(randomiser.Next(9) - 4, randomiser.Next(9) - 4, 0);
                balls[i].velocity /= astspeed;

                do
                {
                    balls[i].position = new Vector3(randomiser.Next(displaywidth - balls[i].rect.Width) + balls[i].rect.Width / 2,
                        randomiser.Next(displayheight - balls[i].rect.Height) + balls[i].rect.Height / 2, 0);
                } while (checkcollision(i));
            }
                   gameover = false;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        Boolean checkcollision(int ballnumber)
        {
            Boolean hit = false;

            for (int counter = 0; counter < numberofballs; counter++)
                if (ballnumber != counter && balls[ballnumber].bsphere.Intersects(balls[counter].bsphere) && balls[ballnumber].visible && balls[counter].visible)
                {
                    hit = true;
                    sfunctions.cresponse(balls[ballnumber].position, balls[counter].position, ref balls[ballnumber].velocity,
                    ref balls[counter].velocity, balls[ballnumber].size, balls[counter].size);

                    balls[ballnumber].position = balls[ballnumber].oldposition;
                    balls[counter].position = balls[counter].oldposition;
                    balls[ballnumber].updateobject();
                    balls[counter].updateobject();

                    if (balls[counter].size < balls[ballnumber].size)
                        balls[counter].visible = false;
                    else
                        balls[ballnumber].visible = false;
                }
            if (!hit)
                balls[ballnumber].oldposition = balls[ballnumber].position;

            return hit;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            GamePadState pad_p1 = GamePad.GetState(PlayerIndex.One);
            GamePadState pad_p2 = GamePad.GetState(PlayerIndex.Two);
            KeyboardState keys = Keyboard.GetState();
            float timebetweenupdates = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (pad_p1.Buttons.Back == ButtonState.Pressed || pad_p2.Buttons.Back == ButtonState.Pressed
                || keys.IsKeyDown(Keys.Escape))
                this.Exit();

            if (!gameover)
            {
                if (ybuttonreleased && pad_p1.Buttons.Y == ButtonState.Pressed)
                    playtrack1 = !playtrack1;
                ybuttonreleased = pad_p1.Buttons.Y == ButtonState.Released;

                if (playtrack1)
                {
                    if (music1.State == SoundState.Stopped) music1.Play();
                    if (music2.State == SoundState.Playing) music2.Pause();
                    if (music1.State == SoundState.Paused) music1.Resume();
                }
                else
                {
                    if (music1.State == SoundState.Playing) music1.Pause();
                    if (music2.State == SoundState.Stopped) music2.Play();
                    if (music2.State == SoundState.Paused) music2.Resume();
                }


                if (pad_p1.Buttons.X == ButtonState.Pressed)
                    gameover = true;

                for (int i = 0; i < ballsinplay; i++)
                {
                    balls[i].automove(displaywidth, displayheight, timebetweenupdates);
                    if (checkcollision(i))
                        collidesound.Play();
                }
            }
            else
            {
                if (music1.State == SoundState.Playing)
                    music1.Stop();
                if (music2.State == SoundState.Playing)
                    music2.Stop();

                if (pad_p1.Buttons.Start == ButtonState.Pressed || pad_p2.Buttons.Start == ButtonState.Pressed
                    || keys.IsKeyDown(Keys.Enter))
                    reset();
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

            background.drawme(ref spriteBatch);

            if (gameover)
                gameoverimage.drawme(ref spriteBatch);

            spriteBatch.DrawString(mainfont, "Screen Resoloution" + displaywidth.ToString() + "  " + displayheight.ToString(), new Vector2(40, 40), Color.White, MathHelper.ToRadians(0), new Vector2(0, 0), 1.5f, SpriteEffects.None, 0);

            for (int i = 0; i < ballsinplay; i++)
            {
                balls[i].drawme(ref spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
