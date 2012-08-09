using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class Decal
    {

        public TerrainEngine.VertexDecaled[] decalVertices;
        public int[] decalIndices;
        public VertexDeclaration decalVertexDeclaration;
        public Vector3 decalPosition;
        //public Vector3 decalRotation;
        public Texture2D decalTexture;
        public int decalWidth, decalHeight;
        //private float decalScale;

        public Decal(Texture2D decaltexture, Vector3 decalposition)
        {
            decalTexture = decaltexture;
            decalPosition = decalposition;

            
        }

        public void Load()
        {
            decalWidth = (int)(decalTexture.Width) / 15;
            decalHeight = (int)(decalTexture.Height) / 15;
        }

        public void Update()
        {

        }

    }
}
