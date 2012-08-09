using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TerrainGame4_0
{
    public class Level_4 : LevelCode
    {
        public Level_4()
            : base()
        {
        }

        public override void Load()
        {
            terrainMap = "Terrain\\level4Ter";
            texture1 = "Terrain\\001_1mud";
            texture2 = "Terrain\\001_2grass";
            texture3 = "Terrain\\001_3grass_rock";
            texture4 = "Terrain\\001_4rock";
            decalTexture = "Terrain\\decal_empty";
            terrainScale = 10.0f;

            playerSpawn = new Spawn(new Vector3(129.4f, 10, -125.44999999999999f));
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

            EventFlag flag = new EventFlag(
                new Vector2(16, -4),
                new Vector2(20, -8),
                new Vector2(8, -8),
                new Vector2(16, -16));

            GameEvent gEvent = new GameEvent();
            gEvent.EventType = GameEvent.GameEventType.PUT_ENEMY;

            EnemySpawn spawn = new EnemySpawn();

            string type = "BUNKER";
            string scale = "0.05";

            //------------------------------------

            gEvent = new GameEvent();
            gEvent.EventType = GameEvent.GameEventType.PUT_ENEMY;

            spawn = new EnemySpawn();
            spawn.Position = new Vector3(119.65f, 0, -145.1f) * terrainScale;
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
            spawn.Position = new Vector3(131.6f, 0, -137.25f) * terrainScale;
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
            spawn.Position = new Vector3(115.3f, 0, -109.25f) * terrainScale;
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
            spawn.Position = new Vector3(146.6f, 0, -134.65f) * terrainScale;
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
            spawn.Position = new Vector3(128.95f, 0, -117.25f) * terrainScale;
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
            spawn.Position = new Vector3(113.45f, 0, -129.65f) * terrainScale;
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
            spawn.Position = new Vector3(146.45f, 0, -107.8f) * terrainScale;
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
