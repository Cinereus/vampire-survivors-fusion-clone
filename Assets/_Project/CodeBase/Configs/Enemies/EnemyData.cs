using System;

namespace CodeBase.Configs.Enemies
{
    [Serializable]
    public struct EnemyData
    {
        public EnemyType type;
        public float health;
        public float damage;
        public float attackCooldown;
        public float speed;
        public float lootProbability;
        public float spawnProbability;
    }
}