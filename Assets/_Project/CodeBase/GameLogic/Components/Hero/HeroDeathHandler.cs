using CodeBase.GameLogic.Models;
using CodeBase.Infrastructure;
using Fusion;

namespace CodeBase.GameLogic.Components.Hero
{
    public class HeroDeathHandler : NetworkBehaviour
    {
        private HeroesModel _heroes;
        
        public override void Spawned()
        {
            _heroes = BehaviourInjector.instance.Resolve<HeroesModel>();
            
            if (HasStateAuthority) 
                _heroes.onHealthChanged += OnHealthChanged;
        }
        
        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (HasStateAuthority) 
                _heroes.onHealthChanged -= OnHealthChanged;
        }
        
        private void OnHealthChanged(uint id, float currentHealth)
        {
            if (Object.Id.Raw == id && currentHealth <= 0)
                Runner.Shutdown();
        }
    }
}