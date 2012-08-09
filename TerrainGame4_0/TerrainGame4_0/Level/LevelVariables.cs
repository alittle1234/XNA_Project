using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TerrainGame4_0
{
    /// <summary>
    /// Container of variables for access in level.
    /// </summary>
    public class LevelVariables
    {
        public TerrainEngine terrainEngine = null;
        public TriangleMeshActor terrainActor = null;

        public Model playerModel = null;
        public Texture2D[] playerTextures = null;

        public Model rocketModel = null;
        public Texture2D[] rocketTextures = null;

        public PlayerTank Player = null;

        public MouseCamera MouseCam = null;

        public LevelVariables()
        {

        }
    }
}
