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
using gameworld3d;

namespace GradedUnit
{
    // define possible game states
    enum gamestates { menu, playingMinigame1, playingMinigame2 };

    // create an independent class to give global access to important game-wide services and information.
    static class globals
    {
        public static gamestates currentState;
        public static GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;
        public static ContentManager Content;
        public static Game1 mainGameScreen;
        public static int displaywidth = 1024;    // setting the width and height manually will simplify screen display and avoid a couple of common errors
        public static int displayheight = 768;    // such as developing for a particular resolution and forgetting that other computers will be different.
    }

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public minigame1 minigame1Screen;
        public minigame2 minigame2Screen;
        public mainmenu menuScreen;

        public Game1()
        {
            globals.graphics = new GraphicsDeviceManager(this);
            globals.graphics.PreferredBackBufferWidth = globals.displaywidth;
            globals.graphics.PreferredBackBufferHeight = globals.displayheight;
            globals.Content = Content;
            globals.Content.RootDirectory = "Content";
            globals.mainGameScreen = this;
        }

        protected override void Initialize()
        {
            globals.currentState = gamestates.menu;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            globals.spriteBatch = new SpriteBatch(GraphicsDevice);

            minigame1Screen = new minigame1();
            minigame1Screen.LoadContent();

            minigame2Screen = new minigame2();
            minigame2Screen.LoadContent();

            menuScreen = new mainmenu();
            menuScreen.LoadContent();
        }
        protected override void UnloadContent() { }

        protected override void Update(GameTime gameTime)
        {
            switch (globals.currentState)
            {
                case gamestates.menu:
                    menuScreen.Update(gameTime);
                    break;
                case gamestates.playingMinigame1:
                    minigame1Screen.Update(gameTime);
                    break;
                case gamestates.playingMinigame2:
                    minigame2Screen.Update(gameTime);
                    break;
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            switch (globals.currentState)
            {
                case gamestates.menu:
                    menuScreen.Draw(gameTime);
                    break;
                case gamestates.playingMinigame1:
                    minigame1Screen.Draw(gameTime);
                    break;
                case gamestates.playingMinigame2:
                    minigame2Screen.Draw(gameTime);
                    break;
            }
            base.Draw(gameTime);
        }
    }

    // The program entry point
    static class Program
    {
        static void Main(string[] args)
        {
            using (Game1 game = new Game1())
                game.Run();
        }
    }
}