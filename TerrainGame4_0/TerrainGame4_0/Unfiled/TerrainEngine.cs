using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace TerrainGame4_0
{
    public class TerrainEngine
    {
        public struct TerrainArguments
        {
            public Texture2D heightMap,
                terrainTexture1,
                terrainTexture2,
                terrainTexture3,
                terrainTexture4,
                decalTexture,
                skyTexture;

            public Model skyDome;

            public float terrainScale;
        }

        public struct VertexPositionNormalColored
        {
            public Vector3 Position;
            public Color Color;
            public Vector3 Normal;

            public static int SizeInBytes = 7 * 4;
            public static VertexElement[] VertexElements = new VertexElement[]
            {
                new VertexElement( 0, VertexElementFormat.Vector3,  VertexElementUsage.Position, 0 ),
                new VertexElement( sizeof(float) * 3, VertexElementFormat.Color,  VertexElementUsage.Color, 0 ),
                new VertexElement( sizeof(float) * 4, VertexElementFormat.Vector3,  VertexElementUsage.Normal, 0 ),
             };
        }

        public struct VertexMultitextured : IVertexType

        {
            public Vector3 Position;
            public Vector2 TextureCoordinate;
            public Vector3 Normal;
            public Vector4 TexWeights;

            public static int SizeInBytes = (3 + 2 + 3 + 4) * sizeof(float);
            public readonly static VertexDeclaration vDeclaration = new VertexDeclaration
                (

                new VertexElement( 0, VertexElementFormat.Vector3,
                     VertexElementUsage.Position, 0 ),

                new VertexElement( sizeof(float) * 3, VertexElementFormat.Vector2,
                     VertexElementUsage.TextureCoordinate, 0 ),

                new VertexElement( sizeof(float) * 5, VertexElementFormat.Vector3,
                     VertexElementUsage.Normal, 0 ),

                new VertexElement( sizeof(float) * 8, VertexElementFormat.Vector4,
                     VertexElementUsage.TextureCoordinate, 1 )
                     );


            VertexDeclaration IVertexType.VertexDeclaration { get { return vDeclaration; } }

        }

        public struct VertexDecaled : IVertexType
        {
            public Vector3 Position;
            public Vector2 TextureCoordinate;
            public Vector3 Normal; 
           

            public static int SizeInBytes = (3 + 2 + 3) * sizeof(float);
            public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
                (
                new VertexElement( 0, VertexElementFormat.Vector3,
                     VertexElementUsage.Position, 0 ),
                new VertexElement( sizeof(float) * 3, VertexElementFormat.Vector4,
                     VertexElementUsage.TextureCoordinate, 0 ),
                new VertexElement( sizeof(float) * 5, VertexElementFormat.Vector3,
                     VertexElementUsage.Normal, 0 )              
            );

                VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }

        }

        ParentGame parentGame;
        GraphicsDevice device;
        KeyboardState keyboardState;

        public Matrix worldMatrix;
        
        public VertexMultitextured[] vertices;
        public int[] indices;
        //public VertexDeclaration myVertexDeclaration;

        public VertexBuffer vb;
        IndexBuffer ib;

        //public VertexDecaled[] decalVertices;
        //public int[] decalIndices;
        //public VertexDeclaration decalVertexDeclaration;
        //public Vector3 decalPosition = new Vector3(20, 20, -20);
        //public Vector3 decalRotation;
        public Texture2D decalTexture;
        //public int decalWidth, decalHeight;
        //private float decalScale;
        public List<Decal> terrainDecals;

        private int terrainWidth = 0;
        private int terrainHeight = 0;
        public float[,] heightData;

        float top, mid, topD2, midD2;
        public float minHeight, maxHeight;

        public Texture2D heightMap, terrainTexture, terrainTexture0,
            terrainTexture1, terrainTexture2;

        public float terrainScale;

        public Vector3 heightmapPosition;
        public int heightmapWidth, heightmapHeight;

        public Model skyDome;
        public Texture2D cloudMap;

        public int mLeft, mTop, mPosZ;  //-------DEBUG
        //-------------------------------------------------------

        StreamWriter sw;

        public TerrainEngine(ParentGame game, MouseCamera mouseCam, TerrainArguments terrainArgs)
        {
            this.device = game.GraphicsDevice;
            parentGame = game;

            heightMap = terrainArgs.heightMap;
            terrainTexture = terrainArgs.terrainTexture1;
            terrainTexture0 = terrainArgs.terrainTexture2;
            terrainTexture1 = terrainArgs.terrainTexture3;
            terrainTexture2 = terrainArgs.terrainTexture4;
            decalTexture = terrainArgs.decalTexture;
            terrainScale = terrainArgs.terrainScale;

            skyDome = terrainArgs.skyDome;
            cloudMap = terrainArgs.skyTexture;
//#if!XBOX
//            sw = new StreamWriter("C:/Temp_Heights" + parentGame.LevelNum + ".jpeg");
//#endif
           
        }

        public void LoadContent()
        {
            terrainDecals = new List<Decal>();
            //decalVertexDeclaration = new VertexDeclaration(VertexDecaled.VertexElements);

            LoadHeightData(heightMap);

            SetUpVertices();
            SetUpIndices();            
            CalculateNormals();

            vb = new VertexBuffer(device, typeof(VertexMultitextured), vertices.Length, BufferUsage.WriteOnly);
            vb.SetData(vertices);           

            ib = new IndexBuffer(device, typeof(int), indices.Length, BufferUsage.WriteOnly);
            ib.SetData(indices);

            

            worldMatrix = Matrix.Identity;
            
        }

        public void Update()
        {  

        }

        public void Unload()
        {
            vb.Dispose();
            ib.Dispose();
            //heightMap.Dispose();
        }

        void Draw()
        {            
          
        }

        public void DrawSkyDome(Matrix currentViewMatrix, Vector3 Position, Matrix ProjectionMatrix, String Technique)
        {
            device.DepthStencilState = DepthStencilState.None;

            Matrix[] modelTransforms = new Matrix[skyDome.Bones.Count];
            skyDome.CopyAbsoluteBoneTransformsTo(modelTransforms);

            Matrix wMatrix = Matrix.CreateTranslation(0, -0.3f, 0)
                * Matrix.CreateScale(100)
                * Matrix.CreateTranslation(Position);

            foreach (ModelMesh mesh in skyDome.Meshes)
            {
                foreach (Effect currentEffect in mesh.Effects)
                {
                    Matrix worldMatrix = modelTransforms[mesh.ParentBone.Index] * wMatrix;

                    //SetEffectParameters(currentEffect, Technique, cloudMap, worldMatrix, false, false);
                    currentEffect.Parameters["xSky"].SetValue(true);
                    
                    //currentEffect.Parameters["xEnableLighting"].SetValue(false);
                }
                mesh.Draw();
            }
            device.DepthStencilState = DepthStencilState.Default;
        }

        //void SetEffectParameters(String technique)
        //{
        //    effect.CurrentTechnique = effect.Techniques[technique];
           
        //    //effect.Parameters["Texture"].SetValue(terrainTexture);
        //    //effect.Parameters["xTexture1"].SetValue(terrainTexture2);
        //    //effect.Parameters["xTexture2"].SetValue(terrainTexture0);
        //   // effect.Parameters["xTexture3"].SetValue(terrainTexture1);
            
        //    effect.Parameters["xCamerasViewProjection"].SetValue(viewMatrix * projectionMatrix);

        //    effect.Parameters["xTexture"].SetValue(terrainTexture);
        //    effect.Parameters["xShadowMap"].SetValue(parentGame.shadowMap);
        //    effect.Parameters["xSolidBrown"].SetValue(false);
        //    effect.Parameters["xWorld"].SetValue(worldMatrix);
        //    effect.Parameters["xLightPos"].SetValue(parentGame.lightPos);
        //    effect.Parameters["xLightPower"].SetValue(parentGame.lightPower);
        //    effect.Parameters["xAmbient"].SetValue(parentGame.ambientPower);

        //    effect.Parameters["xLightsViewProjection"].SetValue(parentGame.lightViewProjection);

        //    if (technique == "ShadowMap")
        //    {
        //        // DRAW TARGET
              
        //        // Draw the dude model    
                
        //    }
        //    else if (technique == "ShadowedScene")
        //    {
        //        // DRAW TARGET
               

                
        //        //effect.Parameters["xWorldViewProjection"].SetValue(worldMatrix * viewMatrix * projectionMatrix);
        //    }

        //}

        private void LoadHeightData(Texture2D heightMap)
        {
            terrainWidth = heightMap.Width;
            terrainHeight = heightMap.Height;

            Color[] heightMapColors = new Color[terrainWidth * terrainHeight];
            heightMap.GetData(heightMapColors);            

            heightData = new float[terrainWidth, terrainHeight];
            for (int x = 0; x < terrainWidth; x++)
            {
                for (int y = 0; y < terrainHeight; y++)
                {
                    heightData[x, y] = heightMapColors[x + y * terrainWidth].R / 5.0f * 2;
                }
            }

            heightmapWidth = heightData.GetLength(0) - 1;  // <- size of tex2d heightmap
            heightmapHeight = heightData.GetLength(1) - 1;

            heightmapPosition = new Vector3(0, 0, -heightmapHeight * terrainScale);

        }

        public void SaveHeightMap()
        {
            Texture2D tempHeightMap = heightMap;

            Color[] heightMapColors = new Color[terrainWidth * terrainHeight];           

          

            for (int x = 0; x < terrainWidth; x++)
            {
                for (int y = 0; y < terrainHeight; y++)
                {
                    int index = x + y * terrainWidth;
                    heightMapColors[index].R =
                        (byte)(vertices[index].Position.Y * 5.0f / 2);
                    heightMapColors[index].G =
                        (byte)(vertices[index].Position.Y * 5.0f / 2);
                    heightMapColors[index].B =
                       (byte)(vertices[index].Position.Y * 5.0f / 2);
                    heightMapColors[index].A =
                       (byte)(vertices[index].Position.Y * 5.0f / 2);
                }
            }

            
            
            //heightMap.GetData(heightMapColors);
            tempHeightMap.SetData(heightMapColors);
//            #if!XBOX
//            tempHeightMap.SaveAsJpeg(sw.BaseStream, heightMap.Width, heightMap.Height);
//#endif
            
        }

        private void SetUpVertices()
        {
            // find min/max height
            minHeight = float.MaxValue;
            maxHeight = float.MinValue;
            for (int x = 0; x < terrainWidth; x++)
            {
                for (int y = 0; y < terrainHeight; y++)
                {
                    if (heightData[x, y] < minHeight)
                        minHeight = heightData[x, y];
                    if (heightData[x, y] > maxHeight)
                        maxHeight = heightData[x, y];
                }
            }

            vertices = new VertexMultitextured[terrainWidth * terrainHeight];
            // abs ( height - point ) / range

            // get position for each vertex
            // get tex coords for each vertex
            // get tex weights for each vertex
            top =  (maxHeight - (maxHeight * 0.4f)); // 30 - 12 = 18
            mid = (maxHeight - (maxHeight * 0.6f));  // 30 - 18 = 12
            topD2 = top / 2;                         // 18 / 2 = 9
            midD2 = mid / 2;                         // 12 / 2 = 6

            for (int x = 0; x < terrainWidth; ++x)
            {
                for (int y = 0; y < terrainHeight; ++y)
                {
                    vertices[x + y * terrainWidth].Position = new Vector3(
                        x * terrainScale,
                        heightData[x, y],
                        -y * terrainScale);

                    vertices[x + y * terrainWidth].TextureCoordinate.X =
                        (float)x / 20;//heightMap.Width * terrainScale * 4;// maxHeight; //30.0f;
                    vertices[x + y * terrainWidth].TextureCoordinate.Y =
                        (float)y / 20;//heightMap.Height * terrainScale * 4;//maxHeight; //30.0f;
                    // texture scale

                    // abs ( height - point ) / range
                    #region texcoords
                    vertices[x + y * terrainWidth].TexWeights.X = 
                        MathHelper.Clamp(
                        1.0f - Math.Abs(heightData[x, y] - minHeight) / (midD2), 0, 1);

                    vertices[x + y * terrainWidth].TexWeights.Y =
                        MathHelper.Clamp(
                        1.0f - Math.Abs(heightData[x, y] - midD2) / (topD2), 0, 1);

                    vertices[x + y * terrainWidth].TexWeights.Z =
                        MathHelper.Clamp(
                        1.0f - Math.Abs(heightData[x, y] - top) / midD2, 0, 1);

                    vertices[x + y * terrainWidth].TexWeights.W =
                        MathHelper.Clamp(
                        1.0f - Math.Abs(heightData[x, y] - maxHeight) / topD2, 0, 1);

                    float total = vertices[x + y * terrainWidth].TexWeights.X;
                    total += vertices[x + y * terrainWidth].TexWeights.Y;
                    total += vertices[x + y * terrainWidth].TexWeights.Z;
                    total += vertices[x + y * terrainWidth].TexWeights.W;

                    vertices[x + y * terrainWidth].TexWeights.X /= total;
                    vertices[x + y * terrainWidth].TexWeights.Y /= total;
                    vertices[x + y * terrainWidth].TexWeights.Z /= total;
                    vertices[x + y * terrainWidth].TexWeights.W /= total;
                    #endregion texcoords
                }
            }

            //myVertexDeclaration = new VertexDeclaration(
            //    VertexMultitextured.VertexElements);
        }

        
        public void ReCalcWeights(int index)
        {            
            float HEIGHT = vertices[index].Position.Y;

            vertices[index].TexWeights.X = 
                        MathHelper.Clamp(
                        1.0f - Math.Abs(HEIGHT - minHeight) / (midD2), 0, 1);

            vertices[index].TexWeights.Y =
                        MathHelper.Clamp(
                        1.0f - Math.Abs(HEIGHT - midD2) / (topD2), 0, 1);

            vertices[index].TexWeights.Z =
                        MathHelper.Clamp(
                        1.0f - Math.Abs(HEIGHT - top) / midD2, 0, 1);

            vertices[index].TexWeights.W =
                        MathHelper.Clamp(
                        1.0f - Math.Abs(HEIGHT - maxHeight) / topD2, 0, 1);

            float total = vertices[index].TexWeights.X;
            total += vertices[index].TexWeights.Y;
            total += vertices[index].TexWeights.Z;
            total += vertices[index].TexWeights.W;

            vertices[index].TexWeights.X /= total;
            vertices[index].TexWeights.Y /= total;
            vertices[index].TexWeights.Z /= total;
            vertices[index].TexWeights.W /= total;
        }

        private void SetUpIndices()
        {
            indices = new int[(terrainWidth - 1) * (terrainHeight - 1) * 6];

            int counter = 0;
            for (int y = 0; y < terrainHeight - 1; y++)
            {
                for (int x = 0; x < terrainWidth - 1; x++)
                {
                    int lowerLeft = x + y * terrainWidth;
                    int lowerRight = (x + 1) + y * terrainWidth;
                    int topLeft = x + (y + 1) * terrainWidth;
                    int topRight = (x + 1) + (y + 1) * terrainWidth;

                    indices[counter++] = topLeft;
                    indices[counter++] = lowerRight;
                    indices[counter++] = lowerLeft;

                    indices[counter++] = topLeft;
                    indices[counter++] = topRight;
                    indices[counter++] = lowerRight;
                }
            }

        }

        public void CalculateNormals()
        {
            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal = new Vector3(0, 0, 0);

            for (int i = 0; i < indices.Length / 3; i++)
            {
                int index1 = indices[i * 3];
                int index2 = indices[i * 3 + 1];
                int index3 = indices[i * 3 + 2];

                Vector3 side1 = vertices[index1].Position - vertices[index3].Position;
                Vector3 side2 = vertices[index1].Position - vertices[index2].Position;
                Vector3 normal = Vector3.Cross(side1, side2);

                vertices[index1].Normal += normal;
                vertices[index2].Normal += normal;
                vertices[index3].Normal += normal;
            }

            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal.Normalize();

        }


        public void ExtractVertices( int decalWidth, int decalHeight, Vector3 decalPosition, 
            out VertexDecaled[] decalVertices)
        {
            decalVertices = new VertexDecaled[decalWidth * decalHeight];

            // for recalculating z-value
            float subValue = heightmapHeight * terrainScale;  // size of bitmap

            float leftPos, upPos, downPos;
            leftPos = decalPosition.X - decalWidth / 2 ;            
            upPos = decalPosition.Z + decalWidth / 2 ;
            downPos = Math.Abs(decalPosition.Z) - decalWidth / 2; 

            for (int x = 0; x < decalWidth; ++x)
            {
                for (int y = 0; y < decalHeight; ++y)
                {

                    decalVertices[x + y * decalWidth].Position = new Vector3(0,0,0);


                    decalVertices[x + y * decalWidth].Position.X =
                        x  + (leftPos);

                    decalVertices[x + y * decalWidth].Position.Z =
                        -y - downPos;

                   // decalVertices[x + y * decalWidth].Position.Y =
                    if (IsOnHeightmap(decalVertices[x + y * decalWidth].Position))
                    {
                        GetHeight(decalVertices[x + y * decalWidth].Position,
                        out decalVertices[x + y * decalWidth].Position.Y,
                        out decalVertices[x + y * decalWidth].Normal);


                        decalVertices[x + y * decalWidth].Position.Y += 0.001f;
                        decalVertices[x + y * decalWidth].Position.Z += 0.001f;
                    }


                    decalVertices[x + y * decalWidth].TextureCoordinate.X =
                        (float)((float)x / (float)decalWidth); // texture scale
                    decalVertices[x + y * decalWidth].TextureCoordinate.Y =
                        (float)((float)y / (float)decalWidth); // texture scale
                    // texture scale
                }
            }
            //decalVertexDeclaration = new VertexDeclaration(
            //    TerrainEngine.VertexDecaled.VertexElements);
           
        }

        public void ExtractIndices(int decalWidth, int decalHeight, out int[] decalIndices)
        {
            decalIndices = new int[(decalWidth - 1) * (decalHeight - 1) * 6];

            int counter = 0;
            for (int y = 0; y < decalHeight - 1; y++)
            {
                for (int x = 0; x < decalWidth - 1; x++)
                {
                    int lowerLeft = x + y * decalWidth;
                    int lowerRight = (x + 1) + y * decalWidth;
                    int topLeft = x + (y + 1) * decalWidth;
                    int topRight = (x + 1) + (y + 1) * decalWidth;

                    decalIndices[counter++] = topLeft;
                    decalIndices[counter++] = lowerRight;
                    decalIndices[counter++] = lowerLeft;

                    decalIndices[counter++] = topLeft;
                    decalIndices[counter++] = topRight;
                    decalIndices[counter++] = lowerRight;
                }
            }

        }

        public void CreateNewDecal(Vector3 position)
        {
            terrainDecals.Add(new Decal(decalTexture, position));

            terrainDecals[terrainDecals.Count - 1].Load();

            ExtractVertices(terrainDecals[terrainDecals.Count - 1].decalWidth,
                terrainDecals[terrainDecals.Count - 1].decalHeight,
                terrainDecals[terrainDecals.Count - 1].decalPosition,
                out terrainDecals[terrainDecals.Count - 1].decalVertices);

            ExtractIndices(terrainDecals[terrainDecals.Count - 1].decalWidth,
                terrainDecals[terrainDecals.Count - 1].decalHeight,
                out terrainDecals[terrainDecals.Count - 1].decalIndices);
            
            
        }

        public bool IsOnHeightmap(Vector3 position)
        {
           
            Vector3 positionOnHeightmap = position - heightmapPosition;

            return (positionOnHeightmap.X > 2 &&
                positionOnHeightmap.X  < (heightmapWidth * terrainScale) - 5 &&
                positionOnHeightmap.Z  > 5 &&
                positionOnHeightmap.Z < (heightmapHeight * terrainScale) - 5);
        }


       // heightMapInfo.GetHeightAndNormal
        //            (cameraPosition, out minimumHeight, out normal);
        public void GetHeight(Vector3 position, out float height, out Vector3 normal)
        {
            
            Vector3 positionOnHeightmap = position - heightmapPosition;  // size of entire terrain

            // to get an interger in heightdata[]
            float subValue = heightmapHeight * terrainScale;  // size of bitmap
            float left_f, top_f;
           
            left_f =  positionOnHeightmap.X / terrainScale;
            top_f = (subValue  - positionOnHeightmap.Z) / terrainScale;

            

            int left, top;
            left = 1 + (int)left_f;
            top = 1 + (int)top_f; 
            
            mLeft = left;
            mTop = top;

            mPosZ = (int)positionOnHeightmap.Z;

            float xNormalized = (positionOnHeightmap.X
                % terrainScale) / terrainScale;
            float zNormalized = ( (subValue -(positionOnHeightmap.Z))
                % terrainScale) / terrainScale;


            float topHeight = MathHelper.Lerp(
                heightData[left, top],
                heightData[left + 1, top],
                xNormalized);

            float first, second;
            first = heightData[left, top + 1];
            second = heightData[left + 1, top + 1];

            float bottomHeight = MathHelper.Lerp(
                first,
                second,
                xNormalized);

            height = MathHelper.Lerp(topHeight, bottomHeight, zNormalized);

            //vertices[ y*w + x].Normal
            // We'll repeat the same process to calculate the normal.
            int width = heightMap.Width;
            Vector3 topNormal = Vector3.Lerp(
                vertices[(top * width) + left].Normal,
                vertices[left + 1 + (top * width)].Normal,
                xNormalized);

            Vector3 bottomNormal = Vector3.Lerp(
                vertices[left + ((top + 1) * width)].Normal,
                vertices[left + 1 + ((top + 1) * width)].Normal,
                xNormalized);

           

            normal = Vector3.Lerp(topNormal, bottomNormal, zNormalized);
            normal.Normalize();           
            
        }

        //private VertexPositionNormalTexture[] SetUpTerrainVertices()
        //{
        //    VertexPositionNormalTexture[] terrainVertices = new VertexPositionNormalTexture[terrainWidth * terrainLength];

        //    for (int x = 0; x < terrainWidth; x++)
        //    {
        //        for (int y = 0; y < terrainLength; y++)
        //        {
        //            terrainVertices[x + y * terrainWidth].Position = new Vector3(x, heightData[x, y], -y);
        //            terrainVertices[x + y * terrainWidth].TextureCoordinate.X = (float)x / 30.0f;
        //            terrainVertices[x + y * terrainWidth].TextureCoordinate.Y = (float)y / 30.0f;
        //        }
        //    }

        //    return terrainVertices;
        //}



        public void DrawTerrain(Effect effect)
        {
            device.SetVertexBuffer(vb);
            device.Indices = ib;

            int noVertices = vb.VertexCount;
            int noTriangles = ib.IndexCount / 3;  //<-- must be correct

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, noVertices, 0, noTriangles);
            }

           
        }

        public void DrawDecals(Effect effect)
        {

            
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                foreach (Decal decal in terrainDecals)
                {
                    //new VertexDeclaration(device, VertexDecaled.VertexElements);
                    
                    device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList,
                        decal.decalVertices, //vertex data []
                        0,          //vertex offset
                        decal.decalVertices.Length,    //num vertexes
                        decal.decalIndices,    //index data []
                        0,          //index offset
                        (decal.decalIndices.Length / 3));    //num primitives

                }
            }

        }


    }
}
