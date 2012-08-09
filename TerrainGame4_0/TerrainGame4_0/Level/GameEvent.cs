using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TerrainGame4_0
{
    public class GameEvent 
    {
        public enum GameEventType
        {
            PUT_PLAYER,
            PUT_ENEMY,
            SET_CONDITION,
            TRIGGER_FLAG,
            PLAY_ANIMATION,
            PLAY_SOUND,
            SET_OBJECTIVE,
            END_LEVEL,
            END_GAME
        }

        public GameEventType EventType;

        public Spawn SpawnPoint = new Spawn();
       

        public GameEvent()
        {
        }

        public override string ToString()
        {
            return EventType.ToString();
        }

    }
}
