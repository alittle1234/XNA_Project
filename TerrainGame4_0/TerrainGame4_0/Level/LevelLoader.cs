using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using System.Collections;
using System.Xml;

namespace TerrainGame4_0
{
    public class LevelLoader
    {
        Dictionary<string, string> Level_Load_List;
        LoadingStage Load_Stage;
        ResourceList LoadedResources;
        public Level myLevel = new Level();
        ParentGame parentGame;

        public LevelLoader(ParentGame parentGame)
        {
            this.parentGame = parentGame;
            Load_Stage = parentGame.Loading_Stage;
            this.LoadedResources = parentGame.LoadedResources;
        }

        public Dictionary<string, string> GetLevelLoadList()
        {
            return Level_Load_List;
        }

        public float CheckProgress()
        {
            return Load_Stage.CheckProgress();
        }

        public void Stop()
        {
            Load_Stage.EndLoading();
        }

        public void LoadLevel(int Number)
        {
            //GetLevelFromXML();

            GetLevelFromLevelCode(Number);

            if (Load_Stage != null)
            {


                //Load_Stage.backgroundThread.Abort();
                Load_Stage.EndLoading();

                //while (Load_Stage.backgroundThread.IsAlive)
                //{
                //    //wait?
                //}
                Load_Stage.SetThreads();
            }

            //Loaded = false;
            //LOADING = false;

            Load_Stage = new LoadingStage(LoadedResources, parentGame, false);

            
            // set level_load_list
            GetLoadList();

            List<String> resList = GetLevelTextures();
            Load_Stage.AddToTex2DList(resList);

            resList = GetLevelModels();
            Load_Stage.AddToModelList(resList);

            Load_Stage.LoadContent();
        }

        private void GetLevelFromLevelCode(int Number)
        {
            LevelCode  LEVEL;
            switch (Number)
            {
                case (1):
                    Level_1 level1  = new Level_1();
                    level1.Load();
                    LEVEL = level1;
                    break;
                case (2):
                    Level_2 level2 = new Level_2();
                    level2.Load();
                    LEVEL = level2;
                    break;
                case (3):
                    Level_3 level3 = new Level_3();
                    level3.Load();
                    LEVEL = level3;
                    break;
                case (4):
                    Level_4 level4 = new Level_4();
                    level4.Load();
                    LEVEL = level4;
                    break;
                case (5):
                    Level_5 level5 = new Level_5();
                    level5.Load();
                    LEVEL = level5;
                    break;
                case (6):
                    Level_6 level6 = new Level_6();
                    level6.Load();
                    LEVEL = level6;
                    break;
                case (7):
                    Level_7 level7 = new Level_7();
                    level7.Load();
                    LEVEL = level7;
                    break;
                case (8):
                    Level_8 level8 = new Level_8();
                    level8.Load();
                    LEVEL = level8;
                    break;
                case (9):
                    Level_9 level9 = new Level_9();
                    level9.Load();
                    LEVEL = level9;
                    break;
                case (10):
                    Level_10 level10 = new Level_10();
                    level10.Load();
                    LEVEL = level10;
                    break;
                case (11):
                    Level_11 level11 = new Level_11();
                    level11.Load();
                    LEVEL = level11;
                    break;
                case (12):
                    Level_12 level12 = new Level_12();
                    level12.Load();
                    LEVEL = level12;
                    break;
                case (21):
                    Level_21 level21 = new Level_21();
                    level21.Load();
                    LEVEL = level21;
                    break;
                case (22):
                    Level_22 level22 = new Level_22();
                    level22.Load();
                    LEVEL = level22;
                    break;
                case (23):
                    Level_23 level23 = new Level_23();
                    level23.Load();
                    LEVEL = level23;
                    break;
                case (24):
                    Level_24 level24 = new Level_24();
                    level24.Load();
                    LEVEL = level24;
                    break;
                case (25):
                    Level_25 level25 = new Level_25();
                    level25.Load();
                    LEVEL = level25;
                    break;
                case (26):
                    Level_26 level26 = new Level_26();
                    level26.Load();
                    LEVEL = level26;
                    break;
                case (27):
                    Level_27 level27 = new Level_27();
                    level27.Load();
                    LEVEL = level27;
                    break;
                case (28):
                    Level_28 level28 = new Level_28();
                    level28.Load();
                    LEVEL = level28;
                    break;
                case (29):
                    Level_29 level29 = new Level_29();
                    level29.Load();
                    LEVEL = level29;
                    break;
                case (30):
                    Level_30 level30 = new Level_30();
                    level30.Load();
                    LEVEL = level30;
                    break;
                case (31):
                    Level_31 level31 = new Level_31();
                    level31.Load();
                    LEVEL = level31;
                    break;
                case (32):
                    Level_32 level32 = new Level_32();
                    level32.Load();
                    LEVEL = level32;
                    break;
                default: 
                    Level_1 leveld = new Level_1();
                    leveld.Load();
                    LEVEL = leveld;                    
                    break;                
            }

            myLevel.terrainMap = LEVEL.terrainMap;
            myLevel.texture1 = LEVEL.texture1;
            myLevel.texture2 = LEVEL.texture2;
            myLevel.texture3 = LEVEL.texture3;
            myLevel.texture4 = LEVEL.texture4;
            myLevel.decalTexture = LEVEL.decalTexture;

            myLevel.terrainScale = LEVEL.terrainScale;

            myLevel.playerSpawn.Position = LEVEL.playerSpawn.Position * LEVEL.terrainScale;
            myLevel.cameraSpawn.Position = LEVEL.cameraSpawn.Position * LEVEL.terrainScale;

            myLevel.NormalThreshold = LEVEL.NormalThreshold;
            myLevel.DepthThreshold = LEVEL.DepthThreshold;
            myLevel.NormalSensitivity = LEVEL.NormalSensitivity;
            myLevel.DepthSensitivity = LEVEL.DepthSensitivity;
            myLevel.EdgeWidth = LEVEL.EdgeWidth;
            myLevel.EdgeIntensity = LEVEL.EdgeIntensity;

            myLevel.FlagsList = LEVEL.FlagsList;

            myLevel.Missiles = LEVEL.Missiles;

            myLevel.Time = LEVEL.Time;

            myLevel.FogColor = LEVEL.FogColor;

            myLevel.cloudTexture = LEVEL.cloudTexture;
        }

        void GetLoadList()
        {
            Level_Load_List = new Dictionary<string, string>();
            Level_Load_List.Add("Terrain", myLevel.terrainMap);
            Level_Load_List.Add("TerrainTexture1", myLevel.texture1);
            Level_Load_List.Add("TerrainTexture2", myLevel.texture2);
            Level_Load_List.Add("TerrainTexture3", myLevel.texture3);
            Level_Load_List.Add("TerrainTexture4", myLevel.texture4);
            Level_Load_List.Add("DecalTexture", myLevel.decalTexture);

            Level_Load_List.Add("PlayerTankModel", "Model\\artillery2");
            Level_Load_List.Add("PlayerRocketModel", "Model\\bullet");

            Level_Load_List.Add("SkyDome", "Terrain\\sphere");
            Level_Load_List.Add("SkyTexture", myLevel.cloudTexture);

            Level_Load_List.Add("BunkerModel", "Model\\building_tower");
            Level_Load_List.Add("BunkerDestroyed", "Model\\building_tower_base");
            Level_Load_List.Add("BunkerPiece1", "Model\\building_tower_front1");
            Level_Load_List.Add("BunkerPiece2", "Model\\building_tower_front2");
            Level_Load_List.Add("BunkerPiece3", "Model\\building_tower_front3");
            Level_Load_List.Add("BunkerPiece4", "Model\\building_tower_front4");

            Level_Load_List.Add("DepotModel", "Model\\fueldepot");
            Level_Load_List.Add("DepotDestroyed", "Model\\fueldepot_base");
            Level_Load_List.Add("DepotPiece1", "Model\\fueldepot_1");
            Level_Load_List.Add("DepotPiece2", "Model\\fueldepot_2");
            Level_Load_List.Add("DepotPiece3", "Model\\fueldepot_3");
            Level_Load_List.Add("DepotPiece4", "Model\\fueldepot_4");

            Level_Load_List.Add("Ring", "Model\\ring_Sub");
            //Level_Load_List.Add("RingTexture1", "Ring\\RingTexture1");
            //Level_Load_List.Add("RingTexture2", "Model\\ring_Sub_Cube2");            

            Level_Load_List.Add("RadarTexture", "Radar\\radar");
            Level_Load_List.Add("BlipTexture", "Radar\\blip");

        }

        List<String> GetLevelTextures()
        {
            List<String> toAdd = new List<string>();

            toAdd.Add(Level_Load_List["Terrain"]);
            toAdd.Add(Level_Load_List["TerrainTexture1"]);
            toAdd.Add(Level_Load_List["TerrainTexture2"]);
            toAdd.Add(Level_Load_List["TerrainTexture3"]);
            toAdd.Add(Level_Load_List["TerrainTexture4"]);
            toAdd.Add(Level_Load_List["DecalTexture"]);
            toAdd.Add(Level_Load_List["SkyTexture"]);
            toAdd.Add(Level_Load_List["RadarTexture"]);
            toAdd.Add(Level_Load_List["BlipTexture"]);
            //toAdd.Add(Level_Load_List["RingTexture1"]);
            //toAdd.Add(Level_Load_List["RingTexture2"]);

            return toAdd;
        }

        List<String> GetLevelModels()
        {
            List<String> toAdd = new List<string>();
            toAdd.Add(Level_Load_List["PlayerTankModel"]);
            toAdd.Add(Level_Load_List["PlayerRocketModel"]);
            toAdd.Add(Level_Load_List["SkyDome"]);

            toAdd.Add(Level_Load_List["BunkerModel"]);
            toAdd.Add(Level_Load_List["BunkerDestroyed"]);
            toAdd.Add(Level_Load_List["BunkerPiece1"]);
            toAdd.Add(Level_Load_List["BunkerPiece2"]);
            toAdd.Add(Level_Load_List["BunkerPiece3"]);
            toAdd.Add(Level_Load_List["BunkerPiece4"]);

            toAdd.Add(Level_Load_List["DepotModel"]);
            toAdd.Add(Level_Load_List["DepotDestroyed"]);
            toAdd.Add(Level_Load_List["DepotPiece1"]);
            toAdd.Add(Level_Load_List["DepotPiece2"]);
            toAdd.Add(Level_Load_List["DepotPiece3"]);
            toAdd.Add(Level_Load_List["DepotPiece4"]);

            toAdd.Add(Level_Load_List["Ring"]);

            return toAdd;
        }

        string PATH = "..\\..\\..\\..\\TerrainGame4_0Content\\";
        void GetLevelFromXML()
        {
            //TerrainTutorial\bin\x86\Debug
            string file = "Level.xml";
            XmlReader textReader;// = new XmlReader("..\\..\\..\\Content\\" + file);
            textReader = XmlReader.Create(PATH + file);

            //C:\Users\Admin\Documents\Visual Studio 2010\Projects\TerrainGame4_0\TerrainGame4_0\TerrainGame4_0Content

            myLevel.terrainMap = GetTextAt("map", file);
            myLevel.texture1 = GetTextAt("texture1", file);
            myLevel.texture2 = GetTextAt("texture2", file);
            myLevel.texture3 = GetTextAt("texture3", file);
            myLevel.texture4 = GetTextAt("texture4", file);
            myLevel.decalTexture = GetTextAt("decal_texture", file);

            myLevel.terrainScale = Convert.ToSingle(GetTextAt("terrain_scale", file));

            myLevel.playerSpawn.Position = GetVector3At("player_spawn", file) * myLevel.terrainScale;
            myLevel.cameraSpawn.Position = GetVector3At("camera_spawn", file) * myLevel.terrainScale;

            myLevel.NormalThreshold = Convert.ToSingle(GetTextAt("NormalThreshold", file));
            myLevel.DepthThreshold = Convert.ToSingle(GetTextAt("DepthThreshold", file));
            myLevel.NormalSensitivity = Convert.ToSingle(GetTextAt("NormalSensitivity", file));
            myLevel.DepthSensitivity = Convert.ToSingle(GetTextAt("DepthSensitivity", file));
            myLevel.EdgeWidth = Convert.ToSingle(GetTextAt("EdgeWidth", file));
            myLevel.EdgeIntensity = Convert.ToSingle(GetTextAt("EdgeIntensity", file));

            myLevel.FlagsList = GetFlags(file);
        }

        String GetTextAt(String element, String file)
        {

            XmlReader textReader;// = new XmlReader("..\\..\\..\\Content\\" + file);
            textReader = XmlReader.Create(PATH + file);
            String Value = "";

            while (textReader.Read())
            {
                XmlNodeType nType = textReader.NodeType;

                if (nType == XmlNodeType.Element)
                {
                    Value = textReader.Name.ToString();
                    if (Value == element)
                    {
                        Console.WriteLine("Element:" + textReader.Name.ToString());
                        textReader.Read();
                        nType = textReader.NodeType;
                        if (nType == XmlNodeType.Text)
                        {
                            Value = textReader.Value;
                            Console.WriteLine("Text:" + textReader.Value);
                            break;
                        }
                    }
                }


            }

            return Value;

        }

        void GetTextForFlags(String file, out List<string> elemType, out List<string> elemText)
        {
            
            XmlReader textReader;// = new XmlReader("..\\..\\..\\Content\\" + file);
            textReader = XmlReader.Create(PATH + file);
            String Value = "";
                        
            elemType = new List<string>();
            elemText = new List<string>();

            while (textReader.Read())
            {
                XmlNodeType nType = textReader.NodeType;

                if (nType == XmlNodeType.Element)
                {
                    Value = textReader.Name.ToString();
                    if (Value == "flag")
                    {
                        elemType.Add("flag");
                        textReader.Read();
                        Value = textReader.Value;
                        elemText.Add(Value);
                    }

                    Value = textReader.Name.ToString();
                    if (Value == "event")
                    {
                        elemType.Add("event");
                        textReader.Read();
                        Value = textReader.Value;
                        elemText.Add(Value);
                    }
                    Value = textReader.Name.ToString();
                    if (Value == "clas")
                    {
                        elemType.Add("clas");
                        textReader.Read();
                        Value = textReader.Value;
                        elemText.Add(Value);
                    }

                    Value = textReader.Name.ToString();
                    if (Value == "spawn_point")
                    {
                        elemType.Add("spawn_point");
                        textReader.Read();
                        Value = textReader.Value;
                        elemText.Add(Value);
                    }

                    Value = textReader.Name.ToString();
                    if (Value == "enemy_model")
                    {
                        elemType.Add("enemy_model");
                        textReader.Read();
                        Value = textReader.Value;
                        elemText.Add(Value);
                    }

                    Value = textReader.Name.ToString();
                    if (Value == "scale")
                    {
                        elemType.Add("scale");
                        textReader.Read();
                        Value = textReader.Value;
                        elemText.Add(Value);
                    }

                    Value = textReader.Name.ToString();
                    if (Value == "rotation")
                    {
                        elemType.Add("rotation");
                        textReader.Read();
                        Value = textReader.Value;
                        elemText.Add(Value);
                    }
                }
            }
        }

        List<EventFlag> GetFlags(String file)
        {
            List<EventFlag> FlagList = new List<EventFlag>();

            List<string> elemType = new List<string>();
            List<string> elemText = new List<string>();

            GetTextForFlags(file, out elemType, out elemText);

            EventFlag flag = null;
            GameEvent g_event = null;

            for (int i = 0; i < elemType.Count; ++i)
            {
                if ((string)elemType[i] == "flag")
                {
                    if (flag != null)
                        FlagList.Add(flag);
                    flag = new EventFlag();
                    flag.Corners = GetCorners((string)elemText[i]);
                }

                else if ((string)elemType[i] == "event")
                {
                    g_event = new GameEvent();

                    if ((string)elemText[i] == "PUT_ENEMY")
                    {
                        g_event.EventType = GameEvent.GameEventType.PUT_ENEMY;

                        EnemySpawn spawn = new EnemySpawn();

                        string type = (string)elemText[i + 1];
                        Console.WriteLine("clas: " + type);


                        spawn.Position = GetVector3At((string)elemText[i + 2]) * myLevel.terrainScale;
                        Console.WriteLine("Position: " + spawn.Position);

                        string scale = (string)elemText[i + 3];
                        Console.WriteLine("scale: " + scale);

                        spawn.Rotation = GetVector3At((string)elemText[i + 4]);

                        spawn.EnemyStats.Add("Type", type);
                        spawn.EnemyStats.Add("Scale", scale);

                        g_event.SpawnPoint = spawn;
                    }

                    flag.Events.Add(g_event);
                }

            }

            if (flag != null)
                FlagList.Add(flag);

            return FlagList;

        }

        Vector2[] GetCorners(string elemText)
        {
            Vector2[] corners = new Vector2[4];

            string[] tArray = new string[8];
            tArray = elemText.Split(new Char[] { ',', ';', ' ' });

            corners[0].X = Convert.ToSingle(tArray[0]);
            corners[0].Y = Convert.ToSingle(tArray[1]);

            corners[1].X = Convert.ToSingle(tArray[2]);
            corners[1].Y = Convert.ToSingle(tArray[3]);

            corners[2].X = Convert.ToSingle(tArray[4]);
            corners[2].Y = Convert.ToSingle(tArray[5]);

            corners[3].X = Convert.ToSingle(tArray[6]);
            corners[3].Y = Convert.ToSingle(tArray[7]);

            return corners;
        }

        Vector3 GetVector3At(String element, String file)
        {
            Vector3 Value = new Vector3();

            string text = GetTextAt(element, file);
            string[] tArray = new string[3];
            tArray = text.Split(new Char[] { ',', ' ' });

            Value.X = Convert.ToSingle(tArray[0]);
            Value.Y = Convert.ToSingle(tArray[1]);
            Value.Z = Convert.ToSingle(tArray[2]);
            //Value.Z = Convert.ToDouble(tArray[2]);

            return Value;

        }

        Vector3 GetVector3At(String text)
        {
            Vector3 Value = new Vector3();

            string[] tArray = new string[3];
            tArray = text.Split(new Char[] { ',', ' ' });

            Value.X = Convert.ToSingle(tArray[0]);
            Console.WriteLine("num: " + Value.X);
            Value.Y = Convert.ToSingle(tArray[1]);
            Console.WriteLine("num: " + Value.Y);
            Value.Z = Convert.ToSingle(tArray[2]);
            Console.WriteLine("num: " + Value.Z);
            //Value.Z = Convert.ToDouble(tArray[2]);

            return Value;

        }



    }
}
