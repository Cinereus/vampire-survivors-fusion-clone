using CodeBase.GameLogic.Models;
using CodeBase.Infrastructure;
using Fusion;


namespace CodeBase.GameLogic.Components.Enemy
{
    public class EnemyDeathHandler : NetworkBehaviour
    {
        private EnemiesModel _enemies;
        
        public override void Spawned()
        {
            _enemies = BehaviourInjector.instance.Resolve<EnemiesModel>();
            
            if (HasStateAuthority) 
                _enemies.onHealthChanged += OnHealthChanged;
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (HasStateAuthority) 
                _enemies.onHealthChanged -= OnHealthChanged;
        }

        private void OnHealthChanged(uint id, float currentHealth)
        {
            if (Object.Id.Raw == id && currentHealth <= 0)
                Runner.Despawn(Object);
        }

      
    }
}