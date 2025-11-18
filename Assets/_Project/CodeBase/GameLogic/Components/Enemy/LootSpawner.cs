using CodeBase.GameLogic.Models;
using CodeBase.GameLogic.Services;
using CodeBase.Infrastructure;
using Fusion;

namespace CodeBase.GameLogic.Components.Enemy
{
    public class LootSpawner : NetworkBehaviour
    {
        private LootSpawnService _lootService;
        private EnemyModel _model;
        
        public override void Spawned()
        {
            if (HasStateAuthority)
            {
                SetupDependencies();
                _model.onHealthChanged += OnHeathChanged;
            }
        }
        
        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (HasStateAuthority) 
                _model.onHealthChanged -= OnHeathChanged;
        }
        
        private void SetupDependencies()
        {
            _lootService = BehaviourInjector.instance.Resolve<LootSpawnService>();
            _model = BehaviourInjector.instance.Resolve<Enemies>().GetBy(Object.Id.Raw);
        }
        
        private void OnHeathChanged()
        {
            if (_model.currentHealth <= 0)
            {
                _lootService.SpawnXp(_model.type, transform.position);
                _lootService.SpawnHealPotion(_model.lootProbability, transform.position);
            }
        }
    }
}