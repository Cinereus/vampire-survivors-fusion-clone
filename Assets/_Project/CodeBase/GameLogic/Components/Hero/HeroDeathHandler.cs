using CodeBase.GameLogic.Models;
using CodeBase.Infrastructure.Services;
using Fusion;

namespace CodeBase.GameLogic.Components.Hero
{
    public class HeroDeathHandler : NetworkBehaviour
    {
        private HeroesModel _heroes;

        public override void Spawned()
        {
            _heroes = ServiceLocator.instance.Get<HeroesModel>();
            
            if (HasStateAuthority)
            {
                _heroes.onHealthChanged += OnHealthChanged;
            }
        }
        
        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (HasStateAuthority)
            { 
                _heroes.onHealthChanged -= OnHealthChanged;
            }
        }
        
        private void OnHealthChanged(uint id)
        {
            if (Object.Id.Raw == id && _heroes.TryGetBy(Object.Id.Raw, out var model) && model.currentHealth <= 0)
                Runner.Shutdown();
        }
    }
}