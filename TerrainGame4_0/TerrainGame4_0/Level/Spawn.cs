using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TerrainGame4_0
{
    
    public class Spawn
    {
        public Vector3 Position = new Vector3();
        public Vector3 Rotation = new Vector3();
        public float leftRightRotation = 0.0f;
        public float upDownRotation = 0.0f;

        public Spawn()
        {
        }

        public Spawn(Vector3 position)
        {
            Position = position;
        }

    }
}
