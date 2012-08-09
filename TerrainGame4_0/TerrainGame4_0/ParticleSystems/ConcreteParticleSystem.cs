#region File Description
//-----------------------------------------------------------------------------
// ExplosionSmokeParticleSystem.cs
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
    /// Custom particle system for creating the smokey part of the explosions.
    /// </summary>
    class ConcreteParticleSystem : ParticleSystem
    {
        public ConcreteParticleSystem(Game game, ContentManager content)
            : base(game, content)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = "Particles\\concrete";

            settings.MaxParticles = 500;

            settings.Duration = TimeSpan.FromSeconds(3);

            settings.MinHorizontalVelocity = 8;
            settings.MaxHorizontalVelocity = 35;

            settings.MinVerticalVelocity = 10;
            settings.MaxVerticalVelocity = 25;

            settings.Gravity = new Vector3(0, -10, 0);

            settings.EndVelocity = 0;

            settings.MinColor = Color.DarkGray;
            settings.MaxColor = Color.White;

            settings.MinRotateSpeed = -6;
            settings.MaxRotateSpeed = 6;

            float size = 0.5f;
            settings.MinStartSize = .1f;
            settings.MaxStartSize = .6f;

            settings.MinEndSize = .4f;
            settings.MaxEndSize = .6f;

            settings.BlendState = BlendState.NonPremultiplied;
        }
    }
}
