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
        public float spawnProbability { get; private set; }
        public float attackCooldown { get; private set; }
        
        public event Action<uint> onHealthChanged;

        public EnemyModel(uint id, EnemyData data)
        {
            this.id = id;
            type = data.type;
            maxHealth = data.health;
            currentHealth = data.health;
            damage = data.damage;
            speed = data.speed;
            attackCooldown = data.attackCooldown;
            lootProbability = data.lootProbability;
            spawnProbability = data.spawnProbability;
        }
        
        public void TakeDamage(float damageTaken)
        {
            currentHealth -= damageTaken;
            onHealthChanged?.Invoke(id);
        }
    }
}