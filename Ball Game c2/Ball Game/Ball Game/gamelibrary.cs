using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace WindowsGameLibrary1
{
    public class graphics2d
    {
        public Texture2D image;
        public Rectangle rect;

        public graphics2d() { }
        public graphics2d(ContentManager content, string spritename, int dwidth, int dheight)
        {
            image = content.Load<Texture2D>(spritename);
            float ratio = ((float)dwidth / image.Width);
            rect.Width = dwidth;
            rect.Height = (int)(image.Height * ratio);
            rect.X = 0;
            rect.Y = (dheight - rect.Height) / 2;
        }
        public void drawme(ref SpriteBatch spriteBatch2)
        {
            spriteBatch2.Draw(image, rect, Color.White);
        }
    }
    public class sprites2d
    {
        public Texture2D image;
        public Vector3 position;
        public Vector3 oldposition;
        public Rectangle rect;
        public Vector2 origin;
        public float rotation = 0;
        public Vector3 velocity;
        public BoundingSphere bsphere;
        public Boolean visible = true;
        public Color colour = Color.White;

        public sprites2d() { }
        public sprites2d(ContentManager content, string spritename, int x, int y, float msize, Color mcolour, Boolean mvis)
        {
            image = content.Load<Texture2D>(spritename);
            position = new Vector3((float)x, (float)y, 0);
            rect.X = x;
            rect.Y = y
                ;
            origin.X = image.Width / 2;
            origin.Y = image.Height / 2;
            rect.Width = (int)(image.Width * msize);
            rect.Height = (int)(image.Height * msize);
            colour = mcolour;
            visible = mvis;
        }
        public void moveme(GamePadState gpad, KeyboardState keys, int dwidth, int dheight, float gtime)
        {
            velocity.X = gpad.ThumbSticks.Left.X;
            velocity.Y = -gpad.ThumbSticks.Left.Y;

            float speed = 0.5f;
            position += velocity * gtime * speed;

            if (position.X < rect.Width / 2) position.X = rect.Width / 2;
            if (position.X > dwidth - rect.Width / 2) position.X = dwidth - rect.Width / 2;
            if (position.Y < rect.Height / 2) position.Y = rect.Height / 2;
            if (position.Y > dheight - rect.Height / 2) position.Y = dheight - rect.Height / 2;

            updateobject();
        }

        public void updateobject()
        {
            rect.Y = (int)position.Y;
            rect.X = (int)position.X;
            bsphere = new BoundingSphere(position, rect.Width / 2);
        }
        public void drawme(ref SpriteBatch sbatch)
        {
            if (visible)
                sbatch.Draw(image, rect, null, colour, rotation, origin, SpriteEffects.None, 0);
        }
    }
}
