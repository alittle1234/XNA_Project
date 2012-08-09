using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TerrainGame4_0
{
    public class Level_24 : LevelCode
    {
        

        public Level_24() : base( )
        { 
            
        }

        public override void Load()
        {
            terrainMap = "Terrain\\level24Ter";
            texture1 = "Terrain\\001_1mud";
            texture2 = "Terrain\\001_2grass";
            texture3 = "Terrain\\001_3grass_rock";
            texture4 = "Terrain\\001_4rock";
            decalTexture = "Terrain\\decal_empty";
            terrainScale = 10.0f;

            playerSpawn = new Spawn(new Vector3(135.45f, 10, -130.25f));
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

            EventFlag flag = new EventFlag(
                new Vector2(16, -4),
                new Vector2(20, -8),
                new Vector2(8, -8),
                new Vector2(16, -16));


            GameEvent gEvent = new GameEvent();
            EnemySpawn spawn = new EnemySpawn();
            spawn.Position = new Vector3(0, 0, 0);
            spawn.Rotation = new Vector3(0, 0, 0);

            string type = "BUNKER";
            string scale = "0.05";
            //------------------------------------


            //------------------------------------

            gEvent = new GameEvent();
            gEvent.EventType = GameEvent.GameEventType.PUT_ENEMY;

            spawn = new EnemySpawn();

            spawn.Position = new Vector3(129.5f, 10, -166.05f) * terrainScale;
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

            spawn.Position = new Vector3(118.75f, 10, -158.2f) * terrainScale;
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

            spawn.Position = new Vector3(134.6f, 10, -158.3f) * terrainScale;
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

            spawn.Position = new Vector3(155.45f, 10, -111.9f) * terrainScale;
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

            spawn.Position = new Vector3(147.5f, 10, -112.80000000000001f) * terrainScale;
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

            spawn.Position = new Vector3(154.15f, 10, -119.05000000000001f) * terrainScale;
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

            spawn.Position = new Vector3(132.45f, 9, -147.95f) * terrainScale;
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

            spawn.Position = new Vector3(144.1f, 8, -122.19999999999999f) * terrainScale;
            spawn.Rotation = new Vector3(3.14f/4, 0, 0);

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
