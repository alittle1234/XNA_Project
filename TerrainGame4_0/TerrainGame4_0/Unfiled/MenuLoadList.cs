using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerrainGame4_0
{
    /// <summary>
    /// Automaticlay generated load list for Loading Stage.
    /// Set to 'null' after Loaded Resources are assigned to Menu Resources.
    /// </summary>
    public class MenuLoadList
    {

        public Dictionary<string, string> Textures  = new Dictionary<string,string> ();
        public Dictionary<string, string> Models = new Dictionary<string, string>();

        public MenuLoadList()
        {
            Textures.Add("BackgroundTime", "Menu\\background02");
            Textures.Add("BackgroundScreen", "Menu\\background01");
            Textures.Add("MenuSelect", "Menu\\menu_select");
            Models.Add("MenuModel", "Menu\\model");
        }

    }
}
