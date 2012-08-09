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
    public class Ring : Building
    {

        const int numExplosionParticles = 500;
        const int numExplosionSmokeParticles = 100;

        const float trailParticlesPerSecond = 200;

        float radius = 12;

        public Ring(ParentGame game, Model modelObj, Texture2D[] modelTextures, 
            Vector3 Position, float Scale, Vector3 Rotation, string Type, GameplayScreen Screen)
            :base(game, modelObj, modelTextures,
            Position, Scale, Rotation, Type, Screen)
        {

        }

        public override void Initialize()
        {
            Body = new Body();
            Skin = new CollisionSkin(Body);
            Body.CollisionSkin = Skin;

            radius = 12;
            float length = 3;

            //Capsule middle = new Capsule(Vector3.Zero, Matrix.Identity, radius, length - 2.0f * radius);

            float sideLength = 2.0f * radius / (float)Math.Sqrt(2.0d);

            Vector3 sides = new Vector3(-0.5f * sideLength, -0.5f * sideLength, -radius);

            Box supply0 = new Box(sides, Matrix.Identity,
                new Vector3(sideLength, sideLength, length));

            Box supply1 = new Box(Vector3.Transform(sides, Matrix.CreateRotationZ(MathHelper.PiOver4)),
                Matrix.CreateRotationZ(MathHelper.PiOver4), new Vector3(sideLength, sideLength, length));

            Box supply2 = new Box(Vector3.Transform(sides, Matrix.CreateRotationZ( (MathHelper.PiOver4 * 0.5f) )),
                Matrix.CreateRotationZ( (MathHelper.PiOver4 * 0.5f) ), new Vector3(sideLength, sideLength, length));

            Box supply3 = new Box(Vector3.Transform(sides, Matrix.CreateRotationZ((MathHelper.PiOver4 * 1.5f))),
                Matrix.CreateRotationZ((MathHelper.PiOver4 * 1.5f)), new Vector3(sideLength, sideLength, length));

            //Skin.AddPrimitive(middle, new MaterialProperties(0.8f, 0.8f, 0.7f));
            Skin.AddPrimitive(supply0, new MaterialProperties(0.8f, 0.8f, 0.7f));
            Skin.AddPrimitive(supply1, new MaterialProperties(0.8f, 0.8f, 0.7f));
            Skin.AddPrimitive(supply2, new MaterialProperties(0.8f, 0.8f, 0.7f));
            Skin.AddPrimitive(supply3, new MaterialProperties(0.8f, 0.8f, 0.7f));


            Vector3 com = SetMass(1.0f);

            Body.MoveTo(position, Matrix.Identity);
            Skin.ApplyLocalTransform(new Transform(-com, Matrix.Identity));

            #region Manually set body inertia
            float cylinderMass = Body.Mass;

            float comOffs = (length - 2.0f * radius) * 0.5f; ;

            float Ixx = 0.5f * cylinderMass * radius * radius + cylinderMass * comOffs * comOffs;
            float Iyy = 0.25f * cylinderMass * radius * radius + (1.0f / 12.0f) * cylinderMass * length * length + cylinderMass * comOffs * comOffs;
            float Izz = Iyy;

            Body.SetBodyInertia(Ixx, Iyy, Izz);
            #endregion

            Body.EnableBody();

            Body.ExternalData = this;

            SetBody(rotation);
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
               
        override public void Destroy()
        {
            isDestroyed = true;

            Vector3 pos = position;
            //float amount = (float)(min + (float)random.NextDouble() * (max - min));
            double min = 0;
            double max = MathHelper.TwoPi;
            double angle;
            for (int i = 0; i < numExplosionSmokeParticles; i++)
            {
                pos = position;
                angle = min + Screen.random.NextDouble() * (max - min);
                Matrix.CreateFromAxisAngle(new Vector3(1, 0, 0), rotation.X);
                pos.Y += (float)(radius * Math.Sin(angle));
                pos.X += (float)(radius * Math.Cos(angle));

                drawClass.ringExplosionParticles.AddParticle(pos, Body.Velocity);
                pos.Y -= 0.02f;
            }


            Screen.cue = parentGame.soundBank.GetCue("ring");

            AudioEmitter emitter = new AudioEmitter();
            emitter.Position = position;
            Screen.cue.Apply3D(Screen.listener, emitter);
            Screen.cue.Play();
        }

        override public Matrix GetWorldMatrix()
        {
            return
                Matrix.CreateScale(scale) *
                Body.CollisionSkin.GetPrimitiveLocal(0).Transform.Orientation *
                (Body.Orientation)
                *
                Matrix.CreateTranslation(Body.Position);
        }

    }
}
