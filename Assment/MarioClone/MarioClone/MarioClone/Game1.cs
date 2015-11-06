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
using gamelib2d;

namespace MarioClone
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        int displaywidth; int displayheight;


        SpriteFont mainfont;
        scrollingbackground backgscroll;
        animatedsprite megaman;
        Vector3 gameeoffset;
        int score = 0;
        int lives = 3;
        int levelon = 1;

        graphic2d winner;
        

        float timebetweenupdates;
        float gameruntime;

        animation powerup;

        const int numberofspikes = 10;
        sprite2d[] spikes = new sprite2d[numberofspikes];

        graphic2d gameoverimage;
        Boolean gameover = false;

        Random randomiser = new Random();

        float timeplaying = 0;

        float invinsibletime = 1000;
        const int numberofballs = 40;
        sprite2d[] balls = new sprite2d[numberofballs];

        const int numberofplatforms = 112;
        sprite2d[] platforms = new sprite2d[numberofplatforms];

        sprite2d collectable, collectable2;

        SoundEffect playerdeath, playerdeath2;
        SoundEffect gamemusic;
        SoundEffectInstance music;

        const int numberofends = 7;
        sprite2d[] ends = new sprite2d[numberofends];
       // sprite2d[] ends2 = new sprite2d[numberofends];

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.graphics.PreferredBackBufferWidth = 800;
            this.graphics.PreferredBackBufferHeight = 480;
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


            mainfont = Content.Load<SpriteFont>("quartz4");
            gameoverimage = new graphic2d(Content, "gameover", displaywidth, displayheight);
            playerdeath = Content.Load<SoundEffect>("GLASS02");
            playerdeath2 = Content.Load<SoundEffect>("CRASH03");
            winner = new graphic2d(Content, "winner", displaywidth, displayheight);
            gamemusic = Content.Load<SoundEffect>("iron-man");
            music = gamemusic.CreateInstance();

            resetgame();

        }

        void resetgame()
        {
            levelon = 1;
            score = 0;
            lives = 300;

            resetlevel();
        }
        void resetlevel()
        {
            // Level 1
            if (levelon == 1)
            {
                megaman = new animatedsprite(new Vector3(50, 500, 0), 0.95f, 2f, 1f, 4);
                megaman.spriteanimation[0] = new animation(Content, "megamanx", 100, 100, 2f, Color.White, true, 13, 1, 11, true, false, false);
                megaman.spriteanimation[1] = new animation(Content, "megamanx", 100, 100, 2f, Color.White, true, 13, 1, 11, true, false, true);
                megaman.spriteanimation[2] = new animation(Content, "megamanjump", 100, 100, 2f, Color.White, true, 14, 1, 7, false, false, false);
                megaman.spriteanimation[3] = new animation(Content, "megamanjump", 100, 100, 2f, Color.White, true, 14, 1, 7, false, false, true);

                backgscroll = new scrollingbackground(Content, "skypan", 4f, 1, 1);
                backgscroll.makehorizontal(displayheight);


                balls[0] = new sprite2d(Content, "ball", 340, displayheight - 50, 0.2f, Color.White, true);
                for (int i = 1; i < numberofballs; i++)
                {
                    //balls[1] = new sprite2d(Content, "ball", 830, displayheight / 2, 0.2f, Color.White, true);
                    balls[i] = new sprite2d(Content, "ball", 500 + (i * 335), displayheight - 50, 0.2f, Color.White, true);

                }
              

                balls[5] = new sprite2d(Content, "ball", 500 + (5 * 335), displayheight - 25, 0.2f, Color.White, true);
                balls[6] = new sprite2d(Content, "ball", 500 + (8 * 335), displayheight - 25, 0.2f, Color.White, true);

                for (int i = 7; i < numberofballs; i++)
                    balls[i] = new sprite2d(Content, "ball", 500 + (i * 335), displayheight - 50, 0.2f, Color.White, false);

                for (int i = 7; i < numberofballs; i++)
                    balls[7].visible = false;

                platforms[0] = new sprite2d(Content, "red", 340, displayheight - 20, 1, Color.White, true);

                for (int i = 1; i < 5; i++)
                    platforms[i] = new sprite2d(Content, "red", 500 + (i * platforms[0].rect.Width) * 5, displayheight - 20, 1, Color.White, true);
                platforms[5] = new sprite2d(Content, "redflip", 339, platforms[0].rect.Height / 2, 1, Color.White, true);
                platforms[6] = new sprite2d(Content, "redflip", 834, platforms[0].rect.Height / 2, 1, Color.White, true);
                platforms[7] = new sprite2d(Content, "redflip", 839 + 330, platforms[0].rect.Height / 2, 1, Color.White, true);
                platforms[8] = new sprite2d(Content, "redflip", 845 + 330 + 330, platforms[0].rect.Height / 2, 1, Color.White, true);
                platforms[9] = new sprite2d(Content, "redflip", 849 + 330 + 330 + 330, platforms[0].rect.Height / 2, 1, Color.White, true);

                platforms[10] = new sprite2d(Content, "red", 849 + 990 + 600, displayheight - platforms[0].rect.Height / 2, 1, Color.White, true);
                platforms[11] = new sprite2d(Content, "redflip", 849 + 990 + 600, displayheight - 200, 1, Color.White, true);
                platforms[12] = new sprite2d(Content, "redflip", 849 + 990 + 600, displayheight - 200, 1, Color.White, true);

                for (int i = 13; i < numberofplatforms; i++)
                    platforms[i] = new sprite2d(Content, "redflip", 849 + 990 + 600, displayheight + 500 + 200, 1, Color.White, false);


                spikes[0] = new sprite2d(Content, "spikes", 100, 500, 1, Color.White, false);

                for (int i = 1; i < numberofspikes; i++)
                    spikes[i] = new sprite2d(Content, "spikes", platforms[9].rect.X + 330, displayheight - spikes[0].rect.Height / 2, 1, Color.White, false);


                for (int i = 0; i < numberofballs; i++)
                {
                    do
                    {
                        balls[i].velocity = new Vector3(0, (float)(randomiser.Next(20) + 1) / 2, 0);
                    } while (balls[i].velocity.Y == 0);
                }
                //for (int i = 0; i < numberofballs; i++)
                //{
                //    balls[i].velocity = new Vector3(0, -2, 0);
                //}
                for (int i = 5; i < numberofballs; i++)
                    balls[i].velocity = new Vector3(2, 0, 0);

                collectable = new sprite2d(Content, "car", platforms[9].rect.X + 330, displayheight - 300, 1, Color.White, false);

                for (int i = 0; i < numberofends; i++)
                {
                    ends[i] = new sprite2d(Content, "green", 3830, displayheight - (i * 35), 1, Color.White, false);
                }
            }

            // Level 2
            if (levelon == 2)
            {
                backgscroll = new scrollingbackground(Content, "skypan2", 4f, 1, 1);
                backgscroll.makehorizontal(displayheight);


                megaman = new animatedsprite(new Vector3(50, 500, 0), 0.95f, 2f, 1f, 4);
                megaman.spriteanimation[0] = new animation(Content, "megamanx", 100, 100, 2f, Color.White, true, 13, 1, 11, true, false, false);
                megaman.spriteanimation[1] = new animation(Content, "megamanx", 100, 100, 2f, Color.White, true, 13, 1, 11, true, false, true);
                megaman.spriteanimation[2] = new animation(Content, "megamanjump", 100, 100, 2f, Color.White, true, 14, 1, 7, false, false, false);
                megaman.spriteanimation[3] = new animation(Content, "megamanjump", 100, 100, 2f, Color.White, true, 14, 1, 7, false, false, true);

                balls[0] = new sprite2d(Content, "ball", 340, displayheight - 50, 0.2f, Color.White, true);
                for (int i = 0; i < numberofballs; i++)
                {
                    //balls[1] = new sprite2d(Content, "ball", 830, displayheight / 2, 0.2f, Color.White, true);
                    balls[i] = new sprite2d(Content, "ball", 500 + (i * 335), displayheight - 50, 0.2f, Color.White, false);

                }
                //balls[5] = new sprite2d(Content, "ball", 500 + (5 * 335), displayheight - 25, 0.2f, Color.White, false);
                //balls[6] = new sprite2d(Content, "ball", 500 + (8 * 335), displayheight - 25, 0.2f, Color.White, false);

                platforms[0] = new sprite2d(Content, "green", 340, displayheight - 20, 1, Color.White, true);

                for (int i = 1; i < 5; i++)
                    platforms[i] = new sprite2d(Content, "green", 500 + (i * platforms[0].rect.Width) * 5, displayheight - 20 - (i * 40), 1, Color.White, true);
                for (int i = 5; i < numberofplatforms; i++)
                    platforms[i] = new sprite2d(Content, "green", platforms[4].rect.X + 330 + (i * platforms[3].rect.Width), displayheight / 2, 1, Color.White, true);
                //platforms[6] = new sprite2d(Content, "greenflip", 834, platforms[0].rect.Height / 2, 1, Color.White, false);
                //platforms[7] = new sprite2d(Content, "greenflip", 839 + 330, platforms[0].rect.Height / 2, 1, Color.White, false);
                //platforms[8] = new sprite2d(Content, "greenflip", 845 + 330 + 330, platforms[0].rect.Height / 2, 1, Color.White, false);
                //platforms[9] = new sprite2d(Content, "greenflip", 849 + 330 + 330 + 330, platforms[0].rect.Height / 2, 1, Color.White, false);

                platforms[10] = new sprite2d(Content, "green", platforms[4].rect.X + 330, displayheight / 2, 1, Color.White, true);
                platforms[11] = new sprite2d(Content, "green", platforms[9].rect.X + 330, displayheight / 2, 1, Color.White, true);
                platforms[12] = new sprite2d(Content, "greenflip", platforms[9].rect.X + 330, displayheight - 400, 1, Color.White, true);

                for (int i = 13; i < numberofplatforms; i++)
                    platforms[i] = new sprite2d(Content, "redflip", 849 + 990 + 600, displayheight + 500 + 200, 1, Color.White, false);

                collectable = new sprite2d(Content, "car", platforms[9].rect.X + 330, displayheight - 300, 0.1f, Color.White, true);

                for (int i = 0; i < numberofspikes; i++)
                spikes[i] = new sprite2d(Content, "spikes", platforms[9].rect.X + 330, displayheight - spikes[i].rect.Height / 2, 1, Color.White, true);

                animation powerup = new animation(Content, "cast_006", platforms[9].rect.X + 330, displayheight - 200, 0.1f, Color.White, true, 25, 6, 6);
                powerup.start(new Vector3(200, 300, 0));

              

                for (int i = 0; i < numberofends; i++)
                {
                    ends[i] = new sprite2d(Content, "green", 3830, displayheight - (i * 35), 1, Color.White, false);
                }


                //for (int i = 0; i < numberofballs; i++)
                //{
                //    do
                //    {
                //        balls[i].velocity = new Vector3(0, (float)(randomiser.Next(20) + 1) / 2, 0);
                //    } while (balls[i].velocity.Y == 0);
                //}
            }


            // Level 3
            if (levelon == 3)
            {
                backgscroll = new scrollingbackground(Content, "sky", 4f, 3, 1);
                backgscroll.makehorizontal(displayheight);


                megaman = new animatedsprite(new Vector3(50, 350, 0), 0.95f, 2f, 1f, 4);
                megaman.spriteanimation[0] = new animation(Content, "megamanx", 100, 100, 2f, Color.White, true, 13, 1, 11, true, false, false);
                megaman.spriteanimation[1] = new animation(Content, "megamanx", 100, 100, 2f, Color.White, true, 13, 1, 11, true, false, true);
                megaman.spriteanimation[2] = new animation(Content, "megamanjump", 100, 100, 2f, Color.White, true, 14, 1, 7, false, false, false);
                megaman.spriteanimation[3] = new animation(Content, "megamanjump", 100, 100, 2f, Color.White, true, 14, 1, 7, false, false, true);

               
                balls[0] = new sprite2d(Content, "ball", 340, displayheight - 50, 0.2f, Color.White, false);
                for (int i = 1; i < numberofballs; i++)
                {
                    //balls[1] = new sprite2d(Content, "ball", 830, displayheight / 2, 0.2f, Color.White, true);
                    balls[i] = new sprite2d(Content, "ball", 500 + (i * 335), displayheight - 50, 0.2f, Color.White, false);

                }
                balls[5] = new sprite2d(Content, "ball", 500 + (5 * 335), displayheight - 25, 0.2f, Color.White, true);
                balls[6] = new sprite2d(Content, "ball", 500 + (8 * 335), displayheight - 25, 0.2f, Color.White, true);

                platforms[0] = new sprite2d(Content, "diamond", 50, displayheight - 100, 1, Color.White, true);
                for (int i = 1; i < 5; i++)
                    platforms[i] = new sprite2d(Content, "diamond", 80 + (i * platforms[0].rect.Width) * 4, displayheight - 100, 1, Color.White, true);

                for (int i = 5; i < 8; i++)
                    platforms[i] = new sprite2d(Content, "diamond", (i * platforms[0].rect.Width - 50) * 5, displayheight - 20 - (i * 40), 1, Color.White, true);


                platforms[10] = new sprite2d(Content, "diamond", (platforms[9].rect.X - 400), displayheight - 200, 1, Color.White, true);
                platforms[11] = new sprite2d(Content, "diamond", platforms[9].rect.X - 50, displayheight - 150, 1, Color.White, true);
                platforms[8] = new sprite2d(Content, "diamond", platforms[7].rect.X, displayheight - 75, 1, Color.White, true);
                platforms[9] = new sprite2d(Content, "diamond", platforms[7].rect.X + 60, displayheight - 120, 1, Color.White, true);
                platforms[12] = new sprite2d(Content, "diamond", platforms[9].rect.X + 1000, displayheight - 200, 1, Color.White, true);
                for (int i = 13; i < 17; i++)
                    platforms[i] = new sprite2d(Content, "triangle", i * platforms[9].rect.Width + 2700, displayheight - 300, 1, Color.White, true);

                platforms[20] = new sprite2d(Content, "diamond", platforms[19].rect.Width + 3400, displayheight -100, 1, Color.White, true);

                for (int i = 21; i < numberofplatforms; i++)
                    platforms[i] = new sprite2d(Content, "triangle", 849 + 990 + 600, displayheight + 500 + 200, 1, Color.White, false);

                collectable = new sprite2d(Content, "car", platforms[9].rect.X + 100, 100, 0.1f, Color.White, true);
                spikes[0] = new sprite2d(Content, "spikes",100 , displayheight - 40, 1, Color.White, true);

                for (int i = 1; i < numberofspikes; i++)
                    spikes[i] = new sprite2d(Content, "spikes", 100 + (i * spikes[0].rect.Width), displayheight - 40, 1, Color.White, true);


                for (int i = 0; i < numberofends; i++)
                {
                    ends[i] = new sprite2d(Content, "green", 3830, (i * 35), 1, Color.White, false);
                }
     

                for (int i = 0; i < numberofballs; i++)
                {
                    do
                    {
                        balls[i].velocity = new Vector3(0, (float)(randomiser.Next(40) + 1) / 2f, 0);
                    } while (balls[i].velocity.Y == 0);
                }

            }
            if (levelon == 4)
            {
                backgscroll = new scrollingbackground(Content, "skypan2", 4f, 1, 1);
                backgscroll.makehorizontal(displayheight);


                megaman = new animatedsprite(new Vector3(50, 500, 0), 0.95f, 2f, 1f, 4);
                megaman.spriteanimation[0] = new animation(Content, "megamanx", 100, 100, 2f, Color.White, true, 13, 1, 11, true, false, false);
                megaman.spriteanimation[1] = new animation(Content, "megamanx", 100, 100, 2f, Color.White, true, 13, 1, 11, true, false, true);
                megaman.spriteanimation[2] = new animation(Content, "megamanjump", 100, 100, 2f, Color.White, true, 14, 1, 7, false, false, false);
                megaman.spriteanimation[3] = new animation(Content, "megamanjump", 100, 100, 2f, Color.White, true, 14, 1, 7, false, false, true);


                for (int i = 0; i < numberofplatforms; i++)
                    platforms[i] = new sprite2d(Content, "red",  (i * 70), displayheight  + 100 + platforms[i].rect.Height/2, 1, Color.White, false);
                for (int i = 56; i < numberofplatforms; i++)
                    platforms[i] = new sprite2d(Content, "redflip", platforms[0].rect.X + (10 * 1), 100 + platforms[i].rect.Height / 2, 1, Color.White, false);


                for (int i = 0; i < numberofspikes; i++)
                    spikes[i] = new sprite2d(Content, "spikes", 100 + (i * spikes[0].rect.Width), displayheight + 340, 1, Color.White, false);

                //balls[0] = new sprite2d(Content, "ball", 50, displayheight - 50, 0.2f, Color.White, true);
                for (int i = 7; i < numberofballs; i++)
                {
                    //balls[1] = new sprite2d(Content, "ball", 830, displayheight / 2, 0.2f, Color.White, true);
                    balls[i] = new sprite2d(Content, "ball", 40 +(i * 120), displayheight - 50, 0.2f, Color.White, true);

                }

                balls[39] = new sprite2d(Content, "ball", 340, displayheight - balls[39].rect.Height / 2, 0.2f, Color.White, true);
                 
                for (int i = 0; i < numberofballs; i++)
                {
                    do
                    {
                        balls[i].velocity = new Vector3(0, (float)(randomiser.Next(40) + 1) / 2f, 0);
                    } while (balls[i].velocity.Y == 0);
                }
                balls[39].velocity = new Vector3(8, 0, 0);

                for (int i = 0; i < numberofends; i++)
                {
                    ends[i] = new sprite2d(Content, "green", 3830, displayheight - (i * 35)  , 1, Color.White, false);
                }

                collectable = new sprite2d(Content, "car", 2980, 205, 0.1f, Color.White, true);
            }
            if (levelon == 5)
            {
                backgscroll = new scrollingbackground(Content, "skypan2", 4f, 1, 1);
                backgscroll.makehorizontal(displayheight);


                megaman = new animatedsprite(new Vector3(50, 500, 0), 0.95f, 2f, 1f, 4);
                megaman.spriteanimation[0] = new animation(Content, "megamanx", 100, 100, 2f, Color.White, true, 13, 1, 11, true, false, false);
                megaman.spriteanimation[1] = new animation(Content, "megamanx", 100, 100, 2f, Color.White, true, 13, 1, 11, true, false, true);
                megaman.spriteanimation[2] = new animation(Content, "megamanjump", 100, 100, 2f, Color.White, true, 14, 1, 7, false, false, false);
                megaman.spriteanimation[3] = new animation(Content, "megamanjump", 100, 100, 2f, Color.White, true, 14, 1, 7, false, false, true);

                platforms[0] = new sprite2d(Content, "orange", 200, 350, 1, Color.White, true);
                for (int i = 0; i < numberofplatforms; i++)
                platforms[i] = new sprite2d(Content, "orange", ( i *700), 350, 1, Color.White, true);
              

                balls[37] = new sprite2d(Content, "ball", 540, displayheight - balls[39].rect.Height / 2, 0.2f, Color.White, true);

                balls[38] = new sprite2d(Content, "ball", 1500, displayheight - balls[39].rect.Height / 2, 0.2f, Color.White, true);
                balls[39] = new sprite2d(Content, "ball", 2500, displayheight - balls[39].rect.Height / 2, 0.2f, Color.White, true);

                for (int i = 0; i < numberofballs; i++)
                {
                    balls[i].visible = false;
                }
                balls[37].visible = true;
                balls[37].velocity = new Vector3((float)(randomiser.Next(12) + 8) , 0, 0);
                balls[38].visible = true;
                balls[38].velocity = new Vector3((float)(randomiser.Next(12) + 8) , 0, 0);

                balls[39].visible = true;
                balls[39].velocity = new Vector3((float)(randomiser.Next(12) + 8) , 0, 0);

                for (int i = 0; i < numberofends; i++)
                {
                    ends[i] = new sprite2d(Content, "green", 3830, displayheight - (i * 35), 1, Color.White, false);
                }

                    collectable = new sprite2d(Content, "car", 2980, 205, 0.1f, Color.White, true);
            }
            if (levelon == 6)
            {
                gameover = true;
            }
        }
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        public void checkcollision(animatedsprite mega, sprite2d fballs)
        {
            if (mega.bbox.Intersects(fballs.bbox) && invinsibletime >= 1000 && fballs.visible)
            {
                lives--;
                invinsibletime = 0;
                gameruntime += 1000f;
                playerdeath.Play();
            }
        }

        public void checkcollision2(animatedsprite mega, sprite2d fballs)
        {
            if (mega.bbox.Intersects(fballs.bbox) && invinsibletime >= 1000 && fballs.visible)
            {
                lives--;
                invinsibletime = 0;
                gameruntime += 1000f;
                playerdeath2.Play();
            }
        }

        //public void checkcollision2(sprite2d mega, sprite2d fballs)
        //{
        //    if (mega.bbox.Intersects(fballs.bbox))
        //    {
        //        mega.velocity.X =- fballs.velocity.X;
        //        mega.position.X = fballs.position.X / 2;
        //    }
        //}
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

            GamePadState[] pad = new GamePadState[4];
            pad[0] = GamePad.GetState(PlayerIndex.One);
            KeyboardState keys = Keyboard.GetState();

            if (keys.IsKeyDown(Keys.Escape) || pad[0].Buttons.Back == ButtonState.Pressed)
                this.Exit();
            timebetweenupdates = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            gameruntime += (float)gameTime.TotalGameTime.TotalSeconds;
            
            if (!gameover)
            {
                music.Play();

                if (lives == 0)
                {
                    gameover = true;
                }

                megaman.move(pad[0], timebetweenupdates, backgscroll.gamewidth, backgscroll.gameheight, false);

          
                if (pad[0].Buttons.Start == ButtonState.Pressed)
                    gameover = true;

                //balls[0].automove(platforms[0].rect.Height, platforms[5].rect.Height, timebetweenupdates);
                for (int i = 0; i < 5; i++)
                {
                    balls[i].automove(platforms[3].rect.Y - platforms[3].rect.Height / 2, platforms[5].rect.Y + platforms[5].rect.Height / 2, timebetweenupdates);
                }


                balls[5].automove2(platforms[9].rect.X + platforms[9].rect.Width / 2, platforms[10].rect.X - platforms[10].rect.Width / 2, timebetweenupdates);

                for (int i = 0; i < numberofends; i++)
                    balls[6].automove2(platforms[10].rect.X + platforms[10].rect.Width / 2, ends[i].rect.X + ends[i].rect.Width / 2, timebetweenupdates);
                for (int i = 7; i < numberofballs; i++)
                    balls[i].automove(displayheight, 0, timebetweenupdates);

                balls[37].automove2(0, backgscroll.imagemain.Width * 4 - 260, timebetweenupdates);
                balls[38].automove2(0, backgscroll.imagemain.Width * 4 - 260, timebetweenupdates);
                balls[39].automove2(0, backgscroll.imagemain.Width * 4 - 260, timebetweenupdates);

                platforms[0].automove3(0, backgscroll.imagemain.Width * 4 - 260, timebetweenupdates);
                platforms[1].automove3(100, backgscroll.imagemain.Width * 4 - 860, timebetweenupdates);
          
                for (int i = 0; i < numberofballs; i++)
                    checkcollision(megaman, balls[i]);

                //if(levelon ==4)
                //for (int i = 0; i < numberofplatforms; i++)
                //    checkcollision2(platforms[i], platforms[i]);

                for (int i = 0; i < numberofspikes; i++)
                    checkcollision2(megaman, spikes[i]);
          
                //if (levelon == 2)
                //    powerup.update(timebetweenupdates);
         
                invinsibletime += timebetweenupdates;
                timeplaying += timebetweenupdates;

                for (int i = 0; i < numberofends; i++)
                    if (megaman.bbox.Intersects(ends[i].bbox) && collectable.visible == false)
                    {
                        levelon++;
                        resetlevel();
                    }

                if (megaman.bbox.Intersects(collectable.bbox))
                    collectable.visible = false;
            
                Boolean collision = false;
                Boolean collision2 = false;
                // Check for collisions with platforms
                for (int i = 0; i < numberofplatforms; i++)
                    if (megaman.bbox.Intersects(platforms[i].bbox))
                    {
                        collision = true;

                        // If dude hits left or right side of platform bounce him back
                        if ((megaman.bboxold.Max.X < platforms[i].bbox.Min.X || megaman.bboxold.Min.X > platforms[i].bbox.Max.X)
                            && megaman.bboxold.Max.Y > platforms[i].bbox.Min.Y && megaman.bboxold.Min.Y < platforms[i].bbox.Max.Y && platforms[i].visible)
                        {
                            megaman.velocity.X = -(megaman.velocity.X / 1.5f);
                            megaman.velocity.Y /= 1.5f;
                            megaman.position = megaman.oldposition;
                            collision2 = true;

                        }
                        else
                            if (megaman.velocity.Y < 0)
                            {
                                // If he hits underneath the platform stop him going through it and reverse his velocity.
                                // Remove the 4 lines of code underneath if you want to allow the character to jump through the platforms
                                megaman.velocity.Y = 0.001f;
                                megaman.position.Y = megaman.oldposition.Y;
                                collision2 = true;

                            }
                            else if (megaman.bboxold.Max.Y < platforms[i].bbox.Min.Y)
                            {
                                // Dude is dropping onto platform or is already on top of platform, set his velocity vertically to 0 and set his position to the top of the platform
                                megaman.velocity.Y = 0;
                                megaman.position.Y = platforms[i].bbox.Min.Y - megaman.spriteanimation[megaman.state].rect.Height / 2;
                            }
                    }

                // If no collisions occured store last safe position
                if (!collision)
                {
                    megaman.bboxold = megaman.bbox;
                    megaman.oldposition = megaman.position;
                }
                else if (!collision2)
                {
                    if (megaman.velocity.X >= 0)
                        megaman.oldposition.X = megaman.position.X;
                    else
                        megaman.oldposition.X = megaman.position.X;
                    megaman.oldposition.Y = megaman.position.Y - 2;
                }

                if (pad[0].Buttons.A == ButtonState.Pressed)
                    megaman.jump(40);

            }

            else
            {
                gameruntime = timebetweenupdates;

                if (pad[0].Buttons.Start == ButtonState.Pressed)
                    resetgame();
                music.Stop();
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

            megaman.screenposition = backgscroll.drawme(ref spriteBatch, megaman.position, displaywidth, displayheight, out gameeoffset);

            megaman.drawme(ref spriteBatch);

            spriteBatch.DrawString(mainfont, gameruntime.ToString("0.0"), new Vector2(displaywidth / 2 - 50, 10),
            Color.Blue, MathHelper.ToRadians(0), new Vector2(0, 0), 1.5f, SpriteEffects.None, 0);
            spriteBatch.DrawString(mainfont, "Score" + "  " + score.ToString("0"), new Vector2(10, 10),
            Color.White, MathHelper.ToRadians(0), new Vector2(0, 0), 1f, SpriteEffects.None, 0);
            spriteBatch.DrawString(mainfont, "Lives" + "  " + lives.ToString("0"), new Vector2(displaywidth - 200, 10),
     Color.White, MathHelper.ToRadians(0), new Vector2(0, 0), 1f, SpriteEffects.None, 0);



            for (int i = 0; i < numberofplatforms; i++)
                platforms[i].drawme(ref spriteBatch, platforms[i].position - gameeoffset);

            for (int i = 0; i < numberofballs; i++)
                balls[i].drawme(ref spriteBatch, balls[i].position - gameeoffset);

            collectable.drawme(ref spriteBatch, collectable.position - gameeoffset);
            for (int i = 0; i < numberofspikes; i++)
            spikes[i].drawme(ref spriteBatch, spikes[i].position - gameeoffset);

            for (int i = 0; i < numberofends; i++)
                ends[i].drawme(ref spriteBatch, ends[i].position - gameeoffset);
            //if (levelon == 2)
            //    powerup.drawme(ref spriteBatch);



              if(gameover)
                  gameoverimage.drawme(ref spriteBatch); 

            if (gameover && levelon == 6 && gameruntime < 70000)

                winner.drawme(ref spriteBatch);
          

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
