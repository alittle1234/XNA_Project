#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Net;

using JigLibX.Physics;
using JigLibX.Collision;

using GameDebugTools;
#endregion

using System.Collections.Generic;

namespace TerrainGame4_0
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    public class GameplayScreen : GameScreen
    {
        public enum SessionState
        {
            StartUp,
            Loading,
            Loaded,
            Starting,
            Started,
            Edit,
            Complete
        }

        public SessionState sessionState = SessionState.StartUp;
        public int framecount = 0;

        #region     BACKGROUND THREAD
        public Thread           backgroundThread;
        EventWaitHandle         backgroundThreadExit;
        #endregion

        ParentGame parentGame;

        GameTime gameTime;

        public bool IsPaused = false;
        bool CamIsFollow = true;
        MouseState prevMouseState = Mouse.GetState();

        public List<Missile> BulletList;
        public List<BuildingPiece> PieceList;        

        public DrawingClass NormalDrawing;
        Matrix SpriteScale;

        List<ParticleEmitter> Particles;

        TerrainEditor TerrEditor;

        public float TimeRemaining;

        #region SOUND
        public AudioEmitter emitter = new AudioEmitter();
        public AudioListener listener = new AudioListener();
        public Cue cue, ambieCue, birdCue;
        Cue RotateCue, ElevateCue, PowerCue;
        public Cue FireworkCue;
        float ambientWindTimer = 21;
        float newBirdTimeSpan = 0;
        float ambientBirdTimer = 0;
        #endregion

        #region LEVEL VARIABLES
        public TerrainEngine terrainEngine = null;
        public TriangleMeshActor terrainActor = null;

        public Model playerModel = null;
        public Texture2D[] playerTextures = null;

        public Model rocketModel = null;
        public Texture2D[] rocketTextures = null;

        public PlayerTank Player = null;

        public MouseCamera MouseCam = null;
        public FollowCam FollowCam = null;
        public Vector3 CameraPosition;

        public List<Building> Buildings;

        Model BunkerModel;
        Texture2D[] BunkerTextures;
        Model BunkerDestroyed;
        Texture2D[] BunkerDestroyedTextures;

        Model BunkerPiece1;
        Texture2D[] BunkerPiece1Textures;
        Model BunkerPiece2;
        Texture2D[] BunkerPiece2Textures;
        Model BunkerPiece3;
        Texture2D[] BunkerPiece3Textures;
        Model BunkerPiece4;
        Texture2D[] BunkerPiece4Textures;

        Model DepotModel;
        Texture2D[] DepotTextures;
        Model DepotDestroyed;
        Texture2D[] DepotDestroyedTextures;

        Model DepotPiece1;
        Texture2D[] DepotPiece1Textures;
        Model DepotPiece2;
        Texture2D[] DepotPiece2Textures;
        Model DepotPiece3;
        Texture2D[] DepotPiece3Textures;
        Model DepotPiece4;
        Texture2D[] DepotPiece4Textures;

        public Texture2D RadarTexture;
        public Texture2D BlipTexture;

        Model RingModel;
        Texture2D RingTex1;
        Texture2D RingTex2;
        Texture2D[] RingTextures;

        public Model SunModel;

        public int MissileRemain = 5;
        public int MissileStart = 5;
        public int BuildingRemain = 0;
        float ReloadTimeSec = 1.0f;
        float shotTime = 0;
        int LevelNum;
        public Color FogColor;
        #endregion
        

        #region Fields

        NetworkSession networkSession;

        ContentManager content;
        SpriteFont gameFont;

        Vector2 playerPosition = new Vector2(100, 100);
        Vector2 enemyPosition = new Vector2(100, 100);

        public Random random = new Random();

        #endregion

        #region Properties


        /// <summary>
        /// The logic for deciding whether the game is paused depends on whether
        /// this is a networked or single player game. If we are in a network session,
        /// we should go on updating the game even when the user tabs away from us or
        /// brings up the pause menu, because even though the local player is not
        /// responding to input, other remote players may not be paused. In single
        /// player modes, however, we want everything to pause if the game loses focus.
        /// </summary>
        new bool IsActive
        {
            get
            {
                if (networkSession == null)
                {
                    // Pause behavior for single player games.
                    return base.IsActive;
                }
                else
                {
                    // Pause behavior for networked games.
                    return !IsExiting;
                }
            }
        }


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen(NetworkSession networkSession, ParentGame parentGame)
        {
            this.networkSession = networkSession;

            this.parentGame = parentGame;

            sessionState = SessionState.Loading;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public GameplayScreen(NetworkSession networkSession)
        {
            this.networkSession = networkSession;

            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            sessionState = SessionState.Loading;

            //gameFont = content.Load<SpriteFont>("gamefont");

            backgroundThread = new Thread(Loading);
            backgroundThreadExit = new ManualResetEvent(false);

            backgroundThread.Start();

            backgroundThread.Name = "Loading Game " + LevelNum;
        }

        /// <summary>
        /// Background thread if level is still loading.
        /// </summary>
        void Loading()
        {
            while (parentGame.Game_State != ParentGame.GameState.LevelLoaded)
               // while (parentGame.Level_Loader.CheckProgress() < 1.000f)
                    Thread.Sleep(100);

            gameFont = content.Load<SpriteFont>("Font\\gamefont");

            LevelPostLoad();

            sessionState = SessionState.Loaded;
            parentGame.Game_State = ParentGame.GameState.LevelStarted;

        }

        /// <summary>
        /// Assign variables the proper loaded objects.
        /// </summary>
        void LevelPostLoad()
        {
            parentGame.InitializePhysics();

            LoadTerrain();
            LoadObjects();
            LoadCamera();

            BulletList = new List<Missile>();
            Buildings = new List<Building>();
            PieceList = new List<BuildingPiece>();

            Particles = new List<ParticleEmitter>();

            NormalDrawing = new DrawingClass(parentGame, this,
                BulletList, Buildings, PieceList);

            float screenscale =
                (float)parentGame.device.Viewport.Width / 800f;
            SpriteScale = Matrix.CreateScale(screenscale, screenscale, 1);

            RotateCue = parentGame.soundBank.GetCue("rotate");
            ElevateCue = parentGame.soundBank.GetCue("elevate");
            PowerCue = parentGame.soundBank.GetCue("power");
            FireworkCue = parentGame.soundBank.GetCue("firework");

            TerrEditor = new TerrainEditor(parentGame, this, terrainEngine);

            parentGame.SlowTime = false;
        }

        void LoadTerrain()
        {
            ResourceList LR = parentGame.LoadedResources;
            Dictionary<string, string> LLL = parentGame.Level_Loader.GetLevelLoadList();

            TerrainEngine.TerrainArguments terrainArgument = new TerrainEngine.TerrainArguments();

            terrainArgument.heightMap =         LR.Tex2dList[LLL["Terrain"]];

            terrainArgument.terrainTexture1 = LR.Tex2dList[LLL["TerrainTexture1"]];
            terrainArgument.terrainTexture2 = LR.Tex2dList[LLL["TerrainTexture2"]];
            terrainArgument.terrainTexture3 = LR.Tex2dList[LLL["TerrainTexture3"]];
            terrainArgument.terrainTexture4 = LR.Tex2dList[LLL["TerrainTexture4"]];
            terrainArgument.decalTexture =      LR.Tex2dList[LLL["DecalTexture"]];
            terrainArgument.skyTexture =        LR.Tex2dList[LLL["SkyTexture"]];            
            terrainArgument.skyDome =           LR.ModelList[LLL["SkyDome"]].Model_rez;

            terrainArgument.terrainScale = parentGame.Level_Loader.myLevel.terrainScale;


            terrainEngine = new TerrainEngine(
                parentGame,
                MouseCam,
                terrainArgument);
            terrainEngine.LoadContent();




            terrainActor = new TriangleMeshActor(
                parentGame,
                new Vector3(terrainEngine.heightmapPosition.X, 0, terrainEngine.heightmapPosition.Z),
                terrainEngine.terrainScale,
                terrainEngine.heightMap,
                terrainEngine.heightData);
            terrainActor.Body.Immovable = true;
            //terrainActor.Skin.callbackFn += new CollisionCallbackFn(handleCollisionDetection);

            parentGame.Components.Add(terrainActor);

        }

        void LoadObjects()
        {
            ResourceList LR = parentGame.LoadedResources;
            Dictionary<string, string> LLL = parentGame.Level_Loader.GetLevelLoadList();

            playerModel =     LR.ModelList[LLL["PlayerTankModel"]].Model_rez;
            playerTextures =  LR.ModelList[LLL["PlayerTankModel"]].Textures_rez;
            rocketModel =     LR.ModelList[LLL["PlayerRocketModel"]].Model_rez;
            rocketTextures =  LR.ModelList[LLL["PlayerRocketModel"]].Textures_rez;
            //cornerModel = LoadedResources.ModelList["Model\\sphere"].Model_rez;


            Vector3 pos = parentGame.Level_Loader.myLevel.playerSpawn.Position;
            float h; Vector3 n;
            terrainEngine.GetHeight(pos, out h, out n);
            pos.Y = h;

            Player = new PlayerTank(
                parentGame,
                playerModel,
                playerTextures,
                pos,
                rocketModel,
                rocketTextures,
                this);
            Player.Load();
            Player.scale = 1.0f;
            Player.alive = true;
            Player.rotation = new Vector3(0, 0, 0);

            Player.baseBone.Transform = Matrix.Identity;

            Player.UpdateWorldMatrix();

            BunkerModel = LR.ModelList[LLL["BunkerModel"]].Model_rez;
            BunkerTextures = LR.ModelList[LLL["BunkerModel"]].Textures_rez;
            BunkerDestroyed = LR.ModelList[LLL["BunkerDestroyed"]].Model_rez;
            BunkerDestroyedTextures = LR.ModelList[LLL["BunkerDestroyed"]].Textures_rez;

            BunkerPiece1 = LR.ModelList[LLL["BunkerPiece1"]].Model_rez;
            BunkerPiece1Textures = LR.ModelList[LLL["BunkerPiece1"]].Textures_rez;
            BunkerPiece2 = LR.ModelList[LLL["BunkerPiece2"]].Model_rez;
            BunkerPiece2Textures = LR.ModelList[LLL["BunkerPiece2"]].Textures_rez;
            BunkerPiece3 = LR.ModelList[LLL["BunkerPiece3"]].Model_rez;
            BunkerPiece3Textures = LR.ModelList[LLL["BunkerPiece3"]].Textures_rez;
            BunkerPiece4 = LR.ModelList[LLL["BunkerPiece4"]].Model_rez;
            BunkerPiece4Textures = LR.ModelList[LLL["BunkerPiece4"]].Textures_rez;


            DepotModel = LR.ModelList[LLL["DepotModel"]].Model_rez;
            DepotTextures = LR.ModelList[LLL["DepotModel"]].Textures_rez;
            DepotDestroyed = LR.ModelList[LLL["DepotDestroyed"]].Model_rez;
            DepotDestroyedTextures = LR.ModelList[LLL["DepotDestroyed"]].Textures_rez;

            DepotPiece1 = LR.ModelList[LLL["DepotPiece1"]].Model_rez;
            DepotPiece1Textures = LR.ModelList[LLL["DepotPiece1"]].Textures_rez;
            DepotPiece2 = LR.ModelList[LLL["DepotPiece2"]].Model_rez;
            DepotPiece2Textures = LR.ModelList[LLL["DepotPiece2"]].Textures_rez;
            DepotPiece3 = LR.ModelList[LLL["DepotPiece3"]].Model_rez;
            DepotPiece3Textures = LR.ModelList[LLL["DepotPiece3"]].Textures_rez;
            DepotPiece4 = LR.ModelList[LLL["DepotPiece4"]].Model_rez;
            DepotPiece4Textures = LR.ModelList[LLL["DepotPiece4"]].Textures_rez;

            RingModel = LR.ModelList[LLL["Ring"]].Model_rez;
            //RingTex1 = LR.Tex2dList[LLL["RingTexture1"]];
            //RingTex2 = LR.Tex2dList[LLL["RingTexture2"]];

            RingTex1 = LR.ModelList[LLL["Ring"]].Textures_rez[0];
            RingTex2 = LR.ModelList[LLL["Ring"]].Textures_rez[0];
            RingTextures = new Texture2D[2];
            RingTextures[0] = RingTex1;
            RingTextures[1] = RingTex2;

            TimeRemaining = parentGame.Level_Loader.myLevel.Time;

            RadarTexture = LR.Tex2dList[LLL["RadarTexture"]];
            BlipTexture = LR.Tex2dList[LLL["BlipTexture"]];
            

            MissileStart = parentGame.Level_Loader.myLevel.Missiles;
            MissileRemain = MissileStart;

            LevelNum = parentGame.LevelNum;

            FogColor = parentGame.Level_Loader.myLevel.FogColor;
            //LevelVars.Player.position = parentGame.Level_Loader.myLevel.playerSpawn.Position;
        }

        void LoadCamera()
        {
            MouseCam = new MouseCamera(parentGame);

            MouseCam.LoadContent();

            MouseCam.cameraPosition = parentGame.Level_Loader.myLevel.cameraSpawn.Position;
            MouseCam.leftrightRot = MathHelper.PiOver2;

            FollowCam = new FollowCam(parentGame, Player);

        }

       
        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            List<Body> Bodies = new List<Body>();
            foreach (Body body in PhysicsSystem.CurrentPhysicsSystem.Bodies)
                Bodies.Add(body);

            foreach (Body body in Bodies)
                PhysicsSystem.CurrentPhysicsSystem.RemoveBody(body);

            List<CollisionSkin> Skins = new List<CollisionSkin>();
            foreach (CollisionSkin skin in PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.CollisionSkins)
                Skins.Add(skin);

            foreach (CollisionSkin skin in Skins)
                PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.RemoveCollisionSkin(skin);
            
            
            parentGame.Components.Remove(terrainActor);
            terrainActor.Dispose();

            NormalDrawing.Unload();
            terrainEngine.Unload();

            Player.baseRotationValue = 0;
            Player.cannonRotationValue = 0;
            Player.turretRotationValue = 0;

            Player.baseBone.Transform = Matrix.Identity;
            Player.Unload();

            Player = null;
            MouseCam = null;
            terrainEngine = null;

            //LevelVars = null;
            NormalDrawing = null;

            //ambieCue = parentGame.soundBank.GetCue("ambient");
            //ambieCue.Stop(AudioStopOptions.Immediate);

            
            if (ambieCue != null && ambieCue.IsPlaying)
                ambieCue.Stop(AudioStopOptions.Immediate);

            if (birdCue.IsCreated && birdCue.IsPlaying)
                birdCue.Stop(AudioStopOptions.Immediate);

            if (cue != null && cue.IsPlaying)
                cue.Stop(AudioStopOptions.Immediate);

            SetScore();

#if XBOX
            parentGame.SaveGame();
#endif

            
            parentGame = null;

            content.Unload();
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            //DebugSystem.Instance.TimeRuler.BeginMark("UPDATE", Color.Red);

            this.gameTime = gameTime;

            base.Update(gameTime, otherScreenHasFocus, false);

            if (sessionState == SessionState.Loading)
            {
                
            }

            if (sessionState == SessionState.Loaded)
            {
                sessionState = SessionState.Starting;
            }

            if (sessionState == SessionState.Starting)
            {
                if (CamIsFollow)
                    SetCamera_Follow();
                else
                    SetCamera();                
                sessionState = SessionState.Started;
                SpawnBuildings();
                
            }



            if (IsActive)
            {
                parentGame.Paused = IsPaused = false;

                if (sessionState == SessionState.Started
                    || 
                    sessionState == SessionState.Complete)
                {
                    if (CamIsFollow)
                    {
                        FollowCam.Update(gameTime);
                        CameraPosition = FollowCam.cameraPosition;
                    }
                    else
                    {
                        MouseCam.Update(gameTime);
                        CameraPosition = MouseCam.cameraPosition;                        
                    }

                    NormalDrawing.Update();
                    Player.Update(gameTime);

                    foreach (Missile missile in BulletList)
                        missile.Update(gameTime);

                    foreach (Building building in Buildings)
                        building.Update(gameTime);

                    foreach (BuildingPiece piece in PieceList)
                        piece.Update(gameTime);

                    for (int i = 0; i < BulletList.Count; ++i)
                    {
                        if (BulletList[i].isCleanedUp)
                        {
                            if (BulletList[i] == FollowCam.myMissile)
                            {
                                FollowCam.Following = false;
                                FollowCam.myMissile = null;
                            }

                            BulletList.Remove(BulletList[i]);
                        }

                    }

                    for (int i = 0; i < PieceList.Count; ++i)
                    {
                        if (PieceList[i].isCleanedUp)
                        {
                            //BulletList.Remove(BulletList[i]);
                        }

                        if (!PieceList[i].Body.IsActive)
                        {
                            //BulletList[i].Destroy();
                        }

                    }

                    for (int i = 0; i < NormalDrawing.Particles.Count; ++i)
                        NormalDrawing.Particles[i].Update(parentGame.gameTime);

                    listener.Position = CameraPosition;

                    PlayAmbient(gameTime);

                    if (sessionState == SessionState.Started)
                    {
                        if (BuildingRemain == 0 ||
                            MissileRemain == 0)
                        {
                            if (BulletList.Count <= 0)
                                EndLevel();
                        }

                        if (!parentGame.NormalMode)
                        {

                            if (!IsPaused && TimeRemaining > 0)
                                TimeRemaining -= gameTime.ElapsedGameTime.Milliseconds;

                            if (TimeRemaining <= 0)
                            {
                                TimeRemaining = 0;
                                if (BulletList.Count <= 0)
                                    EndLevel();
                            }
                        }
                    }

                    parentGame.IsMouseVisible = false;
                    
                }

                if (sessionState == SessionState.Edit)
                {                    
                    parentGame.IsMouseVisible = true;
                    
                }
            }
            else
            {                
                    parentGame.Paused = IsPaused = true;
            }


            // If we are in a network game, check if we should return to the lobby.
            if ((networkSession != null) && !IsExiting)
            {
                if (networkSession.SessionState == NetworkSessionState.Lobby)
                {
                    LoadingScreen.Load(ScreenManager, true, null,
                                       new BackgroundScreen(),
                                       new LobbyScreen(networkSession));
                }
            }

            if (networkSession == null && IsExiting)
                ScreenManager.RemoveScreen(this);


            //DebugSystem.Instance.TimeRuler.EndMark("UPDATE");
        }

        private void PlayAmbient(GameTime gameTime)
        {
            
            float timeDifference = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;

            ambientWindTimer += timeDifference;

            if (ambientWindTimer > 20)
            {
                ambientWindTimer = 0;
                ambieCue = parentGame.soundBank.GetCue("ambient");
                ambieCue.Play();
            }

            ambientBirdTimer += timeDifference;
            
            if (ambientBirdTimer > newBirdTimeSpan)
            {
                ambientBirdTimer = 0;
                birdCue = parentGame.soundBank.GetCue("bird");
                birdCue.Play();

                 //float amount = (float)(min + (float)random.NextDouble() * (max - min));
                newBirdTimeSpan = (float)(5 + (float)random.NextDouble() * (20 - 5));
            }
        }

        private void SpawnBuildings()
        {
           
            List<EventFlag> FlagList = parentGame.Level_Loader.myLevel.FlagsList;
            foreach (EventFlag flag in FlagList)
                foreach (GameEvent evnt in flag.Events)
                {
                    double ROTATION = (3 + random.NextDouble() * (84 - 3));

                    EnemySpawn spwn = (evnt.SpawnPoint as EnemySpawn);
                    if (spwn.EnemyStats["Type"] == "BUNKER")
                    {
                        float HEIGHT; 
                        Vector3 NORMAL;
                        

                        terrainEngine.GetHeight(spwn.Position, out HEIGHT, out NORMAL);

                        Building building = new Building(
                            parentGame,
                            BunkerModel,
                            BunkerTextures,
                            new Vector3(spwn.Position.X, HEIGHT + 4, spwn.Position.Z),
                            (float)Convert.ToDouble((spwn.EnemyStats["Scale"])),
                            new Vector3(
                                spwn.Rotation.X + MathHelper.ToRadians((float)ROTATION),
                                spwn.Rotation.Y,
                                spwn.Rotation.Z),
                            "BUNKER",
                            this);

                        //+ MathHelper.ToRadians(-90)

                        building.Body.Immovable = true;

                        Buildings.Add(building);
                        ++BuildingRemain;
                    }

                    else if (spwn.EnemyStats["Type"] == "DEPOT")
                    {
                        float HEIGHT;
                        Vector3 NORMAL;
                        terrainEngine.GetHeight(spwn.Position, out HEIGHT, out NORMAL);
                        Building building = new Building(
                            parentGame,
                            DepotModel,
                            DepotTextures,
                            new Vector3(spwn.Position.X, HEIGHT + 1, spwn.Position.Z),
                            (float)Convert.ToDouble((spwn.EnemyStats["Scale"])),
                            new Vector3(
                                spwn.Rotation.X + MathHelper.ToRadians((float)ROTATION),
                                spwn.Rotation.Y,
                                spwn.Rotation.Z),
                            "DEPOT",
                            this);

                        building.Body.Immovable = true;

                        Buildings.Add(building);
                        ++BuildingRemain;
                    }

                    else if (spwn.EnemyStats["Type"] == "RING")
                    {
                        Ring ring = new Ring(
                            parentGame,
                            RingModel,
                            RingTextures,
                            new Vector3(spwn.Position.X, spwn.Position.Y, spwn.Position.Z),
                            (float)Convert.ToDouble((spwn.EnemyStats["Scale"])),
                            new Vector3(spwn.Rotation.X, spwn.Rotation.Y, spwn.Rotation.Z),
                            "RING",
                            this);

                        ring.Body.Immovable = true;
                        
                        Buildings.Add(ring);
                        ++BuildingRemain;
                    }
                    
                }

            
        }

        
        private void SetCamera()
        {
            MouseCam.cameraPosition = Player.position;
            MouseCam.cameraPosition.Y += 10;
            MouseCam.leftrightRot = Player.FacingDirection;
            Vector3 forwardVec = Matrix.CreateFromYawPitchRoll(Player.FacingDirection, 0f, 0f).Forward;
            forwardVec.Normalize();
            MouseCam.cameraPosition -= forwardVec * 40;
        }

        private void SetCamera_Follow()
        {
            FollowCam.cameraPosition = Player.position;
            FollowCam.cameraPosition.Y += 10;
            FollowCam.leftrightRot = Player.turretRotationValue - MathHelper.ToRadians(90);
            Vector3 forwardVec = Matrix.CreateFromYawPitchRoll(FollowCam.leftrightRot, 0f, 0f).Backward;
            forwardVec.Normalize();
            FollowCam.cameraPosition += forwardVec * 35;
        }

        void CreatePiece(Building Building, float Xoffset, float Yoffset, float Zoffset, Model PieceModel, Texture2D[] PieceTextures,
            Vector3 ExplosionPosition, float Force, float Radius, float L, float W, float H)
        {
            ExplosionPosition = new Vector3(ExplosionPosition.X, ExplosionPosition.Y - 15, ExplosionPosition.Z);

            BuildingPiece Piece = new BuildingPiece(parentGame, PieceModel, PieceTextures, NormalDrawing, this, L, W, H);
            Piece.position = new Vector3(
                Building.position.X + Xoffset,
                Building.position.Y + Yoffset,
                Building.position.Z + Zoffset);

            Piece.scale = Building.scale;
                        
            Vector3 ROTATION = Building.rotation;
            Matrix Orientation = Matrix.CreateFromYawPitchRoll(ROTATION.X, ROTATION.Y, ROTATION.Z);

            Piece.SetBody(Orientation);

            Vector3 DIRECTION = Piece.position - ExplosionPosition;
            DIRECTION.Normalize();
            Vector3 ANGLE = DIRECTION;

            float min = 1.1f;
            float max = 1.6f;
            float AMOUNT = (float)(min + (float)random.NextDouble() * (max - min));
            //float amount = (float)(min + (float)random.NextDouble() * (max - min));

            float DIS = Vector3.Distance(Piece.position, ExplosionPosition);
            float POWER = Force * (1 - DIS / Radius);

            Piece.Body.ApplyWorldImpulse(DIRECTION * POWER);
            Piece.Body.AngularVelocity = (ANGLE * AMOUNT);

            Piece.Skin.callbackFn += new CollisionCallbackFn(PieceColllisionFunction);

            //Piece.Body.Immovable = true;

            PieceList.Add(Piece);
        }

        void FireBullet()
        {
            float timeNow = (float)gameTime.TotalGameTime.TotalMilliseconds / 1000.0f;
            float timeDiff = timeNow - shotTime;
           

            if ((MissileRemain > 0) && (timeDiff > ReloadTimeSec))
            {
                shotTime = timeNow;
                //MissileRemain--;

                Missile missile = new Missile(parentGame, rocketModel, rocketTextures, NormalDrawing, this);
                missile.position = new Vector3(
                    Player.position.X,
                    Player.position.Y,
                    Player.position.Z);
                Vector3 forwardVec = Matrix.CreateFromYawPitchRoll(
                    Player.turretRotationValue - MathHelper.ToRadians(90f),
                    0f,
                    0f).Forward;
                forwardVec.Normalize();

                missile.position += forwardVec * 11;

                missile.scale = 1;

                float DECLINATION = -Player.cannonRotationValue; // stays in radians

                // sin in radians                       // width                    // height
                missile.position.Y = (float)(Math.Sin(DECLINATION) * 13.3) + Player.position.Y + 4;

                float ROTATION = Player.turretRotationValue - MathHelper.ToRadians(90f);
                Matrix Orientation = Matrix.CreateFromYawPitchRoll(ROTATION, DECLINATION, 0f);
                //LevelVars.Player.FacingDirection
                //DECLINATION
                missile.SetBody(Orientation);

                Vector3 force = Orientation.Forward;
                force.Normalize();
                Vector3 angle = Orientation.Left;
                angle.Normalize();

                float power = 120f * Player.Power;
                missile.Body.ApplyWorldImpulse(force * power);
                missile.Body.AngularVelocity = (angle * MathHelper.Clamp((0.8f - Player.Power), 0.20f, 0.6f));

                missile.rgob.limbs[0].PhysicsBody.ApplyWorldImpulse(force * power * 5);
                missile.rgob.limbs[0].PhysicsBody.AngularVelocity = (angle * MathHelper.Clamp((0.8f - Player.Power), 0.20f, 0.6f) * 1);
                                
                //missile.Body.AngularVelocity += (force * 1.5f);                

                missile.Skin.callbackFn += new CollisionCallbackFn(MissileColllisionFunction);

                BulletList.Add(missile);

                FollowCam.Following = true;
                FollowCam.myMissile = missile;
                FollowCam.NewUpDownRot = missile.rotation.Y;
                FollowCam.NewLeftRightRot = (Player.turretRotationValue - MathHelper.ToRadians(90));

                cue = parentGame.soundBank.GetCue("fire");
                AudioEmitter emmit = new AudioEmitter();
                emmit.Position = Player.position;
                cue.Apply3D(listener, emmit);
                cue.Play();

                Vector3 effectPos = (missile.position - forwardVec * 2);
                for (int i = 0; i < 25; ++i)
                    NormalDrawing.shotSmokeParticles.AddParticle(effectPos, new Vector3(0, 0, 0));

                for (int i = 0; i < 25; ++i)
                    NormalDrawing.shotExplosionParticles.AddParticle(effectPos, new Vector3(0, 0, 0));
            }
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (ControllingPlayer.HasValue)
            {
                // In single player games, handle input for the controlling player.
                HandlePlayerInput(input, ControllingPlayer.Value);
            }
            else if (networkSession != null)
            {
                // In network game modes, handle input for all the
                // local players who are participating in the session.
                foreach (LocalNetworkGamer gamer in networkSession.LocalGamers)
                {
                    if (!HandlePlayerInput(input, gamer.SignedInGamer.PlayerIndex))
                        break;
                }
            }
        }


        /// <summary>
        /// Handles input for the specified player. In local game modes, this is called
        /// just once for the controlling player. In network modes, it can be called
        /// more than once if there are multiple profiles playing on the local machine.
        /// Returns true if we should continue to handle input for subsequent players,
        /// or false if this player has paused the game.
        /// </summary>
        bool HandlePlayerInput(InputState input, PlayerIndex playerIndex)
        {
            // Look up inputs for the specified player profile.
            KeyboardState keyboardState = input.CurrentKeyboardStates[(int)playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[(int)playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[(int)playerIndex];

            if (input.IsPauseGame(playerIndex) || gamePadDisconnected)
            {
                if (sessionState == SessionState.Started)
                {
                    ScreenManager.AddScreen(new PauseMenuScreen(networkSession), playerIndex);                    

                    ScreenManager.menu_change.Play();

                    return false;
                }

                if (sessionState == SessionState.Complete)
                {
                    Exit();                    
                }
            }

            if (sessionState == SessionState.Started
                || sessionState == SessionState.Edit)
            {
                float timeDifference = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;

                Player.HandleInput(timeDifference);

                PlayerIndex pI = PlayerIndex.Four;
                if (input.IsNewKeyPress(Keys.Space, playerIndex, out pI)
                    ||
                    input.IsNewButtonPress(Buttons.RightTrigger, playerIndex, out pI))
                {
                    if (!parentGame.NormalMode)
                    {
                        if (TimeRemaining > 0)
                            FireBullet();
                    }
                    else
                        FireBullet();
                }

                float thumbRX = gamePadState.ThumbSticks.Right.X;
                if (thumbRX != 0)
                {
                    Player.turretRotationValue -= 0.05f * thumbRX;
                    float value = MathHelper.ToDegrees(Player.turretRotationValue);
                    value = (float)Math.Round(value, 0);
                    if (value % 10 == 0)
                    {
                        if (!RotateCue.IsPlaying)
                        {
                            RotateCue = parentGame.soundBank.GetCue("rotate");
                            AudioEmitter emmit = new AudioEmitter();
                            emmit.Position = Player.position;
                            RotateCue.Apply3D(listener, emmit);
                            RotateCue.Play();
                        }
                    }
                }

                float thumbRY = gamePadState.ThumbSticks.Right.Y;
                if (thumbRY != 0)
                {
                    Player.cannonRotationValue -= 0.05f * thumbRY;
                    Player.cannonRotationValue = MathHelper.Clamp(Player.cannonRotationValue, MathHelper.ToRadians(-45), MathHelper.ToRadians(10));

                    float value = MathHelper.ToDegrees(Player.cannonRotationValue);
                    value = (float)Math.Round(value, 0);
                    if (value % 2 == 0
                        && value != -45
                        && value != 10)
                    {
                        if (!ElevateCue.IsPlaying)
                        {
                            ElevateCue = parentGame.soundBank.GetCue("elevate");
                            AudioEmitter emmit = new AudioEmitter();
                            emmit.Position = Player.position;
                            ElevateCue.Apply3D(listener, emmit);
                            ElevateCue.Play();
                            
                        }
                    }
                }

                float thumbLY = gamePadState.ThumbSticks.Left.Y;
                if (thumbLY != 0)
                {
                    Player.Power += 0.01f * thumbLY;

                    if (Player.Power > 1)
                        Player.Power = 1f;
                    else if (Player.Power < 0.1f)
                        Player.Power = 0.1f;

                    //power = 0 - 1
                    float value = Player.Power;
                    value *= 100;
                    //value = (float)Math.Round(value, 2);                   
                    value = (float)Math.Floor(value);
                    if (value % 5 == 0 &&
                        value != 100 && value != 10)
                    {
                        if (!PowerCue.IsPlaying)
                        {
                            PowerCue = parentGame.soundBank.GetCue("power");
                            PowerCue.SetVariable("PowerLevel", Player.Power);
                            AudioEmitter emmit = new AudioEmitter();
                            emmit.Position = Player.position;
                            PowerCue.Apply3D(listener, emmit);
                            PowerCue.Play();
                        }
                    }
                }

                MouseState mouseState = Mouse.GetState();
                if (input.IsNewKeyPress(Keys.M, playerIndex, out pI))
                    CamIsFollow = (CamIsFollow) ? false : true;

                if (input.IsNewKeyPress(Keys.N, playerIndex, out pI))
                    sessionState = (sessionState == SessionState.Edit) ? SessionState.Started : SessionState.Edit;

                if (input.IsNewKeyPress(Keys.P, playerIndex, out pI))
                    NormalDrawing.BLEND += 1;

                if (sessionState == SessionState.Edit)
                {
                    TerrEditor.HandleInput();
                }

                prevMouseState = mouseState;
            }

            if (sessionState == SessionState.Complete)
            {
                if (input.IsMenuSelect(playerIndex, out playerIndex))
                {
                    Exit();
                }
            }


            #region OLD PLAYER
            // Otherwise move the player position.
            Vector2 movement = Vector2.Zero;

            if (keyboardState.IsKeyDown(Keys.Left))
                movement.X--;

            if (keyboardState.IsKeyDown(Keys.Right))
                movement.X++;

            if (keyboardState.IsKeyDown(Keys.Up))
                movement.Y--;

            if (keyboardState.IsKeyDown(Keys.Down))
                movement.Y++;            

            Vector2 thumbstick = gamePadState.ThumbSticks.Left;

            movement.X += thumbstick.X;
            movement.Y -= thumbstick.Y;

            if (movement.Length() > 1)
                movement.Normalize();

            playerPosition += movement * 2;
            #endregion
            

            return true;
        }

       
        public override void Draw(GameTime gameTime)
        {
            
            framecount += 1;
            if (framecount > 60)
                framecount = 0;

            #region NORMAL
            if (sessionState != SessionState.Loading)
            {
                
                ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                             Color.CornflowerBlue, 0, 0);

                NormalDrawing.UpdateFrameRate(gameTime);

                NormalDrawing.Draw();
                
                

                if (sessionState == SessionState.Edit)
                {
                    TerrEditor.DrawCursor();
                }


            }
            #endregion


            #region LOADING
            if (sessionState == SessionState.Loading)
            {
                if (framecount > 2 )
                {                    
                    framecount = 0;
                    parentGame.LoadingAnimFrame++;
                }
                //if (parentGame.Game_State != ParentGame.GameState.LoadingLevel)
                    parentGame.DrawMenuLoading();
            }
            #endregion

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0)
                //ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);
                ScreenManager.FadeBackBufferToBlack(((1 - TransitionPosition)) * 255);

           
        }


        #endregion

        public bool PieceColllisionFunction(CollisionSkin skin0, CollisionSkin skin1)
        {

            // here is handled what happens if your Object collides with another special Object (= OtherObject)
            if ((skin1.ExternalData) != null && (skin1.ExternalData) is TriangleMeshActor)
            {
                // since this instance is a 'bad guy' he deactivates 'Good Guys' when it collides with them
                if ((skin0.Owner.ExternalData) is BuildingPiece)
                {
                    //((BuildingPiece)skin0.Owner.ExternalData).Destroy();

                    BuildingPiece piece = (skin0.Owner.ExternalData) as BuildingPiece;

                    if (!piece.ContactGround)
                    {
                        cue = parentGame.soundBank.GetCue("piece_bounce");
                        AudioEmitter emmit = new AudioEmitter();
                        emmit.Position = piece.position;
                        cue.Apply3D(listener, emmit);
                        cue.Play();
                    }
                    piece.ContactGround = true;
                }

                return true;
            }
            //else
            //{
            //    if ((skin0.Owner.ExternalData) is BuildingPiece)
            //    {
            //        BuildingPiece piece = (skin0.Owner.ExternalData) as BuildingPiece;
            //        piece.ContactGround = false;
            //    }
            //    return true;
            //}
            
            
            return true;
        }

        public bool MissileColllisionFunction(CollisionSkin skin0, CollisionSkin skin1)
        {

            // here is handled what happens if your Object collides with another special Object (= OtherObject)
            if ((skin1.ExternalData) is TriangleMeshActor)
            {
                // since this instance is a 'bad guy' he deactivates 'Good Guys' when it collides with them
                if ((skin0.Owner.ExternalData) is Missile)
                {
                    ((Missile)skin0.Owner.ExternalData).Destroy();
                }
                
                return true;
            }

            if ((skin1.Owner.ExternalData) is Building)
            {
                // since this instance is a 'bad guy' he deactivates 'Good Guys' when it collides with them
                if (  !((skin1.Owner.ExternalData) is Ring)  )
                {
                    Building BUILD = ((Building)skin1.Owner.ExternalData);
                    BUILD.Destroy();
                    BuildingRemain--;

                    if (BUILD.Type == "BUNKER")
                    {
                        BUILD.model = BunkerDestroyed;
                        BUILD.textures = BunkerDestroyedTextures;
                    }
                    else
                    {
                        BUILD.model = DepotDestroyed;
                        BUILD.textures = DepotDestroyedTextures;
                    }


                    if ((skin0.Owner.ExternalData) is Missile)
                    {
                        Missile MISS = ((Missile)skin0.Owner.ExternalData);
                        MISS.Destroy();

                        float Force = 100f;
                        float Radius = 50f;

                        if (BUILD.Type == "BUNKER")                        
                            CreateBunkerPieces(BUILD, MISS, Force, Radius);                        
                        else
                            CreateDepotPieces(BUILD, MISS, Force, Radius);

                    }

                    return true;
                }
            }

            if ((skin1.Owner.ExternalData) is Ring)
            {
                if (!((Ring)skin1.Owner.ExternalData).isDestroyed)
                {
                    ((Ring)skin1.Owner.ExternalData).Destroy();
                    BuildingRemain--;
                }

                return false;
            }

            //if ((skin1.Owner.ExternalData) is JigLibX.Objects.PhysicObject)
            //{

            //    return false;
            //}
            
            return true;
        }

        void CreateBunkerPieces(Building BUILD, Missile MISS, float Force, float Radius)
        {

            // x y z
            CreatePiece(BUILD, 3, 13, -3, BunkerPiece1, BunkerPiece1Textures,
                MISS.position, Force, Radius,
                4, 4, 4);

            CreatePiece(BUILD, -3, 3, -3, BunkerPiece2, BunkerPiece2Textures,
                MISS.position, Force, Radius,
                4, 4, 4);

            CreatePiece(BUILD, -3, 9, 3, BunkerPiece3, BunkerPiece3Textures,
                MISS.position, Force, Radius,
                4, 4, 7);


            CreatePiece(BUILD, -2, 7, 1, BunkerPiece4, BunkerPiece4Textures,
                MISS.position, Force, Radius,
                2, 2, 2);

            CreatePiece(BUILD, 2, 14, 1, BunkerPiece4, BunkerPiece4Textures,
                MISS.position, Force, Radius,
                2, 2, 2);

            CreatePiece(BUILD, 2, 5, 2, BunkerPiece4, BunkerPiece4Textures,
                MISS.position, Force, Radius,
                2, 2, 2);


            CreatePiece(BUILD, -5, 15, -2, BunkerPiece4, BunkerPiece4Textures,
                MISS.position, Force, Radius,
               2, 2, 2);

            CreatePiece(BUILD, -3, 9, 2, BunkerPiece4, BunkerPiece4Textures,
                MISS.position, Force, Radius,
                2, 2, 2);
        }

        void CreateDepotPieces(Building BUILD, Missile MISS, float Force, float Radius)
        {
            // x y z
            CreatePiece(BUILD, -5, 3, 7, DepotPiece1, DepotPiece1Textures,
                MISS.position, Force, Radius,
                4, 4, 1);

            CreatePiece(BUILD, 2, 2, -3, DepotPiece2, DepotPiece2Textures,
                MISS.position, Force, Radius,
                3, 3, 1);


            CreatePiece(BUILD, -3, 5, -7, DepotPiece3, DepotPiece3Textures,
                MISS.position, Force, Radius,
                1, 1, 1);

            CreatePiece(BUILD, -2, 5, -5, DepotPiece4, DepotPiece4Textures,
                MISS.position, Force, Radius,
                2, 2, 2);

            CreatePiece(BUILD, 3, 5, -7, DepotPiece3, DepotPiece3Textures,
                MISS.position, Force, Radius,
                1, 1, 1);

            CreatePiece(BUILD, 2, 5, -5, DepotPiece4, DepotPiece4Textures,
                MISS.position, Force, Radius,
                2, 2, 2);


            CreatePiece(BUILD, -10, 5, 2, DepotPiece3, DepotPiece3Textures,
                MISS.position, Force, Radius,
                1, 1, 1);

            CreatePiece(BUILD, 10, 5, 3, DepotPiece4, DepotPiece4Textures,
                MISS.position, Force, Radius,
                2, 2, 2);
        }

        public int GetScore()
        {
            int score = 0;
            if (BuildingRemain == 0)
                score = (MissileRemain * 200) + ((Buildings.Count) * 100);
            else
                score = ((Buildings.Count - BuildingRemain) * 100);
            return score;
        }

        void SetScore()
        {
            int lev = LevelNum - 1;
            int kiled = Buildings.Count - BuildingRemain;
            if (parentGame.LevelStat[lev].hiTarKil <= kiled)
            {
                parentGame.LevelStat[lev].hiTarKil = kiled;
                if (parentGame.LevelStat[lev].hiMisRem < MissileRemain)
                    parentGame.LevelStat[lev].hiMisRem = MissileRemain;
            }

            int score = GetScore();
            if (parentGame.LevelStat[lev].hiscore < score)
                parentGame.LevelStat[lev].hiscore = score;

            if (BuildingRemain == 0)
            {
                parentGame.LevelStat[lev].passed = true;
                FollowCam.Won = true;
                if (lev < parentGame.LevelStat.Length - 1)
                    parentGame.LevelStat[lev + 1].locked = false;
            }
        }

        /// <summary>
        /// After level complete.  Not on quit.
        /// </summary>
        void Exit()
        {
            ScreenManager.menu_change.Play();

            parentGame.LevelNum = 0;

            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(parentGame.NormalMode),
                                                           new MainMenuScreen(ScreenManager.Game as ParentGame));
        }

        void EndLevel()
        {
            sessionState = SessionState.Complete;
            SetScore();
        }
    }
}
