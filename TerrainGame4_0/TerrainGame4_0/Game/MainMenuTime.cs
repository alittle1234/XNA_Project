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
    class MainMenuTime : MenuScreen
    {
        #region Initialization

        Texture2D Level, Clear, Locked;
        int columns = 2;
        int rows = 2;

        bool NormalMode = true;

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        /// 
        public MainMenuTime(ParentGame parentGame)
            : base("Terrific Game")
        {
            Initialize(parentGame);
            parentGame.LevelNum = 0;
        }

       

        void Initialize(ParentGame parentGame)
        {
            // Create our menu entries.
            MenuEntry continueEntry = new MenuEntry("Continue");
            continueEntry.LevelNum = 0;
            continueEntry.Selected += SinglePlayerMenuEntrySelected;
            MenuEntries.Add(continueEntry);

            MenuEntry mode = new MenuEntry("Time Trial Mode");
            mode.LevelNum = 0;
            mode.Selected += ModeSelected;
            MenuEntries.Add(mode);

            for (int i = 0; i < 12; ++i)
            {
                MenuEntry me = new MenuEntry((i+1).ToString());
                me.LevelNum = (i + 1);
                me.Selected += SinglePlayerMenuEntrySelected;
                MenuEntries.Add(me);
            }

            MenuEntry exitMenuEntry = new MenuEntry(Resources.Exit);
            exitMenuEntry.Selected += OnCancel;
            MenuEntries.Add(exitMenuEntry);

            Level = parentGame.Content.Load<Texture2D>("Menu\\level");
            Clear = parentGame.Content.Load<Texture2D>("Menu\\clear");
            Locked = parentGame.Content.Load<Texture2D>("Menu\\locked");
        }


        #endregion

        #region Handle Input

        public override void HandleInput(InputState input)
        {
            // Move to the previous menu entry?
            if (input.IsMenuUp(ControllingPlayer))
            {
                if (selectedEntry <= columns)
                    selectedEntry--;
                else
                    selectedEntry -= columns;

                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;

                ScreenManager.menu_change.Play();

                ParentGame game = ScreenManager.Game as ParentGame;
                game.LevelNum = menuEntries[selectedEntry].LevelNum;
            }

            // Move to the next menu entry?
            if (input.IsMenuDown(ControllingPlayer))
            {


                if (selectedEntry == 0)
                    selectedEntry++;
                else if ((selectedEntry >= menuEntries.Count - columns) && (selectedEntry < menuEntries.Count - 1))
                    selectedEntry = menuEntries.Count - 1;
                else
                    selectedEntry += columns;

                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;

                ScreenManager.menu_change.Play();

                ParentGame game = ScreenManager.Game as ParentGame;
                game.LevelNum = menuEntries[selectedEntry].LevelNum;
            }

            // Move to the next menu entry?
            if (input.IsMenuRight(ControllingPlayer))
            {
                selectedEntry++;

                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;

                ScreenManager.menu_change.Play();

                ParentGame game = ScreenManager.Game as ParentGame;
                game.LevelNum = menuEntries[selectedEntry].LevelNum;
            }

            if (input.IsMenuLeft(ControllingPlayer))
            {
                selectedEntry--;

                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;

                ScreenManager.menu_change.Play();

                ParentGame game = ScreenManager.Game as ParentGame;
                game.LevelNum = menuEntries[selectedEntry].LevelNum;
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
            if (!NormalMode)
                game.LevelNum += 20;
            if (!game.LevelStat[game.LevelNum - 1].locked)
            {
                LoadingScreen.Load(ScreenManager, false, e.PlayerIndex,
                                   new GameplayScreen(null, ScreenManager.Game as ParentGame));

                game.Game_State = ParentGame.GameState.OnLevelScreen;
            }
        }

        void ModeSelected(object sender, PlayerIndexEventArgs e)
        {
            NormalMode = (NormalMode) ? false : true;
            selectedEntry = 0;
            if (NormalMode)
                menuEntries[1].Text = "Time Trial Mode";
            else
                menuEntries[1].Text = "Normal Mode";

            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), this);
        }


        /// <summary>
        /// Event handler for when the Live menu entry is selected.
        /// </summary>
        void LiveMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            CreateOrFindSession(NetworkSessionType.PlayerMatch, e.PlayerIndex);
        }


        /// <summary>
        /// Event handler for when the System Link menu entry is selected.
        /// </summary>
        void SystemLinkMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            CreateOrFindSession(NetworkSessionType.SystemLink, e.PlayerIndex);
        }


        /// <summary>
        /// Helper method shared by the Live and System Link menu event handlers.
        /// </summary>
        void CreateOrFindSession(NetworkSessionType sessionType,
                                 PlayerIndex playerIndex)
        {
            // First, we need to make sure a suitable gamer profile is signed in.
            //ProfileSignInScreen profileSignIn = new ProfileSignInScreen(sessionType);

            //// Hook up an event so once the ProfileSignInScreen is happy,
            //// it will activate the CreateOrFindSessionScreen.
            //profileSignIn.ProfileSignedIn += delegate
            //{
            //    GameScreen createOrFind = new CreateOrFindSessionScreen(sessionType);

            //    ScreenManager.AddScreen(createOrFind, playerIndex);
            //};

            //// Activate the ProfileSignInScreen.
            //ScreenManager.AddScreen(profileSignIn, playerIndex);
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
            ScreenManager.Game.Exit();
        }


        #endregion

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;


            float w = ScreenManager.GraphicsDevice.PresentationParameters.BackBufferWidth;
            float h = ScreenManager.GraphicsDevice.PresentationParameters.BackBufferHeight;

            Vector2 position = new Vector2(w * .25f, h * .25f);

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            if (ScreenState == ScreenState.TransitionOn)
                position.X -= transitionOffset * w;
            else
                position.X += transitionOffset * h;

            float buffer = 35 * (w / 800);
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            // CONTINUE
            MenuEntry menuE = menuEntries[0];
            bool isSelec = IsActive && (0 == selectedEntry);
            menuE.Draw(this, position, isSelec, gameTime);

            position.Y += menuE.GetHeight(this) + buffer;

            // MODE
            position.X += w * 0.1f;
            menuE = menuEntries[1];
            isSelec = IsActive && (0 == selectedEntry);
            menuE.Draw(this, position, isSelec, gameTime);

            position.Y += menuE.GetHeight(this) + buffer;
            position.X -= w * 0.1f;

            // Draw each menu entry in turn.
            #region Levels
            int count = menuEntries.Count - 1;
            for (int i = 2; i < count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];

                i -= 2;

                int row = (int)Math.Floor((double)(i / rows));
                int col = i - (int)Math.Floor((double)(columns * (i / columns)));
                Vector2 Pos = new Vector2();
                Pos.X = col * (menuEntry.GetHeight(this) + buffer);
                Pos.Y = row * (menuEntry.GetHeight(this) + buffer);
                Pos += position;

                bool isSelected = IsActive && (i + 2 == selectedEntry);

                menuEntry.Draw(this, Pos, isSelected, gameTime);

                float scl = 0.6f * menuEntry.scale;
                Vector2 origin = new Vector2(Level.Width / 2, Level.Height / 2);
                spriteBatch.Draw(Level, Pos, null, Color.White, 0, origin, scl, SpriteEffects.None, 0.5f);

                ParentGame pg = ScreenManager.Game as ParentGame;
                if (NormalMode)
                {
                    if (pg.LevelStat[i].locked)
                        spriteBatch.Draw(Locked, Pos, null, Color.White, 0, origin, scl, SpriteEffects.None, 0f);
                    else if (pg.LevelStat[i].passed)
                        spriteBatch.Draw(Clear, Pos, null, Color.White, 0, origin, scl, SpriteEffects.None, 0f);
                }
                else
                {
                    if (pg.LevelStat[i + 20].locked)
                        spriteBatch.Draw(Locked, Pos, null, Color.White, 0, origin, scl, SpriteEffects.None, 0f);
                    else if (pg.LevelStat[i + 20].passed)
                        spriteBatch.Draw(Clear, Pos, null, Color.White, 0, origin, scl, SpriteEffects.None, 0f);
                }

                i += 2;

            }
            #endregion


            // EXIT

            position.Y += (rows) * (menuE.GetHeight(this) + buffer);
            position.X -= buffer / 2;
            menuE = menuEntries[count];
            isSelec = IsActive && (count == selectedEntry);
            menuE.Draw(this, position, isSelec, gameTime);

            // Draw the menu title.
            Vector2 titlePosition = new Vector2(w * .8f, h * .2f);
            Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
            Color titleColor = new Color(192, 192, 192, TransitionAlpha);
            float titleScale = 1.25f;

            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, menuTitle, titlePosition, titleColor, 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);

            //DrawStats(h, w);
            if (selectedEntry < menuEntries.Count - 1)
            {
                ParentGame pg = ScreenManager.Game as ParentGame;
                int entry = pg.LevelNum;
                if (entry > 0)
                {
                    if (!NormalMode)
                        entry += 20;

                    string text = "Level " + entry + "\n\n";
                    text += "Most Targets:  " + pg.LevelStat[entry - 1].hiTarKil + "\n";
                    text += "Least Bullets:  " + pg.LevelStat[entry - 1].hiMisRem + "\n";
                    text += "High Score:    " + pg.LevelStat[entry - 1].hiscore + "\n";
                    if(!NormalMode)
                        text += "Fastest Time: " + pg.LevelStat[entry - 1].bestTime + "\n";
                    position = new Vector2(w / 2 + buffer, h / 3);
                    position.X += transitionOffset * w / 2;
                    spriteBatch.DrawString(font, text, position, Color.White, 0,
                                      Vector2.Zero, 1, SpriteEffects.None, 0);
                }
            }

            spriteBatch.End();
        }

        
    }
}
