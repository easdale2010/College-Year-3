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

        const int numberofleftwalls = 18;
        const int numberofrightwalls = 31;
        staticmesh[] leftwall = new staticmesh[numberofleftwalls];  // 3D graphic for walls
        staticmesh[] rightwall = new staticmesh[numberofrightwalls];  // 3D graphic for walls
        const int numberofcars = 2;
        staticmesh[] car = new staticmesh[numberofcars]; // 3d graphics for cars 
        const int numberofbarrels = 5;
        staticmesh[] barrel = new staticmesh[numberofbarrels];
        const int numberofbtrees = 6;
        staticmesh[] tree1 = new staticmesh[numberofbtrees];
        const int numberofbtrees1 = 5;
        staticmesh[] tree2 = new staticmesh[numberofbtrees1];
        const int numberofhorses = 2;
        staticmesh[] horse = new staticmesh[numberofhorses];
        const int numberofboxes = 10;
        staticmesh[] mark = new staticmesh[numberofboxes];

    

        const int numberofbullets = 5;
        model3d[] bullet = new model3d[numberofbullets];

        model3d spray;
        Boolean sprayvisible = false;
        float sprayvisibletime = 0;
        // Main 3D Game Camera
        camera gamecamera;

        const int numberofground = 25;
        staticmesh[] ground = new staticmesh[numberofground];  // 3D graphic for the ground in-game

        const int numberofrobots = 2;
        model3d[] robot = new model3d[numberofrobots];     // Robot model for user control
        const int numberofenemys = 43;
        model3d[] enemy = new model3d[numberofenemys];
        model3d[] ibsphere = new model3d[numberofenemys];// array of invisible bsphere used for enemy attacks based on hearing.

        const int numberofhouses = 13;
        staticmesh[] house1 = new staticmesh[numberofhouses];
        const int numberofhouses2 = 11;
        staticmesh[] house2 = new staticmesh[numberofhouses2];
        const int numberofhouses3 = 6;

        staticmesh[] house3 = new staticmesh[numberofhouses2];
        staticmesh[] house4 = new staticmesh[numberofhouses];
        staticmesh[] house5 = new staticmesh[numberofhouses];
        staticmesh[] house6 = new staticmesh[numberofhouses];
        staticmesh[] house7 = new staticmesh[numberofhouses];
        staticmesh[] house8 = new staticmesh[numberofhouses3];
        staticmesh[] house9 = new staticmesh[numberofhouses3];
        staticmesh[] house10 = new staticmesh[numberofhouses3];

        const int numberofbodyparts = 6;
        staticmesh[] bodyparts = new staticmesh[numberofbodyparts];

      


        const int numberofshops = 2;
        staticmesh[] shop = new staticmesh[numberofshops];
        staticmesh[] shop2 = new staticmesh[numberofshops];
        staticmesh[] shop3 = new staticmesh[numberofshops];
        staticmesh[] church = new staticmesh[1];
        // Create an array of trees
        const int numberoftrees = 14;
        staticmesh[] lamppost = new staticmesh[numberoftrees];

        SoundEffect moveplayer;
        SoundEffectInstance playermove;
        SoundEffect jump;
        SoundEffect track;
        SoundEffectInstance music;
        SoundEffect moan;
        SoundEffectInstance moaned;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Set the screen resolution
            this.graphics.PreferredBackBufferWidth = displaywidth;
            this.graphics.PreferredBackBufferHeight = displayheight;

        }

          // -------------FUNTION TO WORKOUT THE ANGLE OF ENEMY TO FACE------
        float workoutangletoface(Vector3 goodguy, Vector3 badguy)
        {
            Vector3 difference = goodguy - badguy;
            float angletoface = (float)Math.Atan2(difference.X, difference.Z);
            if (angletoface < 0) angletoface += MathHelper.ToRadians(360);

            return angletoface;
        }


        //---------- FUNCTION TO WORK OUT DIRECTION TO TURN--------------------------------------
        float directiontoturn(float currentangle, float desiredangle, float minangleb4turn)
        {
            float direction = 0;
            if (Math.Abs(desiredangle - currentangle) > minangleb4turn)
            {
                if (desiredangle > currentangle)
                {
                    if (MathHelper.ToDegrees(desiredangle) - MathHelper.ToDegrees(currentangle) <= 180)
                        direction = 1;
                    else
                        direction = -1;
                }
                else
                {
                    if (MathHelper.ToDegrees(desiredangle) - MathHelper.ToDegrees(currentangle) <= 180)
                        direction = -1;
                    else
                        direction = 1;
                }
            }
            return direction;
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

            gamecamera = new camera(new Vector3(0, 0, 0), new Vector3(0,0,0), displaywidth, displayheight, 45, Vector3.Up, 1000, 20000); // cameras posiiton on the screen

            base.Initialize();
            ShapeRenderingSample.DebugShapeRenderer.Initialize(graphics.GraphicsDevice);

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

            background = new graphic2d(Content, "Background for Menus", displaywidth, displayheight);// loads background image
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

        // Reset values for the start of a new game
        void reset()
        {
            gameover = false;

            // Load the 3D models for the static objects in the game from the ContentManager
            //ground[0] = new staticmesh(Content, "sground", 1000f, new Vector3(0, -40, 100), new Vector3(0, 0, 0));
            //ground[1] = new staticmesh(Content, "sground", 100f, new Vector3(-10000, -40, 0), new Vector3(0, 0, 0));
            //ground[2] = new staticmesh(Content, "sground", 100f, new Vector3(10000, -40, 0), new Vector3(0, 0, 0));
            //ground[3] = new staticmesh(Content, "sground", 100f, new Vector3(-5000, -40, 0), new Vector3(0, 0, 0));
            //ground[4] = new staticmesh(Content, "sground", 100f, new Vector3(5000, -40, 0), new Vector3(0, 0, 0));
            //ground[5] = new staticmesh(Content, "sground", 100f, new Vector3(5000, -40, -10000), new Vector3(0, 0, 0));
            //ground[6] = new staticmesh(Content, "sground", 100f, new Vector3(-5000, -40, 10000), new Vector3(0, 0, 0));
            //ground[7] = new staticmesh(Content, "sground", 100f, new Vector3(-300000, -40, -300000), new Vector3(0, 0, 0));
            //ground[8] = new staticmesh(Content, "sground", 100f, new Vector3(300000, -40, 300000), new Vector3(0, 0, 0));
            //ground[9] = new staticmesh(Content, "sground", 100f, new Vector3(300, -40, 0), new Vector3(0, 0, 0));
            //ground[10] = new staticmesh(Content, "sground", 100f, new Vector3(-300, -40, 0), new Vector3(0, 0, 0));
            //ground[11] = new staticmesh(Content, "sground", 100f, new Vector3(0, -40, 300), new Vector3(0, 0, 0));
            //ground[12] = new staticmesh(Content, "sground", 100f, new Vector3(0, -40, -300), new Vector3(0, 0, 0));

            for (int i = 0; i < numberofground; i++)
                ground[i] = new staticmesh(Content, "tgrounds", 1000f, new Vector3(0, 0, 0), new Vector3(0, 0, 0), true);

            // Initialise robot1 object
            robot[0] = new model3d(Content, "r2d2v2", 4f, new Vector3(-31010, 460, 19300), new Vector3(0, MathHelper.ToRadians(75), 0), 0.002f, 0.05f, 10, true);//-36220
            robot[1] = new model3d(Content, "r2d2v2", 7f, new Vector3(-25010, 660, -52820), new Vector3(0, MathHelper.ToRadians(75), 0), 0.002f, 0.05f, 10, true);//-59000

            spray = new model3d(Content, "spray", 8f, new Vector3(0, 0, 0), new Vector3(0, 0, 0), 0, 0, 0, false);
            for (int i = 0; i < numberofbullets; i++)
            {
                bullet[i] = new model3d(Content, "ballbullet", 40f, new Vector3(robot[0].position.X, robot[0].position.Y + 100, robot[0].position.Z), new Vector3(0, 0, 0), 0.002f, 0.05f, 10, false);
            }


            barrel[0] = new staticmesh(Content, "barrel", 300, new Vector3(-30000, 545, -49000), new Vector3(0, MathHelper.ToRadians(90), 0), true);
            barrel[1] = new staticmesh(Content, "barrel", 300, new Vector3(-23000, 545, -55000), new Vector3(0, MathHelper.ToRadians(270), 0), true);
            barrel[2] = new staticmesh(Content, "barrel", 300, new Vector3(-12200, 950, -48000), new Vector3(0, MathHelper.ToRadians(110), 0), true);
            barrel[3] = new staticmesh(Content, "barrel", 300, new Vector3(11000, 545, -49000), new Vector3(0, MathHelper.ToRadians(90), 0), true);
            barrel[4] = new staticmesh(Content, "barrel", 300, new Vector3(31000, 745, -58000), new Vector3(0, MathHelper.ToRadians(90), 0), true);

            tree1[0] = new staticmesh(Content, "broadleavedtreeB_mesh", 100, new Vector3(-6000, 1170, -47500), new Vector3(0, MathHelper.ToRadians(90), 0), true);
            tree1[1] = new staticmesh(Content, "broadleavedtreeB_mesh", 400, new Vector3(5500, 1870, -46900), new Vector3(0, MathHelper.ToRadians(90), 0), true);
            tree1[2] = new staticmesh(Content, "broadleavedtreeB_mesh", 600, new Vector3(-26500, 1170, -23000), new Vector3(0, MathHelper.ToRadians(90), 0), true);
            tree1[3] = new staticmesh(Content, "broadleavedtreeB_mesh", 800, new Vector3(30050, 1170, -17000), new Vector3(0, MathHelper.ToRadians(90), 0), true);
            tree1[4] = new staticmesh(Content, "broadleavedtreeB_mesh", 800, new Vector3(10000, 1170, -19000), new Vector3(0, MathHelper.ToRadians(90), 0), true);
            tree1[5] = new staticmesh(Content, "broadleavedtreeB_mesh", 1000, new Vector3(-5000, 1170, -18500), new Vector3(0, MathHelper.ToRadians(90), 0), true);

            horse[0] = new staticmesh(Content, "horsebrownhair", 150, new Vector3(-30900, 1000, -6500), new Vector3(0, MathHelper.ToRadians(270), 0), true);
            horse[1] = new staticmesh(Content, "horsegreyhair", 150, new Vector3(-29100, 1000, -8000), new Vector3(0, 0, 0), true);


            mark[0] = new staticmesh(Content, "simplebox", 1000, new Vector3(-20000, 500, -8000), new Vector3(0, 0, 0), true);
            mark[1] = new staticmesh(Content, "simplebox", 1000, new Vector3(33010, 500, -55320), new Vector3(0, 0, 0), true);
            mark[2] = new staticmesh(Content, "simplebox", 1000, new Vector3(-28520, 860, -40000), new Vector3(0, 0, MathHelper.ToRadians(45)), true);
            mark[3] = new staticmesh(Content, "simplebox", 1000, new Vector3(-27665, 500, -40000), new Vector3(0, 0, 0), true);
            mark[4] = new staticmesh(Content, "simplebox", 1000, new Vector3(-5000, 500, -40000), new Vector3(0, 0, 0), true);
            mark[5] = new staticmesh(Content, "simplebox", 1000, new Vector3(20000, 500, -7600), new Vector3(0, 0, 0), true);
            mark[6] = new staticmesh(Content, "simplebox", 1000, new Vector3(600, 500, -7600), new Vector3(0, 0, 0), true);
            mark[7] = new staticmesh(Content, "simplebox", 1000, new Vector3(-30000, 500, 4000), new Vector3(0, 0, 0), true);
            mark[8] = new staticmesh(Content, "simplebox", 1000, new Vector3(0, 500, 4000), new Vector3(0, 0, 0), true);
            mark[9] = new staticmesh(Content, "simplebox", 1000, new Vector3(20000, 500, 4000), new Vector3(0, 0, 0), true);

            for (int i = 0; i < numberofbtrees1; i++)
            {
                tree2[0] = new staticmesh(Content, "fir_mesh", 600, new Vector3(29000, 1170, -8500), new Vector3(0, MathHelper.ToRadians(90), 0), true);
                tree2[i] = new staticmesh(Content, "fir_mesh", 600f, new Vector3(tree2[0].position.X - (i * 3010 * 5f), 1170, -8500), new Vector3(0, 0, 0), true);
            }
            //tree2[1] = new staticmesh(Content, "fir_mesh", 400, new Vector3(5500, 1870, -46900), new Vector3(0, MathHelper.ToRadians(90), 0));
            //tree2[2] = new staticmesh(Content, "fir_mesh", 600, new Vector3(-26500, 1170, -23000), new Vector3(0, MathHelper.ToRadians(90), 0));
            //tree2[3] = new staticmesh(Content, "fir_mesh", 800, new Vector3(30050, 1170, -17000), new Vector3(0, MathHelper.ToRadians(90), 0));
            //tree2[4] = new staticmesh(Content, "fir_mesh", 800, new Vector3(10000, 1170, -19000), new Vector3(0, MathHelper.ToRadians(90), 0));
            //tree2[5] = new staticmesh(Content, "fir_mesh", 1000, new Vector3(-5000, 1170, -18500), new Vector3(0, MathHelper.ToRadians(90), 0));


            resetstart();// calls restart

            car[0] = new staticmesh(Content, "Ferrari2", 400, new Vector3(-500, 475, -49500), new Vector3(0, MathHelper.ToRadians(90), 0), true);
            car[1] = new staticmesh(Content, "Lamborghini2", 400, new Vector3(-7100, 820, -49500), new Vector3(0, MathHelper.ToRadians(90), 0), true);

            for (int i = 0; i < numberofhouses; i++)
            {
                house1[0] = new staticmesh(Content, "houseA", 1000f, new Vector3(30010, 460, -57320), new Vector3(0, 0, 0), true);
                house1[i] = new staticmesh(Content, "houseA", 1000f, new Vector3(house1[0].position.X - (i * 1010 * 5f), 460, -57320), new Vector3(0, 0, 0), true);
            }
            for (int i = 0; i < numberofhouses2; i++)
            {
                house2[0] = new staticmesh(Content, "houseB_mesh", 1000f, new Vector3(30010, 460, -47320), new Vector3(0, MathHelper.ToRadians(180), 0), true);
                house2[i] = new staticmesh(Content, "houseB_mesh", 1000f, new Vector3(house2[0].position.X - (i * 1010 * 5.9f), 460, -47320), new Vector3(0, MathHelper.ToRadians(180), 0), true);
            }
            for (int i = 0; i < numberofhouses2; i++)
            {
                house3[0] = new staticmesh(Content, "houseB_mesh", 1000f, new Vector3(30010, 460, -41960), new Vector3(0, 0, 0), true);
                house3[i] = new staticmesh(Content, "houseB_mesh", 1000f, new Vector3(house3[0].position.X - (i * 1010 * 5.9f), 460, -41960), new Vector3(0, MathHelper.ToRadians(0), 0), true);
            }
            for (int i = 0; i < numberofhouses; i++)
            {
                house4[0] = new staticmesh(Content, "houseC_mesh", 1000f, new Vector3(30010, 490, -32320), new Vector3(0, MathHelper.ToRadians(180), 0), true);
                house4[i] = new staticmesh(Content, "houseC_mesh", 1000f, new Vector3(house3[0].position.X - (i * 1010 * 5f), ground[0].position.Y + 490, -32320), new Vector3(0, MathHelper.ToRadians(180), 0), true);
            }
            for (int i = 0; i < numberofhouses; i++)
            {
                house5[0] = new staticmesh(Content, "houseC_mesh", 1000f, new Vector3(30010, ground[0].position.Y + 525, -13040), new Vector3(0, MathHelper.ToRadians(180), 0), true);
                house5[i] = new staticmesh(Content, "houseC_mesh", 1000f, new Vector3(house3[0].position.X - (i * 1010 * 5f), ground[0].position.Y + 525, -13040), new Vector3(0, MathHelper.ToRadians(180), 0), true);
            }

            for (int i = 0; i < numberofhouses; i++)
            {
                house6[0] = new staticmesh(Content, "houseD_mesh", 1000f, new Vector3(30010, 560, 10040), new Vector3(0, MathHelper.ToRadians(180), 0), true);
                house6[i] = new staticmesh(Content, "houseD_mesh", 1000f, new Vector3(house3[0].position.X - (i * 1010 * 5f), 560, 10040), new Vector3(0, MathHelper.ToRadians(180), 0), true);
            }
            for (int i = 0; i < numberofhouses; i++)
            {
                house7[0] = new staticmesh(Content, "houseD_mesh", 1000f, new Vector3(30010, 560, 15310), new Vector3(0, 0, 0), true);
                house7[i] = new staticmesh(Content, "houseD_mesh", 1000f, new Vector3(house3[0].position.X - (i * 1010 * 5f), 560, 15310), new Vector3(0, MathHelper.ToRadians(180), 0), true);
            }
            for (int i = 0; i < numberofhouses3; i++)
            {
                house8[0] = new staticmesh(Content, "houseF_mesh", 1000f, new Vector3(30010, 560, 30750), new Vector3(0, 0, 0), true);
                house8[i] = new staticmesh(Content, "houseF_mesh", 1000f, new Vector3(house3[0].position.X - (i * 2110 * 5f), 560, 30750), new Vector3(0, MathHelper.ToRadians(180), 0), true);
            }
            for (int i = 0; i < numberofhouses3; i++)
            {
                house9[0] = new staticmesh(Content, "houseE_mesh", 270f, new Vector3(30010, ground[0].position.Y + 725, 24750), new Vector3(0, 0, 0), true);
                house9[i] = new staticmesh(Content, "houseE_mesh", 270f, new Vector3(house3[0].position.X - (i * 1990 * 5f), 760, 24750), new Vector3(0, MathHelper.ToRadians(180), 0), true);
            }
            for (int i = 0; i < numberofhouses3; i++)
            {
                house10[0] = new staticmesh(Content, "houseE_mesh", 1000f, new Vector3(30010, ground[0].position.Y + 725, 30750), new Vector3(0, 0, 0), true);
                house10[i] = new staticmesh(Content, "houseE_mesh", 1000f, new Vector3(house3[0].position.X - (i * 2110 * 5f), 760, 30750), new Vector3(0, MathHelper.ToRadians(180), 0), true);
            }



            for (int i = 0; i < numberofshops; i++)
                shop[0] = new staticmesh(Content, "cornershop_mesh", 1000f, new Vector3(30850, 4560, -22222), new Vector3(0, 0, 0), true);

            shop[1] = new staticmesh(Content, "cornershop_mesh", 1000f, new Vector3(-29840, 4560, -22022), new Vector3(0, MathHelper.ToRadians(180), 0), true);

            church[0] = new staticmesh(Content, "church_mesh", 1000f, new Vector3(-26750, 560, 0), new Vector3(0, MathHelper.ToRadians(90), 0), true);
            //       church[1] = new staticmesh(Content, "church_mesh", 1000f, new Vector3(-216750, 360, 0), new Vector3(0, MathHelper.ToRadians(90), 0));


            shop2[0] = new staticmesh(Content, "shop_mesh", 985f, new Vector3(14860, 350, -26400), new Vector3(0, 0, 0), true);
            shop2[1] = new staticmesh(Content, "shop_mesh", 985f, new Vector3(-10390, 560, -26400), new Vector3(0, 0, 0), true);
            shop3[0] = new staticmesh(Content, "shop_mesh", 650f, new Vector3(4685, 560, -27500), new Vector3(0, 0, 0), true);
            shop3[1] = new staticmesh(Content, "shop_mesh", 650, new Vector3(-290, 560, -27500), new Vector3(0, 0, 0), true);

            for (int i = 0; i < 14; i++)// creats and positions lampost
            {

                {
                    lamppost[0] = new staticmesh(Content, "lamppost", 30,
                                  new Vector3(31710, 1860, -54020),
                                      new Vector3(0, MathHelper.ToRadians(270), 0), true);

                    lamppost[i] = new staticmesh(Content, "lamppost", 30,
                                    new Vector3(lamppost[0].position.X - (i * 1010 * 5f), 1860, -54020),
                                        new Vector3(0, MathHelper.ToRadians(270), 0), true);


                    //tree[i].position.Y = tree[i].size * 40;
                }
            }



            //for (int i = 13; i < numberoftrees; i++)
            //{
            //    lamppost[i] = new staticmesh(Content, "lamppost", 30f, new Vector3(lamppost[0].position.X - (i * 1010 * 5f), 460, -54020), new Vector3(0, 0, 0));
            //    lamppost[i].position.Y = lamppost[i].size * 40;
            //}


            for (int i = 0; i < 9; i++)
            {
                leftwall[i] = new staticmesh(Content, "wall", 10f, new Vector3(-30000, 460, -70000), new Vector3(0, 0, 0), true);
                leftwall[i] = new staticmesh(Content, "wall", 10f, new Vector3(leftwall[0].position.X + i * 8000, 460, -60000), new Vector3(0, 0, 0), true);
                leftwall[i].rotation = new Vector3(0, MathHelper.ToRadians(90), 0);
            }

            for (int i = 9; i < numberofleftwalls; i++)
            {
                leftwall[9] = new staticmesh(Content, "wall", 10f, new Vector3(-30000, 460, 40000), new Vector3(0, MathHelper.ToRadians(90), 0), true);
                leftwall[10] = new staticmesh(Content, "wall", 10f, new Vector3(-22000, 460, 40000), new Vector3(0, MathHelper.ToRadians(90), 0), true);
                leftwall[11] = new staticmesh(Content, "wall", 10f, new Vector3(-14000, 460, 40000), new Vector3(0, MathHelper.ToRadians(90), 0), true);
                leftwall[12] = new staticmesh(Content, "wall", 10f, new Vector3(-6000, 460, 40000), new Vector3(0, MathHelper.ToRadians(90), 0), true);
                leftwall[13] = new staticmesh(Content, "wall", 10f, new Vector3(2000, 460, 40000), new Vector3(0, MathHelper.ToRadians(90), 0), true);
                leftwall[14] = new staticmesh(Content, "wall", 10f, new Vector3(10000, 460, 40000), new Vector3(0, MathHelper.ToRadians(90), 0), true);
                leftwall[15] = new staticmesh(Content, "wall", 10f, new Vector3(18000, 460, 40000), new Vector3(0, MathHelper.ToRadians(90), 0), true);
                leftwall[16] = new staticmesh(Content, "wall", 10f, new Vector3(26000, 460, 40000), new Vector3(0, MathHelper.ToRadians(90), 0), true);
                leftwall[17] = new staticmesh(Content, "wall", 10f, new Vector3(32000, 460, 40000), new Vector3(0, MathHelper.ToRadians(90), 0), true);
                leftwall[i].rotation = new Vector3(0, MathHelper.ToRadians(90), 0);
            }



            for (int i = 0; i < 15; i++)
            {
                rightwall[0] = new staticmesh(Content, "wall", 10f, new Vector3(-34000, 460, -56000), new Vector3(0, 0, 0), true);

                rightwall[i] = new staticmesh(Content, "wall", 10f, new Vector3(-34000, 460, rightwall[0].position.Z + i * 8000), new Vector3(0, 0, 0), true);
            }


            rightwall[15] = new staticmesh(Content, "wall", 10f, new Vector3(35000, 460, -56000), new Vector3(0, 0, 0), true);
            rightwall[16] = new staticmesh(Content, "wall", 10f, new Vector3(35000, 460, -48000), new Vector3(0, 0, 0), true);
            rightwall[17] = new staticmesh(Content, "wall", 10f, new Vector3(35000, 460, -40000), new Vector3(0, 0, 0), true);
            rightwall[18] = new staticmesh(Content, "wall", 10f, new Vector3(35000, 460, -32000), new Vector3(0, 0, 0), true);
            rightwall[19] = new staticmesh(Content, "wall", 10f, new Vector3(35000, 460, -24000), new Vector3(0, 0, 0), true);
            rightwall[20] = new staticmesh(Content, "wall", 10f, new Vector3(35000, 460, -16000), new Vector3(0, 0, 0), true);
            rightwall[21] = new staticmesh(Content, "wall", 10f, new Vector3(35000, 460, -8000), new Vector3(0, 0, 0), true);
            rightwall[22] = new staticmesh(Content, "wall", 10f, new Vector3(35000, 460, 0), new Vector3(0, 0, 0), true);
            rightwall[23] = new staticmesh(Content, "wall", 10f, new Vector3(35000, 460, 8000), new Vector3(0, 0, 0), true);
            rightwall[24] = new staticmesh(Content, "wall", 10f, new Vector3(35000, 460, 16000), new Vector3(0, 0, 0), true);
            rightwall[25] = new staticmesh(Content, "wall", 10f, new Vector3(35000, 460, 24000), new Vector3(0, 0, 0), true);
            rightwall[26] = new staticmesh(Content, "wall", 10f, new Vector3(35000, 460, 32000), new Vector3(0, 0, 0), true);
            rightwall[27] = new staticmesh(Content, "wall", 10f, new Vector3(35000, 460, 40000), new Vector3(0, 0, 0), true);
            rightwall[28] = new staticmesh(Content, "wall", 10f, new Vector3(35000, 460, 48000), new Vector3(0, 0, 0), true);
            rightwall[29] = new staticmesh(Content, "wall", 10f, new Vector3(35000, 460, 56000), new Vector3(0, 0, 0), true);
            rightwall[30] = new staticmesh(Content, "wall", 10f, new Vector3(35000, 460, 56000), new Vector3(0, 0, 0), true);



            for (int count = 0; count < leftwall.Length; count++)// creatrs a bounding box around the object
            {
                leftwall[count].bboxsize = new Vector3(4000, 1000, 100);
                leftwall[count].updateobject();
            }
            for (int count = 0; count < rightwall.Length; count++)
            {
                rightwall[count].bboxsize = new Vector3(100, 1000, 4000);
                rightwall[count].updateobject();
            }
            for (int count = 0; count < house1.Length; count++)
            {
                house1[count].bboxsize = new Vector3(2200, 6000, 2570);
                house1[count].updateobject();
            }
            for (int count = 0; count < house2.Length; count++)
            {
                house2[count].bboxsize = new Vector3(2700, 6000, 2700);
                house2[count].updateobject();
            }
            for (int count = 0; count < house3.Length; count++)
            {
                house3[count].bboxsize = new Vector3(2700, 6000, 2700);
                house3[count].updateobject();
            }
            for (int count = 0; count < house4.Length; count++)
            {
                house4[count].bboxsize = new Vector3(2090, 6000, 2650);
                house4[count].updateobject();
            }
            for (int count = 0; count < house5.Length; count++)
            {
                house5[count].bboxsize = new Vector3(2090, 6000, 2650);
                house5[count].updateobject();
            }
            for (int count = 0; count < house6.Length; count++)
            {
                house6[count].bboxsize = new Vector3(2090, 6000, 2650);
                house6[count].updateobject();
            }
            for (int count = 0; count < house7.Length; count++)
            {
                house7[count].bboxsize = new Vector3(2090, 6000, 2650);
                house7[count].updateobject();
            }
            for (int count = 0; count < house8.Length; count++)
            {
                house8[count].bboxsize = new Vector3(4080, 6000, 6000);
                house8[count].updateobject();
            }
            for (int count = 0; count < house9.Length; count++)
            {
                house9[count].bboxsize = new Vector3(1480, 4000, 160);
                house9[count].updateobject();
            }
            for (int count = 0; count < house8.Length; count++)
            {
                house10[count].bboxsize = new Vector3(480, 6000, 660);
                house10[count].updateobject();
            }

            for (int count = 0; count < shop.Length; count++)
            {
                shop[count].bboxsize = new Vector3(4090, 6000, 4050);
                shop[count].updateobject();
            }
            for (int count = 0; count < shop2.Length; count++)
            {
                shop2[count].bboxsize = new Vector3(2120, 6000, 3280);
                shop2[count].updateobject();
            }
            for (int count = 0; count < shop3.Length; count++)
            {
                shop3[count].bboxsize = new Vector3(1400, 6000, 2165);
                shop3[count].updateobject();
            }
            for (int count = 0; count < church.Length; count++)
            {
                church[count].bboxsize = new Vector3(6540, 6000, 3050);
                church[count].updateobject();
            }
            for (int count = 0; count < car.Length; count++)
            {
                car[count].bboxsize = new Vector3(6540, 6000, 3050);
                car[count].updateobject();
            }
            for (int count = 0; count < barrel.Length; count++)
            {
                barrel[count].bboxsize = new Vector3(6540, 6000, 3050);
                barrel[count].updateobject();
            }
            for (int count = 0; count < tree1.Length; count++)
            {
                tree1[count].bboxsize = new Vector3(100, 6000, 100);
                tree1[count].updateobject();
            }
            for (int count = 0; count < tree2.Length; count++)
            {
                tree2[count].bboxsize = new Vector3(100, 6000, 100);
                tree2[count].updateobject();
            }
            for (int count = 0; count < horse.Length; count++)
            {
                horse[count].bboxsize = new Vector3(130, 6000, 670);
                horse[count].updateobject();
            }
            for (int count = 0; count < mark.Length; count++)
            {
                mark[count].bboxsize = new Vector3(500, 1000, 500);
                mark[count].updateobject();
            }

            for (int count = 0; count < robot.Length; count++)
            {
                robot[count].bboxsize = new Vector3(100, 300, 100);
                robot[count].updateobject();
            }
            for (int count = 0; count < ibsphere.Length; count++)
            {
                ibsphere[count].bboxsize = new Vector3(400, 300, 400);
                ibsphere[count].updateobject();
            }

            moveplayer = Content.Load<SoundEffect>("LASER19");// loads player sound effect
            playermove = moveplayer.CreateInstance();// creats sound track of sound effect
            playermove.IsLooped = true;// repeats trck
            playermove.Volume = 0.2f;// set the volume to 0.2
            track = Content.Load<SoundEffect>("explosion");
            music = track.CreateInstance();
            music.IsLooped = true;
            music.Volume = 0.2f;
            jump = Content.Load<SoundEffect>("TALK");
            moan = Content.Load<SoundEffect>("zombiemaon");
            moaned = moan.CreateInstance();
            moaned.IsLooped = false;
            moaned.Volume = 0.5f;



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


        public void resetstart()// used for loading, placing and moving enemies
        {

            for (int i = 0; i < numberofenemys; i++)
            {
                enemy[i] = new model3d(Content, "r2d2v2", 8f, new Vector3(0, 0, 0), new Vector3(0, MathHelper.ToRadians(90), 0), 0.002f, 0.5f, 10, true);//-59000

            }

            enemy[0].position = new Vector3(-20000, 760, -52000);
            enemy[1].position = new Vector3(-20000, 760, -54000);
            enemy[2].position = new Vector3(-29000, 760, -53000);
            enemy[3].position = new Vector3(-29000, 760, -51000);
            enemy[2].rotation = new Vector3(0, MathHelper.ToRadians(90), 0);
            enemy[3].rotation = new Vector3(0, MathHelper.ToRadians(90), 0);
            enemy[4].position = new Vector3(-27010, 760, -36220);
            enemy[4].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[5].position = new Vector3(-7000, 760, -36320);
            enemy[5].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[6].position = new Vector3(8000, 760, -36420);
            enemy[6].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[7].position = new Vector3(3000, 760, -36520);
            enemy[7].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[8].position = new Vector3(0, 760, -36620);
            enemy[8].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[9].position = new Vector3(-20000, 760, -36120);
            enemy[9].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[10].position = new Vector3(-30850, 760, -27620);
            enemy[10].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[11].position = new Vector3(-30850, 760, -26720);
            enemy[11].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[12].position = new Vector3(-20850, 760, -27720);
            enemy[12].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[13].position = new Vector3(-20850, 760, -26720);
            enemy[13].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[14].position = new Vector3(-10850, 760, -27720);
            enemy[14].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[15].position = new Vector3(-10850, 760, -26720);
            enemy[15].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[16].position = new Vector3(-25700, 760, -17420);
            enemy[16].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[17].position = new Vector3(-15850, 760, -16800);
            enemy[17].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[18].position = new Vector3(0, 760, -17800);
            enemy[18].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[19].position = new Vector3(15000, 760, -17000);
            enemy[19].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[20].position = new Vector3(30000, 760, -26720);
            enemy[20].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[21].position = new Vector3(30000, 760, -28720);
            enemy[21].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[22].position = new Vector3(20000, 760, -26720);
            enemy[22].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[23].position = new Vector3(20000, 760, -28720);
            enemy[23].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[24].position = new Vector3(-5200, 760, -28720);
            enemy[24].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[25].position = new Vector3(-4400, 760, -7020);
            enemy[25].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[25].position = new Vector3(16400, 760, -5720);
            enemy[25].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[26].position = new Vector3(14400, 760, -3720);
            enemy[26].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[27].position = new Vector3(24400, 760, 1720);
            enemy[27].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);


            enemy[28].position = new Vector3(12000, 760, -3720);
            enemy[28].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[29].position = new Vector3(-14400, 760, 2520);
            enemy[29].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[30].position = new Vector3(24400, 760, 3000);
            enemy[30].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[31].position = new Vector3(34400, 760, 4000);
            enemy[31].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[32].position = new Vector3(-10000, 760, 6000);
            enemy[32].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);

            enemy[28].position = new Vector3(10000, 760, 5050);
            enemy[28].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[29].position = new Vector3(5400, 760, -120);
            enemy[29].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[30].position = new Vector3(400, 760, -2200);
            enemy[30].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[31].position = new Vector3(-4400, 760, -5000);
            enemy[31].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[32].position = new Vector3(-8000, 760, -700);
            enemy[32].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[33].position = new Vector3(-27000, 760, -6500);
            enemy[33].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[34].position = new Vector3(-30000, 760, -6300);
            enemy[34].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[35].position = new Vector3(-20000, 760, 20300);
            enemy[36].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[36].position = new Vector3(-30000, 760, 19300);
            enemy[36].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[37].position = new Vector3(0, 760, 21300);
            enemy[37].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[38].position = new Vector3(30000, 760, 22300);
            enemy[38].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[39].position = new Vector3(20000, 760, 23000);
            enemy[39].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[40].position = new Vector3(10000, 760, 24000);
            enemy[40].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[41].position = new Vector3(0, 760, 19000);
            enemy[41].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            enemy[42].position = new Vector3(-10000, 760, 22000);
            enemy[42].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);
            //enemy[36].size = 200;
            //30010, ground[0].position.Y + 725, 30750

            // moves enemys
            enemy[0].velocity = new Vector3(-10, 0, -2);
            enemy[1].velocity = new Vector3(-10, 0, 2);
            enemy[2].velocity = new Vector3(10, 0, 0);
            enemy[3].velocity = new Vector3(10, 0, -4);
            enemy[4].velocity = new Vector3(30, 0, 0);
            enemy[5].velocity = new Vector3(-30, 0, 0);
            enemy[6].velocity = new Vector3(-30, 0, 0);
            enemy[6].velocity = new Vector3(30, 0, 0);
            enemy[7].velocity = new Vector3(30, 0, 0);
            enemy[8].velocity = new Vector3(30, 0, 0);
            enemy[9].velocity = new Vector3(30, 0, 0);
            enemy[10].velocity = new Vector3(30, 0, 0);
            enemy[11].velocity = new Vector3(30, 0, 0);
            enemy[12].velocity = new Vector3(30, 0, 0);
            enemy[13].velocity = new Vector3(30, 0, 0);
            enemy[14].velocity = new Vector3(30, 0, 0);
            enemy[15].velocity = new Vector3(30, 0, 0);
            enemy[16].velocity = new Vector3(30, 0, 0);
            enemy[17].velocity = new Vector3(30, 0, 0);
            enemy[18].velocity = new Vector3(30, 0, 0);
            enemy[19].velocity = new Vector3(30, 0, 0);
            enemy[20].velocity = new Vector3(30, 0, 0);
            enemy[21].velocity = new Vector3(30, 0, 0);
            enemy[22].velocity = new Vector3(30, 0, 0);
            enemy[23].velocity = new Vector3(30, 0, 0);
            enemy[24].velocity = new Vector3(30, 0, 30);
            enemy[25].velocity = new Vector3(30, 0, 0);
            enemy[26].velocity = new Vector3(30, 0, 0);
            enemy[27].velocity = new Vector3(30, 0, 0);
            enemy[28].velocity = new Vector3(40, 0, 0);
            enemy[29].velocity = new Vector3(30, 0, 30);
            enemy[30].velocity = new Vector3(40, 0, 0);
            enemy[31].velocity = new Vector3(30, 0, 30);
            enemy[32].velocity = new Vector3(30, 0, 30);
            enemy[33].velocity = new Vector3(30, 0, -30);
            enemy[34].velocity = new Vector3(40, 0, -10);
            enemy[35].velocity = new Vector3(40, 0, 0);
            enemy[36].velocity = new Vector3(40, 0, 0);
            enemy[37].velocity = new Vector3(40, 0, 10);
            enemy[38].velocity = new Vector3(0, 0, 40);
            enemy[39].velocity = new Vector3(40, 0, 0);
            enemy[40].velocity = new Vector3(40, 0, 0);
            enemy[41].velocity = new Vector3(40, 0, 0);
            enemy[42].velocity = new Vector3(40, 0, 0);
            //enemy[43].velocity = new Vector3(40, 0, 0);


            for (int i = 0; i < numberofenemys; i++)// creats bounding shpere around the enemies
            {
                ibsphere[i] = new model3d(Content, "r2d2v2", 8f, new Vector3(0, 0, 0), new Vector3(0, MathHelper.ToRadians(90), 0), 0.002f, 0.05f, 10, false);
                ibsphere[i].position = enemy[i].position;
                ibsphere[i].velocity = enemy[i].velocity;
            }


            // used for semi cut sceen at start of game
            bodyparts[0] = new staticmesh(Content, "limb", 40f, new Vector3(-24710, 550, -52320), new Vector3(0, 0, MathHelper.ToRadians(61)), true);
            bodyparts[1] = new staticmesh(Content, "limb", 30f, new Vector3(-25110, 550, -52320), new Vector3(0, 0, MathHelper.ToRadians(61)), true);
            bodyparts[2] = new staticmesh(Content, "limb", 40f, new Vector3(-24710, 550, -53120), new Vector3(0, 0, MathHelper.ToRadians(61)), true);
            bodyparts[3] = new staticmesh(Content, "limb", 30f, new Vector3(-24110, 550, -53120), new Vector3(0, 0, MathHelper.ToRadians(61)), true);
            bodyparts[4] = new staticmesh(Content, "head", 20f, new Vector3(-25710, 550, -52720), new Vector3(0, 0, MathHelper.ToRadians(61)), true);
            bodyparts[5] = new staticmesh(Content, "thurso", 40f, new Vector3(-25010, 450, -52720), new Vector3(MathHelper.ToRadians(90), 0, MathHelper.ToRadians(90)), true);

            //enemystartingposition();
        }

        //public void enemystartingposition()
        //{
        //    for (int u = 0; u < numberofenemys; u++)
        //        if (robot[1].position.X - enemy[u].position.X > 8000)
        //            for (int i = 0; i < numberofenemys; i++)
        //                if (robot[1].visible == false)
        //                {
        //                    enemy[0].position = new Vector3(-20000, 760, -52000);
        //                    enemy[1].position = new Vector3(-20000, 760, -54000);
        //                    enemy[2].position = new Vector3(-29000, 760, -52900);
        //                    enemy[3].position = new Vector3(-29000, 760, -51000);
        //                    enemy[2].rotation = new Vector3(0, MathHelper.ToRadians(90), 0);
        //                    enemy[3].rotation = new Vector3(0, MathHelper.ToRadians(90), 0);
        //                    enemy[4].position = new Vector3(-27010, 760, -36220);
        //                    enemy[4].rotation = new Vector3(0, MathHelper.ToRadians(180), 0);

        //                    enemy[0].velocity = new Vector3(-20, 0, 0);
        //                    enemy[1].velocity = new Vector3(-20, 0, 0);
        //                    enemy[2].velocity = new Vector3(20, 0, 0);
        //                    enemy[3].velocity = new Vector3(20, 0, 0);
        //                    enemy[4].velocity = new Vector3(30, 0, 0);
        //                }
        //}


        public void checkcollisionmesh(staticmesh mesh)// used to check for collsion between robot and meshes 
        {
            for (int i = 0; i < numberofrobots; i++)
                //for (int i = 0; i < numberofhouses; i++)
                if (robot[i].bsphere.Intersects(mesh.bbox))
                {
                    robot[i].position.X = mesh.position.X;
                    robot[i].position.Y = mesh.position.Y;
                    robot[i].position.Z = mesh.position.Z;

                    robot[i].velocity.X = -robot[i].velocity.X;
                    robot[i].velocity.Y = -robot[i].velocity.Y;
                    robot[i].velocity.Z = -robot[i].velocity.Z;
                    robot[i].position = robot[i].oldposition;
                }
                else
                {
                    robot[i].oldposition = robot[i].position;
                }

        }

        //public void checkcollisionehite(model3d mesh)// used to check for collsion between robot and meshes 
        //{
 
        //    for (int count = 0; count < numberofenemys; count++)
        //        for (int i = 0; i < numberofenemys; i++)

       
        //        //for (int i = 0; i < numberofhouses; i++)
        //        if (enemy[i].bsphere.Intersects(mesh.bbox))
        //        {
        //            enemy[i].position.X = enemy[i].position.X;
        //            enemy[i].position.Y = enemy[i].position.Y;
        //            enemy[i].position.Z = enemy[i].position.Z;

        //            enemy[i].velocity.X = -enemy[i].velocity.X;
        //            enemy[i].velocity.Y = -enemy[i].velocity.Y;
        //            enemy[i].velocity.Z = -enemy[i].velocity.Z;
        //            enemy[i].position = enemy[i].oldposition;
        //        }
        //        else
        //        {
        //            enemy[i].oldposition = enemy[i].position;
        //        }

        //}

        public void checkenemycollisionmesh(staticmesh mesh)// used to check for colliosn between enmies and meshes
        {
            for (int i = 0; i < numberofenemys; i++)
                //for (int i = 0; i < numberofhouses; i++)
                if (enemy[i].bsphere.Intersects(mesh.bbox))
                {
                    //enemy[i].position.X = mesh.position.X;
                    //enemy[i].position.Y = mesh.position.Y;
                    //enemy[i].position.Z = mesh.position.Z;

                    enemy[i].velocity.X = -enemy[i].velocity.X;
                    enemy[i].velocity.Y = -enemy[i].velocity.Y;
                    enemy[i].velocity.Z = -enemy[i].velocity.Z;

                    if (enemy[i].velocity.Length() < 20)  // If the enemy has been chasing player then its velocity might be low so we reset it.
                    {
                        enemy[i].velocity.Normalize();
                        enemy[i].velocity *= 30;
                    }
                    enemy[i].position = enemy[i].oldposition;
             
                }
                else
                {
                    enemy[i].oldposition = enemy[i].position;
                }

        }

        public void checkcollisionenemybegiging(model3d mesh, float tim)// used for semi cut sceen at start set vitim to invisible and his dismeebe r body to visible
        {
            float timer = 10f;

            for (int i = 0; i < numberofenemys; i++)
     
                //for (int i = 0; i < numberofhouses; i++)
                if (enemy[i].bsphere.Intersects(mesh.bbox))
                {
                    //timer -= tim;
                    mesh.visible = false;
                    enemy[i].velocity = new Vector3(0, 0, 0);
                 

                }
                   
                else
                {
                    enemy[i].oldposition = enemy[i].position;
                }

        }

        public void bulletcollisionmesh(staticmesh mesh)// check for hit between buttes and meshes
        {
            for (int i = 0; i < numberofbullets; i++)
                if (bullet[i].bsphere.Intersects(mesh.bbox))
                {
                    bullet[i].visible = false;
                }
        }
        public void bullethitenemy()// used for checkking for hits between bullets and enemies
        {
           
            for(int i =0;i<numberofbullets;i++)
                for(int o=0;o<numberofenemys;o++)
                if (bullet[i].bsphere.Intersects(enemy[o].bbox))
                {
                    bullet[i].visible = false;
                    enemy[o].visible = false;
                }
        }
        //public void bullethitenemy()// used for checkking for hits between bullets and enemies
        //{

        //    for (int i = 0; i < numberofbullets; i++)
        //        for (int o = 0; o < numberofenemys; o++)
        //            if (bullet[i].visible && bullet[i].bbox.Intersects(enemy[o].bbox) && enemy[o].visible)
        //            {
        //                bullet[i].visible = false;
        //                enemy[o].visible = false;
        //            }
        //}

        void spraygun(float gtime)
        {
            
                sprayvisibletime = 3000;
                spray.visible = true;
                //updategame(spray);
         
        }

        public void updategame(float gtime)// updates the game 
        {
     
            // Main game code
            if (!gameover)
            {
               
                // Game is being played

                if (robot[0].visible)// move the player
                    robot[0].moveme(pad[0], gtime, 680);

                spray.position = new Vector3(robot[0].position.X + 150, robot[0].position.Y + 100, robot[0].position.Z +80);
                spray.direction.X = robot[0].direction.X;
                if (pad[0].Buttons.X == ButtonState.Pressed && spray.visible == false && sprayvisibletime <=-3000)
                    spraygun(gtime);
                sprayvisibletime -= gtime;
                if (sprayvisibletime <= 0)
                    spray.visible = false;
                spray.rotation.Y -= pad[0].ThumbSticks.Right.X / 12;
                //spray.position += spray.rotation;

                if (moaned.State == SoundState.Stopped && robot[1].visible)// plays sound effect
                    moaned.Play();

                if (music.State == SoundState.Stopped || music.State == SoundState.Paused) // plays sound effect
                    music.Play();

                //enemystartingposition();

                if (robot[0].velocity.Length() > 0.01f)// play sound effect when player is moving
                {
                    if (playermove.State == SoundState.Paused)
                        playermove.Resume();
                    if (playermove.State == SoundState.Stopped)
                        playermove.Play();
                }
                else
                {
                    if (playermove.State == SoundState.Playing)// pauses sound efect when palyer is moving
                        playermove.Pause();
                }


                  Boolean buttonreleased = true;// boolea for check ing the player is not pressing a button
                  float bulletreload = 0;

                  for (int i = 0; i < numberofbullets; i++)
                      if (pad[0].Buttons.A == ButtonState.Pressed && !bullet[i].visible && bulletreload <= 0 && buttonreleased)// fires a player bullet based on the button being pressed and timer being at 0 and the button is released and one is not bieng fired
                      {
                          buttonreleased = false;// resets the gun so the player can fire again
                          bullet[i].position = robot[0].position;// makes the staring point of the bullet the ships position
                          bullet[i].visible = true;// makes the bullet visible
                          //goodfire.Play();// playes the sound effect for the player bullet
                          bulletreload = 1000;// make the player wait before he can refire
                          //bullet[i].position += bullet[i].velocity;
                          //bullet[i].updateobject();
                          //  vibration = 1;
                      }
                  for (int i = 0; i < numberofbullets; i++)
                      if (bullet[i].visible)// only runs if the player bulet is visible
                      {
                          bullet[i].velocity = robot[0].direction * 350f;
                          bullet[i].position += bullet[i].velocity;// adds the player bullets velocity to player bulet position
                          bullet[i].updateobject();
                          bulletreload -= gtime;
                      }
                

                  if (pad[0].Buttons.A == ButtonState.Released) buttonreleased = true;// set


                  for (int i = 0; i < numberofbullets; i++)
                      if (bulletreload < -2000)
                          bullet[i].visible = false;


                  //for (int i = 0; i < numberofenemys; i++)
                  //    enemy[i].automove1(100, 100, 100, 100, 100, new Vector3(0, 0, 0));

                // Allow player to jump
                if (pad[0].Buttons.RightShoulder == ButtonState.Pressed) robot[0].jump(50, jump);



                bullethitenemy();// calls the method bullethitement

              
            

                //for (int i = 0; i < numberofleftwalls; i++)
                //    checkcollisionmesh(leftwall[i]);
                //for (int i = 0; i < numberofrightwalls; i++)
                //    checkcollisionmesh(rightwall[i]);
                //for (int i = 0; i < numberofhouses; i++)
                //    checkcollisionmesh(house1[i]);
                //for (int i = 0; i < numberofhouses2; i++)
                //    checkcollisionmesh(house2[i]);
                //for (int i = 0; i < numberofhouses2; i++)
                //    checkcollisionmesh(house3[i]);
                //for (int i = 0; i < numberofhouses; i++)
                //    checkcollisionmesh(house4[i]);
                //for (int i = 0; i < numberofhouses; i++)
                //    checkcollisionmesh(house5[i]);
                //for (int i = 0; i < numberofhouses; i++)
                //    checkcollisionmesh(house6[i]);
                //for (int i = 0; i < numberofhouses; i++)
                //    checkcollisionmesh(house7[i]);
                //for (int i = 0; i < numberofhouses3; i++)
                //    checkcollisionmesh(house8[i]);
                //for (int i = 0; i < numberofhouses3; i++)
                //    checkcollisionmesh(house9[i]);
                //for (int i = 0; i < numberofhouses3; i++)
                //    checkcollisionmesh(house10[i]);



                //for (int i = 0; i < church.Count(); i++)
                //    checkcollisionmesh(church[i]);
                //for (int i = 0; i < numberofshops; i++)
                //    checkcollisionmesh(shop[i]);
                //for (int i = 0; i < numberofshops; i++)
                //    checkcollisionmesh(shop2[i]);
                //for (int i = 0; i < numberofshops; i++)
                //    checkcollisionmesh(shop3[i]);
                for (int i = 0; i < numberofbtrees; i++)
                checkcollisionmesh(tree1[i]);
                for (int i = 0; i < numberofbtrees1; i++)
                    checkcollisionmesh(tree2[i]);
                for (int i = 0; i < numberofhorses; i++)
                    checkcollisionmesh(horse[i]);
                for (int i = 0; i < numberofboxes; i++)
                    checkcollisionmesh(mark[i]);

// checking for enmys hitting strucutres
                for (int i = 0; i < numberofleftwalls; i++)
                    checkenemycollisionmesh(leftwall[i]);
                for (int i = 0; i < numberofrightwalls; i++)
                    checkenemycollisionmesh(rightwall[i]);
                for (int i = 0; i < numberofhouses; i++)
                    checkenemycollisionmesh(house1[i]);
                for (int i = 0; i < numberofhouses2; i++)
                    checkenemycollisionmesh(house2[i]);
                for (int i = 0; i < numberofhouses2; i++)
                    checkenemycollisionmesh(house3[i]);
                for (int i = 0; i < numberofhouses; i++)
                    checkenemycollisionmesh(house4[i]);
                for (int i = 0; i < numberofhouses; i++)
                    checkenemycollisionmesh(house5[i]);
                for (int i = 0; i < numberofhouses; i++)
                    checkenemycollisionmesh(house6[i]);
                for (int i = 0; i < numberofhouses; i++)
                    checkenemycollisionmesh(house7[i]);
                for (int i = 0; i < numberofhouses3; i++)
                    checkenemycollisionmesh(house8[i]);
                for (int i = 0; i < numberofhouses3; i++)
                    checkenemycollisionmesh(house9[i]);
                for (int i = 0; i < numberofhouses3; i++)
                    checkenemycollisionmesh(house10[i]);



                for (int i = 0; i < church.Count(); i++)
                    checkenemycollisionmesh(church[i]);
                for (int i = 0; i < numberofshops; i++)
                    checkenemycollisionmesh(shop[i]);
                for (int i = 0; i < numberofshops; i++)
                    checkcollisionmesh(shop2[i]);
                for (int i = 0; i < numberofshops; i++)
                    checkenemycollisionmesh(shop3[i]);
                for (int i = 0; i < numberofbtrees; i++)
                    checkenemycollisionmesh(tree1[i]);
                for (int i = 0; i < numberofbtrees1; i++)
                    checkenemycollisionmesh(tree2[i]);
                for (int i = 0; i < numberofhorses; i++)
                    checkenemycollisionmesh(horse[i]);
                for (int i = 0; i < numberofboxes; i++)
                    checkenemycollisionmesh(mark[i]);

                // enemybounce
                //for (int i = 0; i < numberofhouses3; i++)
                //makeenemymoveawayfromwall(house3[i]);
                //for (int i = 0; i < numberofhouses; i++)
                //    makeenemymoveawayfromwall(house4[i]); 

                /// look for bullets itting building
                for (int i = 0; i < numberofleftwalls; i++)
                    bulletcollisionmesh(leftwall[i]);
                for (int i = 0; i < numberofrightwalls; i++)
                    bulletcollisionmesh(rightwall[i]);
                for (int i = 0; i < numberofhouses; i++)
                    bulletcollisionmesh(house1[i]);
                for (int i = 0; i < numberofhouses2; i++)
                    bulletcollisionmesh(house2[i]);
                for (int i = 0; i < numberofhouses2; i++)
                    bulletcollisionmesh(house3[i]);
                for (int i = 0; i < numberofhouses; i++)
                    bulletcollisionmesh(house4[i]);
                for (int i = 0; i < numberofhouses; i++)
                    bulletcollisionmesh(house5[i]);
                for (int i = 0; i < numberofhouses; i++)
                    bulletcollisionmesh(house6[i]);
                for (int i = 0; i < numberofhouses; i++)
                    bulletcollisionmesh(house7[i]);
                for (int i = 0; i < numberofhouses3; i++)
                    bulletcollisionmesh(house8[i]);
                for (int i = 0; i < numberofhouses3; i++)
                    bulletcollisionmesh(house9[i]);
                for (int i = 0; i < numberofhouses3; i++)
                    bulletcollisionmesh(house10[i]);



                for (int i = 0; i < church.Count(); i++)
                    bulletcollisionmesh(church[i]);
                for (int i = 0; i < numberofshops; i++)
                    bulletcollisionmesh(shop[i]);
                for (int i = 0; i < numberofshops; i++)
                    bulletcollisionmesh(shop2[i]);
                for (int i = 0; i < numberofshops; i++)
                    bulletcollisionmesh(shop3[i]);
                for (int i = 0; i < numberofbtrees; i++)
                    bulletcollisionmesh(tree1[i]);
                for (int i = 0; i < numberofbtrees1; i++)
                    bulletcollisionmesh(tree2[i]);
                for (int i = 0; i < numberofhorses; i++)
                    bulletcollisionmesh(horse[i]);
                for (int i = 0; i < numberofboxes; i++)
                    bulletcollisionmesh(mark[i]);
                for (int i = 0; i < numberofshops; i++)
                    bulletcollisionmesh(shop3[i]);


                for (int i = 0; i < numberofenemys; i++)
                    checkcollisionenemybegiging(robot[1],gtime);

                //for (int i = 0; i < numberofenemys; i++)
                //    checkcollisionehite(enemy[i]);

                for (int o = 0; o < numberofenemys; o++)
                {
                    if (robot[1].visible == false)
                    {
                        Vector3 distanceaway = enemy[o].position - robot[0].position;  //-- Calculate distance between zombie and player
                        if (distanceaway.Length() < 5000)
                        {
                            //if (soundeffecttimer_zombie <= 0 && enemy[i].visible)
                            //{
                            //    zombie_groan.Play();
                            //    soundeffecttimer_zombie = 5000;
                            //}


                            float waytogo = workoutangletoface(robot[0].position, enemy[o].position); // calculate which way to rotate to player
                            float direction2go = directiontoturn(enemy[o].rotation.Y, waytogo, MathHelper.ToRadians(2)); // calculate which direction zombie to move.
                            enemy[o].rotation.Y += direction2go / 10f;

                            enemy[o].moveAI(gtime, 780); // Move zombie
                            if (moaned.State == SoundState.Stopped)
                                moaned.Play();

                            enemy[o].bsphere.Center.Y += 140;


                        }

                    }
                    //else
                    //{
                    //    resetenemy();
                    //}
                }

                //enemyattack();


                // Set the camera to first person
                gamecamera.setFPor3P(robot[0].position, robot[0].direction, new Vector3(0,0,0), -60, 300, 60, 45);
                // Set side on camera view
                gamecamera.setsideon(robot[0].position, robot[0].rotation, 500,50);
                // Set overhead camera view
                gamecamera.setoverhead(robot[0].position, 1000);
                // Set the camera to third person
                gamecamera.setFPor3P(robot[0].position, robot[0].direction, robot[0].velocity, 500, 130, 180, 60);


                // Allow the camera to look up and down
                gamecamera.position.Y += (pad[0].ThumbSticks.Right.Y * 140);

                // Spin flying saucer object
                //lamppost[51].rotation.Y+=0.1f;

                ////atempt at using protected override void void bounding box

                for (int i = 0; i < numberofrobots; i++)
                    robot[i].updateobject();

                for (int i = 0; i < numberofbullets; i++)
                    bullet[i].updateobject();

                for (int i = 0; i < numberofenemys; i++)
                {
                    enemy[i].position += enemy[i].velocity;
                    enemy[i].updateobject();
                    ibsphere[i].updateobject();
                    ibsphere[i].position += enemy[i].velocity;
                   
                }
                //robot.UpdateBoundingBox(robot, transforms);

                //int wall_limits = 822;
                //if (robot.position.X < -wall_limits) robot.position.X = -wall_limits;
                //if (robot.position.X > wall_limits) robot.position.X = wall_limits;

                if (keys.IsKeyDown(Keys.Escape) || pad[0].Buttons.Back == ButtonState.Pressed)
                    gameover = true;    // End Game
            }
            else
            {
                // Game is over

                playermove.Stop();
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
                sfunctions3d.resetgraphics(GraphicsDevice);

                for (int i = 0; i < numberofground; i++)
                    ground[i].drawme(gamecamera, true);

                for (int i = 0; i < numberofrobots; i++)
                    robot[i].drawme(gamecamera, true);
                spray.drawme(gamecamera, true);

                if (robot[1].visible == false)
                    for (int i = 0; i < numberofbodyparts; i++)
                        bodyparts[i].drawme(gamecamera, true);

                for (int i = 0; i < numberofbullets; i++)
                    bullet[i].drawme(gamecamera, true);


                for (int i = 0; i < numberofenemys; i++)
                enemy[i].drawme(gamecamera, true);

                for (int i = 0; i < numberofenemys; i++)
                    ibsphere[i].drawme(gamecamera, true);

                for (int i = 0; i < numberoftrees; i++)
                    lamppost[i].drawme(gamecamera, true);

               

                for (int i = 0; i < numberofleftwalls; i++)
                {
                    leftwall[i].drawme(gamecamera, false);
                }
                for (int i = 0; i < numberofrightwalls; i++)
                {
                    rightwall[i].drawme(gamecamera, false);
                }
                for (int i = 0; i < numberofhouses; i++)
                house1[i].drawme(gamecamera, false);
                for (int i = 0; i < numberofhouses2; i++)
                    house2[i].drawme(gamecamera, false);
                for (int i = 0; i < numberofhouses2; i++)
                    house3[i].drawme(gamecamera, false);
                for (int i = 0; i < numberofhouses; i++)
                    house4[i].drawme(gamecamera, false);
                for (int i = 0; i < numberofhouses; i++)
                    house5[i].drawme(gamecamera, false);
                for (int i = 0; i < numberofhouses; i++)
                    house6[i].drawme(gamecamera, false);
                for (int i = 0; i < numberofhouses; i++)
                    house7[i].drawme(gamecamera, false);
                for (int i = 0; i < numberofhouses3; i++)
                    house8[i].drawme(gamecamera, false);
                //for (int i = 0; i < numberofhouses3; i++)
                //    house9[i].drawme(gamecamera, false);
                for (int i = 0; i < numberofhouses3; i++)
                    house10[i].drawme(gamecamera, false);

                for (int i = 0; i < numberofshops; i++)
                    shop[i].drawme(gamecamera, false);
                for (int i = 0; i < numberofshops; i++)
                    shop2[i].drawme(gamecamera, false);
                for (int i = 0; i < numberofshops; i++)
                    shop3[i].drawme(gamecamera, false);
                for (int i = 0; i < church.Count(); i++)
                    church[i].drawme(gamecamera, false);
                for (int i = 0; i < numberofcars; i++)
                    car[i].drawme(gamecamera, false);
                for (int i = 0; i < numberofbarrels; i++)
                barrel[i].drawme(gamecamera, false);

                for (int i = 0; i < numberofbtrees; i++)
                    tree1[i].drawme(gamecamera, false);
                for (int i = 0; i < numberofbtrees1; i++)
                    tree2[i].drawme(gamecamera, false);
                for (int i = 0; i < numberofhorses; i++)
                    horse[i].drawme(gamecamera, false);
                for (int i = 0; i < numberofboxes; i++)
                    mark[i].drawme(gamecamera, false);
           
            


                if (gameover)
                {
                    spriteBatch.Begin();
                    spriteBatch.DrawString(mainfont, "GAME OVER", new Vector2(130, 100),
                    Color.White, MathHelper.ToRadians(0), new Vector2(0, 0), 3f, SpriteEffects.None, 0);
                    spriteBatch.End();
                }

            for(int c=0;c<leftwall.Length;c++)
                ShapeRenderingSample.DebugShapeRenderer.AddBoundingBox(leftwall[c].bbox, Color.Red);
            
            for (int c = 0; c < rightwall.Length; c++)
                ShapeRenderingSample.DebugShapeRenderer.AddBoundingBox(rightwall[c].bbox, Color.Red);
           

            for (int c = 0; c < house1.Length; c++)
                ShapeRenderingSample.DebugShapeRenderer.AddBoundingBox(house1[c].bbox, Color.Red);
         
            for (int c = 0; c < house2.Length; c++)
                ShapeRenderingSample.DebugShapeRenderer.AddBoundingBox(house2[c].bbox, Color.Red);


            for (int c = 0; c < house3.Length; c++)
                ShapeRenderingSample.DebugShapeRenderer.AddBoundingBox(house3[c].bbox, Color.Red);

            for (int c = 0; c < house4.Length; c++)
                ShapeRenderingSample.DebugShapeRenderer.AddBoundingBox(house4[c].bbox, Color.Red);

            for (int c = 0; c < house5.Length; c++)
                ShapeRenderingSample.DebugShapeRenderer.AddBoundingBox(house5[c].bbox, Color.Red);
          
            for (int c = 0; c < house6.Length; c++)
                ShapeRenderingSample.DebugShapeRenderer.AddBoundingBox(house6[c].bbox, Color.Red);

            for (int c = 0; c < house7.Length; c++)
                ShapeRenderingSample.DebugShapeRenderer.AddBoundingBox(house7[c].bbox, Color.Red);
                     
            for (int c = 0; c < shop.Length; c++)
                ShapeRenderingSample.DebugShapeRenderer.AddBoundingBox(shop[c].bbox, Color.Red);
            for (int c = 0; c < house8.Length; c++)
                ShapeRenderingSample.DebugShapeRenderer.AddBoundingBox(house8[c].bbox, Color.Red);
            for (int c = 0; c < house9.Length; c++)
                ShapeRenderingSample.DebugShapeRenderer.AddBoundingBox(house8[c].bbox, Color.Red);
            for (int c = 0; c < house10.Length; c++)
                ShapeRenderingSample.DebugShapeRenderer.AddBoundingBox(house8[c].bbox, Color.Red);


            for (int c = 0; c < shop2.Length; c++)
                ShapeRenderingSample.DebugShapeRenderer.AddBoundingBox(shop2[c].bbox, Color.Red);

            for (int c = 0; c < shop3.Length; c++)
                ShapeRenderingSample.DebugShapeRenderer.AddBoundingBox(shop3[c].bbox, Color.Red);

            for (int c = 0; c < shop3.Length; c++)
                ShapeRenderingSample.DebugShapeRenderer.AddBoundingBox(shop3[c].bbox, Color.Red);

            for (int c = 0; c < church.Length; c++)
                ShapeRenderingSample.DebugShapeRenderer.AddBoundingBox(church[c].bbox, Color.Red);


            for (int c = 0; c < tree1.Length; c++)
                ShapeRenderingSample.DebugShapeRenderer.AddBoundingBox(tree1[c].bbox, Color.Red);
            for (int c = 0; c < tree2.Length; c++)
                ShapeRenderingSample.DebugShapeRenderer.AddBoundingBox(tree2[c].bbox, Color.Red);
            for (int c = 0; c < horse.Length; c++)
                ShapeRenderingSample.DebugShapeRenderer.AddBoundingBox(horse[c].bbox, Color.Red);
            for (int c = 0; c < mark.Length; c++)
                ShapeRenderingSample.DebugShapeRenderer.AddBoundingBox(mark[c].bbox, Color.Red);
            for (int c = 0; c < robot.Length; c++)
                ShapeRenderingSample.DebugShapeRenderer.AddBoundingBox(robot[c].bbox, Color.Red);
            for (int c = 0; c < ibsphere.Length; c++)
                ShapeRenderingSample.DebugShapeRenderer.AddBoundingBox(ibsphere[c].bbox, Color.Red);
            //for (int c = 0; c < car.Length; c++)
            //    ShapeRenderingSample.DebugShapeRenderer.AddBoundingBox(car[c].bbox, Color.Red);

            ShapeRenderingSample.DebugShapeRenderer.Draw(new GameTime(new TimeSpan(0, 0, 0), new TimeSpan(0, 0, 0), false), gamecamera.getview(), gamecamera.getproject());
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
                if (highscorenames[i].Length >= 24)
                    spriteBatch.DrawString(mainfont, (i + 1).ToString("0") + ". " + highscorenames[i].Substring(0, 24), new Vector2(60, 100 + (i * 30)),
                        Color.White, MathHelper.ToRadians(0), new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                else
                    spriteBatch.DrawString(mainfont, (i + 1).ToString("0") + ". " + highscorenames[i], new Vector2(60, 100 + (i * 30)),
                        Color.White, MathHelper.ToRadians(0), new Vector2(0, 0), 1f, SpriteEffects.None, 0);

                spriteBatch.DrawString(mainfont, highscores[i].ToString("0"), new Vector2(displaywidth - 180, 100 + (i * 30)),
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
