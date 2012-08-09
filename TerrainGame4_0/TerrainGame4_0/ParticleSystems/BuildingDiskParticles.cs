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
    class BuildingDiskParticles : ParticleSystem
    {
        public BuildingDiskParticles(Game game, ContentManager content)
            : base(game, content)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = "Particles\\smoke2";

            settings.MaxParticles = 100;

            settings.Duration = TimeSpan.FromSeconds(5);

            settings.MinHorizontalVelocity = 17;
            settings.MaxHorizontalVelocity = 23;

            settings.MinVerticalVelocity = 2;
            settings.MaxVerticalVelocity = 5;

            settings.Gravity = new Vector3(0, -4, 0);

            settings.EndVelocity = 0;

            settings.MinColor = Color.WhiteSmoke;
            settings.MaxColor = Color.LightGray;

            settings.MinRotateSpeed = -2;
            settings.MaxRotateSpeed = 2;

            float scale = 5;
            settings.MinStartSize = 5 * scale;
            settings.MaxStartSize = 7 * scale;

            settings.MinEndSize = 8 * scale;
            settings.MaxEndSize = 12 * scale;

            settings.BlendState = BlendState.NonPremultiplied;
        }
    }
}
