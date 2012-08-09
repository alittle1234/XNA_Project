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
    class SparkParticleSystem : ParticleSystem
    {
        public SparkParticleSystem(Game game, ContentManager content)
            : base(game, content)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = "Particles\\spark";

            settings.MaxParticles = 100;

            settings.Duration = TimeSpan.FromSeconds(5);

            settings.MinHorizontalVelocity = 8;
            settings.MaxHorizontalVelocity = 25;

            settings.MinVerticalVelocity = 10;
            settings.MaxVerticalVelocity = 25;

            settings.Gravity = new Vector3(0, -10, 0);

            settings.EndVelocity = 0;

            settings.MinColor = Color.Yellow;
            settings.MaxColor = Color.Red;

            settings.MinRotateSpeed = -2;
            settings.MaxRotateSpeed = 2;

            float size = 0.3f;
            settings.MinStartSize = size;
            settings.MaxStartSize = size;

            settings.MinEndSize = .05f;
            settings.MaxEndSize = .1f;

            settings.BlendState = BlendState.AlphaBlend;
        }
    }
}
