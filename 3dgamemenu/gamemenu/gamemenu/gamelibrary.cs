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
    //public class camera
    //{
    //    public Vector3 position;
    //    public Vector3 lookAt;
    //    public float aspectratio;
    //    public float fov;
    //    public Vector2 ViewAreaAtFarPlane;
    //    public Vector2 ViewAreaAtFocalPlane;
    //    public Vector2 ViewAreaAtNearPlane;
    //    public Vector3 WhichWayIsUp;
    //    private float nearplane;
    //    private float farplane;
    //    private float focalplane;

    //    public Matrix getview()
    //    {
    //        return Matrix.CreateLookAt(position, lookAt, WhichWayIsUp);    // Set the position of the camera and tell it what to look at
    //    }

    //    public Matrix getproject()
    //    {
    //        return Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fov), aspectratio, nearPlane, farPlane);
    //    }

    //    public float farPlane
    //    {
    //        set
    //        {
    //            farplane = value;
    //            ViewAreaAtFarPlane = ViewAreaAt(farplane);
    //        }

    //        get
    //        {
    //            return farplane;
    //        }
    //    }

    //    public float focalPlane
    //    {
    //        set
    //        {
    //            focalplane = value;
    //            ViewAreaAtFocalPlane = ViewAreaAt(focalplane);
    //        }

    //        get
    //        {
    //            return focalplane;
    //        }
    //    }

    //    public float nearPlane
    //    {
    //        set
    //        {
    //            nearplane = value;
    //            ViewAreaAtNearPlane = ViewAreaAt(nearplane);
    //        }

    //        get
    //        {
    //            return nearplane;
    //        }
    //    }

    //    public Vector2 ViewAreaAt(float distance)
    //    {
    //        Vector2 area = new Vector2(0, 0);
    //        area.Y = (int)(distance * Math.Tan(MathHelper.ToRadians(fov / 2)));
    //        area.X = area.Y * aspectratio;
    //        return area;
    //    }

    //    public Matrix FOVMatrix()
    //    {
    //        return Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fov), aspectratio, nearplane, farplane);
    //    }

    //    public Matrix LookMatrix()
    //    {
    //        return Matrix.CreateLookAt(position, lookAt, WhichWayIsUp);
    //    }

    //    public camera() { }

    //    public camera(Vector3 initialPosition, Vector3 lookat, float w_width, float w_height, int FOV, Vector3 camorient, float cameradistance, float farplanedistance)
    //    {
    //        position = initialPosition;
    //        lookAt = lookat;
    //        aspectratio = w_width / w_height;
    //        fov = FOV;
    //        WhichWayIsUp = camorient;
    //        nearPlane = 1f;
    //        farPlane = farplanedistance;
    //        focalPlane = cameradistance;
    //    }

    //    // Set camera for a first or third person viewpoint
    //    public void setFPor3P(Vector3 character_position, Vector3 character_direction, Vector3 character_velocity, float howfarbehind, float distanceinfront, float distanceabove1, float distanceabove2)
    //    {
    //        character_velocity.Y = 0;
    //        position = character_position - ((character_direction * howfarbehind) + character_velocity * 3);
    //        position.Y += distanceabove1;
    //        lookAt = character_position + ((character_direction * distanceinfront) + character_velocity * 1);
    //        lookAt.Y += distanceabove2;
    //        focalPlane = howfarbehind + distanceinfront;
    //        WhichWayIsUp = Vector3.Up;
    //    }

    //    // Set overhead view of main game character
    //    public void setoverhead(Vector3 character_position, float distanceabove)
    //    {
    //        lookAt = character_position;
    //        position = character_position;
    //        position.Y = distanceabove;
    //        focalPlane = distanceabove;
    //        WhichWayIsUp = Vector3.Left;
    //    }

    //    // Set camera for a side on view of the main game character
    //    public void setsideon(Vector3 character_position, Vector3 character_rotation, float distancefromcharacter, float distanceabove)
    //    {
    //        Vector3 camdirection = new Vector3(0, 0, 0);
    //        camdirection.Z = (float)(Math.Cos(character_rotation.Y + MathHelper.ToRadians(90)));
    //        camdirection.X = (float)(Math.Sin(character_rotation.Y + MathHelper.ToRadians(90)));
    //        lookAt = character_position;
    //        position = character_position - (camdirection * distancefromcharacter);
    //        position.Y += distanceabove;
    //        focalPlane = distancefromcharacter;
    //        WhichWayIsUp = Vector3.Up;
    //    }

    //}

   
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
        public void drawme(ref  SpriteBatch sbatch, Vector3 newpos)
        {
            if (visible)
            {
                Rectangle newrect = rect;
                newrect.Y = (int)newpos.Y;
                newrect.X = (int)newpos.X;

                sbatch.Draw(image, newrect, null, colour, rotation, origin, SpriteEffects.None, 0);
            }
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


        // Reset graphics device for 3D drawing
        public static void resetgraphics(GraphicsDevice graphics)
        {
            // These lines reset the graphics device for drawing 3D
            graphics.BlendState = BlendState.Opaque;
            graphics.DepthStencilState = DepthStencilState.Default;
            graphics.SamplerStates[0] = SamplerState.LinearWrap;
        }

/*        // This method draws a 3D model
        public static void drawmesh(Vector3 position, Vector3 rotation, float scale, Model graphic, Matrix[] transforms, Vector3 orient,Boolean lightson,Vector3 camerapos, Vector3 cameralookat,
            float fov, float aspectrat)
        {
            // Quaternion rot = Quaternion.Normalize(Quaternion.CreateFromAxisAngle(Vector3.Up, rotation.Y) * Quaternion.CreateFromAxisAngle(Vector3.Right, rotation.X)  * Quaternion.CreateFromAxisAngle(Vector3.Backward, rotation.Z));
            foreach (ModelMesh mesh in graphic.Meshes)               // loop through the mesh in the 3d model, drawing each one in turn.
            {
                foreach (BasicEffect effect in mesh.Effects)                // This loop then goes through every effect in each mesh.
                {

                    if (lightson) effect.EnableDefaultLighting();           // Enables default lighting when lightson==TRUE, this can do funny things with textured on 3D models.
                    effect.PreferPerPixelLighting = true;                   // Makes it shiner and reflects light better
                    effect.World = transforms[mesh.ParentBone.Index];       // begin dealing with transforms to render the object into the game world
                    effect.World *= Matrix.CreateScale(scale);              // scale the mesh to the right size
                    effect.World *= Matrix.CreateRotationX(rotation.X);     // rotate the mesh
                    effect.World *= Matrix.CreateRotationY(rotation.Y);     // rotate the mesh
                    effect.World *= Matrix.CreateRotationZ(rotation.Z);     // rotate the mesh
                    //effect.World *= Matrix.CreateFromQuaternion(rot);     // Rotate the mesh using Quaternions (This may work better if you are rotating multiple axes)
                    //effect.World *= Matrix.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z); // rotate the mesh
                    effect.World *= Matrix.CreateTranslation(position);     // position the mesh in the game world

                    effect.View = Matrix.CreateLookAt(camerapos, cameralookat, orient);    // Set the position of the camera and tell it what to look at

                    // Sets the FOV (Field of View) of the camera. The first paramter is the angle for the FOV, the 2nd is the aspect ratio of your game, 
                    // the 3rd is the nearplane distance from the camera and the last paramter is the farplane distance from the camera.
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fov), aspectrat, 1f, 20000.0f);
                }
                mesh.Draw(); // draw the current mesh using the effects.
            }
        }
 */


        // This method draws a 3D model
        public static void drawmesh(Vector3 position, Vector3 rotation, float scale, Model graphic, Matrix[] transforms, Vector3 orient, Boolean lightson, Vector3 camerapos,
                               Vector3 camlookat, float fov, float aspectrat)
        {
            // Quaternion rot = Quaternion.Normalize(Quaternion.CreateFromAxisAngle(Vector3.Up, rotation.Y) * Quaternion.CreateFromAxisAngle(Vector3.Right, rotation.X)  * Quaternion.CreateFromAxisAngle(Vector3.Backward, rotation.Z));
            foreach (ModelMesh mesh in graphic.Meshes)               // loop through the mesh in the 3d model, drawing each one in turn.
            {
                foreach (BasicEffect effect in mesh.Effects)                // This loop then goes through every effect in each mesh.
                {
                    if (lightson) effect.EnableDefaultLighting();           // Enables default lighting when lightson==TRUE, this can do funny things with textured on 3D models.
                    effect.PreferPerPixelLighting = true;                   // Makes it shiner and reflects light better
                    effect.World = transforms[mesh.ParentBone.Index];       // begin dealing with transforms to render the object into the game world
                    effect.World *= Matrix.CreateScale(scale);              // scale the mesh to the right size
                    effect.World *= Matrix.CreateRotationX(rotation.X);     // rotate the mesh
                    effect.World *= Matrix.CreateRotationY(rotation.Y);     // rotate the mesh
                    effect.World *= Matrix.CreateRotationZ(rotation.Z);     // rotate the mesh
                    //effect.World *= Matrix.CreateFromQuaternion(rot);     // Rotate the mesh using Quaternions (This may work better if you are rotating multiple axes)
                    //effect.World *= Matrix.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z); // rotate the mesh
                    effect.World *= Matrix.CreateTranslation(position);     // position the mesh in the game world

                    effect.View = Matrix.CreateLookAt(camerapos, camlookat, orient);    // Set the position of the camera and tell it what to look at

                    // Sets the FOV (Field of View) of the camera. The first paramter is the angle for the FOV, the 2nd is the aspect ratio of your game, 
                    // the 3rd is the nearplane distance from the camera and the last paramter is the farplane distance from the camera.
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fov), aspectrat, 1f, 20000.0f);
                }
                mesh.Draw(); // draw the current mesh using the effects.
            }
        }


    }


    public class animation
    {
        Texture2D image;            // Texture which holds animation sheet
        public Vector3 position;    // Position of animation
        public Rectangle rect;      // Rectangle to hold size and position
        Rectangle frame_rect;       // Rectangle to hold position of frame to draw
        Vector2 origin;             // Centre point
        public float rotation = 0;  // Rotation amount
        public Color colour = Color.White; // Colour
        public float size;          // Size Ratio
        public Boolean visible;     // Should object be drawn true or false
        public int framespersecond; // Frame Rate
        int frames;                 // Number of frames of animation
        int rows;                   // Number of rows in the sprite sheet
        int columns;                // Number of columns in the sprite sheet
        int frameposition;          // Current position in the animation
        int framewidth;             // Width in pixels of each frame of animation
        int frameheight;            // Height in pixels of each frame of animation
        float timegone;             // Time since animation began
        public Boolean loop = false;// Should animation loop
        int noofloops = 0;          // Number of loops to do
        int loopsdone = 0;          // Number of loops completed
        public Boolean paused = false;  // Freeze frame animation

        public animation() { }

        // Constructor which initialises the animation
        public animation(ContentManager content, string spritename, int x, int y, float msize, Color mcolour, Boolean mvis, int fps, int nrows, int ncol, Boolean loopit)
        {
            image = content.Load<Texture2D>(spritename);    // Load image into texture
            position = new Vector3((float)x, (float)y, 0);  // Set position
            rect.X = x;                                     // Set position of draw rectangle x
            rect.Y = y;                                     // Set position of draw rectangle y
            size = msize;                                   // Store size ratio
            colour = mcolour;                               // Set colour
            visible = mvis;                                 // Image visible TRUE of FALSE? 
            framespersecond = fps;                          // Store frames per second
            rows = nrows;                                   // Number of rows in the sprite sheet
            columns = ncol;                                 // Number of columns in the sprite sheet
            frames = rows * columns;                          // Store no of frames
            framewidth = (int)(image.Width / columns);      // Calculate the width of each frame of animation
            frameheight = (int)(image.Height / rows);       // Calculate the heigh of each frame of animation
            rect.Width = (int)(framewidth * size);          // Set the new width based on the size ratio    
            rect.Height = (int)(frameheight * size);	    // Set the new height based on the size ratio
            frame_rect.Width = framewidth;                  // Set the width of each frame
            frame_rect.Height = frameheight;                // Set the height of each frame
            origin.X = framewidth / 2;                      // Set X origin to half of frame width
            origin.Y = frameheight / 2;              	    // Set Y origin to half of frame heigh
            loop = loopit;                                  // Should it be looped or not
        }

        public void start(Vector3 pos, float rot, int repeatnumber)
        {
            // Set position of object into the rectangle from the position Vector
            position = pos;
            rect.X = (int)position.X;
            rect.Y = (int)position.Y;

            // Start new animation
            noofloops = repeatnumber;
            rotation = rot;
            visible = true;
            frameposition = 0;
            timegone = 0;
            loopsdone = 0;
            paused = false;
        }

        public void update(float gtime)
        {
            if (framespersecond < 1) framespersecond = 1; // Error checking to avoid divide by zero

            if (visible && !paused)
            {
                frameposition = (int)(timegone / (1000 / framespersecond));   // Work out what frame the animation is on
                timegone += gtime;                                          // Time gone during the animation
                // Check if the animation is at the end
                if (frameposition >= frames)
                {
                    // Repeat animation if necessary
                    if (loop || loopsdone < noofloops)
                    {
                        loopsdone++;
                        frameposition = 0;
                        timegone = 0;
                    }
                    else
                    {
                        visible = false;   // End animation
                    }
                }
            }
        }

        // Use this method to draw the image
        public void drawme(ref SpriteBatch sbatch)
        {
            if (visible)
            {   // Work out the co-ordinates of the current frame and then draw that frame
                frame_rect.Y = ((int)(frameposition / columns)) * frameheight;
                frame_rect.X = (frameposition - ((int)(frameposition / columns)) * columns) * framewidth;
                sbatch.Draw(image, rect, frame_rect, colour, rotation, origin, SpriteEffects.None, 0);
            }
        }

        // Use this method to draw the image at a specified position
        public void drawme(ref SpriteBatch sbatch, Vector3 newpos)
        {
            if (visible)
            {
                Rectangle newrect = rect;
                newrect.X = (int)newpos.X;
                newrect.Y = (int)newpos.Y;

                frame_rect.Y = ((int)(frameposition / columns)) * frameheight;
                frame_rect.X = (frameposition - ((int)(frameposition / columns)) * columns) * framewidth;
                sbatch.Draw(image, newrect, frame_rect, colour, rotation, origin, SpriteEffects.None, 0);
            }
        }

        // Use this method to draw the image at a specified position and allow image to be flipped horizontally or vertically
        public void drawme(ref SpriteBatch sbatch, Vector3 newpos, Boolean flipx, Boolean flipy)
        {
            if (visible)
            {
                Rectangle newrect = rect;
                newrect.X = (int)newpos.X;
                newrect.Y = (int)newpos.Y;

                frame_rect.Y = ((int)(frameposition / columns)) * frameheight;
                frame_rect.X = (frameposition - ((int)(frameposition / columns)) * columns) * framewidth;
                if (flipx)
                    sbatch.Draw(image, newrect, frame_rect, colour, rotation, origin, SpriteEffects.FlipHorizontally, 0);
                else if (flipy)
                    sbatch.Draw(image, newrect, frame_rect, colour, rotation, origin, SpriteEffects.FlipVertically, 0);
                else
                    sbatch.Draw(image, newrect, frame_rect, colour, rotation, origin, SpriteEffects.None, 0);
            }
        }
    }
    public class staticmesh
    {
        public Model graphic;
        public Matrix[] transforms;
        public Vector3 position;
        public Vector3 rotation;
        public float size;
        public BoundingSphere bsphere;
        public BoundingBox bbox;
        public float radius;
        public Boolean visible=true;
        public float weight;

        public staticmesh() { }
        public staticmesh(ContentManager content, string modelname, float msize, Vector3 mpos, Vector3 mrot)
        {
            graphic = content.Load<Model>(modelname);
            transforms = new Matrix[graphic.Bones.Count];
            graphic.CopyAbsoluteBoneTransformsTo(transforms);
            size = msize;
            radius = graphic.Meshes[0].BoundingSphere.Radius * size;
            position = mpos;
            rotation = mrot;
            updateobject();

        }

        public void drawme(Vector3 orient, Boolean lightson, Vector3 cameraposition, Vector3 cameralookat, float fov, float aspectratio)
        {
            if (visible)
                sfunctions.drawmesh(position, rotation, size, graphic, transforms, orient, lightson, cameraposition, cameralookat, fov, aspectratio);
        }

        public void updateobject()
        {
            bsphere = new BoundingSphere(position, radius);
        }
    }
    public class model3d : staticmesh
    {
        public Vector3 velocity;
        public Vector3 direction;
        public float speed;
        public float rotspeed;
        public Vector3 oldposition;
        public Vector3 oldrotation;
        const float friction = 0.97f;
        const float airrestance = 0.9999f;
        const float gravity = 2f;
        public float power;

        public model3d() { }
        public model3d(ContentManager content, string modelname, float msize, Vector3 mpos, Vector3 mrot, float rspeed, float mpower, float mweight)
        {
            graphic = content.Load<Model>(modelname);
            transforms = new Matrix[graphic.Bones.Count];
            graphic.CopyAbsoluteBoneTransformsTo(transforms);
            size = msize;
            radius = graphic.Meshes[0].BoundingSphere.Radius * size;
            position = mpos;
            rotation = mrot;
            rotspeed = rspeed;
            power = mpower;
            weight = mweight;
            updateobject();
            visible = true;
        }

        public void moveme(GamePadState pad, float gtime, int groundpos)
        {
            if (visible)
            {
                rotation.Y -= pad.ThumbSticks.Right.X * rotspeed * (gtime);

                direction.Z = (float)(Math.Cos(rotation.Y));
                direction.X = (float)(Math.Sin(rotation.Y));

                float currentthrust = (float)(Math.Sqrt(Math.Pow(pad.ThumbSticks.Left.X, 2) + Math.Pow(pad.ThumbSticks.Left.Y, 2)));
                currentthrust *= power * (gtime / 17);

                float botdirection = (float)Math.Atan2(-pad.ThumbSticks.Left.X, pad.ThumbSticks.Left.Y);

                Vector3 stickdirection = new Vector3(0, 0, 0);
                stickdirection.Z = (float)(Math.Cos(rotation.Y + botdirection));
                stickdirection.X = (float)(Math.Sin(rotation.Y + botdirection));

                velocity += (stickdirection * currentthrust);

                if (position.Y > groundpos) velocity.Y -= gravity;
                if (position.Y < groundpos)
                {
                    position.Y = groundpos;
                    velocity.Y = 0;
                }
                if (velocity.Y == 0)
                    velocity *= friction * (gtime / 17);
                else
                    velocity *= airrestance * (gtime / 17);

                speed = (float)(Math.Sqrt(velocity.X * velocity.X + velocity.Y * velocity.Y));

                position += velocity;

            }
        }
    
        public void jump(float forceapplied, float gtime)
        {
            if (visible && velocity.Y == 0)
                velocity.Y += forceapplied * (gtime / 17);
        }
    }

}
