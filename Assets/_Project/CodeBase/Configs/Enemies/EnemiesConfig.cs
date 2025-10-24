using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Configs.Enemies
{
    [CreateAssetMenu(fileName = "EnemiesConfig", menuName = "Configs/EnemiesConfig")]
    public class EnemiesConfig : ScriptableObject
    {
        public List<EnemyData> enemies;
    }
}