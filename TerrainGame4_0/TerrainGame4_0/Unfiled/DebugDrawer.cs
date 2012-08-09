using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TerrainGame4_0
{

    public class DebugDrawer : DrawableGameComponent
    {
        BasicEffect basicEffect;
        List<VertexPositionColor> vertexData;

        public DebugDrawer(Game game)
            : base(game)
        {
            this.vertexData = new List<VertexPositionColor>();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Initialize()
        {
            base.Initialize();

            basicEffect = new BasicEffect(this.GraphicsDevice);
            //GraphicsDevice. = new VertexDeclaration(GraphicsDevice, VertexPositionColor.VertexElements);
        }

        public override void Draw(GameTime gameTime)
        {
            if (vertexData.Count == 0 || !Enabled) return;

            ParentGame playGround = this.Game as ParentGame;

            this.basicEffect.AmbientLightColor = Vector3.One;
            this.basicEffect.View = playGround.viewMatrix;
            this.basicEffect.Projection = playGround.projectionMatrix;
            this.basicEffect.VertexColorEnabled = true;


            foreach (EffectPass pass in this.basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip,
                    vertexData.ToArray(), 0, vertexData.Count - 1);

            }

            vertexData.Clear();

            base.Draw(gameTime);
        }


        #region Alex's addition for Body Renderer

        public void DrawShape(List<Vector3> shape, Color color)
        {
            if (vertexData.Count > 0)
            {
                Vector3 v = vertexData[vertexData.Count - 1].Position;
                vertexData.Add(new VertexPositionColor(v, new Color(0, 0, 0, 0)));
                vertexData.Add(new VertexPositionColor(shape[0], new Color(0, 0, 0, 0)));
            }

            foreach (Vector3 p in shape)
            {
                vertexData.Add(new VertexPositionColor(p, color));
            }
        }

        public void DrawShape(List<Vector3> shape, Color color, bool closed)
        {
            DrawShape(shape, color);

            Vector3 v = shape[0];
            vertexData.Add(new VertexPositionColor(v, color));
        }

        public void DrawShape(List<VertexPositionColor> shape)
        {
            if (vertexData.Count > 0)
            {
                Vector3 v = vertexData[vertexData.Count - 1].Position;
                vertexData.Add(new VertexPositionColor(v, new Color(0, 0, 0, 0)));
                vertexData.Add(new VertexPositionColor(shape[0].Position, new Color(0, 0, 0, 0)));
            }

            foreach (VertexPositionColor vps in shape)
            {
                vertexData.Add(vps);
            }
        }

        public void DrawShape(VertexPositionColor[] shape)
        {
            if (vertexData.Count > 0)
            {
                Vector3 v = vertexData[vertexData.Count - 1].Position;
                vertexData.Add(new VertexPositionColor(v, new Color(0, 0, 0, 0)));
                vertexData.Add(new VertexPositionColor(shape[0].Position, new Color(0, 0, 0, 0)));
            }

            foreach (VertexPositionColor vps in shape)
            {
                vertexData.Add(vps);
            }
        }

        public void DrawShape(List<VertexPositionColor> shape, bool closed)
        {
            DrawShape(shape);

            VertexPositionColor v = shape[0];
            vertexData.Add(v);
        }

        #endregion



    }

}