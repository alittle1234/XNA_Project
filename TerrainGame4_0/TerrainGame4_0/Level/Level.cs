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
    public class Level
    {
        //structure containg level specific data
        
        public String terrainMap = null;
        public String texture1 = null;
        public String texture2 = null;
        public String texture3 = null;
        public String texture4 = null;
        public String decalTexture = null;
        public String cloudTexture = "Terrain\\clouds";
        public float terrainScale = 0.0f;

        public Spawn playerSpawn = new Spawn();
        public Spawn cameraSpawn = new Spawn();
        public List<EventFlag> FlagsList = new List<EventFlag>();
        public List<Condition> LevelConditions = new List<Condition>();
        public Condition[] LevelConditionsArray;
        public Array ConditionsArray;

        public float NormalThreshold, DepthThreshold,
            NormalSensitivity, DepthSensitivity,
            EdgeWidth, EdgeIntensity;

        public int Missiles = 0;

        public float Time = 0.0f;

        public Color FogColor = new Color(140, 229, 255, 0);

        public Level()
        {

        }


    }
}
