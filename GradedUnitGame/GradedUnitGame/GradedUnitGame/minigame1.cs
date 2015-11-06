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
    public class minigame1
    {
        object3d runner;
        camera cam = new camera(0,-250,250);
        float timer = 0;

        public void LoadContent()
        {
            // this is all you need to do to if your 3d model is static, or all the meshes have the pivot at the origin, or your 3d package exports pivot points.
            runner = new object3d(globals.Content.Load<Model>("runner"));

            // if your 3d package doesn't export pivot points properly, and you rotate bones using them, and they dont pivot round the origin, you will need to manually set them up like this:
            runner.SetPivotPoint("ShinL", new Vector3(0, 0, 20));
            runner.SetPivotPoint("ShinR", new Vector3(0, 0, 20));
            runner.SetPivotPoint("LowerArmL", new Vector3(0, 0, 20));
            runner.SetPivotPoint("LowerArmR", new Vector3(0, 0, 20));
            runner.SetPivotPoint("Neck", new Vector3(0, 0, 45));
            runner.SetPivotPoint("ShoulderL", new Vector3(0, 0, 45));
            runner.SetPivotPoint("ShoulderR", new Vector3(0, 0, 45));
            
            // if your 3d model does not include textures, you can add them to individual meshes like this:
            Texture2D tex1 = globals.Content.Load<Texture2D>("tex");
            Texture2D tex2 = globals.Content.Load<Texture2D>("tex2");
            runner.SetTexture("ShinRMesh", tex1);
            runner.SetTexture("ShinLMesh", tex1);
            runner.SetTexture("ThighRMesh", tex1);
            runner.SetTexture("ThighLMesh", tex1);
            runner.SetTexture("ArmUpperLMesh", tex1);
            runner.SetTexture("ArmUpperRMesh", tex1);
            runner.SetTexture("ArmLowerLMesh", tex1);
            runner.SetTexture("ArmLowerRMesh", tex1);
            runner.SetTexture("NeckMesh", tex1);
            runner.SetTexture("HEIDMesh", tex1);
            runner.SetTexture("FootLMesh", tex2);
            runner.SetTexture("FootRMesh", tex2);
            runner.SetTexture("BodyMesh", tex2);
            runner.SetTexture("ShoulderLMesh", tex2);
            runner.SetTexture("ShoulderRMesh", tex2);
            runner.rotation.Z = (float)Math.PI / 2;
        }

        float runspeed = 4;
        float verticalVelocity = 0;
        public void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) runner.rotation.Z += 0.1f;
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) runner.rotation.Z -= 0.1f;
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) 
            {
                timer += 12f;
                runner.position.X += (float)Math.Sin(runner.rotation.Z) * runspeed;
                runner.position.Y -= (float)Math.Cos(runner.rotation.Z) * runspeed;
            }
            if(Keyboard.GetState().IsKeyDown(Keys.Space) && runner.position.Z <=0) verticalVelocity = 10;
            runner.position.Z += verticalVelocity;
            
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                globals.currentState = gamestates.menu;
            if (timer < 100)
            {
                // lift left leg
                float movement = timer / 100;
                runner.SetBoneRotationTo("ThighL", new Vector3(0, 0, movement));
                runner.SetBoneRotationTo("ShinL", new Vector3(movement*2, 0, 0));
                runner.SetBoneRotationTo("ThighR", new Vector3(0, 0, movement));
                runner.SetBoneRotationTo("ShinR", new Vector3(movement, 0, 0));
                runner.SetBoneRotationTo("UpperArmR", new Vector3(0, 0, movement));
                runner.SetBoneRotationTo("UpperArmL", new Vector3(0, 0, movement));
            }
            else if (timer < 200)
            {
                // lower left leg
                float movement = 2 - timer / 100;
                runner.SetBoneRotationTo("ThighL", new Vector3(0, 0, movement));
                runner.SetBoneRotationTo("ShinL", new Vector3(movement*2, 0, 0));
                runner.SetBoneRotationTo("ThighR", new Vector3(0, 0, movement));
                runner.SetBoneRotationTo("ShinR", new Vector3(movement, 0, 0));
                runner.SetBoneRotationTo("UpperArmR", new Vector3(0, 0, movement));
                runner.SetBoneRotationTo("UpperArmL", new Vector3(0, 0, movement));
            }
            else if (timer < 300)
            {
                // lift right leg
                float movement = 2 - timer / 100;
                runner.SetBoneRotationTo("ThighL", new Vector3(0, 0, movement));
                runner.SetBoneRotationTo("ShinL", new Vector3(-movement, 0, 0));
                runner.SetBoneRotationTo("ThighR", new Vector3(0, 0, movement));
                runner.SetBoneRotationTo("ShinR", new Vector3(-movement*2, 0, 0));
                runner.SetBoneRotationTo("UpperArmR", new Vector3(0, 0, movement));
                runner.SetBoneRotationTo("UpperArmL", new Vector3(0, 0, movement));
            }
            else if (timer < 400)
            {
                // lift left leg
                float movement = -4 + timer / 100;
                runner.SetBoneRotationTo("ThighL", new Vector3(0, 0, movement));
                runner.SetBoneRotationTo("ShinL", new Vector3(-movement, 0, 0));
                runner.SetBoneRotationTo("ThighR", new Vector3(0, 0, movement));
                runner.SetBoneRotationTo("ShinR", new Vector3(-movement*2, 0, 0));
                runner.SetBoneRotationTo("UpperArmR", new Vector3(0, 0, movement));
                runner.SetBoneRotationTo("UpperArmL", new Vector3(0, 0, movement));
                //runner.SetBoneRotationTo("ArmUpperLMesh", new Vector3(0, 0, movement));
            }
            else timer = 0;

            if (runner.position.Z > 0)
            {
                verticalVelocity -= 1f;
                // jump bone settings
                runner.SetBoneRotationTo("ThighL", new Vector3(0, 0, 2.0f));
                runner.SetBoneRotationTo("ShinL", new Vector3(0.8f, 0, 0));
                runner.SetBoneRotationTo("ThighR", new Vector3(0, 0, -2.0f));
                runner.SetBoneRotationTo("ShinR", new Vector3(0.8f, 0, 0));
                runner.SetBoneRotationTo("UpperArmR", new Vector3(0, 0, 1.4f));
                runner.SetBoneRotationTo("UpperArmL", new Vector3(0, 0, -1.4f));
                runner.SetBoneRotationTo("LowerArmR", new Vector3(-1.1f, 0, 0));
                runner.SetBoneRotationTo("LowerArmL", new Vector3(-1.1f, 0, 0));
            }
            else
                verticalVelocity = 0;

            if (Keyboard.GetState().IsKeyDown(Keys.Z))
            {
                // taunt
                runner.SetBoneRotationTo("Spine", new Vector3(-0.25f, 0f, 0f));
                runner.SetBoneRotationTo("ThighL", new Vector3(0, 0, -0.05f));
                runner.SetBoneRotationTo("ShinL", new Vector3(0.0f, 0, 0));
                runner.SetBoneRotationTo("ThighR", new Vector3(0, 0, -0.05f));
                runner.SetBoneRotationTo("ShinR", new Vector3(0.0f, 0, 0));
                runner.SetBoneRotationTo("UpperArmR", new Vector3(0f, 0f, 0.5f));
                runner.SetBoneRotationTo("UpperArmL", new Vector3(0f, 0f, -0.5f));
                runner.SetBoneRotationTo("LowerArmR", new Vector3(0.3f, 0, 0));
                runner.SetBoneRotationTo("LowerArmL", new Vector3(0.3f, 0, 0));
            }
            else
            {
                runner.SetBoneRotationTo("Spine", new Vector3(0f, 0f, 0f));
            }
        }

        public void Draw(GameTime gameTime)
        {
            globals.graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            globals.graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            globals.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;  
            runner.Draw(cam);
        }
    }
}
