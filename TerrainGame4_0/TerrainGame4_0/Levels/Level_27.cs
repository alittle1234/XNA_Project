using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TerrainGame4_0
{
    public class Level_27 : LevelCode
    {
        

        public Level_27() : base( )
        { 
            
        }

        public override void Load()
        {
            terrainMap = "Terrain\\level27Ter";
            texture1 = "Terrain\\002_1sand";
            texture2 = "Terrain\\002_2grass";
            texture3 = "Terrain\\002_3grass_rock";
            texture4 = "Terrain\\002_4rock";
            terrainScale = 10.0f;

            cloudTexture = "Terrain\\002_clouds_green";
            FogColor = new Color(181, 228, 210, 0);

            //playerSpawn = new Spawn(new Vector3(135.3f, 10, -129.95f));
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

            Time = 1000.0f * 130;

            playerSpawn = new Spawn(new Vector3(105.3f, 10, -107.69999999999999f));

            EventFlag flag = new EventFlag(
                new Vector2(16, -4),
                new Vector2(20, -8),
                new Vector2(8, -8),
                new Vector2(16, -16));


            GameEvent gEvent = new GameEvent();
            EnemySpawn spawn = new EnemySpawn();

            string type = "BUNKER";
            string scale = "0.05";
            //------------------------------------


            //------------------------------------

            gEvent = new GameEvent();
            gEvent.EventType = GameEvent.GameEventType.PUT_ENEMY;

            spawn = new EnemySpawn();

            spawn.Position = new Vector3(113.9f, 10, -123.55000000000001f) * terrainScale;
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

            spawn.Position = new Vector3(129.2f, 10, -132.35f) * terrainScale;
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

            spawn.Position = new Vector3(136.8f, 10, -123.6f) * terrainScale;
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

            spawn.Position = new Vector3(108.3f, 10, -139.6f) * terrainScale;
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

            spawn.Position = new Vector3(134.4f, 10, -94.1f) * terrainScale;
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

            spawn.Position = new Vector3(92.3f, 10, -123.4f) * terrainScale;
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

            spawn.Position = new Vector3(118.35f, 12, -121.15f) * terrainScale;
            spawn.Rotation = new Vector3(-3.14f / 4, 0, 0);

            type = "RING";
            scale = "0.021";

            spawn.EnemyStats.Add("Type", type);
            spawn.EnemyStats.Add("Scale", scale);

            gEvent.SpawnPoint = spawn;

            flag.Events.Add(gEvent);

            //------------------------------------

            gEvent = new GameEvent();
            gEvent.EventType = GameEvent.GameEventType.PUT_ENEMY;

            spawn = new EnemySpawn();

            spawn.Position = new Vector3(124.05f, 10, -126.94999999999999f) * terrainScale;
            spawn.Rotation = new Vector3(-3.14f/4, 0, 0);

            type = "RING";
            scale = "0.021";

            spawn.EnemyStats.Add("Type", type);
            spawn.EnemyStats.Add("Scale", scale);

            gEvent.SpawnPoint = spawn;

            flag.Events.Add(gEvent);

            //------------------------------------

            gEvent = new GameEvent();
            gEvent.EventType = GameEvent.GameEventType.PUT_ENEMY;

            spawn = new EnemySpawn();

            spawn.Position = new Vector3(127.45f, 12, -118.94999999999999f) * terrainScale;
            spawn.Rotation = new Vector3(-3.14f / 4, 0, 0);

            type = "RING";
            scale = "0.021";

            spawn.EnemyStats.Add("Type", type);
            spawn.EnemyStats.Add("Scale", scale);

            gEvent.SpawnPoint = spawn;

            flag.Events.Add(gEvent);

            //------------------------------------

            gEvent = new GameEvent();
            gEvent.EventType = GameEvent.GameEventType.PUT_ENEMY;

            spawn = new EnemySpawn();

            spawn.Position = new Vector3(117.65f, 16, -101.9f) * terrainScale;
            spawn.Rotation = new Vector3(3.14f / 4, 0, 0);

            type = "RING";
            scale = "0.021";

            spawn.EnemyStats.Add("Type", type);
            spawn.EnemyStats.Add("Scale", scale);

            gEvent.SpawnPoint = spawn;

            flag.Events.Add(gEvent);

            //------------------------------------

            gEvent = new GameEvent();
            gEvent.EventType = GameEvent.GameEventType.PUT_ENEMY;

            spawn = new EnemySpawn();

            spawn.Position = new Vector3(126.25f, 14, -98f) * terrainScale;
            spawn.Rotation = new Vector3(3.14f / 4, 0, 0);

            type = "RING";
            scale = "0.021";

            spawn.EnemyStats.Add("Type", type);
            spawn.EnemyStats.Add("Scale", scale);

            gEvent.SpawnPoint = spawn;

            flag.Events.Add(gEvent);


            FlagsList.Add(flag);
        }

    }
}
