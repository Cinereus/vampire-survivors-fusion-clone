using CodeBase.GameLogic.Models;
using CodeBase.Infrastructure;
using Fusion;

namespace CodeBase.GameLogic.Components.Hero
{
    public class HeroDeathHandler : NetworkBehaviour
    {
        private HeroModel _model;
        
        public override void Spawned()
        {
            if (HasStateAuthority)
            {
                _model = BehaviourInjector.instance.Resolve<Heroes>().GetBy(Object.Id.Raw);
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
            if (_model.currentHealth > 0) 
                return;
            
            if (Runner.LocalPlayer == Object.InputAuthority)
                Runner.Shutdown();
            else
                Runner.Disconnect(Object.InputAuthority);
        }
    }
}