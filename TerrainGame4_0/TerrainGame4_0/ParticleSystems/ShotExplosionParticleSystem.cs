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
    class ShotExplosionParticleSystem : ParticleSystem
    {
        public ShotExplosionParticleSystem(Game game, ContentManager content)
            : base(game, content)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = "Particles\\explosion3";

            settings.MaxParticles = 100;

            settings.Duration = TimeSpan.FromSeconds(0.2);
            settings.DurationRandomness = 1;

            settings.MinHorizontalVelocity = 20;
            settings.MaxHorizontalVelocity = 30;// 30;

            settings.MinVerticalVelocity = -20;
            settings.MaxVerticalVelocity = 20;// 20;

            settings.EndVelocity = 0;

            settings.MinColor = Color.White;
            settings.MaxColor = Color.Black;

            settings.MinRotateSpeed = -1;
            settings.MaxRotateSpeed = 1;

            settings.MinStartSize = 2;
            settings.MaxStartSize = 5;

            settings.MinEndSize = 9;
            settings.MaxEndSize = 15;

            settings.BlendState = BlendState.Additive;
        }
    }
}
