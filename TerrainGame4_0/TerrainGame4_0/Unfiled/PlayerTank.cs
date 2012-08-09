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
    public class PlayerTank : GameObject
    {
        KeyboardState keyboardState, prevKeyState;
        GameTime tempGameTime;

        GameScreen Screen;

        //ParticleEmitter emitter;

        public Missile[] missiles;
        int numMissiles = 15;
        public Model missileModel;
        public Texture2D[] missileTextures;

        float rotationSpeed = 1;

        public float Power = 0.5f;

        const float missilePower = 0.7f;
        const float launcherHeadMuzzleOffset = 5.0f;

        public ModelBone turretBone;
        public ModelBone cannonBone;
        public ModelBone baseBone;

        Matrix baseTransform;
        Matrix turretTransform;
        Matrix cannonTransform;

        public float turretRotationValue;
        public float cannonRotationValue;
        public float baseRotationValue;
        

        public CarObject mobileObject;


        public PlayerTank(ParentGame Game, Model ModelObj, Texture2D[] ModelTextures, Vector3 Position,
            Model MissileModel, Texture2D[] MissileTextures, GameScreen Screen) 
            : base( Game, ModelObj, ModelTextures )
        {
            this.Screen = Screen;
            //LoadPhysicsObject();
            missileModel = MissileModel;
            missileTextures = MissileTextures;
            position = Position;
        }



        public void Load()
        {
            base.LoadContent();
            
            //missiles = new Missile[numMissiles];
            
            //for (int i = 0; i < numMissiles; ++i)
            //{
            //    missiles[i] = new Missile(parentGame,
            //        missileModel,
            //        missileTextures);

            //    missiles[i].scale = 10.0f;
            //}

            //turretBone = model.Bones["turret_geo"];
            //cannonBone = model.Bones["canon_geo"];

            turretBone = model.Bones["turret"];
            cannonBone = model.Bones["cannon"];
            baseBone = model.Bones["base"];

            turretTransform = turretBone.Transform;
            cannonTransform = cannonBone.Transform;
            baseTransform = baseBone.Transform;            

            Console.WriteLine("MODEL: " + model.Meshes.Count + " : B: " + model.Bones.Count);
        }

        void LoadPhysicsObject()
        {
            mobileObject = new CarObject(parentGame, null, null,
                true,   // fw drive
                true,   // rw drive
                30.0f,  // max steer angle
                5.0f,   // steer rate
                4.7f,   // wheel side friction
                5.0f,   // wheel fwd friction
                0.20f,  // wheel travel
                0.6f,   // wheel radius
                0.05f,  // wheel z offset
                0.45f,  // wheel resting fract
                0.3f,   // wheel damping fract
                1,      // wheel num rays
                520.0f, // drive tourqe
                JigLibX.Physics.PhysicsSystem.CurrentPhysicsSystem.Gravity.Length()); // gravity

            mobileObject.Car.Chassis.Body.MoveTo(position, Matrix.Identity);
            mobileObject.Car.EnableCar();
            mobileObject.Car.Chassis.Body.AllowFreezing = false;

            parentGame.Components.Add(mobileObject);
        }

        override public void Update(float amount, GameTime gameTime)
        {
            tempGameTime = gameTime;

            //foreach(Missile missile in missiles)
            //    if (missile.alive)
            //        missile.Update(gameTime);


            //if( moving )
            //    parentGame.smokePlumeParticles.AddParticle(position, Vector3.Zero);

            

            
            baseRotationValue = MathHelper.ToRadians(-90);

            cannonRotationValue = MathHelper.Clamp(cannonRotationValue, MathHelper.ToRadians(-45), MathHelper.ToRadians(10));

            if (turretRotationValue < MathHelper.ToRadians(-180))
                turretRotationValue = MathHelper.ToRadians(360) + turretRotationValue;

            if (turretRotationValue > MathHelper.ToRadians(180))
                turretRotationValue = turretRotationValue - MathHelper.ToRadians(360);

            Matrix turretRotation = Matrix.CreateRotationZ(turretRotationValue);
            Matrix cannonRotation = Matrix.CreateRotationY(cannonRotationValue);
            Matrix baseRotation = Matrix.CreateRotationX(baseRotationValue);

            turretBone.Transform = turretRotation * turretTransform;
            cannonBone.Transform = cannonRotation * cannonTransform;
            baseBone.Transform = baseRotation * baseTransform;


            if (Power > 1)
                Power = 1f;
            else if (Power < 0.1f)
                Power = 0.1f;

            Power = (float)Math.Round(Power, 2);
        }

        void Reload()
        {
            AudioEmitter emitter;
            emitter = new AudioEmitter();
            emitter.Position = position;

            //Screen.cue = parentGame.soundBank.GetCue("reload");
            //Screen.cue.Apply3D(Screen.listener, emitter);
            //Screen.cue.Play();
        }
                

        public void Draw(GameTime gameTime)
        {
        }

        //override public void UpdateVelocity(float amount)
        //{
        //    HandleInput(amount);

        //    newPosition = position + velocity;
            
        //    // gravity affects after this call in base
        //}

        public void HandleInput(float amount)
        {
            prevKeyState = keyboardState;
            keyboardState = Keyboard.GetState();                        
           
            #region Accelerate

            //float FacingDirection = 0;

            Vector3 MoveDirection = new Vector3(0, 0, 0);

            //if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.S))
            //{
            //    if (keyboardState.IsKeyDown(Keys.W))
            //        mobileObject.Car.Accelerate = 1.0f;
            //    else
            //        mobileObject.Car.Accelerate = -1.0f;
            //}
            //else
            //    mobileObject.Car.Accelerate = 0.0f;

            //if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.D))
            //{
            //    if (keyboardState.IsKeyDown(Keys.A))
            //        mobileObject.Car.Steer = 1.0f;
            //    else
            //        mobileObject.Car.Steer = -1.0f;
            //}
            //else
            //    mobileObject.Car.Steer = 0.0f;

            //if (true)//(!inAir)
            //{
            //    if (keyState.IsKeyDown(Keys.W))
            //        MoveDirection += new Vector3(0, 0, 1);
            //    if (keyState.IsKeyDown(Keys.S))
            //        MoveDirection += new Vector3(0, 0, -1);

            //    if (keyState.IsKeyDown(Keys.D))
            //        FacingDirection -= rotationSpeed * amount;
            //    if (keyState.IsKeyDown(Keys.A))
            //        FacingDirection += rotationSpeed * amount;


                //Quaternion additionalRot = Quaternion.CreateFromAxisAngle(
                //   // new Vector3(0, 0, -1),
                //   Vector3.Up,
                //    FacingDirection);

                //rotationQ *= additionalRot;


                //objFacingMatrix = Matrix.CreateFromQuaternion(rotationQ);

                //Vector3 velocity = Vector3.Transform(MoveDirection * amount, objFacingMatrix);
                
                //prevVelocity = velocity * moveSpeed;
            //}

            #endregion // Accelerate


            //if (keyboardState.IsKeyDown(Keys.Space)
            //    && prevKeyState != keyboardState)
            //{
            //    FireMissile();
            //    //parentGame.AddExplosion(GetMissileMuzzlePosition(), 10, 80, 2000.0f, tempGameTime);
            //}


            #region turret
            if (keyboardState.IsKeyDown(Keys.Q)
                )
            {
                turretRotationValue += 0.05f;                
            }

            if (keyboardState.IsKeyDown(Keys.E)
                )
            {
                turretRotationValue -= 0.05f;
            }

            if (keyboardState.IsKeyDown(Keys.R)
                )
            {
                cannonRotationValue -= 0.05f;
            }

            if (keyboardState.IsKeyDown(Keys.F)
                )
            {
                cannonRotationValue += 0.05f;
            }


            if (keyboardState.IsKeyDown(Keys.T)
                && keyboardState != prevKeyState)
            {
                Power += 0.1f;
            }
            if (keyboardState.IsKeyDown(Keys.G)
                && keyboardState != prevKeyState)
            {
                Power -= 0.1f;
            }

            #endregion turret
        }

        override public void UpdateWorldMatrix()
        {

            worldMatrix = //mobileObject.GetWorldMatrixScale(scale);
                Matrix.CreateScale(scale)
                *
                Matrix.CreateFromYawPitchRoll(rotation.X, rotation.Y, rotation.Z)
                //Matrix.CreateFromQuaternion(rotationQ)
                *
                Matrix.CreateTranslation(position);
        }

        
        Vector3 GetDirection(Vector3 position, Vector3 target)
        {
            Vector3 dir = (target - position);
            dir.Normalize();
            return dir;
        }
        

        void FireMissile()
        {

            foreach (Missile missile in missiles)
            {
                if ( ! missile.alive)
                {
                    //soundBank.PlayCue("missilelaunch");

                    missile.rotation.Y = Math.Abs(turretRotationValue + FacingDirection);
                    missile.rotation.X = (cannonRotationValue);  

                   // missile.newPosition = GetMissileMuzzlePosition();
                    missile.position = GetMissileMuzzlePosition(missile);// missile.newPosition;
                    missile.velocity = GetMissileMuzzleVelocity(missile);

                    missile.rotation.X = (cannonRotationValue) + MathHelper.Pi;  

                    missile.alive = true;
                    //missile.inAir = true;
                    //missile.nearGround = false;

                    break;
                }
            }

        }       

        Vector3 GetMissileMuzzleVelocity(Missile missile)
        {
            Matrix rotationMatrix =
                Matrix.CreateFromYawPitchRoll(
                missile.rotation.Y,
                missile.rotation.X,
                missile.rotation.Z);
           
            return Vector3.Normalize(
                Vector3.Transform(Vector3.Backward,
                rotationMatrix)) * missilePower;
        }

        Vector3 GetMissileMuzzlePosition(Missile missile)
        {
            return position +
                new Vector3(0.05f, 4 + cannonRotationValue, 0) +  // variable offset.  should change based on scale
                (Vector3.Normalize(
                GetMissileMuzzleVelocity(missile)) *
                launcherHeadMuzzleOffset);
        }



        internal void Unload()
        {
            baseRotationValue = 0;
            cannonRotationValue = 0;
            turretRotationValue = 0;

            Matrix turretRotation = Matrix.CreateRotationZ(turretRotationValue);
            Matrix cannonRotation = Matrix.CreateRotationY(cannonRotationValue);
            Matrix baseRotation = Matrix.CreateRotationY(baseRotationValue);

            turretBone.Transform = turretRotation * turretTransform;
            cannonBone.Transform = cannonRotation * cannonTransform;
            baseBone.Transform = baseRotation * baseTransform;
        }
    }
}
