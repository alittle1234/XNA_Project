#region File Description
//-----------------------------------------------------------------------------
// ProjectileTrailParticleSystem.cs
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
    /// Custom particle system for leaving smoke trails behind the rocket projectiles.
    /// </summary>
    class ProjectileTrailParticleSystem : ParticleSystem
    {
        public ProjectileTrailParticleSystem(Game game, ContentManager content)
            : base(game, content)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = "Particles\\smoke2";

            settings.MaxParticles = 1000;

            settings.Duration = TimeSpan.FromSeconds(3);

            settings.DurationRandomness = 1f;

            settings.EmitterVelocitySensitivity = 0.2f;

            settings.MinHorizontalVelocity = 1;
            settings.MaxHorizontalVelocity = 1;

            settings.MinVerticalVelocity = -1;
            settings.MaxVerticalVelocity = -1;

            settings.MinColor = Color.Gray;
            settings.MaxColor = Color.LightGray;

            settings.MinRotateSpeed = -4;
            settings.MaxRotateSpeed = -4;

            settings.MinStartSize = 2;
            settings.MaxStartSize = 2;

            settings.MinEndSize = 5;
            settings.MaxEndSize = 15;

            settings.BlendState = BlendState.NonPremultiplied;
        }
    }
}
