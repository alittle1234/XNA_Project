using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace TerrainTutorial
{
    public class LevelEditor
    {
        enum BrushState
        {
            Empty, FlagSelected, EventSelected, Moving, MovingSpawn
        };

        ParentGame parentGame;

        KeyboardState keyboardState;
        MouseState prevMouseState;
        MouseState mouseState;

        BrushState brush = BrushState.Empty;

        string textque = "nothing";

        public string[] EventTypes;

        Form2 FlagForm;
        public EventFlag activeFlag = new EventFlag();

        public int activeCornerIndex;
        public Vector2 activeCorner;
        public GameEvent activeEvent = new GameEvent();

        public Condition ConditionToAdd = null;

        Effect effect;
        Color currentColor = Color.AliceBlue;

        public LevelEditor(ParentGame game)
        {
            parentGame = game;
            effect = parentGame.objEffect;

            EventTypes = new string[]{
                "PUT_PLAYER",
                "PUT_ENEMY",
                "SET_CONDITION",
                "TRIGGER_FLAG",
                "PLAY_ANIMATION",
                "PLAY_SOUND",
                "SET_OBJECTIVE",
                "END_LEVEL",
                "END_GAME"
            };
        }

        public void Update()
        {
            HandleInput();
            MovingElement();

            FlagForm = parentGame.form2;
            FlagForm.Show();
            FlagForm.label1.Text = textque;

            if (activeFlag != null)
            {
                UpdateActiveFlag();

                ShowActiveFlag();
            }

            parentGame.PutLevelInXML();
        }

        void UpdateActiveFlag()
        {
           
        }

        void ShowActiveEvent()
        {

        }

        void ShowActiveFlag()
        {
            FlagForm.label_box.Text = activeFlag.Label;
            FlagForm.delay_box.Text = activeFlag.Delay.ToString();
            FlagForm.event_type.Text = activeEvent.EventType.ToString();

            ShowEventTarget();
           
            FlagForm.events_list.DataSource = activeFlag.Events;
            FlagForm.listBox1.DataSource = EventTypes;

            ShowInitialConditions();
        }

        void ShowEventTarget()
        {
            if (activeEvent != null)
            {
                if (activeEvent.EventType == GameEvent.GameEventType.PUT_PLAYER)
                    FlagForm.event_target.Text = activeEvent.SpawnPoint.Position.ToString();

                else if (activeEvent.EventType == GameEvent.GameEventType.PUT_ENEMY)
                    FlagForm.event_target.Text = activeEvent.SpawnPoint.Position.ToString();

                FlagForm.listBox1.SelectedIndex = (int)activeEvent.EventType;
            }
        }

        void ShowInitialConditions()
        {
            string box_text = "";

            int count = 0;
            foreach (Condition cond in activeFlag.Conditions)
            {
                string discription = cond.Description;
                box_text += discription;
                count++;
                box_text += ";   \r" ;
            }

            FlagForm.ConditionBox.Text = box_text;
        }

        void HandleInput()
        {
            
                KeyboardState prevKeyState = keyboardState;
                keyboardState = Keyboard.GetState();
                mouseState = Mouse.GetState();

                if (parentGame.GAME_WINDOW.Focused)
                {
                    if (keyboardState.IsKeyDown(Keys.F)
                           && prevKeyState != keyboardState)
                    {

                        GrabActiveFlag();
                    }

                    if (keyboardState.IsKeyDown(Keys.S)
                           && prevKeyState != keyboardState)
                    {

                        GrabEventSpawn();
                    }

                    if (keyboardState.IsKeyDown(Keys.N)
                           && prevKeyState != keyboardState)
                    {
                        AddNewFlag();
                    }

                    if (keyboardState.IsKeyDown(Keys.K)
                           && prevKeyState != keyboardState)
                    {
                        DeleteFlag();
                    }

                    if (mouseState.LeftButton == ButtonState.Released
                        && prevMouseState.LeftButton == ButtonState.Pressed)
                    {

                        SelectNextElement();

                        textque = activeCorner.ToString();
                    }
                }

                prevMouseState = mouseState;
            
        }

        private void DeleteFlag()
        {
            if (activeFlag != null)
                parentGame.myLevel.FlagsList.Remove(activeFlag);
        }

        private void AddNewFlag()
        {
            Vector3 mousePos = GetCursorPos();

            EventFlag flag = new EventFlag(
                new Vector2(mousePos.X + 10, mousePos.Z + 10), // top left
                new Vector2(mousePos.X + 10, mousePos.Z - 10), // top right
                new Vector2(mousePos.X - 10, mousePos.Z + 10), // bot left
                new Vector2(mousePos.X - 10, mousePos.Z - 10));// bot right

            parentGame.myLevel.FlagsList.Add(flag);

        }

        void SelectNextElement()
        {
            if (brush == BrushState.Empty)
                GetNearestFlagCorner();

            if (brush == BrushState.FlagSelected)
                GetNearestFlagCorner();


            //if (brush == BrushState.FlagSelected)
            //    GetNearestEventSpawn();

            //if (brush == BrushState.EventSelected)
            //    GetNearestEventSpawn();
        }

        Vector2 GetCursor2D()
        {
            Vector3 cursorPos = GetCursorPos();
            Vector2 cursor2Dpos = new Vector2(cursorPos.X, cursorPos.Z);

            return cursor2Dpos;
        }

        Vector3 GetCursorPos()
        {
            Ray mouseRay = CalculateCursorRay(parentGame.projectionMatrix, parentGame.viewMatrix);
            Vector3 Pos = GetTerrainVector(mouseRay);
            return Pos;
        }

        void GetNearestFlagCorner()
        {
            Vector2 cursor2Dpos = GetCursor2D();

            float dist = 5;
            float lastDistance = 100;
            int cornerIndex = 0;

            foreach (EventFlag flag in parentGame.myLevel.FlagsList)
            {
                cornerIndex = 0;

                foreach (Vector2 corner in flag.Corners)
                {
                    float thisDistance = Vector2.Distance(cursor2Dpos, corner);
                    if (thisDistance <= dist
                        && thisDistance < lastDistance)
                    {
                        lastDistance = thisDistance;
                        activeFlag = flag;
                        activeCornerIndex = cornerIndex;
                        activeCorner = corner;
                        brush = BrushState.FlagSelected;
                    }
                    cornerIndex += 1;
                }
            }

        }

        //void GetNearestEventSpawn()
        //{
        //    Vector3 cursorPos = GetCursorPos();

        //    float dist = 5;
        //    float lastDistance = 100;

        //    foreach (GameEvent g_event in activeFlag.Events)
        //    {
        //            float thisDistance = Vector3.Distance(cursorPos, g_event.SpawnPoint.Position);
        //            if (thisDistance <= dist
        //                && thisDistance < lastDistance)
        //            {
        //                lastDistance = thisDistance;

        //                activeEvent = g_event;
        //                FlagForm.events_list.SelectedIndex = activeFlag.Events.IndexOf(g_event);

        //                brush = BrushState.EventSelected;
        //            }                
        //    }

        //}

        public void Draw()
        {
            SetDraw3D();
            DrawFlags(effect, "ShadowedScene");
            DrawSpawns(effect, "ShadowedScene");
        }

        void SetDraw3D()
        {
            GraphicsDevice device = parentGame.device;
                        
            device.RenderState.DepthBufferEnable = true;
            device.RenderState.AlphaBlendEnable = false;
            device.RenderState.AlphaTestEnable = false;
        }

        void DrawFlags(Effect effectToUse, String technique)
        {
            GraphicsDevice device = parentGame.device;

            //effectToUse = new BasicEffect(device, new EffectPool());
            
            foreach (EventFlag flag in parentGame.myLevel.FlagsList)
            {
                currentColor.G -= 1;

                VertexPositionColor[] pointList = new VertexPositionColor[5];
                int x = 0;

                #region Get Corners

                foreach (Vector2 Crnr in flag.Corners)
                {
                    Vector3 corner = new Vector3(Crnr.X, 0, Crnr.Y);
                    Vector3 norm;

                    parentGame.terrainEngine.GetHeight(corner, out corner.Y, out norm);
                    corner.Y += 2;

                    pointList[x] = new VertexPositionColor(corner, Color.Red);
                    if (x == 0)
                    {
                        pointList[4] = new VertexPositionColor(corner, Color.Red);
                        currentColor = Color.Red;
                    }
                    else
                        currentColor = Color.Chartreuse;

                    float scale = 0.05f;
                    if (activeFlag == flag
                        && activeCorner == Crnr)
                        scale *= 1.5f;

                    parentGame.currentColor = Color.Brown;

                    parentGame.DrawModel(effectToUse, parentGame.cornerModel, parentGame.tankTextures,

                    Matrix.CreateScale(scale)
                    *
                    Matrix.Identity
                    *
                    Matrix.CreateTranslation(corner),

                    technique, true);



                    x++;
                }

                #endregion

                short[] lineStripIndices = new short[5] { 0, 1, 2, 3, 4 };

                device.RenderState.FillMode = FillMode.WireFrame;
                device.RenderState.CullMode = CullMode.None;

                effectToUse.Begin();
                foreach (EffectPass pass in effectToUse.CurrentTechnique.Passes)
                {
                    pass.Begin();

                    device.VertexDeclaration = new
                        VertexDeclaration(device,
                        VertexPositionColor.VertexElements);

                    device.DrawUserIndexedPrimitives<VertexPositionColor>(
                        PrimitiveType.LineStrip,
                        pointList,
                        0,   // vertex buffer offset to add to each element of the index buffer
                        5,   // number of vertices to draw
                        lineStripIndices,
                        0,   // first index element to read
                        4    // number of primitives to draw
                    );

                    pass.End();

                }
                effectToUse.End();
                //device.RenderState.FillMode = FillMode.Solid;
                device.RenderState.FillMode = FillMode.Solid;
                device.RenderState.CullMode = CullMode.CullClockwiseFace;
            }
        }

        void DrawSpawns(Effect effectToUse, String technique)
        {
            GraphicsDevice device = parentGame.device;

            foreach (EventFlag flag in parentGame.myLevel.FlagsList)
            {
                if (flag == activeFlag)
                {
                    foreach (GameEvent g_event in flag.Events)
                    {

                        VertexPositionColor[] pointList = new VertexPositionColor[2];
                        

                        #region Get Corners

                        Vector3 corner = g_event.SpawnPoint.Position;
                        Vector2 center = FindCenter2D(flag.Corners);

                        Vector3 Center = new Vector3(center.X, 0, center.Y);
                        Vector3 norm;

                        if(parentGame.terrainEngine.IsOnHeightmap(Center))
                            parentGame.terrainEngine.GetHeight(Center, out Center.Y, out norm);

                        pointList[0] = new VertexPositionColor(corner, Color.Red);
                        pointList[1] = new VertexPositionColor(Center, Color.Red);
                        
                        float scale = 0.05f;

                        if (activeEvent == g_event)
                            scale *= 1.5f;

                        parentGame.currentColor = Color.GreenYellow;

                            parentGame.DrawModel(effectToUse, parentGame.cornerModel, parentGame.tankTextures,

                            Matrix.CreateScale(scale)
                            *
                            Matrix.Identity
                            *
                            Matrix.CreateTranslation(corner),

                            technique, true);

                            parentGame.currentColor = Color.Brown;
                        
                        #endregion

                        #region draw lines
                        short[] lineStripIndices = new short[2] { 0, 1 };

                        device.RenderState.FillMode = FillMode.WireFrame;
                        device.RenderState.CullMode = CullMode.None;

                        effectToUse.Begin();
                        foreach (EffectPass pass in effectToUse.CurrentTechnique.Passes)
                        {
                            pass.Begin();

                            device.VertexDeclaration = new
                                VertexDeclaration(device,
                                VertexPositionColor.VertexElements);

                            device.DrawUserIndexedPrimitives<VertexPositionColor>(
                                PrimitiveType.LineStrip,
                                pointList,
                                0,   // vertex buffer offset to add to each element of the index buffer
                                2,   // number of vertices to draw
                                lineStripIndices,
                                0,   // first index element to read
                                1    // number of primitives to draw
                            );

                            pass.End();

                        }
                        effectToUse.End();
                        //device.RenderState.FillMode = FillMode.Solid;
                        device.RenderState.FillMode = FillMode.Solid;
                        device.RenderState.CullMode = CullMode.CullClockwiseFace;

                        #endregion

                    }

                }
            }
        }

        Vector2 FindCenter2D(Vector2[] corners)
        {
            float dist = Vector2.Distance(corners[0], corners[1]);
            Vector2 center = (corners[0]);
            center.X -= dist / 2;

            dist = Vector2.Distance(corners[3], corners[1]);
            center.Y -= dist / 2;

            return center;
        }

        void SetActiveFlag()
        { }

        void SetActiveCorner()
        { }

        void SetActiveEvent()
        { }

        void GrabActiveFlag()
        {
            if (activeFlag != null)
            {
                activeCorner = GetCursor2D();
                UpdateCorner();
            }

        }

        void GrabEventSpawn()
        {

            if (activeEvent != null)
            {
                activeEvent.SpawnPoint.Position = GetCursorPos();
                activeEvent.SpawnPoint.Position.Y += 2;
            }

        }

        void UpdateCorner()
        {
            activeFlag.Corners[activeCornerIndex] = activeCorner;
        }

        void MovingElement()
        {
            if (brush == BrushState.Moving)
            {
                if (activeEvent == null)
                {
                    
                }
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
            Vector3 nearPoint = parentGame.device.Viewport.Unproject(nearSource,
                projectionMatrix, viewMatrix, Matrix.Identity);

            Vector3 farPoint = parentGame.device.Viewport.Unproject(farSource,
                projectionMatrix, viewMatrix, Matrix.Identity);

            // find the direction vector that goes from the nearPoint to the farPoint
            // and normalize it....
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            // and then create a new ray using nearPoint as the source.
            return new Ray(nearPoint, direction);
        }

        Vector3 GetTerrainVector(Ray mRay)
        {

            Vector3 pos = new Vector3(0, 0, 0);
            Nullable<float> curDist = 10000f;

            for (int i = 0; i < parentGame.terrainEngine.vertices.Length - 3; i += 3)
            {
                Nullable<float> rayDist;
                Vector3 firstPos;

                firstPos = parentGame.terrainEngine.vertices[i].Position;

                BoundingSphere sphere = new BoundingSphere(firstPos, 4);
                rayDist = mRay.Intersects(sphere);
                if (rayDist != null)
                {
                    if (rayDist < curDist)
                    {
                        curDist = rayDist;
                        pos = sphere.Center;                       
                    }

                }
            }

            return pos;

        }

    }
}
