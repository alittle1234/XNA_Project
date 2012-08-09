using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TerrainGame4_0
{
    public class EnemySpawn : Spawn
    {
        public Dictionary<String, String> EnemyStats = new Dictionary<string, string>();

        public EnemySpawn()
            : base()
        {
            
        }

        public EnemySpawn(Vector3 position)
            : base(position)
        {
            EnemyStats.Add("Type", "default_enemy_model");
            EnemyStats.Add("Scale", "default_enemy_scale");
        }

    }
}
