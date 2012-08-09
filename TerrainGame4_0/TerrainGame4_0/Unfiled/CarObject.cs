using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JigLibX.Vehicles;
using JigLibX.Collision;
using JigLibX.Objects;

namespace TerrainGame4_0
{
    public class CarObject : PhysicObject
    {

        private Car car;
        private Model wheel;

        public CarObject(Game game, Model model,Model wheels, bool FWDrive,
                       bool RWDrive,
                       float maxSteerAngle,
                       float steerRate,
                       float wheelSideFriction,
                       float wheelFwdFriction,
                       float wheelTravel,
                       float wheelRadius,
                       float wheelZOffset,
                       float wheelRestingFrac,
                       float wheelDampingFrac,
                       int wheelNumRays,
                       float driveTorque,
                       float gravity)
            : base(game, model)
        {
            car = new Car(FWDrive, RWDrive, maxSteerAngle, steerRate,
                wheelSideFriction, wheelFwdFriction, wheelTravel, wheelRadius,
                wheelZOffset, wheelRestingFrac, wheelDampingFrac,
                wheelNumRays, driveTorque, gravity);

            this.body = car.Chassis.Body;
            this.collision = car.Chassis.Skin;
            this.wheel = wheels;

            SetCarMass(100.0f);

           // this.scale *= 0.005f;
        }

        private void DrawWheel(Wheel wh, bool rotated)
        {
            //Camera camera = ((JiggleGame)this.Game).Camera;

            foreach (ModelMesh mesh in wheel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    float steer = wh.SteerAngle;

                    Matrix rot;
                    if (rotated) rot = Matrix.CreateRotationY(MathHelper.ToRadians(180.0f));
                    else rot = Matrix.Identity;

                    effect.World = rot * Matrix.CreateRotationZ(MathHelper.ToRadians(-wh.AxisAngle)) * // rotate the wheels
                        Matrix.CreateRotationY(MathHelper.ToRadians(steer)) *
                        Matrix.CreateTranslation(wh.Pos + wh.Displacement * wh.LocalAxisUp) * car.Chassis.Body.Orientation * // oritentation of wheels
                        Matrix.CreateTranslation(car.Chassis.Body.Position); // translation

                    //effect.View = this.parentGame.viewMatrix;
                    //effect.Projection = this.parentGame.projectionMatrix;
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                }
                mesh.Draw();
            }
        }


        public override void Draw(GameTime gameTime)
        {
            //DrawWheel(car.Wheels[0], true);
            //DrawWheel(car.Wheels[1], true);
            //DrawWheel(car.Wheels[2], false);
            //DrawWheel(car.Wheels[3], false);

           // Matrix orient = body.Orientation;
            //body.Orientation = body.Orientation * Matrix.CreateFromYawPitchRoll(MathHelper.PiOver2, 0, 0);
            base.Draw(gameTime);
           // body.Orientation = orient;
        }

        public Car Car
        {
            get { return this.car; }
        }

        private void SetCarMass(float mass)
        {
            body.Mass = mass;
            Vector3 min, max;
            car.Chassis.GetDims(out min, out max);
            Vector3 sides = max - min;

            float Ixx = (1.0f / 12.0f) * mass * (sides.Y * sides.Y + sides.Z * sides.Z);
            float Iyy = (1.0f / 12.0f) * mass * (sides.X * sides.X + sides.Z * sides.Z);
            float Izz = (1.0f / 12.0f) * mass * (sides.X * sides.X + sides.Y * sides.Y);

            Matrix inertia = Matrix.Identity;
            inertia.M11 = Ixx; inertia.M22 = Iyy; inertia.M33 = Izz;
            car.Chassis.Body.BodyInertia = inertia;
            car.SetupDefaultWheels();
        }

        public override void ApplyEffects(BasicEffect effect)
        {
            //
        }

        public Matrix GetWorldMatrixScale(float f_scale)
        {
            return
                 Matrix.CreateScale(scale)
                * body.CollisionSkin.GetPrimitiveLocal(0).Transform.Orientation
                * body.Orientation
                * Matrix.CreateTranslation(body.Position);
        }
    }
}
