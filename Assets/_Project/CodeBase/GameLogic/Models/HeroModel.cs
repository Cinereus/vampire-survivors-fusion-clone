using System;
using CodeBase.Configs;
using CodeBase.Configs.Heroes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CodeBase.GameLogic.Models
{
    public class HeroModel : IAttackData
    {
        public uint id { get; private set; }
        public HeroType type { get; private set; }
        public float maxHealth { get; private set; }
        public float currentHealth { get; private set; }
        public float speed { get; private set; }
        public float damage { get; private set; }
        public float attackCooldown { get; private set; }
        public float maxXP { get; private set; }
        public float currentXP { get; private set; }
        public int currentLevel { get; private set; }

        public event Action<uint> onXpChanged;
        public event Action<uint> onHealthChanged;

        private readonly float _progressionCoeff;
        private readonly float _statIncreaseCoeff;

        public HeroModel(uint id, HeroData data)
        {
            this.id = id;
            type = data.heroType;
            maxHealth = data.health;
            currentHealth = data.health;
            speed = data.speed;
            damage = data.damage;
            attackCooldown = data.attackCooldown;
            currentXP = data.currentXP;
            maxXP = data.maxXP;
            currentLevel = data.currentLevel;
            _progressionCoeff = data.progressionCoeff;
            _statIncreaseCoeff = data.statIncreaseCoeff;
        }

        public void TakeDamage(float damageTaken)
        {
            currentHealth -= damageTaken;
            onHealthChanged?.Invoke(id);
        }

        public void PickUp(ItemType item, float value)
        {
            switch (item)
            {
                case ItemType.HealthPotion:
                {
                    currentHealth += value;
                    if (currentHealth >= maxHealth)
                        currentHealth = maxHealth;

                    onHealthChanged?.Invoke(id);
                    break;
                }
                case ItemType.XpPage:
                case ItemType.XpBook:
                {
                    currentXP += value;
                    if (currentXP >= maxXP)
                        IncreaseLevel();
                    
                    onXpChanged?.Invoke(id);
                    break;
                }
            }
        }

        private void IncreaseLevel()
        {
            currentXP = 0;
            currentLevel++;
            maxXP *= _statIncreaseCoeff;
            switch (Random.Range(0, 4))
            {
                case 0:
                    speed *= _statIncreaseCoeff;
                    Debug.Log($"{type} Level up! New level is: {currentLevel} Speed increased to {speed}");
                    break;
                case 1:
                    damage *= _statIncreaseCoeff;
                    Debug.Log($"{type} Level up! New level is: {currentLevel} Damage increased to {damage}");
                    break;
                case 2:
                    attackCooldown /= _statIncreaseCoeff;
                    Debug.Log($"{type} Level up! New level is: {currentLevel} Attack cooldown reduced to {attackCooldown}");
                    break;
                case 3:
                    maxHealth *= _statIncreaseCoeff;
                    Debug.Log($"{type} Level up! New level is: {currentLevel} Max health increased to {maxHealth}");
                    break;
            }
            currentHealth = maxHealth;
            onHealthChanged?.Invoke(id);
            onXpChanged?.Invoke(id);
        }
    }
}