#region File Description
//-----------------------------------------------------------------------------
// SmokePlumeParticleSystem.cs
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
    /// Custom particle system for creating a giant plume of long lasting smoke.
    /// </summary>
    class SmokePlumeParticleSystem : ParticleSystem
    {
        public SmokePlumeParticleSystem(Game game, ContentManager content)
            : base(game, content)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = "Particles\\smoke";

            settings.MaxParticles = 500; //600

            settings.Duration = TimeSpan.FromSeconds(3);

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 1;

            settings.MinVerticalVelocity = -1;
            settings.MaxVerticalVelocity = -1;

            // Create a wind effect by tilting the gravity vector sideways.
            settings.Gravity = new Vector3(-2, 5, 0);

            settings.EndVelocity = 0.75f;

            settings.MinRotateSpeed = -1;
            settings.MaxRotateSpeed = 1;

            settings.MinStartSize = 5;
            settings.MaxStartSize = 10;

            settings.MinColor = Color.Black;
            settings.MaxColor = Color.LightGray;

            //settings.MinColor = new Color(64, 96, 128, 18);
            //settings.MaxColor = new Color(45, 45, 45, 1);

            settings.MinEndSize = 20;
            settings.MaxEndSize = 30;

            settings.BlendState = BlendState.NonPremultiplied;            
        }
    }
}
