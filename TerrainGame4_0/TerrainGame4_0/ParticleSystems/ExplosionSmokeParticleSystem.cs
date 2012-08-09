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
    class ExplosionSmokeParticleSystem : ParticleSystem
    {
        public ExplosionSmokeParticleSystem(Game game, ContentManager content)
            : base(game, content)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = "Particles\\smoke";

            settings.MaxParticles = 200;

            settings.Duration = TimeSpan.FromSeconds(3);

            settings.MinHorizontalVelocity = 2;
            settings.MaxHorizontalVelocity = 15;

            settings.MinVerticalVelocity = 2;
            settings.MaxVerticalVelocity = 25;

            settings.Gravity = new Vector3(0, -9, 0);

            settings.EndVelocity = 0;

            settings.MinColor = Color.SaddleBrown;
            settings.MaxColor = Color.Orange;

            settings.MinRotateSpeed = -2;
            settings.MaxRotateSpeed = 2;

            settings.MinStartSize = 2;
            settings.MaxStartSize = 2;

            settings.MinEndSize = 25;
            settings.MaxEndSize = 40;
        }
    }
}
