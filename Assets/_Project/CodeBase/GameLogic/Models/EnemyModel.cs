using System;
using CodeBase.Configs.Enemies;

namespace CodeBase.GameLogic.Models
{
    public class EnemyModel : EntityModel<EnemyData>
    {
        public EnemyType type { get; private set; }
        public float maxHealth { get; private set; }
        public float currentHealth { get; private set; }
        public float damage { get; private set; }
        public float speed { get; private set; }
        public float lootProbability { get; private set; }
        public float attackCooldown { get; private set; }
        
        public event Action onHealthChanged;
        
        public override void Setup(EnemyData data)
        {
            type = data.enemyType;
            maxHealth = data.maxHealth;
            currentHealth = data.currentHealth;
            damage = data.damage;
            speed = data.speed;
            attackCooldown = data.attackCooldown;
            lootProbability = data.lootProbability;
        }

        public override void Dispose() {}
        
        public void TakeDamage(float damageTaken)
        {
            currentHealth -= damageTaken;
            onHealthChanged?.Invoke();
        }

        public EnemyData ToData()
        { 
            return new EnemyData 
            {
                enemyType = type,
                maxHealth = maxHealth,
                currentHealth = currentHealth,
                damage = damage,
                speed = speed,
                attackCooldown = attackCooldown,
                lootProbability = lootProbability,
            };
        }
    }
}