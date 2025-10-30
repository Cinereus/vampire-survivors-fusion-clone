using CodeBase.GameLogic.Models;
using CodeBase.GameLogic.Services;
using CodeBase.Infrastructure.Services;
using Fusion;

namespace CodeBase.GameLogic.Components.Enemy
{
    public class LootSpawner : NetworkBehaviour
    {
        private EnemyModel _model;
        private LootSpawnService _lootService;
        private NetworkContainer _network;

        public void Setup(EnemyModel model)
        {
            _model = model;
        }

        public override void Spawned()
        {
            _lootService = ServiceLocator.instance.Get<LootSpawnService>();
            
            if (HasStateAuthority)
            { 
                _model.onHealthChanged += OnDeath;   
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (HasStateAuthority)
            { 
                _model.onHealthChanged -= OnDeath;   
            }
        }
        
        private void OnDeath(uint obj)
        {
            if (_model.currentHealth <= 0)
            {
                _lootService.SpawnXp(_model.type, transform.position);
                _lootService.SpawnHealPotion(_model.lootProbability, transform.position);    
            }
        }
    }
}