using System;
using System.Collections.Generic;
using System.Linq;

using System.Collections;
using System.Xml;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using Ray = Microsoft.Xna.Framework.Ray;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

using JigLibX.Physics;
using JigLibX.Geometry;
using JigLibX.Collision;


namespace TerrainTutorial
{
    /// <summary>
    /// Entry point for main program.
    /// 
    /// Loading stage: load menu resorces.  Draw loading splash and intro.
    /// Menu system:
    /// PreLoad game:
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {

        //physics collision drawer
        DebugDrawer debugDrawer;

        public System.Windows.Forms.Form 
            GAME_WINDOW;

        //level edit
        public LevelEditor Editor;
        public Form2 form2;
        public Form1 form1;
        public Color currentColor = Color.Brown;

        public Level myLevel = new Level();

        Dictionary<string, string> level_load_list;
        TerrainEngine.TerrainArguments terrainArgument;

        /// <summary>
        /// Contains acctual models and textures.  Is populated by a Loading Stage.
        /// Use to assign multiple objects the same model or texture.
        /// </summary>
        public ResourceList LoadedResources;

        enum GameMode
        {
            PlayMode = 0,
            MoveMode = 1,
            TerrainMode = 2,
            ScriptMode = 3,
            MAX = 4
        }
        GameMode CurrentMode = GameMode.MoveMode;

        // SCRIPT MODE
        int VertChangFrame = 0;

        int LoadingAnimFrame = 0;
        Texture2D[] LoadingAnimation;
        
        LoadingStage load_stage;
        int load_frame_count;
        float load_progress = 0.0f;
        DateTime timeStart;
        TimeSpan timeEnd;
        bool LOADING = false;
        bool Loaded = false;
        bool DrawLoaded = false;

        
        //-- DEFAULT VALUES --
        float PLAYER_SCALE = 2.0f;

        Vector3 TANK_POSITION = new Vector3(72, 20, -10);
        Vector3 CAM_POSITION =  new Vector3(72, 80, -99);
        public bool DRAW_MODE = false;

        Vector3 CAM_POSITION1 = new Vector3(200, 500, -200);
        
        

        #region draw_mode_variables
        String drawModeString;
        List<Vector3> posList;
        List<int> vectorList;
        List<int> BigChangeList;
        int brushWidth = 5;
        float changeHeight = 1;
        Texture2D cursorTexture;
        Vector2 textureCenter;
        #endregion draw_mode_variables

        #region graphics_variables

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public GraphicsDevice device;
        const int windowWidth = 800;        
        const int windowHeight = 500;

        #endregion graphics_variables

        KeyboardState keyboardState;
        MouseState prevMouseState;
        MouseState mouseState;

        SpriteFont font;

        public TerrainEngine terrainEngine;
        public MouseCamera mouseCamera;

        //---------------------------------
        PlayerTank playerTank;
        PlayerTank playerTank2;

        PlayerTank[] playerTanks;
        int numTanks = 0;

        List<EnemyTank> enemyTanks;

        // GAME OBJECT MODELS... SHOULD BE STATIC
        public Model tankModel;
        public Texture2D[] tankTextures;
        public Model missileModel;
        public Texture2D[] missileTextures;
        public Model cornerModel;
        //---------------------------------
       

        //---------------------------------
        public Matrix worldMatrix;
        public Matrix projectionMatrix;
        public Matrix viewMatrix;
        //---------------------------------
        
        //-------EXAMPLE-------------
        Matrix WORLDMatrix = Matrix.CreateScale(1.0f)  //SCALE
            * Matrix.CreateRotationY(MathHelper.Pi) // ROTATED
            * Matrix.CreateTranslation(-3, 0, -15); // POSITION
        //---------------------------------


        #region FRAM RATE
        
        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;

        int frameRateU = 0;
        int frameCounterU = 0;
        TimeSpan elapsedTimeU = TimeSpan.Zero;
        
        #endregion FRAM RATE
        

        #region effects_variables
        public Effect objEffect;
        Effect myVSM;
        Effect GausBlur;
        bool toonLighting = true;
        bool drawText = true;
        bool edgeDetect = true;

        public float NormalThreshold = 0.1f;
        public float DepthThreshold = 0.01f;

        public float NormalSensitivity = 1;
        public float DepthSensitivity = 40;

        public float EdgeWidth = 1;
        public float EdgeIntensity = 2.5f;
        
        public Blend blendEnumS = Blend.One;
        public Blend blendEnumD = Blend.One;
        
        //-----SHADOW MAP------------------        
        // light for 'sun' - directional, orthagonal light        
        public Vector3 lightDir = new Vector3(30.0f, 87.0f, 0.0f);
        Vector3 lightPos = new Vector3(4.0f, 6.0f, -6.0f);
        public float lightPower;
        public float maxLightPower = 3;
        public float ambientPower;
        public float maxAmbiePower = 2;
        // matrix for rendering from light perspective, always changing
        Matrix lightViewProjection;

        // render targets for effects
        RenderTarget2D renderTarget1, renderTarget2;
        DepthStencilBuffer shadowDepthBuffer;
        Texture2D shadowMap;
       
        
        const int shadowMapWidthHeight = 5000;
        int shadowMapW, shadowMapH;
        #endregion effects_variables


        #region particles_variables
        public ParticleSystem explosionParticles;
        public ParticleSystem explosionSmokeParticles;
        public ParticleSystem projectileTrailParticles;
        public ParticleSystem smokePlumeParticles;
        public ParticleSystem fireParticles;

        #endregion particles_variables


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            

            // Construct our particle system components.
            explosionParticles = new ExplosionParticleSystem(this, Content);
            explosionSmokeParticles = new ExplosionSmokeParticleSystem(this, Content);
            projectileTrailParticles = new ProjectileTrailParticleSystem(this, Content);
            smokePlumeParticles = new SmokePlumeParticleSystem(this, Content);
            fireParticles = new FireParticleSystem(this, Content);

            // Set the draw order so the explosions and fire
            // will appear over the top of the smoke.
            smokePlumeParticles.DrawOrder = 100;
            explosionSmokeParticles.DrawOrder = 200;
            projectileTrailParticles.DrawOrder = 300;
            explosionParticles.DrawOrder = 400;
            fireParticles.DrawOrder = 500;

            // Register the particle system components.
            Components.Add(explosionParticles);
            Components.Add(explosionSmokeParticles);
            Components.Add(projectileTrailParticles);
            Components.Add(smokePlumeParticles);
            Components.Add(fireParticles);



            GAME_WINDOW = 
            (System.Windows.Forms.Form)
            System.Windows.Forms.Form.FromHandle(
            this.Window.Handle);



            InitializePhysics();

            debugDrawer = new DebugDrawer(this);
            debugDrawer.Enabled = true;
            Components.Add(debugDrawer);
        }

        BoxActor fallingBox;
        //BoxActor immovableBox;
        TriangleMeshActor terrainActor;
        VertexPositionColor[] terrainFrm;
        ForceControler fallBoxForce;
       
 
        private void InitializePhysics()
        {
            PhysicsSystem world = new PhysicsSystem();
            world.CollisionSystem = new CollisionSystemSAP();
            world.SolverType = PhysicsSystem.Solver.Normal;

            //world.CollisionTollerance = 0.0001f;
            //world.AllowedPenetration = 0.001f;
            

            fallingBox = new BoxActor(this, new Vector3(85, 25, -65), new Vector3(3f, 2f, 4f));
            //immovableBox = new BoxActor(this, new Vector3(40, 15, -25), new Vector3(5f, 1f, 5f));
            //immovableBox.Body.Immovable = true;
            Components.Add(fallingBox);
            //Components.Add(immovableBox);
                        
           
            fallBoxForce = new ForceControler(fallingBox.Body, new Vector3(0, 0, 0), false);
            //fallBoxForce.Force = new Vector3(0, 19, 0);

           
        }

        
 
        protected override void Initialize()
        {
            graphics.MinimumVertexShaderProfile = ShaderProfile.VS_2_0;
            graphics.MinimumPixelShaderProfile = ShaderProfile.PS_2_0;

            graphics.PreferredBackBufferWidth = windowWidth;
            graphics.PreferredBackBufferHeight = windowHeight;
            graphics.IsFullScreen = false;
            
            Window.Title = "Terrain Tutorial";
            graphics.ApplyChanges();


            device = graphics.GraphicsDevice;
            spriteBatch = new SpriteBatch(GraphicsDevice);

            
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
               device.Viewport.AspectRatio,
               1f, 10000);

            

            prevMouseState = Mouse.GetState();

            

            base.Initialize();

            
            //this.IsFixedTimeStep = false;
            //graphics.SynchronizeWithVerticalRetrace = false;
        }        
 
        protected override void LoadContent()
        {

            LoadedResources = new ResourceList();           

            font = Content.Load<SpriteFont>("Font\\SpriteFont1");
          
            myVSM = Content.Load<Effect>("Effect\\MyVSM");
            GausBlur = Content.Load<Effect>("Effect\\GaussianBlur");
            objEffect = myVSM;

            lightPower = 0.9f;
            ambientPower = 0.2f;

            InitializeRenderTargets();

            // DRAW MODE VARIABLES

            //----MOVE TO APPROPRIATE CLASS---
            posList = new List<Vector3>();
            vectorList = new List<int>();
            BigChangeList = new List<int>();
            //----MOVE TO APPROPRIATE CLASS---

            cursorTexture = Content.Load<Texture2D>("Cursor\\cursor");
            textureCenter = new Vector2(
                cursorTexture.Width / 2, cursorTexture.Height / 2);

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



            //----MOVE TO APPROPRIATE CLASS---
            GetLevelFromXML();
            PutLevelInXML();
            //----MOVE TO APPROPRIATE CLASS---

            //----MOVE TO APPROPRIATE CLASS---
            Editor = new LevelEditor(this);
            //----MOVE TO APPROPRIATE CLASS---
           
        }
        
        void InitializeRenderTargets()
        {
            PresentationParameters pp = device.PresentationParameters;

            shadowMapW = pp.BackBufferWidth;
            shadowMapH = pp.BackBufferHeight;

            // Create new floating point render target
            renderTarget1 = new RenderTarget2D(graphics.GraphicsDevice,
                                                    shadowMapW,
                                                    shadowMapH,
                                                    1, SurfaceFormat.Vector4);

            renderTarget2 = new RenderTarget2D(graphics.GraphicsDevice,
                                                    shadowMapW,
                                                    shadowMapH,
                                                    1, SurfaceFormat.Vector4);


            // Create depth buffer to use when rendering to the shadow map
            shadowDepthBuffer = new DepthStencilBuffer(graphics.GraphicsDevice,
                                                       shadowMapW,
                                                       shadowMapH,
                                                       DepthFormat.Depth24);
        }



        void LoadLevel()
        {
            if (load_stage != null)
                //load_stage = new LoadingStage(LoadedResources, this);
            //else
            {


                load_stage.backgroundThread.Abort();
                load_stage.EndLoading();

                while (load_stage.backgroundThread.IsThreadPoolThread)
                {
                    //wait?
                }
                load_stage.SetThreads();
            }

            Loaded = false;
            LOADING = false;

            load_stage = new LoadingStage(LoadedResources, this);

            timeStart = DateTime.Now;
            timeEnd = TimeSpan.Zero;
            load_frame_count = 0;

            GetLoadList();

            List<String> resList = GetLevelTextures();            
            load_stage.AddToTex2DList(resList);

            resList = GetLevelModels();
            load_stage.AddToModelList(resList);

            load_stage.LoadContent(); 
        }

        void GetLoadList()
        {
            level_load_list  = new Dictionary<string,string> ();
            level_load_list.Add("Terrain", myLevel.terrainMap);
            level_load_list.Add("TerrainTexture1", myLevel.texture1);
            level_load_list.Add("TerrainTexture2", myLevel.texture2);
            level_load_list.Add("TerrainTexture3", myLevel.texture3);
            level_load_list.Add("TerrainTexture4", myLevel.texture4);
            level_load_list.Add("DecalTexture", myLevel.decalTexture);

            level_load_list.Add("PlayerTankModel", "Model\\block3");
            level_load_list.Add("PlayerRocketModel", "Model\\missile");

            level_load_list.Add("SkyDome", "Terrain\\dome");
            level_load_list.Add("SkyTexture", "Terrain\\clouds");
        }

        List<String> GetLevelTextures()
        {
            List<String> toAdd = new List<string>();

            toAdd.Add(level_load_list["Terrain"]);
            toAdd.Add(level_load_list["TerrainTexture1"]);
            toAdd.Add(level_load_list["TerrainTexture2"]);
            toAdd.Add(level_load_list["TerrainTexture3"]);
            toAdd.Add(level_load_list["TerrainTexture4"]);
            toAdd.Add(level_load_list["DecalTexture"]);
            toAdd.Add(level_load_list["SkyTexture"]);

            return toAdd;
        }

        List<String> GetLevelModels()
        {
            List<String> toAdd = new List<string>();
            toAdd.Add(level_load_list["PlayerTankModel"]);
            toAdd.Add(level_load_list["PlayerRocketModel"]);
            toAdd.Add(level_load_list["SkyDome"]);

            toAdd.Add("Model\\sphere");
            
            return toAdd;
        }

        VertexPositionColor[] TerrainFrame;
        void LoadTerrain()
        {

            terrainArgument = new TerrainEngine.TerrainArguments();
            terrainArgument.heightMap = LoadedResources.Tex2dList[level_load_list["Terrain"]];

            terrainArgument.terrainTexture1 = LoadedResources.Tex2dList[level_load_list["TerrainTexture1"]];
            terrainArgument.terrainTexture2 = LoadedResources.Tex2dList[level_load_list["TerrainTexture2"]];
            terrainArgument.terrainTexture3 = LoadedResources.Tex2dList[level_load_list["TerrainTexture3"]];
            terrainArgument.terrainTexture4 = LoadedResources.Tex2dList[level_load_list["TerrainTexture4"]];
            terrainArgument.decalTexture = LoadedResources.Tex2dList[level_load_list["DecalTexture"]];
            terrainArgument.skyTexture = LoadedResources.Tex2dList[level_load_list["SkyTexture"]];
            String key = level_load_list["SkyDome"];
            terrainArgument.skyDome = LoadedResources.ModelList[key].Model_rez;
            

            terrainArgument.terrainScale = myLevel.terrainScale;

            terrainEngine = new TerrainEngine(this, mouseCamera, terrainArgument);
            
            terrainEngine.LoadContent();




            terrainActor = new TriangleMeshActor(this, new Vector3(0,0,0), new Vector3(1, 1, 1),
                terrainEngine.vertices, terrainEngine.indices);
            terrainActor.Body.Immovable = true;


            Components.Add(terrainActor);

            TerrainFrame = terrainActor.Skin.GetLocalSkinWireframe();
            terrainActor.Body.TransformWireframe(TerrainFrame);

        }

        void LoadObjects()
        {

            tankModel = LoadedResources.ModelList[level_load_list["PlayerTankModel"]].Model_rez;
            tankTextures = LoadedResources.ModelList[level_load_list["PlayerTankModel"]].Textures_rez;
            missileModel = LoadedResources.ModelList[level_load_list["PlayerRocketModel"]].Model_rez;
            missileTextures = LoadedResources.ModelList[level_load_list["PlayerRocketModel"]].Textures_rez;
            cornerModel = LoadedResources.ModelList["Model\\sphere"].Model_rez;


            playerTank = new PlayerTank(this, tankModel, tankTextures, myLevel.playerSpawn.Position);
            playerTank.Load();
            playerTank.scale = PLAYER_SCALE;
            playerTank.alive = true;

            //playerTank.position = TANK_POSITION;
            playerTank.position = myLevel.playerSpawn.Position;

            playerTank2 = new PlayerTank(this, tankModel, tankTextures, myLevel.playerSpawn.Position);
            playerTank2.Load();
            playerTank2.scale = PLAYER_SCALE;
            playerTank2.alive = true;

            playerTank2.position = TANK_POSITION;
            playerTank2.position = new Vector3 (playerTank2.position.X - 20, playerTank2.position.Y,
                playerTank2.position.Z);

            playerTanks = new PlayerTank[numTanks];

            for (int i = 0; i < numTanks; ++i)
            {
                playerTanks[i] = new PlayerTank(this, tankModel, tankTextures, myLevel.playerSpawn.Position);
                playerTanks[i].Load();
                playerTanks[i].scale = PLAYER_SCALE;
                playerTanks[i].alive = true;

                playerTanks[i].position = TANK_POSITION;
                playerTanks[i].position = new Vector3(
                    playerTank2.position.X + 20 * (1 + i),
                    playerTank2.position.Y,
                    playerTank2.position.Z); ;
            }

            enemyTanks = new List<EnemyTank>();
        }

        void LoadCamera()
        {
            mouseCamera = new MouseCamera(this);

            mouseCamera.LoadContent();

            mouseCamera.cameraPosition = myLevel.cameraSpawn.Position;
            mouseCamera.leftrightRot = MathHelper.PiOver2;

        }

        

       
        //private Model LoadModel(String assetName, out Texture2D[] textures)
        //{
        //    // create model
        //    // find num of textures
        //    // assign textures to array
        //    // clone current effect into model effects

        //    Model newModel = Content.Load<Model>(assetName);

        //    int i = 0;
        //    foreach (ModelMesh mesh in newModel.Meshes)
        //        foreach (BasicEffect currentEffect in mesh.Effects)
        //            i++;

        //    textures = new Texture2D[i];


        //    i = 0;
        //    foreach (ModelMesh mesh in newModel.Meshes)
        //        foreach (BasicEffect currentEffect in mesh.Effects)
        //            textures[i++] = currentEffect.Texture;

        //    foreach (ModelMesh mesh in newModel.Meshes)
        //        foreach (ModelMeshPart meshPart in mesh.MeshParts)
        //            meshPart.Effect = myVSM.Clone(device);

        //    return newModel;
        //}

        protected override void UnloadContent()
        {
           
        }

        
       
        protected override void Update(GameTime gameTime)
        {
                        
            frameCounterU++;
            UpdateFrameRate(gameTime);

            HandleInput();            

            float timeStep = (float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;
            PhysicsSystem.CurrentPhysicsSystem.Integrate(timeStep);


            #region Loading
            if (load_stage != null)
            {
                if (!Loaded)
                {
                    load_progress = load_stage.CheckProgress();
                    ++load_frame_count;
                    if (load_progress < 1)
                    {
                        LOADING = true;
                    }
                    else
                    {
                        LOADING = false;
                        Loaded = true;

                        LoadTerrain();
                        LoadObjects();
                        LoadCamera();

                        timeEnd = DateTime.Now - timeStart;

                        load_stage.EndLoading();

                        //remove references to unneeded assests
                        load_stage.UpdateReferenceList();

                        DrawLoaded = true;
                    }
                }
                
                
            }
            else
            // stage not loaded
            {
                LoadLevel();
            }
            #endregion


            // RUN UPDATES FOR LEVEL RELATED OBJECTS
            if (DrawLoaded)
            {
                mouseCamera.Update(gameTime);

                UpdateLightData();

                if (CurrentMode == GameMode.PlayMode
                    || CurrentMode == GameMode.MoveMode)
                {
                    playerTank.Update(gameTime);
                    playerTank2.Update(gameTime);
                    for (int i = 0; i < numTanks; ++i)
                    {
                        playerTanks[i].Update(gameTime);
                    }
                }

                if (CurrentMode == GameMode.ScriptMode)
                {
                    Editor.Update();
                }

               
               
            }

            base.Update(gameTime);
        }

        void HandleInput()
        {            
            KeyboardState prevKeyState = keyboardState;
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.Escape))
                this.Exit();

            if (GAME_WINDOW.Focused)
            {
                #region mode

                // MODE
                if (keyboardState.IsKeyDown(Keys.Z)
                   && prevKeyState != keyboardState)
                {
                    DRAW_MODE = false;
                    CurrentMode = GameMode.PlayMode;
                    form1.Hide();
                }

                if (keyboardState.IsKeyDown(Keys.X)
                   && prevKeyState != keyboardState)
                {
                    DRAW_MODE = false;
                    CurrentMode = GameMode.MoveMode;
                    form1.Hide();
                }

                if (keyboardState.IsKeyDown(Keys.C)
                   && prevKeyState != keyboardState)
                {
                    DRAW_MODE = true;
                    BigChangeList = new List<int>();
                    CurrentMode = GameMode.TerrainMode;
                    form1.Hide();
                }

                if (keyboardState.IsKeyDown(Keys.V)
                   && prevKeyState != keyboardState)
                {
                    DRAW_MODE = true;
                    BigChangeList = new List<int>();
                    CurrentMode = GameMode.ScriptMode;

                    form1.Show();
                    GAME_WINDOW.Activate();
                }

                #endregion

                
            }

            
            

            if (CurrentMode == GameMode.TerrainMode)
            {
                #region lights
                if (keyboardState.IsKeyDown(Keys.P))
                {
                    lightPower += 0.1f;
                }
                if (keyboardState.IsKeyDown(Keys.O))
                {
                    lightPower -= 0.1f;
                }

                if (keyboardState.IsKeyDown(Keys.L))
                {
                    ambientPower += 0.1f;
                }
                if (keyboardState.IsKeyDown(Keys.K))
                {
                    ambientPower -= 0.1f;
                }

                if (keyboardState.IsKeyDown(Keys.U)
                    && prevKeyState != keyboardState)
                {
                    lightDir.X += 1.0f;

                }
                if (keyboardState.IsKeyDown(Keys.I)
                    && prevKeyState != keyboardState)
                {
                    lightDir.X -= 1.0f;
                }
                if (keyboardState.IsKeyDown(Keys.J)
                    && prevKeyState != keyboardState)
                {
                    lightDir.Y += 1.0f;
                }
                if (keyboardState.IsKeyDown(Keys.H)
                    && prevKeyState != keyboardState)
                {
                    lightDir.Y -= 1.0f;
                }
                if (keyboardState.IsKeyDown(Keys.N)
                    && prevKeyState != keyboardState)
                {
                    lightDir.Z += 1.0f;
                }
                if (keyboardState.IsKeyDown(Keys.B)
                    && prevKeyState != keyboardState)
                {
                    lightDir.Z -= 1.0f;
                }

                #endregion lights

                #region render/decals
                if (keyboardState.IsKeyDown(Keys.NumPad1)
                    && prevKeyState != keyboardState)
                {
                    // TOON LIGHTING

                    toonLighting = toonLighting ? false : true;
                }




                if (keyboardState.IsKeyDown(Keys.NumPad2)
                    && prevKeyState != keyboardState)
                {
                    // DRAW TEXT

                    drawText = drawText ? false : true;
                }

                if (keyboardState.IsKeyDown(Keys.NumPad4)
                    && prevKeyState != keyboardState)
                {
                    // EDGE DETECT

                    edgeDetect = edgeDetect ? false : true;
                }

                if (keyboardState.IsKeyDown(Keys.NumPad9)
                    && prevKeyState != keyboardState)
                {
                    // blend factor

                    blendEnumS += 1;
                }
                if (keyboardState.IsKeyDown(Keys.NumPad6)
                    && prevKeyState != keyboardState)
                {
                    // blend factor

                    blendEnumS -= 1;
                }
                if (keyboardState.IsKeyDown(Keys.NumPad8)
                    && prevKeyState != keyboardState)
                {
                    // blend factor

                    blendEnumD += 1;
                }
                if (keyboardState.IsKeyDown(Keys.NumPad5)
                    && prevKeyState != keyboardState)
                {
                    // blend factor

                    blendEnumD -= 1;
                }

                #endregion render/decals

                #region edgedetect

                //float NormalThreshold = 0.5;
                //float DepthThreshold = 0.1;




                if (keyboardState.IsKeyDown(Keys.D2)
                    && keyboardState.IsKeyDown(Keys.LeftControl)
                        && prevKeyState != keyboardState)
                {
                    NormalThreshold += 0.05f;
                }
                if (keyboardState.IsKeyDown(Keys.D1)
                    && keyboardState.IsKeyDown(Keys.LeftControl)
                        && prevKeyState != keyboardState)
                {
                    NormalThreshold -= 0.05f;
                }

                if (keyboardState.IsKeyDown(Keys.D2)
                     && keyboardState.IsKeyDown(Keys.LeftShift)
                        && prevKeyState != keyboardState)
                {
                    DepthThreshold += 0.01f;
                }
                if (keyboardState.IsKeyDown(Keys.D1)
                     && keyboardState.IsKeyDown(Keys.LeftShift)
                        && prevKeyState != keyboardState)
                {
                    DepthThreshold -= 0.01f;
                }


                //float NormalSensitivity = 1;
                //float DepthSensitivity = 10;
                if (keyboardState.IsKeyDown(Keys.D4)
                    && keyboardState.IsKeyDown(Keys.LeftControl)
                        && prevKeyState != keyboardState)
                {
                    NormalSensitivity += 0.05f;
                }
                if (keyboardState.IsKeyDown(Keys.D3)
                    && keyboardState.IsKeyDown(Keys.LeftControl)
                        && prevKeyState != keyboardState)
                {
                    NormalSensitivity -= 0.05f;
                }

                if (keyboardState.IsKeyDown(Keys.D4)
                     && keyboardState.IsKeyDown(Keys.LeftShift)
                        && prevKeyState != keyboardState)
                {
                    DepthSensitivity += 1.0f;
                }
                if (keyboardState.IsKeyDown(Keys.D3)
                     && keyboardState.IsKeyDown(Keys.LeftShift)
                        && prevKeyState != keyboardState)
                {
                    DepthSensitivity -= 1.0f;
                }


                //float EdgeWidth = 1;
                //float EdgeIntensity = 1;

                if (keyboardState.IsKeyDown(Keys.D6)
                    && keyboardState.IsKeyDown(Keys.LeftControl)
                        && prevKeyState != keyboardState)
                {
                    EdgeWidth += 0.05f;
                }
                if (keyboardState.IsKeyDown(Keys.D5)
                    && keyboardState.IsKeyDown(Keys.LeftControl)
                        && prevKeyState != keyboardState)
                {
                    EdgeWidth -= 0.05f;
                }

                if (keyboardState.IsKeyDown(Keys.D6)
                     && keyboardState.IsKeyDown(Keys.LeftShift)
                        && prevKeyState != keyboardState)
                {
                    EdgeIntensity += 0.05f;
                }
                if (keyboardState.IsKeyDown(Keys.D5)
                     && keyboardState.IsKeyDown(Keys.LeftShift)
                        && prevKeyState != keyboardState)
                {
                    EdgeIntensity -= 0.05f;
                }
                #endregion

                #region tank_scale

                if (mouseState.ScrollWheelValue > prevMouseState.ScrollWheelValue)
                {
                    playerTank.scale += playerTank.scale * (0.1f);
                }
                if (mouseState.ScrollWheelValue < prevMouseState.ScrollWheelValue)
                {
                    playerTank.scale -= playerTank.scale * (0.1f);
                }
                #endregion tank_scale
            }

            if (CurrentMode == GameMode.TerrainMode)
            {
                #region drawmode


                //------- DRAW_MODE------------------------

                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    drawModeString = "Clicked.";
                    ClickInDrawMode();
                }

                if (mouseState.LeftButton == ButtonState.Released)
                {
                    if (BigChangeList.Count > 0)
                    {
                        ChangeVerticies(changeHeight);
                    }
                }

                if (keyboardState.IsKeyDown(Keys.OemMinus)
                    && prevKeyState != keyboardState)
                {
                    brushWidth -= 1;
                }
                if (keyboardState.IsKeyDown(Keys.OemPlus)
                    && prevKeyState != keyboardState)
                {
                    brushWidth += 1;
                }

                if (keyboardState.IsKeyDown(Keys.D0)
                    && prevKeyState != keyboardState)
                {
                    changeHeight += 0.25f;
                }
                if (keyboardState.IsKeyDown(Keys.D9)
                    && prevKeyState != keyboardState)
                {
                    changeHeight -= 0.25f;
                }

                if (keyboardState.IsKeyDown(Keys.D1)
                    && prevKeyState != keyboardState)
                {
                    mouseCamera.cameraPosition = CAM_POSITION1;
                    mouseCamera.leftrightRot = 0;
                    mouseCamera.updownRot = -MathHelper.PiOver2;
                }

                #endregion

            }

            if (CurrentMode == GameMode.MoveMode
                || CurrentMode == GameMode.ScriptMode)
            {
                #region   camspeed
                
                //------- CAM SPEED------------------------
                if (keyboardState.IsKeyDown(Keys.OemMinus)
                    && prevKeyState != keyboardState)
                {
                    mouseCamera.moveSpeed -= 10;
                }
                if (keyboardState.IsKeyDown(Keys.OemPlus)
                    && prevKeyState != keyboardState)
                {
                    mouseCamera.moveSpeed += 10;
                }

                #endregion
            }

            if (CurrentMode == GameMode.ScriptMode)
            {
                
                VertChangFrame += 1;
                if (VertChangFrame == 2)
                {
                    VertChangFrame = 0;
                    //MoveCursorScriptMode();
                    
                }

            }
                

            #region loading
            
            if (keyboardState.IsKeyDown(Keys.V)
                && keyboardState.IsKeyDown(Keys.LeftShift)
                && prevKeyState != keyboardState)
            {
                LoadLevel();                
            }

            if (keyboardState.IsKeyDown(Keys.V)
                && keyboardState.IsKeyDown(Keys.LeftControl)
                && prevKeyState != keyboardState)
            {
                //int absValue = (a < 0) ? -a : a;
                //if true ? this : else that;
                DrawLoaded = DrawLoaded ? (false) : (true);
            }

            #endregion

            prevMouseState = mouseState;
        }

        void UpdateFrameRate(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;           

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }


            
            elapsedTimeU += gameTime.ElapsedGameTime;
            
            if (elapsedTimeU > TimeSpan.FromSeconds(1))
            {
                elapsedTimeU = TimeSpan.MinValue;  //TimeSpan.FromSeconds(1);
                frameRateU = frameCounterU;
                frameCounterU = 0;
            }

            
        }

        void UpdateLightData()
        {
            //lightPos;// = new Vector3(4.0f, 6.0f, -6.0f);
            //lightPower = 1.5f;
            //ambientPower = 1.1f;

            lightViewProjection = CreateLightViewProjectionMatrix();
        }
       
        Matrix CreateLightViewProjectionMatrix()
        {
            // Matrix with that will rotate in points the direction of the light
            Matrix lightRotation = Matrix.CreateLookAt(Vector3.Zero,
                                                       -lightDir,
                                                       Vector3.Up);

            // Get the corners of the frustum
            //float FOV = MathHelper.Clamp(
            //            (25 * (mouseCamera.cameraPosition.Y / 5)),  // == 90 at height: 18
            //            10, 90 );

            
            float FOV = MathHelper.ToRadians(45);

            float FAR_PLANE = 20000;
           

            //float FAR_PLANE = (100 * (MathHelper.Clamp(
            //    (mouseCamera.cameraPosition.Y / 10),
            //    1f, 10f) ));
            //   // *      MathHelper.Clamp( mouseCamera.updownRot, 0.5f, 1f) );
            

            Matrix newProjMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians( FOV ),   //MathHelper.PiOver4,
               1.0f,
               1f, FAR_PLANE);

            
            BoundingFrustum newFrustum = new BoundingFrustum(Matrix.CreateLookAt
                (mouseCamera.cameraPosition,
                mouseCamera.cameraPosition + lightDir,
                Vector3.Up) *
                newProjMatrix);
           
            //cameraFrustum.Matrix = viewMatrix * parentGame.projectionMatrix;
            //Vector3[] frustumCorners = mouseCamera.cameraFrustum.GetCorners();
            Vector3[] frustumCorners = newFrustum.GetCorners();

            // Transform the positions of the corners into the direction of the light
            for (int i = 0; i < frustumCorners.Length; i++)
            {
                frustumCorners[i] = Vector3.Transform(frustumCorners[i], lightRotation);
            }

            // Find the smallest box around the points
            BoundingBox lightBox = BoundingBox.CreateFromPoints(frustumCorners);

            Vector3 boxSize = lightBox.Max - lightBox.Min ;
            Vector3 halfBoxSize = boxSize * 0.5f;

            // The position of the light should be in the center of the back
            // pannel of the box. 
            
            Vector3 lightPosition = lightBox.Min + halfBoxSize;
            lightPosition.Z = lightBox.Min.Z;

           

            // We need the position back in world coordinates so we transform 
            // the light position by the inverse of the lights rotation
            lightPosition = Vector3.Transform(lightPosition,
                                              Matrix.Invert(lightRotation));

           

            // Create the view matrix for the light
            Matrix lightView = Matrix.CreateLookAt(lightPosition,
                                                   lightPosition - lightDir,
                                                   Vector3.Up);

            // Create the projection matrix for the light
            // The projection is orthographic since we are using a directional light
            Matrix lightProjection = Matrix.CreateOrthographic(boxSize.X, boxSize.Y,
                                                               -boxSize.Z, boxSize.Z);

            return lightView * lightProjection;
        }


        

        protected override void Draw(GameTime gameTime)
        {
            frameCounter++;

            bool DRAW = false;
            if (LOADING)
            {
                if (load_frame_count > 5)
                {
                    load_frame_count = 0;
                    DRAW = true;
                }
            }
            else
                DRAW = true;

            if (DRAW)
            {
                device.Clear(Color.Black);

                if (drawText)
                    DrawLoadingText();

                #region Loaded
                if (DrawLoaded)
                {
                    

                    RenderToShadowMap();  //<- to rendT 1
                    // shadow map must be rendered or effect file must change
                    
                    RenderToEdgeDetect(); //<- to rendT 2
                    // optional
                    
                    RenderFromShadowMap(); // opt to RendT 1  OR  null
                    

                    // Run the postprocessing filter over the scene that we just rendered.
                    if (edgeDetect)
                        ApplyPostprocess();

                    SetParticleParameters();

                    //DrawShadowMapToScreen();

                    

                    if (drawText)
                        DrawText();

                }

                #endregion Loaded

                if (DRAW_MODE)
                    DrawCursor();

                if (LOADING)
                    DrawLoading();

                
                base.Draw(gameTime);
            }
        }

        

        void RenderToEdgeDetect()
        {
            Effect CurrentEffect = objEffect;
            String CurrentTechnique = "NormalDepth";

            //--------------EDGE DETECT---------------------------
            SetDraw3D();
            device.SetRenderTarget(0, renderTarget2);
            device.Clear(Color.Black);

            DrawTerrain(CurrentEffect, CurrentTechnique);
            DrawGameObjects(CurrentEffect, CurrentTechnique);
            //-----------------------------------------------------
        }

        void RenderFromShadowMap()
        {
            Effect CurrentEffect = objEffect;
            String CurrentTechnique = "ShadowedScene";

            //-------------DRAW FROM SHADOW MAP---------------------------
            if (edgeDetect)
                device.SetRenderTarget(0, renderTarget1);
            else
                device.SetRenderTarget(0, null);

            //device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
            device.Clear(Color.CadetBlue);

            DrawTerrain(CurrentEffect, CurrentTechnique);
            DrawGameObjects(CurrentEffect, CurrentTechnique);

           
            //-----------------------------------------------------

        }

        void RenderToShadowMap()
        {
            Effect CurrentEffect = objEffect;
            String CurrentTechnique = "ShadowMap";

            SetDraw3D();
            //-----------------------------------------------------
            device.SetRenderTarget(0, renderTarget1);
            DepthStencilBuffer oldDepthBuffer = GraphicsDevice.DepthStencilBuffer;
            GraphicsDevice.DepthStencilBuffer = shadowDepthBuffer;

            // device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
            device.Clear(Color.Black);


            DrawTerrain(CurrentEffect, CurrentTechnique);
            DrawGameObjects(CurrentEffect, CurrentTechnique);
            //-----------------------------------------------------


            //  BLUR
            DrawBlur(1.0f / (float)renderTarget2.Width, 0,
                GausBlur, renderTarget1, renderTarget2);
            DrawBlur(0, 1.0f / (float)renderTarget1.Height,
                GausBlur, renderTarget2, renderTarget1);

            device.SetRenderTarget(0, null);
            device.DepthStencilBuffer = oldDepthBuffer;
            shadowMap = renderTarget1.GetTexture();
        }

        void SetDraw3D()
        {

            GraphicsDevice.RenderState.DepthBufferEnable = true;
            GraphicsDevice.RenderState.AlphaBlendEnable = false;
            GraphicsDevice.RenderState.AlphaTestEnable = false;
        }


        void DrawLoadingText()
        {
            spriteBatch.Begin();

            spriteBatch.DrawString(font, "Left Shift + V   To Load\n",
                 new Vector2(420, 215), Color.Pink);

            spriteBatch.DrawString(font, "LOADING: " + LOADING + " : " + load_progress
                   + "\nLOADED: " + Loaded,
                 new Vector2(420, 250), Color.Pink);

            if (Loaded)
            {
                spriteBatch.DrawString(font, "Elapsed: " + timeEnd,
                 new Vector2(420, 300), Color.Pink);


                spriteBatch.DrawString(font, "Left Cntrl + V   To Draw\n",
                     new Vector2(420, 330), Color.Pink);
            }

            spriteBatch.End();
        }

        void DrawText_Script()
        {
            spriteBatch.DrawString(font,
                "(S):  Move Active Event" 
                + "\n"
                + "(F): Move Active Flag"
                + "\n"
                + "\n"
                + "(N): New Flag"
                 + "\n"
                 + "(K): Delete Active Flag"
                ,
               new Vector2(20, 150), Color.Wheat);
        }

        void DrawText_Terrain()
        {
            //-------- DRAW_MODE ---------------------------
            spriteBatch.DrawString(font,
                "(-/+) brush Width: " + brushWidth
                + "\n"
                + "(9/0) amnt_Chng: " + changeHeight
                + "\n" + "\n"
                //+ "(-n)SAVE to TEMP" 
                //+ "\n"
                // + "(+n)LOAD frm TEMP",
                ,
               new Vector2(20, 150), Color.Pink);

            spriteBatch.DrawString(font, "CAMS:"
                + "\n"
                + "(1) Overhead.All",
               new Vector2(20, 380), Color.Pink);

        }

        void DrawText_Move()
        {
            //--------FRAME RATE---------------------------
            string fps = string.Format("fps: {0}", frameRate);
            spriteBatch.DrawString(font, fps,
                new Vector2(20, 15), Color.White);

            fps = string.Format("fpsU: {0}", frameRateU);
            spriteBatch.DrawString(font, fps,
                new Vector2(120, 15), Color.White);


            //-------- TERRAIN ---------------------------
            spriteBatch.DrawString(font, "MinHeight: " + terrainEngine.minHeight
                + "\nMaxHeight: " + terrainEngine.maxHeight
                + "\nHeightMapWid: " + terrainEngine.heightmapWidth
                + "\nHeight: " + terrainEngine.heightmapWidth
                + "\nVertices: " + (terrainEngine.vertices.Length),
                new Vector2(320, 275), Color.White);

            #region TANK
            //--------TANK  POS-----------------------------
            spriteBatch.DrawString(font, "Tank1:\n" + "X: " + playerTank.position.X +
                         "\n" +
                         "Z: " + playerTank.position.Z +
                          "\n" +
                           "on HeightMap: " + terrainEngine.IsOnHeightmap(playerTank.position),
                new Vector2(20, 45), Color.LightSeaGreen);

            

            //--------Missile 0 POS-----------------------------
            //if (playerTank.missiles.Length > 0)
            //{

            //    spriteBatch.DrawString(font, "MISS1: ALIVE: " + playerTank.missiles[0].alive +
            //       "POS: X: " + playerTank.missiles[0].position.X +
            //        " Y: " + playerTank.missiles[0].position.Y +
            //        " Z: " + playerTank.missiles[0].position.Z,
            //        new Vector2(20, 430), Color.White);

            //    spriteBatch.DrawString(font, "VELOCITY: " + terrainEngine.IsOnHeightmap(playerTank.missiles[0].position) +
            //       " X: " + playerTank.missiles[0].prevVelocity.X +
            //        " Y: " + playerTank.missiles[0].prevVelocity.Y +
            //        " Z: " + playerTank.missiles[0].prevVelocity.Z,
            //        new Vector2(20, 450), Color.White);

            //}

            ////-------TANK SCALE---------------------------
            //spriteBatch.DrawString(font, "TANK SCALE: " + playerTank.scale,
            //            new Vector2(20, 460), Color.White);
            //    //-------TANK SCALE---------------------------

           
            #endregion TANK



            //------CAMERA WORLD COORDS-------------------------------
            spriteBatch.DrawString(font,
                "MouseCamPos:\n" +
                "X: " + mouseCamera.cameraPosition.X.ToString()
                + " Y: " + mouseCamera.cameraPosition.Y.ToString()
                + " Z: " + mouseCamera.cameraPosition.Z.ToString()
                + "\n" + "(-/+)MoveSpeed: " + mouseCamera.moveSpeed,
                new Vector2(220, 0), Color.White);


            #region LIGHTING
            spriteBatch.DrawString(font, "(P/0)LIGHT: " + lightPower +

            "\n" + "(L/K)AMBIN: " + ambientPower +
            "\n" + "(U/I)DIR X: " + lightDir.X +
            "\n" + "(J/H)DIR Y: " + lightDir.Y +
            "\n" + "(N/B)DIR Z: " + lightDir.Z,
               new Vector2(160, 250), Color.SandyBrown);



            //-------- LIGHTING ---------------------------

            spriteBatch.DrawString(font, "(4n)EDGE: " + edgeDetect,
                new Vector2(20, 340), Color.PowderBlue);
            spriteBatch.DrawString(font, "(1n)TOON: " + toonLighting,
                new Vector2(20, 360), Color.PowderBlue);
            //-------- blend.en: ---------------------------
            spriteBatch.DrawString(font, "(9n/6n)blend.S: " + (blendEnumS),
                new Vector2(20, 380), Color.PowderBlue);
            spriteBatch.DrawString(font, "(8n/5n)blend.D: " + (blendEnumD),
               new Vector2(20, 400), Color.PowderBlue);

            spriteBatch.DrawString(font, "DECALS: " + (terrainEngine.terrainDecals.Count),
               new Vector2(20, 255), Color.PowderBlue);


            spriteBatch.DrawString(font,
                "(Ctl 1/2)NormalThreshold: " + (NormalThreshold)
                + "\n"
                + "(Shf 1/2)DepthThreshold: " + DepthThreshold

                + "\n"
                + "(Ctl 3/4)NormalSensitivity: " + NormalSensitivity
                + "\n"
                + "(Shf 3/4)DepthSensitivity: " + DepthSensitivity

                + "\n"
                + "(Ctl 5/6)EdgeWidth: " + EdgeWidth
                + "\n"
                + "(Shf 5/6)EdgeIntensity: " + EdgeIntensity,
               new Vector2(420, 40), Color.PowderBlue);
            #endregion LIGHTING


               
        }

        void DrawText()
        {
            spriteBatch.Begin();

            spriteBatch.DrawString(font, "(2D)TEXT: " + drawText,
                new Vector2(10, -5), Color.Red);

            if (CurrentMode == GameMode.MoveMode)
            {
                DrawText_Move();

                spriteBatch.DrawString(font, "(z)PlayMode, (x)MOVE MODE, (c)TerrainMode, (v)ScriptMode " ,
                   new Vector2(20, 450), Color.Pink);
            }

            if (CurrentMode == GameMode.TerrainMode)
            {
                DrawText_Terrain();

                spriteBatch.DrawString(font, "(z)PlayMode, (x)MoveMode, (c)TERRAINMODE, (v)ScriptMode ",
                   new Vector2(20, 450), Color.Pink);
            }

            if (CurrentMode == GameMode.ScriptMode)
            {
                DrawText_Script();

                spriteBatch.DrawString(font, "(z)PlayMode, (x)MoveMode, (c)TerrainMode, (v)SCRIPTMODE ",
                   new Vector2(20, 450), Color.Pink);
            }

            if (CurrentMode == GameMode.PlayMode)
            {
                //DrawText_Play();

                spriteBatch.DrawString(font, "(z)PLAYMODE, (x)MoveMode, (c)TerrainMode, (v)ScriptMode ",
                   new Vector2(20, 450), Color.Pink);
            }
           
            
            spriteBatch.End();
        }


        public void SetEffectParameters(Effect effectToUse, String technique, Texture2D texture,
            Matrix worldMatrix, bool solidBrown, bool multiTexture)
        {
            Effect curEffect = effectToUse;
            curEffect.CurrentTechnique = curEffect.Techniques[technique];

            
            if (multiTexture)
            {
                curEffect.Parameters["xTexture0"].SetValue(terrainEngine.terrainTexture0);
                curEffect.Parameters["xTexture1"].SetValue(terrainEngine.terrainTexture1);
                curEffect.Parameters["xTexture2"].SetValue(terrainEngine.terrainTexture2);

                curEffect.Parameters["xDecalTex"].SetValue(terrainEngine.decalTexture);
                
            }
            curEffect.Parameters["xMultiTextured"].SetValue(multiTexture);


            curEffect.Parameters["xCamerasViewProjection"].SetValue(viewMatrix * projectionMatrix);

            curEffect.Parameters["xTexture"].SetValue(texture);
            curEffect.Parameters["xShadowMap"].SetValue(shadowMap);

            curEffect.Parameters["xSolidBrown"].SetValue(solidBrown);            
            curEffect.Parameters["xColor"].SetValue(currentColor.ToVector4());

            curEffect.Parameters["xWorld"].SetValue(worldMatrix);
            curEffect.Parameters["xLightPos"].SetValue(lightPos);
            curEffect.Parameters["xLightPower"].SetValue(lightPower);
            curEffect.Parameters["xAmbient"].SetValue(ambientPower);

            curEffect.Parameters["xLightsViewProjection"].SetValue(lightViewProjection);

            curEffect.Parameters["xToon"].SetValue( toonLighting );
            curEffect.Parameters["xSky"].SetValue(false);


        }


        void SetParticleParameters()
        {
            // Pass camera matrices through to the particle system components.
            explosionParticles.SetCamera(viewMatrix, projectionMatrix);
            explosionSmokeParticles.SetCamera(viewMatrix, projectionMatrix);
            projectileTrailParticles.SetCamera(viewMatrix, projectionMatrix);
            smokePlumeParticles.SetCamera(viewMatrix, projectionMatrix);
            fireParticles.SetCamera(viewMatrix, projectionMatrix);
        }
        
        void DrawTerrain(Effect CurrentEffect, String CurrentTechnique)
        {
            SetEffectParameters(CurrentEffect, CurrentTechnique,
                null, terrainEngine.worldMatrix, false, false);

            terrainEngine.DrawSkyDome(viewMatrix, mouseCamera.cameraPosition, projectionMatrix, CurrentTechnique);



            SetEffectParameters(CurrentEffect, CurrentTechnique,
                terrainEngine.terrainTexture, terrainEngine.worldMatrix, false, true);
            CurrentEffect.Parameters["xDecal"].SetValue(false);

            terrainEngine.DrawTerrain(CurrentEffect);

            if (CurrentTechnique == "ShadowedScene")
            {

                CurrentEffect.Parameters["xDecal"].SetValue(true);

                GraphicsDevice.RenderState.AlphaTestEnable = true;
                GraphicsDevice.RenderState.AlphaBlendEnable = true;
                GraphicsDevice.RenderState.BlendFunction = BlendFunction.Add;
                
                

                terrainEngine.DrawDecals(CurrentEffect);



                GraphicsDevice.RenderState.AlphaBlendEnable = false;
                GraphicsDevice.RenderState.AlphaTestEnable = false;
                CurrentEffect.Parameters["xDecal"].SetValue(false);
            }

        }

        

        void DrawGameObjects(Effect effectToUse, String technique)
        {
            GameObject obj = playerTank;
            //DrawModel(effectToUse, obj.model, obj.textures, fallingBox.GetWorldMatrix(), technique, true);
            //DrawModel(effectToUse, obj.model, obj.textures, immovableBox.GetWorldMatrix(), technique, true);

            if (fallingBox != null)
            {
                VertexPositionColor[] wrFrm = fallingBox.Skin.GetLocalSkinWireframe();
                fallingBox.Body.TransformWireframe(wrFrm);
                debugDrawer.DrawShape(wrFrm);

                wrFrm = playerTank.mobileObject.Car.Chassis.Skin.GetLocalSkinWireframe();
                playerTank.mobileObject.Car.Chassis.Body.TransformWireframe(wrFrm);
                debugDrawer.DrawShape(wrFrm);

                

                //DrawModel(effectToUse, obj.model, obj.textures, carObject.GetWorldMatrixScale(obj.scale), technique, false);
                
            }
            else
            {
                //obj.worldMatrix
                DrawModel(effectToUse, obj.model, obj.textures, obj.worldMatrix, technique, false);
            }
            

           
            obj = playerTank2;
            DrawModel(effectToUse, obj.model, obj.textures, obj.worldMatrix, technique, false);


            for (int i = 0; i < numTanks; ++i)
            {                
                obj = playerTanks[i];
                DrawModel(effectToUse, obj.model, obj.textures, obj.worldMatrix, technique, false);

                foreach (Missile missile in playerTanks[i].missiles)
                {
                    if (missile.alive)
                    {
                        obj = missile;
                        DrawModel(effectToUse, obj.model, obj.textures, obj.worldMatrix, technique, false);
                    }
                }
            }

            foreach (Missile missile in playerTank.missiles)
            {
                if (missile.alive)
                {
                    obj = missile;
                    DrawModel(effectToUse, obj.model, obj.textures, obj.worldMatrix, technique, false);
                }
            }

            foreach (Missile missile in playerTank2.missiles)
            {
                if (missile.alive)
                {
                    obj = missile;
                    DrawModel(effectToUse, obj.model, obj.textures, obj.worldMatrix, technique, false);
                }
            }

            if (technique == "ShadowedScene")
                Editor.Draw();

        }

        public void DrawModel(Effect effectToUse, Model model, Texture2D[] textures, 
            Matrix wMatrix, string technique, bool solidBrown)
        {
            Matrix[] modelTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(modelTransforms);
            int i = 0;
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (Effect currentEffect in mesh.Effects)
                {
                    Matrix world = modelTransforms[mesh.ParentBone.Index] * wMatrix;

                    SetEffectParameters(currentEffect, technique, textures[i++], world, solidBrown, false);                    
                    
                }
                mesh.Draw();
            }
        }

        void DrawBlur(float x, float y, Effect effectToUse,
            RenderTarget2D source, RenderTarget2D target)
        {


            SetBlurEffectParameters(x, y, effectToUse);

            GraphicsDevice.SetRenderTarget(0, target);

            spriteBatch.Begin(SpriteBlendMode.None,
                              SpriteSortMode.Immediate,
                              SaveStateMode.None);

            effectToUse.Begin();
            effectToUse.CurrentTechnique.Passes[0].Begin();

            spriteBatch.Draw(source.GetTexture(),
                new Rectangle(0, 0,
                    target.Width,
                    target.Height), Color.White);
            spriteBatch.End();

            effectToUse.CurrentTechnique.Passes[0].End();
            effectToUse.End();

        }

        void SetBlurEffectParameters(float dx, float dy, Effect effectToUse)
        {
            // Look up the sample weight and offset effect parameters.
            EffectParameter weightsParameter, offsetsParameter;

            weightsParameter = effectToUse.Parameters["SampleWeights"];
            offsetsParameter = effectToUse.Parameters["SampleOffsets"];

            // Look up how many samples our gaussian blur effect supports.
            int sampleCount = weightsParameter.Elements.Count;

            // Create temporary arrays for computing our filter settings.
            float[] sampleWeights = new float[sampleCount];
            Vector2[] sampleOffsets = new Vector2[sampleCount];

            // The first sample always has a zero offset.
            sampleWeights[0] = ComputeGaussian(0);
            sampleOffsets[0] = new Vector2(0);

            // Maintain a sum of all the weighting values.
            float totalWeights = sampleWeights[0];

            // Add pairs of additional sample taps, positioned
            // along a line in both directions from the center.
            for (int i = 0; i < sampleCount / 2; i++)
            {
                // Store weights for the positive and negative taps.
                float weight = ComputeGaussian(i + 1);

                sampleWeights[i * 2 + 1] = weight;
                sampleWeights[i * 2 + 2] = weight;

                totalWeights += weight * 2;

                // To get the maximum amount of blurring from a limited number of
                // pixel shader samples, we take advantage of the bilinear filtering
                // hardware inside the texture fetch unit. If we position our texture
                // coordinates exactly halfway between two texels, the filtering unit
                // will average them for us, giving two samples for the price of one.
                // This allows us to step in units of two texels per sample, rather
                // than just one at a time. The 1.5 offset kicks things off by
                // positioning us nicely in between two texels.
                float sampleOffset = i * 2 + 1.5f;

                Vector2 delta = new Vector2(dx, dy) * sampleOffset;

                // Store texture coordinate offsets for the positive and negative taps.
                sampleOffsets[i * 2 + 1] = delta;
                sampleOffsets[i * 2 + 2] = -delta;
            }

            // Normalize the list of sample weightings, so they will always sum to one.
            for (int i = 0; i < sampleWeights.Length; i++)
            {
                sampleWeights[i] /= totalWeights;
            }

            // Tell the effect about our new filter settings.
            weightsParameter.SetValue(sampleWeights);
            offsetsParameter.SetValue(sampleOffsets);
        }


        float ComputeGaussian(float n)
        {
            float theta = 1; //1; 2;  3;  4; //Settings.BlurAmount;

            return (float)((1.0 / Math.Sqrt(2 * Math.PI * theta)) *
                           Math.Exp(-(n * n) / (2 * theta * theta)));
        }

        void DrawShadowMapToScreen()
        {
            spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate,
                              SaveStateMode.SaveState);
            spriteBatch.Draw(shadowMap, new Rectangle(650, 0, 128, 128), Color.White);
            spriteBatch.End();
        }


       
        void ApplyPostprocess()
        {
            device.SetRenderTarget(0, null);

            EffectParameterCollection parameters = objEffect.Parameters;
            string effectTechniqueName;
            effectTechniqueName = "EdgeDetect";
            
            if ( edgeDetect )//Settings.EnableEdgeDetect)
            {
                Vector2 resolution = new Vector2(shadowMapW,
                                                shadowMapH);

                Texture2D normalDepthTexture = renderTarget2.GetTexture();

                parameters["EdgeWidth"].SetValue(EdgeWidth); //Settings.EdgeWidth);
                parameters["EdgeIntensity"].SetValue(EdgeIntensity); //Settings.EdgeIntensity);


                parameters["NormalThreshold"].SetValue(NormalThreshold);
                parameters["DepthThreshold"].SetValue(DepthThreshold);

                parameters["NormalSensitivity"].SetValue(NormalSensitivity);
                parameters["DepthSensitivity"].SetValue(DepthSensitivity); 

                parameters["ScreenResolution"].SetValue(resolution);
                parameters["NormalDepthTexture"].SetValue(normalDepthTexture);
                
                effectTechniqueName = "EdgeDetect";
            }
            else
            {
                //// If edge detection is off, just pick one of the sketch techniques.
                //if (Settings.SketchInColor)
                //    effectTechniqueName = "ColorSketch";
                //else
                //    effectTechniqueName = "MonoSketch";
            }

            // Activate the appropriate effect technique.

            objEffect.CurrentTechnique =
                                    objEffect.Techniques[effectTechniqueName];

            // Draw a fullscreen sprite to apply the postprocessing effect.
            spriteBatch.Begin(SpriteBlendMode.None,
                              SpriteSortMode.Immediate,
                              SaveStateMode.None);

            objEffect.Begin();
            objEffect.CurrentTechnique.Passes[0].Begin();

            spriteBatch.Draw(renderTarget1.GetTexture(), Vector2.Zero, Color.White);

            spriteBatch.End();

            objEffect.CurrentTechnique.Passes[0].End();
            objEffect.End();
        }




        void ClickInDrawMode()
        {
            Ray mouseRay = CalculateCursorRay(projectionMatrix, viewMatrix);

            posList = new List<Vector3>();
            vectorList = new List<int>();

            Vector3 Pos = GetTerrainVector(mouseRay);

            drawModeString = "X: " + Pos.X +
                "\n " +
                "Z: " + Pos.Z;

            GetBrushVerticies(Pos, brushWidth);

            AddToChangeList();
           
        }

        void MoveCursorScriptMode()
        {
            
                

            Ray mouseRay = CalculateCursorRay(projectionMatrix, viewMatrix);

            posList = new List<Vector3>();
            vectorList = new List<int>();

            float CHANGE_AMOUNT = 2.0f;
            int WIDTH = 4;

            if (BigChangeList.Count > 0)
                ChangeVerticiesTemp(-CHANGE_AMOUNT);

            BigChangeList = new List<int>();

            Vector3 Pos = GetTerrainVector(mouseRay);

            drawModeString = "X: " + Pos.X +
                "\n " +
                "Z: " + Pos.Z;

            GetBrushVerticies(Pos, WIDTH);

            AddToChangeList();

            ChangeVerticiesTemp(CHANGE_AMOUNT);

        }

        void ChangeVerticiesTemp(float HeightChange)
        {
            foreach (int index in BigChangeList)
            {
                terrainEngine.vertices[index].Position.Y += HeightChange;
                //terrainEngine.ReCalcWeights(index);
            }
        }

        Ray CalculateCursorRay(Matrix projectionMatrix, Matrix viewMatrix)
        {
            // create 2 positions in screenspace using the cursor position. 0 is as
            // close as possible to the camera, 1 is as far away as possible.
            Vector2 Position = new Vector2(mouseState.X, mouseState.Y);

            Vector3 nearSource = new Vector3(Position, 0f);
            Vector3 farSource = new Vector3(Position, 1f);

            // use Viewport.Unproject to tell what those two screen space positions
            // would be in world space. we'll need the projection matrix and view
            // matrix, which we have saved as member variables. We also need a world
            // matrix, which can just be identity.
            Vector3 nearPoint = GraphicsDevice.Viewport.Unproject(nearSource,
                projectionMatrix, viewMatrix, Matrix.Identity);

            Vector3 farPoint = GraphicsDevice.Viewport.Unproject(farSource,
                projectionMatrix, viewMatrix, Matrix.Identity);

            // find the direction vector that goes from the nearPoint to the farPoint
            // and normalize it....
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            // and then create a new ray using nearPoint as the source.
            return new Ray(nearPoint, direction);
        }

        Vector3 GetTerrainVector(Microsoft.Xna.Framework.Ray mRay)
        {
            
            Vector3 pos = new Vector3(0,0,0);
            Nullable<float> curDist = 10000f;

            for (int i = 0; i < terrainEngine.vertices.Length - 3; i += 3)
            {
                Nullable<float> rayDist;
                Vector3 firstPos;

                firstPos = terrainEngine.vertices[i].Position;
               
                BoundingSphere sphere = new BoundingSphere(firstPos, 2);
                rayDist = mRay.Intersects(sphere);
                if (rayDist != null)
                {
                    if (rayDist < curDist)
                    {
                        curDist = rayDist;
                        pos = sphere.Center;
                        posList.Add(pos);
                    }
                    
                }
            }
           
            return pos;

        }

        void GetBrushVerticies(Vector3 brushPos, int brushWidth)
        {
            BoundingSphere BRUSH = new BoundingSphere(brushPos, brushWidth * terrainEngine.terrainScale);

            if (brushPos != new Vector3(0, 0, 0))
            {
                for (int i = 0; i < terrainEngine.vertices.Length; ++i)
                {

                    Vector3 firstPos;

                    firstPos = terrainEngine.vertices[i].Position;

                    BoundingSphere sphere = new BoundingSphere(firstPos, terrainEngine.terrainScale);

                    if (BRUSH.Intersects(sphere))
                    {
                        vectorList.Add(i);
                    }

                }
            }
        }

        void AddToChangeList()
        {
            
            bool inside = false;

            foreach (int index in vectorList)
            {
                inside = false;
                foreach (int bigIndex in BigChangeList)
                {
                    if (index == bigIndex)
                    {
                        inside = true;
                        break;
                    }
                }

                if (!inside)
                {
                    BigChangeList.Add(index);
                }
            }

        }

        void ChangeVerticies(float HeightChange)
        {
            foreach (int index in BigChangeList)
            {
                terrainEngine.vertices[index].Position.Y += HeightChange;
                terrainEngine.ReCalcWeights(index);
            }

            terrainEngine.CalculateNormals();

            terrainEngine.SaveHeightMap();

            BigChangeList = new List<int>();
        }

        void DrawCursor()
        {
            spriteBatch.Begin();

            // use textureCenter as the origin of the sprite, so that the cursor is 
            // drawn centered around Position.
            spriteBatch.Draw(cursorTexture,
                new Vector2(mouseState.X, mouseState.Y),
                null, Color.White, 0.0f,
                textureCenter,
                1.0f, SpriteEffects.None, 0.0f);

            spriteBatch.End();
        }

        void DrawLoading()
        {
            spriteBatch.Begin();

            // use textureCenter as the origin of the sprite, so that the cursor is 
            // drawn centered around Position.
            
            spriteBatch.Draw(LoadingAnimation[LoadingAnimFrame++],
                new Vector2(250, 250),
                null, Color.White, 0.0f,
                new Vector2(
                LoadingAnimation[0].Width / 2, LoadingAnimation[0].Height / 2),
                0.3f, SpriteEffects.None, 0.0f);

            spriteBatch.End();

            if (LoadingAnimFrame > 15)
                LoadingAnimFrame = 0;
        }

        

        public void PutLevelInXML()
        {
            // Create a new file in C:\\ dir
            XmlTextWriter textWriter = new XmlTextWriter("C:\\myXmFile.xml", null);

            // Opens the document
            textWriter.WriteStartDocument();           

            // Write first element
            textWriter.WriteWhitespace("\n");
            textWriter.WriteStartElement("level");
            textWriter.WriteWhitespace("\n");
            
            #region Terrain
            // Write next element
            textWriter.WriteWhitespace("\t");
            textWriter.WriteStartElement("map", "");
            textWriter.WriteString(myLevel.terrainMap);
            textWriter.WriteEndElement();
            textWriter.WriteWhitespace("\n");

            textWriter.WriteWhitespace("\t");
            textWriter.WriteStartElement("texture1", "");
            textWriter.WriteString(myLevel.texture1);
            textWriter.WriteEndElement();
            textWriter.WriteWhitespace("\n");

            textWriter.WriteWhitespace("\t");
            textWriter.WriteStartElement("texture2", "");
            textWriter.WriteString(myLevel.texture2);
            textWriter.WriteEndElement();
            textWriter.WriteWhitespace("\n");

            textWriter.WriteWhitespace("\t");
            textWriter.WriteStartElement("texture3", "");
            textWriter.WriteString(myLevel.texture3);
            textWriter.WriteEndElement();
            textWriter.WriteWhitespace("\n");

            textWriter.WriteWhitespace("\t");
            textWriter.WriteStartElement("texture4", "");
            textWriter.WriteString(myLevel.texture4);
            textWriter.WriteEndElement();
            textWriter.WriteWhitespace("\n");

            textWriter.WriteWhitespace("\t");
            textWriter.WriteStartElement("decal_texture", "");
            textWriter.WriteString(myLevel.decalTexture);
            textWriter.WriteEndElement();
            textWriter.WriteWhitespace("\n");

            textWriter.WriteWhitespace("\t");
            textWriter.WriteStartElement("terrain_scale", "");
            textWriter.WriteString(myLevel.terrainScale.ToString());
            textWriter.WriteEndElement();
            textWriter.WriteWhitespace("\n");

#endregion

            textWriter.WriteWhitespace("\n");

            #region Draw Style

            textWriter.WriteWhitespace("\t");
            textWriter.WriteStartElement("NormalThreshold", "");
            textWriter.WriteString(NormalThreshold.ToString());
            textWriter.WriteEndElement();
            textWriter.WriteWhitespace("\n");

            textWriter.WriteWhitespace("\t");
            textWriter.WriteStartElement("DepthThreshold", "");
            textWriter.WriteString(DepthThreshold.ToString());
            textWriter.WriteEndElement();
            textWriter.WriteWhitespace("\n");

            textWriter.WriteWhitespace("\n");

            textWriter.WriteWhitespace("\t");
            textWriter.WriteStartElement("NormalSensitivity", "");
            textWriter.WriteString(NormalSensitivity.ToString());
            textWriter.WriteEndElement();
            textWriter.WriteWhitespace("\n");

            textWriter.WriteWhitespace("\t");
            textWriter.WriteStartElement("DepthSensitivity", "");
            textWriter.WriteString(DepthSensitivity.ToString());
            textWriter.WriteEndElement();
            textWriter.WriteWhitespace("\n");

            textWriter.WriteWhitespace("\n");

            textWriter.WriteWhitespace("\t");
            textWriter.WriteStartElement("EdgeWidth", "");
            textWriter.WriteString(EdgeWidth.ToString());
            textWriter.WriteEndElement();
            textWriter.WriteWhitespace("\n");

            textWriter.WriteWhitespace("\t");
            textWriter.WriteStartElement("EdgeIntensity", "");
            textWriter.WriteString(EdgeIntensity.ToString());
            textWriter.WriteEndElement();
            textWriter.WriteWhitespace("\n");

            #endregion

            textWriter.WriteWhitespace("\n");

            textWriter.WriteWhitespace("\t");
            textWriter.WriteStartElement("player_spawn", "");
            textWriter.WriteString(
                myLevel.playerSpawn.Position.X.ToString()
                + "," +
                myLevel.playerSpawn.Position.Y.ToString()
                + "," +
                myLevel.playerSpawn.Position.Z.ToString()
                );
            textWriter.WriteEndElement();
            textWriter.WriteWhitespace("\n");

            textWriter.WriteWhitespace("\t");
            textWriter.WriteStartElement("camera_spawn", "");
            textWriter.WriteString(
                myLevel.cameraSpawn.Position.X.ToString()
                + "," +
                myLevel.cameraSpawn.Position.Y.ToString()
                + "," +
                myLevel.cameraSpawn.Position.Z.ToString()
                );
            textWriter.WriteEndElement();
            textWriter.WriteWhitespace("\n");


            foreach (EventFlag flag in myLevel.FlagsList)
            {
                textWriter.WriteWhitespace("\n");
                textWriter.WriteWhitespace("\t");
                textWriter.WriteStartElement("flag", "");
                

                foreach (Vector2 corner in flag.Corners)
                {
                    textWriter.WriteString(
                        corner.X.ToString() + "," +
                        corner.Y.ToString() + ";");
                }

                textWriter.WriteEndElement();

                foreach (GameEvent g_event in flag.Events)
                {
                    textWriter.WriteWhitespace("\n");
                    textWriter.WriteWhitespace("\t\t");
                    textWriter.WriteStartElement("event", "");
                    textWriter.WriteString(g_event.EventType.ToString().ToLower());
                    textWriter.WriteEndElement();

                    

                    if (g_event.EventType == GameEvent.GameEventType.PUT_ENEMY)
                    {
                        EnemySpawn thisSpawn = (EnemySpawn)g_event.SpawnPoint;

                        textWriter.WriteWhitespace("\n");
                        textWriter.WriteWhitespace("\t\t\t");
                        textWriter.WriteStartElement("spawn_point", "");
                        textWriter.WriteString(
                            thisSpawn.Position.X.ToString()
                            + "," +
                            thisSpawn.Position.Y.ToString()
                            + "," +
                            thisSpawn.Position.Z.ToString()
                            );
                        textWriter.WriteEndElement();

                        textWriter.WriteWhitespace("\n");
                        textWriter.WriteWhitespace("\t\t\t");
                        textWriter.WriteStartElement("enemy_model", "");
                        textWriter.WriteString(thisSpawn.EnemyStats["Model"]);
                        textWriter.WriteEndElement();

                        textWriter.WriteWhitespace("\n");
                        textWriter.WriteWhitespace("\t\t\t");
                        textWriter.WriteStartElement("scale", "");
                        textWriter.WriteString(thisSpawn.EnemyStats["Scale"]);
                        textWriter.WriteEndElement();
                    }


                    textWriter.WriteWhitespace("\n");

                }
            }

            textWriter.WriteWhitespace("\n");

            // Ends the document.
            textWriter.WriteEndDocument();

            // close writer
            textWriter.Close();
        }

        void GetLevelFromXML()
        {
            //TerrainTutorial\bin\x86\Debug
            XmlTextReader textReader = new XmlTextReader("..\\..\\..\\Content\\Level.xml");           

            string file = "Level.xml";

            myLevel.terrainMap = GetTextAt("map", file);
            myLevel.texture1 = GetTextAt("texture1", file);
            myLevel.texture2 = GetTextAt("texture2", file);
            myLevel.texture3 = GetTextAt("texture3", file);
            myLevel.texture4 = GetTextAt("texture4", file);
            myLevel.decalTexture = GetTextAt("decal_texture", file);

            myLevel.terrainScale = Convert.ToSingle(GetTextAt("terrain_scale", file));            
           
            myLevel.playerSpawn.Position = GetVector3At("player_spawn", file);
            myLevel.cameraSpawn.Position = GetVector3At("camera_spawn", file);

            NormalThreshold = Convert.ToSingle(GetTextAt("NormalThreshold", file));
            DepthThreshold = Convert.ToSingle(GetTextAt("DepthThreshold", file));
            NormalSensitivity = Convert.ToSingle(GetTextAt("NormalSensitivity", file));
            DepthSensitivity = Convert.ToSingle(GetTextAt("DepthSensitivity", file));
            EdgeWidth = Convert.ToSingle(GetTextAt("EdgeWidth", file));
            EdgeIntensity = Convert.ToSingle(GetTextAt("EdgeIntensity", file));


            myLevel.FlagsList = GetFlags(file);

            Console.WriteLine("Flags: " + myLevel.FlagsList.Count);

            foreach (EventFlag flag in myLevel.FlagsList)
            {
                foreach (Vector2 corner in flag.Corners)
                    Console.WriteLine("Corner: " + corner);

                Console.WriteLine("==========");

                foreach (GameEvent evnt in flag.Events)
                    Console.WriteLine("EventType: " + evnt.EventType);
            }


        }

        void GetTextForFlags(String file, out ArrayList elemType, out ArrayList elemText)
        {
            XmlTextReader textReader = new XmlTextReader("..\\..\\..\\Content\\" + file);
            String Value = "";

            elemType = new ArrayList();
            elemText = new ArrayList();

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
                }
            }
        }

        List<EventFlag> GetFlags(String file)
        {
            List<EventFlag> FlagList = new List<EventFlag>();

            ArrayList elemType = new ArrayList();
            ArrayList elemText = new ArrayList();

            GetTextForFlags(file, out elemType, out elemText);

            EventFlag flag = null;
            GameEvent g_event = null;

            for (int i = 0; i < elemType.Count; ++i)
            {                
                if((string)elemType[i] == "flag")
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

                        spawn.Position = GetVector3At((string)elemText[i + 1]);
                        
                        string model = (string)elemText[i + 2];
                        string scale = (string)elemText[i + 3];

                        spawn.EnemyStats.Add("Model", model);
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
            Value.Y = Convert.ToSingle(tArray[1]);
            Value.Z = Convert.ToSingle(tArray[2]);
            //Value.Z = Convert.ToDouble(tArray[2]);

            return Value;

        }

        String GetTextAt(String element, String file)
        {

            XmlTextReader textReader = new XmlTextReader("..\\..\\..\\Content\\" + file);
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

        void InstanceNewLevel()
        {
            

            //myLevel.FlagsList.Event = new GameEvent();
            //myLevel.FlagsList.Event.EventType = GameEvent.GameEventType.ENEMY_SPAWN;

            //myLevel.FlagsList.Event.SpawnPoint.Position = new Vector3(2, 4, -5);

            //String EnemyModel = "Model\\car";
            //myLevel.FlagsList.Event.EnemyStats.Add("Model", EnemyModel);
            //myLevel.FlagsList.Event.EnemyStats.Add("Scale", "0.02");
            
            
        }

        void GameEventLogic(GameEvent.GameEventType EventType)
        {
            switch (EventType)
            {
                case GameEvent.GameEventType.PUT_PLAYER:
                    SpawnPlayer();
                    break;
                case GameEvent.GameEventType.PUT_ENEMY:
                    //SpawnEnemy();
                    break;
            }         
        }

        void SpawnPlayer()
        {
        }

        void SpawnEnemy(Spawn SpawnPoint, Dictionary<String, String> EnemyStats)
        {
            EnemyTank tank = new EnemyTank(this, 
                LoadedResources.ModelList[EnemyStats["Model"]].Model_rez,
                LoadedResources.ModelList[EnemyStats["Model"]].Textures_rez);
            tank.position = SpawnPoint.Position;
            float h = 0.0f;
            Vector3 n;
            terrainEngine.GetHeight(SpawnPoint.Position, out h, out n);
            if (SpawnPoint.Position.Y < h)
                tank.position = new Vector3 (tank.position.X, h ,tank.position.Z);

            tank.FacingDirection = SpawnPoint.leftRightRotation;

            enemyTanks.Add(tank);
        }
    }
}
