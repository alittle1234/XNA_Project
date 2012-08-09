using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TerrainGame4_0
{
    public class LevelCode
    {
        
        public String terrainMap = "Undefined";
        public String texture1;
        public String texture2;
        public String texture3;
        public String texture4;
        public String decalTexture = "Terrain\\decal_empty";
        public String cloudTexture = "Terrain\\001_clouds";
        public float terrainScale = 10.0f;

        public Spawn playerSpawn = new Spawn(new Vector3(127.4f, 0f, -138.55f));
        public Spawn cameraSpawn = new Spawn(new Vector3(127.4f, 10f, -138.55f));
        public List<EventFlag> FlagsList = new List<EventFlag>();
        public List<Condition> LevelConditions = new List<Condition>();
        public Condition[] LevelConditionsArray;
        public Array ConditionsArray;

        public float NormalThreshold = 0.1f;
        public float DepthThreshold = 0.01f;
        public float NormalSensitivity = 1.0f;
        public float DepthSensitivity = 50.0f;
        public float EdgeWidth = 1.0f;
        public float EdgeIntensity = 2.5f;

        public Color FogColor = new Color(140, 229, 255, 0);

        public int Missiles = 2;

        public float Time = 0.0f;

        public LevelCode()
        {
            
        }

        public virtual void Load()
        { 

        }
    }
}
