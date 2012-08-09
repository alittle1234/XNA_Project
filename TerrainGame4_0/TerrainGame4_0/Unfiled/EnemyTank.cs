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
    class EnemyTank : GameObject
    {

        public EnemyTank(ParentGame game, Model modelObj, Texture2D[] modelTextures) 
            : base( game, modelObj, modelTextures )
        {
            
        }

        override public void Update(float amount, GameTime gameTime)
        {
        }
                

        override public void UpdateWorldMatrix()
        {
            worldMatrix =
               Matrix.CreateFromYawPitchRoll(
               rotation.Y,
               rotation.X,
               rotation.Z)
               *
               Matrix.CreateScale(scale)
               *
               Matrix.CreateTranslation(position);
        }
    }
}
