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
using WindowsGameLibrary1;

namespace gamemenu
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

        const int numberofhighscores = 10;
        int[] highscore = new int[numberofhighscores];
        string[] highscorename = new string[numberofhighscores];

        Vector3 cameraposition;
        const float cameradistance = 1000f;
        Vector3 cameralookat;
        Vector3 cameraorientation =Vector3.Up;
        float fov = 45;
        float aspectratio;

        model3d robot;
        staticmesh ground;

        const int numberoftrees = 60;
        staticmesh[] trees = new staticmesh[numberoftrees];


        Boolean gameover = false;
        float gameruntime = 0;

        graphics2d background;
        Random randomiser = new Random();

        int gamestate = -1;

        GamePadState[] pad = new GamePadState[4];
        KeyboardState keys;
        MouseState mouse;

        SpriteFont mainfont;

        sprites2d mousepointer1, mousepointer2;

        const int numberofoptions = 4;
        sprites2d[,] menuoptions = new sprites2d[numberofoptions, 2];
        int optionselected = 0;
        

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.graphics.PreferredBackBufferWidth = 800;
            this.graphics.PreferredBackBufferHeight = 600;
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

            if (File.Exists(@"highscore.txt"))
            {
                String line;
                StreamReader sr = new StreamReader(@"highscore.txt");
                line = sr.ReadLine();
                sr.Close();
                line = line.Trim();
                for (int i = 0; i < numberofhighscores; i++)
                    highscore[i] = (int)Convert.ToDecimal(line);


            }

            cameraposition = new Vector3(1000, 100, 10);
            cameralookat = new Vector3(0, 400, 0);
            aspectratio = (float)displaywidth / (float)displayheight;


            ground = new staticmesh(Content, "sground", 100f, new Vector3(0, -40, 0), new Vector3(0, 0, 0));
            robot = new model3d(Content, "r2d2v2", 2f, new Vector3(1000, 0, 1000), new Vector3(0, 0, 0), 0.05f, 0.5f, 10);

            for (int i = 0; i < numberoftrees; i++)
            {
                if (i % 2 == 0)
                    trees[i] = new staticmesh(Content, "tree", (float)(randomiser.Next(20) + 1) / 10,
                        new Vector3(randomiser.Next(6000) - 3000, 0, randomiser.Next(6000) - 3000),
                        new Vector3(0, randomiser.Next(7), 0));
                else
                {
                    trees[i] = new staticmesh(Content, "lamppost", (float)(randomiser.Next(20) + 1) / 3,
                        new Vector3(randomiser.Next(6000) - 3000, 0, randomiser.Next(6000) - 3000),
                        new Vector3(0, randomiser.Next(70), 0));

                    trees[i].position.Y = trees[i].size * 40;
                }
            }

            background = new graphics2d(Content, "Background for Menus", displaywidth, displayheight);
            mousepointer1 = new sprites2d(Content, "X-Games-Cursor", 0, 0, 0.15f, Color.White, true, randomiser);
            mousepointer2 = new sprites2d(Content, "X-Games-Cursor-Highlight", 0, 0, 0.15f, Color.White, true,randomiser);

            menuoptions[0, 0] = new sprites2d(Content, "Start-Normal", displaywidth / 2, 200, 1, Color.White, true,randomiser);
            menuoptions[0, 1] = new sprites2d(Content, "Start-Selected", displaywidth / 2, 200, 1,Color.White, true,randomiser);
            menuoptions[1, 0] = new sprites2d(Content, "Options-Normal", displaywidth / 2, 300, 1, Color.White, true,randomiser);
            menuoptions[1, 1] = new sprites2d(Content, "options-Selected", displaywidth / 2, 300, 1, Color.White, true,randomiser);
            menuoptions[2, 0] = new sprites2d(Content, "High-Score-Normal", displaywidth / 2, 400, 1, Color.White, true,randomiser);
            menuoptions[2, 1] = new sprites2d(Content, "High-Score-Selected", displaywidth / 2, 400, 1, Color.White, true,randomiser);
            menuoptions[3, 0] = new sprites2d(Content, "Exit-Normal", displaywidth / 2, 500, 1, Color.White, true,randomiser);
            menuoptions[3, 1] = new sprites2d(Content, "Exit-Selected", displaywidth / 2, 500, 1, Color.White, true,randomiser);

            for (int i = 0; i < numberofoptions; i++)
                menuoptions[i, 0].updateobject();

            mainfont = Content.Load<SpriteFont>("mainfont");

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {

            StreamWriter sw = new StreamWriter(@"highscore.txt");
            for (int i = 0; i < numberofhighscores; i++)
            sw.WriteLine(highscore[i].ToString());
            sw.Close();
      
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            pad[0] = GamePad.GetState(PlayerIndex.One);
            pad[1] = GamePad.GetState(PlayerIndex.Two);
            pad[2] = GamePad.GetState(PlayerIndex.Three);
            pad[3] = GamePad.GetState(PlayerIndex.Four);
            keys = Keyboard.GetState();
            mouse = Mouse.GetState();

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            float timebetweenupdates = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            gameruntime += timebetweenupdates;

            mousepointer1.position.X = mouse.X;
            mousepointer1.position.Y = mouse.Y;
            mousepointer1.updateobject();

            mousepointer1.bsphere = new BoundingSphere(mousepointer1.position, 2);

            switch (gamestate)
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
            optionselected = -1;

            for (int i = 0; i < numberofoptions; i++)
            {
                if (mousepointer1.bsphere.Intersects(menuoptions[i, 0].bbox))
                {
                    optionselected = i;
                    if (mouse.LeftButton == ButtonState.Pressed)
                        gamestate = optionselected;
                }
            }
        }
        public void drawmenu()
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

        public void updategame(float gtime)
        {
            if (!gameover)
            {
                robot.moveme(pad[0], gtime, 70);

                if (pad[0].Buttons.A == ButtonState.Pressed) robot.jump(100, gtime);

                cameraposition = robot.position + (robot.direction * 60);
                //    cameraposition = robot.position - (robot.direction * 300);
                cameraposition.Y += 60;
                cameralookat = robot.position + (robot.direction * 300);
                cameralookat.Y += 45;
                cameraorientation = Vector3.Up;

                //// top down view
                //cameralookat = robot.position;
                //cameraposition.X = robot.position.X;
                //cameraposition.Z = robot.position.Z;
                //cameraposition.Y = cameradistance;
                //cameraorientation = Vector3.Left;


                ////side view
                //Vector3 cameradirection = new Vector3(0, 0, 0);
                //cameradirection.Z = (float)(Math.Cos(robot.rotation.Y + MathHelper.ToRadians(90)));
                //cameradirection.X = (float)(Math.Sin(robot.rotation.Y + MathHelper.ToRadians(90)));
                //cameralookat = robot.position;
                //cameraposition = robot.position - (cameradirection * 500);
                //cameraposition.Y += 50;
                //cameraorientation = Vector3.Up;


                // third person veiw
                //Vector3 tempvel = robot.velocity;
                //tempvel.Y = 0;
                //cameraposition = robot.position - ((robot.direction * 300) + (tempvel * 3));
                //cameraposition.Y += 100;
                //cameralookat = robot.position + ((robot.velocity * 100) + (tempvel * 1));
                //cameralookat.Y += 60;
                //cameraorientation = Vector3.Up;

                if (pad[0].Buttons.Back == ButtonState.Pressed)
                    gameover = true;
            }
            else
                if (keys.IsKeyDown(Keys.Escape)) gamestate = -1;
        }

        public void drawgame()
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(mainfont, "GAMEPLAY", new Vector2(displaywidth / 2 - 100, 20), Color.Red, 0,
            new Vector2(0, 0), 1.5f, SpriteEffects.None, 0);
            spriteBatch.End();

            sfunctions.resetgraphics(GraphicsDevice);

            ground.drawme(cameraorientation, true, cameraposition, cameralookat, fov, aspectratio);
            robot.drawme(cameraorientation, true, cameraposition, cameralookat, fov, aspectratio);

            for (int i = 0; i < numberoftrees; i++)
               trees[i].drawme(cameraorientation, true, cameraposition, cameralookat, fov, aspectratio);
        }

        public void updateoptions()
        {
            if (keys.IsKeyDown(Keys.Escape)) gamestate = -1;
        }
        public void drawoptions()
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

        public void updatehighscore()
        {
            if (keys.IsKeyDown(Keys.Escape)) gamestate = -1;
        }
        public void drawhighscore()
        {
            spriteBatch.Begin();


            spriteBatch.DrawString(mainfont, "High Score".ToString(), new Vector2(displaywidth / 2 - 100, 20), Color.Red, 0,
            new Vector2(0, 0), 1.5f, SpriteEffects.None, 0);

            for (int i = 0; i < numberofhighscores; i++)
                spriteBatch.DrawString(mainfont, " " + (i + 1) +". "+ highscore[i].ToString("0"), new Vector2(displaywidth / 2 -100, (i *40 + 60)), Color.White, 0,
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

            switch (gamestate)
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
