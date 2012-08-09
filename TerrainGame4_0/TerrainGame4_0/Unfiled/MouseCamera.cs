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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace TerrainGame4_0
{
    public class MouseCamera
    {
        GraphicsDevice device;
        ParentGame parentGame;

        public Matrix viewMatrix;

        public BoundingFrustum cameraFrustum = new BoundingFrustum(Matrix.Identity);

       // public Vector3 cameraPosition = new Vector3(168, 25, -150);
        public Vector3 cameraPosition = new Vector3(40, 25, -40);
        public float leftrightRot = MathHelper.TwoPi;
        public float updownRot = 0;//-MathHelper.Pi / 10.0f;  //<- use for light fov
        const float rotationSpeed = 0.3f;
        public float moveSpeed = 100.0f;
        MouseState originalMouseState;
        bool prevStateDRAW;

        public MouseCamera(ParentGame game)
        {
            this.device = game.GraphicsDevice;
            parentGame = game;
            
        }

        public void LoadContent()
        {
            UpdateViewMatrix();
            

            Mouse.SetPosition(device.Viewport.Width / 2, device.Viewport.Height / 2);
            originalMouseState = Mouse.GetState();

        }

        public void Update(GameTime gameTime)
        {
            float timeDifference = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;

            //if( prevStateDRAW != parentGame.DRAW_MODE )
                //Mouse.SetPosition(device.Viewport.Width / 2, device.Viewport.Height / 2);

            
            ProcessInput(timeDifference);

            //prevStateDRAW = parentGame.DRAW_MODE;
        }

        private void ProcessInput(float amount)
        {
            //if (!parentGame.DRAW_MODE)
            //{
                MouseState currentMouseState = Mouse.GetState();
                if (currentMouseState != originalMouseState)
                {
                    float xDifference = currentMouseState.X - originalMouseState.X;
                    float yDifference = currentMouseState.Y - originalMouseState.Y;


                    leftrightRot -= rotationSpeed * xDifference * amount;
                    updownRot -= rotationSpeed * yDifference * amount;
                    Mouse.SetPosition(device.Viewport.Width / 2, device.Viewport.Height / 2);


                    UpdateViewMatrix();
                }
            //}

            Vector3 moveVector = new Vector3(0, 0, 0);
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.Up))
                moveVector += new Vector3(0, 0, -1);
            if (keyState.IsKeyDown(Keys.Down))
                moveVector += new Vector3(0, 0, 1);
            if (keyState.IsKeyDown(Keys.Right))
                moveVector += new Vector3(1, 0, 0);
            if (keyState.IsKeyDown(Keys.Left))
                moveVector += new Vector3(-1, 0, 0);
            //if (keyState.IsKeyDown(Keys.Q))
            //    moveVector += new Vector3(0, 1, 0);
            //if (keyState.IsKeyDown(Keys.Z))
            //    moveVector += new Vector3(0, -1, 0);

           
            AddToCameraPosition(moveVector * amount);
        }

        private void AddToCameraPosition(Vector3 vectorToAdd)
        {
            Matrix cameraRotation = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);
            Vector3 rotatedVector = Vector3.Transform(vectorToAdd, cameraRotation);
            cameraPosition += moveSpeed * rotatedVector;
            
            UpdateViewMatrix();
        }

        private void UpdateViewMatrix()
        {
            Matrix cameraRotation = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);

            Vector3 cameraOriginalTarget = new Vector3(0, 0, -1);
            Vector3 cameraOriginalUpVector = new Vector3(0, 1, 0);

            Vector3 cameraRotatedTarget = Vector3.Transform(cameraOriginalTarget, cameraRotation);
            Vector3 cameraFinalTarget = cameraPosition + cameraRotatedTarget;

            Vector3 cameraRotatedUpVector = Vector3.Transform(cameraOriginalUpVector, cameraRotation);

            viewMatrix = Matrix.CreateLookAt(cameraPosition, cameraFinalTarget, cameraRotatedUpVector);
            parentGame.viewMatrix = viewMatrix;

            cameraFrustum.Matrix = viewMatrix * parentGame.projectionMatrix;
        }

    }
}
