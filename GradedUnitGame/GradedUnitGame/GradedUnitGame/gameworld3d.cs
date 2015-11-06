using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Audio;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
//using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Media;

namespace gameworld3d
{
    /// <summary>
    /// A light source with parallel rays of light, like from a very distant object such as the sun.
    /// </summary>
    struct directionalLightSource
    {
        public Vector3 diffuseColor;    // the main color of the light
        public Vector3 specularColor;   // highlight color
        public Vector3 direction;       // direction the light is shining in
    }

    /// <summary>
    /// A 3d camera that can be used when rendering 3d objects.
    /// </summary>
    class camera
    {
        public Vector3 position;
        public Vector3 lookAt;
        public float aspectratio;
        public float fov;
        public Vector2 ViewAreaAtFarPlane;
        public Vector2 ViewAreaAtFocalPlane;
        public Vector2 ViewAreaAtNearPlane;
        public Vector3 WhichWayIsUp;
        private float nearplane;
        private float farplane;
        private float focalplane;

        public float farPlane
        {
            set
            {
                farplane = value;
                ViewAreaAtFarPlane = ViewAreaAt(farplane);
            }

            get
            {
                return farplane;
            }
        }

        public float focalPlane
        {
            set
            {
                focalplane = value;
                ViewAreaAtFocalPlane = ViewAreaAt(focalplane);
            }

            get
            {
                return focalplane;
            }
        }

        public float nearPlane
        {
            set
            {
                nearplane = value;
                ViewAreaAtNearPlane = ViewAreaAt(nearplane);
            }

            get
            {
                return nearplane;
            }
        }
        
        /// <summary>
        /// Find the on-screen X and Y area, in 3d units, at the specified distance from the camera.
        /// </summary>
        /// <param name="distance">The distance from the camera.</param>
        /// <returns>A Vector with X and Y values that represent the maximum distance along the X and Y axis that an object can move and still be visible at the specified distance from the camera.  These values represent one side of the screen; negative values represent the other side.</returns>
        public Vector2 ViewAreaAt(float distance)
        {
            Vector2 area;
            area.Y = (int)(distance * (Math.Tan(MathHelper.ToRadians(fov / 2))));
            area.X = area.Y * aspectratio;
            return area;
        }

        /// <summary>
        /// Create a camera Field Of Vision matrix with the current camera's settings.
        /// </summary>
        /// <returns></returns>
        public Matrix FOVMatrix()
        {
            return Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fov), aspectratio, nearplane, farplane);
        }

        /// <summary>
        /// Create a camera Look-At matrix with the current camera's settings.
        /// </summary>
        /// <returns></returns>
        public Matrix LookMatrix()
        {
            return Matrix.CreateLookAt(position, lookAt, WhichWayIsUp);
        }

        /// <summary>
        /// Create a camera with all default settings.
        /// </summary>
        public camera()
        {
            aspectratio = 4 / 3;
            fov = 45;
            WhichWayIsUp = Vector3.Up;
            nearplane = 1f;
            farplane = 1000f;
            ViewAreaAtFarPlane = ViewAreaAt(farplane);
            focalplane = 500f;
            ViewAreaAtFocalPlane = ViewAreaAt(focalplane);
        }

        /// <summary>
        /// Create a camera at a 3d co-ordinate, but with all other settings at default.
        /// </summary>
        /// <param name="initialPosition">The initial co-ordinate as a vector.</param>
        public camera(Vector3 initialPosition)
        {
            position = initialPosition;
            aspectratio = 4 / 3;
            fov = 45;
            WhichWayIsUp = Vector3.Up;
            nearplane = 1f;
            farplane = 1000f;
            ViewAreaAtFarPlane = ViewAreaAt(farplane);
            focalplane = 500f;
            ViewAreaAtFocalPlane = ViewAreaAt(focalplane);
        }

        /// <summary>
        /// Create a camera at a 3d co-ordinate, but with all other settings at default.
        /// </summary>
        /// <param name="initialX">The X co-ordinate.</param>
        /// <param name="initialY">The Y co-ordinate.</param>
        /// <param name="initialZ">The Z co-ordinate.</param>
        public camera(float initialX, float initialY, float initialZ)
        {
            position = new Vector3(initialX, initialY, initialZ);
            aspectratio = 4 / 3;
            fov = 45;
            WhichWayIsUp = Vector3.Up;
            nearplane = 1f;
            farplane = 1000f;
            ViewAreaAtFarPlane = ViewAreaAt(farplane);
            focalplane = 500f;
            ViewAreaAtFocalPlane = ViewAreaAt(focalplane);
        }

        /// <summary>
        /// Create a camera, specifying initial settings except for the near plane which will default to 1 unit.
        /// </summary>
        /// <param name="initialPosition">The camera's initial co-ordinates in the 3d world.</param>
        /// <param name="lookat">The 3d co-ordinate that the camera should point at initially.</param>
        /// <param name="w_width">The width of the viewport the camera will render onto</param>
        /// <param name="w_height">The height of the viewport the camera will render onto</param>
        /// <param name="FOV">The Field Of Vision of the viewport, in degrees</param>
        /// <param name="camorient">The orientation of the camera - in other words, which direction is considered 'Up'.</param>
        /// <param name="cameradistance">The distance which will be used with 'viewarea'.</param>
        /// <param name="farplanedistance">The maximum distance to render 3d objects at.</param>
        public camera(Vector3 initialPosition, Vector3 lookat, float w_width, float w_height, int FOV, Vector3 camorient, float cameradistance, float farplanedistance)
        {
            position = initialPosition;
            lookAt = lookat;
            aspectratio = w_width / w_height;
            fov = FOV;
            WhichWayIsUp = camorient;
            nearPlane = 1f;
            farPlane = farplanedistance;
            focalPlane = cameradistance;
        }

        /// <summary>
        /// Set camera for a first or third person viewpoint
        /// </summary>
        /// <param name="character_position"></param>
        /// <param name="character_direction"></param>
        /// <param name="character_velocity"></param>
        /// <param name="howfarbehind"></param>
        /// <param name="distanceinfront"></param>
        /// <param name="distanceabove1"></param>
        /// <param name="distanceabove2"></param>
        public void setFPor3P(Vector3 character_position, Vector3 character_direction, Vector3 character_velocity, float howfarbehind, float distanceinfront, float distanceabove1, float distanceabove2)
        {
            character_velocity.Y = 0;
            position = character_position - ((character_direction * howfarbehind) + character_velocity * 3);
            position.Y += distanceabove1;
            lookAt = character_position + ((character_direction * distanceinfront) + character_velocity * 1);
            lookAt.Y += distanceabove2;
            focalPlane = howfarbehind + distanceinfront;
            WhichWayIsUp = Vector3.Up;
        }

        /// <summary>
        ///  Set overhead view of main game character
        /// </summary>
        /// <param name="character_position"></param>
        /// <param name="distanceabove"></param>
        public void setoverhead(Vector3 character_position, float distanceabove)
        {
            lookAt = character_position;
            position = character_position;
            position.Y = distanceabove;
            focalPlane = distanceabove;
            WhichWayIsUp = Vector3.Left;
        }

        /// <summary>
        /// Set camera for a side on view of the main game character
        /// </summary>
        /// <param name="character_position"></param>
        /// <param name="character_rotation"></param>
        /// <param name="distancefromcharacter"></param>
        /// <param name="distanceabove"></param>
        public void setsideon(Vector3 character_position, Vector3 character_rotation, float distancefromcharacter, float distanceabove)
        {
            Vector3 camdirection = new Vector3(0, 0, 0);
            camdirection.Z = (float)(Math.Cos(character_rotation.Y + MathHelper.ToRadians(90)));
            camdirection.X = (float)(Math.Sin(character_rotation.Y + MathHelper.ToRadians(90)));
            lookAt = character_position;
            position = character_position - (camdirection * distancefromcharacter);
            position.Y += distanceabove;
            focalPlane = distancefromcharacter;
            WhichWayIsUp = Vector3.Up;
        }


    }

    class object3d
    {
        //public bool visible=true;
        private Model objectModel;
        public Matrix[] transforms;
        private Matrix[] originaltransforms;
        public Vector3 rotation;
        public Vector3 position;
        public float scale=1;
        public Vector3[] pivotPoints;
        //public float collisionscale=1;
        //public Vector3 velocity;
        //public BoundingSphere bsphere;

        public Model ObjectModel
        {
            set
            {
                objectModel = value;
                originaltransforms = new Matrix[objectModel.Bones.Count];
                objectModel.CopyBoneTransformsTo(originaltransforms);
                transforms = new Matrix[objectModel.Bones.Count];           // probably not necessary but
                objectModel.CopyAbsoluteBoneTransformsTo(transforms);       // better safe than sorry
                pivotPoints = new Vector3[objectModel.Bones.Count];
            }

            get
            {
                return objectModel;
            }
        }

        public object3d()
        {
        }

        public object3d(Model modelToUse)
        {
            objectModel = modelToUse;
            originaltransforms = new Matrix[objectModel.Bones.Count];
            objectModel.CopyBoneTransformsTo(originaltransforms);
            transforms = new Matrix[objectModel.Bones.Count];           // probably not necessary but
            objectModel.CopyAbsoluteBoneTransformsTo(transforms);       // better safe than sorry
            pivotPoints = new Vector3[objectModel.Bones.Count];
        }

        public void RotateBone(string boneName, Vector3 rotation)
        {
            if (objectModel.Bones[boneName] != null)
            {
                objectModel.Bones[boneName].Transform *= Matrix.CreateTranslation(pivotPoints[objectModel.Bones[boneName].Index]);
                objectModel.Bones[boneName].Transform *= Matrix.CreateRotationX(rotation.X);
                objectModel.Bones[boneName].Transform *= Matrix.CreateRotationY(rotation.Y);
                objectModel.Bones[boneName].Transform *= Matrix.CreateRotationZ(rotation.Z);
                objectModel.Bones[boneName].Transform *= Matrix.CreateTranslation(-pivotPoints[objectModel.Bones[boneName].Index]);
            }
        }

        public void SetBoneRotationTo(string boneName, Vector3 rotation)
        {
            if (objectModel.Bones[boneName] != null)
            {
                objectModel.Bones[boneName].Transform = originaltransforms[objectModel.Bones[boneName].Index];
                objectModel.Bones[boneName].Transform *= Matrix.CreateTranslation(pivotPoints[objectModel.Bones[boneName].Index]);
                objectModel.Bones[boneName].Transform *= Matrix.CreateRotationX(rotation.X);
                objectModel.Bones[boneName].Transform *= Matrix.CreateRotationY(rotation.Y);
                objectModel.Bones[boneName].Transform *= Matrix.CreateRotationZ(rotation.Z);
                objectModel.Bones[boneName].Transform *= Matrix.CreateTranslation(-pivotPoints[objectModel.Bones[boneName].Index]);
            }
        }

        public void SetPivotPoint(string boneName, Vector3 pivotLocation)
        {
            pivotPoints[objectModel.Bones[boneName].Index] = pivotLocation;
        }

        public void SetTexture(string meshname, Texture2D textureToUse)
        {
            try
            {
                objectModel.Meshes[meshname].MeshParts[0].Effect = new BasicEffect(objectModel.Meshes[meshname].MeshParts[0].Effect.GraphicsDevice);
            }
            catch (KeyNotFoundException ex)
            {
                throw new ArgumentException("Invalid mesh name: " + meshname);
            }
            BasicEffect e = (BasicEffect)objectModel.Meshes[meshname].MeshParts[0].Effect;
            e.Texture = textureToUse;
            
            if(textureToUse!=null)
                e.TextureEnabled = true;
        }

        public void Draw(camera cam)
        {
            transforms = new Matrix[objectModel.Bones.Count];
            objectModel.CopyAbsoluteBoneTransformsTo(transforms);
            int i=0;
            foreach (ModelMesh mesh in objectModel.Meshes)               // loop through the mesh in the 3d model, drawing each one in turn.
            {
                i++;
                foreach (BasicEffect effect in mesh.Effects)                // This loop then goes through every effect in each mesh.
                {
                    // The following effects allow the object to be drawn in the correct place, with the correct rotation and scale.
                    effect.World = transforms[mesh.ParentBone.Index];       // begin dealing with transforms to render the object into the game world
                    effect.World *= Matrix.CreateScale(scale);              // scale the mesh to the right size
                    effect.World *= Matrix.CreateRotationX(rotation.X);     // rotate the mesh
                    effect.World *= Matrix.CreateRotationY(rotation.Y);     // rotate the mesh
                    effect.World *= Matrix.CreateRotationZ(rotation.Z);     // rotate the mesh
                    effect.World *= Matrix.CreateTranslation(position);     // position the mesh in the game world
                    
                    // This sets the FOV (Field of View) of the camera. The first parameter is the angle for the FOV, the 2nd is the aspect ratio of your game, 
                    // the 3rd is the nearplane distance from the camera and the last paramter is the farplane distance from the camera.
                    effect.Projection = cam.FOVMatrix();

                    // This effect sets the View Matrix, which determines how the camera sees the object.
                    effect.View = cam.LookMatrix();                        // Set the position of the camera and tell it what to look at

                    // the following effects are related to lighting and texture settings, feel free to tweak them to see what happens.
                    effect.LightingEnabled = true;
                    effect.EnableDefaultLighting();                               // default lighting is 'easymode' lighting, looks nice but isnt custom configured.
                    //effect.Alpha = 50.0f;                                          // amount of transparency
                    //effect.AmbientLightColor = new Vector3(0.25f);                  // fills in dark areas with a small amount of light
                    //effect.DiffuseColor = new Vector3(0.1f);                            // Diffuse is the standard colour method
                    //effect.DirectionalLight0.Enabled = true;                        // allows a directional light
                    //effect.DirectionalLight0.DiffuseColor = light.diffuseColor;     // the directional light's main colour
                    //effect.DirectionalLight0.SpecularColor = light.specularColor;   // the directional light's colour used for highlights
                    //effect.DirectionalLight0.Direction = light.direction;           // the direction of the light
                    //effect.EmissiveColor = new Vector3(0.15f);
                    //effect.FogColor = Vector3.Zero;
                    //effect.FogEnabled = false;
                    //effect.PreferPerPixelLighting = true;                         // Makes it shiner and reflects light better
                    //effect.SpecularColor = Vector3.One;
                    //effect.SpecularPower = 200.0f;
                    //effect.Texture = null;
                    //effect.TextureEnabled = true;
                }

                mesh.Draw(); // draw the current mesh using the effects.
            }
        }
    
        public void Draw(camera cam, directionalLightSource light)
        {
            transforms = new Matrix[objectModel.Bones.Count];
            objectModel.CopyAbsoluteBoneTransformsTo(transforms);
            int i = 0;
            foreach (ModelMesh mesh in objectModel.Meshes)               // loop through the mesh in the 3d model, drawing each one in turn.
            {
                i++;
                foreach (BasicEffect effect in mesh.Effects)                // This loop then goes through every effect in each mesh.
                {
                    // The following effects allow the object to be drawn in the correct place, with the correct rotation and scale.
                    effect.World = transforms[mesh.ParentBone.Index];       // begin dealing with transforms to render the object into the game world
                    effect.World *= Matrix.CreateScale(scale);              // scale the mesh to the right size
                    effect.World *= Matrix.CreateRotationX(rotation.X);     // rotate the mesh
                    effect.World *= Matrix.CreateRotationY(rotation.Y);     // rotate the mesh
                    effect.World *= Matrix.CreateRotationZ(rotation.Z);     // rotate the mesh
                    effect.World *= Matrix.CreateTranslation(position);     // position the mesh in the game world
                    
                    // This sets the FOV (Field of View) of the camera. The first parameter is the angle for the FOV, the 2nd is the aspect ratio of your game, 
                    // the 3rd is the nearplane distance from the camera and the last paramter is the farplane distance from the camera.
                    effect.Projection = cam.FOVMatrix();

                    // This effect sets the View Matrix, which determines how the camera sees the object.
                    effect.View = cam.LookMatrix();                        // Set the position of the camera and tell it what to look at

                    // the following effects are related to lighting and texture settings, feel free to tweak them to see what happens.
                    effect.LightingEnabled = true;
                    //effect.EnableDefaultLighting();                               // default lighting is 'easymode' lighting, looks nice but isnt custom configured.
                    //effect.Alpha = 50.0f;                                          // amount of transparency
                    effect.AmbientLightColor = new Vector3(0.25f);                  // fills in dark areas with a small amount of light
                    effect.DiffuseColor = new Vector3(0.1f);                            // Diffuse is the standard colour method
                    effect.DirectionalLight0.Enabled = true;                        // allows a directional light
                    effect.DirectionalLight0.DiffuseColor = light.diffuseColor;     // the directional light's main colour
                    effect.DirectionalLight0.SpecularColor = light.specularColor;   // the directional light's colour used for highlights
                    effect.DirectionalLight0.Direction = light.direction;           // the direction of the light
                    effect.EmissiveColor = new Vector3(0.15f);
                    //effect.FogColor = Vector3.Zero;
                    //effect.FogEnabled = false;
                    //effect.PreferPerPixelLighting = true;                         // Makes it shiner and reflects light better
                    //effect.SpecularColor = Vector3.One;
                    //effect.SpecularPower = 200.0f;
                    //effect.Texture = null;
                    //effect.TextureEnabled = true;
                }

                mesh.Draw(); // draw the current mesh using the effects.
            }
        }
    }
}
