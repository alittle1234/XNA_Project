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
    public class GameObject
    {

        public ParentGame parentGame;
        public GameTime objGameTime;
        
        public Model model = null;       

        public Texture2D[] textures;

        public Matrix worldMatrix;

        public Vector3 position;

        public Vector3 rotation = Vector3.Zero;        
        public Vector3 velocity = Vector3.Zero;
       
        public float scale = 1.0f;       
        
        public float FacingDirection = MathHelper.Pi;
        public Matrix objFacingMatrix;

        public Vector3 normal;

        public BoundingSphere boundSphere;
        // set true/false in game class
        public bool alive = false;


        public GameObject(ParentGame game, Model modelObj, Texture2D[] modelTextures)
        {           
            parentGame = game;

            model = modelObj;
            textures = modelTextures;

            boundSphere = new BoundingSphere();
            boundSphere.Radius = 5;
        }

        public void LoadContent()
        {

        }

       

        public void Update(GameTime gameTime)
        {
            objGameTime = gameTime;
            float timeDifference = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;

            
            //virtual
            //updates non-movement components
            Update(timeDifference, gameTime);

            //virtual
            UpdateWorldMatrix();

            if (position.Y < -50)
                Destroy();

            
            boundSphere.Center = position;
            
        }

       
        virtual public void Update(float amount, GameTime gameTime)
        {

        }

        virtual public void Destroy()
        {

        }  

        virtual public void UpdateWorldMatrix()
        {
            worldMatrix = 
            //Matrix.CreateFromYawPitchRoll(
            //            FacingDirection,      //float
            //            rotation.X,           //float
            //            rotation.Z)           //float
            Matrix.CreateScale(scale)            
            * 
            objFacingMatrix
            * 
            Matrix.CreateTranslation(position) ;
           
        }

    }
}
