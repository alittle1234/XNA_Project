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

using JigLibX.Math;
using JigLibX.Physics;
using JigLibX.Geometry;
using JigLibX.Collision;
using JigLibX.Objects;

namespace TerrainGame4_0
{
    public class Missile : GameObject
    {

        const int numExplosionParticles = 30;
        const int numExplosionSmokeParticles = 50;

        const float trailParticlesPerSecond = 6;

        public bool isDestroyed = false;
        public bool isCleanedUp = false;

        ParticleEmitter trailEmitter;
        DrawingClass drawClass;
        GameplayScreen Screen;

        public RagdollObject rgob;
        public List<Matrix> RagdollTransforms;

        private Body _body;
        public Body Body
        {
            get
            {
                return _body;
            }
        }

        private CollisionSkin _skin;
        public CollisionSkin Skin
        {
            get
            {
                return _skin;
            }
        }

    

        public Missile(ParentGame game, Model modelObj, Texture2D[] modelTextures, DrawingClass drawClass, GameplayScreen Screen) 
            : base( game, modelObj, modelTextures )
        {

            this.drawClass = drawClass;
            this.Screen = Screen;

            _body = new Body();
            _skin = new CollisionSkin(_body);
            _body.CollisionSkin = _skin;


            Box box = new Box(Vector3.Zero, Matrix.Identity, new Vector3(1f,1f,4f));
            _skin.AddPrimitive(box, new MaterialProperties(0.8f, 0.8f, 0.7f));

            Vector3 com = SetMass(2.0f);

            _body.MoveTo(position, Matrix.Identity);
            _skin.ApplyLocalTransform(new Transform(-com, Matrix.Identity));
            _body.EnableBody();

            Body.ExternalData = this;



            Vector3 pos = position;
            Vector3 forwardVec = Body.Orientation.Forward;
            forwardVec.Normalize();

            pos -= forwardVec * 10;
            // Use the particle emitter helper to output our trail particles.
            trailEmitter = new ParticleEmitter(drawClass.projectileTrailParticles,
                                               trailParticlesPerSecond, position);

            
            rgob = new RagdollObject(parentGame, null, null, null, RagdollObject.RagdollType.Simple, 1.0f, 3);
            rgob.Position = position;
            //rgob.PutToSleep();

            //rgob.limbs[0].PhysicsBody.AngularVelocity = (new Vector3(1, 1, 0) * 2000);

            RagdollTransforms = new List<Matrix>();
            
            RagdollTransforms = rgob.GetWorldMatrix();

            foreach (JigLibX.Objects.PhysicObject lim in rgob.limbs)
                DisableCollisions(lim.PhysicsBody, Body);

            foreach (JigLibX.Objects.PhysicObject lim in rgob.limbs)
                foreach (BuildingPiece pic in Screen.PieceList)
                    DisableCollisions(lim.PhysicsBody, pic.Body);

            foreach (JigLibX.Objects.PhysicObject lim in rgob.limbs)
                foreach (Building bld in Screen.Buildings)
                    DisableCollisions(lim.PhysicsBody, bld.Body);

            foreach (JigLibX.Objects.PhysicObject lim in rgob.limbs)
                DisableCollisions(lim.PhysicsBody, Screen.terrainActor.Body);

            foreach (JigLibX.Objects.PhysicObject limb0 in rgob.limbs)
                foreach (Missile mis in Screen.BulletList)
                    foreach (JigLibX.Objects.PhysicObject limb1 in mis.rgob.limbs)
                        DisableCollisions(limb1.PhysicsBody, limb0.PhysicsBody);
        }

        public void SetBody(Matrix Orientation)
        {
            Body.MoveTo(position, Orientation);
        }


        override public void Update(float amount, GameTime gameTime)
        {
            Vector3 pos = position;
            Vector3 forwardVec = Body.Orientation.Forward;
            forwardVec.Normalize();

            pos -= forwardVec * 4;

            trailEmitter.Update(gameTime, pos);

            if (isDestroyed && !isCleanedUp)
                Release();

            if (!isDestroyed)
            {
                position = Body.Position;

                rgob.Position = position;
                
                RagdollTransforms = rgob.GetWorldMatrix();
            }
        }

        override public void Destroy()
        {
            isDestroyed = true;
            Body.Position = new Vector3(0, 500, 0);

            for (int i = 0; i < numExplosionParticles; i++)
                drawClass.explosionParticles.AddParticle(position, Body.Velocity);

            for (int i = 0; i < numExplosionSmokeParticles; i++)
                drawClass.explosionSmokeParticles.AddParticle(position, Vector3.Zero);
            
            Screen.cue = parentGame.soundBank.GetCue("explo2");
            Screen.cue.Apply3D(Screen.listener, Screen.emitter);
            Screen.cue.Play();

            Screen.emitter.Position = position;


            //parentGame.soundBank.PlayCue("explo2");
        }

        private void Release()
        {
            this.Body.DisableBody();
            this.Skin.RemoveAllPrimitives();

            foreach (JigLibX.Physics.HingeJoint hin in rgob.joints)
            {
                hin.DisableHinge();
                hin.DisableController();
            }

            foreach (JigLibX.Objects.PhysicObject lim in rgob.limbs)
            {
                lim.PhysicsBody.DisableBody();
                lim.PhysicsSkin.RemoveAllPrimitives();

                bool phys = PhysicsSystem.CurrentPhysicsSystem.RemoveBody(lim.PhysicsBody);

                bool gone = parentGame.Components.Remove(lim);
                lim.Dispose();
            }

            isCleanedUp = true;
        }


        

        override public void UpdateWorldMatrix()
        {
            

            worldMatrix =
                Matrix.CreateFromYawPitchRoll(
                rotation.Y,
                rotation.X,
                rotation.Z)
                *
                Matrix.CreateScale(scale)
                *
                Matrix.CreateTranslation(position);
        }

        private Vector3 SetMass(float mass)
        {
            PrimitiveProperties primitiveProperties = new PrimitiveProperties(
                PrimitiveProperties.MassDistributionEnum.Solid,
                PrimitiveProperties.MassTypeEnum.Mass, mass);

            float junk;
            Vector3 com;
            Matrix it;
            Matrix itCoM;

            Skin.GetMassProperties(primitiveProperties, out junk, out com, out it, out itCoM);

            Body.BodyInertia = itCoM;
            Body.Mass = junk;

            return com;
        }

        public Matrix GetWorldMatrix()
        {
            return
                Matrix.CreateScale(scale) *
                //_body.CollisionSkin.GetPrimitiveLocal(0).Transform.Orientation *
                _body.Orientation *
                Matrix.CreateTranslation(_body.Position);
        }

        public Matrix GetWorldMatrixScale(float f_scale)
        {
            return
                Matrix.CreateScale(f_scale) *
                //_body.CollisionSkin.GetPrimitiveLocal(0).Transform.Orientation *
                _body.Orientation *
                Matrix.CreateTranslation(_body.Position);
        }

        public Matrix GetOrientation()
        {
            return
                _skin.GetPrimitiveLocal(0).Transform.Orientation *
                _body.Orientation;
        }

        public Matrix GetTranslation()
        {
            return
                Matrix.CreateTranslation(_body.Position);
        }


        private void DisableCollisions(Body rb0, Body rb1)
        {
            if ((rb0.CollisionSkin == null) || (rb1.CollisionSkin == null))
                return;
            rb0.CollisionSkin.NonCollidables.Add(rb1.CollisionSkin);
            rb1.CollisionSkin.NonCollidables.Add(rb0.CollisionSkin);
        }

    }
}
