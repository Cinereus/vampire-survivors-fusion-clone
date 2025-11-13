using CodeBase.GameLogic.Models;
using CodeBase.GameLogic.Services;
using CodeBase.Infrastructure;
using Fusion;

namespace CodeBase.GameLogic.Components.Enemy
{
    public class LootSpawner : NetworkBehaviour
    {
        private LootSpawnService _lootService;
        private EnemiesModel _enemies;
        
        public override void Spawned()
        {
            _lootService = BehaviourInjector.instance.Resolve<LootSpawnService>();
            _enemies = BehaviourInjector.instance.Resolve<EnemiesModel>();
            
            if (HasStateAuthority) 
                _enemies.onHealthChanged += OnDeath;
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (HasStateAuthority) 
                _enemies.onHealthChanged -= OnDeath;
        }
        
        private void OnDeath(uint id, float _)
        {
            if (Object.Id.Raw == id && _enemies.TryGetBy(id, out var model) && model.currentHealth <= 0)
            {
                _lootService.SpawnXp(model.type, transform.position);
                _lootService.SpawnHealPotion(model.lootProbability, transform.position);
            }
        }
    }
}