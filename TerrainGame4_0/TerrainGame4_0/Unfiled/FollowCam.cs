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
    public class FollowCam
    {
        GraphicsDevice device;
        ParentGame parentGame;
        PlayerTank Player;

        public Matrix viewMatrix;

        public BoundingFrustum cameraFrustum = new BoundingFrustum(Matrix.Identity);
        
        public Vector3 cameraPosition = new Vector3(40, 25, -40);
        public float leftrightRot = MathHelper.TwoPi;
        public float updownRot = 0;
        const float rotationSpeed = 0.3f;
        public float moveSpeed = 100.0f;

        Vector3 NewPosition;
        public float NewUpDownRot;
        public float NewLeftRightRot;
        Matrix cameraRotation;
        Vector3 cameraOriginalTarget = new Vector3(0, 0, -1);
        Vector3 cameraOriginalUpVector = new Vector3(0, 1, 0);

        public bool Following = false;
        public Missile myMissile = null;

        public bool Won = false;

        public FollowCam(ParentGame game, PlayerTank player)
        {
            this.device = game.GraphicsDevice;
            parentGame = game;
            Player = player;
        }

        public void LoadContent()
        {
            UpdateViewMatrix();
        }

        public void Update(GameTime gameTime)
        {
            float timeDifference = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;

            if (Won)
                GetNewCameraWon(timeDifference);
            else if (Following)
                GetNewCameraMissile(myMissile);
            else
                GetNewCameraPlayer();

            SetCameraPosition();

            UpdateViewMatrix();
        }

        void GetNewCameraPlayer()
        {
            
            NewPosition = Player.position;
            NewPosition.Y += 10;
            NewLeftRightRot = ( Player.turretRotationValue - MathHelper.ToRadians(90) );
            NewUpDownRot = 0;
            Vector3 forwardVec = Matrix.CreateFromYawPitchRoll(NewLeftRightRot, 0f, 0f).Backward;
            forwardVec.Normalize();
            NewPosition += forwardVec * 30;
            
        }

        float wonRotation = 0;
        void GetNewCameraWon(float timeDifference)
        {
            wonRotation += timeDifference * 0.35f;

            NewPosition = Player.position;

            NewPosition.Y += 2;
            NewLeftRightRot = (Player.turretRotationValue - MathHelper.ToRadians(90) - wonRotation );
            NewUpDownRot = 3.14f / 10;
            Vector3 forwardVec = Matrix.CreateFromYawPitchRoll(NewLeftRightRot, 0, 0f).Backward;
            forwardVec.Normalize();
            NewPosition += forwardVec * 25;

        }

        public void GetNewCameraMissile(Missile Missile)
        {
            NewPosition = Missile.position;
            NewPosition.Y += 10;

            //NewLeftRightRot = (Player.turretRotationValue - MathHelper.ToRadians(90));
            
            //NewUpDownRot -= (0.2f) * (0.0166f);
            NewUpDownRot -= (0.00167f / Player.Power);
            NewUpDownRot = MathHelper.Clamp(NewUpDownRot, -0.9f, 0.9f);
            Vector3 forwardVec = Matrix.CreateFromYawPitchRoll(NewLeftRightRot, 0f, 0f).Backward;
            forwardVec.Normalize();
            NewPosition += forwardVec * 20;           

        }

        void SetCameraPosition()
        {
            float AMOUNT = 0.1f;
            
            if (NewLeftRightRot > (MathHelper.ToRadians(120) - MathHelper.ToRadians(90)) &&
                leftrightRot < (MathHelper.ToRadians(-120) - MathHelper.ToRadians(90)) )
                leftrightRot = MathHelper.ToRadians(360) + leftrightRot;

            if (NewLeftRightRot < (MathHelper.ToRadians(-120) - MathHelper.ToRadians(90)) &&
                leftrightRot > (MathHelper.ToRadians(120) - MathHelper.ToRadians(90)))
                leftrightRot = leftrightRot - MathHelper.ToRadians(360);

            updownRot = MathHelper.Lerp(updownRot, NewUpDownRot, AMOUNT);
            leftrightRot = MathHelper.Lerp(leftrightRot, NewLeftRightRot, AMOUNT);

            cameraRotation =
                Matrix.CreateRotationX(
                    updownRot)
                * Matrix.CreateRotationY(
                    leftrightRot);

            //Vector3 rotatedVector = Vector3.Transform(vectorToAdd, cameraRotation);

            cameraPosition = Vector3.Lerp(cameraPosition, NewPosition, AMOUNT);

            
        }

        //private void ProcessInput(float amount)
        //{
                        
        //    if (currentMouseState != originalMouseState)
        //    {
        //        float xDifference = currentMouseState.X - originalMouseState.X;
        //        float yDifference = currentMouseState.Y - originalMouseState.Y;


        //        leftrightRot -= rotationSpeed * xDifference * amount;
        //        updownRot -= rotationSpeed * yDifference * amount;
        //        Mouse.SetPosition(device.Viewport.Width / 2, device.Viewport.Height / 2);


        //        UpdateViewMatrix();
        //    }

        //    Vector3 moveVector = new Vector3(0, 0, 0);
        //    KeyboardState keyState = Keyboard.GetState();
        //    if (keyState.IsKeyDown(Keys.Up))
        //        moveVector += new Vector3(0, 0, -1);
        //    if (keyState.IsKeyDown(Keys.Down))
        //        moveVector += new Vector3(0, 0, 1);
        //    if (keyState.IsKeyDown(Keys.Right))
        //        moveVector += new Vector3(1, 0, 0);
        //    if (keyState.IsKeyDown(Keys.Left))
        //        moveVector += new Vector3(-1, 0, 0);


        //    AddToCameraPosition(moveVector * amount);
        //}

        //private void AddToCameraPosition(Vector3 vectorToAdd)
        //{
        //    Matrix cameraRotation = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);
        //    Vector3 rotatedVector = Vector3.Transform(vectorToAdd, cameraRotation);
        //    cameraPosition += moveSpeed * rotatedVector;

        //    UpdateViewMatrix();
        //}

        private void UpdateViewMatrix()
        {

            Vector3 cameraRotatedTarget = Vector3.Transform(cameraOriginalTarget, cameraRotation);
            Vector3 cameraFinalTarget = cameraPosition + cameraRotatedTarget;

            Vector3 cameraRotatedUpVector = Vector3.Transform(cameraOriginalUpVector, cameraRotation);

            viewMatrix = Matrix.CreateLookAt(cameraPosition, cameraFinalTarget, cameraRotatedUpVector);
            parentGame.viewMatrix = viewMatrix;

            cameraFrustum.Matrix = viewMatrix * parentGame.projectionMatrix;
        }

    }
}
