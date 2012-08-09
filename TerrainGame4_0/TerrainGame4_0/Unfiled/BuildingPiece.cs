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
    public class BuildingPiece : GameObject
    {
        ParticleEmitter trailEmitter;
        ParticleEmitter fireEmitter;

        int numExplosionParticles = 30;
        const int numExplosionSmokeParticles = 1;

        const float trailParticlesPerSecond = 200;

        public bool isDestroyed = false;
        public bool isCleanedUp = false;

        DrawingClass drawClass;
        GameplayScreen Screen;

        public bool ContactGround = false;

        Matrix RotatMat = Matrix.CreateFromYawPitchRoll(0, MathHelper.ToRadians(90), 0);

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



        public BuildingPiece(ParentGame game, Model modelObj, Texture2D[] modelTextures,
            DrawingClass drawClass, GameplayScreen Screen, float Length, float Width, float Height)
            : base(game, modelObj, modelTextures)
        {
            // Use the particle emitter helper to output our trail particles.
            trailEmitter = new ParticleEmitter(drawClass.smokePlumeParticles,
                                               3, position);

            fireEmitter = new ParticleEmitter(drawClass.fireParticles,
                                               30, position);

            this.Screen = Screen;
            this.drawClass = drawClass;

            _body = new Body();
            _skin = new CollisionSkin(_body);
            _body.CollisionSkin = _skin;

            //Box box = new Box(Vector3.Zero, Matrix.Identity, new Vector3(10f, 7f, 7f));
            Box box = new Box(Vector3.Zero, Matrix.Identity, new Vector3(Length, Width, Height));
            if (Length > Width && Length > Height)
                boundSphere.Radius = Length;
            else if (Width > Length && Width > Height)
                boundSphere.Radius = Width;
            else
                boundSphere.Radius = Height;
            _skin.AddPrimitive(box, new MaterialProperties(0.8f, 0.8f, 0.7f));

            Vector3 com = SetMass(3.0f);

            _body.MoveTo(position, Matrix.Identity);
            _skin.ApplyLocalTransform(new Transform(-com, Matrix.Identity));
            _body.EnableBody();

            Body.ExternalData = this;

            SetBody(rotation);

            Body.AllowFreezing = true;
            Body.SetDeactivationTime(1);

            foreach (BuildingPiece pic in Screen.PieceList)
                DisableCollisions(this.Body, pic.Body);

            foreach (Building bld in Screen.Buildings)
                DisableCollisions(this.Body, bld.Body);
        }

        public void SetBody(Matrix Orientation)
        {
            Body.MoveTo(position, Orientation);
        }

        public void SetBody(Vector3 Rotation)
        {
            Matrix Orientation = Matrix.CreateFromYawPitchRoll(Rotation.X, Rotation.Y, Rotation.Z);
            Body.MoveTo(position, Orientation);
        }

        override public void Update(float amount, GameTime gameTime)
        {
            
            if (isDestroyed && !isCleanedUp)
                Release();

            if (!isDestroyed)
            {
                position = Body.Position;
                trailEmitter.Update(gameTime, position);
                fireEmitter.Update(gameTime, position);

                float small = 0.05f; //0.009f;
                if (Body.Velocity.Length() < small)
                    Destroy();

                
            }

            
        }

        override public void Destroy()
        {
            isDestroyed = true;
        }

        private void Release()
        {
            this.Body.DisableBody();
            this.Skin.RemoveAllPrimitives();

            isCleanedUp = true;
        }




        override public void UpdateWorldMatrix()
        {
            if(!isDestroyed)
            worldMatrix =
                Matrix.CreateScale(scale) *
                _body.CollisionSkin.GetPrimitiveLocal(0).Transform.Orientation *
                (_body.Orientation * RotatMat)
                *
                Matrix.CreateTranslation(_body.Position);
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
