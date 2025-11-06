using System;
using CodeBase.Configs.Enemies;

namespace CodeBase.GameLogic.Models
{
    public class EnemyModel
    {
        public uint id { get; private set; }
        public EnemyType type { get; private set; }
        public float maxHealth { get; private set; }
        public float currentHealth { get; private set; }
        public float damage { get; private set; }
        public float speed { get; private set; }
        public float lootProbability { get; private set; }
        public float attackCooldown { get; private set; }
        
        public event Action<uint> onHealthChanged;

        public EnemyModel(EnemyData data)
        { 
            Setup(data);
        }
        
        public void TakeDamage(float damageTaken)
        {
            currentHealth -= damageTaken;
            onHealthChanged?.Invoke(id);
        }

        public void Setup(EnemyData data)
        {
            id = data.id;
            type = data.type;
            maxHealth = data.maxHealth;
            currentHealth = data.currentHealth;
            damage = data.damage;
            speed = data.speed;
            attackCooldown = data.attackCooldown;
            lootProbability = data.lootProbability;
        }

        public EnemyData ToData()
        { 
            return new EnemyData 
            {
                id = id,
                type = type,
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