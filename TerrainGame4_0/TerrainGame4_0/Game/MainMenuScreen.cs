#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Net;
#endregion

namespace TerrainGame4_0
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        #region Initialization

        Texture2D Level, Clear, Locked;
        int columns = 4;
        int rows = 4;

        bool NormalMode = true;

        string Mode1 = "Normal Mode";
        string Mode2 = "Time Atack";
        Texture2D selectTexture;
        Texture2D ButtonA, ButtonB;

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        /// 
        public MainMenuScreen(ParentGame parentGame)
            : base("Artillery Fantastic")
        {
            NormalMode = parentGame.NormalMode;
            Initialize(parentGame);
            parentGame.LevelNum = 0;
            selectTexture = parentGame.MenuVariables.MenuSelect;
            if(NormalMode)
                base.menuTitle = Mode1;
            else
                base.menuTitle = Mode2;
        }



        void Initialize(ParentGame parentGame)
        {
            // Create our menu entries.
            MenuEntry continueEntry = new MenuEntry("Continue");
            continueEntry.LevelNum = 0;
            continueEntry.Selected += SinglePlayerMenuEntrySelected;
            MenuEntries.Add(continueEntry);

            MenuEntry mode = new MenuEntry(Mode2);
            if (!NormalMode)
                mode.Text = Mode1;
            mode.LevelNum = 0;
            mode.Selected += ModeSelected;
            MenuEntries.Add(mode);

            MenuEntry levelSelect = new MenuEntry("Level Select");
            levelSelect.Selected += LevelSelectSelected;
            MenuEntries.Add(levelSelect);

            MenuEntry options = new MenuEntry("Options");
            //levelSelect.Selected += LevelSelectSelected;
            MenuEntries.Add(options);

            MenuEntry exitMenuEntry = new MenuEntry(Resources.Exit);
            exitMenuEntry.Selected += OnCancel;
            MenuEntries.Add(exitMenuEntry);

            Level = parentGame.Content.Load<Texture2D>("Menu\\level");
            Clear = parentGame.Content.Load<Texture2D>("Menu\\clear");
            Locked = parentGame.Content.Load<Texture2D>("Menu\\locked");
            ButtonA = parentGame.Content.Load<Texture2D>("Menu\\buttona");
            ButtonB = parentGame.Content.Load<Texture2D>("Menu\\buttonb");

           
            //if (parentGame.LoadingLevel)
            //    parentGame.LoadingLevel = false;
                        
        }


        #endregion

        public override void LoadContent()
        {
            ParentGame pg = ScreenManager.Game as ParentGame;
            NormalMode = pg.NormalMode;
        }

        #region Handle Input

        void SetLevelNum()
        {
            ParentGame game = ScreenManager.Game as ParentGame;
            
            game.LevelNum = 0;            
        }

        public override void HandleInput(InputState input)
        {
            // Move to the previous menu entry?
            if (input.IsMenuUp(ControllingPlayer))
            {
                selectedEntry--;

                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;

                ScreenManager.menu_change.Play();
            }

            // Move to the next menu entry?
            if (input.IsMenuDown(ControllingPlayer))
            {
                selectedEntry++;

                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;

                ScreenManager.menu_change.Play();
            }

            // Move to the next menu entry?
            if (input.IsMenuRight(ControllingPlayer))
            {
                selectedEntry++;

                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;

                ScreenManager.menu_change.Play();
            }

            if (input.IsMenuLeft(ControllingPlayer))
            {
                selectedEntry--;

                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;

                ScreenManager.menu_change.Play();
            }

            // Accept or cancel the menu? We pass in our ControllingPlayer, which may
            // either be null (to accept input from any player) or a specific index.
            // If we pass a null controlling player, the InputState helper returns to
            // us which player actually provided the input. We pass that through to
            // OnSelectEntry and OnCancel, so they can tell which player triggered them.
            PlayerIndex playerIndex;

            if (input.IsMenuSelect(ControllingPlayer, out playerIndex))
            {
                OnSelectEntry(selectedEntry, playerIndex);
            }
            else if (input.IsMenuCancel(ControllingPlayer, out playerIndex))
            {
                OnCancel(playerIndex);
            }
        }

        /// <summary>
        /// Event handler for when the Single Player menu entry is selected.
        /// </summary>
        void SinglePlayerMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ParentGame game = ScreenManager.Game as ParentGame;            
            if (!game.LevelStat[game.LevelNum - 1].locked)
            {
                LoadingScreen.Load(ScreenManager, false, e.PlayerIndex,
                                   new GameplayScreen(null, ScreenManager.Game as ParentGame));
                
            }
        }

        void LevelSelectSelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(NormalMode),
                new LevelSelectScreen(ScreenManager.Game as ParentGame));
        }


        void ModeSelected(object sender, PlayerIndexEventArgs e)
        {
            NormalMode = (NormalMode) ? false : true;
            selectedEntry = 0;

            if (NormalMode)
            {
                menuEntries[1].Text = Mode2;
                base.menuTitle = Mode1;
            }
            else
            {
                menuEntries[1].Text = Mode1;
                base.menuTitle = Mode2;
            }

            ParentGame pg = ScreenManager.Game as ParentGame;
            pg.NormalMode = NormalMode;
            pg.LevelNum = 0;
            pg.GetLevelNum();

            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(NormalMode), this);
        }



        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            MessageBoxScreen confirmExitMessageBox =
                                    new MessageBoxScreen(Resources.ConfirmExitSample);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);

            ScreenManager.menu_change.Play();

        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ParentGame pg=ScreenManager.Game as ParentGame;
            pg.Level_Loader.Stop();
            ScreenManager.Game.Exit();
        }


        #endregion

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;


            float w = ScreenManager.GraphicsDevice.PresentationParameters.BackBufferWidth;
            float h = ScreenManager.GraphicsDevice.PresentationParameters.BackBufferHeight;

            Vector2 position = new Vector2(w * .20f, h * .20f);
            Vector2 textScale = new Vector2(w / 1272, h / 720);

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            if (ScreenState == ScreenState.TransitionOn)
                position.X -= transitionOffset * w;
            else
                position.X += transitionOffset * h;

            float buffer = 35 * (w / 800);
            float rightBuff = w * .3f;
            float rightOffset = 500;
            float offset;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            // CONTINUE
            MenuEntry menuE = menuEntries[0];
            bool isSelec = IsActive && (0 == selectedEntry);
            menuE.scale *= 1.2f;
            offset = font.MeasureString(menuE.Text).X / 2 * menuE.scale * textScale.X;
            position.X = rightBuff - offset - transitionOffset * rightOffset;
            if (isSelec)
            {
                DrawSelected(spriteBatch, position, offset);
            }
            menuE.Draw(this, position, isSelec, gameTime);

            position.Y += menuE.GetHeight(this) + buffer - 30;

            // MODE
            //position.X += w * 0.1f;
            menuE = menuEntries[1];
            isSelec = IsActive && (1 == selectedEntry);
            offset = font.MeasureString(menuE.Text).X / 2 * textScale.X;
            position.X = rightBuff - offset * menuE.scale - transitionOffset * rightOffset;
            if (isSelec)
            {
                DrawSelected(spriteBatch, position, offset);
            }
            menuE.Draw(this, position, isSelec, gameTime);

            position.Y += menuE.GetHeight(this) + buffer - 10;

            // level select
            //position.X += w * 0.1f;
            menuE = menuEntries[2];
            isSelec = IsActive && (2 == selectedEntry);
            offset = font.MeasureString(menuE.Text).X / 2 * textScale.X;
            position.X = rightBuff - offset * menuE.scale - transitionOffset * rightOffset;
            if (isSelec)
            {
                DrawSelected(spriteBatch, position, offset);
            }
            menuE.Draw(this, position, isSelec, gameTime);

            position.Y += menuE.GetHeight(this) + buffer - 10;
            

            // options
            //position.X += w * 0.1f;
            menuE = menuEntries[3];
            isSelec = IsActive && (3 == selectedEntry);
            offset = font.MeasureString(menuE.Text).X / 2 * textScale.X;
            position.X = rightBuff - offset * menuE.scale - transitionOffset * rightOffset;
            if (isSelec)
            {
                DrawSelected(spriteBatch, position, offset);
            }
            menuE.Draw(this, position, isSelec, gameTime);

            position.Y += menuE.GetHeight(this) + buffer - 10;
           

           
            // EXIT
             menuE = menuEntries[4];
            position.Y += (menuE.GetHeight(this) + buffer - 20);
           
            isSelec = IsActive && (4 == selectedEntry);
            offset = font.MeasureString(menuE.Text).X / 2 * textScale.X;
            position.X = rightBuff - offset * menuE.scale - transitionOffset * rightOffset;
            if (isSelec)
            {
                DrawSelected(spriteBatch, position, offset);
            }
            menuE.Draw(this, position, isSelec, gameTime);

            // TITLE            
            float xW = font.MeasureString(menuTitle).X;
            float yW = font.MeasureString(menuTitle).Y;
            Vector2 titlePosition = new Vector2(w - (xW / 2) - 100, h * .12f);
            Vector2 titleOrigin = new Vector2(xW / 2, yW / 2);
            Color titleColor = new Color(152, 152, 152, 155);
            float titleScale = 1.25f;

            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, menuTitle, titlePosition, titleColor, 0,
                                   titleOrigin, titleScale * textScale, SpriteEffects.None, 0);

            

            #region Stats
            //DrawStats(h, w);
            if (selectedEntry == 0)
            {
                ParentGame pg = ScreenManager.Game as ParentGame;
                int entry = pg.LevelNum;
                if (entry > 0 && (!pg.LevelStat[entry - 1].locked) )
                {
                        int add = 0;
                        if (!NormalMode)
                            add = -20;
                    string text = "Level " + (entry + add) + "\n\n";
                    text += "Most Targets:  " + pg.LevelStat[entry - 1].hiTarKil + "\n";
                    text += "Least Bullets:  " + pg.LevelStat[entry - 1].hiMisRem + "\n";
                    text += "High Score:    " + pg.LevelStat[entry - 1].hiscore + "\n";
                    if (!NormalMode)
                        text += "Fastest Time: " + pg.LevelStat[entry - 1].bestTime + "\n";
                    position = new Vector2(w * 0.6f + buffer, h / 3);
                    position.X += transitionOffset * w / 2;
                    spriteBatch.DrawString(font, text, position, Color.LightGray, 0,
                                      Vector2.Zero, 1, SpriteEffects.None, 0);
                }
            }
            #endregion

            spriteBatch.End();
        }

        void DrawSelected(SpriteBatch spriteBatch, Vector2 position, float offset)
        {
            float w = ScreenManager.GraphicsDevice.PresentationParameters.BackBufferWidth;
            float h = ScreenManager.GraphicsDevice.PresentationParameters.BackBufferHeight;
            Vector2 textScale = new Vector2(w / 1272, h / 720);
            //position.Y -= selectTexture.Height / 2;
            position.X += offset;

            Vector2 origin = new Vector2(selectTexture.Width * 0.8f, selectTexture.Height / 2);
            Vector2 scale = new Vector2(w / 1272, h / 720);
            scale.Y *= 1.2f;
            scale.X *= 1.35f;
            spriteBatch.Draw(selectTexture, position, null,
                             Color.White, 0, origin, scale, SpriteEffects.None, 0);

            origin = new Vector2(ButtonA.Width / 2, ButtonA.Height / 2);
            position.X += w * 0.05f;
            spriteBatch.Draw(ButtonA, position, null, Color.White, 0, origin, scale * 0.4f, SpriteEffects.None, 0);
        }
    }
}
