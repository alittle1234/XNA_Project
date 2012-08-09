using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace TerrainGame4_0
{
    public class TerrainEditor
    {
        ParentGame parentGame;
        GameplayScreen gameScreen;
        TerrainEngine terrainEngine;
        
        KeyboardState keyboardState;
        MouseState mouseState;

        List<Vector3> posList;
        List<int> vectorList;
        List<int> BigChangeList;
 
        int brushWidth = 5;
        float changeHeight = 1;

        public TerrainEditor(ParentGame parentGame,
            GameplayScreen gameScreen, TerrainEngine terrainEngine)
        {
            this.parentGame = parentGame;
            this.gameScreen = gameScreen;
            this.terrainEngine = terrainEngine;

            posList = new List<Vector3>();
            vectorList = new List<int>();
            BigChangeList = new List<int>();
        }


        void ClickInDrawMode()
        {

            Ray mouseRay = CalculateCursorRay(
                gameScreen.NormalDrawing.projectionMatrix,
                gameScreen.NormalDrawing.viewMatrix);

            posList = new List<Vector3>();
            vectorList = new List<int>();

            Vector3 Pos = GetTerrainVector(mouseRay);

            //drawModeString = "X: " + Pos.X +
            //    "\n " +
            //    "Z: " + Pos.Z;

            GetBrushVerticies(Pos, brushWidth);

            AddToChangeList();

        }

        void MoveCursorScriptMode()
        {



            Ray mouseRay = CalculateCursorRay(
                gameScreen.NormalDrawing.projectionMatrix,
                gameScreen.NormalDrawing.viewMatrix);

            posList = new List<Vector3>();
            vectorList = new List<int>();

            float CHANGE_AMOUNT = 2.0f;
            int WIDTH = 4;

            if (BigChangeList.Count > 0)
                ChangeVerticiesTemp(-CHANGE_AMOUNT);

            BigChangeList = new List<int>();

            Vector3 Pos = GetTerrainVector(mouseRay);

            //drawModeString = "X: " + Pos.X +
            //    "\n " +
            //    "Z: " + Pos.Z;

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
            MouseState mouseState = Mouse.GetState();
            // create 2 positions in screenspace using the cursor position. 0 is as
            // close as possible to the camera, 1 is as far away as possible.
            Vector2 Position = new Vector2(mouseState.X, mouseState.Y);

            Vector3 nearSource = new Vector3(Position, 0f);
            Vector3 farSource = new Vector3(Position, 1f);

            // use Viewport.Unproject to tell what those two screen space positions
            // would be in world space. we'll need the projection matrix and view
            // matrix, which we have saved as member variables. We also need a world
            // matrix, which can just be identity.
            Vector3 nearPoint = gameScreen.NormalDrawing.device.Viewport.Unproject(nearSource,
                projectionMatrix, viewMatrix, Matrix.Identity);

            Vector3 farPoint = gameScreen.NormalDrawing.device.Viewport.Unproject(farSource,
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

            Vector3 pos = new Vector3(0, 0, 0);
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
                terrainEngine.vertices[index].Position.Y += HeightChange * terrainEngine.terrainScale;
                terrainEngine.ReCalcWeights(index);
                terrainEngine.vb.SetData(terrainEngine.vertices);
            }

            terrainEngine.CalculateNormals();

            terrainEngine.SaveHeightMap();

            BigChangeList = new List<int>();
        }

        public void DrawCursor()
        {

            gameScreen.NormalDrawing.spriteBatch.Begin();

            //// use textureCenter as the origin of the sprite, so that the cursor is 
            //// drawn centered around Position.
            //gameScreen.NormalDrawing.spriteBatch.Draw(cursorTexture,
            //    new Vector2(mouseState.X, mouseState.Y),
            //    null, Color.White, 0.0f,
            //    textureCenter,
            //    1.0f, SpriteEffects.None, 0.0f);
            string fps = string.Format("change(9.0): {0}", changeHeight);
            gameScreen.NormalDrawing.spriteBatch.DrawString(
                gameScreen.NormalDrawing.font, fps,
                new Vector2(120, 15), Color.White);

            gameScreen.NormalDrawing.spriteBatch.End();

            
        }

        void DrawLoading()
        {
            //spriteBatch.Begin();

            //// use textureCenter as the origin of the sprite, so that the cursor is 
            //// drawn centered around Position.

            //spriteBatch.Draw(LoadingAnimation[LoadingAnimFrame++],
            //    new Vector2(250, 250),
            //    null, Color.White, 0.0f,
            //    new Vector2(
            //    LoadingAnimation[0].Width / 2, LoadingAnimation[0].Height / 2),
            //    0.3f, SpriteEffects.None, 0.0f);

            //spriteBatch.End();

            //if (LoadingAnimFrame > 15)
            //    LoadingAnimFrame = 0;
        }

        public void HandleInput()
        {
            KeyboardState prevKeyState = keyboardState;
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed)
            {                
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
        }
    }
}
