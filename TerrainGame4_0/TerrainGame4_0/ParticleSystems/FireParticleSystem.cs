#region File Description
//-----------------------------------------------------------------------------
// FireParticleSystem.cs
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
#endregion

namespace TerrainGame4_0
{
    /// <summary>
    /// Custom particle system for creating a flame effect.
    /// </summary>
    class FireParticleSystem : ParticleSystem
    {
        public FireParticleSystem(Game game, ContentManager content)
            : base(game, content)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = "Particles\\fire";

            settings.MaxParticles = 2400;

            settings.Duration = TimeSpan.FromSeconds(2);

            settings.DurationRandomness = 2;

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 3;

            settings.MinVerticalVelocity = -10;
            settings.MaxVerticalVelocity = 7;

            // Set gravity upside down, so the flames will 'fall' upward.
            settings.Gravity = new Vector3(0, 3.5f, 0);

            //settings.MinColor = new Color(255, 255, 255, 10);
            //settings.MaxColor = new Color(255, 255, 255, 40);
            Color yel = new Color(Color.Yellow.R, Color.Yellow.G, Color.Yellow.B, 50);
            Color red = new Color(Color.Red.R, Color.Red.G, Color.Red.B, 50);
            settings.MinColor = yel;
            settings.MaxColor = red;

            settings.MinStartSize = 3;
            settings.MaxStartSize = 4;

            settings.MinEndSize = 0;
            settings.MaxEndSize = 0.5f;

            settings.BlendState = BlendState.NonPremultiplied;
        }
    }
}
