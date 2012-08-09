using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TerrainGame4_0
{
    public class Level_26 : LevelCode
    {
        

        public Level_26() : base( )
        { 
            
        }

        public override void Load()
        {
            terrainMap = "Terrain\\level26Ter";
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

            playerSpawn = new Spawn(new Vector3(128.3f, 10, -127.44999999999999f));

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

            spawn.Position = new Vector3(126.4f, 10, -148.3f) * terrainScale;
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

            spawn.Position = new Vector3(146.95f, 10, -144.6f) * terrainScale;
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

            spawn.Position = new Vector3(152.9f, 10, -127.1f) * terrainScale;
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

            spawn.Position = new Vector3(106.8f, 10, -139.35f) * terrainScale;
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

            spawn.Position = new Vector3(130f, 10, -106.6f) * terrainScale;
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

            spawn.Position = new Vector3(108.55f, 10, -117.15f) * terrainScale;
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

            spawn.Position = new Vector3(127f, 5, -140.25f) * terrainScale;
            spawn.Rotation = new Vector3(0, 0, 0);

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

            spawn.Position = new Vector3(141.3f, 5, -127.4f) * terrainScale;
            spawn.Rotation = new Vector3(1.57f, 0, 0);

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

            spawn.Position = new Vector3(117.55f, 5, -121.80000000000001f) * terrainScale;
            spawn.Rotation = new Vector3((1.57f * 3/2), 0, 0);

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

            spawn.Position = new Vector3(116.55f, 5, -133.65f) * terrainScale;
            spawn.Rotation = new Vector3(1.57f * 1/2, 0, 0);

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

            spawn.Position = new Vector3(138.35f, 5, -136.85f) * terrainScale;
            spawn.Rotation = new Vector3(1.57f * 3/2, 0, 0);

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

            spawn.Position = new Vector3(128.95f, 5, -117.05000000000001f) * terrainScale;
            spawn.Rotation = new Vector3(0, 0, 0);

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
