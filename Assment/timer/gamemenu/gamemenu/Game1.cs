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
using System.IO;
using gamelib2d;

namespace timer
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
        
        const int numberofhighscores = 10; // int to hold number of highscores
        float[] highscore = new float[numberofhighscores];// array to the high scores
        string[] highscorename = new string[numberofhighscores];// arry to hold highscore names

        Boolean gameover = false;// set gmeover to false
        float gameruntime = 0;// holds the amount of time the game has been running

        Boolean released = false;// boolean to chech if player has released the button

        graphic2d background;// hold back ground picture
        Random randomiser = new Random();// used for creating random numbers.


        int gamestate = -1;// current state of game

        GamePadState[] pad = new GamePadState[4];// allows for 4 game oads to be connected
        KeyboardState keys;// allows the use of the keyboard
        MouseState mouse;// allows th euse of the mouse

        SpriteFont mainfont;// holds a font

        sprite2d mousepointer1, mousepointer2;// hold 2 graphics for the mouse pointer

        const int numberofoptions = 3;// number of options avaible on the menu
        sprite2d[,] menuoptions = new sprite2d[numberofoptions, 2];// creats an arry of menuoptions
        int optionselected = 0;// current selection





        scrollingbackground backgscroll;// allows for a background to scroll
        animatedsprite megaman;// main charecter
        Vector3 gameeoffset;// allows for the drawing of game based on the players position
        float score;// players score
        int lives = 3;// number of lives the player has
        int levelon = 1;// current level

        float gameallowedtime = 70000;// amount of time the player is allowed to play for
        graphic2d winner;// winner screan
        //        float timebetweenupdates;
        animation powerup;// give the player more time to play the game

        const int numberofspikes = 10;
        sprite2d[] spikes = new sprite2d[numberofspikes];// array of spikes

        graphic2d gameoverimage;// game over image



        float timeplaying = 0;// amount of time the player has been playing

        float invinsibletime = 1000;
        const int numberofballs = 40;
        sprite2d[] balls = new sprite2d[numberofballs];

        const int numofsuperballs = 5;
        animatedsprite[] superballs = new animatedsprite[numofsuperballs];

        const int numberofplatforms = 112;
        sprite2d[] platforms = new sprite2d[numberofplatforms];

        sprite2d collectable;
        const int numberofcollectables = 2;
        sprite2d[] collectable2 = new sprite2d[numberofcollectables];

        SoundEffect playerdeath, playerdeath2;// used for adding sound effects
        SoundEffect gamemusic;
        SoundEffectInstance music;// allows the music to played and stopped etc.

        const int numberofends = 7;
        sprite2d[] ends = new sprite2d[numberofends];// used to mark end of level


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

            // Load in high scores
            if (File.Exists(@"highscore.txt")) // This checks to see if the file exists
            {
                StreamReader sr = new StreamReader(@"highscore.txt");	// Open the file

                String line;		// Create a string variable to read each line into
                for (int i = 0; i < numberofhighscores && !sr.EndOfStream; i++)
                {
                    line = sr.ReadLine();	// Read the first line in the text file
                    highscorename[i] = line.Trim(); // Read high score name

                    if (!sr.EndOfStream)
                    {
                        line = sr.ReadLine();	// Read the first line in the text file
                        line = line.Trim(); 	// This trims spaces from either side of the text
                        highscore[i] = (int)Convert.ToDecimal(line);	// This converts line to numeric
                    }
                }
                sr.Close();			// Close the file
            }

            // sorts highscores and high score names
            Array.Sort(highscore, highscorename);
            Array.Reverse(highscore);
            Array.Reverse(highscorename);

            background = new graphic2d(Content, "backgroundformenus", displaywidth, displayheight);// load in graphic
            mousepointer1 = new sprite2d(Content, "X-Games-Cursor", 0, 0, 0.15f, Color.White, true);// loads in sprites
            mousepointer2 = new sprite2d(Content, "X-Games-Cursor-Highlight", 0, 0, 0.15f, Color.White, true);

            menuoptions[0, 0] = new sprite2d(Content, "1", displaywidth / 2, 75, 0.7f, Color.White, true);
            menuoptions[0, 1] = new sprite2d(Content, "2", displaywidth / 2, 75, .7f, Color.White, true);
            menuoptions[1, 0] = new sprite2d(Content, "5", displaywidth / 2, 232, .7f, Color.White, true);
            menuoptions[1, 1] = new sprite2d(Content, "6", displaywidth / 2, 232, .7f, Color.White, true);
            menuoptions[2, 0] = new sprite2d(Content, "7", displaywidth / 2, 388, 0.7f, Color.White, true);
            menuoptions[2, 1] = new sprite2d(Content, "8", displaywidth / 2, 388, 0.7f, Color.White, true);

            for (int i = 0; i < numberofoptions; i++)// update menu options
                menuoptions[i, 0].updateobject();

            mainfont = Content.Load<SpriteFont>("mainfont");


            //mainfont = Content.Load<SpriteFont>("quartz4");
            gameoverimage = new graphic2d(Content, "gameover", displaywidth, displayheight);
            gameoverimage.stretch2fit(displaywidth, displayheight);
            playerdeath = Content.Load<SoundEffect>("GLASS02");
            playerdeath2 = Content.Load<SoundEffect>("CRASH03");
            winner = new graphic2d(Content, "winner", displaywidth, displayheight);
            gamemusic = Content.Load<SoundEffect>("iron-man");
            music = gamemusic.CreateInstance();
            
            megaman = new animatedsprite(new Vector3(50, 500, 0), 0.95f, 2f, 1f, 4);// loads animation
            megaman.spriteanimation[0] = new animation(Content, "megamanx", 100, 100, 2f, Color.White, true, 13, 1, 11, true, false, false);// animatesa animation
            megaman.spriteanimation[1] = new animation(Content, "megamanx", 100, 100, 2f, Color.White, true, 13, 1, 11, true, false, true);
            megaman.spriteanimation[2] = new animation(Content, "megamanjump", 100, 100, 2f, Color.White, true, 14, 1, 7, false, false, false);
            megaman.spriteanimation[3] = new animation(Content, "megamanjump", 100, 100, 2f, Color.White, true, 14, 1, 7, false, false, true);


            for (int i = 0; i < numofsuperballs; i++)
            {
                superballs[i] = new animatedsprite(new Vector3(100, displayheight - 25, 0), 0.95f, 2f, 0.5f, 1);// loads animation
                superballs[i].spriteanimation[0] = new animation(Content, "cast_006", 0, 0, 0.3f, Color.White, true, 18, 4, 5, true, false, false);// animatesa animation
                superballs[i].state = 0;
                superballs[i].gravity = 0;
            }

            //resetgame();
        }
        void resetgame()// calls resetgame
        {
            levelon = 1;// current startinglevel
            score = 70000;
            lives = 20;
            gameallowedtime = 70000;
            resetlevel();
            gameover = false;
        }

        void resetlevel()// used for moving the player on to the next level
        {
            // Level 1
            if (levelon == 1)
            {

                megaman.position = new Vector3(50, 500, 0);

                backgscroll = new scrollingbackground(Content, "backgroundforscrolling", 4f, 3, 1);// scrools back ground
                backgscroll.makehorizontal(displayheight);


                balls[0] = new sprite2d(Content, "ball", 340, displayheight - 50, 0.2f, Color.White, true);// move balls
                for (int i = 1; i < numberofballs; i++)
                {
                    //balls[1] = new sprite2d(Content, "ball", 830, displayheight / 2, 0.2f, Color.White, true);
                    balls[i] = new sprite2d(Content, "ball", 500 + (i * 335), displayheight - 50, 0.2f, Color.White, true);

                }


                balls[5] = new sprite2d(Content, "ball", 500 + (5 * 335), displayheight - 25, 0.2f, Color.White, false);
                balls[6] = new sprite2d(Content, "ball", 500 + (8 * 335), displayheight - 25, 0.2f, Color.White, false);

                for (int i = 7; i < numberofballs; i++)
                    balls[i] = new sprite2d(Content, "ball", 500 + (i * 335), displayheight - 50, 0.2f, Color.White, false);

                for (int i = 7; i < numberofballs; i++)// make unsed balls invisible
                    balls[7].visible = false;

                platforms[0] = new sprite2d(Content, "red", 340, displayheight - 20, 1, Color.White, true);// draw platforms

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
                platforms[13] = new sprite2d(Content, "redflip", 3830, displayheight - 20, 1, Color.White, false);

                for (int i = 14; i < numberofplatforms; i++)
                    platforms[i] = new sprite2d(Content, "redflip", 849 + 990 + 600, displayheight + 500 + 200, 1, Color.White, false);


                spikes[0] = new sprite2d(Content, "spikes", 100, 500, 1, Color.White, false);// set the unsued spikes visiblity to false

                for (int i = 1; i < numberofspikes; i++)
                    spikes[i] = new sprite2d(Content, "spikes", platforms[9].rect.X + 330, displayheight - spikes[0].rect.Height / 2, 1, Color.White, false);


                for (int i = 0; i < numberofballs; i++)
                {
                    do
                    {
                        balls[i].velocity = new Vector3(0, (float)(randomiser.Next(20) + 1) / 2, 0);
                    } while (balls[i].velocity.Y == 0);// give the ball a random velocity ip and down
                }
                //for (int i = 0; i < numberofballs; i++)
                //{
                //    balls[i].velocity = new Vector3(0, -2, 0);
                //}

                for (int i = 0; i < numofsuperballs; i++)
                    superballs[i].visible = false;
                for (int i = 0; i < numofsuperballs; i++)
                {

//                    superballs[i].position = new Vector3(500 + (5 * 335), displayheight - 25, 0);
                    superballs[0].position = new Vector3(500 + (5 * 335), displayheight - 25, 0);
                    superballs[1].position = new Vector3(3630, displayheight - 25, 0);
                    superballs[0].visible = true;
                    superballs[1].visible = true;
                    
                }

                for (int i = 5; i < numberofballs; i++)
                    balls[i].velocity = new Vector3(2, 0, 0);

                collectable = new sprite2d(Content, "car", platforms[9].rect.X + 330, displayheight - 300, 1, Color.White, false);// Draw these as true when in use

                collectable2[0] = new sprite2d(Content, "car", platforms[9].rect.X + 330, displayheight - 300, 1, Color.White, false);
                collectable2[1] = new sprite2d(Content, "car", platforms[9].rect.X + 330, displayheight - 300, 1, Color.White, false);


                for (int i = 0; i < numberofends; i++)
                {
                    ends[i] = new sprite2d(Content, "green", 3830, displayheight - (i * 35), 1, Color.White, false);// used for checking when the player has reached the end.
                }
            }

            // Level 2
            if (levelon == 2)// loads level 2
            {
                backgscroll = new scrollingbackground(Content, "skypan2", 4f, 1, 1);// moves back ground
                backgscroll.makehorizontal(displayheight);

                for (int i = 0; i < numofsuperballs; i++)
                    superballs[i].visible = false;
                megaman.position = new Vector3(50, 500, 0);

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

                collectable = new sprite2d(Content, "key", platforms[9].rect.X + 330, displayheight - 300, 0.2f, Color.White, true);

                collectable2[0] = new sprite2d(Content, "car", platforms[9].rect.X + 330, displayheight - 300, 1, Color.White, false);
                collectable2[1] = new sprite2d(Content, "car", platforms[9].rect.X + 330, displayheight - 300, 1, Color.White, false);

                for (int i = 0; i < numberofspikes; i++)
                    spikes[i] = new sprite2d(Content, "spikes", platforms[9].rect.X + 330, displayheight - spikes[i].rect.Height / 2, 1, Color.White, true);

                animation powerup = new animation(Content, "cast_006", platforms[9].rect.X + 330, displayheight - 200, 0.1f, Color.White, true, 25, 6, 6);
                powerup.start(new Vector3(200, 300, 0));// decided not to use this 

                for (int i = 0; i < numofsuperballs; i++)
                    superballs[i].visible = false;

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
            if (levelon == 3)// loads level 3
            {
                backgscroll = new scrollingbackground(Content, "sky", 4f, 3, 1);
                backgscroll.makehorizontal(displayheight);

                megaman.position = new Vector3(50, 500, 0);

                for (int i = 0; i < numofsuperballs; i++)
                    superballs[i].visible = false;

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
                platforms[8] = new sprite2d(Content, "red", platforms[7].rect.X, displayheight - 75, 1, Color.White, true);
                platforms[9] = new sprite2d(Content, "diamond", platforms[7].rect.X + 60, displayheight - 120, 1, Color.White, true);
                platforms[12] = new sprite2d(Content, "diamond", platforms[9].rect.X + 1000, displayheight - 200, 1, Color.White, true);
                for (int i = 13; i < 17; i++)
                    platforms[i] = new sprite2d(Content, "triangle", i * platforms[9].rect.Width + 2700, displayheight - 300, 1, Color.White, true);

                platforms[20] = new sprite2d(Content, "diamond", platforms[19].rect.Width + 3400, displayheight - 100, 1, Color.White, true);

                for (int i = 21; i < numberofplatforms; i++)
                    platforms[i] = new sprite2d(Content, "triangle", 849 + 990 + 600, displayheight + 500 + 200, 1, Color.White, false);

                collectable = new sprite2d(Content, "key", platforms[9].rect.X + 100, 100, 0.2f, Color.White, true);
                collectable2[0] = new sprite2d(Content, "clock", 2285, 400, 0.3f, Color.White, true);

                spikes[0] = new sprite2d(Content, "spikes", 100, displayheight - 40, 1, Color.White, true);

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
            if (levelon == 4)// load level 5
            {
                backgscroll = new scrollingbackground(Content, "backgroundscrolling2", 4f, 2, 1);
                backgscroll.makehorizontal(displayheight);

                for (int i = 0; i < numofsuperballs; i++)
                    superballs[i].visible = false;

                megaman.position = new Vector3(50, 500, 0);


                for (int i = 0; i < numberofplatforms; i++)
                    platforms[i] = new sprite2d(Content, "red", (i * 70), displayheight + 100 + platforms[i].rect.Height / 2, 1, Color.White, false);
                for (int i = 56; i < numberofplatforms; i++)
                    platforms[i] = new sprite2d(Content, "redflip", platforms[0].rect.X + (10 * 1), 100 + platforms[i].rect.Height / 2, 1, Color.White, false);


                for (int i = 0; i < numberofspikes; i++)
                    spikes[i] = new sprite2d(Content, "spikes", 100 + (i * spikes[0].rect.Width), displayheight + 340, 1, Color.White, false);

                //balls[0] = new sprite2d(Content, "ball", 50, displayheight - 50, 0.2f, Color.White, true);
                for (int i = 7; i < numberofballs; i++)
                {
                    //balls[1] = new sprite2d(Content, "ball", 830, displayheight / 2, 0.2f, Color.White, true);
                    balls[i] = new sprite2d(Content, "ball", 40 + (i * 120), displayheight - 50, 0.2f, Color.White, true);

                }

                balls[39] = new sprite2d(Content, "ball", 340, displayheight - balls[39].rect.Height / 2, 0.2f, Color.White, false);
           

                for (int i = 0; i < numberofballs; i++)
                {
                    do
                    {
                        balls[i].velocity = new Vector3(0, (float)(randomiser.Next(40) + 1) / 2f, 0);
                    } while (balls[i].velocity.Y == 0);
                }
                balls[39].velocity = new Vector3(8, 0, 0);
                //superballs[3].velocity = new Vector3(8, 0, 0);

                for (int i = 0; i < numberofends; i++)
                {
                    ends[i] = new sprite2d(Content, "green", 3830, displayheight - (i * 35), 1, Color.White, false);
                }

                collectable = new sprite2d(Content, "key", 2980, 205, 0.2f, Color.White, true);

                collectable2[0] = new sprite2d(Content, "saucer2", platforms[9].rect.X + 330, displayheight - 300, 1, Color.White, false);
                collectable2[1] = new sprite2d(Content, "saucer2", platforms[9].rect.X + 330, displayheight - 300, 1, Color.White, false);
            }
            if (levelon == 5)// load level 5
            {
                backgscroll = new scrollingbackground(Content, "skypan2", 4f, 1, 1);
                backgscroll.makehorizontal(displayheight);

                //for (int i = 0; i < numofsuperballs; i++)
                //    superballs[i].visible = false;

                for (int i = 0; i < numofsuperballs; i++)
                {
                    if (numofsuperballs < 2)
                        superballs[i].visible = false;
                    else
                        superballs[i].visible = true;

                    //superballs[i].velocity = new Vector3(100, 0, 0);
                }
                superballs[3].visible = false;
                
                superballs[2].position = new Vector3(1140, displayheight - 25, 0);
                superballs[3].position = new Vector3(1500, displayheight - 25, 0);
                superballs[4].position = new Vector3(2500, displayheight - 25, 0);
              

                megaman.position = new Vector3(50, 500, 0);

                platforms[0] = new sprite2d(Content, "orange", 200, 350, 1, Color.White, true);
                for (int i = 0; i < numberofplatforms; i++)
                    platforms[i] = new sprite2d(Content, "orange", (i * 700), 350, 1, Color.White, true);


                //balls[37] = new sprite2d(Content, "ball", 540, displayheight - balls[39].rect.Height / 2, 0.2f, Color.White, false);

                //balls[38] = new sprite2d(Content, "ball", 1500, displayheight - balls[39].rect.Height / 2, 0.2f, Color.White, false);
                //balls[39] = new sprite2d(Content, "ball", 2500, displayheight - balls[39].rect.Height / 2, 0.2f, Color.White, false);

                for (int i = 0; i < numberofballs; i++)
                {
                    balls[i].visible = false;
                }
                //balls[37].visible = true;
                //balls[37].velocity = new Vector3((float)(randomiser.Next(12) + 8), 0, 0);
                //balls[38].visible = true;
                //balls[38].velocity = new Vector3((float)(randomiser.Next(12) + 8), 0, 0);

                //balls[39].visible = true;
                //balls[39].velocity = new Vector3((float)(randomiser.Next(12) + 8), 0, 0);

                platforms[16] = new sprite2d(Content, "orange", 3780, displayheight - 25, 1, Color.White, false);
                platforms[17] = new sprite2d(Content, "orange", 100, displayheight - 25, 1, Color.White, false);

                for (int i = 0; i < numberofends; i++)
                {
                    ends[i] = new sprite2d(Content, "green", 3830, displayheight - (i * 35), 1, Color.White, false);
                }

                collectable = new sprite2d(Content, "key", 2980, 205, 0.2f, Color.White, true);

                collectable2[0] = new sprite2d(Content, "car", platforms[9].rect.X + 330, displayheight - 300, 1, Color.White, false);
                collectable2[1] = new sprite2d(Content, "car", platforms[9].rect.X + 330, displayheight - 300, 1, Color.White, false);
            }
            if (levelon == 6)/// if player gets here the game will end 
            {
                gameover = true;

            }
        }


        public void checkcollision(animatedsprite mega, sprite2d fballs)// // checks for collision between player and balls move vertically
        {
            if (mega.bbox.Intersects(fballs.bbox) && invinsibletime >= 1000 && fballs.visible)
            {
                lives--;
                invinsibletime = 0;
                gameallowedtime -= 1000f;
                playerdeath.Play();
            }
        }

        public void checkcollision2(animatedsprite mega, sprite2d fballs)// checks for collision between player and balls move horizontally
        {
            if (mega.bbox.Intersects(fballs.bbox) && invinsibletime >= 1000 && fballs.visible)
            {
                lives--;
                invinsibletime = 0;
                gameallowedtime -= 1000f;
                playerdeath2.Play();
            }
        }
        public void checkcollision3(animatedsprite mega, animatedsprite aballs)// checks for collision between player and animations move horizontally
        {
            if (mega.bbox.Intersects(aballs.bbox) && invinsibletime >= 1000 && aballs.visible)
            {
                lives--;
                invinsibletime = 0;
                gameallowedtime -= 1000f;
                playerdeath2.Play();
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()// unloads high score
        {

            StreamWriter sw = new StreamWriter(@"highscore.txt");
            for (int i = 0; i < numberofhighscores; i++)
            {
                sw.WriteLine(highscorename[i]);
                sw.WriteLine(highscore[i].ToString());
            }
            sw.Close();

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            pad[0] = GamePad.GetState(PlayerIndex.One);// allows the use of the game pad
            pad[1] = GamePad.GetState(PlayerIndex.Two);
            pad[2] = GamePad.GetState(PlayerIndex.Three);
            pad[3] = GamePad.GetState(PlayerIndex.Four);
            keys = Keyboard.GetState();
            mouse = Mouse.GetState();

            this.Window.Title = "Beat the Clock       Current Level: " + levelon.ToString("0");

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)// allows the game to exit
                this.Exit();

            float timebetweenupdates = (float)gameTime.ElapsedGameTime.TotalMilliseconds;// a float mesussring the time gone inthe game used for keeping the running at the same speed on all platforms
            gameruntime += timebetweenupdates;// adds the time the game has been running to time betwee updates
            gameallowedtime -= timebetweenupdates;// take time away fro the player
            score = gameallowedtime;

            mousepointer1.position.X = mouse.X;// mouses poisiton
            mousepointer1.position.Y = mouse.Y;
            mousepointer1.updateobject();

            mousepointer1.bsphere = new BoundingSphere(mousepointer1.position, 2);// mouses bounding sphere

            switch (gamestate)// calls each case when  prompted
            {
                case -1:
                    updatemenu();
                    break;
                case 0:
                    updategame(timebetweenupdates);
                    break;
                case 1:
                    updateoptions();
                    break;

                case 2:
                    updatehighscore();
                    break;
                default:
                    this.Exit();
                    break;

            }


            // TODO: Add your update logic here

            base.Update(gameTime);
        }
        public void updatemenu()
        {
            GamePadState pad = GamePad.GetState(PlayerIndex.One);

            if (released)                                                   //Updates Menu Based On User Input
            {
                if (pad.ThumbSticks.Left.Y > 0.5f)
                {
                    optionselected--;
                    released = false;

                }
                if (pad.ThumbSticks.Left.Y < -0.5f)
                {
                    optionselected++;
                    released = false;

                }
            }
            else
                if (Math.Abs(pad.ThumbSticks.Left.Y) < 0.5f)
                {
                    released = true;
                }


            for (int i = 0; i < numberofoptions; i++)
                if (pad.Buttons.A == ButtonState.Pressed)
                {

                    gamestate = optionselected;
                    if (gamestate == 0)
                        resetgame();
                    released = false;

                }


            for (int i = 0; i < numberofoptions; i++)
            {
                if (mousepointer1.bsphere.Intersects(menuoptions[i, 0].bbox))
                {
                    optionselected = i;
                    if (mouse.LeftButton == ButtonState.Pressed)
                        gamestate = optionselected;

                    if (gamestate == 0)
                        resetgame();
                }
            }


        }
        public void drawmenu()// draws menu
        {
            spriteBatch.Begin();

            for (int i = 0; i < numberofoptions; i++)
            {
                if (optionselected == i)
                    menuoptions[i, 1].drawme(ref spriteBatch);
                else
                    menuoptions[i, 0].drawme(ref spriteBatch);
            }
            if (optionselected > -1)
            {
                mousepointer2.rect = mousepointer1.rect;
                mousepointer2.drawme(ref spriteBatch);
            }
            else
                mousepointer1.drawme(ref spriteBatch);

            spriteBatch.End();
        }

        public void updategame(float gtime)// updates hte game
        {
            if (!gameover)
            {
                music.Play();

                if (pad[0].Buttons.Start == ButtonState.Pressed)
                    gameover = false;// exist the game if the player presses start
                if (lives == 0)
                {
                    gameover = true;// stops the game if the playerdeath ghas nolives
                }

                if (gameallowedtime < 0) gameover = true;// stops the game if the player rns out of time

                megaman.move(pad[0], gtime, backgscroll.gamewidth, backgscroll.gameheight, false);// move the background based on user input.


                for (int i = 0; i < numofsuperballs; i++)
                {
                    superballs[i].automove2(backgscroll.gamewidth, backgscroll.gameheight,gtime);
                }
          


                for (int q = 0; q < numofsuperballs; q++)
                for (int i = 0; i < numberofplatforms; i++)
                    if (superballs[q].bbox.Intersects(platforms[i].bbox))
                    {
                        superballs[q].velocity.X = -superballs[q].velocity.X;
                        superballs[q].power = -superballs[q].power;
                    }
                //balls[0].automove(platforms[0].rect.Height, platforms[5].rect.Height, timebetweenupdates);
                for (int i = 0; i < 5; i++)
                {
                    balls[i].automove(platforms[3].rect.Y - platforms[3].rect.Height / 2, platforms[5].rect.Y + platforms[5].rect.Height / 2, gtime);// moves and bounces off the ball off platforms when collision occur
                }


                balls[5].automove2(platforms[9].rect.X + platforms[9].rect.Width / 2, platforms[10].rect.X - platforms[10].rect.Width / 2, gtime);// moves and bounces off the ball off platforms when collision occur

                for (int i = 0; i < numberofends; i++)
                    balls[6].automove2(platforms[10].rect.X + platforms[10].rect.Width / 2, ends[i].rect.X + ends[i].rect.Width / 2, gtime);// stop balls leving the level
                for (int i = 7; i < numberofballs; i++)
                    balls[i].automove(displayheight, 0, gtime);

                balls[37].automove2(0, backgscroll.imagemain.Width * 4 - 260, gtime);// bounces ball
                balls[38].automove2(0, backgscroll.imagemain.Width * 4 - 260, gtime);
                balls[39].automove2(0, backgscroll.imagemain.Width * 4 - 260, gtime);

                platforms[0].automove3(0, backgscroll.imagemain.Width * 4 - 260, gtime);// moves platfomrs this is not used in the game
                platforms[1].automove3(100, backgscroll.imagemain.Width * 4 - 860, gtime);

                for (int i = 0; i < numberofballs; i++)
                    checkcollision(megaman, balls[i]);// looks for collision between player and ball

                //if(levelon ==4)
                //for (int i = 0; i < numberofplatforms; i++)
                //    checkcollision2(platforms[i], platforms[i]);

                for (int i = 0; i < numberofspikes; i++)
                    checkcollision2(megaman, spikes[i]);// looks for collison between player and spikes
                for (int i = 0; i < numofsuperballs; i++)
                    checkcollision3(megaman, superballs[i]);// looks for collison between player and spikes
                //if (levelon == 2)
                //    powerup.update(timebetweenupdates);

                invinsibletime += gtime;
                timeplaying += gtime;



                if (megaman.bbox.Intersects(collectable.bbox))// make the collectable invsble if player collects it
                    collectable.visible = false;

                for (int i = 0; i < numberofends; i++)
                    if (megaman.bbox.Intersects(ends[i].bbox) && collectable.visible == false)// starts a new level by increase the levelon by 1.
                    {
                        levelon++;
                        resetlevel();
                    }


                if (megaman.bbox.Intersects(collectable2[0].bbox) && collectable2[0].visible)// make the collectable invsble if player collects it and adds time
                {
                    gameallowedtime += 10000;
                    collectable2[0].visible = false;
                }
                if (megaman.bbox.Intersects(collectable2[1].bbox) && collectable2[1].visible)
                {
                    gameallowedtime += 15000;
                    collectable2[1].visible = false;
                }

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

                for (int i = 0; i < numberofhighscores; i++)// if player beats high score add player score to high core table
                    if (gameover && gameallowedtime > 0 && score > highscore[9])//&& score > gameallowedtime)
                    {
                        highscore[9] = score;
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
                if (pad[0].Buttons.X == ButtonState.Pressed)
                    megaman.position.X -= 10;
                if (pad[0].Buttons.B == ButtonState.Pressed)
                    megaman.position.X += 10;
                if (keys.IsKeyDown(Keys.Space))
                    megaman.jump(40);


            }

            else
            {
                //gameruntime += timebetweenupdates;

                if (pad[0].Buttons.Start == ButtonState.Pressed)
                    gamestate = -1;
                music.Stop();
                if (keys.IsKeyDown(Keys.Escape)) gamestate = -1;
            }


        }
        public void drawgame()// draws game
        {
            spriteBatch.Begin();

            megaman.screenposition = backgscroll.drawme(ref spriteBatch, megaman.position, displaywidth, displayheight, out gameeoffset);

            megaman.drawme(ref spriteBatch);

            foreach(animatedsprite sball in superballs)
            {
                sball.drawme(ref spriteBatch, sball.position - gameeoffset);
            }

            //spriteBatch.DrawString(mainfont, (gameallowedtime / 1000f).ToString("0.0"), new Vector2(displaywidth / 2 - 50, 10),
            //Color.Black, MathHelper.ToRadians(0), new Vector2(0, 0), 1.5f, SpriteEffects.None, 0);

            spriteBatch.DrawString(mainfont,"Time Till Death " + "   " +  (score).ToString("0"), new Vector2(displaywidth / 2 - 400, 10),
    Color.Black, MathHelper.ToRadians(0), new Vector2(0, 0), 1.5f, SpriteEffects.None, 0);
            //spriteBatch.DrawString(mainfont, "Score" + "  " + score.ToString("0"), new Vector2(10, 10),
            //Color.White, MathHelper.ToRadians(0), new Vector2(0, 0), 1f, SpriteEffects.None, 0);
            //       spriteBatch.DrawString(mainfont, "Lives" + "  " + lives.ToString("0"), new Vector2(displaywidth - 200, 10),
            //Color.White, MathHelper.ToRadians(0), new Vector2(0, 0), 1f, SpriteEffects.None, 0);


            spriteBatch.DrawString(mainfont,"Highscore" + "  " + (highscore[0]).ToString("0"), new Vector2(displaywidth / 2 + 50, 10),
Color.Blue, MathHelper.ToRadians(0), new Vector2(0, 0), 1.5f, SpriteEffects.None, 0);


            for (int i = 0; i < numberofplatforms; i++)
                platforms[i].drawme(ref spriteBatch, platforms[i].position - gameeoffset);

            for (int i = 0; i < numberofballs; i++)
                balls[i].drawme(ref spriteBatch, balls[i].position - gameeoffset);

            collectable.drawme(ref spriteBatch, collectable.position - gameeoffset);

            collectable2[0].drawme(ref spriteBatch, collectable2[0].position - gameeoffset);
            collectable2[1].drawme(ref spriteBatch, collectable2[1].position - gameeoffset);

            for (int i = 0; i < numberofspikes; i++)
                spikes[i].drawme(ref spriteBatch, spikes[i].position - gameeoffset);

            for (int i = 0; i < numberofends; i++)
                ends[i].drawme(ref spriteBatch, ends[i].position - gameeoffset);
            //if (levelon == 2)
            //    powerup.drawme(ref spriteBatch);



            if (gameover)// draws the game over of game winner image depending on players score
            {
                if (gameallowedtime >= 0)
                {
                    winner.drawme(ref spriteBatch);
                    winner.stretch2fit(displaywidth, displayheight);
                }
                else
                {
                    gameoverimage.drawme(ref spriteBatch);
                }
            }

            spriteBatch.End();
        }

        public void updateoptions()// update options on the menu
        {
            if (keys.IsKeyDown(Keys.Escape)) gamestate = -1;
            GamePadState pad = GamePad.GetState(PlayerIndex.One);
            if (pad.Buttons.B == ButtonState.Pressed) gamestate = -1;
        }
        public void drawoptions()// draws th eoptions
        {
            spriteBatch.Begin();

            if (optionselected > -1)
            {
                mousepointer2.rect = mousepointer1.rect;
                mousepointer2.drawme(ref spriteBatch);
            }
            else
                mousepointer1.drawme(ref spriteBatch);

            spriteBatch.End();
        }

        public void updatehighscore()// update highscore
        {
            GamePadState pad = GamePad.GetState(PlayerIndex.One);
            if (keys.IsKeyDown(Keys.Escape)) gamestate = -1;
            if (pad.Buttons.B == ButtonState.Pressed) gamestate = -1;
        }
        public void drawhighscore()// draws highscore
        {
            spriteBatch.Begin();


            spriteBatch.DrawString(mainfont, "High Score".ToString(), new Vector2(displaywidth / 2 - 100, 20), Color.Red, 0,
            new Vector2(0, 0), 1.5f, SpriteEffects.None, 0);

            for (int i = 0; i < numberofhighscores; i++)
                spriteBatch.DrawString(mainfont, " " + (i + 1) + ". " + highscore[i].ToString("0"), new Vector2(displaywidth / 2 - 100, (i * 40 + 60)), Color.Yellow, 0,
                new Vector2(0, 0), 1.5f, SpriteEffects.None, 0);

            if (optionselected > -1)
            {
                mousepointer2.rect = mousepointer1.rect;
                mousepointer2.drawme(ref spriteBatch);
            }
            else
                mousepointer1.drawme(ref spriteBatch);

            spriteBatch.End();
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

            spriteBatch.End();

            switch (gamestate)// draw the corrrect images depending on what state the game is in.
            {
                case -1:
                    drawmenu();
                    break;
                case 0:
                    drawgame();
                    break;
                case 1:
                    drawoptions();
                    break;

                case 2:
                    drawhighscore();
                    break;
                default:
                    this.Exit();
                    break;

            }

            base.Draw(gameTime);
        }
    }
}
