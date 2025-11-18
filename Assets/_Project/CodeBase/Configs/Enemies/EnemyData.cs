using System;
using Fusion;

namespace CodeBase.Configs.Enemies
{
    [Serializable]
    public struct EnemyData : INetworkStruct
    {
        public EnemyType enemyType;
        public float maxHealth;
        public float currentHealth;
        public float damage;
        public float attackCooldown;
        public float speed;
        public float lootProbability;
        public float spawnProbability;
    }
}