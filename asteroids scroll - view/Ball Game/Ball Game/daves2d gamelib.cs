using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Input.Touch; // Include for Windows Phone games
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace gamelib2d
{
  

    public class scrollingbackground
    {
        public Texture2D imagemain;                 // Texture to hold image
        public float scale;
        public int gamewidth;
        public int gameheight;
        public int columns = 1;
        public int rows = 1;

        public scrollingbackground() { }  // Empty constructor to avoid crashes

        // Constructor which loads image and works out the new gamewidth & gameheight. Also set the size of the main background rectangle and the size of the part2draw rectangle
        public scrollingbackground(ContentManager content, string spritename, float sizeratio, int colnum, int rownum)
        {
            imagemain = content.Load<Texture2D>(spritename);
            scale = sizeratio;
            columns = colnum;
            rows = rownum;
            gamewidth = (int)(imagemain.Width * columns * scale);
            gameheight = (int)(imagemain.Height * rows * scale);
        }

        public void makehorizontal(int dheight)
        {
            rows = 1;
            gameheight = dheight;
            scale = (float)gameheight / (float)imagemain.Height;
            gamewidth = (int)(imagemain.Width * columns * scale);
        }

        public void makevertical(int dwidth)
        {
            columns = 1;
            gamewidth = dwidth;
            scale = (float)gamewidth / (float)imagemain.Width;
            gameheight = (int)(imagemain.Height * rows * scale);
        }

        // Use this method to draw a part of the image at a specific position
        virtual public Vector3 drawme(ref SpriteBatch sbatch, Vector3 position, int dwidth, int dheight, out Vector3 offset)
        {
            Vector3 corner = new Vector3(0, 0, 0);
            Rectangle panel = new Rectangle(0, 0, 0, 0);
            panel.Width = (int)(imagemain.Width * scale);
            panel.Height = (int)(imagemain.Height * scale);
            Vector3 screenpos = new Vector3((float)(dwidth / 2), (float)(dheight / 2), 0);

            // Check if ship is near the edges of the game world and adjust it's position
            if (position.X < dwidth / 2)
                screenpos.X = position.X;
            if (position.Y < dheight / 2)
                screenpos.Y = position.Y;
            if (position.X > gamewidth - dwidth / 2)
                screenpos.X = dwidth - (gamewidth - position.X);
            if (position.Y > gameheight - dheight / 2)
                screenpos.Y = dheight - (gameheight - position.Y);

            // Loop through all the background panels in the game and draw them
            for (int x = 0; x < columns; x++)
            {
                corner.X = position.X - screenpos.X;
                panel.X = (int)(x * imagemain.Width * scale - corner.X);
                for (int y = 0; y < rows; y++)
                {
                    corner.Y = position.Y - screenpos.Y;
                    panel.Y = (int)(y * imagemain.Height * scale - corner.Y);

                    // If panel is within the visible screen draw it
                    if ((panel.X < gamewidth || panel.Y < gameheight) && (panel.Right > 0 || panel.Bottom > 0))
                        sbatch.Draw(imagemain, panel, null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, 0);
                }
            }

            // Work out the position offset amount for everything else in the game
            offset = new Vector3(position.X - screenpos.X, position.Y - screenpos.Y, 0);

            return screenpos;
        }
    }

    public class scrollingbackground2 : scrollingbackground
    {
        public Texture2D[,] image;                 // Texture to hold image

        public scrollingbackground2() { }  // Empty constructor to avoid crashes

        // Constructor which loads image and works out the new gamewidth & gameheight. Also set the size of the main background rectangle and the size of the part2draw rectangle
        public scrollingbackground2(ContentManager content, string spritename, float sizeratio, int colnum, int rownum)
        {
            columns = colnum;
            rows = rownum;
            image = new Texture2D[colnum, rownum];
            imagemain = content.Load<Texture2D>(spritename);
            for (int x = 0; x < colnum; x++)
                for (int y = 0; y < rownum; y++)
                {
                    image[x, y] = content.Load<Texture2D>(spritename);
                }
            scale = sizeratio;
            gamewidth = (int)(image[0, 0].Width * columns * scale);
            gameheight = (int)(image[0, 0].Height * rows * scale);
        }

        // Use this method to draw a part of the image at a specific position
        override public Vector3 drawme(ref SpriteBatch sbatch, Vector3 position, int dwidth, int dheight, out Vector3 offset)
        {
            Vector3 corner = new Vector3(0, 0, 0);
            Rectangle panel = new Rectangle(0, 0, 0, 0);
            panel.Width = (int)(image[0, 0].Width * scale);
            panel.Height = (int)(image[0, 0].Height * scale);
            Vector3 screenpos = new Vector3((float)(dwidth / 2), (float)(dheight / 2), 0);

            // Check if ship is near the edges of the game world and adjust it's position
            if (position.X < dwidth / 2)
                screenpos.X = position.X;
            if (position.Y < dheight / 2)
                screenpos.Y = position.Y;
            if (position.X > gamewidth - dwidth / 2)
                screenpos.X = dwidth - (gamewidth - position.X);
            if (position.Y > gameheight - dheight / 2)
                screenpos.Y = dheight - (gameheight - position.Y);

            // Loop through all the background panels in the game and draw them
            for (int x = 0; x < columns; x++)
            {
                corner.X = position.X - screenpos.X;
                panel.X = (int)(x * image[0, 0].Width * scale - corner.X);
                for (int y = 0; y < rows; y++)
                {
                    corner.Y = position.Y - screenpos.Y;
                    panel.Y = (int)(y * image[0, 0].Height * scale - corner.Y);

                    // If panel is within the visible screen draw it
                    if ((panel.X < gamewidth || panel.Y < gameheight) && (panel.Right > 0 || panel.Bottom > 0))
                        sbatch.Draw(image[x, y], panel, null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, 0);
                }
            }

            // Work out the position offset amount for everything else in the game
            offset = new Vector3(position.X - screenpos.X, position.Y - screenpos.Y, 0);

            return screenpos;
        }
    }


}