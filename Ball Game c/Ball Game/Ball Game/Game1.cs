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

namespace Ball_Game
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
        sprites2d football;

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

            football = new sprites2d(Content, "football2", displaywidth / 2, displayheight / 2, 0.2f, Color.Tomato, true);

            mainfont = Content.Load<SpriteFont>("quartz4");

            reset();
        }

        void reset()
        {
            football.position = new Vector3(displaywidth / 2, displayheight / 2, 0);
            football.velocity = new Vector3(2, 3, 0);
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

            if (pad_p1.Buttons.Back == ButtonState.Pressed || pad_p2.Buttons.Back == ButtonState.Pressed
                || keys.IsKeyDown(Keys.Escape))
                this.Exit();

            if (!gameover)
            {
                football.rotation += 0.05f;
                football.position += football.velocity;

                if ((football.position.X + football.rect.Width / 2) >= displaywidth)
                    football.velocity.X = -football.velocity.X;
                if ((football.position.X - football.rect.Width / 2) <= 0)
                    football.velocity.X = -football.velocity.X;
                if ((football.position.Y + football.rect.Height / 2) >= displayheight)
                    football.velocity.Y = -football.velocity.Y;
                if ((football.position.Y - football.rect.Height / 2) <= 0)
                    football.velocity.Y = -football.velocity.Y;

                football.rect.X = (int)football.position.X;
                football.rect.Y = (int)football.position.Y;
                football.bsphere = new BoundingSphere(football.position, football.rect.Width / 2);

                if (pad_p1.Buttons.X == ButtonState.Pressed)
                    gameover = true;
            }
            else
            {
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

            football.drawme(ref spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
