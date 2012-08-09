using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;

using System.Threading;

using JigLibX.Physics;
using JigLibX.Geometry;
using JigLibX.Collision;

//using GameDebugTools;

namespace TerrainGame4_0
{
    public class ParentGame : Microsoft.Xna.Framework.Game
    {

        public class LevelStats
        {
            public bool locked = false;
            public int hiscore = 0;
            public int hiMisRem = 0;
            public int hiTarKil = 0;
            public bool passed = false;
            public float bestTime = 0.0f;
        }

        public string GameTitle = "Artillery Fantastic";

        public enum GameState
        {
            LoadingMenu,
            MenuLoaded,
            LoadingLevel,
            LevelLoaded,
            OnLevelScreen,
            LevelStarted
        }

        public GameState Game_State = GameState.LoadingMenu;

        public string modes = null;

        public bool NormalMode = true;

        public GameTime gameTime;

        public MenuResources           MenuVariables;

        public bool SlowTime = false;

        #region AVATAR
        // The AvatarDescription and AvatarRenderer that are used to render the avatar
        public AvatarRenderer avatarRenderer;
        public AvatarDescription avatarDescription;
        #endregion

        #region     LOADING
        public LoadingStage     Loading_Stage;
        int                     Load_FrameCount;            //During load, count frames, to draw only after max num        
        public int                     LoadingAnimFrame = 0;       //Use to animate Loading Animation
        Texture2D[]             LoadingAnimation;
        public MenuLoadList     Menu_Load_List;
        public ResourceList     LoadedResources;
        public ResourceList     MenuObjects;
        public int LevelNum = 0;
        int PrevLevelNum;
        #endregion

        #region     INPUT
        KeyboardState           keyboardState;
        MouseState              mouseState;
        #endregion

        #region     GRAPHICS
        GraphicsDeviceManager   graphics;
        public SpriteBatch      spriteBatch;
        public GraphicsDevice   device;
        RenderTarget2D          renderTarget1;
        const int windowWidth = 848;        //848   //1272
        const int windowHeight = 480;       //480   //720
        public Effect           objEffect;
       // BloomComponent          bloom;
        //public int              bloomSettingsIndex = 5;
        #endregion 

        #region     MATRICES
        public Matrix           worldMatrix;
        public Matrix           projectionMatrix;
        public Matrix           viewMatrix;
        #endregion 

        #region     SCREENS
        ScreenManager           screenManager;
        #endregion

        #region     BACKGROUND THREAD
        bool                    ScreensStarted = false;
        bool                    ScreensLoaded = false;
        public Thread           backgroundThread;
        EventWaitHandle         backgroundThreadExit;
        #endregion

        #region     LEVEL
        public LevelLoader      Level_Loader;
        public bool             Paused;
        public LevelStats[]     LevelStat;
        public int[]            MaxScores;
        #endregion

        #region     AUDIO
        AudioEngine audioEngine;
        public SoundBank soundBank;
        WaveBank waveBank;
        #endregion
        

        public ParentGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

#if XBOX360
            this.Components.Add(new GamerServicesComponent(this));
#endif
        }

        protected override void Initialize()
        { 
            //graphics.MinimumVertexShaderProfile = ShaderProfile.VS_2_0;
            //graphics.MinimumPixelShaderProfile = ShaderProfile.PS_2_0;

            //DebugSystem.Initialize(this, "Font\\gamefont");
            //DebugSystem.Instance.FpsCounter.Visible = false;
            //DebugSystem.Instance.TimeRuler.Visible = false;
            //DebugSystem.Instance.TimeRuler.ShowLog = false;

            ////DebugSystem.Instance.FpsCounter.DrawOrder = 1000;
            //DebugSystem.Instance.TimeRuler.DrawOrder = 1000;

            DisplayModeCollection coll = GraphicsAdapter.DefaultAdapter.SupportedDisplayModes;
            
            foreach (DisplayMode mode in coll)
                modes += mode.ToString() + ":::\n";
            
            graphics.PreferredBackBufferWidth = windowWidth;
            graphics.PreferredBackBufferHeight = windowHeight;
            graphics.IsFullScreen = false;

            Window.Title = "Terrain Tutorial";
            

            device = graphics.GraphicsDevice;
            graphics.PreferMultiSampling = true;

            spriteBatch = new SpriteBatch(device);

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                device.Viewport.AspectRatio,
                1f,
                800);

            

            SetThreads();            

            // To test the trial mode behavior while developing your game,
            // uncomment this line:

            Guide.SimulateTrialMode = true;

            PrevLevelNum = LevelNum;

            graphics.ApplyChanges();

            base.Initialize();
        }        

        protected override void LoadContent()
        {
            InitializeRenderTargets();

            LoadLoadingAnimation();

            objEffect = Content.Load<Effect>("Effect\\MyVSM");

           // How to use:
    /// Added TimerRuler instance to Game.Components and call timerRuler.StartFrame in
    /// top of the Game.Update method.
    /// 
            

            Menu_Load_List = new MenuLoadList();
            MenuVariables = new MenuResources();
            MenuObjects = new ResourceList();
            LoadedResources = new ResourceList();

            GetLevelStats();

            MenuLoadMenu();
            //Loading stage loads content in background
            //Need to check its progress, display "loading",
            //and continue after completion


        }

        bool Loading = false;
        bool LoadingComplete = false;
        TimeSpan loadWait = TimeSpan.Zero;
        LoadingGameScreen loadingLevel;

        void LoadLevel(GameTime gameTime)
        {
            Game_State = GameState.MenuLoaded;
            LoadingComplete = false;
            Loading = false;

            if (Level_Loader != null)
            {
                Level_Loader.Stop();
            }

            if (loadWait > TimeSpan.FromSeconds(0.2d))
            {
                if (!LevelStat[LevelNum - 1].locked)
                {
                    LoadedResources = new ResourceList();
                    Level_Loader = new LevelLoader(this);
                    Level_Loader.LoadLevel(LevelNum);
                    Load_FrameCount = 0;
                    Game_State = GameState.LoadingLevel;
                    Loading = true;
                    LoadingComplete = false;
                    loadWait = TimeSpan.Zero;

                    if (loadingLevel != null)
                        if (loadingLevel.IsActive)
                            loadingLevel.ExitScreen();

                    loadingLevel = new LoadingGameScreen(Level_Loader);
                    loadingLevel.ScreenManager = screenManager;
                }
            }
            else
                loadWait += gameTime.ElapsedGameTime;
            
        }

        bool MenuLoaded = false;
        protected override void Update(GameTime gameTime)
        {
            this.gameTime = gameTime;

            //DebugSystem.Instance.TimeRuler.StartFrame();

            
            HandleInput();

            #region Loading Menu
            if (Game_State == GameState.LoadingMenu)
            {                
                if (Loading_Stage.CheckProgress() < 1.0000f)
                {
                    Game_State = GameState.LoadingMenu;
                }
                else
                {
                    
                    if(!ScreensStarted)
                        MenuPostLoad();

                    if (ScreensLoaded)
                        MenuLoaded = true;
                }
            }
            #endregion

#if XBOX360
            if (MenuLoaded && (LoadGameComplete == false))
            {
                LoadSaveGame();
            }
#endif

            #region PRELOAD LEVEL

            if (LevelNum == 0)
                GetLevelNum();

            if (MenuLoaded && !Loading && !LoadingComplete)
            {
                LoadLevel(gameTime);
            }

            if (MenuLoaded && (PrevLevelNum != LevelNum))
            {
                PrevLevelNum = LevelNum;
                LoadLevel(gameTime);
            }
            
            if(loadingLevel != null)
                loadingLevel.Update(gameTime, false, false, Loading);

            #endregion


            #region  GAMEPLAY SESSION
            if (Game_State == GameState.LevelStarted)
            {
                //Level_Loader = null;
                if (!Paused)
                {
                    float timeStep = (float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;
                    if(SlowTime)
                        PhysicsSystem.CurrentPhysicsSystem.Integrate(timeStep / 2);
                    else
                        PhysicsSystem.CurrentPhysicsSystem.Integrate(timeStep);
                }

               // bloom.Settings = BloomSettings.PresetSettings[5];
                audioEngine.Update();

                if (this.TargetElapsedTime > TimeSpan.FromSeconds((double) frmRate))
                    TargetElapsedTime = TimeSpan.FromSeconds(frmRate);

            }
            #endregion
            

            if (GameSaveRequested)
                SaveGame();

            if (LoadGameRequested)
                LoadSaveGame();

            base.Update(gameTime);
        }

        public void StopLoading()
        {
            Level_Loader.Stop();
        }

        SaveGameData save = new SaveGameData();

        float frmRate = 1.0f / 61.0f;
        double loadRate = 1.0d / 30.0d;
        protected override void Draw(GameTime gameTime)
        {
            
            device.Clear(ClearOptions.Target, Color.Black, 0, 0);

            if(Game_State == GameState.LoadingMenu)
            {
                ++Load_FrameCount;
                if (Load_FrameCount > 2)
                {
                    Load_FrameCount = 0;
                    LoadingAnimFrame++;
                }

                this.TargetElapsedTime = TimeSpan.FromSeconds(loadRate);

                DrawMenuLoading();
            }

            else if (Game_State == GameState.LoadingLevel)
            {
                this.TargetElapsedTime = TimeSpan.FromSeconds(loadRate);

                if (Level_Loader.CheckProgress() < 1.000f)
                {
                    //++Load_FrameCount;
                    //if (Load_FrameCount > 5)
                    //{
                    //    Load_FrameCount = 0;

                    //device.Clear(Color.Black);
                    //DrawMenuLoading();

                    //}
                }
                else
                {
                    Game_State = GameState.LevelLoaded;
                    Loading = false;
                    LoadingComplete = true;
                }
            }
            
            base.Draw(gameTime);

            if (loadingLevel != null)
                loadingLevel.Draw(gameTime);
        }

        private void DrawSavingLoading()
        {
            spriteBatch.Begin();
            string title = "Opening...";
            if (GameSaveRequested)
                title += " Saving";
            else
                title += " Loading";

            
            spriteBatch.DrawString(
                screenManager.Font,
                title,
                new Vector2(400, 50),                
                Color.White);
            spriteBatch.End();
        }
                

        protected override void UnloadContent()
        {
            Level_Loader.Stop();
            Thread.Sleep(100);            
        }

        void InitializeRenderTargets()
        {
            PresentationParameters pp = device.PresentationParameters;

            renderTarget1 = new RenderTarget2D(graphics.GraphicsDevice,
                                                    pp.BackBufferWidth,
                                                    pp.BackBufferHeight,
                                                    true, SurfaceFormat.Vector4, DepthFormat.Depth24);

            graphics.PreferMultiSampling = true;
        }

        PhysicsSystem world;
        public void InitializePhysics()
        {
            world = new PhysicsSystem();
            world.CollisionSystem = new CollisionSystemSAP();
            world.SolverType = PhysicsSystem.Solver.Normal;
        }

        

        void LoadLoadingAnimation()
        {
            //Load animation for loading [0 - 16] in two loops
            LoadingAnimation = new Texture2D[16];
            for (int i = 0; i < 10; ++i)
            {
                LoadingAnimation[i] = Content.Load<Texture2D>("Loading\\IMG0000" + i);
            }
            for (int i = 10; i < 16; ++i)
            {
                LoadingAnimation[i] = Content.Load<Texture2D>("Loading\\IMG000" + i);
            }
        }

        void MenuLoadMenu()
        {
            Load_FrameCount = 0;
            Loading_Stage = new LoadingStage(MenuObjects, this, true);

            List<String> resList = MenuGetTextures();
            Loading_Stage.AddToTex2DList(resList);

            resList = MenuGetModels();
            Loading_Stage.AddToModelList(resList);

            Loading_Stage.LoadContent();
        }

        List<String> MenuGetTextures()
        {
            List<String> toAdd = new List<string>();

            foreach (KeyValuePair<string,string> KeyPair in Menu_Load_List.Textures)
                toAdd.Add(Menu_Load_List.Textures[KeyPair.Key]);

            return toAdd;
        }

        List<String> MenuGetModels()
        {
            List<String> toAdd = new List<string>();

            foreach (KeyValuePair<string, string> KeyPair in Menu_Load_List.Models)
                toAdd.Add(Menu_Load_List.Models[KeyPair.Key]);

            return toAdd;
        }

        void MenuPostLoad()
        {
            // SCREENS FROM NETWORK STATE MANAGEMENT
            screenManager = new ScreenManager(this);
            Components.Add(screenManager);

            
            //bloom = new BloomComponent(this);

            //Components.Add(bloom);

            backgroundThread.Start();
            backgroundThread.Name = "Initialize Screens";
            ScreensStarted = true;
           
        }


        /// <summary>
        /// This is run as a background thread.  Variables will not be used in main game until
        /// ScreensLoaded is set to true.
        /// </summary>
        void InitializeScreens()
        {

            //MessageDisplayComponent md = new MessageDisplayComponent(this);
            //Components.Add(new MessageDisplayComponent(this));
            //GamerServicesComponent cd = new GamerServicesComponent(this);
            //Components.Add(new GamerServicesComponent(this));

            SignedInGamer.SignedIn += new EventHandler<SignedInEventArgs>(SignedInGamer_SignedIn);

            // Assigns Loaded Resources to Menu Variables to be used later 
            string address = Menu_Load_List.Models["MenuModel"];
            MenuVariables.MyModel = MenuObjects.ModelList[address].Model_rez;
            address = Menu_Load_List.Textures["BackgroundTime"];
            MenuVariables.TimeBackground = MenuObjects.Tex2dList[address];
            address = Menu_Load_List.Textures["BackgroundScreen"];
            MenuVariables.NormalBackground = MenuObjects.Tex2dList[address];
            address = Menu_Load_List.Textures["MenuSelect"];
            MenuVariables.MenuSelect = MenuObjects.Tex2dList[address];

            // Activate the first screens.
            
            BackgroundScreen BACK = new BackgroundScreen();
            MainMenuScreen main = new MainMenuScreen(this);

            screenManager.AddScreen(BACK, null);
            screenManager.AddScreen(main, null);


            // Listen for invite notification events.
            NetworkSession.InviteAccepted += (sender, e)
                => NetworkSessionComponent.InviteAccepted(screenManager, e);

            ScreensLoaded = true;

            //screenManager.DrawOrder = 1;

            InitializePhysics();

            audioEngine = new AudioEngine("Content\\Audio\\ArtillarySound.xgs");
            waveBank = new WaveBank(audioEngine, "Content\\Audio\\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, "Content\\Audio\\Sound Bank.xsb");
        }

        /// <summary>
        /// Handle signed in gamer event as start avatar loading
        /// </summary>
        void SignedInGamer_SignedIn(object sender, SignedInEventArgs e)
        {
            // Only load the avatar for player one
            if (e.Gamer.PlayerIndex == PlayerIndex.One)
            {
                // Load the player one avatar
                LoadAvatar(e.Gamer);
            }
        }

        #region Avatar Loading

        /// <summary>
        /// Load the avatar for a gamer
        /// </summary>
        private void LoadAvatar(Gamer gamer)
        {
            UnloadAvatar();

            AvatarDescription.BeginGetFromGamer(gamer, LoadAvatarDescription, null);
        }


        /// <summary>
        /// AsyncCallback for loading the AvatarDescription
        /// </summary>
        private void LoadAvatarDescription(IAsyncResult result)
        {
            // Get the AvatarDescription for the gamer
            avatarDescription = AvatarDescription.EndGetFromGamer(result);

            // Load the AvatarRenderer if description is valid
            if (avatarDescription.IsValid)
            {
                avatarRenderer = new AvatarRenderer(avatarDescription);
                avatarRenderer.Projection = projectionMatrix;
            }
            // Load random for an invalid description
            else
            {
                LoadRandomAvatar();
            }

            
            //List<Matrix> BONES = new List<Matrix>(71);

            ////head
            //// == matrix from head bone in local space
            //BONES[19] = new Matrix();

            ////back  0 , 1 , 5
            //// == matrix from chest bone in world space
            //BONES[0] = new Matrix();
            
            //avatarRenderer.Draw(BONES, new AvatarExpression());
            if (avatarDescription.IsValid)
            {
                Console.WriteLine(" VALID AVATAR ");
            }
            
        }


        /// <summary>
        /// Load a random avatar
        /// </summary>
        private void LoadRandomAvatar()
        {
            UnloadAvatar();

            avatarDescription = AvatarDescription.CreateRandom();
            avatarRenderer = new AvatarRenderer(avatarDescription);
            avatarRenderer.Projection = projectionMatrix;
        }


        /// <summary>
        /// Unloads the current avatar
        /// </summary>
        private void UnloadAvatar()
        {
            // Dispose the current Avatar
            if (avatarRenderer != null)
            {
                avatarRenderer.Dispose();
                avatarRenderer = null;
            }
        }

        #endregion

        public void DrawMenuLoading()
        {
            if (LoadingAnimFrame >= LoadingAnimation.Length)
                LoadingAnimFrame = 0;

            spriteBatch.Begin();    
            
            spriteBatch.Draw(
                LoadingAnimation[LoadingAnimFrame],
                new Vector2(windowWidth / 2, windowHeight / 2),
                null,
                Color.White, 0.0f,
                new Vector2(LoadingAnimation[0].Width / 2, LoadingAnimation[0].Height / 2),
                0.3f,
                SpriteEffects.None, 0.0f);

            spriteBatch.End();
        }

        private void DrawMenu()
        {
            //device.Clear(Color.Black);

            //spriteBatch.Begin();

            //spriteBatch.Draw(MenuVariables.MyBackground,    // Texture2D
            //    new Vector2(windowWidth/2, windowHeight/2), // Position
            //    null, Color.White,
            //    0.0f,                                       // Rotation
            //    new Vector2(
            //    MenuVariables.MyBackground.Width / 2,       // Origin
            //    MenuVariables.MyBackground.Height / 2),
            //    1.0f,                                       // Scale
            //    SpriteEffects.None, 0.0f);

            //spriteBatch.End();
        }

        void HandleInput()
        {
            KeyboardState prevKeyState = keyboardState;
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            //if (keyboardState.IsKeyDown(Keys.T) &&
            //    prevKeyState != keyboardState)
            //{
            //    bloomSettingsIndex = (bloomSettingsIndex + 1) %
            //                             BloomSettings.PresetSettings.Length;
            //    bloom.Settings = BloomSettings.PresetSettings[bloomSettingsIndex];
            //}

            //// Allows the game to exit
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            //    keyboardState.IsKeyDown(Keys.Escape))
            //    this.Exit();
            
        }

        void SetThreads()
        {
            backgroundThread = new Thread(InitializeScreens);
            backgroundThreadExit = new ManualResetEvent(false);
        }

        public void GetLevelNum()
        {
            if (NormalMode)
            {
                for (int i = 0; i < 12; ++i)
                {
                    LevelNum = i + 1;
                    if (!LevelStat[i].locked && !LevelStat[i].passed)
                        i = 12;
                }
            }
            else
            {
                for (int i = 0; i < 12; ++i)
                {
                    LevelNum = i + 21;
                    if (!LevelStat[i + 20].locked && !LevelStat[i + 20].passed)
                        i = 12;
                }
            }

        }

        void GetLevelStats()
        {
            LevelStat = new LevelStats[32];

            for(int i = 0; i < 32; ++i)
                LevelStat[i] = new LevelStats();
           
            LevelStat[0].locked = false;
            LevelStat[0].passed = false;

            LevelStat[20].locked = false;
            LevelStat[20].passed = false;
        }

        void GetLevelStats(SaveGameData save)
        {
            LevelStat = new LevelStats[32];

            for (int i = 0; i < save.LevelStat.Length; ++i)
                LevelStat[i] = save.LevelStat[i];
        }

        void PutLevelStats()
        {
            save = new SaveGameData();
            save.LevelStat = new LevelStats[32];

            for (int i = 0; i < 32; ++i)
               save.LevelStat[i] = LevelStat[i];
        }

        IAsyncResult result;
        Object stateobj;
        public bool GameSaveRequested = false;
        public bool LoadGameRequested = false;
        GamePadState currentState;

       
        public struct SaveGameData
        {
            public LevelStats[] LevelStat;
        }


        public void SaveGame()
        {
            // Set the request flag
            if ((!Guide.IsVisible) && (GameSaveRequested == false))
            {
                GameSaveRequested = true;
                result = StorageDevice.BeginShowSelector(
                        PlayerIndex.One, null, null);
            }

            // If a save is pending, save as soon as the
            // storage device is chosen
            if ((GameSaveRequested) && (result.IsCompleted))
            {
                StorageDevice device = StorageDevice.EndShowSelector(result);
                if (device != null && device.IsConnected)
                {
                    DoSaveGame(device);
                }
                // Reset the request flag
                GameSaveRequested = false;
            }

        }

        bool LoadGameComplete = false;
        void LoadSaveGame()
        {
            // Set the request flag
            if ((!Guide.IsVisible) && (LoadGameRequested == false))
            {
                LoadGameRequested = true;
                LoadGameComplete = false;
                result = StorageDevice.BeginShowSelector(
                        PlayerIndex.One, null, null);
            }

            // If a save is pending, save as soon as the
            // storage device is chosen
            if ((LoadGameRequested) && (result.IsCompleted))
            {
                StorageDevice device = StorageDevice.EndShowSelector(result);
                if (device != null && device.IsConnected)
                {
                    OpenSave(device);
                }
                // Reset the request flag
                LoadGameRequested = false;
                LoadGameComplete = true;
            }

        }

        private void DoSaveGame(StorageDevice device)
        {
            // Create the data to save.
            SaveGameData data = new SaveGameData();
            PutLevelStats();
            data = save;


            // Open a storage container.
            IAsyncResult result =
                device.BeginOpenContainer(GameTitle, null, null);

            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            StorageContainer container = device.EndOpenContainer(result);

            // Close the wait handle.
            result.AsyncWaitHandle.Close();


            string filename = "savegame.sav"; 

            // Check to see whether the save exists.
            if (container.FileExists(filename))
                // Delete it so that we can create one fresh.
                container.DeleteFile(filename);

            // Create the file.
            Stream stream = container.CreateFile(filename);

            // Convert the object to XML data and put it in the stream.
            XmlSerializer serializer = new XmlSerializer(typeof(SaveGameData));

            serializer.Serialize(stream, data);

            // Close the file.
            stream.Close();

            // Dispose the container, to commit changes.
            container.Dispose();
        }

        void OpenSave(StorageDevice device)
        {
            // Open a storage container.
            IAsyncResult result =
                device.BeginOpenContainer(GameTitle, null, null);

            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            StorageContainer container = device.EndOpenContainer(result);

            // Close the wait handle.
            result.AsyncWaitHandle.Close();

            string filename = "savegame.sav";

            // Check to see whether the save exists.
            if (!container.FileExists(filename))
            {
                GetLevelStats();
                // If not, dispose of the container and return.
                container.Dispose();
                return;
            }

            // Open the file.
            Stream stream = container.OpenFile(filename, FileMode.Open);

            XmlSerializer serializer = new XmlSerializer(typeof(SaveGameData));

            SaveGameData data = (SaveGameData)serializer.Deserialize(stream);

            save = data;

            GetLevelStats(save);

            GetLevelNum();

            // Close the file.
            stream.Close();

            // Dispose the container.
            container.Dispose();

        }
    }
}
