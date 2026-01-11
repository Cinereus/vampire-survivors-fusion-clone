using System;
using System.Collections.Generic;

namespace CodeBase.Configs.Enemies
{
    [Serializable]
    public class EnemiesConfig : IConfig
    {
        public List<EnemyData> enemies;
    }
}