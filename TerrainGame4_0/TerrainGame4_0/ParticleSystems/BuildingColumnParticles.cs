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
    class BuildingColumnParticles : ParticleSystem
    {
        public BuildingColumnParticles(Game game, ContentManager content)
            : base(game, content)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = "Particles\\smoke2";

            settings.MaxParticles = 100;

            settings.Duration = TimeSpan.FromSeconds(5);

            settings.MinHorizontalVelocity = 5;
            settings.MaxHorizontalVelocity = 10;

            settings.MinVerticalVelocity = 1;
            settings.MaxVerticalVelocity = 15;

            settings.Gravity = new Vector3(0, -7, 0);

            settings.EndVelocity = 0;

            settings.MinColor = Color.DarkGray;
            settings.MaxColor = Color.NavajoWhite;

            settings.MinRotateSpeed = -2;
            settings.MaxRotateSpeed = 2;

            float scale = 4;
            settings.MinStartSize = 5 * scale;
            settings.MaxStartSize = 10 * scale;

            settings.MinEndSize = 3 * scale;
            settings.MaxEndSize = 12 * scale;

            //settings.BlendState = BlendState.Opaque;
            //settings.BlendState = BlendState.Additive;
            //settings.BlendState = BlendState.AlphaBlend;
            settings.BlendState = BlendState.NonPremultiplied;
        }
    }
}
