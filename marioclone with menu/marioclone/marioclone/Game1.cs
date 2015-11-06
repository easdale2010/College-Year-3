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
using System.IO;


namespace marioclone
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
        SpriteFont mainfont;        // Main font for drawing in-game text

        Boolean gameover = false;   // Is the game over TRUE or FALSE?      
        float gameruntime = 0;      // Time since game started

        graphic2d background;       // Background image
        Random randomiser = new Random();       // Variable to generate random numbers

        int gamestate = -1;         // Current game state
        GamePadState[] pad = new GamePadState[4];       // Array to hold gamepad states
        KeyboardState keys;                             // Variable to hold keyboard state
        MouseState mouse;                               // Variable to hold mouse state
        Boolean released = true;                        // Check for sticks or buttons being released

        sprite2d mousepointer1, mousepointer2;          // Sprite to hold a mouse pointer
        const int numberofoptions = 4;                    // Number of main menu options
        sprite2d[,] menuoptions = new sprite2d[numberofoptions, 2]; // Array of sprites to hold the menu options
        int optionselected = 0;                         // Current menu option selected

        const int numberofhighscores = 10;                              // Number of high scores to store
        int[] highscores = new int[numberofhighscores];                 // Array of high scores
        string[] highscorenames = new string[numberofhighscores];       // Array of high score names

        scrollingbackground2 backgscroll;    // Scrolling background
        animatedsprite cutedude;    // Animated moving sprite for the main in game character
        Vector3 gameoffset;         // Amount that all game objects should be offset by when drawn based on the main character's position
        const int numberofplatforms = 40;
        sprite2d[] platform = new sprite2d[numberofplatforms];

        SoundEffect hitsound;

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
            // TODO: Add your initialization logic here
            displaywidth = graphics.GraphicsDevice.Viewport.Width;
            displayheight = graphics.GraphicsDevice.Viewport.Height;
            //graphics.ToggleFullScreen(); // Put game into full screen mode

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

            // TODO: use this.Content to load your game content here
            background = new graphic2d(Content, "Background for Menus", displaywidth, displayheight);
            mousepointer1 = new sprite2d(Content, "X-Games-Cursor", 0, 0, 0.15f, Color.White, true);
            mousepointer2 = new sprite2d(Content, "X-Games-Cursor-Highlight", 0, 0, 0.15f, Color.White, true);

            menuoptions[0, 0] = new sprite2d(Content, "Start-Normal", displaywidth / 2, 100, 1, Color.White, true);
            menuoptions[0, 1] = new sprite2d(Content, "Start-Selected", displaywidth / 2, 100, 1, Color.White, true);
            menuoptions[1, 0] = new sprite2d(Content, "Options-Normal", displaywidth / 2, 200, 1, Color.White, true);
            menuoptions[1, 1] = new sprite2d(Content, "Options-Selected", displaywidth / 2, 200, 1, Color.White, true);
            menuoptions[2, 0] = new sprite2d(Content, "High-Score-Normal", displaywidth / 2, 300, 1, Color.White, true);
            menuoptions[2, 1] = new sprite2d(Content, "High-Score-Selected", displaywidth / 2, 300, 1, Color.White, true);
            menuoptions[3, 0] = new sprite2d(Content, "Exit-Normal", displaywidth / 2, 400, 1, Color.White, true);
            menuoptions[3, 1] = new sprite2d(Content, "Exit-Selected", displaywidth / 2, 400, 1, Color.White, true);
            for (int i = 0; i < numberofoptions; i++)
            {
                menuoptions[i, 0].updateobject();
            }

            
            backgscroll = new scrollingbackground2(Content, "skypan", 4f, 6, 1);
            backgscroll.image[1, 0] = Content.Load<Texture2D>("skypan2");
           // backgscroll.image[2, 0] = Content.Load<Texture2D>("skypan2");
          //  backgscroll.image[5, 0] = Content.Load<Texture2D>("skypan2");
            backgscroll.makehorizontal(displayheight);
            mainfont = Content.Load<SpriteFont>("quartz4"); // Load font

            // Initialise game character, setting position, friction and gravity
            cutedude = new animatedsprite(new Vector3(50, 300, 0), 0.95f, 2f, 1f, 4);
            //            cutedude.spriteanimation[0] = new animation(Content, "superguy1", 100, 100, 2f, Color.White, true, 6, 1, 4, true);
            //          cutedude.spriteanimation[1] = new animation(Content, "superguy2", 100, 100, 2f, Color.White, true, 6, 1, 4, true);
            cutedude.spriteanimation[0] = new animation(Content, "cuteman", 100, 100, 0.8f, Color.White, true, 6, 1, 4, true,false);
            cutedude.spriteanimation[1] = new animation(Content, "cuteman2", 100, 100, 0.8f, Color.White, true, 6, 1, 4, true, true);
            cutedude.spriteanimation[2] = new animation(Content, "cutejump", 100, 100, 0.8f, Color.White, true, 14, 1, 7, false, false);
            cutedude.spriteanimation[3] = new animation(Content, "cutejump2", 100, 100, 0.8f, Color.White, true, 14, 1, 7, false,true);

            hitsound = Content.Load<SoundEffect>("ballhit");

            // Values for sonic type game
            //cutedude = new animatedsprite(new Vector3(300, 300, 0), 0.995f, 1.5f, 2f); 
            //cutedude.running = new animation(Content, "sonic", 100, 100, 1f, Color.White, true, 24, 1, 4);

            // cutedude = new animatedsprite(new Vector3(300, 300, 0), 0.95f, 1f);
            // cutedude.running = new animation(Content, "dragon", 100, 100, 1f, Color.White, true, 24, 4, 3);

            // Load the platforms and position them
            platform[0] = new sprite2d(Content, "BRICK_blue", 450, 350, 1, Color.White, true);
            for (int i = 1; i < numberofplatforms; i++)
                platform[i] = new sprite2d(Content, "BRICK_blue", 500 + (i * platform[0].rect.Width), 250, 1, Color.White, true);
            for (int i = 11; i < numberofplatforms; i++)
                platform[i] = new sprite2d(Content, "BRICK_blue", 500 + (i * platform[0].rect.Width), 350, 1, Color.White, true);

            platform[8] = new sprite2d(Content, "BRICK_green", (int)platform[8].position.X, (int)(platform[8].position.Y - 150), 1f, Color.White, true);
            platform[9] = new sprite2d(Content, "BRICK_green", (int)platform[9].position.X, (int)(platform[9].position.Y - 150), 1f, Color.White, true);
            platform[10] = new sprite2d(Content, "BRICK_green", (int)platform[16].position.X, (int)(platform[16].position.Y - platform[16].rect.Height), 1f, Color.White, true);
            platform[18] = new sprite2d(Content, "BRICK_green", 3100, 420, 3f, Color.White, true);
            platform[19] = new sprite2d(Content, "BRICK_green", 3500, 220, 1.5f, Color.White, true);

            for (int i = 20; i < numberofplatforms; i++)
                platform[i] = new sprite2d(Content, "BRICK_blue", (int)platform[19].bbox.Max.X + ((i-17) * platform[0].rect.Width), 250, 1, Color.White, true);

            // Load in high scores
            if (File.Exists(@"highscore.txt")) // This checks to see if the file exists
            {
                StreamReader sr = new StreamReader(@"highscore.txt");	// Open the file

                String line;		// Create a string variable to read each line into
                for (int i = 0; i < numberofhighscores && !sr.EndOfStream; i++)
                {
                    line = sr.ReadLine();	// Read the first line in the text file
                    highscorenames[i] = line.Trim(); // Read high score name

                    if (!sr.EndOfStream)
                    {
                        line = sr.ReadLine();	// Read the first line in the text file
                        line = line.Trim(); 	// This trims spaces from either side of the text
                        highscores[i] = (int)Convert.ToDecimal(line);	// This converts line to numeric
                    }
                }
                sr.Close();			// Close the file
            }

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            // Save high scores
            StreamWriter sw = new StreamWriter(@"highscore.txt");
            for (int i = 0; i < numberofhighscores; i++)
            {
                sw.WriteLine(highscorenames[i]);
                sw.WriteLine(highscores[i].ToString());
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
            pad[0] = GamePad.GetState(PlayerIndex.One);     // Reads gamepad 1
            pad[1] = GamePad.GetState(PlayerIndex.Two);     // Reads gamepad 2
            pad[2] = GamePad.GetState(PlayerIndex.Three);   // Reads gamepad 2
            pad[3] = GamePad.GetState(PlayerIndex.Four);    // Reads gamepad 2
            keys = Keyboard.GetState();       // Read keyboard
            mouse = Mouse.GetState();            // Read Mouse

            float timebetweenupdates = (float)gameTime.ElapsedGameTime.TotalMilliseconds; // Time between updates
            gameruntime += timebetweenupdates;  // Count how long the game has been running for

            // Read the mouse and set the mouse cursor
            mousepointer1.position.X = mouse.X;
            mousepointer1.position.Y = mouse.Y;
            mousepointer1.updateobject();
            // Set a small bounding sphere at the center of the mouse cursor
            mousepointer1.bsphere = new BoundingSphere(mousepointer1.position, 2);

            // TODO: Add your update logic here
            switch (gamestate)
            {
                case -1:
                    // Game is on the main menu
                    updatemenu();
                    break;
                case 0:
                    // Game is being played
                    updategame(timebetweenupdates);
                    break;
                case 1:
                    // Options menu
                    updateoptions();
                    break;
                case 2:
                    // High Score table
                    updatehighscore();
                    break;
                default:
                    // Do something if none of the above are selected
                    this.Exit();    // Quit Game
                    break;
            }

            base.Update(gameTime);
        }

        
        public void updatemenu()
        {
            // Check for mousepointer being over a menu option
            for (int i = 0; i < numberofoptions; i++)
            {
                // Check for up and down on left stick of pad1 for navagating the menu options
                if (released)
                {
                    if (pad[0].ThumbSticks.Left.Y > 0.5f)
                    {
                        optionselected--;
                        released = false;
                    }
                    if (pad[0].ThumbSticks.Left.Y < -0.5f)
                    {
                        optionselected++;
                        released = false;
                    }
                }
                else
                {
                    if (Math.Abs(pad[0].ThumbSticks.Left.Y) < 0.5)
                        released = true;
                }

                // Impose limits on the selectio of menu options 
                if (optionselected < 0) optionselected = 0;
                if (optionselected >= numberofoptions) optionselected = numberofoptions - 1;

                // Check for mouse over a menu option
                if (mousepointer1.bsphere.Intersects(menuoptions[i, 0].bbox))
                {
                    optionselected = i;
                    if (mouse.LeftButton == ButtonState.Pressed)
                        gamestate = optionselected;
                }

                if (pad[0].Buttons.A == ButtonState.Pressed)
                    gamestate = optionselected;

            }

            if (gamestate == 0)
                reset();
        }

        // Reset values for the start of a new game
        void reset()
        {
            gameover = false;
            cutedude.position.X = 50;
            cutedude.position.Y = 300;
            cutedude.velocity.X = 0;
            cutedude.velocity.Y = 0;

        }
        
        public void drawmenu()
        {
            spriteBatch.Begin();
            // Draw menu options
            for (int i = 0; i < numberofoptions; i++)
            {
                if (optionselected == i)
                    menuoptions[i, 1].drawme(ref spriteBatch);
                else
                    menuoptions[i, 0].drawme(ref spriteBatch);
            }

            // Draw mouse
            if (optionselected > -1)
            {
                mousepointer2.rect = mousepointer1.rect;
                mousepointer2.drawme(ref spriteBatch);
            }
            else
                mousepointer1.drawme(ref spriteBatch);

            spriteBatch.End();
        }

        public void updategame(float gtime)
        {
            // Main game code
            if (!gameover)
            {
                // Main is being played
                // move game character and update his position
                cutedude.move(pad[0], gtime, backgscroll.gamewidth, backgscroll.gameheight, false);

                Boolean collision = false;
                Boolean collision2 = false;
                // Check for collisions with platforms
                for (int i = 0; i < numberofplatforms; i++)
                    if (cutedude.bbox.Intersects(platform[i].bbox))
                    {
                        collision = true;

                        // If dude hits left or right side of platform bounce him back
                        if ((cutedude.bboxold.Max.X < platform[i].bbox.Min.X || cutedude.bboxold.Min.X > platform[i].bbox.Max.X)
                            && cutedude.bboxold.Max.Y > platform[i].bbox.Min.Y && cutedude.bboxold.Min.Y < platform[i].bbox.Max.Y)
                        {
                            cutedude.velocity.X = -(cutedude.velocity.X / 1.5f);
                            cutedude.velocity.Y /= 1.5f;
                            cutedude.position = cutedude.oldposition;
                            collision2 = true;
                            hitsound.Play();
                        }
                        else
                            if (cutedude.velocity.Y < 0)
                            {
                                // If he hits underneath the platform stop him going through it and reverse his velocity.
                                // Remove the 4 lines of code underneath if you want to allow the character to jump through the platforms
                                cutedude.velocity.Y = 0.001f;
                                cutedude.position.Y = cutedude.oldposition.Y;
                                collision2 = true;
                                hitsound.Play();
                            }
                            else if (cutedude.bboxold.Max.Y < platform[i].bbox.Min.Y)
                            {
                                // Dude is dropping onto platform or is already on top of platform, set his velocity vertically to 0 and set his position to the top of the platform
                                cutedude.velocity.Y = 0;
                                cutedude.position.Y = platform[i].bbox.Min.Y - cutedude.spriteanimation[cutedude.state].rect.Height / 2;
                            }
                    }

                // If no collisions occured store last safe position
                if (!collision)
                {
                    cutedude.bboxold = cutedude.bbox;
                    cutedude.oldposition = cutedude.position;
                }
                else if (!collision2)
                {
                    if (cutedude.velocity.X >= 0)
                        cutedude.oldposition.X = cutedude.position.X;
                    else
                        cutedude.oldposition.X = cutedude.position.X;
                    cutedude.oldposition.Y = cutedude.position.Y - 2;
                }

                // Allow game character to jump if A is pressed on pad
                if (pad[0].Buttons.A == ButtonState.Pressed || keys.IsKeyDown(Keys.Space))
                    cutedude.jump(40);

                // Adjust the speed of the animation depending on the speed the character is running at
                if (cutedude.state < 2)
                {
                    cutedude.spriteanimation[cutedude.state].framespersecond = (int)Math.Abs(cutedude.velocity.X * 0.5f);
                    if (cutedude.spriteanimation[cutedude.state].framespersecond <= 1)
                        cutedude.spriteanimation[cutedude.state].framespersecond = 0;

                    // Pause the animation is the sprite stops moving
                    cutedude.spriteanimation[cutedude.state].paused = (Math.Round(cutedude.velocity.X, 0) == 0);
                }

                if (keys.IsKeyDown(Keys.Escape) || pad[0].Buttons.Back == ButtonState.Pressed)
                    gameover = true;    // End Game
            }
            else
            {
                // Game is over

                // Allow game to return to the main menu
                if (keys.IsKeyDown(Keys.Escape) || pad[0].Buttons.B == ButtonState.Pressed)
                {
                    // SORT HIGH SCORE TABLE
                    Array.Sort(highscores, highscorenames);
                    Array.Reverse(highscores);
                    Array.Reverse(highscorenames);

                    gamestate = -1;
                }
            }
        }

        public void drawgame()
        {
            // Draw the in-game graphics
            spriteBatch.Begin();

            // Draw the scrolling background
            cutedude.screenposition = backgscroll.drawme(ref spriteBatch, cutedude.position, displaywidth, displayheight, out gameoffset);

            for (int i = 0; i < numberofplatforms; i++)
                platform[i].drawme(ref spriteBatch, platform[i].position - gameoffset);

            // Draw the main game character
            cutedude.drawme(ref spriteBatch);

            // Show the characters horizontal speed on the screen
            if (!gameover)
            {
                spriteBatch.DrawString(mainfont, "Speed X" + cutedude.velocity.X.ToString("000.0"), new Vector2(10, 10),
                    Color.White, MathHelper.ToRadians(0), new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                spriteBatch.DrawString(mainfont, "Speed Y" + cutedude.velocity.Y.ToString("000.0"), new Vector2(10, 30),
                    Color.White, MathHelper.ToRadians(0), new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                spriteBatch.DrawString(mainfont, "Position X" + cutedude.position.X.ToString("0000.0"), new Vector2(10, 80),
                    Color.White, MathHelper.ToRadians(0), new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                spriteBatch.DrawString(mainfont, "Position Y" + cutedude.position.Y.ToString("0000.0"), new Vector2(10, 100),
                    Color.White, MathHelper.ToRadians(0), new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
            }
            else
            {
                spriteBatch.DrawString(mainfont, "GAME OVER" , new Vector2(130, 100),
                    Color.White, MathHelper.ToRadians(0), new Vector2(0, 0), 3f, SpriteEffects.None, 0);
            }
            
            spriteBatch.End();
        }

        public void updateoptions()
        {
            // Update code for the options screen

            // Allow game to return to the main menu
            if (keys.IsKeyDown(Keys.Escape) || pad[0].Buttons.B == ButtonState.Pressed) gamestate = -1;
        }

        public void drawoptions()
        {
            // Draw graphics for OPTIONS screen
            spriteBatch.Begin();
            // Draw mouse
            if (optionselected > -1)
            {
                mousepointer2.rect = mousepointer1.rect;
                mousepointer2.drawme(ref spriteBatch);
            }
            else
                mousepointer1.drawme(ref spriteBatch);
            spriteBatch.End();
        }

        public void updatehighscore()
        {
            // Update code for the high score screen

            // Allow game to return to the main menu
            if (keys.IsKeyDown(Keys.Escape) || pad[0].Buttons.B == ButtonState.Pressed) gamestate = -1;

        }

        public void drawhighscore()
        {
            // Draw graphics for High Score table
            spriteBatch.Begin();

            // Draw top ten high scores
            for (int i = 0; i < numberofhighscores; i++)
            {
                if (highscorenames[i].Length>=24)
                    spriteBatch.DrawString(mainfont, (i + 1).ToString("0") + ". " + highscorenames[i].Substring(0,24), new Vector2(60, 100 + (i * 30)),
                        Color.White, MathHelper.ToRadians(0), new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                else
                    spriteBatch.DrawString(mainfont, (i + 1).ToString("0") + ". " + highscorenames[i], new Vector2(60, 100 + (i * 30)),
                        Color.White, MathHelper.ToRadians(0), new Vector2(0, 0), 1f, SpriteEffects.None, 0);

                spriteBatch.DrawString(mainfont, highscores[i].ToString("0"), new Vector2(displaywidth - 180, 100 + (i * 30)),
                    Color.White, MathHelper.ToRadians(0), new Vector2(0, 0), 1f, SpriteEffects.None, 0);
            }

            
            // Draw mouse
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

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            background.drawme(ref spriteBatch);
            spriteBatch.End();

            // Draw stuff depending on the game state
            switch (gamestate)
            {
                case -1:
                    // Game is on the main menu
                    drawmenu();
                    break;
                case 0:
                    // Game is being played
                    drawgame();
                    break;
                case 1:
                    // Options menu
                    drawoptions();
                    break;
                case 2:
                    // High Score table
                    drawhighscore();
                    break;
                default:
                    break;
            }
      
            base.Draw(gameTime);
        }
    }
}
