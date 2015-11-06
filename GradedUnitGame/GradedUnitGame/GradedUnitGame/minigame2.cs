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
    public class minigame2
    {
        object3d runner;
        object3d[] ground;
        object3d judge;
        camera cam = new camera();
        SpriteFont minigamefont;
        Texture2D bars;
        
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

            judge = new object3d(globals.Content.Load<Model>("pothead"));
            judge.position = new Vector3(500, 150, 0);
            Model groundmodel = globals.Content.Load<Model>("ground");
            ground = new object3d[20];
            for (int count = 0; count < ground.Count(); count++)
            {
                ground[count] = new object3d(groundmodel);
                ground[count].rotation.X = (float)Math.PI / 2;
                ground[count].scale = 2;
                ground[count].position.Z = -70;
                ground[count].position.Y = 40 + 250 * (int)(count/5);
                ground[count].position.X = -200 + 250 * (count%5);
            }
            minigamefont = globals.Content.Load<SpriteFont>("Verdana");
            bars = globals.Content.Load<Texture2D>("2d_bars");
        }

        public void reset()
        {
            runner.position = Vector3.Zero;
            cam.position = new Vector3(0, -250, 250);
            cam.lookAt = Vector3.Zero;
            runspeed = 0;
        }

        float runspeed;
        float verticalVelocity = 0;
        KeyboardState previousState;
        float timer = 0;
        float jumpStrength = 8;
        float gravity = 0.5f;

        public void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                globals.currentState = gamestates.menu;

            if (runner.position.X > judge.position.X && runner.position.Z <= 0)
            {
            }
            else
            {

                if (Keyboard.GetState().IsKeyDown(Keys.Z) && previousState.IsKeyUp(Keys.Z) ||
                    Keyboard.GetState().IsKeyDown(Keys.X) && previousState.IsKeyUp(Keys.X))
                {
                    runspeed += 0.05f;
                }
                runspeed *= 0.99f;
                timer += (gameTime.ElapsedGameTime.Milliseconds * runspeed);
                float distanceTravelled = runspeed * 8;
                runner.position.X += distanceTravelled;
                cam.position.X += distanceTravelled;
                cam.lookAt.X += distanceTravelled;
                previousState = Keyboard.GetState();

                if (Keyboard.GetState().IsKeyDown(Keys.Space) && runner.position.Z <= 0) verticalVelocity = jumpStrength;
                runner.position.Z += verticalVelocity;

                if (timer < 100)
                {
                    // lift left leg
                    float movement = timer / 100;
                    runner.SetBoneRotationTo("ThighL", new Vector3(0, 0, movement));
                    runner.SetBoneRotationTo("ShinL", new Vector3(movement * 2, 0, 0));
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
                    runner.SetBoneRotationTo("ShinL", new Vector3(movement * 2, 0, 0));
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
                    runner.SetBoneRotationTo("ShinR", new Vector3(-movement * 2, 0, 0));
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
                    runner.SetBoneRotationTo("ShinR", new Vector3(-movement * 2, 0, 0));
                    runner.SetBoneRotationTo("UpperArmR", new Vector3(0, 0, movement));
                    runner.SetBoneRotationTo("UpperArmL", new Vector3(0, 0, movement));
                    //runner.SetBoneRotationTo("ArmUpperLMesh", new Vector3(0, 0, movement));
                }
                else timer = 0;

                if (runner.position.Z > 0)
                {
                    verticalVelocity -= gravity;
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
            }
        }

        public void Draw(GameTime gameTime)
        {
            globals.graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            globals.graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            globals.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            runner.Draw(cam);
            judge.Draw(cam);
            for(int count=0;count<ground.Count();count++)
                ground[count].Draw(cam);
            globals.spriteBatch.Begin();
            if (runner.position.X > judge.position.X && runner.position.Z <= 0)
            {
                globals.spriteBatch.Draw(bars, new Rectangle(0, 0, 1024, 100), new Rectangle(0, 45, 254, 159 - 128), Color.LightCyan);
                globals.spriteBatch.DrawString(minigamefont, "Distance:" + (runner.position.X - judge.position.X) / 10 + "m.", new Vector2(300, 20), Color.Red);
            }
            globals.spriteBatch.Draw(bars, new Rectangle(0, 668, 1024, 100), new Rectangle(0, 126, 254, 159 - 128), Color.White);
            globals.spriteBatch.Draw(bars, new Rectangle(0, 668, (int)(1024 * runspeed), 100), new Rectangle(0, 85, (int)(254 * runspeed), 159 - 128), Color.White);
            globals.spriteBatch.End();
        }
    }
}
