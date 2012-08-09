#region File Description
//-----------------------------------------------------------------------------
// PauseMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;
#endregion

namespace TerrainGame4_0
{
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume or quit.
    /// </summary>
    class LoadingGameScreen : GameScreen
    {
        #region Fields

        NetworkSession networkSession;
        
        #endregion

        #region Initialization

        public event EventHandler<PlayerIndexEventArgs> Cancelled;
        LevelLoader Stage;

        /// <summary>
        /// Constructor.
        /// </summary>
        public LoadingGameScreen(LevelLoader Stage)
            : base()
        {

            this.Stage = Stage;
            // Flag that there is no need for the game to transition
            // off when the pause menu is on top of it.
            IsPopup = true;

            transOnTime = TimeSpan.FromSeconds(1);
            transOffTime = TimeSpan.FromSeconds(1);

            Temporary = true;
           
        }


        #endregion

        bool isexiting = false;
        ScreenState screenstate = ScreenState.TransitionOn;
        TimeSpan transOffTime, transOnTime;

        public virtual void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                      bool coveredByOtherScreen, bool loading)
        {


            if (!loading)
            {
                // If the screen is going away to die, it should transition off.
                screenstate = ScreenState.TransitionOff;

                if (!Updatetransition(gameTime, transOffTime, 1))
                {
                    //// When the transition finishes, remove the screen.
                    //ScreenManager.RemoveScreen(this);
                }
            }
            else if (coveredByOtherScreen)
            {
                // If the screen is covered by another, it should transition off.
                if (Updatetransition(gameTime, transOffTime, 1))
                {
                    // Still busy transitioning.
                    screenstate = ScreenState.TransitionOff;
                }
                else
                {
                    // Transition finished!
                    screenstate = ScreenState.Hidden;
                }
            }
            else
            {
                // Otherwise the screen should transition on and become active.
                if (Updatetransition(gameTime, transOnTime, -1))
                {
                    // Still busy transitioning.
                    screenstate = ScreenState.TransitionOn;
                }
                else
                {
                    // Transition finished!
                    screenstate = ScreenState.Active;
                }
            }
        }

        float transPos = 1;
        bool Updatetransition(GameTime gameTime, TimeSpan time, int direction)
        {
            // How much should we move by?
            float transitionDelta;

            if (time == TimeSpan.Zero)
                transitionDelta = 1;
            else
                transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                          time.TotalMilliseconds);

            // Update the transition position.
            transPos += transitionDelta * direction;

            // Did we reach the end of the transition?
            if (((direction < 0) && (transPos <= 0)) ||
                ((direction > 0) && (transPos >= 1)))
            {
                transPos = MathHelper.Clamp(transPos, 0, 1);
                return false;
            }

            // Otherwise we are still busy transitioning.
            return true;
        }


        #region Handle Input
        public override void HandleInput(InputState input)
        {
            PlayerIndex playerIndex;

            

            // We pass in our ControllingPlayer, which may either be null (to
            // accept input from any player) or a specific index. If we pass a null
            // controlling player, the InputState helper returns to us which player
            // actually provided the input. We pass that through to our Accepted and
            // Cancelled events, so they can tell which player triggered them.
            if (input.IsMenuSelect(ControllingPlayer, out playerIndex))
            {
                //// Raise the accepted event, then exit the message box.
                //if (Accepted != null)
                //    Accepted(this, new PlayerIndexEventArgs(playerIndex));


                

                ExitScreen();
            }
            else if (input.IsMenuCancel(ControllingPlayer, out playerIndex))
            {
                // Raise the cancelled event, then exit the message box.
                if (Cancelled != null)
                    Cancelled(this, new PlayerIndexEventArgs(playerIndex));

                ScreenManager.menu_enter.Play();

                ExitScreen();
            }
        }
        

        #endregion

        #region Draw


        /// <summary>
        /// Draws the pause menu screen. This darkens down the gameplay screen
        /// that is underneath us, and then chains to the base MenuScreen.Draw.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {            
            if(Stage.CheckProgress() >= 1.0f)
                // Raise the cancelled event, then exit the message box.
                //if (Cancelled != null)
                {
                    //Cancelled(this, new PlayerIndexEventArgs(PlayerIndex.One));
                    ExitScreen();
                }

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;


            float w = ScreenManager.GraphicsDevice.PresentationParameters.BackBufferWidth;
            float h = ScreenManager.GraphicsDevice.PresentationParameters.BackBufferHeight;

            Vector2 position = new Vector2(w * .20f, h * .30f);

            position.X -= transPos * 300;

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied);
            spriteBatch.DrawString(font, "LOADING", position, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        
        #endregion
    }
}
