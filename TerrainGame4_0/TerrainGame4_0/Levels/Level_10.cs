using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TerrainGame4_0
{
    public class Level_10 : LevelCode
    {
        public Level_10()
            : base()
        {
        }

        public override void Load()
        {
            terrainMap = "Terrain\\level10Ter";
            texture1 = "Terrain\\004_1lower";
            texture2 = "Terrain\\004_2mid";
            texture3 = "Terrain\\004_3mid_high";
            texture4 = "Terrain\\004_4high";
            decalTexture = "Terrain\\decal_empty";
            terrainScale = 10.0f;

            cloudTexture = "Terrain\\004_clouds";
            FogColor = new Color(197, 43, 160, 0);

            playerSpawn = new Spawn(new Vector3(128f, 10, -129.05f));
            cameraSpawn = new Spawn(new Vector3(131.4f, 0, -125.4499f));
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
            string scale = "0.05";

            //------------------------------------


            //------------------------------------

            gEvent = new GameEvent();
            gEvent.EventType = GameEvent.GameEventType.PUT_ENEMY;

            spawn = new EnemySpawn();

            spawn.Position = new Vector3(112.65f, 10, -123.75f) * terrainScale;
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

            spawn.Position = new Vector3(122.85f, 10, -117.80000000000001f) * terrainScale;
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

            spawn.Position = new Vector3(140.4f, 10, -109.94999999999999f) * terrainScale;
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

            spawn.Position = new Vector3(146f, 10, -115.6f) * terrainScale;
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

            spawn.Position = new Vector3(111.9f, 10, -149.35f) * terrainScale;
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

            spawn.Position = new Vector3(119.35f, 10, -152.95f) * terrainScale;
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

            spawn.Position = new Vector3(115.7f, 10, -116.35f) * terrainScale;
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

            spawn.Position = new Vector3(140.95f, 10, -137.85f) * terrainScale;
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

            spawn.Position = new Vector3(137.2f, 10, -140.85f) * terrainScale;
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

            spawn.Position = new Vector3(117.7f, 10, -145.1f) * terrainScale;
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
