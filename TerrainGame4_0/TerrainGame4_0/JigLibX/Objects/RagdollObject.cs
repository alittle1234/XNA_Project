using System;
using System.Collections.Generic;
using System.Text;
using JigLibX.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JigLibX.Objects
{
    public class RagdollObject : PhysicObject
    {

        public enum RagdollType
        {
            Simple,Complex
        }

        public enum LimbId
        {
            Torso,
            Head,
            UpperLegLeft,
            UpperLegRight,
            LowerLegLeft,
            LowerLegRight,
            UpperArmLeft,
            UpperArmRight,
            LowerArmLeft,
            LowerArmRight,
            FootLeft,
            FootRight,
            HandLeft,
            HandRight,
            Hips,
            NumLimbs
        }

        private enum JointId
        {
            Neck,
            ShoulderLeft,
            ShoulderRight,
            ElbowLeft,
            ElbowRight,
            HipLeft,
            HipRight,
            KneeLeft,
            KneeRight,
            WristLeft,
            WristRight,
            AnkleLeft,
            AnkleRight,
            Spine,
            NumJoints
        }

        public PhysicObject[] limbs;
        public HingeJoint[] joints;

        int numLimbs;
        int numJoints;

        float Scale;

        private void DisableCollisions(Body rb0, Body rb1)
        {
            if ((rb0.CollisionSkin == null) || (rb1.CollisionSkin == null))
                return;
            rb0.CollisionSkin.NonCollidables.Add(rb1.CollisionSkin);
            rb1.CollisionSkin.NonCollidables.Add(rb0.CollisionSkin);
        }

        public RagdollObject(Game game,Model capsule,Model sphere,Model box, RagdollType type,float density, float SCALE)
            : base(game)
        {
            Scale = SCALE;

            if (type == RagdollType.Complex)
            {
                numLimbs = (int)LimbId.NumLimbs;
                numJoints = (int)JointId.NumJoints;
            }
            else
            {
                numLimbs = (int)LimbId.NumLimbs - 5;
                numJoints = (int)JointId.NumJoints - 5;
            }

            limbs = new PhysicObject[numLimbs];
            joints = new HingeJoint[numJoints];

            limbs[(int)LimbId.Head] = new SphereObject(this.Game, sphere, 0.15f * Scale, Matrix.Identity, Vector3.Zero);
            limbs[(int)LimbId.UpperLegLeft] = new CapsuleObject(this.Game, capsule, 0.08f * Scale, 0.3f * Scale, Matrix.CreateRotationX(MathHelper.ToRadians(90)), Vector3.Zero);
            limbs[(int)LimbId.UpperLegRight] = new CapsuleObject(this.Game, capsule, 0.08f * Scale, 0.3f * Scale, Matrix.CreateRotationX(MathHelper.ToRadians(90)), Vector3.Zero);
            limbs[(int)LimbId.LowerLegLeft] = new CapsuleObject(this.Game, capsule, 0.08f * Scale, 0.3f * Scale, Matrix.CreateRotationX(MathHelper.ToRadians(90)), Vector3.Zero);
            limbs[(int)LimbId.LowerLegRight] = new CapsuleObject(this.Game, capsule, 0.08f * Scale, 0.3f * Scale, Matrix.CreateRotationX(MathHelper.ToRadians(90)), Vector3.Zero);
            limbs[(int)LimbId.UpperArmLeft] = new CapsuleObject(this.Game, capsule, 0.07f * Scale, 0.2f * Scale, Matrix.CreateRotationZ(MathHelper.ToRadians(90)), Vector3.Zero);
            limbs[(int)LimbId.UpperArmRight] = new CapsuleObject(this.Game, capsule, 0.07f * Scale, 0.2f * Scale, Matrix.CreateRotationZ(MathHelper.ToRadians(90)), Vector3.Zero);
            limbs[(int)LimbId.LowerArmLeft] = new CapsuleObject(this.Game, capsule, 0.06f * Scale, 0.2f * Scale, Matrix.CreateRotationZ(MathHelper.ToRadians(90)), Vector3.Zero);
            limbs[(int)LimbId.LowerArmRight] = new CapsuleObject(this.Game, capsule, 0.06f * Scale, 0.2f * Scale, Matrix.CreateRotationZ(MathHelper.ToRadians(90)), Vector3.Zero);

            if (type == RagdollType.Complex)
            {
                limbs[(int)LimbId.FootLeft] = new SphereObject(this.Game, sphere, 0.07f, Matrix.Identity, Vector3.Zero);
                limbs[(int)LimbId.FootRight] = new SphereObject(this.Game, sphere, 0.07f, Matrix.Identity, Vector3.Zero);
                limbs[(int)LimbId.HandLeft] = new SphereObject(this.Game, sphere, 0.05f, Matrix.Identity, Vector3.Zero);
                limbs[(int)LimbId.HandRight] = new SphereObject(this.Game, sphere, 0.05f, Matrix.Identity, Vector3.Zero);
                limbs[(int)LimbId.Torso] = new BoxObject(this.Game, box, new Vector3(0.2f, 0.4f,0.35f), Matrix.Identity, Vector3.Zero);
                limbs[(int)LimbId.Hips] = new BoxObject(this.Game, box, new Vector3(0.2f, 0.2f,0.35f), Matrix.Identity, Vector3.Zero);
            }
            else
            {
                limbs[(int)LimbId.Torso] = new BoxObject(this.Game, box, new Vector3(0.2f * Scale, 0.6f * Scale, 0.35f * Scale), Matrix.Identity, Vector3.Zero);
            }

            limbs[(int)LimbId.Head].PhysicsBody.Position = new Vector3(0.03f * Scale, 0.5f * Scale, 0);
            limbs[(int)LimbId.UpperLegLeft].PhysicsBody.Position = new Vector3(0, -0.4f * Scale, 0.12f * Scale);
            limbs[(int)LimbId.UpperLegRight].PhysicsBody.Position = new Vector3(0, -0.4f * Scale, -0.12f);
            limbs[(int)LimbId.LowerLegLeft].PhysicsBody.Position = new Vector3(0, -0.7f * Scale, 0.12f * Scale);
            limbs[(int)LimbId.LowerLegRight].PhysicsBody.Position = new Vector3(0, -0.7f * Scale, -0.12f * Scale);
            limbs[(int)LimbId.UpperArmLeft].PhysicsBody.Position = new Vector3(0, 0.25f * Scale, 0.25f * Scale);
            limbs[(int)LimbId.UpperArmRight].PhysicsBody.Position = new Vector3(0, 0.25f * Scale, -0.25f * Scale);
            limbs[(int)LimbId.LowerArmLeft].PhysicsBody.Position = new Vector3(0, 0.25f * Scale, 0.5f * Scale);
            limbs[(int)LimbId.LowerArmRight].PhysicsBody.Position = new Vector3(0, 0.25f * Scale, -0.5f * Scale);

            if (type == RagdollType.Complex)
            {
                limbs[(int)LimbId.FootLeft].PhysicsBody.Position = new Vector3(0.13f * Scale, -0.85f * Scale, 0.12f * Scale);
                limbs[(int)LimbId.FootRight].PhysicsBody.Position = new Vector3(0.13f * Scale, -0.85f * Scale, -0.12f * Scale);
                limbs[(int)LimbId.HandLeft].PhysicsBody.Position = new Vector3(0, 0.25f * Scale, 0.72f * Scale);
                limbs[(int)LimbId.HandRight].PhysicsBody.Position = new Vector3(0, 0.25f * Scale, -0.72f * Scale);
                limbs[(int)LimbId.Torso].PhysicsBody.Position = new Vector3(0, 0.2f * Scale, 0.0f * Scale);
                limbs[(int)LimbId.Hips].PhysicsBody.Position = new Vector3(0, -0.1f * Scale, 0.0f * Scale);
            }
            else
            {
                limbs[(int)LimbId.Torso].PhysicsBody.Position = new Vector3(0, 0, 0);
            }


            // set up hinge joints
            float haldWidth = 0.2f * Scale;
            float sidewaysSlack = 0.1f * Scale;
            float damping = 0.5f;

            for (int i = 0; i < numJoints; i++)
                joints[i] = new HingeJoint();

            #region COMPLEX
            if (type == RagdollType.Complex)
            {
                joints[(int)JointId.Spine].Initialise(limbs[(int)LimbId.Hips].PhysicsBody,
                                        limbs[(int)LimbId.Torso].PhysicsBody,
                                        new Vector3(0.0f, 0.0f, 1.0f),
                                        new Vector3(0.0f, 0.1f, 0.0f),
                                        haldWidth, 70.0f, 30.0f,
                                        3.0f * sidewaysSlack,
                                        damping);

                joints[(int)JointId.Neck].Initialise(limbs[(int)LimbId.Hips].PhysicsBody,
                                        limbs[(int)LimbId.Head].PhysicsBody,
                                        new Vector3(0.0f, 0.0f, 1.0f),
                                        new Vector3(-0.05f, 0.25f, 0.0f),
                                        haldWidth, 50.0f, 20.0f,
                                        3.0f * sidewaysSlack,
                                        damping);

                joints[(int)JointId.ShoulderLeft].Initialise(limbs[(int)LimbId.Torso].PhysicsBody,
                                        limbs[(int)LimbId.UpperArmLeft].PhysicsBody,
                                        new Vector3(1.0f, 0.0f, 0.0f),
                                        new Vector3(0.0f, 0.05f, 0.15f),
                                        haldWidth, 70.0f, 30.0f,
                                        0.7f,
                                        damping);

                joints[(int)JointId.ShoulderRight].Initialise(limbs[(int)LimbId.Torso].PhysicsBody,
                                        limbs[(int)LimbId.UpperArmRight].PhysicsBody,
                                        new Vector3(1.0f, 0.0f, 0.0f),
                                        new Vector3(0.0f, 0.05f, -0.15f),
                                        haldWidth, 30.0f, 75.0f,
                                        0.7f,
                                        damping);

                joints[(int)JointId.HipLeft].Initialise(limbs[(int)LimbId.Hips].PhysicsBody,
                                        limbs[(int)LimbId.UpperLegLeft].PhysicsBody,
                                        new Vector3(0.0f, 0.0f, 1.0f),
                                        new Vector3(0.0f, -0.25f, 0.12f),
                                        haldWidth, 10.0f, 60.0f,
                                        0.4f,
                                        damping);

                joints[(int)JointId.HipRight].Initialise(limbs[(int)LimbId.Hips].PhysicsBody,
                                        limbs[(int)LimbId.UpperLegRight].PhysicsBody,
                                        new Vector3(0.0f, 0.0f, 1.0f),
                                        new Vector3(0.0f, -0.25f, -0.12f),
                                        haldWidth, 10.0f, 60.0f,
                                        0.4f,
                                        damping);

                joints[(int)JointId.AnkleLeft].Initialise(limbs[(int)LimbId.LowerLegLeft].PhysicsBody,
                                        limbs[(int)LimbId.FootLeft].PhysicsBody,
                                        new Vector3(0.0f, 0.0f, 1.0f),
                                        new Vector3(0.0f, -0.15f, 0.15f),
                                        haldWidth, 30.0f, 10.0f,
                                        0.01f,
                                        damping);

                joints[(int)JointId.AnkleRight].Initialise(limbs[(int)LimbId.LowerLegRight].PhysicsBody,
                                        limbs[(int)LimbId.FootRight].PhysicsBody,
                                        new Vector3(0.0f, 0.0f, 1.0f),
                                        new Vector3(0.0f, -0.15f, -0.15f),
                                        haldWidth, 30.0f, 10.0f,
                                        0.01f,
                                         damping);

                joints[(int)JointId.WristLeft].Initialise(limbs[(int)LimbId.LowerArmLeft].PhysicsBody,
                                        limbs[(int)LimbId.HandLeft].PhysicsBody,
                                        new Vector3(0.0f, 1.0f, 0.0f),
                                        new Vector3(0.0f, 0.0f, 0.12f),
                                        haldWidth, 45.0f, 70.0f,
                                        0.01f,
                                        damping);

                joints[(int)JointId.WristRight].Initialise(limbs[(int)LimbId.LowerArmRight].PhysicsBody,
                                        limbs[(int)LimbId.HandRight].PhysicsBody,
                                        new Vector3(0.0f, 1.0f, 0.0f),
                                        new Vector3(0.0f, 0.0f, -0.12f),
                                        haldWidth, 45.0f, 70.0f,
                                        0.01f,
                                        damping);

            }
            #endregion
            else
            {
                joints[(int)JointId.Neck].Initialise(limbs[(int)LimbId.Torso].PhysicsBody,
                              limbs[(int)LimbId.Head].PhysicsBody,
                              new Vector3(0.0f, 0.0f, 1.0f),
                              new Vector3(-0.05f * Scale, 0.25f * Scale, 0.0f),
                              haldWidth, 50.0f, 20.0f,
                              3 * sidewaysSlack,
                              damping);

                joints[(int)JointId.ShoulderLeft].Initialise(limbs[(int)LimbId.Torso].PhysicsBody,
                              limbs[(int)LimbId.UpperArmLeft].PhysicsBody,
                              new Vector3(1.0f, 0.0f, 0.0f),
                              new Vector3(0.0f, 0.25f * Scale, 0.15f * Scale),
                              haldWidth, 30.0f, 75.0f,
                              0.7f * Scale,
                              damping);

                joints[(int)JointId.ShoulderRight].Initialise(limbs[(int)LimbId.Torso].PhysicsBody,
                              limbs[(int)LimbId.UpperArmRight].PhysicsBody,
                              new Vector3(1.0f, 0.0f, 0.0f),
                              new Vector3(0.0f, 0.25f * Scale, -0.15f * Scale),
                              haldWidth, 75.0f, 30.0f,
                              0.7f * Scale,
                              damping);


                joints[(int)JointId.HipLeft].Initialise(limbs[(int)LimbId.Torso].PhysicsBody,
                              limbs[(int)LimbId.UpperLegLeft].PhysicsBody,
                              new Vector3(0.0f, 0.0f, 1.0f),
                              new Vector3(0.0f, -0.25f * Scale, 0.12f * Scale),
                              haldWidth, 10.0f, 60.0f,
                              0.4f * Scale,
                              damping);


                joints[(int)JointId.HipRight].Initialise(limbs[(int)LimbId.Torso].PhysicsBody,
                              limbs[(int)LimbId.UpperLegRight].PhysicsBody,
                              new Vector3(0.0f, 0.0f, 1.0f),
                              new Vector3(0.0f, -0.25f * Scale, -0.12f * Scale),
                              haldWidth, 10.0f, 60.0f,
                              0.4f * Scale,
                              damping);


            }


            joints[(int)JointId.KneeLeft].Initialise(limbs[(int)LimbId.UpperLegLeft].PhysicsBody,
                        limbs[(int)LimbId.LowerLegLeft].PhysicsBody,
                        new Vector3(0.0f, 0.0f, 1.0f),
                        new Vector3(0.0f, -0.15f * Scale, 0.0f),
                        haldWidth, 100.0f, 0.0f,
                        sidewaysSlack,
                        damping);

            joints[(int)JointId.KneeRight].Initialise(limbs[(int)LimbId.UpperLegRight].PhysicsBody,
                        limbs[(int)LimbId.LowerLegRight].PhysicsBody,
                        new Vector3(0.0f, 0.0f, 1.0f),
                        new Vector3(0.0f, -0.15f * Scale, 0.0f),
                        haldWidth, 100.0f, 0.0f,
                        sidewaysSlack,
                        damping);

            joints[(int)JointId.ElbowLeft].Initialise(limbs[(int)LimbId.UpperArmLeft].PhysicsBody,
                        limbs[(int)LimbId.LowerArmLeft].PhysicsBody,
                        new Vector3(0.0f, 0.0f, 1.0f),
                        new Vector3(0.0f, 0.0f, 0.13f * Scale),
                        haldWidth, 0.0f, 130.0f,
                        sidewaysSlack,
                        damping);

            joints[(int)JointId.ElbowRight].Initialise(limbs[(int)LimbId.UpperArmRight].PhysicsBody,
                        limbs[(int)LimbId.LowerArmRight].PhysicsBody,
                        new Vector3(0.0f, 0.0f, 1.0f),
                        new Vector3(0.0f, 0.0f, -0.13f * Scale),
                        haldWidth, 130.0f, 0.0f,
                        sidewaysSlack,
                        damping);



            // disable some collisions between adjacent pairs
            DisableCollisions(limbs[(int)LimbId.Torso].PhysicsBody, limbs[(int)LimbId.Head].PhysicsBody);
            DisableCollisions(limbs[(int)LimbId.Torso].PhysicsBody, limbs[(int)LimbId.UpperLegLeft].PhysicsBody);
            DisableCollisions(limbs[(int)LimbId.Torso].PhysicsBody, limbs[(int)LimbId.UpperLegRight].PhysicsBody);
            DisableCollisions(limbs[(int)LimbId.Torso].PhysicsBody, limbs[(int)LimbId.UpperArmLeft].PhysicsBody);
            DisableCollisions(limbs[(int)LimbId.Torso].PhysicsBody, limbs[(int)LimbId.UpperArmRight].PhysicsBody);
            DisableCollisions(limbs[(int)LimbId.UpperLegLeft].PhysicsBody, limbs[(int)LimbId.LowerLegLeft].PhysicsBody);
            DisableCollisions(limbs[(int)LimbId.UpperLegRight].PhysicsBody, limbs[(int)LimbId.LowerLegRight].PhysicsBody);
            DisableCollisions(limbs[(int)LimbId.UpperArmLeft].PhysicsBody, limbs[(int)LimbId.LowerArmLeft].PhysicsBody);
            DisableCollisions(limbs[(int)LimbId.UpperArmRight].PhysicsBody, limbs[(int)LimbId.LowerArmRight].PhysicsBody);

            if (type == RagdollType.Complex)
            {
                DisableCollisions(limbs[(int)LimbId.Torso].PhysicsBody, limbs[(int)LimbId.Hips].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.LowerLegLeft].PhysicsBody, limbs[(int)LimbId.FootLeft].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.LowerLegRight].PhysicsBody, limbs[(int)LimbId.FootRight].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.LowerArmLeft].PhysicsBody, limbs[(int)LimbId.HandLeft].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.LowerArmRight].PhysicsBody, limbs[(int)LimbId.HandRight].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.Hips].PhysicsBody, limbs[(int)LimbId.UpperLegLeft].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.Hips].PhysicsBody, limbs[(int)LimbId.UpperLegRight].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.Hips].PhysicsBody, limbs[(int)LimbId.UpperArmLeft].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.Hips].PhysicsBody, limbs[(int)LimbId.UpperArmRight].PhysicsBody);
            }

            // he's not double-jointed...
            DisableCollisions(limbs[(int)LimbId.Torso].PhysicsBody, limbs[(int)LimbId.LowerLegLeft].PhysicsBody);
            DisableCollisions(limbs[(int)LimbId.Torso].PhysicsBody, limbs[(int)LimbId.LowerLegRight].PhysicsBody);
            DisableCollisions(limbs[(int)LimbId.UpperArmLeft].PhysicsBody, limbs[(int)LimbId.UpperArmRight].PhysicsBody);
            DisableCollisions(limbs[(int)LimbId.UpperArmRight].PhysicsBody, limbs[(int)LimbId.UpperArmLeft].PhysicsBody);
            DisableCollisions(limbs[(int)LimbId.UpperArmLeft].PhysicsBody, limbs[(int)LimbId.Head].PhysicsBody);
            DisableCollisions(limbs[(int)LimbId.UpperArmRight].PhysicsBody, limbs[(int)LimbId.Head].PhysicsBody);
            DisableCollisions(limbs[(int)LimbId.UpperLegLeft].PhysicsBody, limbs[(int)LimbId.Head].PhysicsBody);
            DisableCollisions(limbs[(int)LimbId.UpperLegRight].PhysicsBody, limbs[(int)LimbId.Head].PhysicsBody);
            DisableCollisions(limbs[(int)LimbId.LowerLegLeft].PhysicsBody, limbs[(int)LimbId.Head].PhysicsBody);
            DisableCollisions(limbs[(int)LimbId.LowerLegRight].PhysicsBody, limbs[(int)LimbId.Head].PhysicsBody);
            DisableCollisions(limbs[(int)LimbId.LowerLegLeft].PhysicsBody, limbs[(int)LimbId.Head].PhysicsBody);
            DisableCollisions(limbs[(int)LimbId.LowerLegRight].PhysicsBody, limbs[(int)LimbId.Head].PhysicsBody);

            if (type == RagdollType.Complex)
            {
                DisableCollisions(limbs[(int)LimbId.Torso].PhysicsBody, limbs[(int)LimbId.FootLeft].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.Torso].PhysicsBody, limbs[(int)LimbId.FootRight].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.Hips].PhysicsBody, limbs[(int)LimbId.FootLeft].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.Hips].PhysicsBody, limbs[(int)LimbId.FootRight].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.Hips].PhysicsBody, limbs[(int)LimbId.LowerLegLeft].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.Hips].PhysicsBody, limbs[(int)LimbId.LowerLegRight].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.Hips].PhysicsBody, limbs[(int)LimbId.LowerArmLeft].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.Hips].PhysicsBody, limbs[(int)LimbId.LowerArmRight].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.FootLeft].PhysicsBody, limbs[(int)LimbId.HandLeft].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.FootRight].PhysicsBody, limbs[(int)LimbId.HandRight].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.FootLeft].PhysicsBody, limbs[(int)LimbId.Head].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.FootRight].PhysicsBody, limbs[(int)LimbId.Head].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.FootLeft].PhysicsBody, limbs[(int)LimbId.UpperArmLeft].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.FootRight].PhysicsBody, limbs[(int)LimbId.UpperArmRight].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.FootLeft].PhysicsBody, limbs[(int)LimbId.UpperArmRight].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.FootRight].PhysicsBody, limbs[(int)LimbId.UpperArmLeft].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.FootLeft].PhysicsBody, limbs[(int)LimbId.UpperLegLeft].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.FootRight].PhysicsBody, limbs[(int)LimbId.UpperLegRight].PhysicsBody);
            }

            foreach (HingeJoint joint in joints)
            {
                if (joint != null)
                    joint.EnableHinge();
                
            }

            foreach (PhysicObject limb in limbs)
            {
                if (limb != null)
                {
                    limb.PhysicsBody.CollisionSkin.SetMaterialProperties(0, new JigLibX.Collision.MaterialProperties(0.2f, 3.0f, 2.0f));
                    this.Game.Components.Add(limb);
                }
            }
           
        }

        public void PutToSleep()
        {
            foreach (PhysicObject limb in limbs)
            {
                if (limb != null)
                    limb.PhysicsBody.SetInactive();
            } 
        }

        private void MoveTorso(Vector3 pos)
        {
            Vector3 delta = pos - limbs[(int)LimbId.Torso].PhysicsBody.Position;
            foreach (PhysicObject limb in limbs)
            {
                if (limb != null)
                {
                    Vector3 origPos = limb.PhysicsBody.Position;

                    MoveTo(limb.PhysicsBody, origPos + delta, limb.PhysicsBody.Orientation);
                    //limb.PhysicsBody.MoveTo(origPos + delta, limb.PhysicsBody.Orientation);
                }
            }
        }

        public void MoveTo(Body BODY, Vector3 pos, Matrix orientation)
        {
            if (BODY.IsBodyEnabled && !BODY.IsActive)
            {
                BODY.SetActive();
            }

            BODY.Position = pos;
            BODY.Orientation = orientation;

            BODY.CopyCurrentStateToOld();

            if (BODY.CollisionSkin != null)
                BODY.CollisionSkin.SetTransform(ref BODY.oldTransform, ref BODY.transform);
        }

        public Vector3 Position
        {
            set { MoveTorso(value); }
            get { return limbs[(int)LimbId.Torso].PhysicsBody.Position; }
        }

        public List<Matrix> GetWorldMatrix()
        {
            List<Matrix> LM = new List<Matrix>();
            float SCALE = 1.0f;

            LM.Add(GetMatrix0(limbs[(int)LimbId.Torso].PhysicsBody, SCALE));
            LM.Add(GetMatrix0(limbs[(int)LimbId.Head].PhysicsBody, SCALE));
            LM.Add(GetMatrixX(limbs[(int)LimbId.UpperLegLeft].PhysicsBody, SCALE));
            LM.Add(GetMatrixX(limbs[(int)LimbId.UpperLegRight].PhysicsBody, SCALE));
            LM.Add(GetMatrixX(limbs[(int)LimbId.LowerLegLeft].PhysicsBody, SCALE));
            LM.Add(GetMatrixX(limbs[(int)LimbId.LowerLegRight].PhysicsBody, SCALE));
            LM.Add(GetMatrixZ(limbs[(int)LimbId.UpperArmLeft].PhysicsBody, SCALE));
            LM.Add(GetMatrixZ(limbs[(int)LimbId.UpperArmRight].PhysicsBody, SCALE));
            LM.Add(GetMatrixZ(limbs[(int)LimbId.LowerArmLeft].PhysicsBody, SCALE));
            LM.Add(GetMatrixZ(limbs[(int)LimbId.LowerArmRight].PhysicsBody, SCALE));
            

            return
                LM;
        }

        public Matrix GetMatrix(Body BODY, float SCALE)
        {
            return
                GetMatrixZ(BODY,SCALE);
        }

        public Matrix GetMatrix1(Body BODY, float SCALE)
        {

            return
                Matrix.CreateScale(SCALE) *
                BODY.Orientation *
                Matrix.CreateTranslation(BODY.Position);
        }

        public Matrix GetWorld()
        {
            Vector3 p = limbs[(int)LimbId.Torso].PhysicsBody.Position;
            Matrix orient = (limbs[(int)LimbId.Torso].PhysicsBody.Orientation);
            p += orient.Down * 3;
            return
                Matrix.CreateScale(Scale) 
                //*
                //(limbs[(int)LimbId.Torso].PhysicsBody).CollisionSkin.GetPrimitiveLocal(0).Transform.Orientation
                *
                (limbs[(int)LimbId.Torso].PhysicsBody.Orientation)
                *
                Matrix.CreateFromAxisAngle(orient.Up, MathHelper.ToRadians(-90))
                * 
                Matrix.CreateTranslation(p);
        }

        public Matrix GetMatrix2(Body BODY, float SCALE)
        {
            return
                Matrix.CreateScale(SCALE) *
                BODY.CollisionSkin.GetPrimitiveLocal(0).Transform.Orientation * 
                Matrix.CreateRotationX(-MathHelper.ToRadians(90) ) *
                BODY.Orientation *
                Matrix.CreateTranslation(BODY.Position);          
                        
        }

        public Matrix GetMatrixZ(Body BODY, float SCALE)
        {
            Matrix orient = (limbs[(int)LimbId.Torso].PhysicsBody.Orientation);
            return
                
                //BODY.CollisionSkin.GetPrimitiveLocal(0).Transform.Orientation
                //* 
                //Matrix.CreateRotationZ(-MathHelper.ToRadians(90))
                //*

                BODY.Orientation
                *
                Matrix.CreateFromAxisAngle(orient.Left, MathHelper.ToRadians(-90))
                
                *
                Matrix.Invert(limbs[(int)LimbId.Torso].PhysicsBody.Orientation);
        }

        public Matrix GetMatrixX(Body BODY, float SCALE)
        {
            Matrix orient = (limbs[(int)LimbId.Torso].PhysicsBody.Orientation);

            return
                
                //BODY.CollisionSkin.GetPrimitiveLocal(0).Transform.Orientation * Matrix.CreateRotationX(-MathHelper.ToRadians(90))
                //*
                BODY.Orientation 
                //*
                //Matrix.CreateFromAxisAngle(orient.Backward, MathHelper.ToRadians(-90))
                *
                Matrix.Invert(limbs[(int)LimbId.Torso].PhysicsBody.Orientation);
        }

        public Matrix GetMatrix0(Body BODY, float SCALE)
        {
            return

                //BODY.CollisionSkin.GetPrimitiveLocal(0).Transform.Orientation
                //*
                BODY.Orientation
                *
                Matrix.Invert(limbs[(int)LimbId.Torso].PhysicsBody.Orientation);
        }

        public override void ApplyEffects(Microsoft.Xna.Framework.Graphics.BasicEffect effect)
        {
            //
        }
    }
}
