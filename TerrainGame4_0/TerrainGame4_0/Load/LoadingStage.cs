using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

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
    public class LoadingStage : Stage
    {

        float loadProgress = 0.0f;
        int totalCount, currentCount;
        public bool ContentLoaded = false;
        bool InitialLoad = false;
        ParentGame parentGame;

        Dictionary<String, bool> Tex2dLoadList;
        Dictionary<String, bool> ModelLoadList;

        ResourceList theList;
        Effect MODEL_EFFECT;
        GraphicsDevice DEVICE;

        ContentManager ContMang;

        public Thread backgroundThread;
        EventWaitHandle backgroundThreadExit;




        //public LoadingStage(ResourceList res_list, ParentGame parentGame)
        //{
        //    ContMang = parentGame.Content;
        //    theList = res_list;
        //    MODEL_EFFECT = parentGame.objEffect;
        //    DEVICE = parentGame.device;

        //    Type = StageType.Loading;

        //    Tex2dLoadList = new Dictionary<String, bool>();
        //    ModelLoadList = new Dictionary<String, bool>();


        //    SetThreads();
           

        //   // LoadContent();
        //   // EndLoading();
        //}

        /// <summary>
        /// Previously, an old resourse list could be used and modified with new content, 
        /// however, I have changed it to create a new ContentManager for each load, and 
        /// consequently, a new resource list is required for each load... until i fix it.
        /// </summary>
        /// <param name="res_list"></param>
        /// <param name="parentGame"></param>
        /// <param name="initialLoad"></param>
        public LoadingStage(ResourceList res_list, ParentGame parentGame, bool initialLoad)
        {
            if (initialLoad)
                ContMang = parentGame.Content;
            else
                ContMang = new ContentManager(parentGame.Services, "Content");

            theList = res_list;
            MODEL_EFFECT = parentGame.objEffect;
            DEVICE = parentGame.device;

            InitialLoad = initialLoad;
            this.parentGame = parentGame;

            Type = StageType.Loading;

            Tex2dLoadList = new Dictionary<String, bool>();
            ModelLoadList = new Dictionary<String, bool>();


            SetThreads();


            // LoadContent();
            // EndLoading();
        }

        public void SetThreads()
        {
            backgroundThread = new Thread(BackgroundWorkerThread);
            backgroundThreadExit = new ManualResetEvent(false);
        }

        public void AddToTex2DList(List<String> contentList)
        {
            bool alreadyIN = false;
            foreach (String texture in contentList)
            {
                foreach (KeyValuePair<String, bool> text2dKey in Tex2dLoadList)
                    if (texture == text2dKey.Key)
                        alreadyIN = true;

                if (!alreadyIN)
                    Tex2dLoadList.Add(texture, false);
            }

        }

        public void AddToModelList(List<String> contentList)
        {
            bool alreadyIN = false;
            foreach (String model in contentList)
            {
                foreach (KeyValuePair<String, bool> modelKey in ModelLoadList)
                    if (model == modelKey.Key)
                        alreadyIN = true;

                if (!alreadyIN)
                    ModelLoadList.Add(model, false);
            }

        }

        public void UpdateReferenceList()
        {
            // update reference list by cross checking with load list
            // removing any references to variables that do not need to be in memory
            // that may have already been loaded

            List<string> keyList = new List<string>();

            foreach (KeyValuePair<String, Texture2D> dictKey in theList.Tex2dList)
                if (!Tex2dLoadList.ContainsKey(dictKey.Key))
                    keyList.Add(dictKey.Key);

            foreach (string key in keyList)
            {
                //if(key.Contains("Terrain"))
                    //theList.Tex2dList[key].();                
                theList.Tex2dList.Remove(key);
            }

            keyList = new List<string>();

            foreach (KeyValuePair<String, ResourceList.ModelResource> dictKey in theList.ModelList)
                if (!ModelLoadList.ContainsKey(dictKey.Key))
                    keyList.Add(dictKey.Key);

            foreach (string key in keyList)
                theList.ModelList.Remove(key);
        }

        

        public void LoadContent()
        {
            if (backgroundThread != null)
            {
               // loadStartTime = gameTime;
                
                backgroundThread.Start();
                backgroundThread.Name = "Loading INIT:" + InitialLoad.ToString() + parentGame.LevelNum;
            }
        }

        public void BackgroundWorkerThread()
        {
            UpdateReferenceList();

            totalCount = Tex2dLoadList.Count + ModelLoadList.Count + 1;
            currentCount = 0;
            loadProgress = (float)(currentCount / totalCount);

            #region TEXTURE2D
            for (int count = 0; count < Tex2dLoadList.Count; ++count)
            {

                if (!backgroundThreadExit.WaitOne(10))
                {
                    LoadTexture(Tex2dLoadList.ElementAt(count));
                }

            }
            #endregion TEXTURE2D

            #region MODEL
            for (int count = 0; count < ModelLoadList.Count; ++count)
            {

                if (!backgroundThreadExit.WaitOne(50))
                {
                    LoadModel(ModelLoadList.ElementAt(count));
                }

            }

            #endregion MODEL

            #region LAST FUNTION

            if (!backgroundThreadExit.WaitOne(10))
            {
                LastFunction();
            }
            

            #endregion MODEL
        }

        void LastFunction()
        {
            if (!InitialLoad)
                Thread.Sleep(1000);
            SetProgress();
        }

        void LoadTexture(KeyValuePair<String, bool> texKey)
        {

            if (ContMang != null && texKey.Key != null)
            {
                // has not been loaded
                if (Tex2dLoadList[texKey.Key] == false)
                {
                    // is not in resource dictionary
                    if (!theList.Tex2dList.ContainsKey(texKey.Key))
                    {

                        theList.Tex2dList[texKey.Key] = ContMang.Load<Texture2D>(texKey.Key);
                        Tex2dLoadList[texKey.Key] = true;
                        SetProgress();
                    }
                    else
                    // resource has a reference
                    // is it loaded?
                    {
                        if (theList.Tex2dList[texKey.Key] == null)
                        {
                            theList.Tex2dList[texKey.Key] = ContMang.Load<Texture2D>(texKey.Key);
                            Tex2dLoadList[texKey.Key] = true;
                            SetProgress();
                        }
                        else
                        {
                            // resource has reference and is probably loaded
                            // set to TRUE
                            Tex2dLoadList[texKey.Key] = true;
                            SetProgress();
                        }
                    }
                }
            }

        }

        void LoadModel(KeyValuePair<String, bool> model)
        {
            string model_name = model.Key;
            ResourceList.ModelResource ModRez = new ResourceList.ModelResource();
            
            Texture2D[] textureAry;

            // has not been loaded
            if (ModelLoadList[model_name] == false)
            {
                // is NOT in resource dictionary
                if (!theList.ModelList.ContainsKey(model_name))
                {
                    //set early, already in thread, model could take time
                    ModelLoadList[model_name] = true;
                    

                    ModRez.Model_rez = ProcessLoadModel(model_name,
                        out textureAry);
                    ModRez.Textures_rez = textureAry;

                    theList.ModelList[model_name] = ModRez;

                    SetProgress();
                    
                }
                else
                // resource has a reference
                // is it loaded?
                {
                    if (theList.ModelList[model_name].Model_rez == null)
                    {
                        //set early, already in thread, model could take time
                        ModelLoadList[model_name] = true;
                        

                        ModRez.Model_rez = ProcessLoadModel(model_name,
                        out textureAry);
                        ModRez.Textures_rez = textureAry;

                        theList.ModelList[model_name] = ModRez;

                        SetProgress();
                    }
                    else
                    {
                        // resource has reference and is probably loaded
                        // set to TRUE
                        ModelLoadList[model_name] = true;
                        SetProgress();
                    }
                }
            }

        }

        Model ProcessLoadModel(String assetName, out Texture2D[] textures )
        {
            // create model
            // find num of textures
            // assign textures to array
            // clone current effect into model effects
            Model newModel;

            if (!parentGame.GraphicsDevice.IsDisposed && assetName != null)
            {
                //  TRY CATCH BLOCK BECAUSE ERRORS ARE THROWN DURING LOAD AND SIMULTANEOUS QUIT...
                //  APPARENTLY DOSE NOT CATCH ALL EXEPTIONS AT ALL TIMES, EVEN WHEN TOLD...
                
                try
                {
                    newModel = ContMang.Load<Model>(assetName);

                    int i = 0;
                    foreach (ModelMesh mesh in newModel.Meshes)
                        foreach (BasicEffect currentEffect in mesh.Effects)
                            i++;

                    textures = new Texture2D[i];


                    i = 0;
                    foreach (ModelMesh mesh in newModel.Meshes)
                        foreach (BasicEffect currentEffect in mesh.Effects)
                            textures[i++] = currentEffect.Texture;

                    foreach (ModelMesh mesh in newModel.Meshes)
                        foreach (ModelMeshPart meshPart in mesh.MeshParts)
                        {
                            try
                            {
                                meshPart.Effect = MODEL_EFFECT.Clone();
                            }
                            catch (AccessViolationException e)
                            {
                                textures = null;
                                return null;
                            }
                        }

                    return newModel;
                }
                catch (AccessViolationException e)
                {
                    textures = null;
                    return null;
                }
                catch (ObjectDisposedException e)
                {
                    textures = null;
                    return null;
                }
                catch (ContentLoadException e)
                {
                    textures = null;
                    return null;
                }
            }
            else
            {
                textures = null;
                return null;
            }
        }

        
        void UpdateContent()
        {

        }

        /// <summary>
        /// Adds One to Progress .  Called after each item is loaded.
        /// </summary>
        void SetProgress()
        {
            ++currentCount;
            loadProgress = (float)((float)currentCount / (float)totalCount);

            //LoadList = new Dictionary<string, bool>(Tex2dLoadList);
            //foreach (KeyValuePair<String, bool> texKey in LoadList)
            //    if (LoadList[texKey.Key] == true)
            //        count++;

            //LoadList = new Dictionary<string,bool>(ModelLoadList);
            //foreach (KeyValuePair<String, bool> modKey in ModelLoadList)
            //    if (LoadList[modKey.Key] == true)
            //        count++;

            //return (float)((float)count / ((float)Tex2dLoadList.Count + (float)ModelLoadList.Count));
        }

        public float CheckProgress()
        {
            return (float)loadProgress;
        }

        public void EndLoading()
        {
            // Signal the background thread to exit, then wait for it to do so.
            if (backgroundThread != null)
            {
                if(backgroundThread.IsAlive)
                {
                    //Thread.Sleep(50);

                    //' Abort newThread.
                    Console.WriteLine("Aborting (" + parentGame.LevelNum + ") Loading thread.");
                    try
                    {
                        backgroundThread.Abort();
                    }
                    catch
                    {
                        //do nothing
                    }

                    //' Wait for the thread to terminate.
                    backgroundThread.Join();
                    Console.WriteLine("Loading thread (" +parentGame.LevelNum+") terminated - EndLoading() exiting.");
                }

            }
        }
        

    }
}
