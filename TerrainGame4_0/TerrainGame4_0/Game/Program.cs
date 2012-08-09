using System;
using System.Xml;

namespace TerrainGame4_0
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            //using (Game1 game = new Game1())
            //{
            //    game.Run();
            //}

            using (ParentGame game = new ParentGame())
            {
                game.Run();
            }
        }
    }
}

