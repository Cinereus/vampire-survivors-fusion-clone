using System;
using CodeBase.Configs;
using CodeBase.Configs.Heroes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CodeBase.GameLogic.Models
{
    public class HeroModel : EntityModel<HeroData>
    {
        public HeroType type { get; set; }
        public float maxHealth { get; set; }
        public float currentHealth { get; set; }
        public float speed { get; set; }
        public float damage { get; set; }
        public float attackCooldown { get; set; }
        public float maxXp { get; set; }
        public float currentXp { get; set; }
        public int currentLevel { get; set; }

         public event Action onXpChanged;
         public event Action onLevelIncreased;
         public event Action onHealthChanged;

        private float _progressionCoeff;
        private float _statIncreaseCoeff;

        public override void Setup(HeroData data)
        {
            var oldCurrentXp = currentXp;
            var oldCurrentLevel = currentLevel;
            var oldCurrentHealth = currentHealth;
            
            type = data.heroType;
            maxHealth = data.maxHealth;
            currentHealth = data.currentHealth;
            speed = data.speed;
            damage = data.damage;
            attackCooldown = data.attackCooldown;
            currentXp = data.currentXp;
            maxXp = data.maxXp;
            currentLevel = data.currentLevel;
            _progressionCoeff = data.progressionCoeff;
            _statIncreaseCoeff = data.statIncreaseCoeff;
            
            if (currentLevel != oldCurrentLevel)
                onLevelIncreased?.Invoke();
            
            if (Math.Abs(currentXp - oldCurrentXp) > 0.001f)
                onHealthChanged?.Invoke();
            
            if (Math.Abs(currentHealth - oldCurrentHealth) > 0.001f)
                onXpChanged?.Invoke();
        }

        public void TakeDamage(float damageTaken)
        {
            currentHealth -= damageTaken;
            onHealthChanged?.Invoke();
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

                    onHealthChanged?.Invoke();
                    break;
                }
                case ItemType.XpPage:
                case ItemType.XpBook:
                {
                    currentXp += value;
                    if (currentXp >= maxXp)
                        IncreaseLevel();
                    
                    onXpChanged?.Invoke();
                    break;
                }
            }
        }

        private void IncreaseLevel()
        {
            currentXp = 0;
            currentLevel++;
            maxXp *= _progressionCoeff;
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
            onHealthChanged?.Invoke();
            onXpChanged?.Invoke();
            onLevelIncreased?.Invoke();
        }
        
        public HeroData ToData()
        {
            return new HeroData
            {
                heroType = type,
                currentHealth = currentHealth,
                maxHealth = maxHealth,
                speed = speed,
                damage = damage,
                attackCooldown = attackCooldown,
                currentXp = currentXp,
                maxXp = maxXp,
                currentLevel = currentLevel,
                progressionCoeff = _progressionCoeff,
                statIncreaseCoeff = _statIncreaseCoeff,
            };
        }
    }
}