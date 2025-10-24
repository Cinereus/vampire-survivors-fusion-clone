using System;

namespace CodeBase.Configs.Heroes
{
    [Serializable]
    public struct HeroData
    {
        public HeroType heroType;
        public float health;
        public float speed;
        public float damage;
        public float attackCooldown;
        public float maxXP;
        public float currentXP;
        public int currentLevel;
        public float progressionCoeff;
        public float statIncreaseCoeff;
    }
}