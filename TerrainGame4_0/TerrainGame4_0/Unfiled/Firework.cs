using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerrainGame4_0
{
    class Firework
    {
        public int CurFrame;
        public int MaxFrame;
        public float TimeForFrame;
        public float TimeLastChange;
        public Microsoft.Xna.Framework.Vector2 Position;
        public Microsoft.Xna.Framework.Color Color;

        public Firework(int Frames, float Time, Microsoft.Xna.Framework.Vector2 Position, Microsoft.Xna.Framework.Color Color)
        {
            CurFrame = 0;
            MaxFrame = Frames;
            TimeForFrame = Time;
            TimeLastChange = 0;
            this.Position = Position;
            this.Color = Color;
        }

    }
}
