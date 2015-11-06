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
using gamelib3d;
using System.IO;

namespace _3dgame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Screen height and width
        int displaywidth=800;
        int displayheight=600;
        SpriteFont mainfont;        // Main font for drawing in-game text

        Boolean gameover = false;   // Is the game over TRUE or FALSE?      
        float gameruntime = 0;      // Time since game started
        int score = 0;

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

        // Main 3D Game Camera
        camera gamecamera;

        const int numberofgrounds = 5;
        staticmesh[] ground = new staticmesh[numberofgrounds];  // 3D graphic for the ground in-game

        const int numberofwalls = 5;
        staticmesh[] leftwall = new staticmesh[numberofwalls];  // 3D graphic for walls
        staticmesh[] rightwall = new staticmesh[numberofwalls];  // 3D graphic for walls

        model3d robot;     // Robot model for user control


        // Create an array of trees
        const int numberoftrees = 30;
        staticmesh[] tree = new staticmesh[numberoftrees];

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Set the screen resolution
            this.graphics.PreferredBackBufferWidth = displaywidth;
            this.graphics.PreferredBackBufferHeight = displayheight;

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

            gamecamera = new camera(new Vector3(0, 0, 0), new Vector3(0, 0, 0), displaywidth, displayheight, 45, Vector3.Up, 1000, 20000);

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
            mainfont = Content.Load<SpriteFont>("quartz4"); // Load font

            background = new graphic2d(Content, "Background for Menus", displaywidth, displayheight);
            mousepointer1 = new sprite2d(Content, "X-Games-Cursor", 0, 0, 0.15f, Color.White, true);
            mousepointer2 = new sprite2d(Content, "X-Games-Cursor-Highlight", 0, 0, 0.15f, Color.White, true);

            menuoptions[0, 0] = new sprite2d(Content, "Start-Normal", displaywidth / 2, 200, 1, Color.White, true);
            menuoptions[0, 1] = new sprite2d(Content, "Start-Selected", displaywidth / 2, 200, 1, Color.White, true);
            menuoptions[1, 0] = new sprite2d(Content, "Options-Normal", displaywidth / 2, 300, 1, Color.White, true);
            menuoptions[1, 1] = new sprite2d(Content, "Options-Selected", displaywidth / 2, 300, 1, Color.White, true);
            menuoptions[2, 0] = new sprite2d(Content, "High-Score-Normal", displaywidth / 2, 400, 1, Color.White, true);
            menuoptions[2, 1] = new sprite2d(Content, "High-Score-Selected", displaywidth / 2, 400, 1, Color.White, true);
            menuoptions[3, 0] = new sprite2d(Content, "Exit-Normal", displaywidth / 2, 500, 1, Color.White, true);
            menuoptions[3, 1] = new sprite2d(Content, "Exit-Selected", displaywidth / 2, 500, 1, Color.White, true);
            for (int i = 0; i < numberofoptions; i++)
            {
                menuoptions[i, 0].updateobject();
            }

            // Load in high scores
            if (File.Exists(@"highscore.txt")) // This checks to see if the file exists
            {
                StreamReader sr = new StreamReader(@"highscore.txt");	// Open the file

                String line;		// Create a string variable to read each line into
                for (int i = 0; i < numberofhighscores && !sr.EndOfStream; i++)
                {
                    line = sr.ReadLine();	// Read the first line in the text file
                    highscores[i] = Convert.ToInt32(line);	// This converts line to numeric
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
            pad[2] = GamePad.GetState(PlayerIndex.Three);   // Reads gamepad 1
            pad[3] = GamePad.GetState(PlayerIndex.Four);    // Reads gamepad 2
            keys = Keyboard.GetState();                     // Read keyboard
            mouse = Mouse.GetState();                       // Read Mouse

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

                if (gamestate == 0)
                    reset();
            }

        }


        void reset()
        {
            gameover = false;
            gameruntime = 0;
            score = 0;

            // Initialise robot1 object
            robot = new model3d(Content, "r2d2v2", 2f, new Vector3(0, 0, 0), new Vector3(0, 0, 0), 0.002f, 0.06f, 10);

            // Load the 3D models for the static objects in the game from the ContentManager
            for (int i = 0; i < numberofgrounds; i++)
            {
                ground[i] = new staticmesh(Content, "long_ground", 10f, new Vector3(0, -40, 0), new Vector3(0, 0, 0));
                if (i > 0)
                    ground[i].position.Z += ground[i - 1].position.Z + 800 * ground[i - 1].size;
            }

            for (int i = 0; i < numberofwalls; i++)
            {
                leftwall[i] = new staticmesh(Content, "wall", 10f, new Vector3(-1000, -40, 0), new Vector3(0, 0, 0));
                rightwall[i] = new staticmesh(Content, "wall", 10f, new Vector3(1000, -40, 0), new Vector3(0, 0, 0));
                if (i > 0)
                {
                    leftwall[i].position.Z += leftwall[i - 1].position.Z + 800 * leftwall[i - 1].size;
                    rightwall[i].position.Z = leftwall[i].position.Z;
                }
            }

            for (int i = 0; i < numberoftrees; i++)
            {
                if (i % 2 == 0)
                    tree[i] = new staticmesh(Content, "tree", (float)(randomiser.Next(20) + 1) / 10,
                                    new Vector3(randomiser.Next(1600) - 800, 0, randomiser.Next(40000) + 1000),
                                        new Vector3(0, randomiser.Next(7), 0));
                else
                {
                    tree[i] = new staticmesh(Content, "lamppost", (float)(randomiser.Next(20) + 1) / 3,
                                    new Vector3(randomiser.Next(1600) - 800, 0, randomiser.Next(40000) + 1000),
                                        new Vector3(0, randomiser.Next(7), 0));
                    tree[i].position.Y = tree[i].size * 40;
                }
            }

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
                score = (int)(gameruntime / 1000);

                // Game is being played
                float turnamount = MathHelper.ToRadians(0);
                Vector2 dirtomove = new Vector2(0, 0);
                dirtomove.X = -pad[0].ThumbSticks.Left.X;
                dirtomove.Y = pad[0].ThumbSticks.Left.Y;

                // Move Robot based on touch control input
                robot.moveme(dirtomove, turnamount, gtime, 70);

                // Check for collisions between your vehicle and trees
//                for (int i = 0; i < numberoftrees; i++)
//                {
//                    if (uservehicle.bsphere.Intersects(tree[i].bsphere))
//                        gameover = true;
//                }

                // Allow robot to jump
                if (pad[0].Buttons.RightShoulder == ButtonState.Pressed) robot.jump(50);

                // Set a minimum speed for the robot
                if (robot.velocity.Z < 10) robot.velocity.Z = 10;

                // Set limits for the robots movements
                int wall_limits = 822;
                if (robot.position.X < -wall_limits) robot.position.X = -wall_limits;
                if (robot.position.X > wall_limits) robot.position.X = wall_limits;

                // Move ground & wall panels forward once you pass them
                for (int i = 0; i < numberofgrounds; i++)
                {
                    if (robot.position.Z > ground[i].position.Z + (500 * ground[i].size))
                    {
                        ground[i].position.Z += (800 * ground[i].size * (numberofgrounds - 1));
                        leftwall[i].position.Z = ground[i].position.Z;
                        rightwall[i].position.Z = ground[i].position.Z;
                    }
                }

                // Set the camera to first person
                //gamecamera.setFPor3P(robot.position, robot.direction, new Vector3(0, 0, 0), -60, 300, 60, 45);
                // Set side on camera view
                //gamecamera.setsideon(robot.position, robot.rotation, 1000, 50);
                // Set overhead camera view
                //gamecamera.setoverhead(robot.position, 1000);

                // Set the camera to third person
                gamecamera.setFPor3P(robot.position, robot.direction, robot.velocity, 300, 100, 100, 60);

                if (keys.IsKeyDown(Keys.Escape) || pad[0].Buttons.Back == ButtonState.Pressed)
                    gameover = true;    // End Game
            }
            else
            {
                // Game is over
                // Allow game to return to the main menu
                if (keys.IsKeyDown(Keys.Escape) || pad[0].Buttons.B == ButtonState.Pressed)
                {
                    if (score > highscores[9])
                        highscores[9] = score;

                    // SORT HIGHSCORE TABLE
                    Array.Sort(highscores);
                    Array.Reverse(highscores);


                    gamestate = -1;
                }

            }
        }


        public void drawgame()
        {
            // Draw the in-game graphics
            sfunctions3d.resetgraphics(GraphicsDevice);

            for (int i = 0; i < numberoftrees; i++)
                tree[i].drawme(gamecamera, true);

            // Draw the ground & walls
            for (int i = 0; i < numberofgrounds; i++)
                ground[i].drawme(gamecamera, false);
            for (int i = 0; i < numberofwalls; i++)
            {
                leftwall[i].drawme(gamecamera, false);
                rightwall[i].drawme(gamecamera, false);
            }
            // Draw the robot
            robot.drawme(gamecamera, true);

            // Draw the trees
            for (int i = 0; i < numberoftrees; i++)
                tree[i].drawme(gamecamera, true);

            spriteBatch.Begin();
            spriteBatch.DrawString(mainfont, "Score " + score.ToString("0"), new Vector2(0, 0), Color.White, MathHelper.ToRadians(0), new Vector2(0, 0), 2f, SpriteEffects.None, 0);

            if (gameover)
            {
                spriteBatch.DrawString(mainfont, "GAME OVER", new Vector2(130, 100),
                Color.White, MathHelper.ToRadians(0), new Vector2(0, 0), 3f, SpriteEffects.None, 0);
            }
            spriteBatch.End();
        }



        public void updateoptions()
        {
            // Update code for the options screen

            // Allow game to return to the main menu
            if (keys.IsKeyDown(Keys.Escape) || pad[0].Buttons.B == ButtonState.Pressed)
                gamestate = -1;
        }

        public void drawoptions()
        {
            // Draw graphics for OPTIONS screen
            spriteBatch.Begin();
            // Draw mouse
            mousepointer1.drawme(ref spriteBatch);
            spriteBatch.End();
        }

        public void updatehighscore()
        {
            // Update code for the high score screen

            // Allow game to return to the main menu
            if (keys.IsKeyDown(Keys.Escape) || pad[0].Buttons.B == ButtonState.Pressed)
                gamestate = -1;

        }

        public void drawhighscore()
        {
            // Draw graphics for High Score table
            spriteBatch.Begin();
            // Draw top ten high scores
            for (int i = 0; i < numberofhighscores; i++)
            {

                spriteBatch.DrawString(mainfont, (i + 1).ToString("0") + ". "+highscores[i].ToString("0"), new Vector2(displaywidth/2 - 180, 100 + (i * 30)),
                    Color.White, MathHelper.ToRadians(0), new Vector2(0, 0), 1f, SpriteEffects.None, 0);
            }

            // Draw mouse
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
