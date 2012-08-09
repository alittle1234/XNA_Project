using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using JigLibX.Collision;
using JigLibX.Physics;
using JigLibX.Objects;


using GameDebugTools;


namespace TerrainGame4_0
{
    public class DrawingClass
    {
        ParentGame parentGame;
        GameplayScreen LV;

        #region     OPTIONS
        bool edgeDetect = true;
        bool toonLighting = true;
        #endregion

        #region     RENDER TARGETS
        RenderTarget2D          renderTarget1;
        RenderTarget2D          renderTarget2;
        RenderTarget2D depthRT;
        //DepthStencilBuffer depthSB;
        Texture2D depth1Texture;
        Texture2D blurredScene;
        Texture2D depthTexture;
        Texture2D sceneTexture;
        //DepthStencilBuffer      shadowDepthBuffer;
        RenderTarget2D shadowMap, shadowMapAlt;
        #endregion

        #region     GRAPHICS        
        public SpriteBatch      spriteBatch;
        public GraphicsDevice   device;        
        int                     windowWidth = 800;
        int                     windowHeight = 500;
        public Effect           objEffect;
        Effect                  GausBlur;
        Effect                  effectPostDoF;
        Effect                  shadowMapShader;
        Effect                  depthMapShader;
        public SpriteFont              font;
        Texture2D Lines, Post, Bullets, PowerBar, PowerCase, Angle, WinBack, LooseBack;
        Texture2D FireworkSheet, Timer;
        public SpriteFont TitlesFont;
        public SpriteFont TitlesFont2;
        Matrix SpriteScale;
        #endregion

        #region     MATRICES
        public Matrix           worldMatrix;
        public Matrix           projectionMatrix;
        public Matrix           viewMatrix;
        BoundingFrustum boundFrustum;
        #endregion 

        #region     LIGHTING
        public Vector3          lightDir = new Vector3(30.0f, 87.0f, 0.0f);
        Vector3                 lightPos = new Vector3(4.0f, 6.0f, -6.0f);
        public float            lightPower;       
        public float            ambientPower;        
        Matrix                  lightViewProjection;        

        Vector4                 SpecularColor;
        float                   SpecularIntensity;
        float                   Shinniness;
        Vector3                 CameraPosition;
        #endregion

        #region     PARTICLES
        public ParticleSystem explosionParticles;
        public ParticleSystem explosionSmokeParticles;
        public ParticleSystem shotSmokeParticles;
        public ParticleSystem shotExplosionParticles;
        public ParticleSystem ringExplosionParticles;
        public ParticleSystem projectileTrailParticles;
        public ParticleSystem smokePlumeParticles;
        public ParticleSystem fireParticles;
        public ParticleSystem concreteParticles;
        public ParticleSystem sparkParticles;
        public ParticleSystem buildingColumn;
        public ParticleSystem buildingDisk;
        public List<ParticleSystem> Particles;

        #endregion particles_variables

        #region FRAM RATE
        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;
        TimeSpan elapsedTimeFrame = TimeSpan.Zero;
        #endregion FRAM RATE

        List<Matrix> BONES = new List<Matrix>(71);

        List<Missile> BulletList;
        List<Building> BuildingList;
        List<BuildingPiece> PieceList;        

        DebugDrawer debugDrawer;

        float ssX = 0;
        Vector2 WLScale = Vector2.Zero;

        public DrawingClass(ParentGame parentGame, GameplayScreen levelVariables,
            List<Missile> BulletList, List<Building> BuildingList, List<BuildingPiece> PieceList)
        {
            this.parentGame = parentGame;
            this.LV = levelVariables;
            this.BulletList = BulletList;
            this.BuildingList = BuildingList;
            this.PieceList = PieceList;
            

            device = parentGame.GraphicsDevice;
            PresentationParameters PP = device.PresentationParameters;
            spriteBatch = parentGame.spriteBatch;           
            
            windowHeight = PP.BackBufferHeight;
            windowWidth = PP.BackBufferWidth;
            
            objEffect = parentGame.objEffect;
            //shadowMapShader = parentGame.Content.Load<Effect>("Effect\\DrawShadowMap");
            GausBlur = parentGame.Content.Load<Effect>("Effect\\GaussianBlur");
            effectPostDoF = parentGame.Content.Load<Effect>("Effect\\PostProcessDoF");
            depthMapShader = parentGame.Content.Load<Effect>("Effect\\Shader");
            font = parentGame.Content.Load<SpriteFont>("Font\\SpriteFont1");
            Lines = parentGame.Content.Load<Texture2D>("Radar\\lines3");
            Post = parentGame.Content.Load<Texture2D>("Radar\\post_screen");
            PowerBar = parentGame.Content.Load<Texture2D>("Radar\\power_bar");
            PowerCase = parentGame.Content.Load<Texture2D>("Radar\\power_case");
            Bullets = parentGame.Content.Load<Texture2D>("Radar\\bullets");
            Angle = parentGame.Content.Load<Texture2D>("Radar\\angle");
            WinBack = parentGame.Content.Load<Texture2D>("WinLoose\\win_back");
            LooseBack = parentGame.Content.Load<Texture2D>("WinLoose\\loose_back");
            TitlesFont = parentGame.Content.Load<SpriteFont>("Font\\sten36bld");
            TitlesFont2 = parentGame.Content.Load<SpriteFont>("Font\\Titles_font2");
            FireworkSheet = parentGame.Content.Load<Texture2D>("Firework\\firework_sheet");
            Timer = parentGame.Content.Load<Texture2D>("Radar\\timer");

            blurredScene = new Texture2D(device, windowWidth, windowHeight);
            depthTexture = new Texture2D(device, windowWidth, windowHeight);
            sceneTexture = new Texture2D(device, windowWidth, windowHeight);

            InitializeRenderTargets();

            InitializeParticles();

            lightPower = 0.9f;
            ambientPower = 0.2f;

            debugDrawer = new DebugDrawer(parentGame);
            debugDrawer.Enabled = true;
            parentGame.Components.Add(debugDrawer);

            SpecularColor = Color.Purple.ToVector4();
            SpecularIntensity = 1.0f;
            Shinniness        = 500.0f;

            float screenscale =
                (float)device.Viewport.Width / 848f;
            // Create the scale transform for Draw. 
            // Do not scale the sprite depth (Z=1).
            SpriteScale = Matrix.CreateScale(screenscale, screenscale, 1);


            ssX = screenscale;

            WLScale.X = (float)device.Viewport.Width / 800f;
            WLScale.Y = (float)device.Viewport.Height / 600f;

            InitializeBones();
            boundFrustum = new BoundingFrustum(viewMatrix * projectionMatrix);
        }

        void InitializeBones()
        {
            BONES = new List<Matrix>(71);
            for (int i = 0; i < 71; ++i)
                BONES.Add(Matrix.Identity);
        }

        void InitializeParticles()
        {
            // Construct our particle system components.            
            explosionParticles = new ExplosionParticleSystem(parentGame, parentGame.Content);
            explosionSmokeParticles = new ExplosionSmokeParticleSystem(parentGame, parentGame.Content);
            shotSmokeParticles = new ShotSmokeParticleSystem(parentGame, parentGame.Content);
            shotExplosionParticles = new ShotExplosionParticleSystem(parentGame, parentGame.Content);
            ringExplosionParticles = new RingExplosionParticleSystem(parentGame, parentGame.Content);
            projectileTrailParticles = new ProjectileTrailParticleSystem(parentGame, parentGame.Content);
            smokePlumeParticles = new SmokePlumeParticleSystem(parentGame, parentGame.Content);
            fireParticles = new FireParticleSystem(parentGame, parentGame.Content);

            sparkParticles = new SparkParticleSystem(parentGame, parentGame.Content);
            concreteParticles = new ConcreteParticleSystem(parentGame, parentGame.Content);

            buildingColumn = new BuildingColumnParticles(parentGame, parentGame.Content);
            buildingDisk = new BuildingDiskParticles(parentGame, parentGame.Content);
            
            // Set the draw order so the explosions and fire
            // will appear over the top of the smoke.
            smokePlumeParticles.DrawOrder = 100;
            explosionSmokeParticles.DrawOrder = 200;
            shotSmokeParticles.DrawOrder = 250;
            projectileTrailParticles.DrawOrder = 300;
            explosionParticles.DrawOrder = 400;
            shotExplosionParticles.DrawOrder = 450;
            fireParticles.DrawOrder = 500;
            ringExplosionParticles.DrawOrder = 510;

            concreteParticles.DrawOrder = 510;
            sparkParticles.DrawOrder = 520;

            buildingColumn.DrawOrder = 600;
            buildingDisk.DrawOrder = 620;

           

            // Register the particle system components.

            Particles = new List<ParticleSystem>();
            Particles.Add(explosionParticles);
            Particles.Add(explosionSmokeParticles);
            Particles.Add(shotSmokeParticles);
            Particles.Add(shotExplosionParticles);
            Particles.Add(projectileTrailParticles);
            Particles.Add(smokePlumeParticles);
            Particles.Add(fireParticles);
            Particles.Add(ringExplosionParticles);

            Particles.Add(concreteParticles);
            Particles.Add(sparkParticles);

            Particles.Add(buildingColumn);
            Particles.Add(buildingDisk);

            

            for (int i = 0; i < Particles.Count; ++i)
                Particles[i].Initialize();
            
        }
                
        void InitializeRenderTargets()
        {
            PresentationParameters pp = device.PresentationParameters;
            
           
            pp.MultiSampleCount = 4;

            int shadowMapW = pp.BackBufferWidth;
            int shadowMapH = pp.BackBufferHeight;

            // Create new floating point render target
            renderTarget1 = new RenderTarget2D(device,
                                                    shadowMapW,
                                                    shadowMapH,
                                                    true,
                                                    pp.BackBufferFormat,
                                                    DepthFormat.Depth24);
           

            renderTarget2 = new RenderTarget2D(device,
                                                    shadowMapW,
                                                    shadowMapH,
                                                    true,
                                                    pp.BackBufferFormat,
                                                    pp.DepthStencilFormat);

            shadowMap = new RenderTarget2D(device,
                                                    shadowMapW,
                                                    shadowMapH,
                                                    false,
                                                    SurfaceFormat.Single,
                                                    DepthFormat.Depth24);

            shadowMapAlt = new RenderTarget2D(device,
                                                    shadowMapW,
                                                    shadowMapH,
                                                    false,
                                                    SurfaceFormat.Single,
                                                    DepthFormat.Depth24);

            depthRT = new RenderTarget2D(device,
                pp.BackBufferWidth,
                pp.BackBufferHeight,
                true,
                SurfaceFormat.Single,
                DepthFormat.Depth24);
            // 32-bit float format using 32 bits for the red channel.

            //// Depth Stencil buffer
            //depthSB = CreateDepthStencil(depthRT, DepthFormat.Depth24Stencil8);


            //// Create depth buffer to use when rendering to the shadow map
            //shadowDepthBuffer = new DepthStencilBuffer(device,
            //                                           shadowMapW,
            //                                           shadowMapH,
            //                                           DepthFormat.Depth24);

            
        }

        
        public void Update()
        {
            GetProjViewMatrix();

            UpdateLightData();

            CameraPosition = LV.FollowCam.cameraPosition;

            boundFrustum = new BoundingFrustum(viewMatrix * projectionMatrix);

            InitializeBones();
        }

        public void Unload()
        {
            parentGame.Components.Remove(debugDrawer);
            debugDrawer = null;
        }

        void GetProjViewMatrix()
        {
            projectionMatrix = parentGame.projectionMatrix;
            viewMatrix = parentGame.viewMatrix;
        }

        void UpdateLightData()
        {
            //lightPos;// = new Vector3(4.0f, 6.0f, -6.0f);
            //lightPower = 1.5f;
            //ambientPower = 1.1f;

            lightViewProjection = CreateLightViewProjectionMatrix();
        }

        public void UpdateFrameRate(GameTime gameTime)
        {
            elapsedTime = gameTime.ElapsedGameTime;

            elapsedTimeFrame += gameTime.ElapsedGameTime;

            if (elapsedTimeFrame > TimeSpan.FromSeconds(1))
            {
                elapsedTimeFrame -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
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
                MathHelper.ToRadians(FOV),   //MathHelper.PiOver4,
               1.0f,
               1f, FAR_PLANE);


            BoundingFrustum newFrustum = new BoundingFrustum(Matrix.CreateLookAt
                (LV.CameraPosition,
                LV.CameraPosition + lightDir,
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

            Vector3 boxSize = lightBox.Max - lightBox.Min;
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

        List<Firework> Fireworks = new List<Firework>();
        float lastFirework;
        float timeBetween = 0.4f;
        void MakeFireworks()
        {   
            lastFirework += (float)elapsedTime.Milliseconds / 1000;
            if (lastFirework > timeBetween)
            {
                lastFirework = 0;
                timeBetween = (float)(0.4f + LV.random.NextDouble() * (1.2f-0.4f));
                //float amount = (float)(min + (float)random.NextDouble() * (max - min));
                int min = 65;
                float x = (float)(min + LV.random.NextDouble() * (windowWidth - min - min));
                float y = (float)(min + LV.random.NextDouble() * (windowHeight - min - min));
                Vector2 pos = new Vector2(x, y);

                //float amount = (float)(min + (float)random.NextDouble() * (max - min));
                int r = (int)(10 + LV.random.NextDouble() * (220 - 10));
                int g = (int)(10 + LV.random.NextDouble() * (220 - 10));
                int b = (int)(10 + LV.random.NextDouble() * (220 - 10));
                Color color = new Color(r, g, b, 255);
                
                Firework fw = new Firework(10, 0.05f, pos, color);
                Fireworks.Add(fw);

                LV.FireworkCue = parentGame.soundBank.GetCue("firework");
                LV.FireworkCue.Play();
            }
        }

        public void Draw()
        {
            
            device.Clear(ClearOptions.Target, Color.Black, 0, 0);

            
            RenderToShadowMap();  //<- to shadowMap
            // shadow map must be rendered or effect file must change

            //RenderDepthMap();

            
            RenderFromShadowMap(null); // opt to renderTarget1  OR  null
            

            SetParticleParameters();
            for (int i = 0; i < Particles.Count; ++i)
                Particles[i].Draw(parentGame.gameTime);
            

            //RenderToEdgeDetect(renderTarget2); //<- to rendT 2
            //// optional

            //if (edgeDetect)
            //    ApplyPostprocess(null);

            //ApplyDepthOfField(renderTarget2);
            

            DrawText();

            //DebugSystem.Instance.TimeRuler.BeginMark("Radar", Color.Yellow);
            //DrawRadar();
            //DebugSystem.Instance.TimeRuler.EndMark("Radar");

            if(!parentGame.NormalMode)
                DrawTimer();

            #region EndState
            if (LV.sessionState == GameplayScreen.SessionState.Complete)
                DrawWinLoose();

            if (LV.BuildingRemain <= 0)
            {
                MakeFireworks();
                for (int i = 0; i < Fireworks.Count; ++i)
                    DrawFireWork(Fireworks[i]);
            }
            #endregion

           
            //DrawPostImage();

            ++frameCounter;

           // DebugSystem.Instance.TimeRuler.EndMark("DRAW");
        }

        /// <summary>
        /// Render to temporary render target to be used as depth map from camera perspective.
        /// </summary>
        void RenderToEdgeDetect(RenderTarget2D target)
        {
            Effect CurrentEffect = objEffect;
            String CurrentTechnique = "NormalDepth";

            //--------------EDGE DETECT---------------------------
            SetDraw3D();
            device.SetRenderTarget(target);
            device.Clear(Color.White);
            SetDraw3D();
            DrawTerrain(CurrentEffect, CurrentTechnique);
            SetDraw3D();
            DrawGameObjects(CurrentEffect, CurrentTechnique);
            //-----------------------------------------------------
        }

        /// <summary>
        /// Renders to temporary target or the screen, the full image.
        /// </summary>
        void RenderFromShadowMap(RenderTarget2D target)
        {
            Effect CurrentEffect = objEffect;           

            //-------------DRAW FROM SHADOW MAP---------------------------
            //if (edgeDetect)
            //    device.SetRenderTarget(target);
            //else

            device.SetRenderTarget(target);

            device.Clear(ClearOptions.Target, LV.FogColor, 0, 0);

            //device.Clear(Color.Tan);

            //DebugSystem.Instance.TimeRuler.BeginMark("Tex_Terrain", Color.Green);
            
            SetDraw3D();
              
            DrawTerrain(CurrentEffect, "ShadowedScene_Ter");

            //DebugSystem.Instance.TimeRuler.EndMark("Tex_Terrain");


            //DebugSystem.Instance.TimeRuler.BeginMark("Tex_Objects", Color.Green);
            ////SetDraw3D();
            DrawGameObjects(CurrentEffect, "ShadowedScene_Obj");
            //DebugSystem.Instance.TimeRuler.EndMark("Tex_Objects");

        }

        /// <summary>
        /// Create the shadow map from the sun/light source perspective.  Blur shadow map twice.
        /// </summary>
        void RenderToShadowMap()
        {
            Effect CurrentEffect = objEffect;
            String CurrentTechnique = "ShadowMap";

            device.SetRenderTarget(shadowMap);

            device.Clear(Color.Black);

            SetDraw3D();
            
            DrawTerrain(objEffect, CurrentTechnique);

            SetDraw3D();
            DrawGameObjects(objEffect, CurrentTechnique);

            //device.SetRenderTarget(null);
            //DrawBlur(1.0f / (float)shadowMapAlt.Width, 0,
            //    GausBlur, shadowMap, shadowMapAlt);

            //device.SetRenderTarget(null);
            //DrawBlur(0, 1.0f / (float)shadowMap.Height,
            //    GausBlur, shadowMapAlt, shadowMap);

            device.SetRenderTarget(null);

        }

        void SetDraw3D()
        {
            device.RasterizerState = RasterizerState.CullCounterClockwise;

            device.BlendState = BlendState.Opaque;
            device.DepthStencilState = DepthStencilState.Default;

            device.SamplerStates[0] = SamplerState.LinearWrap;
            device.SamplerStates[0] = SamplerState.PointClamp;  
        }



        public void SetEffectParameters(Effect effectToUse, String technique, Texture2D texture,
            Matrix worldMatrix, bool solidBrown, bool multiTexture)
        {
            Effect curEffect = effectToUse;
            curEffect.CurrentTechnique = curEffect.Techniques[technique];


            if (multiTexture)
            {
                if (technique == "ShadowedScene_Ter")
                {
                    curEffect.Parameters["xTexture0"].SetValue(LV.terrainEngine.terrainTexture0);
                    curEffect.Parameters["xTexture1"].SetValue(LV.terrainEngine.terrainTexture1);
                    curEffect.Parameters["xTexture2"].SetValue(LV.terrainEngine.terrainTexture2);

                    //curEffect.Parameters["xDecalTex"].SetValue(LV.terrainEngine.decalTexture);
                    curEffect.Parameters["xDecal"].SetValue(false);
                    curEffect.Parameters["xToon"].SetValue(toonLighting);
                    curEffect.Parameters["xSky"].SetValue(false);
                }
            }

            if (technique == "ShadowMap")
            {
                curEffect.Parameters["xWorld"].SetValue(worldMatrix);
                curEffect.Parameters["xLightsViewProjection"].SetValue(lightViewProjection);
            }

            else if (technique == "NormalDepth")
            {
                curEffect.Parameters["xWorld"].SetValue(worldMatrix);
                curEffect.Parameters["xLightsViewProjection"].SetValue(lightViewProjection);
            }

            else if (technique == "ShadowedScene_Ter"
                || technique == "ShadowedScene_Obj")
            {
                curEffect.Parameters["xMultiTextured"].SetValue(multiTexture);


                curEffect.Parameters["xCamerasViewProjection"].SetValue(viewMatrix * projectionMatrix);

                curEffect.Parameters["xTexture"].SetValue(texture);
                curEffect.Parameters["xShadowMap"].SetValue(shadowMap);

                curEffect.Parameters["xSolidBrown"].SetValue(solidBrown);
                curEffect.Parameters["xColor"].SetValue(Color.White.ToVector4());

                curEffect.Parameters["xWorld"].SetValue(worldMatrix);
                curEffect.Parameters["xLightPos"].SetValue(lightPos);
                curEffect.Parameters["xLightPower"].SetValue(lightPower);
                curEffect.Parameters["xAmbient"].SetValue(ambientPower);

                curEffect.Parameters["xLightsViewProjection"].SetValue(lightViewProjection);

                curEffect.Parameters["xToon"].SetValue(toonLighting);
                curEffect.Parameters["xSky"].SetValue(false);

                curEffect.Parameters["SpecularColor"].SetValue(LV.FogColor.ToVector4());
                curEffect.Parameters["SpecularIntensity"].SetValue(SpecularIntensity);
                curEffect.Parameters["Shinniness"].SetValue(Shinniness);
                curEffect.Parameters["CameraPosition"].SetValue(CameraPosition);

                curEffect.Parameters["FogEnabled"].SetValue(1f);
                curEffect.Parameters["FogStart"].SetValue(400f);
                curEffect.Parameters["FogEnd"].SetValue(1200f);
                curEffect.Parameters["FogColor"].SetValue(LV.FogColor.ToVector4());
            }
            else
            {
                curEffect.Parameters["matWorldViewProj"].SetValue(worldMatrix * viewMatrix * projectionMatrix);
            }


        }

        //public void SetEffectParameters(Effect effectToUse, String technique, Texture2D texture,
        //    Matrix worldMatrix, bool solidBrown, bool multiTexture, bool sky, Color color)
        //{
        //    Effect curEffect = effectToUse;
        //    curEffect.CurrentTechnique = curEffect.Techniques[technique];


        //    if (multiTexture)
        //    {
        //        if (technique != "DepthMapShader")
        //        {
        //            curEffect.Parameters["xTexture0"].SetValue(LV.terrainEngine.terrainTexture0);
        //            curEffect.Parameters["xTexture1"].SetValue(LV.terrainEngine.terrainTexture1);
        //            curEffect.Parameters["xTexture2"].SetValue(LV.terrainEngine.terrainTexture2);

        //            curEffect.Parameters["xDecalTex"].SetValue(LV.terrainEngine.decalTexture);
        //        }
        //    }
        //    if (technique != "DepthMapShader")
        //    {
        //        curEffect.Parameters["xMultiTextured"].SetValue(multiTexture);


        //        curEffect.Parameters["xCamerasViewProjection"].SetValue(viewMatrix * projectionMatrix);

        //        curEffect.Parameters["xTexture"].SetValue(texture);
        //        curEffect.Parameters["xShadowMap"].SetValue(shadowMap);

        //        curEffect.Parameters["xSolidBrown"].SetValue(solidBrown);
        //        curEffect.Parameters["xColor"].SetValue(color.ToVector4());

        //        curEffect.Parameters["xWorld"].SetValue(worldMatrix);
        //        curEffect.Parameters["xLightPos"].SetValue(lightPos);
        //        curEffect.Parameters["xLightPower"].SetValue(lightPower);
        //        curEffect.Parameters["xAmbient"].SetValue(ambientPower);

        //        curEffect.Parameters["xLightsViewProjection"].SetValue(lightViewProjection);

        //        curEffect.Parameters["xToon"].SetValue(toonLighting);
        //        curEffect.Parameters["xSky"].SetValue(sky);

        //        curEffect.Parameters["SpecularColor"].SetValue(SpecularColor);
        //        curEffect.Parameters["SpecularIntensity"].SetValue(SpecularIntensity);
        //        curEffect.Parameters["Shinniness"].SetValue(Shinniness);
        //        curEffect.Parameters["CameraPosition"].SetValue(CameraPosition);
        //    }
        //    else
        //    {
        //        curEffect.Parameters["matWorldViewProj"].SetValue(worldMatrix * viewMatrix * projectionMatrix);
        //    }

        //}

        void SetParticleParameters()
        {
            // Pass camera matrices through to the particle system components.
            explosionParticles.SetCamera(viewMatrix, projectionMatrix, LV.IsPaused, parentGame.SlowTime);
            explosionSmokeParticles.SetCamera(viewMatrix, projectionMatrix, LV.IsPaused, parentGame.SlowTime);
            shotSmokeParticles.SetCamera(viewMatrix, projectionMatrix, LV.IsPaused, parentGame.SlowTime);
            shotExplosionParticles.SetCamera(viewMatrix, projectionMatrix, LV.IsPaused, parentGame.SlowTime);
            projectileTrailParticles.SetCamera(viewMatrix, projectionMatrix, LV.IsPaused, parentGame.SlowTime);
            smokePlumeParticles.SetCamera(viewMatrix, projectionMatrix, LV.IsPaused, parentGame.SlowTime);
            fireParticles.SetCamera(viewMatrix, projectionMatrix, LV.IsPaused, parentGame.SlowTime);
            ringExplosionParticles.SetCamera(viewMatrix, projectionMatrix, LV.IsPaused, parentGame.SlowTime);            

            concreteParticles.SetCamera(viewMatrix, projectionMatrix, LV.IsPaused, parentGame.SlowTime);
            sparkParticles.SetCamera(viewMatrix, projectionMatrix, LV.IsPaused, parentGame.SlowTime);

            buildingColumn.SetCamera(viewMatrix, projectionMatrix, LV.IsPaused, parentGame.SlowTime);
            buildingDisk.SetCamera(viewMatrix, projectionMatrix, LV.IsPaused, parentGame.SlowTime);
        }

        void DrawTerrain(Effect CurrentEffect, String CurrentTechnique)

        {

            if (CurrentTechnique == "ShadowMap")
            {
                SetEffectParameters(CurrentEffect, CurrentTechnique,
                    null, LV.terrainEngine.worldMatrix, false, false);

                LV.terrainEngine.DrawTerrain(CurrentEffect);
            }

            else if (CurrentTechnique == "NormalDepth")
            {
                SetEffectParameters(CurrentEffect, CurrentTechnique,
                    null, LV.terrainEngine.worldMatrix, false, false);

                LV.terrainEngine.DrawTerrain(CurrentEffect);                
            }

            else if (CurrentTechnique == "ShadowedScene_Ter")
            {
                DrawSkyDome(viewMatrix, LV.CameraPosition, projectionMatrix, CurrentTechnique, LV.terrainEngine.cloudMap);

                SetEffectParameters(CurrentEffect, CurrentTechnique,
                    LV.terrainEngine.terrainTexture, LV.terrainEngine.worldMatrix, false, true);

                LV.terrainEngine.DrawTerrain(CurrentEffect);
                
            }

            ////VertexPositionColor[] wrFrm = LV.terrainActor.Skin.GetLocalSkinWireframe();
            ////LV.terrainActor.Body.TransformWireframe(wrFrm);
            ////debugDrawer.DrawShape(wrFrm);
        }

        void DrawSkyDome(Matrix currentViewMatrix, Vector3 Position, Matrix ProjectionMatrix, String Technique, Texture2D cloudMap)
        {            
            device.DepthStencilState = DepthStencilState.None;
            device.RasterizerState = RasterizerState.CullNone;

            Matrix[] modelTransforms = new Matrix[LV.terrainEngine.skyDome.Bones.Count];
            LV.terrainEngine.skyDome.CopyAbsoluteBoneTransformsTo(modelTransforms);

            Vector3 cldPos = Position;
            cldPos.Y -= 25;
            Matrix wMatrix = Matrix.CreateTranslation(0, -0.1f, 0)
                * Matrix.CreateScale(1)
                * Matrix.CreateTranslation(cldPos);

            Technique = "ShadowedScene_Obj";

            foreach (ModelMesh mesh in LV.terrainEngine.skyDome.Meshes)
            {
                foreach (Effect currentEffect in mesh.Effects)
                {
                    Matrix worldMatrix = modelTransforms[mesh.ParentBone.Index] * wMatrix;

                    SetEffectParameters(currentEffect, Technique, cloudMap, worldMatrix, false, false);
                    currentEffect.Parameters["xSky"].SetValue(true);
                }
                mesh.Draw();
            }

            

            //device.RenderState.DepthBufferWriteEnable = true;
            device.DepthStencilState = DepthStencilState.Default;
            device.RasterizerState = RasterizerState.CullCounterClockwise;
        }


        void DrawDOFtoTarget(RenderTarget2D target, Texture2D SceneTexture, Texture2D BlureSceneTexture, 
            float DoFDistance, float DoFRange, float NearPlane, float FarPlane)
        {
            device.SetRenderTarget(target);

            // Render the scene with with post process DoF.
            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            {
                SetShaderParameters(DoFDistance, DoFRange, NearPlane, FarPlane);

                // Apply the post process shader
                foreach (EffectPass pass in effectPostDoF.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        effectPostDoF.Parameters["D1M"].SetValue(depth1Texture);
                        effectPostDoF.Parameters["BlurScene"].SetValue(BlureSceneTexture);
                        //effectPostDoF.Parameters["SceneTexture"].SetValue(SceneTexture);
                        spriteBatch.Draw(SceneTexture, new Rectangle(0, 0, 800, 600), Color.White);
                        
                    }
                
               
            }
            spriteBatch.End();
        }

        void RenderDepthMap()
        {
            // Render our depthmaps. Here we should only render the objects that will be tramsitted
            // create depth-map 1
            depthMapShader.CurrentTechnique = depthMapShader.Techniques["DepthMapShader"];

            //device.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
            device.RasterizerState = RasterizerState.CullCounterClockwise;
            depth1Texture = RenderDepthMap( depthRT);
        }

        private Texture2D RenderDepthMap( RenderTarget2D rt2D)
        {
            //device.RenderState.DepthBufferFunction = CompareFunction.LessEqual;
            device.SetRenderTarget( rt2D);

            // Save our DepthStencilBuffer, so we can restore it later
            //DepthStencilBuffer saveSB = device.DepthStencilBuffer;

            //device.DepthStencilBuffer = dsb;
            device.Clear(Color.Black);

            DrawTerrain(objEffect, "DepthMapShader");
            DrawGameObjects(objEffect, "DepthMapShader");

            // restore old depth stencil buffer
            device.SetRenderTarget( null);
            //device.DepthStencilBuffer = saveSB;

            return rt2D;
        }

        void SetShaderParameters(float fD, float fR, float nC, float fC)
        {
            float focusDistance = fD;
            float focusRange = fR;
            float nearClip = nC;
            float farClip = fC;
            farClip = farClip / (farClip - nearClip);

            effectPostDoF.CurrentTechnique = effectPostDoF.Techniques["PostProcess"];

            effectPostDoF.Parameters["Distance"].SetValue(focusDistance);
            effectPostDoF.Parameters["Range"].SetValue(focusRange);
            effectPostDoF.Parameters["Near"].SetValue(nearClip);
            effectPostDoF.Parameters["Far"].SetValue(farClip);
            
        }

        void DrawWinLoose()
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            float SCALE = 0.82f;

            Vector2 pos = new Vector2(windowWidth / 2, windowHeight / 2);
            Vector2 origin = new Vector2(WinBack.Width / 2, WinBack.Height / 2);
            

            string title = "CONGRATULATIONS";
            if (LV.BuildingRemain > 40)
            {
                spriteBatch.Draw(
                LooseBack,
                    //800,500
                pos,         //Position
                null,                 // rect
                Color.White,            // color
                0.0f,                   // rotation
                origin,           // origin
                1 * WLScale,
                SpriteEffects.None, 1);

                title = "TRY AGAIN";
            }
            else
            {
                spriteBatch.Draw(
                WinBack,
                    //800,500
                pos,         //Position
                null,                 // rect
                Color.White,            // color
                0.0f,                   // rotation
                origin,           // origin
                1 * WLScale,
                SpriteEffects.None, 1);

                parentGame.SlowTime = true;
            }

            Vector2 whitScale = new Vector2(1, 1.10f);
            origin = TitlesFont.MeasureString(title) / 2;                       
            spriteBatch.DrawString(
                TitlesFont,
                title,
                new Vector2(windowWidth * 0.5f, windowHeight * 0.17f),
                Color.Black,
                0,
                origin,
                1.1F * WLScale,
                SpriteEffects.None,
                0);

            title = "SCORE:";
            origin = TitlesFont.MeasureString(title) / 2;

            //spriteBatch.DrawString(TitlesFont2, title,
            //    new Vector2(windowWidth * 0.689f, windowHeight * 0.310f),
            //    Color.White,
            //    0,
            //    origin,
            //    SCALE * WLScale ,
            //    SpriteEffects.None,
            //    1f);

            spriteBatch.DrawString(
                TitlesFont,
                title,
                new Vector2(windowWidth * 0.689f, windowHeight * 0.310f),
                Color.LightYellow,
                0,
                origin,
                SCALE * WLScale,
                SpriteEffects.None,
                0);

            

            title = "TARGETS:";
            //origin = TitlesFont2.MeasureString(title) / 2;
            //spriteBatch.DrawString(
            //    TitlesFont2,
            //    title,
            //    new Vector2(windowWidth * 0.295f, windowHeight * 0.310f),
            //    Color.White,
            //    0,
            //    origin,
            //    SCALE * WLScale ,
            //    SpriteEffects.None,
            //    0);
            origin = TitlesFont.MeasureString(title) / 2;
            spriteBatch.DrawString(
                TitlesFont,
                title,
                new Vector2(windowWidth * 0.295f, windowHeight * 0.310f),
                Color.Wheat,
                0,
                origin,
                SCALE * WLScale,
                SpriteEffects.None,
                0);

            title = "KILLED:";
            //origin = TitlesFont2.MeasureString(title) / 2;
            //spriteBatch.DrawString(
            //    TitlesFont2,
            //    title,
            //    new Vector2(windowWidth * 0.259f, windowHeight * 0.403f),
            //    Color.White,
            //    0,
            //    origin,
            //    SCALE * WLScale * 0.8f ,
            //    SpriteEffects.None,
            //    0);
            origin = TitlesFont.MeasureString(title) / 2;
            spriteBatch.DrawString(
                TitlesFont,
                title,
                new Vector2(windowWidth * 0.259f, windowHeight * 0.403f),
                Color.LightGreen,
                0,
                origin,
                SCALE * WLScale * 0.8f,
                SpriteEffects.None,
                0);

            title = "REMAINING:";
            origin = TitlesFont.MeasureString(title) / 2;
            //spriteBatch.DrawString(
            //    TitlesFont2,
            //    title,
            //    new Vector2(windowWidth * 0.296f, windowHeight * 0.473f),
            //    Color.White,
            //    0,
            //    origin,
            //    SCALE * WLScale * 0.8f ,
            //    SpriteEffects.None,
            //    0);
            spriteBatch.DrawString(
                TitlesFont,
                title,
                new Vector2(windowWidth * 0.296f, windowHeight * 0.473f),
                Color.LightSalmon,
                0,
                origin,
                SCALE * WLScale * 0.8f,
                SpriteEffects.None,
                0);

            title = "AMMO:";
            origin = TitlesFont.MeasureString(title) / 2;
            spriteBatch.DrawString(
                TitlesFont,
                title,
                new Vector2(windowWidth * 0.265f, windowHeight * 0.643f),
                Color.White,
                0,
                origin,
                SCALE * WLScale * 1.05f * whitScale,
                SpriteEffects.None,
                0);
            spriteBatch.DrawString(
                TitlesFont,
                title,
                new Vector2(windowWidth * 0.265f, windowHeight * 0.643f),
                Color.Black,
                0,
                origin,
                SCALE * WLScale,
                SpriteEffects.None,
                0);

            title = "SPENT:";
            origin = TitlesFont.MeasureString(title) / 2;
            spriteBatch.DrawString(
               TitlesFont,
               title,
               new Vector2(windowWidth * 0.258f, windowHeight * 0.730f),
               Color.White,
               0,
               origin,
               SCALE * WLScale * 0.8f * 1.05f * whitScale,
               SpriteEffects.None,
               0);
            spriteBatch.DrawString(
                TitlesFont,
                title,
                new Vector2(windowWidth * 0.258f, windowHeight * 0.730f),
                Color.Black,
                0,
                origin,
                SCALE * WLScale * 0.8f,
                SpriteEffects.None,
                0);

            title = "REMAINING:";
            origin = TitlesFont.MeasureString(title) / 2;
            spriteBatch.DrawString(
                TitlesFont,
                title,
                new Vector2(windowWidth * 0.299f, windowHeight * 0.807f),
                Color.White,
                0,
                origin,
                SCALE * WLScale * 0.8f * 1.025f * whitScale,
                SpriteEffects.None,
                0);
            spriteBatch.DrawString(
                TitlesFont,
                title,
                new Vector2(windowWidth * 0.299f, windowHeight * 0.807f),
                Color.Black,
                0,
                origin,
                SCALE * WLScale * 0.8f,
                SpriteEffects.None,
                0);

            //TARGETS KIL
            title = (LV.Buildings.Count - LV.BuildingRemain).ToString();
            origin = TitlesFont.MeasureString(title) / 2;           
            spriteBatch.DrawString(
                TitlesFont,
                title,
                new Vector2(windowWidth * 0.468f, windowHeight * 0.410f),
                Color.FloralWhite,
                0,
                origin,
                SCALE * WLScale * 0.8f,
                SpriteEffects.None,
                0);

            //TARGETS REM
            title = LV.BuildingRemain.ToString();
            origin = TitlesFont.MeasureString(title) / 2;           
            spriteBatch.DrawString(
                TitlesFont,
                title,
                new Vector2(windowWidth * 0.468f, windowHeight * 0.478f),
                Color.FloralWhite,
                0,
                origin,
                SCALE * WLScale * 0.8f,
                SpriteEffects.None,
                0);

            //AMMO SPEN
            title = (LV.MissileStart - LV.MissileRemain).ToString();
            origin = TitlesFont.MeasureString(title) / 2;
            spriteBatch.DrawString(
                TitlesFont,
                title,
                new Vector2(windowWidth * 0.468f, windowHeight * 0.747f),
                Color.White,
                0,
                origin,
                SCALE * WLScale * 0.8f * 1.05f,
                SpriteEffects.None,
                0);
            spriteBatch.DrawString(
                TitlesFont,
                title,
                new Vector2(windowWidth * 0.468f, windowHeight * 0.747f),
                Color.Black,
                0,
                origin,
                SCALE * WLScale * 0.8f,
                SpriteEffects.None,
                0);

            //AMMO REM
            title = LV.MissileRemain.ToString();
            origin = TitlesFont.MeasureString(title) / 2;
            spriteBatch.DrawString(
                TitlesFont,
                title,
                new Vector2(windowWidth * 0.468f, windowHeight * 0.810f),
                Color.White,
                0,
                origin,
                SCALE * WLScale * 0.8f * 1.05f,
                SpriteEffects.None,
                0);
            spriteBatch.DrawString(
                TitlesFont,
                title,
                new Vector2(windowWidth * 0.468f, windowHeight * 0.810f),
                Color.Black,
                0,
                origin,
                SCALE * WLScale * 0.8f,
                SpriteEffects.None,
                0);

            //SCORE
            title = (LV.GetScore()).ToString(); 
            origin = TitlesFont.MeasureString(title) / 2;
            spriteBatch.DrawString(
                TitlesFont,
                title,
                new Vector2(windowWidth * 0.69f, windowHeight * 0.410f),
                Color.White,
                0,
                origin,
                SCALE * WLScale * 0.8f * 1.05f,
                SpriteEffects.None,
                0);
            spriteBatch.DrawString(
                TitlesFont,
                title,
                new Vector2(windowWidth * 0.69f, windowHeight * 0.410f),
                Color.Black,
                0,
                origin,
                SCALE * WLScale * 0.8f,
                SpriteEffects.None,
                0);

            spriteBatch.End();
        }

        void DrawFireWork(Firework Fw)
        {

            Fw.TimeLastChange += (float)elapsedTime.Milliseconds / 1000;
            if (Fw.TimeLastChange > Fw.TimeForFrame)
            {
                Fw.CurFrame += 1;
                Fw.TimeLastChange = 0;
            }
            if (Fw.CurFrame >= Fw.MaxFrame)
                Fireworks.Remove(Fw);

            
            Vector2 origin = new Vector2(FireworkSheet.Height/2);

            Rectangle frame = new Rectangle(FireworkSheet.Height * Fw.CurFrame, 0, FireworkSheet.Height, FireworkSheet.Height);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            spriteBatch.Draw(FireworkSheet, Fw.Position, frame,
                Fw.Color, 0.0f, origin, ssX * 1.0f,
                SpriteEffects.None, 0.0f);

            spriteBatch.End();
        }

        void DrawTimer()
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            double secs = LV.TimeRemaining / 1000;

            double min = (secs / 60);
            double minf = Math.Floor(secs / 60);
            double sec = Math.Floor((min - minf) * 60);
            string add = "";
            if (sec < 10)
                add = "0";
        
            string text = minf + " : " + add + sec;

            float SCALE = 0.8f * ssX;
            float xpos = windowWidth * 0.85f;
            float ypos = windowHeight * 0.1f;

            spriteBatch.Draw(
                Timer, new Vector2(xpos, ypos),
                null, Color.White, 0.0f,
                new Vector2(Timer.Width / 2, Timer.Height / 2),
                SCALE, SpriteEffects.None, 0.5f);

            spriteBatch.DrawString(TitlesFont, text,
                new Vector2(xpos, ypos), Color.White,
                0, TitlesFont.MeasureString(text) / 2, SCALE, SpriteEffects.None, 0.0f);

            spriteBatch.End();
        }

        void DrawRadar()
        {            
            spriteBatch.Begin( SpriteSortMode.Immediate, BlendState.AlphaBlend);

            float SCALE = 0.6f * ssX;
            float radarR = (LV.RadarTexture.Width / 2) * SCALE;

            float radarX = (windowWidth - (radarR) - (40 * ssX));// -(radarR * 0.1f);
            float radarY = (windowHeight - (radarR) - (40 * ssX));// -(radarR * 0.1f);

           

            spriteBatch.Draw(
                LV.RadarTexture,
                //800,500
                new Vector2(radarX, radarY),         //Position
                null,
                Color.White,
                0.0f,
                new Vector2(
                LV.RadarTexture.Width / 2, LV.RadarTexture.Height / 2),
                SCALE,
                SpriteEffects.None, 0.0f);

            foreach (Building building in LV.Buildings)
            {
                if (!building.isDestroyed)
                {
                    float ratio = (radarX + radarR / radarX);
                    float x = (0.51f) * (building.position.X - LV.Player.position.X);
                    float y = (0.51f) * (building.position.Z - LV.Player.position.Z);

                    float dx = (float)Math.Cos(LV.Player.turretRotationValue) * (x) + x + radarX;
                    float dy = (float)Math.Sin(LV.Player.turretRotationValue) * (y) - y + radarY;
                    

                    Vector2 pos = new Vector2(x, y);

                    pos = Vector2.Transform(pos, Matrix.CreateFromYawPitchRoll(0,
                        0, LV.Player.turretRotationValue - MathHelper.ToRadians(90)));

                    pos *= SCALE;

                    pos.X += radarX;
                    pos.Y += radarY;

                    if (Vector2.Distance(pos, new Vector2(radarX, radarY)) < 100 * SCALE)
                    {
                        spriteBatch.Draw(LV.BlipTexture, new Vector2(pos.X, pos.Y), null,
                            Color.White, 0.0f, new Vector2(LV.BlipTexture.Width / 2, LV.BlipTexture.Height / 2),
                            0.6f * SCALE, SpriteEffects.None, 0.0f);
                    }
                }
            }

            
           

            float xp =windowWidth * 0.05f;
            float yp = windowHeight * 0.9f;
           
            spriteBatch.Draw(
               PowerBar,
                new Vector2(xp, yp),          //Position
               null,
               Color.White,
               0.0f,
               new Vector2(0, PowerBar.Height * 0.974f),        // origin
               new Vector2(ssX * 1, ssX * LV.Player.Power),
               SpriteEffects.None, 0.0f);
            
            spriteBatch.Draw(
               PowerCase,
               new Vector2(xp, yp),         //Position
               null,
               Color.White,
               0.0f,
               new Vector2(0, PowerBar.Height * 0.974f),
               ssX,
               SpriteEffects.None, 0.0f);

            // ANGLE
            float buffer = 80 * ssX;
            SCALE = ssX * 0.8f;
            float xpp = xp +  PowerCase.Width + (buffer);
            spriteBatch.Draw(
               Angle,
                new Vector2(xpp, yp),          //Position
               null,
               Color.White,
               0.0f,
               new Vector2(0, Angle.Height),
               SCALE,
               SpriteEffects.None, 0.0f);

            float value = (float)Math.Round(MathHelper.ToDegrees(LV.Player.cannonRotationValue), 0);
            value = -value;
            string stuff = string.Format("{0}", value.ToString());
            spriteBatch.DrawString(font, stuff,
                new Vector2((xpp + (Angle.Width * 0.78f) * SCALE), (yp - (Angle.Height * 0.45f) * SCALE)), 
                Color.White,
                0, font.MeasureString(stuff)/2, 1.5f, SpriteEffects.None, 0);


            // BULLET 491 622

            float xppp = xpp + Angle.Width + buffer;
            SCALE = ssX * 0.8f;
            spriteBatch.Draw(
              Bullets,
               new Vector2(xppp, yp),          //Position
              null,
              Color.White,
              0.0f,
              new Vector2(0, Bullets.Height),
              SCALE,
              SpriteEffects.None, 0.0f);

            stuff = string.Format("{0}", LV.MissileRemain.ToString());
            spriteBatch.DrawString(font, stuff,
                new Vector2((xppp + (Bullets.Width * 0.491f) * SCALE), (yp - (Bullets.Height * 0.40f) * SCALE)), 
                Color.White,
                0, font.MeasureString(stuff)/2, 1.5f, SpriteEffects.None, 0);


            spriteBatch.End();
        }

        public int BLEND = 0;
        void DrawPostImage()
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            Color myTransparentColor = new Color(0, 0, 0, 5);

            float ssX =
               (float)device.Viewport.Width / 800f;



            //spriteBatch.Draw(
            //    Lines,
            //    //800,500
            //    new Vector2(windowWidth / 2, windowHeight / 2),         //Position
            //    null,
            //    myTransparentColor,
            //    0.0f,
            //    new Vector2(
            //    Lines.Width / 2, Lines.Height / 2),
            //    ssX,
            //    SpriteEffects.None, 0.0f);

            myTransparentColor = new Color(255, 255, 255, 150);
            spriteBatch.Draw(
                Post,
                new Vector2(windowWidth / 2, windowHeight / 2),         //Position
                null,
                myTransparentColor,
                0.0f,
                new Vector2(
                Post.Width / 2, Post.Height / 2),
                ssX,
                SpriteEffects.None, 0.0f);

            spriteBatch.End();
        }

        
        void DrawGameObjects(Effect effectToUse, String technique)
        {
            ContainmentType contains = new ContainmentType();
            
            //if (technique == "ShadowedScene_Obj")
            //    DebugSystem.Instance.TimeRuler.BeginMark("PLAYER", Color.Blue);
            #region PLAYER
            GameObject obj = LV.Player;
            boundFrustum.Contains(ref obj.boundSphere, out contains);
            if (contains == ContainmentType.Contains || contains == ContainmentType.Intersects)
                DrawModel(effectToUse, obj.model, obj.textures, obj.worldMatrix, technique, false);
            #endregion

            #region AVATAR
            
            
            #endregion

            //if (technique == "ShadowedScene_Obj")
            //    DebugSystem.Instance.TimeRuler.EndMark("PLAYER");

            //if (technique == "ShadowedScene_Obj")
            //    DebugSystem.Instance.TimeRuler.BeginMark("MISSILES", Color.Blue);
            #region MISSILES
            foreach (Missile missile in BulletList)
            {
                if (!missile.isDestroyed)
                {
                    obj = missile;
                    boundFrustum.Contains(ref obj.boundSphere, out contains);
#if !XBOX
                    
            if(contains == ContainmentType.Contains || contains == ContainmentType.Intersects)
                        DrawModel(effectToUse, obj.model, obj.textures, (missile).GetWorldMatrix(), technique, false);                    
#endif

                    
                    //LM.Add(GetMatrix(limbs[(int)LimbId.Torso].PhysicsBody, SCALE));
                    //BONES[14] = missile.RagdollTransforms[0];
                    //LM.Add(GetMatrix(limbs[(int)LimbId.Head].PhysicsBody, SCALE));
                    BONES[19] = missile.RagdollTransforms[1];

                    //LM.Add(GetMatrix(limbs[(int)LimbId.UpperLegLeft].PhysicsBody, SCALE));
                    BONES[2] = missile.RagdollTransforms[2];
                    //LM.Add(GetMatrix(limbs[(int)LimbId.UpperLegRight].PhysicsBody, SCALE));
                    BONES[3] = missile.RagdollTransforms[3];

                    //LM.Add(GetMatrix(limbs[(int)LimbId.LowerLegLeft].PhysicsBody, SCALE));
                    BONES[11] = missile.RagdollTransforms[4];
                    //LM.Add(GetMatrix(limbs[(int)LimbId.LowerLegRight].PhysicsBody, SCALE));
                    BONES[15] = missile.RagdollTransforms[5];

                    //LM.Add(GetMatrix(limbs[(int)LimbId.UpperArmLeft].PhysicsBody, SCALE));
                    BONES[20] = missile.RagdollTransforms[6];
                    //LM.Add(GetMatrix(limbs[(int)LimbId.UpperArmRight].PhysicsBody, SCALE));
                    BONES[22] = missile.RagdollTransforms[7];

                    //LM.Add(GetMatrix(limbs[(int)LimbId.LowerArmLeft].PhysicsBody, SCALE));
                    BONES[25] = missile.RagdollTransforms[8];
                    //LM.Add(GetMatrix(limbs[(int)LimbId.LowerArmRight].PhysicsBody, SCALE));
                    BONES[28] = missile.RagdollTransforms[9];

#if XBOX
                    parentGame.avatarRenderer.Projection = projectionMatrix;
                    parentGame.avatarRenderer.View = viewMatrix;
                    parentGame.avatarRenderer.World = missile.rgob.GetWorld();// GetWorldMatrixScale(5f);

                    if (contains == ContainmentType.Contains || contains == ContainmentType.Intersects)
                        parentGame.avatarRenderer.Draw(BONES, new Microsoft.Xna.Framework.GamerServices.AvatarExpression());
#endif

                    //VertexPositionColor[] wrFrm;

                    //foreach (JigLibX.Objects.PhysicObject lim in missile.rgob.limbs)
                    //{
                    //    wrFrm = lim.PhysicsSkin.GetLocalSkinWireframe();
                    //    lim.PhysicsBody.TransformWireframe(wrFrm);
                    //    debugDrawer.DrawShape(wrFrm);
                    //}

                    //VertexPositionColor[] 
                //    wrFrm = missile.Skin.GetLocalSkinWireframe();
                //    missile.Body.TransformWireframe(wrFrm);
                //    debugDrawer.DrawShape(wrFrm);
                }
            }
            #endregion
            //if (technique == "ShadowedScene_Obj")
            //    DebugSystem.Instance.TimeRuler.EndMark("MISSILES");

            //if (technique == "ShadowedScene_Obj")
            //    DebugSystem.Instance.TimeRuler.BeginMark("BUILDINGS", Color.Blue);
            #region BUILDINGS
            foreach (Building building in BuildingList)
            {
                obj = building;
                boundFrustum.Contains(ref obj.boundSphere, out contains);
                if (building.Type == "RING")
                {
                    if (!building.isDestroyed)
                    {
                        if (contains == ContainmentType.Contains || contains == ContainmentType.Intersects)
                            DrawModel(effectToUse, obj.model, obj.textures, 
                                (building).GetWorldMatrix(),
                                technique, false);
                    }

                    //VertexPositionColor[] wrFrm = building.Skin.GetLocalSkinWireframe();
                    //building.Body.TransformWireframe(wrFrm);
                    //debugDrawer.DrawShape(wrFrm);
                }
                else
                {
                    if (contains == ContainmentType.Contains || contains == ContainmentType.Intersects)
                        DrawModel(effectToUse, obj.model, obj.textures, (building).GetWorldMatrix(), technique, false);

                    //VertexPositionColor[] wrFrm = building.Skin.GetLocalSkinWireframe();
                    //building.Body.TransformWireframe(wrFrm);
                    //debugDrawer.DrawShape(wrFrm);
                }
            }
            #endregion
            //if (technique == "ShadowedScene_Obj")
            //    DebugSystem.Instance.TimeRuler.EndMark("BUILDINGS");

            //if (technique == "ShadowedScene_Obj")
            //    DebugSystem.Instance.TimeRuler.BeginMark("PIECES", Color.Blue);
            #region PIECES
            pieceList = new List<string>();
            foreach (BuildingPiece piece in PieceList)
            {
                obj = piece;
                boundFrustum.Contains(ref obj.boundSphere, out contains);
                if (contains == ContainmentType.Contains || contains == ContainmentType.Intersects)
                    DrawModelP(effectToUse, obj.model, obj.textures, (piece).GetWorldMatrix(), technique, false);

                //VertexPositionColor[] wrFrm = piece.Skin.GetLocalSkinWireframe();
                //piece.Body.TransformWireframe(wrFrm);
                //debugDrawer.DrawShape(wrFrm);
            }
            #endregion
            //if (technique == "ShadowedScene_Obj")
            //    DebugSystem.Instance.TimeRuler.EndMark("PIECES");

        }
        List<string> pieceList = new List<string>();


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

        public void DrawModelP(Effect effectToUse, Model model, Texture2D[] textures,
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
                //DebugSystem.Instance.TimeRuler.BeginMark("MESH_DRAW", Color.Blue);
                mesh.Draw();
                //DebugSystem.Instance.TimeRuler.EndMark("MESH_DRAW");
            }
        }

        public void DrawModel(Effect effectToUse, Model model, Texture2D texture,
            Matrix wMatrix, string technique, bool solidBrown)
        {
            Matrix[] modelTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(modelTransforms);
            
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (Effect currentEffect in mesh.Effects)
                {
                    Matrix world = modelTransforms[mesh.ParentBone.Index] * wMatrix;

                    SetEffectParameters(currentEffect, technique, texture, world, solidBrown, false);
                }
                mesh.Draw();
            }
        }

        public void DrawModel(Effect effectToUse, Model model, Texture2D[] textures,
            Matrix wMatrix, string technique, bool solidBrown, bool sky, Color color)
        {
            Matrix[] modelTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(modelTransforms);            

            int i = 0;
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (Effect currentEffect in mesh.Effects)
                {
                    Matrix world = modelTransforms[mesh.ParentBone.Index] * wMatrix;

                    SetEffectParameters(effectToUse, technique, textures[i++], world, solidBrown, false);

                    //SetEffectParameters(currentEffect, technique, textures[i++], world, solidBrown, false, sky, color);
                }
                mesh.Draw();
            }
        }

        void DrawBlur(float x, float y, Effect effectToUse,
            RenderTarget2D source, RenderTarget2D target)
        {
            device.SetRenderTarget(target);

            SetBlurEffectParameters(x, y, effectToUse, source);

            //device.SamplerStates[0] = SamplerState.LinearClamp;
            //device.SamplerStates[0] = SamplerState.PointClamp;

            device.SamplerStates[0] = SamplerState.LinearClamp;
            device.SamplerStates[0] = SamplerState.PointClamp;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
            //spriteBatch.Begin(SpriteBlendMode.None,
            //                  SpriteSortMode.Immediate,
            //                  SaveStateMode.None);

            foreach(EffectPass pass in effectToUse.CurrentTechnique.Passes)
                spriteBatch.Draw(source,
                    new Rectangle(0, 0,
                        target.Width,
                        target.Height),
                        Color.White);

            spriteBatch.End();

        }

        //void DrawBlur(float x, float y, Effect effectToUse,
        //    Texture2D source, RenderTarget2D target)
        //{
        //    device.SetRenderTarget(target);

        //    SetBlurEffectParameters(x, y, effectToUse, source);

        //    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);

        //    foreach (EffectPass pass in effectToUse.CurrentTechnique.Passes)
        //        spriteBatch.Draw(source,
        //            new Rectangle(0, 0,
        //                target.Width,
        //                target.Height),
        //                Color.White);

        //    spriteBatch.End();
        //}

        void SetBlurEffectParameters(float dx, float dy, Effect effectToUse, Texture2D texture)
        {
            effectToUse.Parameters["xTexture"].SetValue(texture);
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

        /// <summary>
        /// Put the saved shadow map in a small window.
        /// </summary>
        void DrawShadowMapToScreen()
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            spriteBatch.Draw(shadowMap, new Rectangle(650, 0, 128, 128), Color.White);
            spriteBatch.End();
        }


        /// <summary>
        /// Applies edge effect using texture in target2 to scene in target1 than to screen.
        /// UPDATE: or to 'target' wich should be renderTarget2, after texture extract
        /// </summary>
        void ApplyPostprocess(RenderTarget2D target)
        {
            depthTexture = renderTarget2;

            device.SetRenderTarget(target);

            EffectParameterCollection parameters = objEffect.Parameters;
            string effectTechniqueName;
            effectTechniqueName = "EdgeDetect";

            if (edgeDetect)
            {
                Vector2 resolution = new Vector2(windowWidth,
                                                windowHeight);

                parameters["EdgeWidth"].SetValue(parentGame.Level_Loader.myLevel.EdgeWidth); //Settings.EdgeWidth);
                parameters["EdgeIntensity"].SetValue(parentGame.Level_Loader.myLevel.EdgeIntensity); //Settings.EdgeIntensity);


                parameters["NormalThreshold"].SetValue(parentGame.Level_Loader.myLevel.NormalThreshold);
                parameters["DepthThreshold"].SetValue(parentGame.Level_Loader.myLevel.DepthThreshold);

                parameters["NormalSensitivity"].SetValue(parentGame.Level_Loader.myLevel.NormalSensitivity);
                parameters["DepthSensitivity"].SetValue(parentGame.Level_Loader.myLevel.DepthSensitivity);

                parameters["ScreenResolution"].SetValue(resolution);
                parameters["NormalDepthTexture"].SetValue(depthTexture);

                effectTechniqueName = "EdgeDetect";
            }            

            objEffect.CurrentTechnique =
                                    objEffect.Techniques[effectTechniqueName];


            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, null, null, null, objEffect);
            spriteBatch.Draw(renderTarget1, Vector2.Zero, Color.White);
            spriteBatch.End();
        }

        /// <summary>
        /// Applys depth of field thru two pass blurring and dof shader. 
        /// needs source from edge detect post process. (target)
        /// </summary>
        /// <param name="source"></param>
        void ApplyDepthOfField(RenderTarget2D source)
        {
            device.SetRenderTarget(null);

            blurredScene = source;
            sceneTexture = source;
            

            //blurtwice   

            device.SetRenderTarget(renderTarget1);
            device.Clear(Color.White);

            //DrawBlur(1.0f / (float)renderTarget2.Width, 0,
            //    GausBlur, blurredScene, renderTarget1);

            device.SetRenderTarget(null);
            blurredScene = renderTarget1;

            //device.SetRenderTarget(0, renderTarget1);
            //device.Clear(Color.White);

            //DrawBlur(0, 1.0f / (float)renderTarget1.Height,
            //    GausBlur, blurredScene, renderTarget1);

            //device.SetRenderTarget(0, null);
            //blurredScene = renderTarget1.GetTexture();

            device.Clear(Color.White);
            DrawDOFtoTarget(null, sceneTexture, blurredScene,
                dis,    //distance
                rang,     //range
                near,      //near plane
                far);  //far plane
           
        }
        public float far = 10000;

        public float dis = 166;
        public float rang = 161;
        public float near = 1;
               

        void DrawText()
        {
            
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            spriteBatch.DrawString(font, "(2D)TEXT: ",
                new Vector2(10, -5), Color.White);

            int MODE = 1;
            if (MODE == 1)
            {
                DrawText_Move();
            }

            spriteBatch.End();
        }

        void DrawText_Move()
        {
            //--------FRAME RATE---------------------------
            string fps = string.Format("fps: {0}", frameRate);
            spriteBatch.DrawString(font, fps,
                new Vector2(20, 15), Color.White);

            string stuff = string.Format("pos: {0}", LV.Player.position.ToString());
            spriteBatch.DrawString(font, stuff,
                new Vector2(20, 30), Color.White);

            stuff = string.Format("cam: {0}", LV.CameraPosition.ToString());
            spriteBatch.DrawString(font, stuff,
                new Vector2(20, 45), Color.White);

            stuff = string.Format("mis: {0}", BulletList.Count);
            spriteBatch.DrawString(font, stuff,
                new Vector2(20, 60), Color.White);  

            stuff = string.Format("pow:{0}", LV.Player.Power * 100f);
            spriteBatch.DrawString(font, stuff,
                new Vector2(20, 75), Color.White);

            stuff = string.Format("time:{0}", LV.TimeRemaining);
            spriteBatch.DrawString(font, stuff,
                new Vector2(20, 95), Color.White);

            //for (int i = 0; i < PieceList.Count; ++i)
            //{

            //    stuff = string.Format("len:{0}", PieceList[i].Body.Velocity.Length());
            //    spriteBatch.DrawString(font, stuff,
            //        new Vector2(20, 115 + (15*i)), Color.White);
            //}
            
        }

        /// <summary>
        /// Returns the supported depth stencil buffer.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        //private DepthStencilBuffer CreateDepthStencil(RenderTarget2D target)
        //{
        //    return new DepthStencilBuffer(target.GraphicsDevice, target.Width,
        //        target.Height, target.GraphicsDevice.DepthStencilBuffer.Format,
        //        target.MultiSampleType, target.MultiSampleQuality);
        //}

        /// <summary>
        /// Checks if we have support for the DepthFormat in depth and return the information to CreateDepthStencil(RenderTarget2D target)
        /// </summary>
        /// <param name="target"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        //private DepthStencilBuffer CreateDepthStencil(RenderTarget2D target, DepthFormat depth)
        //{
        //    if (GraphicsAdapter.DefaultAdapter.CheckDepthStencilMatch(DeviceType.Hardware,
        //       GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Format, target.Format,
        //        depth))
        //    {
        //        return new DepthStencilBuffer(target.GraphicsDevice, target.Width,
        //            target.Height, depth, target.MultiSampleType, target.MultiSampleQuality);
        //    }
        //    else
        //        return CreateDepthStencil(target);
        //}

    }
}
