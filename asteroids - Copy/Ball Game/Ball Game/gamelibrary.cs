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
        public BoundingBox bbox;
        public Boolean visible = true;
        public Color colour = Color.White;
        public float size;
        private float spinspeed=0.02f;

        public sprites2d() { }
        public sprites2d(ContentManager content, string spritename, int x, int y, float msize, Color mcolour, Boolean mvis, Random randomiser)
        {
            image = content.Load<Texture2D>(spritename);
            position = new Vector3((float)x, (float)y, 0);
            rect.X = x;
            rect.Y = y;
            origin.X = image.Width / 2;
            origin.Y = image.Height / 2;
            rect.Width = (int)(image.Width * msize);
            rect.Height = (int)(image.Height * msize);
            colour = mcolour;
            visible = mvis;
            size = msize;
            oldposition = position;
            
            spinspeed = (float)(randomiser.Next(100)-50)/100f;

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

        public void automove(int dwidth, int dheight, float gtime)
        {
            rotation += spinspeed;
            position += velocity * gtime;

            if ((position.X + rect.Width / 2) > dwidth)
            {
                velocity.X = -velocity.X;
                position.X = dwidth - rect.Width / 2;
            }
            if ((position.X - rect.Width / 2) <= 0)
            {
                velocity.X = -velocity.X;
                position.X = rect.Width / 2;
            }
            if ((position.Y + rect.Height / 2) >= dheight)
            {
                velocity.Y = -velocity.Y;
                position.Y = dheight - rect.Height / 2;
            }
            if ((position.Y - rect.Height / 2) <= 0)
            {
                velocity.Y = -velocity.Y;
                position.Y = rect.Height / 2;
            }
            updateobject();
        }

        public void updateobject()
        {
            rect.Y = (int)position.Y;
            rect.X = (int)position.X;
            bsphere = new BoundingSphere(position, rect.Width / 2);

            bbox = new BoundingBox(new Vector3(position.X - rect.Width / 2, position.Y - rect.Height / 2, 0),
            new Vector3(position.X + rect.Width / 2, position.Y + rect.Height / 2, 0));

        }
        public void drawme(ref SpriteBatch sbatch)
        {
            if (visible)
                sbatch.Draw(image, rect, null, colour, rotation, origin, SpriteEffects.None, 0);
        }
    }

    public class ships : sprites2d
    {
        public Vector3 direction;
        float thrust;
        float rotationspeeed = 0.005f;
        float shipspeed = 0.01f;
        float friction = 0.99f;
        public int lives = 5;
        public int score = 0;
        public float spawntime = 0;

        public ships(){}
        public ships(ContentManager content, string spritename,int x,int y, float msize, Color mcolour,Boolean mvis)
        {
            image = content.Load<Texture2D>(spritename);
            position = new Vector3((float)x, (float)y, 0);
            rect.Y = y;
            rect.X = x;
            origin.X = image.Width / 2;
            origin.Y = image.Height / 2;
            rect.Width = (int)(image.Width * msize);
            rect.Height = (int)(image.Height * msize);
            colour = mcolour;
            visible = mvis;
            size = msize;
            oldposition = position;
        }
        public void moveme(GamePadState gpad, int dwidth, int dheight, float gtime)
        {
            spawntime += gtime;

            rotation += gpad.ThumbSticks.Left.X * rotationspeeed * gtime;
            thrust = (shipspeed * gtime * (gpad.Triggers.Right - gpad.Triggers.Left));

            direction.X = (float)(Math.Cos(rotation));
            direction.Y = (float)(Math.Sin(rotation));

            velocity += direction * thrust;
            velocity *= friction;
            position += velocity;

            if (position.X < rect.Width / 2) position.X = rect.Width / 2;
            if (position.Y < rect.Height / 2) position.Y = rect.Height / 2;
            if (position.X > dwidth - rect.Width / 2) position.X = dwidth - rect.Width / 2;
            if (position.Y > dheight - rect.Height / 2) position.Y = dheight - rect.Height / 2;

            updateobject();
        }
    }

    public class ammo : sprites2d
    {
        public float bulletlength = 1000;
        public float bulletspawned = 1001;

        public ammo() { }
        public ammo(ContentManager content, string spritename, int x, int y, float msize, Color mcolour, Boolean mvis)
        {
            image = content.Load<Texture2D>(spritename);
            position = new Vector3((float)x, (float)y, 0);
            rect.Y = y;
            rect.X = x;
            origin.X = image.Width / 2;
            origin.Y = image.Height / 2;
            rect.Width = (int)(image.Width * msize);
            rect.Height = (int)(image.Height * msize);
            size = msize;
            colour = mcolour;
            oldposition = position;
            visible = mvis;
        }
        public void firebullet(Vector3 pos, Vector3 dir)
        {
            if (!visible && bulletspawned > bulletlength)
            {
                float bulletspeed = 1.5f;
                visible = true;
                position = pos;
                velocity = dir * bulletspeed;
                updateobject();
              
                bulletspawned = 0;
              
            }
        }
        public void movebullet(float gtime)
        {
            bulletspawned += gtime;
            if (visible)
                position += velocity * gtime;

            if (bulletspawned > bulletlength) visible = false;

            updateobject();
        }
        public void firebullet2(Vector3 pos, Vector3 dir)
        {
            if (!visible)
            {
                float bulletspeed = 1.5f;
                visible = true;
                position = pos;
                velocity = dir * bulletspeed;
                updateobject();

                bulletspawned = 0;

            }
        }
        public void movebullet2(float gtime,int dwidth,int dheight)
        {
    
            if (visible)
                position += velocity * gtime;

            if (position.X > dwidth || position.X < rect.Width || position.Y < rect.Height || position.Y > dheight) visible = false;

            updateobject();
        }
    }

    public static class sfunctions
    {
        // Function to handle collision response
        public static void cresponse(Vector3 position1, Vector3 position2, ref Vector3 velocity1, ref Vector3 velocity2, float weight1, float weight2)
        {
            // Calculate Collision Response Directions
            Vector3 x = position1 - position2;
            x.Normalize();
            Vector3 v1x = x * Vector3.Dot(x, velocity1);
            Vector3 v1y = velocity1 - v1x;
            x = -x;
            Vector3 v2x = x * Vector3.Dot(x, velocity2);
            Vector3 v2y = velocity2 - v2x;

            velocity1 = v1x * (weight1 - weight2) / (weight1 + weight2) + v2x * (2 * weight2) / (weight1 + weight2) + v1y;
            velocity2 = v1x * (2 * weight1) / (weight1 + weight2) + v2x * (weight2 - weight1) / (weight1 + weight2) + v2y;
        }


        public static Vector3 midpoint(Vector3 position1, Vector3 position2)
        {
            Vector3 middle;
            middle.X = (position1.X + position2.X) / 2;
            middle.Y = (position1.Y + position2.Y) / 2;
            middle.Z = (position1.Z + position2.Z) / 2;

            return middle;
        }
    }
    public class animation
    {
        Texture2D image;
        public Vector3 position;
        public Rectangle rect;
        Rectangle frame_rect;
        Vector2 origin;
        public float rotation = 0;
        public Color colour = Color.White;
        public float size;
        public Boolean visible;
        int framespersecond;
        int frames;
        int rows;
        int columns;
        public int frameposition;
        int framewidth;
        int frameheight;
        float timegone;
        Boolean loop = false;
        int noofloops = 0;
        int loopsdone = 0;

        public animation() { }
        public animation(ContentManager content, string spritename, int x, int y, float msize, Color mcolour, Boolean mvis, int fps, int nrows, int ncols)
        {
            image = content.Load<Texture2D>(spritename);
            position = new Vector3((float)x, (float)y, 0);
            rect.X = x;
            rect.Y = y;
            size = msize;
            colour = mcolour;
            visible = mvis;
            framespersecond = fps;
          
            rows = nrows;
            columns = ncols;
            frames = rows * columns;
            framewidth = (int)(image.Width / columns);
            frameheight = (int)(image.Height / rows);
            rect.Width = (int)(framewidth * size);
            rect.Height = (int)(frameheight * size);
            frame_rect.Width = framewidth;
            frame_rect.Height = frameheight;
            origin.Y = frameheight / 2;
            origin.X = framewidth / 2;
        }
        public void start(Vector3 pos, float rot, Boolean loopit, int repeatnumber)
        {
            rect.X = (int)pos.X;
            rect.Y = (int)pos.Y;

            loop = loopit;
            noofloops = repeatnumber;
            rotation = rot;
            visible = true;
            frameposition = 0;
            loopsdone = 0;
            timegone = 0;
        }
        public void update(float gtime)
        {
            if (visible)
            {
                frameposition = (int)(timegone / (1000 / framespersecond));
                timegone += gtime;

                if (frameposition >= frames)
                {
                    if (loop || loopsdone < noofloops)
                    {
                        loopsdone++;
                        frameposition = 0;
                        timegone = 0;
                    }
                    else
                        visible = false;
                }
            }
        }
        public void drawme(ref SpriteBatch sbatch)
        {
            if (visible)
            {
                frame_rect.Y = ((int)(frameposition / columns)) * frameheight;
                frame_rect.X = (frameposition - ((int)(frameposition / columns)) * columns) * framewidth;
                sbatch.Draw(image, rect, frame_rect, colour, rotation, origin, SpriteEffects.None, 0);
            }
        }
    }
}
