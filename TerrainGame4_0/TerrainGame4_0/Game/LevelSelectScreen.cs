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
    class LevelSelectScreen : MenuScreen
    {
        #region Initialization

        Texture2D Level, Clear, Locked;
        Texture2D ButtonA, ButtonB;
        int columns = 4;
        int rows = 4;

        bool NormalMode = true;

        string Mode1 = "Normal Mode";
        string Mode2 = "Time Atack";

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        /// 
        public LevelSelectScreen(ParentGame parentGame)
            : base("Artillery Fantastic")
        {
            NormalMode = parentGame.NormalMode;
            Initialize(parentGame);
            parentGame.LevelNum = 0;
            if(NormalMode)
                base.menuTitle = Mode1;
            else
                base.menuTitle = Mode2;
        }



        void Initialize(ParentGame parentGame)
        {
            
            for (int i = 0; i < 12; ++i)
            {
                MenuEntry me = new MenuEntry((i + 1).ToString());
                me.LevelNum = (i + 1);
                me.Selected += SinglePlayerMenuEntrySelected;
                MenuEntries.Add(me);
            }
           

            Level = parentGame.Content.Load<Texture2D>("Menu\\level");
            Clear = parentGame.Content.Load<Texture2D>("Menu\\clear");
            Locked = parentGame.Content.Load<Texture2D>("Menu\\locked");
            ButtonA = parentGame.Content.Load<Texture2D>("Menu\\buttona");
            ButtonB = parentGame.Content.Load<Texture2D>("Menu\\buttonb");
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
            
                if (NormalMode)
                    game.LevelNum = menuEntries[selectedEntry].LevelNum;
                else
                    game.LevelNum = menuEntries[selectedEntry].LevelNum + 20;
        }

        public override void HandleInput(InputState input)
        {
            // Move to the previous menu entry?
            if (input.IsMenuUp(ControllingPlayer))
            {
                selectedEntry -= columns;

                if (selectedEntry < 0)
                    selectedEntry = (selectedEntry + columns) + ( columns * 2);

                ScreenManager.menu_change.Play();

                SetLevelNum();
            }

            // Move to the next menu entry?
            if (input.IsMenuDown(ControllingPlayer))
            {
                selectedEntry += columns;

                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = (selectedEntry - columns) - (columns * 2);

                ScreenManager.menu_change.Play();

                SetLevelNum();
            }

            // Move to the next menu entry?
            if (input.IsMenuRight(ControllingPlayer))
            {
                selectedEntry++;

                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;

                ScreenManager.menu_change.Play();

                SetLevelNum();
            }

            if (input.IsMenuLeft(ControllingPlayer))
            {
                selectedEntry--;

                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;

                ScreenManager.menu_change.Play();

                SetLevelNum();
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


        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            ScreenManager.menu_change.Play();

            //ParentGame pg = ScreenManager.Game as ParentGame;
            //if (pg.LoadingLevel)
            //    pg.LoadingLevel = false;

            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(NormalMode),
                new MainMenuScreen(ScreenManager.Game as ParentGame));
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ParentGame pg = ScreenManager.Game as ParentGame;
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

            Vector2 position = new Vector2(w * .20f, h * .30f);
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
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied);

            // CONTINUE
            MenuEntry menuE = menuEntries[0];
            bool isSelec = IsActive && (0 == selectedEntry);
            

            // Draw each menu entry in turn.
            #region Levels
            int count = menuEntries.Count;
            for (int i = 0; i < count; i++)
            {
                ParentGame pg = ScreenManager.Game as ParentGame;

                MenuEntry menuEntry = menuEntries[i];

                int row = (int)Math.Floor((double)(i / rows));
                int col = i - (int)Math.Floor((double)(columns * (i / columns)));
                Vector2 Pos = new Vector2();
                Pos.X = col * (menuEntry.GetHeight(this) + buffer);
                Pos.Y = row * (menuEntry.GetHeight(this) + buffer);
                Pos += position;

                bool isSelected = IsActive && (i == selectedEntry);
                if (NormalMode)
                    if (pg.LevelStat[i].locked)
                        menuEntry.myColor = Color.DarkGray;
                
                if(!NormalMode)
                    if (pg.LevelStat[i + 20].locked)
                        menuEntry.myColor = Color.DarkGray;

                menuEntry.Draw(this, Pos, isSelected, gameTime);

                float scl = 0.6f * menuEntry.scale;
                Vector2 origin = new Vector2(Level.Width / 2, Level.Height / 2);
                spriteBatch.Draw(Level, Pos, null, Color.White, 0, origin, scl * textScale, SpriteEffects.None, 0.5f);

               
                if (NormalMode)
                {
                    if (pg.LevelStat[i].locked)
                        spriteBatch.Draw(Locked, Pos, null, Color.White, 0, origin, scl * textScale, SpriteEffects.None, 0f);
                    else if (pg.LevelStat[i].passed)
                        spriteBatch.Draw(Clear, Pos, null, Color.White, 0, origin, scl * textScale, SpriteEffects.None, 0f);
                }
                else
                {
                    if (pg.LevelStat[i + 20].locked)
                        spriteBatch.Draw(Locked, Pos, null, Color.White, 0, origin, scl * textScale, SpriteEffects.None, 0f);
                    else if (pg.LevelStat[i + 20].passed)
                        spriteBatch.Draw(Clear, Pos, null, Color.White, 0, origin, scl * textScale, SpriteEffects.None, 0f);
                }

            }
            #endregion

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


            string back = "Play";           
            xW = font.MeasureString(back).X;
            yW = font.MeasureString(back).Y;
            titlePosition = new Vector2(w * .2f, h * .8f);            
            titleColor = Color.LightGray;
            titleScale = 1.25f;

            titlePosition.X -= transitionOffset * 500;

            titleOrigin = new Vector2(ButtonA.Width / 2, ButtonA.Height / 2);
            spriteBatch.Draw(ButtonA, titlePosition, null, Color.White, 0, titleOrigin, titleScale * textScale * 0.4f, SpriteEffects.None, 0);

            titlePosition.X += ButtonA.Width * 1.1f * textScale.X;
            titleOrigin = new Vector2(xW / 2, yW / 2);
            spriteBatch.DrawString(font, back, titlePosition, titleColor, 0,
                                   titleOrigin, titleScale * textScale, SpriteEffects.None, 0);

            back = "Back";
            xW = font.MeasureString(back).X;
            yW = font.MeasureString(back).Y;
            titlePosition.X += xW * 2f;            
            titleColor = Color.LightGray;
            titleScale = 1.25f;

            titlePosition.X -= transitionOffset * 500;

            titleOrigin = new Vector2(ButtonA.Width / 2, ButtonA.Height / 2);
            spriteBatch.Draw(ButtonB, titlePosition, null, Color.White, 0, titleOrigin, titleScale * textScale * 0.4f, SpriteEffects.None, 0);

            titlePosition.X += ButtonA.Width * 1.1f * textScale.X;
            titleOrigin = new Vector2(xW / 2, yW / 2);
            spriteBatch.DrawString(font, back, titlePosition, titleColor, 0,
                                   titleOrigin, titleScale * textScale, SpriteEffects.None, 0);

            #region Stats
            //DrawStats(h, w);

            ParentGame parGame = ScreenManager.Game as ParentGame;
            int entry = parGame.LevelNum;
            if (entry > 0 && (!parGame.LevelStat[entry - 1].locked))
                {
                        int add = 0;
                        if (!NormalMode)
                            add = -20;
                    string text = "Level " + (entry + add) + "\n\n";
                    text += "Most Targets:  " + parGame.LevelStat[entry - 1].hiTarKil + "\n";
                    text += "Least Bullets:  " + parGame.LevelStat[entry - 1].hiMisRem + "\n";
                    text += "High Score:    " + parGame.LevelStat[entry - 1].hiscore + "\n";
                    if (!NormalMode)
                        text += "Fastest Time: " + parGame.LevelStat[entry - 1].bestTime + "\n";
                    position = new Vector2(w * 0.6f + buffer, h / 3);
                    position.X += transitionOffset * w / 2;
                    spriteBatch.DrawString(font, text, position, Color.LightGray, 0,
                                      Vector2.Zero, 1, SpriteEffects.None, 0);
                }
            #endregion

            spriteBatch.End();
        }


    }
}
