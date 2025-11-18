using System;
using Fusion;

namespace CodeBase.Configs.Heroes
{
    [Serializable]
    public struct HeroData : INetworkStruct
    {
        public HeroType heroType;
        public float maxHealth;
        public float currentHealth;
        public float speed;
        public float damage;
        public float attackCooldown;
        public float maxXp;
        public float currentXp;
        public int currentLevel;
        public float progressionCoeff;
        public float statIncreaseCoeff;
    }
}