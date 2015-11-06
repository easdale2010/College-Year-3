using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
//using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Media;

namespace gamestateManagement
{
    // define possible game states
    enum gamestates { menu, playingMinigame1, playingMinigame2 };

    // create an independent class to store the current game state
    static class gameStateManager
    {
        public static gamestates currentState;
    }

    public class baseGameState
    {
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        public ContentManager Content;

        protected baseGameState(ContentManager ContentManagerObject, SpriteBatch BatchObject, GraphicsDeviceManager GraphicsManagerObject)
        {
            graphics = GraphicsManagerObject;
            spriteBatch = BatchObject;
            Content = ContentManagerObject;
        }
    }
}
