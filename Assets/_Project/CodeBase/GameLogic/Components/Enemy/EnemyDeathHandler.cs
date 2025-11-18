using CodeBase.GameLogic.Models;
using CodeBase.Infrastructure;
using Fusion;


namespace CodeBase.GameLogic.Components.Enemy
{
    public class EnemyDeathHandler : NetworkBehaviour
    {
        private EnemyModel _model;
        
        public override void Spawned()
        {
            if (HasStateAuthority)
            {
                _model = BehaviourInjector.instance.Resolve<Enemies>().GetBy(Object.Id.Raw);
                _model.onHealthChanged += OnHealthChanged;
            } 
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (HasStateAuthority) 
                _model.onHealthChanged -= OnHealthChanged;
        }

        private void OnHealthChanged()
        {
            if (_model.currentHealth <= 0)
                Runner.Despawn(Object);
        }
    }
}