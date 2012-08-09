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
    public class ResourceList
    {

        public Dictionary<String, Texture2D> Tex2dList;

        public struct ModelResource
        {
            public Model Model_rez;
            public Texture2D[] Textures_rez;
        }
        public Dictionary<String, ModelResource> ModelList;
        
       
        public ResourceList()
        {
            Tex2dList = new Dictionary<string, Texture2D>();
            ModelList = new Dictionary<string, ModelResource>();
           
        }
    }
}
