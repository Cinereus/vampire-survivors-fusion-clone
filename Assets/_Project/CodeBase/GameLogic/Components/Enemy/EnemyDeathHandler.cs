using CodeBase.GameLogic.Models;
using Fusion;

namespace CodeBase.GameLogic.Components
{
    public class EnemyDeathHandler : NetworkBehaviour
    {
        private EnemyModel _model;

        public void Setup(EnemyModel model)
        {
            _model = model;
        }

        public override void Spawned()
        {
            if (HasStateAuthority) 
                _model.onHealthChanged += OnHealthChanged;
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (HasStateAuthority) 
                _model.onHealthChanged -= OnHealthChanged;
        }

        private void OnHealthChanged(uint _)
        {
            if (HasStateAuthority && _model.currentHealth <= 0)
                Runner.Despawn(Object);
        }
    }
}