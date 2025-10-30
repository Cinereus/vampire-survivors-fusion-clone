using CodeBase.Configs;
using CodeBase.Configs.Enemies;
using CodeBase.Infrastructure.Services;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CodeBase.GameLogic.Services
{
    public class LootSpawnService : IService
    {
        private readonly GameFactory _factory;
        private readonly NetworkContainer _network;

        public LootSpawnService(GameFactory factory)
        {
            _factory = factory;
        }
        
        public void SpawnXp(EnemyType type, Vector3 position)
        {
            var itWasPowerfulEnemy = type is EnemyType.Troll or EnemyType.DeadKnight; 
            _factory.CreateItem(itWasPowerfulEnemy ? ItemType.XpBook : ItemType.XpPage, position);   
        }

        public void SpawnHealPotion(float spawnProbability, Vector3 position)
        {
            const int FULL_PROBABILITY = 100; 
            if (FULL_PROBABILITY * Random.value < spawnProbability) 
                _factory.CreateItem(ItemType.HealthPotion, position);
        }

        public void Dispose()
        {
        }
    }
}