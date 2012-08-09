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

namespace TerrainGame4_0
{
    public class Building : GameObject
    {

        const int numExplosionParticles = 500;
        const int numExplosionSmokeParticles = 100;

        const float trailParticlesPerSecond = 200;

        public bool isDestroyed = false;
        public bool isCleanedUp = false;
        public string Type;
        public bool Selected = false;
        //public Model ModelDestroyed;

        ParticleEmitter trailEmitter;
        public DrawingClass drawClass;

        private Body _body;
        public Body Body
        {
            get
            {
                return _body;
            }
            set
            {
                _body = value;
            }

        }

        private CollisionSkin _skin;
        public CollisionSkin Skin
        {
            get
            {
                return _skin;
            }
            set
            {
                _skin = value;
            }
        }

        public GameplayScreen Screen;

        Matrix RotatMat = Matrix.CreateFromYawPitchRoll(0, MathHelper.ToRadians(90), 0);


        public Building(ParentGame game, Model modelObj, Texture2D[] modelTextures,
            Vector3 Position, float Scale, Vector3 Rotation, string Type, GameplayScreen Screen)
            : base(game, modelObj, modelTextures)
        {
            // Use the particle emitter helper to output our trail particles.
            //trailEmitter = new ParticleEmitter(parentGame.projectileTrailParticles,
            //                                   trailParticlesPerSecond, position);

            this.Screen = Screen;
            this.drawClass = Screen.NormalDrawing;
            this.Type = Type;

            position = Position;
            scale = Scale;
            rotation = Rotation;
            
            Initialize();
        }

        public virtual void Initialize()
        {
            _body = new Body();
            _skin = new CollisionSkin(_body);
            _body.CollisionSkin = _skin;

            // l w h           
            //Box box = new Box(Vector3.Zero, Matrix.Identity, new Vector3(9f, 9f, 39f));
            //if (Type == "DEPOT")
            //    box = new Box(Vector3.Zero, Matrix.Identity, new Vector3(20, 15, 11f));

            Box box = new Box(Vector3.Zero, Matrix.Identity, new Vector3(9f, 39f, 9f));
            boundSphere.Radius = 39;
            if (Type == "DEPOT")
            {
                box = new Box(Vector3.Zero, Matrix.Identity, new Vector3(20, 11, 15f));
                boundSphere.Radius = 20;
            }

            _skin.AddPrimitive(box, new MaterialProperties(1, 1, 1));

            Vector3 com = SetMass(1);

            
            //_body.MoveTo(position, Matrix.Identity);
            _skin.ApplyLocalTransform(new Transform(-com, Matrix.Identity));
            _body.EnableBody();

            Body.ExternalData = this;

            SetBody(rotation);

            DisableCollisions(Screen.terrainActor.Body, Body);
        }

        public void SetBody(Matrix Orientation)
        {
            Body.MoveTo(position, Orientation);
        }

        public void SetBody(Vector3 Rotation)
        {
            Matrix Orientation = Matrix.CreateFromYawPitchRoll(Rotation.X, 0, 0);
            Body.MoveTo(position, Orientation);
        }

        override public void Update(float amount, GameTime gameTime)
        {
            //trailEmitter.Update(gameTime, position);
            if (isDestroyed && !isCleanedUp)
                Release();

            if (!isDestroyed)
            {
                SetWorldMatrix();

                position = Body.Position;
            }
        }



        override public void UpdateWorldMatrix()
        {
            if (!isDestroyed)
                SetWorldMatrix();
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

        void SetWorldMatrix()
        {
            
            if(Body.IsBodyEnabled)
                worldMatrix =
                Matrix.CreateScale(scale) *
                _body.CollisionSkin.GetPrimitiveLocal(0).Transform.Orientation *
                (_body.Orientation)// * RotatMat)
                *
                Matrix.CreateTranslation(_body.Position);
        }

        public virtual Matrix GetWorldMatrix()
        {
            return
                worldMatrix;
        }

        public Matrix GetWorldMatrixScale(float f_scale)
        {
            return
                Matrix.CreateScale(f_scale) *
                _body.CollisionSkin.GetPrimitiveLocal(0).Transform.Orientation *
                (_body.Orientation + Matrix.CreateFromYawPitchRoll(0, MathHelper.ToRadians(90), 0))
                *
                Matrix.CreateTranslation(_body.Position);
        }

        public  Matrix GetOrientation()
        {
            return
                _skin.GetPrimitiveLocal(0).Transform.Orientation *
                _body.Orientation;
        }

        public  Matrix GetTranslation()
        {
            return
                Matrix.CreateTranslation(_body.Position);
        }

        override public void Destroy()
        {
            isDestroyed = true;

            Vector3 pos = position;
            pos.Y += 20;
            for (int i = 0; i < numExplosionParticles; i++)
            {
                drawClass.concreteParticles.AddParticle(pos, Body.Velocity);
                pos.Y -= 0.04f;
            }

            pos = position;
            pos.Y += 20;
            for (int i = 0; i < numExplosionSmokeParticles; i++)
            {
                drawClass.sparkParticles.AddParticle(pos, Body.Velocity);
                pos.Y -= 0.18f;
            }

            pos = position;
            pos.Y += 20;
            for (int i = 0; i < numExplosionSmokeParticles; i++)
            {
                drawClass.buildingColumn.AddParticle(pos, Body.Velocity);
                pos.Y -= 0.18f;
            }

            pos = position;
            pos.Y += 5;
            for (int i = 0; i < numExplosionSmokeParticles; i++)
            {
                drawClass.buildingDisk.AddParticle(pos, Body.Velocity);
                pos.Y -= 0.02f;
            }


            Screen.cue = parentGame.soundBank.GetCue("building_crack");

            AudioEmitter emitter = new AudioEmitter();
            emitter.Position = position;
            Screen.cue.Apply3D(Screen.listener, emitter);
            Screen.cue.Play();
        }

        private void Release()
        {
            this.Body.DisableBody();
            this.Skin.RemoveAllPrimitives();

            isCleanedUp = true;
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
