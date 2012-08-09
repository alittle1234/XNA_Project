using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TerrainGame4_0
{
    /*  Sturcture for holding the corners of an event triger box
     *  and the list of events that it will trigger
     */

    public class EventFlag
    {
        public enum FlagStatus
        {
            Waiting, Tripped, Done
        }

        public FlagStatus Status;
        public Vector2[] Corners = new Vector2[4];

        public GameEvent[] Event_Array;
        public List<GameEvent> Events = new List<GameEvent>();
        public List<Condition> Conditions = new List<Condition>();        
        public int Delay = 0;
        public String Label = "Not_Labeled";

        public EventFlag()
        {
            Event_Array = new GameEvent[0];
        }

        public EventFlag(Vector2[] corners)
        {
            Corners = corners;
        }

        public EventFlag(Vector2 cornerTL, Vector2 cornerTR,
            Vector2 cornerBL, Vector2 cornerBR)
        {
            Corners[0] = cornerTL;
            Corners[1] = cornerTR;
            Corners[2] = cornerBL;
            Corners[3] = cornerBR;
        }

    }
}
