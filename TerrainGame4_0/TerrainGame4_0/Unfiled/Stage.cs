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
    public class Stage
    {
        public enum StageType
        {
            Loading,
            Working
        };

        public StageType Type;
        public Vector2 Position;
        public Vector2 Size;
        

        public Stage()
        {
            
        }


    }

}
