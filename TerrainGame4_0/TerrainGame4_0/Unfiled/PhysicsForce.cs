using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using JigLibX.Physics;
using JigLibX.Geometry;
using JigLibX.Math;
using JigLibX.Utils;

namespace TerrainGame4_0
{
   
    public class ForceControler : Controller
    {
        private Body body0;
        private Vector3 force0 = new Vector3();
        public Vector3 Force
        {
            get { return force0; }

            set { force0 = value; }
        }

        private Vector3 torque0 = new Vector3();
        public Vector3 Torque
        {
            get { return torque0; }

            set { torque0 = value; }
        }

        private bool WorldF;

        public ForceControler()
        {
        }

        public ForceControler(Body body, Vector3 force, bool W_or_B)
        {
            force0 = force;
            WorldF = W_or_B;
            Initialize(body);
        }
        

        public ForceControler(Body body, Vector3 force, Vector3 torque)
        {
            force0 = force;
            torque0 = torque;
            Initialize(body);
        }

        public void Initialize(Body body0)
        {
            EnableController();
            this.body0 = body0;
        }

        public void ClearControler()
        {
            body0 = null;
            ClearForce();
        }

        public void ClearForce()
        {
            force0 = new Vector3();
            torque0 = new Vector3();
        }

        public override void UpdateController(float dt)
        {
            if (body0 == null)
                return;

            if (force0 != null && force0 != Vector3.Zero)
            {
                if (WorldF)
                    body0.AddWorldForce(force0);
                else
                    body0.AddBodyForce(force0);
                if (!body0.IsActive)
                    body0.SetActive();
            }
            if (torque0 != null && torque0 != Vector3.Zero)
            {
                if (WorldF)
                    body0.AddWorldTorque(torque0);
                else
                    body0.AddBodyTorque(torque0);
                if (!body0.IsActive)
                    body0.SetActive();
            }

        }


    }

}
