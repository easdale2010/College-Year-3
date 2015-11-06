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
    public class mainmenu
    {
        SpriteFont menufont;

        public void LoadContent()
        {
            menufont = globals.Content.Load<SpriteFont>("Verdana");
        }

        public void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.D1))
                globals.currentState = gamestates.playingMinigame1;
            if (Keyboard.GetState().IsKeyDown(Keys.D2))
            {
                globals.currentState = gamestates.playingMinigame2;
                globals.mainGameScreen.minigame2Screen.reset();
            }
        }

        public void Draw(GameTime gameTime)
        {
            globals.spriteBatch.Begin();
            globals.spriteBatch.DrawString(menufont, "MAIN MENU", new Vector2(300, 20), Color.White);
            globals.spriteBatch.DrawString(menufont, "Press (1) to watch a guy run randomly.", new Vector2(10, 100), Color.White);
            globals.spriteBatch.DrawString(menufont, "Or (2) to run sideways.", new Vector2(10, 150), Color.White);
            globals.spriteBatch.End();
        }
    }
}
