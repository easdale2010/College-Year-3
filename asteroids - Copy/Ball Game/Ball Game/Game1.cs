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

        float vibration = 0;

        SpriteFont mainfont;
        Boolean gameover = false;

        animation[] explosion = new animation[numberofballs];
        animation[] shiphit = new animation[numberofships];

        int asteroidsdestroyed = 0;
        ammo[] bullet = new ammo[numberofships];

        const int numberofships = 2;
        ships[] spaceship = new ships[numberofships];

        const float lengthofspawn = 2000;
        graphics2d background, gameoverimage;

        Random randomiser = new Random();

        const int numberofballs = 100;
        int ballsinplay = 3;
        const float ballsizeratio = 0.01f;
        const float miniballsize = 0.05f;
        const float astspeed = 20f;
        sprites2d[] balls = new sprites2d[numberofballs];

        const int numberofbullets = 5;
        ammo[,] bullets5 = new ammo[numberofships,numberofbullets];

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

            for (int i = 0; i < numberofballs; i++)
            {
                explosion[i] = new animation(Content, "effect_005", 0, 0, 1f, Color.White, false, 24, 4, 5);
            }

            shiphit[0] = new animation(Content, "effect_003", 0, 0, 1, Color.White, false, 24, 4, 5);
            shiphit[1] = new animation(Content, "effect_002", 0, 0, 1, Color.White, false, 24, 5, 5);

            bullet[0] = new ammo(Content, "bullet1", 0, 0, .1f, Color.White, false);
            bullet[1] = new ammo(Content, "bullet2", 0, 0, .1f, Color.White, false);

            for(int b=0;b<numberofbullets;b++)
                for(int s=0;s<numberofships;s++)

            bullets5[s,b] = new ammo(Content, "bullet2", 0, 0, .1f, Color.White, false); 

            reset();
        }

        
        void reset()
        {
            ballsinplay=0;

            spaceship[0] = new ships(Content, "defiant", 50, 50, 0.2f, Color.White, true);
            spaceship[1] = new ships(Content, "raptor", displaywidth - 50, displayheight - 50, 0.2f, Color.White, true);
            spaceship[1].rotation = MathHelper.ToRadians(180);
            for (int i = 0; i < numberofships; i++)
            {
                spaceship[i].updateobject();
                spaceship[i].lives = 5;
                spaceship[i].score = 0;
            }
            resetlevel();
        }

        void resetlevel()
        {
            ballsinplay += 3;

            spaceship[0].spawntime = 0;
            spaceship[1].spawntime = 0;

            asteroidsdestroyed = 0;

            for (int i = 0; i < numberofballs; i++)
            {
                balls[i].velocity = new Vector3(randomiser.Next(9) - 4, randomiser.Next(9) - 4, 0);
                balls[i].velocity /= astspeed;
                balls[i].visible = true;
                

                do
                {
                    balls[i].position = new Vector3(randomiser.Next(displaywidth - balls[i].rect.Width) + balls[i].rect.Width / 2,
                        randomiser.Next(displayheight - balls[i].rect.Height) + balls[i].rect.Height / 2, 0);
                } while (checkcollision(i) > -1);
                
            }
            for (int i = 0; i < ballsinplay; i++)
                balls[i].oldposition = balls[i].position;
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

        int checkcollision(int ballnumber)
        {
            int hit = -1;

            for (int i=0;i<numberofships;i++)
            {
                if (balls[ballnumber].visible && spaceship[i].visible && spaceship[i].spawntime > lengthofspawn && balls[ballnumber].bsphere.Intersects(spaceship[i].bsphere))
                {
                    spaceship[i].lives--;
                    hit = ballnumber;
                    balls[ballnumber].visible = false;
                    spaceship[i].score += 10;
                    asteroidsdestroyed++;
                    spaceship[i].spawntime = 0;
                    vibration = 1000f;
                }

            }
            for (int counter = 0; counter < numberofballs; counter++)
                if (ballnumber != counter && balls[ballnumber].bsphere.Intersects(balls[counter].bsphere) && balls[ballnumber].visible && balls[counter].visible)
                {
                    hit = counter;
                    sfunctions.cresponse(balls[ballnumber].position, balls[counter].position, ref balls[ballnumber].velocity,
                    ref balls[counter].velocity, balls[ballnumber].size, balls[counter].size);

                    balls[ballnumber].position = balls[ballnumber].oldposition;
                    balls[counter].position = balls[counter].oldposition;
                    balls[ballnumber].updateobject();
                    balls[counter].updateobject();

                    //if (balls[counter].size < balls[ballnumber].size)
                    //    balls[counter].visible = false;
                    //else
                    //    balls[ballnumber].visible = false;
                }
            if (hit==-1)
                balls[ballnumber].oldposition = balls[ballnumber].position;

            return hit;
        }

        void checkshiphits(int ship1, int ship2)
        {
            if (spaceship[ship1].bsphere.Intersects(spaceship[ship2].bsphere))
            {
                sfunctions.cresponse(spaceship[ship1].position, spaceship[ship2].position, ref spaceship[ship1].velocity,
                    ref spaceship[ship2].velocity, spaceship[ship1].size, spaceship[ship2].size);
                spaceship[ship1].position = spaceship[ship1].oldposition;
                spaceship[ship2].position = spaceship[ship2].oldposition;
                vibration = 1000;
             

         
            }
            else
            {
                spaceship[ship1].oldposition = spaceship[ship1].position;
                spaceship[ship2].oldposition = spaceship[ship2].position;
            }
        }

        void bulletcheck(int ship1, int ship2)
        {
            if (bullet[ship1].visible && spaceship[ship2].visible && bullet[ship1].bsphere.Intersects(spaceship[ship2].bsphere) && spaceship[2].spawntime > lengthofspawn)
            {
                sfunctions.cresponse(bullet[ship1].position, spaceship[ship2].position, ref bullet[0].velocity,
                    ref spaceship[ship2].velocity, bullet[ship1].size, spaceship[ship2].size);

                bullet[ship1].visible = false;
                spaceship[ship2].lives--;
                spaceship[ship1].score += 50;
                explosion[ship2].start(spaceship[ship2].position, spaceship[ship2].rotation, false, 0);
                spaceship[2].spawntime = 0;
            }
        }
        //void bulletcheck2(int ship1, int ship2)
        //{
        //     for(int b=0;b<numberofbullets;b++)
        //    if (bullets5[ship1,b].visible && spaceship[ship2].visible && bullets5[ship1,b].bsphere.Intersects(spaceship[ship2].bsphere))
        //    {
        //        sfunctions.cresponse(bullet[ship1].position, spaceship[ship2].position, ref bullet[0].velocity,
        //            ref spaceship[ship2].velocity, bullet[ship1].size, spaceship[ship2].size);

        //        bullets5[ship1,b].visible = false;
        //        spaceship[ship2].lives--;
        //        spaceship[ship1].score += 50;
        //        explosion[ship2].start(spaceship[ship2].position, spaceship[ship2].rotation, false, 0);
        //    }
        //}
        void bulletast(int astnum, int ship)
        {
            for (int i = 0; i < ballsinplay; i++)
            {
                if(spaceship[ship].visible && bullet[ship].visible && bullet[ship].bsphere.Intersects(balls[i].bsphere) && bullet[ship].visible && balls[i].visible)
                {
                    balls[i].visible = false;
                    spaceship[ship].score += 20;
                    explosion[i].start(balls[i].position, bullet[ship].rotation, false, 0);
                    bullet[ship].visible = false;
                    asteroidsdestroyed++;

                }
            }
        }
        //void bulletast2(int astnum, int ship, int bullnum)
        //{
        //    for (int i = 0; i < ballsinplay; i++)
        //        for (int b = 0; b < numberofbullets; b++)
            
        //    {
        //        if (spaceship[ship].visible && bullets5[ship,bullnum].visible && bullets5[ship,bullnum].bsphere.Intersects(balls[i].bsphere) && bullets5[ship,bullnum].visible && balls[i].visible)
        //        {
        //            balls[i].visible = false;
        //            spaceship[ship].score += 20;
        //            explosion[i].start(balls[i].position, bullet[ship].rotation, false, 0);
        //            bullets5[ship,bullnum].visible = false;
        //            asteroidsdestroyed++;

        //        }
        //    }
        //}
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

 
            KeyboardState keys = Keyboard.GetState();
            GamePadState[] pad = new GamePadState[2];
            pad[0] = GamePad.GetState(PlayerIndex.One);
            pad[1] = GamePad.GetState(PlayerIndex.Two);

            float timebetweenupdates = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (pad[0].Buttons.Back == ButtonState.Pressed || pad[1].Buttons.Back == ButtonState.Pressed
                || keys.IsKeyDown(Keys.Escape))
                this.Exit();

            if (!gameover)
            {
                if (ybuttonreleased && pad[0].Buttons.Y == ButtonState.Pressed)
                    playtrack1 = !playtrack1;
                ybuttonreleased = pad[0].Buttons.Y == ButtonState.Released;

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


                if (pad[0].Buttons.X == ButtonState.Pressed)
                    gameover = true;

                if (spaceship[0].lives <= 0 && spaceship[1].lives <= 0)
                    gameover = true;

                vibration -= timebetweenupdates;
                if (vibration > 0 )
                {
                    GamePad.SetVibration(PlayerIndex.Two, 0.5f, 0.5f);
                    GamePad.SetVibration(PlayerIndex.One, 0.5f, 0.5f);
                }









                if (asteroidsdestroyed >= ballsinplay)
                    resetlevel();

                for (int i = 0; i < numberofships; i++)
                {
                    if (spaceship[i].lives <= 0) spaceship[i].visible = false;

                    if (spaceship[i].spawntime > lengthofspawn)
                    {
                        GamePad.SetVibration(PlayerIndex.One, 0, 0);
                        GamePad.SetVibration(PlayerIndex.Two, 0, 0);
                        spaceship[i].colour = Color.White;
                    }
                    else
                    {
                        spaceship[i].colour = Color.Red;
      
                   
                    }

                 
                    
                        if (pad[i].Buttons.A == ButtonState.Pressed && spaceship[i].visible)
                            bullet[i].firebullet(spaceship[i].position, spaceship[i].direction);
                        bullet[i].movebullet(timebetweenupdates);

                        spaceship[i].moveme(pad[i], displaywidth, displayheight, timebetweenupdates);
                
          
                }
                bulletast(0, 1);
                bulletast(1, 0);
                checkshiphits(0, 1);
                bulletcheck(0, 1);
                bulletcheck(1, 0);


                for (int i = 0; i < ballsinplay; i++)
                {
                    balls[i].automove(displaywidth, displayheight, timebetweenupdates);
                    int ballhit = checkcollision(i);
                    if (ballhit > -1)
                    {
                        collidesound.Play();
                    }
                }
               


           
                //for (int i = 0; i < ballsinplay; i++)
                //    for (int b = 0; b < numberofbullets; b++)
                //    {
                //        bulletast2(0 ,numberofballs, numberofbullets);
                //        bulletast2(1 ,numberofballs,numberofbullets);
                //    }

            

                //for (int i = 0; i < numberofships; i++)
                //    for (int b = 0; b < numberofbullets; b++)
                //        for (int s = 0; s < numberofships; s++)
                //        {
                //            if (pad[i].Buttons.A == ButtonState.Pressed && spaceship[i].visible)
                //                bullets5[s, b].firebullet2(spaceship[i].position, spaceship[i].direction);
                //            bullets5[s, b].movebullet2(timebetweenupdates, displaywidth, displayheight);
                //        }
     

                for (int i = 0; i < numberofballs; i++)
                explosion[i].update(timebetweenupdates);
            }
            else
            {
                if (music1.State == SoundState.Playing)
                    music1.Stop();
                if (music2.State == SoundState.Playing)
                    music2.Stop();

                if (pad[0].Buttons.Start == ButtonState.Pressed || pad[1].Buttons.Start == ButtonState.Pressed
                    || keys.IsKeyDown(Keys.Enter))
                    GamePad.SetVibration(PlayerIndex.One, 0, 0);
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

            spaceship[0].drawme(ref spriteBatch);
            spaceship[1].drawme(ref spriteBatch);


            for (int i = 0; i < numberofships; i++)
                bullet[i].drawme(ref spriteBatch);

            for (int i = 0; i < numberofships; i++)
                bullet[i].drawme(ref spriteBatch);

            spriteBatch.DrawString(mainfont, "P1 Lives " + spaceship[0].lives.ToString("0"), new Vector2(40, 40), Color.White,
                MathHelper.ToRadians(0), new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
            spriteBatch.DrawString(mainfont, "P2 Lives " + spaceship[1].lives.ToString("0"), new Vector2(displaywidth - 140, 40), Color.White,
                MathHelper.ToRadians(0), new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
            spriteBatch.DrawString(mainfont, " P1 Score " + spaceship[0].score.ToString("0"), new Vector2(40, 60), Color.White,
                MathHelper.ToRadians(0), new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
            spriteBatch.DrawString(mainfont, "P2 Score " + spaceship[1].score.ToString("0"), new Vector2(displaywidth - 140, 60), Color.White,
                MathHelper.ToRadians(0), new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
          
            
            //for (int i = 0; i < numberofships; i++)
            //    for (int b = 0; b < numberofbullets; b++)
            //        bullets5[i,b].drawme(ref spriteBatch);

            for (int i = 0; i < numberofballs; i++)
            explosion[i].drawme(ref spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
