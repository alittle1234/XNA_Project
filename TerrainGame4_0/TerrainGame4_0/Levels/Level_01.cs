using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TerrainGame4_0
{
    public class Level_1 : LevelCode
    {
        

        public Level_1() : base( )
        { 
            
        }

        public override void Load()
        {
            terrainMap = "Terrain\\level1Ter";
            texture1 = "Terrain\\001_1mud";
            texture2 = "Terrain\\001_2grass";
            texture3 = "Terrain\\001_3grass_rock";
            texture4 = "Terrain\\001_4rock";
            decalTexture = "Terrain\\decal_empty";
            terrainScale = 10.0f;

            playerSpawn = new Spawn(new Vector3(132.15f,0,-130.55f));
            cameraSpawn = new Spawn(new Vector3(132.15f,0,-130.55f));
            FlagsList = new List<EventFlag>();
            LevelConditions = new List<Condition>();
            //LevelConditionsArray;
            //ConditionsArray;

            NormalThreshold = 0.1f;
            DepthThreshold = 0.01f;
            NormalSensitivity = 1.0f;
            DepthSensitivity = 50.0f;
            EdgeWidth = 1.0f;
            EdgeIntensity = 2.5f;

            Missiles = 10;

            EventFlag flag = new EventFlag(
                new Vector2(16, -4),
                new Vector2(20, -8),
                new Vector2(8, -8),
                new Vector2(16, -16));


            GameEvent gEvent = new GameEvent();
            EnemySpawn spawn = new EnemySpawn();

            string type = "BUNKER";
            // string type = "DEPOT";
            string scale = "0.05";
            //------------------------------------

            gEvent = new GameEvent();
            gEvent.EventType = GameEvent.GameEventType.PUT_ENEMY;

            spawn = new EnemySpawn();
            spawn.Position = new Vector3(132.55f, 0, -117.5f) * terrainScale;
            spawn.Rotation = new Vector3(0, 0, 0);

            type = "DEPOT";
            scale = "0.05";
            spawn.EnemyStats.Add("Type", type);
            spawn.EnemyStats.Add("Scale", scale);

            gEvent.SpawnPoint = spawn;

            flag.Events.Add(gEvent);
            //------------------------------------

            gEvent = new GameEvent();
            gEvent.EventType = GameEvent.GameEventType.PUT_ENEMY;

            spawn = new EnemySpawn();
            spawn.Position = new Vector3(125.75f, 0, -139.45f) * terrainScale;
            spawn.Rotation = new Vector3(0, 0, 0);

            type = "BUNKER";
            scale = "0.05";
            spawn.EnemyStats.Add("Type", type);
            spawn.EnemyStats.Add("Scale", scale);

            gEvent.SpawnPoint = spawn;

            flag.Events.Add(gEvent);
            //------------------------------------

            gEvent = new GameEvent();
            gEvent.EventType = GameEvent.GameEventType.PUT_ENEMY;

            spawn = new EnemySpawn();
            spawn.Position = new Vector3(137.2f, 0, -133.15f) * terrainScale;
            spawn.Rotation = new Vector3(0, 0, 0);

            type = "DEPOT";
            scale = "0.05";
            spawn.EnemyStats.Add("Type", type);
            spawn.EnemyStats.Add("Scale", scale);

            gEvent.SpawnPoint = spawn;

            flag.Events.Add(gEvent);
            //------------------------------------

            gEvent = new GameEvent();
            gEvent.EventType = GameEvent.GameEventType.PUT_ENEMY;

            spawn = new EnemySpawn();
            spawn.Position = new Vector3(120.7f, 0, -125.35f) * terrainScale;
            spawn.Rotation = new Vector3(0, 0, 0);

            type = "BUNKER";
            scale = "0.05";
            spawn.EnemyStats.Add("Type", type);
            spawn.EnemyStats.Add("Scale", scale);

            gEvent.SpawnPoint = spawn;

            flag.Events.Add(gEvent);
            //------------------------------------
           

            gEvent = new GameEvent();
            gEvent.EventType = GameEvent.GameEventType.PUT_ENEMY;

            spawn = new EnemySpawn();
            spawn.Position = new Vector3(147.95f, 0, -126.65f) * terrainScale;
            spawn.Rotation = new Vector3(0, 0, 0);

            type = "BUNKER";
            scale = "0.05";
            spawn.EnemyStats.Add("Type", type);
            spawn.EnemyStats.Add("Scale", scale);

            gEvent.SpawnPoint = spawn;

            flag.Events.Add(gEvent);
            //------------------------------------

            FlagsList.Add(flag);
        }

    }
}
