#region File Description
//-----------------------------------------------------------------------------
// ExplosionParticleSystem.cs
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
    /// Custom particle system for creating the fiery part of the explosions.
    /// </summary>
    class RingExplosionParticleSystem : ParticleSystem
    {
        public RingExplosionParticleSystem(Game game, ContentManager content)
            : base(game, content)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = "Particles\\ring_explosion";

            settings.MaxParticles = 100;

            settings.Duration = TimeSpan.FromSeconds(0.4);
            settings.DurationRandomness = 1;

            settings.MinHorizontalVelocity = -15;
            settings.MaxHorizontalVelocity = 30;// 30;

            settings.MinVerticalVelocity = -10;
            settings.MaxVerticalVelocity = 20;// 20;

            settings.EndVelocity = 1;

            settings.Gravity = new Vector3(0, -3, 0);

            Color yel = new Color(Color.Yellow.R, Color.Yellow.G, Color.Yellow.B, 250);
            Color red = new Color(Color.White.R, Color.White.G, Color.White.B, 150);
            settings.MinColor = yel;
            settings.MaxColor = red;

            settings.MinRotateSpeed = -1;
            settings.MaxRotateSpeed = 1;

            settings.MinStartSize = 1;
            settings.MaxStartSize = 2;

            settings.MinEndSize = 2;
            settings.MaxEndSize = 4;

            settings.BlendState = BlendState.NonPremultiplied;
        }
    }
}
